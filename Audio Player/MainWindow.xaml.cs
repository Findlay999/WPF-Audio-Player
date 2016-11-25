using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.IO;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using TagLib;
using Microsoft.Win32;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace Audio_Player
{
    public partial class MainWindow : Window
    {
        public List<string> Paths = new List<string>();
        public int CurrentIndex = 0;
        public bool Playing = false;
        private LoadAudio load_audios = new LoadAudio();
        public PlayList mainPL = new PlayList() { Name = "Main Playlist" };
        public List<PlayList> playLists { get; set; } = new List<PlayList>();
        public List<Audio> CurrentList;
        private TimeSpan TotalTime;

        public MainWindow()
        {
            InitializeComponent();
            DeserializeData();
            CurrentList = mainPL.AudioList;
        }

        private void MyMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            try
            {
                TotalTime = ms.NaturalDuration.TimeSpan;
                DispatcherTimer timerVideoTime = new DispatcherTimer();
                timerVideoTime.Interval = TimeSpan.FromSeconds(1);
                timerVideoTime.Tick += new EventHandler(timer_Tick);
                timerVideoTime.Start();
            }
            catch(Exception d)
            {
                MessageBox.Show(d.Message);
            }
        }

        private void ms_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (RepeatButt.IsChecked == true)
            {
                ms.Position = TimeSpan.Zero;
                ms.Play();
            }
            else
            {
                if (CurrentIndex < CurrentList.Count - 1)
                    ChangeAudio(CurrentList[++CurrentIndex]);
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (ms.NaturalDuration.HasTimeSpan && ms.NaturalDuration.TimeSpan.TotalSeconds > 0)
            {
                if (TotalTime.TotalSeconds > 0)
                {
                    AudioSlider.Value = 10 / TotalTime.TotalSeconds * ms.Position.TotalSeconds;
                }
            }
        }

        private void timeSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (TotalTime.TotalSeconds > 0)
            {
                ms.Position = TimeSpan.FromSeconds(TotalTime.TotalSeconds * AudioSlider.Value / 10);
            }
        }

        private void Audio_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var audioContext = ((sender as Grid).DataContext as Audio); // получаем данные нажатого grid-а
            ChangeAudio(audioContext);// воспроизводим
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(this.ActualWidth < 850)
            {
                TitleSizer.Visibility = Visibility.Hidden;
                AudioSlider.Visibility = Visibility.Hidden;
            }
            else
            {
                TitleSizer.Visibility = Visibility.Visible;
                AudioSlider.Visibility = Visibility.Visible;
                BotButtons.Children[3].Visibility = Visibility.Visible;
            }

            if(this.ActualWidth < 600)
            {
                BotButtons.Children[5].Visibility = Visibility.Collapsed;
                BotButtons.Children[4].Visibility = Visibility.Collapsed;
            }
            else
            {
                BotButtons.Children[5].Visibility = Visibility.Visible;
                BotButtons.Children[4].Visibility = Visibility.Visible;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (MessageBox.Show("Сохранить данные?", "Сохранение...", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                SerializeData();
            }
        }

        public BitmapImage LoadImage(string text, bool decode)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(text, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            if (decode)
            {
                bitmap.DecodePixelWidth = 200;
                bitmap.DecodePixelHeight = 200;
            }
            bitmap.EndInit();

            return bitmap;
        }

        private void ChangeAudio(Audio audioContext)
        {
            ms.Source = new Uri(audioContext.DirectoryName + "\\" + audioContext.Name);
            BottomPanel.Visibility = Visibility.Visible;
            TopPlayGrid.DataContext = audioContext; // обновляем верхнюю панель

            Playing = true;
            ms.Play();

            CurrentIndex = CurrentList.IndexOf(audioContext);

            (PlayButton.Child as Image).Source = LoadImage(@"pack://application:,,,/Resources/Pause.png", false);
            
            if(!System.IO.File.Exists(audioContext.DirectoryName + "\\" + audioContext.Name))  // проверка наличия файла перед воспроизведением
            {
                // если такой файл не существует - создается диалоговое окно 
                if(MessageBox.Show("Этот файл был удален или перемещен...\nУдалить все данные о нем?", "Ошибка", MessageBoxButton.YesNo) == MessageBoxResult.Yes) 
                {
                    playLists = playLists.Select(x => { x.AudioList = x.AudioList.Where
                        (s => s.Name != audioContext.Name || s.DirectoryName != audioContext.DirectoryName).ToList(); x.GetTime(); return x; }).ToList(); // с списка плейлистов, создаем новый список, в котором не будет такого файла
                    mainPL.AudioList = mainPL.AudioList.Where(x => x.Name != audioContext.Name || x.DirectoryName != audioContext.DirectoryName).ToList(); // то же делаем и для главного плейлиста
                    CurrentList = CurrentList.Where(x => x.Name != audioContext.Name || x.DirectoryName != audioContext.DirectoryName).ToList(); // то же и для текущего списка воспроизведения
                    Play.ItemsSource = CurrentList; // обновляем главний список воспроизведения
                    if (CurrentList.Count > 0)
                    {
                        ChangeAudio(CurrentList[--CurrentIndex]); // воспроизводим следующую песню
                    }
                    return;
                }
                else
                {
                    ChangeAudio(CurrentList[++CurrentIndex]); // воспроизводим следующую песню
                }
            }

            TagLib.File Ds = TagLib.File.Create(audioContext.DirectoryName + "\\" + audioContext.Name);
            if (Ds.Tag.Pictures.Length > 0)
            {
                TagLib.IPicture pic = Ds.Tag.Pictures[0];
                MemoryStream s = new MemoryStream(pic.Data.Data);
                s.Seek(0, SeekOrigin.Begin);

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = s;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.DecodePixelWidth = 200;
                bitmap.DecodePixelHeight = 200;
                bitmap.EndInit();

                s.Close();

                Img_Audio.Source = bitmap;              
                GC.Collect();
            }
            else
            {
                Img_Audio.Source = LoadImage(@"pack://application:,,,/Resources/NoImg.jpg", true);
            }
            BottomInfo.DataContext = audioContext;
        }

        private void PL_Click(object sender, MouseButtonEventArgs e)
        {
            SetVisiblePlayListInfo();
            PListInfo.DataContext = null;
            PListInfo.DataContext = (sender as Border).DataContext;
        }
    }
}

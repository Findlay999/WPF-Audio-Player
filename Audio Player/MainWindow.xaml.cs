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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public List<string> Paths = new List<string>();
        public int CurrentIndex = -1;
        public bool Playing = false;
        public PlayList playList = new PlayList();

        private TimeSpan TotalTime;

        public MainWindow()
        {
            InitializeComponent();
            DeserializeData();

            PL_ListBox.ItemsSource = new List<double>() { 3, 4, 5, 3, 1, 2 ,3 ,4 ,5 };
        }

        private void MyMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            TotalTime = ms.NaturalDuration.TimeSpan;
            DispatcherTimer timerVideoTime = new DispatcherTimer();
            timerVideoTime.Interval = TimeSpan.FromSeconds(1);
            timerVideoTime.Tick += new EventHandler(timer_Tick);
            timerVideoTime.Start();
        }


        private void ms_MediaEnded(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            if (RandomButt.IsChecked == true)
            {
                int[] Mas = Enumerable.Range(0, playList.AudioPathList.Count).Where(x => !playList.AudioPathList[x].IsPlayed).ToArray();
                if (Mas.Length == 0)
                {
                    playList.AudioPathList.Select(x => x.IsPlayed = false);
                    ChangeAudio(playList.AudioPathList[Mas[rand.Next(0, Mas.Length)]]);
                }
            }
            else if (RepeatButt.IsChecked == true)
            {
                ms.Position = TimeSpan.Zero;
                ms.Play();
            }
            else
            {
                if (CurrentIndex < playList.AudioPathList.Count - 1)
                    ChangeAudio(playList.AudioPathList[++CurrentIndex]);
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

        private void RemovePath_Click(object sender, RoutedEventArgs e)
        {
            string r = ((Button)sender).Content.ToString();
            playList.AudioPathList = playList.AudioPathList.Where(x => x.DirectoryName != r).ToList();

            Play.ItemsSource = new List<Audio>(playList.AudioPathList);

            Paths.Remove(((Button)sender).Content.ToString());
            ListPaths.ItemsSource = new List<string>(Paths);
        }

        private void AddPath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog Dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (Dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!Paths.Contains(Dialog.SelectedPath))
                {
                    Paths.Add(Dialog.SelectedPath);
                    ListPaths.ItemsSource = new List<string>(Paths);
                    playList.GetMusic(Dialog.SelectedPath);
                    Play.ItemsSource = new List<Audio>(playList.AudioPathList);
                }
                else
                {
                    MessageBox.Show("Такой путь уже существует");
                }
            }
        }

        private void Audio_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var audioContext = ((sender as Grid).DataContext as Audio);
            ChangeAudio(audioContext);
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


        private void ShowLists_Click(object sender, RoutedEventArgs e)
        {
            PListControl.Visibility = Visibility.Visible;
            AddFolder.Visibility = Visibility.Collapsed;
            PlayGrid.Visibility = Visibility.Collapsed;
            OpacityAnim(PListControl, sender);
        }

        private void PlayList_Click(object sender, RoutedEventArgs e)
        {
            PListControl.Visibility = Visibility.Collapsed;
            AddFolder.Visibility = Visibility.Collapsed;
            PlayGrid.Visibility = Visibility.Visible;
            OpacityAnim(PlayGrid, sender);
        }

        private void AddToFolder_Click(object sender, RoutedEventArgs e)
        {
            PListControl.Visibility = Visibility.Collapsed;
            AddFolder.Visibility = Visibility.Visible;
            PlayGrid.Visibility = Visibility.Collapsed;
            RideAnim(AddFolder, sender);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream("P_S.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, Paths);
            }
            using (FileStream fs = new FileStream("P_List.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, playList.AudioPathList);
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

        private void DeserializeData()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream("P_S.dat", FileMode.OpenOrCreate))
            {
                if (fs.Length != 0)
                    Paths = (List<string>)formatter.Deserialize(fs);
            }
            ListPaths.ItemsSource = new List<string>(Paths);

            using (FileStream fs = new FileStream("P_List.dat", FileMode.OpenOrCreate))
            {
                if (fs.Length != 0)
                    playList.AudioPathList = (List<Audio>)formatter.Deserialize(fs);
            }

            ListPaths.ItemsSource = new List<string>(Paths);
            Play.ItemsSource = new List<Audio>(playList.AudioPathList);
        }

        private void ChangeAudio(Audio audioContext)
        {
            audioContext.IsPlayed = true;
            ms.Source = new Uri(audioContext.DirectoryName + "\\" + audioContext.Name);
            BottomPanel.Visibility = Visibility.Visible;
            TopPlayGrid.DataContext = audioContext;

            BottomPanel.Visibility = Visibility.Visible;

            Playing = true;
            ms.Play();
            CurrentIndex = playList.AudioPathList.IndexOf(audioContext);

            (PlayButton.Content as Image).Source = LoadImage(@"C:\Users\Євгеній\Documents\Pause.png", false);

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
                // Create a System.Windows.Controls.Image control
                Img_Audio.Source = bitmap;
               
                GC.Collect();
            }
            else
            {
                Img_Audio.Source = LoadImage(@"pack://application:,,,/Resources/NoImg.jpg", true);
            }
            BottomInfo.DataContext = audioContext;
        }


        private void OpacityAnim(Grid elem, object sender)
        {
            DoubleAnimation Anim = new DoubleAnimation();
            (sender as Button).IsHitTestVisible = false;
            Anim.Completed += delegate
            {
                (sender as Button).IsHitTestVisible = true;
            };
            Anim.From = 0;
            Anim.To = 1;
            Anim.Duration = new Duration(TimeSpan.FromSeconds(1));
            elem.BeginAnimation(OpacityProperty, Anim);
        }

        private void RideAnim(Grid elem, object sender)
        {
            ThicknessAnimation Anim = new ThicknessAnimation();
            (sender as Button).IsHitTestVisible = false; 
            Anim.Completed += delegate
            {
                (sender as Button).IsHitTestVisible = true;
            };
            Anim.From = new Thickness(-400, 200, 0, 0);
            Anim.To = elem.Margin;
            Anim.Duration = new Duration(TimeSpan.FromSeconds(1));
            elem.BeginAnimation(MarginProperty, Anim);
        }
    }
}

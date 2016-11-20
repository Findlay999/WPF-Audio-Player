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
        public int CurrentIndex = 0;
        public bool Playing = false;

        private LoadAudio load_audios = new LoadAudio();

        public PlayList mainPL = new PlayList();

        public List<PlayList> playLists { get; set; } = new List<PlayList>();

        public List<Audio> CurrentList;

        private TimeSpan TotalTime;

        public MainWindow()
        {
            InitializeComponent();

            mainPL.Name = "Main Playlist";

            DeserializeData();
            CurrentList = mainPL.AudioList;
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

        private void RemovePath_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить этот путь?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (mainPL.AudioList.SequenceEqual(CurrentList))
                {
                    string r = ((Button)sender).Content.ToString();
                    mainPL.AudioList = mainPL.AudioList.Where(x => x.DirectoryName != r).ToList();
                    Paths.Remove(((Button)sender).Content.ToString());
                    ListPaths.ItemsSource = new List<string>(Paths);
                    Play.ItemsSource = mainPL.AudioList;
                    CurrentList = mainPL.AudioList;
                    CurrentIndex = 0;
                }
                else
                {
                    string r = ((Button)sender).Content.ToString();
                    mainPL.AudioList = mainPL.AudioList.Where(x => x.DirectoryName != r).ToList();
                    Paths.Remove(((Button)sender).Content.ToString());
                    ListPaths.ItemsSource = new List<string>(Paths);
                }
                mainPL.GetTime();
            }
        }

        private void AddPath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog Dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (Dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!Paths.Contains(Dialog.SelectedPath)) //если такого пути нет в списку
                {
                    Paths.Add(Dialog.SelectedPath); // добавляем путь
                    ListPaths.ItemsSource = new List<string>(Paths); // отображаем список путей
                    if (mainPL.AudioList.SequenceEqual(CurrentList)) // проверяем какой плейлист отображается, если главный...
                    {
                        mainPL.AudioList.AddRange(load_audios.GetMusic(Dialog.SelectedPath)); // обновляем плейлист
                        Play.ItemsSource = new List<Audio>(mainPL.AudioList); // обновляем текущий список воспроизведения
                        CurrentList = mainPL.AudioList; // обновляем данные текущего списка воспроизведения
                        CurrentIndex = 0; //после окончания песни воспроизведение начнется сначала списка.
                    }
                    else
                    {
                       mainPL.AudioList.AddRange(load_audios.GetMusic(Dialog.SelectedPath));  // если плейлист пользовательский - обновляем только главний плейлист и все
                    }
                    mainPL.GetTime();
                }
                else
                {
                    MessageBox.Show("Такой путь уже существует");
                }
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


        private void ShowLists_Click(object sender, RoutedEventArgs e)
        {
            PListControl.Visibility = Visibility.Visible;
            AddFolder.Visibility = Visibility.Collapsed;
            PlayGrid.Visibility = Visibility.Collapsed;
            PListInfo.Visibility = Visibility.Collapsed;
            OpacityAnim(PListControl, sender);
        }

        private void PlayList_Click(object sender, RoutedEventArgs e)
        {
            PListControl.Visibility = Visibility.Collapsed;
            AddFolder.Visibility = Visibility.Collapsed;
            PListInfo.Visibility = Visibility.Collapsed;
            PlayGrid.Visibility = Visibility.Visible;
            OpacityAnim(PlayGrid, sender);
        }

        private void AddToFolder_Click(object sender, RoutedEventArgs e)
        {
            PListControl.Visibility = Visibility.Collapsed;
            AddFolder.Visibility = Visibility.Visible;
            PListInfo.Visibility = Visibility.Collapsed;
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
                formatter.Serialize(fs, mainPL.AudioList);
            }
            using (FileStream fs = new FileStream("P_lists.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, playLists);
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
                    mainPL.AudioList = (List<Audio>)formatter.Deserialize(fs);
            }

            using (FileStream fs = new FileStream("P_lists.dat", FileMode.OpenOrCreate))
            {
                if (fs.Length != 0)
                {
                    playLists = (List<PlayList>)formatter.Deserialize(fs);
                    playLists[0] = mainPL;
                }
                else
                {
                    playLists.Add(mainPL);
                }
            }

            ListPaths.ItemsSource = new List<string>(Paths);
            PL_ListBox.ItemsSource = new List<PlayList>(playLists);
            Play.ItemsSource = new List<Audio>(mainPL.AudioList);
        }

        private void ChangeAudio(Audio audioContext)
        {
            ms.Source = new Uri(audioContext.DirectoryName + "\\" + audioContext.Name);
            BottomPanel.Visibility = Visibility.Visible;
            TopPlayGrid.DataContext = audioContext; // обновляем верхнюю панель

            Playing = true;
            ms.Play();

            CurrentIndex = CurrentList.IndexOf(audioContext);

            (PlayButton.Content as Image).Source = LoadImage(@"C:\Users\Євгеній\Documents\Pause.png", false);
            
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

        private void AddPlayList_Click(object sender, RoutedEventArgs e)
        {
            CreateListWind Wind = new CreateListWind();
            Wind.Owner = Application.Current.MainWindow;
            this.Opacity = 0.2;
            Wind.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Wind.Show();
        }

        private void Settings_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!SettPopup.IsOpen)
                SettPopup.IsOpen = true;
            else
                SettPopup.IsOpen = false; 
        }

        private void PL_Click(object sender, MouseButtonEventArgs e)
        {
            PListControl.Visibility = Visibility.Collapsed;
            AddFolder.Visibility = Visibility.Collapsed;
            PListInfo.Visibility = Visibility.Visible;
            PlayGrid.Visibility = Visibility.Collapsed;
            PListInfo.DataContext = null;
            PListInfo.DataContext = (sender as Border).DataContext;
        }

        private void PlayListAddAudio_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddAudioWind NewWind = new AddAudioWind();
            NewWind.Owner = Application.Current.MainWindow;
            this.Opacity = 0.2;
            NewWind.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            NewWind.Index = playLists.IndexOf((sender as TextBlock).DataContext as PlayList);

            NewWind.ListOfAudio.ItemsSource = mainPL.AudioList.Where(x => playLists[NewWind.Index].AudioList.Count
                (s => s.DirectoryName == x.DirectoryName && s.Name == x.Name) == 0); // отображаем только песни, которых нет в плейлисте
            NewWind.Show();
        }

        private void RemoveAudioFromPlayList_Click(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Удалить этот аудиофайл?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                int ind = playLists.IndexOf(PListInfo.DataContext as PlayList);
                playLists[ind].AudioList.Remove((sender as TextBlock).DataContext as Audio);
                playLists[ind].GetTime();
                PListInfo.DataContext = null;
                PListInfo.DataContext = playLists[ind];
            }
        }

        private void PlayPlayList_Click(object sender, MouseButtonEventArgs e)
        {
            CurrentList = new List<Audio>((PListInfo.DataContext as PlayList).AudioList);

            PListControl.Visibility = Visibility.Collapsed;
            AddFolder.Visibility = Visibility.Collapsed;
            PListInfo.Visibility = Visibility.Collapsed;
            PlayGrid.Visibility = Visibility.Visible;

            PLrightButt.IsHitTestVisible = false;
            DoubleAnimation Anim = new DoubleAnimation();
            Anim.Completed += delegate
            {
                PLrightButt.IsHitTestVisible = true;
                if (CurrentList.Count > 0)
                {
                    ChangeAudio(CurrentList[0]);
                }
            };
            Anim.From = 0;
            Anim.To = 1;
            Anim.Duration = new Duration(TimeSpan.FromSeconds(1));
            PlayGrid.BeginAnimation(OpacityProperty, Anim);

            Play.ItemsSource = CurrentList;
        }

        private void RandomButt_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            CurrentList = CurrentList.OrderBy(x => x != CurrentList[CurrentIndex] ? rand.Next() : 0).ToList();
            Play.ItemsSource = CurrentList;
            CurrentIndex = 0;
        }

        private void RenamePL_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PlayListName_TB.IsReadOnly = false;
            PlayListName_TB.IsHitTestVisible = true;
            PlayListName_TB.BorderThickness = new Thickness(1);
            PlayListName_TB.CaretBrush = Brushes.White;
            PlayListName_TB.CaretIndex = PlayListName_TB.Text.Length;
            PlayListName_TB.Focus();
            SettPopup.IsOpen = false;
        }

        private void RemovePL_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SettPopup.IsOpen = false;
            if(MessageBox.Show("Удалить этот плейлист?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
               playLists.Remove((sender as TextBlock).DataContext as PlayList);
               PL_ListBox.ItemsSource = new List<PlayList>(playLists);

                PListInfo.Visibility = Visibility.Collapsed; 
                PListControl.Visibility = Visibility.Collapsed;
                AddFolder.Visibility = Visibility.Collapsed;
                PListControl.Visibility = Visibility.Visible;
                PlayGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void PlayListName_TB_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && PlayListName_TB.IsHitTestVisible)
            {
                PlayListName_TB.IsHitTestVisible = false;
                PlayListName_TB.IsReadOnly = true;
                PlayListName_TB.BorderThickness = new Thickness(0);
                PlayListName_TB.IsReadOnlyCaretVisible = false;
            }
        }
    }
}

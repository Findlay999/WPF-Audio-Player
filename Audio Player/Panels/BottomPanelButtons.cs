using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Controls;

namespace Audio_Player
{

    public partial class MainWindow : Window
    {
        
        private void PrevAudio_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentIndex > 0 && CurrentList.Count >= CurrentIndex) //проверка возможности переключения назад
            {
                ChangeAudio(CurrentList[--CurrentIndex]); //переключение песни
            }
        }

        private void NextAudio_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentIndex != -1 && CurrentIndex < CurrentList.Count - 1) //проверка возможности переключения вперед
            {
                ChangeAudio(CurrentList[++CurrentIndex]); //переключение песни
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (Playing)
            {
                ms.Pause(); //приостановить воспроизведение
                Playing = false;
                (PlayButton.Child as Image).Source = LoadImage(@"pack://application:,,,/Resources/PlayB.png", false); //картинка кнопки воспрозведения
            }
            else
            {
                ms.Play(); //продолжить воспроизведение
                Playing = true;
                (PlayButton.Child as Image).Source = LoadImage(@"pack://application:,,,/Resources/Pause.png", false); //картинка кнопки паузы
            }
        }

        private void RandomButt_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            CurrentList = CurrentList.OrderBy(x => x != CurrentList[CurrentIndex] ? rand.Next() : 0).ToList(); //перемешиваем песни
            Play.ItemsSource = CurrentList; // обновляем список песен
            CurrentIndex = 0; //для воспроизведения с начала
        }


        //Слайдер
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
            catch (Exception d)
            {
                MessageBox.Show(d.Message);
            }
        }

        private void timeSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (TotalTime.TotalSeconds > 0)
            {
                ms.Position = TimeSpan.FromSeconds(TotalTime.TotalSeconds * AudioSlider.Value / 10);
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

    }

}
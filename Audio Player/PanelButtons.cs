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
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace Audio_Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private void PrevAudio_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentIndex > 0 && playList.AudioPathList.Count >= CurrentIndex)
            {
                CurrentIndex--;
                ChangeAudio(playList.AudioPathList[CurrentIndex]);
            }
        }

        private void NextAudio_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentIndex != -1 && CurrentIndex < playList.AudioPathList.Count - 1)
            {
                CurrentIndex++;
                ChangeAudio(playList.AudioPathList[CurrentIndex]);
            }
        }


        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (Playing)
            {
                ms.Pause();
                Playing = false;
                (PlayButton.Content as Image).Source = LoadImage(@"C:\Users\Євгеній\Documents\PlayB.png", false);
            }
            else
            {
                ms.Play();
                Playing = true;
                (PlayButton.Content as Image).Source = LoadImage(@"C:\Users\Євгеній\Documents\Pause.png", false);
            }
        }

    }

}
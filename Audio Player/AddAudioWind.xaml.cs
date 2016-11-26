using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Audio_Player
{
    /// <summary>
    /// Interaction logic for AddAudioWind.xaml
    /// </summary>
    public partial class AddAudioWind : Window
    {
        private List<Audio> SelectedAudioList = new List<Audio>();
        public int Index;

        public AddAudioWind()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e) //если checkbox песни отмечен
        {
            SelectedAudioList.Add((sender as CheckBox).DataContext as Audio);  //добавляем песню в список
            if (SelectedAudioList.Count == 1)
            {
                OkB.IsEnabled = true;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)// ecли флажок убран 
        {
            SelectedAudioList.Remove((sender as CheckBox).DataContext as Audio); //удаляем песню с списка
            if (SelectedAudioList.Count == 0)
            {
                OkB.IsEnabled = false;
            }
        }

        private void OkB_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            RefreshMainPlayLists(ref main);
            main.Opacity = 1;
            this.Close();
        }

        private void CancB_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            main.Opacity = 1;
            this.Close();
        }

        private void RefreshMainPlayLists(ref MainWindow main)
        {
            main.playLists[Index].AudioList.AddRange(SelectedAudioList); //добавляем песни в плейлист
            main.PL_ListBox.ItemsSource = new List<PlayList>(main.playLists); //обновляем список плейлистов
            main.playLists[Index].GetTime();//пересчитываем длительность для плейлиста
            main.PListInfo.DataContext = null; 
            main.PListInfo.DataContext = main.playLists[Index]; // обновляем страницу плейлиста
        }
    }
}

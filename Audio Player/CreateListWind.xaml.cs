using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Audio_Player
{
    /// <summary>
    /// Interaction logic for CreateListWind.xaml
    /// </summary>
    public partial class CreateListWind : Window
    {

        public CreateListWind()
        {
            InitializeComponent();
        }

        private void Name_TB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(Name_TB.Text))
            {
                OkB.IsEnabled = true;
            }
            else
            {
                OkB.IsEnabled = false;
            }
        }

        private void CancB_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            main.Opacity = 1;
            this.Close();
        }

        private void OkB_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;

            if (CheckDuplication(main))
                return;

            AddNewPlaylist(ref main);
            SetVisibility(ref main);
            main.Bounce(main.PListInfo, main.ActualWidth, main.ActualHeight);
            this.Close();
        }

        private bool CheckDuplication(MainWindow main)
        {
            if (main.playLists.Any(x => x.Name == Name_TB.Text)) //проверка названия плейлиста
            {
                MessageBox.Show("Плейлист с таким названием уже существует...");
                return true;
            }
            return false;
        }

        private void AddNewPlaylist(ref MainWindow main)
        {
            PlayList NewList = new PlayList();
            NewList.Name = Name_TB.Text;
            main.PListInfo.DataContext = NewList; //обновляем инфу о плейлисте
            main.playLists.Add(NewList); //добавляем новый плейлист
            main.PL_ListBox.ItemsSource = new List<PlayList>(main.playLists); //обновляем список плейлистов
        }

        private void SetVisibility(ref MainWindow main)
        {
            main.PListInfo.Visibility = Visibility.Visible;
            main.PListControl.Visibility = Visibility.Collapsed;
            main.AddFolder.Visibility = Visibility.Collapsed;
            main.PlayGrid.Visibility = Visibility.Collapsed;
            main.Opacity = 1;
        }
    }
}

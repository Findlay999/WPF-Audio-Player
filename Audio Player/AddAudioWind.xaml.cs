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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Audio_Player
{
    /// <summary>
    /// Interaction logic for AddAudioWind.xaml
    /// </summary>
    public partial class AddAudioWind : Window
    {

        List<Audio> SelectedAudioList = new List<Audio>();
        public int Index;

        public AddAudioWind()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SelectedAudioList.Add((sender as CheckBox).DataContext as Audio);
            if (SelectedAudioList.Count == 1)
            {
                OkB.IsEnabled = true;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SelectedAudioList.Remove((sender as CheckBox).DataContext as Audio);
            if (SelectedAudioList.Count == 0)
            {
                OkB.IsEnabled = false;
            }
        }

        private void OkB_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            main.playLists[Index].AudioList.AddRange(SelectedAudioList);
            main.PL_ListBox.ItemsSource = new List<PlayList>(main.playLists);
            main.playLists[Index].GetTime();
            main.PListInfo.DataContext = null;
            main.PListInfo.DataContext = main.playLists[Index];
            main.Opacity = 1;
            this.Close();
        }

        private void CancB_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            main.Opacity = 1;
            this.Close();
        }
    }
}

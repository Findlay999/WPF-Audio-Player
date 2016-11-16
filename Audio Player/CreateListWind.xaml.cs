using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

        private void CancB_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            main.Opacity = 1;
            this.Close();
        }

        private void OkB_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            PlayList NewList = new PlayList();
            NewList.Name = Name_TB.Text;
            main.PListInfo.DataContext = NewList;
            main.playLists.Add(NewList);
            main.PL_ListBox.ItemsSource = new List<PlayList>(main.playLists);
            main.Opacity = 1;
            
            main.PListInfo.Visibility = Visibility.Visible;
            main.PListControl.Visibility = Visibility.Collapsed;
            main.AddFolder.Visibility = Visibility.Collapsed;
            main.PlayGrid.Visibility = Visibility.Collapsed;


            DoubleAnimation Anim = new DoubleAnimation();
            Anim.From = 0;
            Anim.To = 1;
            Anim.Duration = new Duration(TimeSpan.FromSeconds(2));

            main.PListInfo.BeginAnimation(OpacityProperty, Anim);

            this.Close();
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
    }
}

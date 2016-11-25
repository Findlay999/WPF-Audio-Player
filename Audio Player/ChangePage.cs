using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Audio_Player
{
    public partial class MainWindow : Window
    {
        private void SetVisibleMain()
        {
            PListControl.Visibility = Visibility.Collapsed;
            AddFolder.Visibility = Visibility.Collapsed;
            PListInfo.Visibility = Visibility.Collapsed;
            PlayGrid.Visibility = Visibility.Visible;
        }

        private void SetVisiblePlayListsControl()
        {
            PListControl.Visibility = Visibility.Visible;
            AddFolder.Visibility = Visibility.Collapsed;
            PlayGrid.Visibility = Visibility.Collapsed;
            PListInfo.Visibility = Visibility.Collapsed;
        }

        private void SetVisibleFolderAdder()
        {
            PListControl.Visibility = Visibility.Collapsed;
            AddFolder.Visibility = Visibility.Visible;
            PListInfo.Visibility = Visibility.Collapsed;
            PlayGrid.Visibility = Visibility.Collapsed;
        }

        private void SetVisiblePlayListInfo()
        {
            PListControl.Visibility = Visibility.Collapsed;
            AddFolder.Visibility = Visibility.Collapsed;
            PListInfo.Visibility = Visibility.Visible;
            PlayGrid.Visibility = Visibility.Collapsed;
        }

    }
}
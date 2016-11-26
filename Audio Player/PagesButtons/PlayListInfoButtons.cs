using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Audio_Player
{

    public partial class MainWindow : Window
    {

        private void RenamePL_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //делаем текстбокс с названием плейлиста редактируемым
            PlayListName_TB.IsReadOnly = false;
            PlayListName_TB.IsHitTestVisible = true;
            PlayListName_TB.BorderThickness = new Thickness(1);
            PlayListName_TB.CaretBrush = Brushes.White;
            PlayListName_TB.CaretIndex = PlayListName_TB.Text.Length;
            PlayListName_TB.Focus();
            SettPopup.IsOpen = false;
        }

        private void PlayListName_TB_KeyDown(object sender, KeyEventArgs e)
        {
            //убераем возможность редактирования текстбокса с названием (данные названия обновляются)
            if (e.Key == Key.Enter && PlayListName_TB.IsHitTestVisible)
            {
                PlayListName_TB.IsHitTestVisible = false;
                PlayListName_TB.IsReadOnly = true;
                PlayListName_TB.BorderThickness = new Thickness(0);
                PlayListName_TB.IsReadOnlyCaretVisible = false;
            }
        }

        private void PlayPlayList_Click(object sender, MouseButtonEventArgs e)
        {
            CurrentList = new List<Audio>((PListInfo.DataContext as PlayList).AudioList); //присваиваем текущему списку воспроизведения список песен плейлиста
            SetVisibleMain(); //переключаем панель
            PlayPLAnimate(); //анимируем переключение
            Play.ItemsSource = CurrentList;
        }

        private void Settings_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //отображаем меню настроек
            if (!SettPopup.IsOpen)
                SettPopup.IsOpen = true;
            else
                SettPopup.IsOpen = false;
        }


        private void RemovePL_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SettPopup.IsOpen = false;
            MyDialogWindow dialog = new MyDialogWindow();
            dialog.TX.Text = "Удалить этот плейлист?";
            dialog.Owner = this;
            dialog.ShowDialog();
            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                playLists.Remove((sender as TextBlock).DataContext as PlayList); //удаляем плейлист
                PL_ListBox.ItemsSource = new List<PlayList>(playLists); //обновляем окно плейлистов
                SetVisiblePlayListsControl(); //отображаем окно плейлистов
                CenterAnim(PListControl);
            }
        }

        private void PlayListAddAudio_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //создаем окно добавления песен
            AddAudioWind NewWind = new AddAudioWind();
            NewWind.Owner = Application.Current.MainWindow;
            this.Opacity = 0.2;
            NewWind.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            NewWind.Index = playLists.IndexOf((sender as TextBlock).DataContext as PlayList); //передаем в новое окно данные плейлиста

            NewWind.ListOfAudio.ItemsSource = mainPL.AudioList.Where(x => playLists[NewWind.Index].AudioList.Count
                (s => s.DirectoryName == x.DirectoryName && s.Name == x.Name) == 0); // отображаем только песни, которых нет в плейлисте
            NewWind.Show();
        }

        private void RemoveAudioFromPlayList_Click(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Удалить этот аудиофайл?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                int ind = playLists.IndexOf(PListInfo.DataContext as PlayList); //получаем индекс песни
                playLists[ind].AudioList.Remove((sender as TextBlock).DataContext as Audio); //удаляем из плейлистов
                playLists[ind].GetTime(); //пересчитываем время
                PListInfo.DataContext = null;
                PListInfo.DataContext = playLists[ind]; //обновляем окно плейлиста
            }
        }

        private void PlayPLAnimate() //анимация кнопки воспроизвести для плейлиста
        {
            DoubleAnimation Anim = new DoubleAnimation();
            Anim.Completed += delegate
            {
                if (CurrentList.Count > 0)
                {
                    ChangeAudio(CurrentList[0]); // после окончания анимации начинаем воспроизведение с первой песни плейлиста
                }
            };
            Anim.From = 0;
            Anim.To = 1;
            Anim.Duration = new Duration(TimeSpan.FromSeconds(1));
            PlayGrid.BeginAnimation(OpacityProperty, Anim);
        }

    }

}
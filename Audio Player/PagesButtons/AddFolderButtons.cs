using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Audio_Player
{

    public partial class MainWindow : Window
    {

        private void RemovePath_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить этот путь?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (mainPL.AudioList.SequenceEqual(CurrentList))
                {
                    string r = ((Button)sender).Content.ToString(); // получаем директорию, которую удаляем
                    mainPL.AudioList = mainPL.AudioList.Where(x => x.DirectoryName != r).ToList(); //чистим главный плейлист от файлов этой директории
                    Paths.Remove(((Button)sender).Content.ToString()); //удаляем директорию из списка
                    ListPaths.ItemsSource = new List<string>(Paths); // обновляем список директорий
                    Play.ItemsSource = mainPL.AudioList; //обновляем страницу плейлиста
                    CurrentList = mainPL.AudioList; //обновляем список воспроизведения
                    CurrentIndex = 0; //начинаем воспроизведение с начала
                }
                else
                {
                    // то же самое, только без обновления страницы плейлиста и списка воспроизведения
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

    }

}
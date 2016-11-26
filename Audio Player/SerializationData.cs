using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Audio_Player
{
    public partial class MainWindow : Window
    {
        private void DeserializeData()
        {
            //считываем данные
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
                if (fs.Length != 0) //если список плейлистов не пуст
                {
                    playLists = (List<PlayList>)formatter.Deserialize(fs); //переписываем данные
                    mainPL.GetTime();
                    playLists[0] = mainPL; //переписываем главный плейлист в начало
                }
                else
                {
                    mainPL.GetTime();
                    playLists.Add(mainPL); //добавляем главный плейлист
                }
            }

            //обновляем окна
            ListPaths.ItemsSource = new List<string>(Paths);
            PL_ListBox.ItemsSource = new List<PlayList>(playLists);
            Play.ItemsSource = new List<Audio>(mainPL.AudioList);
        }

        private void SerializeData()
        {
            //сохраняем пути, главный плейлист и плейлисты
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

    }
}
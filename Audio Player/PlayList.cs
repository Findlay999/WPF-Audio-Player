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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using TagLib;
using Microsoft.Win32;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;

namespace Audio_Player
{
    [Serializable]
    public class PlayList
    {
        public List<Audio> AudioPathList = new List<Audio>();
        string[] Formats = new string[] { ".aif", ".m3u", ".m4a", ".mid", ".mp3", ".mpa", ".wav", ".wma" };

        public void GetMusic(string Path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(Path);

                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo f in files)
                {
                    if(Formats.Contains(f.Extension))
                    {
                        TagLib.File FD = TagLib.File.Create(f.FullName);
                        AudioPathList.Add(new Audio(f.Name, FD.Tag.Title, f.DirectoryName, FD.Tag.FirstArtist, FD.Tag.Album, FD.Properties.Duration));
                    }
                }

                foreach (DirectoryInfo d in dir.GetDirectories())
                {
                    GetMusic(Path + @"\" + d.Name);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

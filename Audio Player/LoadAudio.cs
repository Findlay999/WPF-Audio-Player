using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using TagLib;
using System.Text;
using System.Threading.Tasks;

namespace Audio_Player
{
    class LoadAudio
    {
        string[] Formats = new string[] { ".aif", ".m3u", ".m4a", ".mid", ".mp3", ".mpa", ".wav", ".wma" };

        public void GetMusic(string Path, ref List<Audio> AudioPathList)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(Path);

                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo f in files)
                {
                    if (Formats.Contains(f.Extension))
                    {
                        TagLib.File FD = TagLib.File.Create(f.FullName);
                        AudioPathList.Add(new Audio(f.Name, FD.Tag.Title, f.DirectoryName, FD.Tag.FirstArtist, FD.Tag.Album, FD.Properties.Duration));
                    }
                }
                /*
                foreach (DirectoryInfo d in dir.GetDirectories())
                {
                    GetMusic(Path + @"\" + d.Name);
                } */
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}

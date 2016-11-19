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
    public class MainPlayList
    {
        public List<Audio> AudioPathList = new List<Audio>();
    }
}

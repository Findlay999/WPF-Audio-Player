using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audio_Player
{
    [Serializable]
    public class PlayList
    {
        public List<Audio> AudioList { get; set; } = new List<Audio>();
        public string Name { get; set; }
        public int AudioTime { get; set; }

        public void GetTime()
        {
            AudioTime = 0;
            foreach(Audio val in AudioList)
            {
                AudioTime += val.Duration.Minutes;
            }
        }
    }
}

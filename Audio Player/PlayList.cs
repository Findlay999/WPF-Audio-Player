using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audio_Player
{
    public class PlayList
    {
        public List<Audio> IndexList { get; set; } = new List<Audio>();
        public string Name { get; set; }
        public TimeSpan AudioTime { get; set; }

        public void GetTime()
        {
            AudioTime = new TimeSpan();
            foreach(Audio val in IndexList)
            {
                AudioTime = AudioTime.Add(val.Duration);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LHLStart.Entity
{
    public class StartGroupInfo
    {
        public StartGroupInfo()
        {
            FileList = new List<StartFileInfo>();
        }

        public string Id { set; get; }

        public string GroupName { set; get; }

        public bool IsChecked { set; get; }

        public List<StartFileInfo> FileList { set; get; }
    }
}

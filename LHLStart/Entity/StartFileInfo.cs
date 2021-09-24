using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LHLStart.Entity
{
    public class StartFileInfo
    {
        public string Id { set; get; }

        public string FileName { set; get; }

        public string FileImagePath { set; get; }

        public string FilePath { set; get; }
         
        public int UseCount { set; get; }

        public DateTime UseTime { set; get; }
    }
}

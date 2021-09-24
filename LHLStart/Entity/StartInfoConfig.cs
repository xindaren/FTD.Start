using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LHLStart.Entity
{
    public class StartInfoConfig
    {

        private StartInfoConfig() { }
        private static object lockObj = new object();
        private static StartInfoConfig _Instance; 
        public static StartInfoConfig Instance
        {
            get
            {
                if (_Instance != null)
                    return _Instance;

                lock (lockObj)
                {
                    if (_Instance == null)
                    {
                        string filePath = string.Format("{0}\\JsonCongfig.txt", System.Windows.Forms.Application.StartupPath.ToLower());
                        string json = "";
                        try
                        {
                            if (File.Exists(filePath))
                            {
                                json = File.ReadAllText(filePath);
                                byte[] mybyte = Encoding.UTF8.GetBytes(json);
                                json = Encoding.UTF8.GetString(mybyte);
                            }
                        }
                        catch (Exception ex)
                        {
                        }

                        var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<StartInfoConfig>(json);

                        if (!string.IsNullOrEmpty(json) && obj != null)
                        {
                            _Instance = obj;
                        }
                        else
                        {
                            _Instance = new StartInfoConfig();
                            _Instance.WinWidth = 500;
                            _Instance.WinHeight = 600;
                            _Instance.DeleteImageFileList = new List<string>();
                            _Instance.GroupInfos = new List<StartGroupInfo>();
                            _Instance.GroupInfos.Add(new StartGroupInfo()
                            {
                                Id = "mr",
                                IsChecked = true,
                                GroupName = "我的文件"
                            });
                        }
                    }
                }
            
                    
                return _Instance;
            }
        }

        public List<StartGroupInfo> GroupInfos { set; get; }

        public bool IsOpenStart { set; get; }

        public double WinWidth { set; get; }

        public double WinHeight { set; get; }

        public List<string> DeleteImageFileList { set; get; }

        public void SaveConfig()
        { 
           var json =  Newtonsoft.Json.JsonConvert.SerializeObject(StartInfoConfig.Instance);

            //文件路径
            string filePath = string.Format("{0}\\JsonCongfig.txt", System.Windows.Forms.Application.StartupPath.ToLower());
             
            byte[] mybyte = Encoding.UTF8.GetBytes(json);
            string mystr1 = Encoding.UTF8.GetString(mybyte);
             
            File.WriteAllText(filePath, mystr1); 
        }
    }
}

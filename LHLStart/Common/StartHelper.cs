using LHLStart.Entity;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IWshRuntimeLibrary;

namespace LHLStart.Common
{
    public class StartHelper
    {   
        /// <summary>
        /// 创建组
        /// </summary> 
        public static void CreateGroup(string name)
        {
            StartGroupInfo group = new StartGroupInfo();
            group.Id = Guid.NewGuid().ToString();
            group.GroupName = name; 
            StartInfoConfig.Instance.GroupInfos.Add(group); 
            StartInfoConfig.Instance.SaveConfig();
        }

        /// <summary>
        /// 创建文件
        /// </summary> 
        public static void CreateFile(string filePath,string groupId)
        {
            var gp = StartInfoConfig.Instance.GroupInfos.FirstOrDefault(p => p.Id == groupId);
            if (gp == null)
                return;

            FileInfo f = new FileInfo(filePath);
            if (!f.Exists)
                return;

            string descName=null;
            if (f.Extension.ToLower() == ".lnk")
            {
                WshShell shell = new WshShell();
                IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(filePath);
                filePath = shortcut.TargetPath;
                descName = f.Name;
                f = new FileInfo(filePath);
            }
             
            var saveDir = string.Format("{0}\\Images\\", System.Windows.Forms.Application.StartupPath.ToLower());
            Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(filePath);
            System.Drawing.Image b = icon.ToBitmap();
            string savePath = saveDir + Guid.NewGuid().ToString("N") + ".jpg";
            b.Save(savePath);
            MemoryStream ms = new MemoryStream();
            b.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] bytes = ms.GetBuffer();
            ms.Close();

            StartFileInfo sf = new StartFileInfo();
            sf.Id = Guid.NewGuid().ToString("N");
            sf.FileName = descName == null? f.Name: descName;
            sf.FilePath = filePath;
            sf.FileImagePath = savePath; 
            gp.FileList.Add(sf);
             
           
            StartInfoConfig.Instance.SaveConfig();
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteFile(string id)
        {
            foreach (var g in StartInfoConfig.Instance.GroupInfos)
            {
                var f = g.FileList.FirstOrDefault(p => p.Id == id);
                if (f != null)
                {
                    g.FileList.Remove(f);
                    StartInfoConfig.Instance.DeleteImageFileList.Add(f.FileImagePath); 
                    DeleteImageFile();
                    StartInfoConfig.Instance.SaveConfig();
                    return;
                } 
            } 
        }

        /// <summary>
        /// 删除图片文件
        /// </summary>
        public static void DeleteImageFile()
        {
            foreach (var imf in StartInfoConfig.Instance.DeleteImageFileList)
            {
                try
                {
                    System.IO.File.Delete(imf); 
                    return;
                }
                catch { }
            }
        }

        /// <summary>
        /// 删除分组
        /// </summary> 
        public static void DeleteGroup(string id)
        {
            foreach (var g in StartInfoConfig.Instance.GroupInfos)
            {
                if (g.Id == id)
                {
                    StartInfoConfig.Instance.GroupInfos.Remove(g);
                    StartInfoConfig.Instance.SaveConfig();
                    return;
                }
            }
        }

        /// <summary>
        /// 修改分组
        /// </summary> 
        public static void EditGroup(string id,string name)
        {
            foreach (var g in StartInfoConfig.Instance.GroupInfos)
            {
                if (g.Id == id)
                {
                    g.GroupName = name;
                    StartInfoConfig.Instance.SaveConfig();
                    return;
                }
            }
        }

        /// <summary>
        /// 运行程序
        /// </summary> 
        public static void RunFile(string path, bool isAdmin = false)
        {
            if (!System.IO.File.Exists(path))
                return;

            if (isAdmin)
            {  
                Process p = new Process(); 
                try
                {
                    p.StartInfo.FileName = path;
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.Verb = "RunAs";
                    p.StartInfo.UseShellExecute = false; 
                    p.Start();
                    p.WaitForExit();
                    p.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show("不是有效的运行程序！");
                }
            }
            else
            {
                System.Diagnostics.Process.Start(path);
            }
        }

        /// <summary>
        /// 打开目录
        /// </summary> 
        public static void OpenDir(string path)
        {
            FileInfo f = new FileInfo(path);
            
            System.Diagnostics.Process.Start("explorer.exe", f.DirectoryName);
        }

        /// <summary>
        /// 设置开机自动运行
        /// </summary> 
        public static void SetOpenRun(bool bl)
        {
            StartInfoConfig.Instance.IsOpenStart = bl;
           

            if (bl)
            {
                string strName = AppDomain.CurrentDomain.BaseDirectory + "LHLStart.exe";//获取要自动运行的应用程序名
                if (!System.IO.File.Exists(strName))//判断要自动运行的应用程序文件是否存在
                    return;
                string strnewName = strName.Substring(strName.LastIndexOf("\\") + 1);//获取应用程序文件名，不包括路径
                RegistryKey registry = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//检索指定的子项
                if (registry == null)//若指定的子项不存在
                    registry = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");//则创建指定的子项
                registry.SetValue(strnewName, strName);//设置该子项的新的“键值对”
            }
            else
            {
                string strName = AppDomain.CurrentDomain.BaseDirectory + "LHLStart.exe";//获取要自动运行的应用程序名
                if (!System.IO.File.Exists(strName))//判断要取消的应用程序文件是否存在
                    return;
                string strnewName = strName.Substring(strName.LastIndexOf("\\") + 1);///获取应用程序文件名，不包括路径
                RegistryKey registry = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);//读取指定的子项
                if (registry == null)//若指定的子项不存在
                    registry = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");//则创建指定的子项
                registry.DeleteValue(strnewName, false);//删除指定“键名称”的键/值对 
            }

            StartInfoConfig.Instance.SaveConfig();
        }

        /// <summary>
        /// 修改窗体大小
        /// </summary> 
        public static void EditWinSize(double width,double hegiht)
        {
            StartInfoConfig.Instance.WinHeight = hegiht;
            StartInfoConfig.Instance.WinWidth = width;
            StartInfoConfig.Instance.SaveConfig();
        }
    }
}

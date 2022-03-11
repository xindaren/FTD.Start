using LHLStart.Common;
using LHLStart.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents; 
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes; 

namespace LHLStart
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon notifyIcon; 
        public MainWindow()
        {
            InitializeComponent();
           
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            this.MouseDown += MainWindow_MouseDown;

            
           
        }

        #region 吸附
        bool isEnter = false; 
        private void XF_MouseLeave(object sender, MouseEventArgs e)
        {
            if (isEnter)
            {
                isEnter = false;
                return;
            }
                
            var top = this.Top;
            if (top == 0)
            {
                this.Top = -(this.ActualHeight - 2);

                POINT point;
                GetCursorPos(out point);
                 
                if (point.Y<20)
                {
                    point.Y = 20;
                    SetCursorPos(point.X, point.Y);
                }  
            } 
        }

        private void XF_MouseEnter(object sender, MouseEventArgs e)
        {
            var top = this.Top;
            var hideTop = -(this.ActualHeight - 2);

            var v = Math.Abs(top - hideTop);

            if (v > 2)
                return;

            this.Top = 0;
        }

         
        [DllImport("User32")]
        public extern static void SetCursorPos(int x, int y);
        public struct POINT
        {
            public int X;
            public int Y;
            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }
         
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out POINT pt);

        #endregion
         
        #region 事件
          
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //隐藏任务栏和切换栏
            HideTaskbarAndAltTab();

            //加载数据
            LoadData();

            //开启托盘
            StartNotifyIcon();
            //开启热键
            StartHot();
            //置顶
            TopWindow();
        }
         
        /// <summary>
        /// 禁止win10 屏幕边缘自动放大
        /// </summary> 
        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    var windowMode = this.ResizeMode;
                    if (this.ResizeMode != ResizeMode.NoResize)
                    {
                        this.ResizeMode = ResizeMode.NoResize;
                    }
                    this.UpdateLayout();

                    DragMove();
                    if (this.ResizeMode != windowMode)
                    {
                        this.ResizeMode = windowMode;
                    }
                    this.UpdateLayout();
                }
            }
        }
         
        /// <summary>
        /// 接收拖拽文件
        /// </summary> 
        private void Window_Drop(object sender, DragEventArgs e)
        {
            string path = "Drop";
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            }

            var gp = StartInfoConfig.Instance.GroupInfos.FirstOrDefault(p => p.IsChecked == true);

            if (gp == null)
            {
                MessageBox.Show("请选择模块！");
                return;
            }

            StartHelper.CreateFile(path, gp.Id);
              
            BindStartFileList();
        }

        /// <summary>
        /// 运行
        /// </summary> 
        private void CbxIsOpenRun_Click(object sender, RoutedEventArgs e)
        {
            StartHelper.SetOpenRun(this.cbxIsOpenRun.IsChecked.Value);
        }

        /// <summary>
        /// 关闭
        /// </summary> 
        private void Close_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
        }
        #endregion

        #region 绑定
        private void LoadData()
        {
            BindStartGroupList();
            BindStartFileList();

            this.Width = StartInfoConfig.Instance.WinWidth;
            this.Height = StartInfoConfig.Instance.WinHeight;
            this.cbxIsOpenRun.IsChecked = StartInfoConfig.Instance.IsOpenStart;
        }

        private void BindStartFileList()
        {
            var gp = StartInfoConfig.Instance.GroupInfos.FirstOrDefault(p => p.IsChecked == true);

            if (gp == null)
                return;
            if (gp.FileList == null)
                return;

            this.lb.ItemsSource = null; 
            this.lb.ItemsSource = gp.FileList; 
        }

        private void BindStartGroupList()
        { 
            var g = StartInfoConfig.Instance.GroupInfos.FirstOrDefault(p=> p.IsChecked == true);
            if (g == null && StartInfoConfig.Instance.GroupInfos.Count>0) 
                StartInfoConfig.Instance.GroupInfos[0].IsChecked = true; 

            this.gp.ItemsSource = null;
            this.gp.ItemsSource = StartInfoConfig.Instance.GroupInfos;
        }
        #endregion
          
        #region 分组
        private string _editGropuId;

        private void Group_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var obj = sender as StackPanel;
            if (obj == null)
                return;

            var id = obj.Tag.ToString();

            var gp = StartInfoConfig.Instance.GroupInfos.FirstOrDefault(p => p.Id == id);

            foreach (var g in StartInfoConfig.Instance.GroupInfos)
            {
                g.IsChecked = false;
            }

            gp.IsChecked = true;

            BindStartGroupList();
            BindStartFileList();
        }
         
        private void Gb_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.isEnter = true;

            var obj = sender as StackPanel;
            if (obj == null)
                return;

            _editGropuId = obj.Tag.ToString();

            gpcm.Visibility = Visibility.Visible;
            gpsc.Visibility = Visibility.Visible;
            if (_editGropuId == "mr")
                gpsc.Visibility = Visibility.Collapsed;
            else
                gpsc.Visibility = Visibility.Visible;

            this.buttonPup.IsOpen = true; 
        }

        private void GN_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.isEnter = true;
            gpcm.Visibility = Visibility.Collapsed;
            gpsc.Visibility = Visibility.Collapsed;
            this.buttonPup.IsOpen = true;
        }

        private void XJ_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void CM_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
 
        private void Gpsc_Click(object sender, RoutedEventArgs e)
        {
            StartHelper.DeleteGroup(_editGropuId);

            BindStartGroupList();
            BindStartFileList();
            this.buttonPup.IsOpen = false;
        }
          
        private void CM_Click(object sender, RoutedEventArgs e)
        {
            this.buttonPup.IsOpen = false;
            CreateGroup n = new CreateGroup(BindStartGroupList, _editGropuId);
            n.ShowDialog();
        }

        private void XJ_Click(object sender, RoutedEventArgs e)
        {
            this.buttonPup.IsOpen = false; 
            CreateGroup n = new CreateGroup(BindStartGroupList);
            n.ShowDialog();
        }

        #endregion

        #region 文件
        private StartFileInfo _fileInfo;
        private void File_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.isEnter = true;
            var obj = sender as StackPanel;
            if (obj == null)
                return;

            _fileInfo = obj.Tag as StartFileInfo; 
            this.filePup.IsOpen = true;
        }

        private void Item_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var obj = sender as StackPanel;
            if (obj == null)
                return;

            var st = obj.Tag as StartFileInfo;

            if (st!=null)
            {
                StartHelper.RunFile(st.FilePath);
            }
            
        }

        private void fileDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_fileInfo != null)
            {
                StartHelper.DeleteFile(_fileInfo.Id);
                BindStartFileList();
            }

            this.filePup.IsOpen = false;
        }

        private void adminRun_Click(object sender, RoutedEventArgs e)
        {
            if (_fileInfo != null)
            {
                StartHelper.RunFile(_fileInfo.FilePath,true);
            }

            this.filePup.IsOpen = false;
        }

        private void openDir_Click(object sender, RoutedEventArgs e)
        {
            if (_fileInfo != null)
            {
                StartHelper.OpenDir(_fileInfo.FilePath);
            }

            this.filePup.IsOpen = false; 
        }

        #endregion

        #region 托盘
        public void StartNotifyIcon()
        {
            string startAppFullName = System.IO.Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "\\AppIcon.ico");

            this.notifyIcon = new System.Windows.Forms.NotifyIcon();
            this.notifyIcon.Icon = new System.Drawing.Icon(startAppFullName);
            this.notifyIcon.Visible = true;

            System.Windows.Forms.MenuItem open = new System.Windows.Forms.MenuItem("显示");
            open.Click += new EventHandler(Show);
          
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("退出");
            exit.Click += new EventHandler(Close);
            //关联托盘控件    
            System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { open, exit };
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);

            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler((o, e) =>
            {
                this.TopShow();
            });
        }

        private void Show(object sender, EventArgs e)
        {
            this.TopShow();
        }

        private void Close(object sender, EventArgs e)
        {
            StartHelper.EditWinSize(this.ActualWidth,this.ActualHeight);
            this.notifyIcon.Visible = false;
            System.Windows.Application.Current.Shutdown();
        }
         
        #endregion

        #region 热键
         
        private void StartHot()
        {
            HotKey hotKey = new HotKey(this, HotKey.KeyFlags.MOD_CONTROL, System.Windows.Forms.Keys.Q);
            hotKey.OnHotKey += HotKey_OnHotKey;
        }

        private void HotKey_OnHotKey()
        {
            if (this.Visibility == Visibility.Visible) {
                this.Hide();
            }
            else { 
                this.TopShow();  
            }
            
        }
        #endregion
         
        #region 置顶显示
        /// <summary>
        /// 置顶显示
        /// </summary>
        private void TopShow()
        {
            this.TopWindow();
            this.Show();
            //var top = this.Top;
            //var hideTop = -(this.ActualHeight - 15);

            //if (top != hideTop)
            //    return;

            this.Top = 0;
        }

        /// <summary>
        /// 置顶窗口
        /// </summary>
        private void TopWindow()
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            SetWindowPos(hwnd.ToInt32(), -1, 0, 0, 0, 0, 0x001 | 0x002 | 0x040);
        }
 
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern int SetWindowPos(int hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        #endregion
         
        #region 隐藏任务栏和切换栏
        private void HideTaskbarAndAltTab()
        {
            //Variable to hold the handle for the form
            var helper = new WindowInteropHelper(this).Handle;
            //Performing some magic to hide the form from Alt+Tab
            SetWindowLong(helper, GWL_EX_STYLE, (GetWindowLong(helper, GWL_EX_STYLE) | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW); 
            this.ShowInTaskbar = false;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private const int GWL_EX_STYLE = -20;
        private const int WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;
        #endregion 
         
    }
}

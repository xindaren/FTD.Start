using LHLStart.Common;
using LHLStart.Entity;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LHLStart
{
    /// <summary>
    /// CreateGroup.xaml 的交互逻辑
    /// </summary>
    public partial class CreateGroup : Window
    {
        public Action CallBackAction;
        public string GroupId;
        public CreateGroup(Action callBackAction, string groupId = null)
        {
            InitializeComponent();
            this.CallBackAction = callBackAction;
            this.GroupId = groupId;

            if (!string.IsNullOrEmpty(this.GroupId))
            {
                var gp = StartInfoConfig.Instance.GroupInfos.FirstOrDefault(p => p.Id == groupId);
                this.tbxName.Text = gp.GroupName;
            }

            this.tbxName.Focus();
        }



        private void XJ_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.tbxName.Text.Trim()))
            {
                MessageBox.Show("名称不能为空");
                return;
            }
                

            if (!string.IsNullOrEmpty(this.GroupId))
                StartHelper.EditGroup(this.GroupId, this.tbxName.Text.Trim());
            else
                StartHelper.CreateGroup(this.tbxName.Text.Trim());

            if (this.CallBackAction != null)
                this.CallBackAction();

            this.Close();
        }

        private void QX_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch { }
        }
    }
}

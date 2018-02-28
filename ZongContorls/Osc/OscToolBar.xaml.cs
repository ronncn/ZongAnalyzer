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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZongContorls.Osc
{
    /// <summary>
    /// OscTool.xaml 的交互逻辑
    /// </summary>
    public partial class OscToolBar : UserControl
    {
        public OscToolBar()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        public void SetContext(object context)
        {
            this.DataContext = context;
        }

        //设置链接成功
        public void SetConnectState(bool flag)
        {
            BitmapImage imgSource;
            if (flag)
            {
                imgSource = new BitmapImage(new Uri("../Images/_link.png", UriKind.Relative));
                this.connectBorder.ToolTip = "已连接";
            }
            else
            {
                imgSource = new BitmapImage(new Uri("../Images/unlink.png", UriKind.Relative));
                this.connectBorder.ToolTip = "未连接";
            }
            this.connect.Source = imgSource;
        }


        //设置BtnStop按钮的上下文
        public void SetBtnStopContext(object context)
        {
            this.BtnStop.DataContext = context;
        }
             
    }
}

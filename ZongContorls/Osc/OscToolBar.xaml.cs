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

        public OscPanel _OscPanel;
        /// <summary>
        /// 设置数据
        /// </summary>
        public void SetContext(object context)
        {
            this.DataContext = context;
        }
        public void SetOscPanel(OscPanel oscPanel)
        {
            this._OscPanel = oscPanel;
            this.BtnStop.DataContext = _OscPanel.display;
        }

        //设置链接成功
        public void SetConnectState(bool flag)
        {
            BitmapImage imgSource;
            if (flag)
            {
                imgSource = new BitmapImage(new Uri("../Images/link.png", UriKind.Relative));
                this.connectBorder.ToolTip = "已连接";
            }
            else
            {
                imgSource = new BitmapImage(new Uri("../Images/unlink.png", UriKind.Relative));
                this.connectBorder.ToolTip = "未连接";
            }
            this.connect.Source = imgSource;
        }

        private void PrintScreen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this._OscPanel.display.PrintScreen();
        }
    }
}

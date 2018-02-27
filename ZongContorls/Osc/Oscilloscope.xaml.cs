using AnalyzerLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ZongContorls
{
    /// <summary>
    /// Oscilloscope.xaml 的交互逻辑
    /// </summary>
    public partial class Oscilloscope : UserControl
    {
        public Oscilloscope()
        {
            InitializeComponent();//初始化元件
            InitializeSignalListBox();//初始化通道列表
            dataHandling = new DataHandling();
            dataHandling.PropertyChanged += new PropertyChangedEventHandler(UsbState_PropertyChanged);
        }

        private DataHandling dataHandling;

        /// <summary>
        /// usb状态属性改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UsbState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //工具条的链接状态信息,改变
            this.OscToolBar_1.SetConnectState(dataHandling.UsbState);
        }

        //初始化通道列表
        private void InitializeSignalListBox()
        {
            this.SignalListBox_1.BindChannels(ChannelManager.Channels);
        }
    }
}

using AnalyzerLogic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ZongContorls.Osc
{
    /// <summary>
    /// AddChannelWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddChannelWindow : Window
    {
        public SignalListBox ParentListBox;
        public AddChannelWindow(SignalListBox listBox)
        {
            InitializeComponent();
            this.ParentListBox = listBox;
            BindChannels(ChannelManager.Signals);
        }

        public AddChannelWindow(ObservableCollection<Signal> signels, SignalListBox listBox)
        {
            InitializeComponent();
            this.ParentListBox = listBox;
            BindChannels(signels);
        }
        public ObservableCollection<Signal> Signels;

        /// <summary>
        /// 绑定信道列表
        /// </summary>
        /// <param name="signels"></param>
        public void BindChannels(ObservableCollection<Signal> signels)
        {
            this.Signels = signels;
            signalListBox.ItemsSource = Signels;
        }

        /// <summary>
        /// 添加相应的信道,可添加单个和多个信道,也可以添加重复通道
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (signalListBox.SelectedIndex == -1)
            {
                MessageBox.Show("请选择至少一个信道添加!");
            }
            else
            {
                var collection = signalListBox.SelectedItems.Cast<Signal>();
                var signals = collection.ToList();
                foreach(Signal si in signals)
                {
                    ChannelManager.AddChannel(si);
                }
                ObservableCollection<ChannelItem> ChannelItems = new ObservableCollection<ChannelItem>();
                foreach(Channel chan in ChannelManager.Channels)
                {
                    ChannelItem channelItem = new ChannelItem(chan);
                    ChannelItems.Add(channelItem);
                }
                this.ParentListBox.BindingChannels(ChannelItems);
                this.StartDisplay();
                this.Close();
            }
            
        }

        /// <summary>
        /// 新建一个空白通道
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void New_Click(object sender, RoutedEventArgs e)
        {
            ChannelManager.AddChannel();
            ObservableCollection<ChannelItem> ChannelItems = new ObservableCollection<ChannelItem>();
            foreach (Channel chan in ChannelManager.Channels)
            {
                ChannelItem channelItem = new ChannelItem(chan);
                ChannelItems.Add(channelItem);
            }
            this.ParentListBox.BindingChannels(ChannelItems);
            this.StartDisplay();
            //关闭窗口
            this.Close();
        }

        private void StartDisplay()
        {
            if (ParentListBox.ChannelItems.Count != 0)
            {
                ParentListBox._OscPanel.display.IsDraw = true;
                ParentListBox._OscPanel.display.Switch = true;
            }
        }
    }
}

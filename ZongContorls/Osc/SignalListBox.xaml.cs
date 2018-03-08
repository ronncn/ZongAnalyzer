using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using AnalyzerLogic;

namespace ZongContorls.Osc
{
    /// <summary>
    /// SignalListBox.xaml 的交互逻辑
    /// </summary>
    public partial class SignalListBox : UserControl
    {
        public ObservableCollection<ChannelItem> ChannelItems = new ObservableCollection<ChannelItem>();
        public SignalListBox()
        {
            InitializeComponent();
        }
        
        public OscPanel _OscPanel;
        
        public void BindingChannels(ObservableCollection<ChannelItem> channelitems)
        {
            this.ChannelItems = channelitems;
            _OscPanel.ClearOscSwiperBtn();
            foreach(ChannelItem chitem in channelitems)
            {
                _OscPanel.AddOscSwiperBtn(chitem);
            }
            listBox.ItemsSource = ChannelItems;
            this._OscPanel.display.BindChannels(ChannelItems);
        }

        public void SetOscPanel(OscPanel oscPanel)
        {
            this._OscPanel = oscPanel;
        }

        private Channel _SelectedChannelItem;
        public Channel SelectedChannelItem
        {
            get { return _SelectedChannelItem; }
            set
            {
                if (value != _SelectedChannelItem)
                {
                    _SelectedChannelItem = value;
                    ValueChanged();
                }
            }
        }
        
        public delegate void ChangedEventHandler();         //定义委托
        public event ChangedEventHandler ValueChanged;      //定义事件
        
        //关闭信道item
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem listItem = GetParentObject<ListBoxItem>((DependencyObject)sender);

            ChannelItem c = (ChannelItem)(listItem).DataContext;
            this.ChannelItems.Remove(c);
            foreach(Channel chan in ChannelManager.Channels)
            {
                if(chan.Id == c.Id)
                {
                    ChannelManager.Channels.Remove(chan);
                    return;
                }
            }
        }

        /// <summary>
        /// 获得指定元素的父元素  
        /// </summary>  
        /// <typeparam name="T">指定页面元素</typeparam>  
        /// <param name="obj"></param>  
        /// <returns></returns>  
        public T GetParentObject<T>(DependencyObject obj) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);

            while (parent != null)
            {
                if (parent is T)
                {
                    return (T)parent;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
        
        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedChannelItem = (Channel)listBox.SelectedItem;
        }

        //隐藏按钮,点击隐藏信道
        private void show_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem listItem = GetParentObject<ListBoxItem>((DependencyObject)sender);
            ChannelItem c = (ChannelItem)(listItem).DataContext;
            listItem.IsSelected = true;
            if(listItem.Background != Brushes.Gray)
            {
                //隐藏
                listItem.Background = Brushes.Gray;
                c.IsShow = false;
            }
            else
            {
                //显示
                listItem.Background = listItem.BorderBrush;
                c.IsShow = true;
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            AddChannelWindow addChannelWindow = new AddChannelWindow(this);
            addChannelWindow.Show();
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            this.ChannelItems.Clear();
            ChannelManager.Channels.Clear();
        }
    }

    /// <summary>
    /// ChannelItem
    /// </summary>
    public class ChannelItem : INotifyPropertyChanged
    {
        private double _Offset_X = 0;         //X轴偏移值
        private double _Offset_Y = 0;         //Y轴偏移值
        private bool _IsShow = true;          //是否显示

        private Channel channel;

        public ChannelItem(Channel chan)
        {
            this.channel = chan;
            this.channel.ValueChanged += new Channel.ChangedEventHandler(Value_Changed);
            this.channel.Init_ValueChanged += new Channel.ChangedEventHandler(Init_Value_Changed);
        }

        private void Value_Changed()
        {
            this.Value = this.channel.Value;
        }

        private void Init_Value_Changed()
        {
            this.Init_Value = this.channel.Init_Value;
        }

        public int Id
        {
            get { return channel.Id; }
        }

        public string Name
        {
            get { return channel.Name; }
        }
        public string From
        {
            get { return channel.From; }
        }

        public string Color
        {
            get { return channel.Color; }
        }

        public double Value
        {
            get { return channel.Value; }
            set
            {
                channel.Value = value;
                OnPropertyChanged("Value");
            }
        }

        public int Init_Value
        {
            get { return channel.Init_Value; }
            set
            {
                channel.Init_Value = value;
                OnPropertyChanged("Init_Value");
            }
        }
        
        public List<double> Values
        {
            get { return channel.Values; }
        }

        public bool IsShow
        {
            get { return _IsShow; }
            set { _IsShow = value; }
        }

        public double Offset_X
        {
            get { return _Offset_X; }
            set { _Offset_X = value; }
        }

        public double Offset_Y
        {
            get { return _Offset_Y; }
            set { _Offset_Y = value; }
        }
        private double _MaxValue;
        public double MaxValue
        {
            get { return _MaxValue; }
            set
            {
                if (value != _MaxValue)
                {
                    _MaxValue = value;
                    OnPropertyChanged("MaxValue");
                }
            }
        }

        private double _MinValue;
        public double MinValue
        {
            get { return _MinValue; }
            set
            {
                if (value != _MinValue)
                {
                    _MinValue = value;
                    OnPropertyChanged("MinValue");
                }
            }
        }

        private double _AvgValue;
        public double AvgValue
        {
            get { return _AvgValue; }
            set
            {
                if (value != _AvgValue)
                {
                    _AvgValue = value;
                    OnPropertyChanged("AvgValue");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

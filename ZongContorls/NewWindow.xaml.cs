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

namespace ZongContorls
{
    /// <summary>
    /// NewWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewWindow : UserControl
    {
        public ObservableCollection<ChannelItem> Channels = new ObservableCollection<ChannelItem>();
        //显示配置参数;
        private DisplayConfig displayConfig;

        public bool IsAnalysis
        {
            get { return DisPlayPanel.IsAnalysis; }
            set { DisPlayPanel.IsAnalysis = value; }
        }

        public NewWindow()
        {
            InitializeComponent();
            ChannelListBox_1.BindChannels(Channels);
            ChannelListBox_1.ValueChanged += new ChannelListBox.ChangedEventHandler(ChannelListBox_1_ValueChanged);
            DisPlayPanel.BindChannels(Channels);

            displayConfig = new DisplayConfig();

            this.DataContext = displayConfig;
            displayConfig.PropertyChanged += new PropertyChangedEventHandler(DisplayPanel_ProPertyChanged);
            this.SetConfig();

            this.BtnBar.DataContext = DisPlayPanel;
        }

        private void DisplayPanel_ProPertyChanged(object sender,PropertyChangedEventArgs e)
        {
            this.SetConfig();
        }

        private void ChannelListBox_1_ValueChanged()
        {
            this.PropertyGrid.DataContext = ChannelListBox_1.SelectedChannelItem;
        }

        /// <summary>
        /// 设置配置
        /// </summary>
        private void SetConfig()
        {
            Ruler_X.Unit_Value = displayConfig.Unit_X;
            Ruler_Y.Unit_Value = displayConfig.Unit_Y;
            DisPlayPanel.Unit_X = displayConfig.Unit_X;
            DisPlayPanel.Unit_Y = displayConfig.Unit_Y;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            List<Channel> dropList = (List<Channel>)e.Data.GetData(typeof(List<Channel>));
            //遍历判断channel是否存在
            foreach (Channel p in dropList)
            {
                bool flag = true;
                foreach (ChannelItem c in Channels)
                {
                    if (p.Id == c.Id) { flag = false; }
                }
                if (flag) { this.AddChannelItem(p); }
            }
            if (Channels.Count != 0)
            {
                //开始绘制定时器
                //TimerSwitch = true;
                DisPlayPanel.IsDraw = true;
                DisPlayPanel.Switch = true;
            }
        }

        /// <summary>
        /// 添加通道
        /// </summary>
        /// <param name="ch">原始通道数据</param>
        public void AddChannelItem(Channel ch)
        {
            ChannelItem channelItem = new ChannelItem(ch);
            Channels.Add(channelItem);
            AddSwiperBtn(channelItem);
        }

        /// <summary>
        /// 添加滑块按钮
        /// </summary>
        /// <param name="ch"></param>
        private void AddSwiperBtn(ChannelItem ch)
        {
            SwiperBtn btn = new SwiperBtn();
            btn.Id = ch.Id;
            Binding binding = new Binding { Source = ch, Path = new PropertyPath("Color") };
            btn.SetBinding(SwiperBtn.colorProperty, binding);
            Binding binding1 = new Binding { Source = ch, Path = new PropertyPath("Name") };
            btn.SetBinding(SwiperBtn.nameProperty, binding1);
            double val = (double)(DisPlayPanel.Height / 2 - 10 - ch.Offset_Y);
            btn.SetValue(Canvas.TopProperty, val);
            btn.PreviewMouseDown += SwiperBtn_MouseDown;
            btn.PreviewMouseMove += SwiperBtn_MouseMove;
            btn.PreviewMouseUp += SwiperBtn_MouseUp;
            this.SwiperCanvas.Children.Add(btn);
        }
        
        private bool isMouseDownSwiperBtn = false;

        double start_y;
        double first;
        private void SwiperBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isMouseDownSwiperBtn = true;
            start_y = e.GetPosition(SwiperCanvas).Y;
            SwiperBtn btn = (SwiperBtn)sender;
            first = (double)btn.GetValue(Canvas.TopProperty);
        }

        double end_y;
        private void SwiperBtn_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDownSwiperBtn && DisPlayPanel.Switch)
            {
                end_y = e.GetPosition(SwiperCanvas).Y;
                SwiperBtn btn = (SwiperBtn)sender;
                double val = first + (end_y - start_y);
                if (val < -10) { val = -10; }
                if (val > DisPlayPanel.Height - 10) { val = DisPlayPanel.Height - 10; }
                foreach(ChannelItem ch in Channels)
                {
                    if(btn.Id == ch.Id)
                    {
                        ch.Offset_Y = DisPlayPanel.Height / 2 - val - 10;
                    }
                }
                btn.SetValue(Canvas.TopProperty, val);
                e.Handled = true;
            }
        }

        private void SwiperBtn_MouseUp(object sender, RoutedEventArgs e)
        {
            isMouseDownSwiperBtn = false;   
        }

        private bool MouseEnterFlag = false;
        private void TextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            MouseEnterFlag = true;
        }

        private void TextBox_1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (MouseEnterFlag)
            {
                TextBox textBox = (TextBox)sender;
                int num = Convert.ToInt32(textBox.Text);
                num += e.Delta / 12;
                textBox.Text = num.ToString();
            }
        }

        private void TextBox_2_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (MouseEnterFlag)
            {
                TextBox textBox = (TextBox)sender;
                int num = Convert.ToInt32(textBox.Text);
                num += e.Delta / 120;
                textBox.Text = num.ToString();
            }
        }

        private void TextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            MouseEnterFlag = false;
        }

        private void PrintScreen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DisPlayPanel.PrintScreen();
        }

    }

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
        }

        private void Value_Changed()
        {
            this.Value = this.channel.Value;
        }

        public int Id
        {
            get { return channel.Id; }
        }

        public string Name
        {
            get { return channel.Name; }
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

        public List<double> Init_Values
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
                if(value != _MaxValue)
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
                if(value != _MinValue)
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
                if(value != _AvgValue)
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

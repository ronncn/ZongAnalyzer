using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using USBLib;

namespace AnalyzerLogic
{
    public class DataHandling : INotifyPropertyChanged
    {
        USBMethod usb;
        Timer timer;
        private bool _usbState;
        //usb数据处理
        public static List<byte> Data;
        private int speed = 60;//采集速度
        public bool UsbState
        {
            get { return _usbState; }
            set
            {
                if (value != _usbState)
                {
                    _usbState = value;
                    OnPropertyChanged("UsbState");
                }
            }
        }

        //初始化函数
        public DataHandling()
        {
            usb = new USBMethod(0x0079, 0x0006);
            usb.OnEnabledChange += new USBMethod.enabledChange(UsbEnabled_Change);
            timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(Timer_Tick);
            timer.Interval = 1000 / speed;

        }

        /// <summary>
        /// 数据采集速度,默认 60 Sa/s
        /// </summary>
        public int Speed
        {
            get { return speed; }
            set
            {
                if (value != speed)
                {
                    speed = value;
                    timer.Interval = (1000 / speed);
                }
            }
        }

        public double Refer = 3.3d;

        private void UsbEnabled_Change(object sender, EventArgs e)
        {
            this.UsbState = usb.enabled;
            if (usb.enabled)
            {
                //生成信道
                if (ChannelManager.Signals.Count == 0)
                {
                    for(int i = 0; i < 8; i++)
                    {
                        ChannelManager.AddSignal();
                    }
                }
                //开始数据处理的定时器.

                flag = true;
                Task.Run(() =>
                {
                    this.Read();
                });
                //timer.Start();
            }
            else
            {
                flag = false;
                //timer.Stop();
            }
        }

        //int i = 0;
        private void Timer_Tick(object sender, ElapsedEventArgs e)
        {
            if (usb.ReadData())
            {
                Data = usb.RecvBuffer.ToList();
                ChannelManager.Update(Data, Refer);
            }
        }

        bool flag = false;
        private void Read()
        {
            while (flag)
            {
                if (usb.ReadData())
                {
                    Data = usb.RecvBuffer.ToList();
                    ChannelManager.Update(Data, Refer);
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

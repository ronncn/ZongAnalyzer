using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzerLogic
{
    //信道
    public class Signal : INotifyPropertyChanged
    {
        public int Id;                     //信道id
        public string _Name;
        private int _Init_Value;
        private double _Value;

        public string Name
        {
            get { return _Name; }
            set
            {
                if(value != _Name)
                {
                    _Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        public int Init_Value
        {
            get { return _Init_Value; }
            set
            {
                if (_Init_Value != value)
                {
                    _Init_Value = value;
                    OnPropertyChanged("Init_Value");
                    if (Init_ValueChanged != null)
                    {
                        Init_ValueChanged();
                    }
                }
            }
        }
        public double Value
        {
            get { return _Value; }
            set
            {
                if(value != _Value)
                {
                    _Value = value;
                    if (ValueChanged != null)
                    {
                        ValueChanged();
                    }
                }
            }
        }
        public List<double> Values = new List<double>();    //初始化值
        public List<int> Init_Values = new List<int>();
        //public double Contrast_Value;       //参考值
        //public int Acquisition_Rate;        //采集率(sa/s)

        public Signal()
        {
            //Contrast_Value = 3.3d;
            //Acquisition_Rate = 60;
        }

        public delegate void ChangedEventHandler();         //定义委托
        public event ChangedEventHandler ValueChanged;      //定义事件

        public event ChangedEventHandler Init_ValueChanged;  //定义事件


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

    //通道
    public class Channel : INotifyPropertyChanged
    {
        private int _Id;
        private string _Name;
        private string _From;
        private List<double> _Values;
        private string _Color;                //通道颜色,定义16进制代码
        private int _Init_Value;
        private double _Value;
        private Signal signal;

        public Channel(Signal si)
        {
            _Values = new List<double>();
            this.signal = si;
            this.signal.ValueChanged += new Signal.ChangedEventHandler(Value_Changed);
            this.signal.Init_ValueChanged += new Signal.ChangedEventHandler(Init_Value_Changed);
                 
        }

        private void Value_Changed()
        {
            this.Value = this.signal.Value;
        }

        private void Init_Value_Changed()
        {
            this.Init_Value = this.signal.Init_Value;
        }

        public Channel()
        {
            _Values = new List<double>();
        }

        /// <summary>
        /// 绑定信道
        /// </summary>
        public void BindSignal(Signal si)
        {
            this.signal = si;
            this.signal.ValueChanged += new Signal.ChangedEventHandler(Value_Changed);
            this.signal.Init_ValueChanged += new Signal.ChangedEventHandler(Init_Value_Changed);
        }

        #region 公开变量

        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        
        public string From
        {
            get { return _From; }
            set { _From = value; }
        }

        public List<double> Values
        {
            get { return _Values; }
            set { _Values = value; }
        }

        public int Init_Value
        {
            get{ return _Init_Value;}
            set
            {
                if(value != _Init_Value)
                {
                    _Init_Value = value;
                    OnPropertyChanged("Init_Value");
                }
            }
        }

        public string Color
        {
            get { return _Color; }
            set { _Color = value; }
        }

        public double Value
        {
            get { return _Value; }
            set
            {
                if(value != _Value)
                {
                    _Value = value;
                    OnPropertyChanged("Value");
                    if (ValueChanged != null)
                    {
                        ValueChanged();
                    }
                }
            }
        }
        
        public delegate void ChangedEventHandler();         //定义委托
        public event ChangedEventHandler ValueChanged;      //定义事件

        
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}

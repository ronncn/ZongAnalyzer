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
    public class Signal
    {
        public int Id;                     //信道id
        private double _Value;
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
        public List<double> Init_Values = new List<double>();    //初始化值
        //public double Contrast_Value;       //参考值
        //public int Acquisition_Rate;        //采集率(sa/s)

        public Signal()
        {
            //Contrast_Value = 3.3d;
            //Acquisition_Rate = 60;
        }

        public delegate void ChangedEventHandler();         //定义委托
        public event ChangedEventHandler ValueChanged;      //定义事件
    }

    //通道
    public class Channel
    {
        private int _Id;
        private string _Name;
        private List<double> _Init_Values;
        private string _Color;                //通道颜色,定义16进制代码
        private double _Value;
        private Signal signal;

        public Channel(Signal si)
        {
            _Init_Values = new List<double>();
            this.signal = si;
            this.signal.ValueChanged += new Signal.ChangedEventHandler(Value_Changed);
        }

        private void Value_Changed()
        {
            this.Value = this.signal.Value;
        }

        public Channel()
        {
            _Init_Values = new List<double>();
        }

        /// <summary>
        /// 绑定信道
        /// </summary>
        public void BindSignal(Signal si)
        {
            this.signal = si;
            this.signal.ValueChanged += new Signal.ChangedEventHandler(Value_Changed);
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

        public List<double> Init_Values
        {
            get { return _Init_Values; }
            set { _Init_Values = value; }
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
                    if (ValueChanged != null)
                    {
                        ValueChanged();
                    }
                }
            }
        }
        
        public delegate void ChangedEventHandler();         //定义委托
        public event ChangedEventHandler ValueChanged;      //定义事件

        #endregion
    }
}

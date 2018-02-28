using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AnalyzerLogic
{
    /// <summary>
    /// 时间计时器,具有暂停,开始功能
    /// </summary>
    public class TimeClock : INotifyPropertyChanged
    {
        public TimeClock()
        {
            //初始化构造函数
            timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Interval = 1;
            timer.Enabled = true;
        }

        private int t = 0;//总时间数(毫秒数)
        private string output;//输出格式化时间

        private Timer timer;

        public string Output
        {
            get { return output; }
            set
            {
                if (value != output)
                {
                    output = value;
                    OnPropertyChanged("Output");
                }
            }
        }

        public void Clock_Switch()
        {
            timer.Enabled = !timer.Enabled;
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            t++;
            Output = GetOutput(t);
        }

        private string GetOutput(int time)
        {
            string hh, mm, ss, fff;
            int f = time % 100; //毫秒
            int s = time / 100; // 转化为秒
            int m = s / 60;     // 分
            int h = m / 60;     // 时
            s = s % 60;         // 秒 
                                //毫秒格式00
            if (f < 10)
            {
                fff = "0" + f.ToString();
            }
            else
            {
                fff = f.ToString();
            }

            //秒格式00
            if (s < 10)
            {
                ss = "0" + s.ToString();
            }
            else
            {
                ss = s.ToString();
            }

            //分格式00
            if (m < 10)
            {
                mm = "0" + m.ToString();
            }
            else
            {
                mm = m.ToString();
            }

            //时格式00
            if (h < 10)
            {
                hh = "0" + h.ToString();
            }
            else
            {
                hh = h.ToString();
            }

            //返回 hh:mm:ss.ff            
            return hh + ":" + mm + ":" + ss + "." + fff;
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

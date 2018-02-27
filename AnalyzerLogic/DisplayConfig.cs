using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzerLogic
{
    public class DisplayConfig
    {
        private int unit_x = 100;
        public int Unit_X
        {
            get { return unit_x; }
            set
            {
                if(value != unit_x)
                {
                    if(value <= 0)
                    {
                        value = 10;
                    }
                    else if(value > 50000)
                    {
                        value = 50000;
                    }
                    unit_x = value;
                    OnPropertyChanged("Unit_X");
                }
            }
        }

        private int unit_y = 1;
        public int Unit_Y
        {
            get { return unit_y; }
            set
            {
                if(value != unit_y)
                {
                    if(value <= 0)
                    {
                        value = 1;
                    }
                    else if(value > 100)
                    {
                        value = 100;
                    }
                    unit_y = value;
                    OnPropertyChanged("Unit_Y");
                }
            }
        }

        private int offset_x = 0;
        public int Offset_X
        {
            get { return offset_x; }
            set
            {
                if(value != offset_x)
                {
                    offset_x = value;
                    OnPropertyChanged("Offset_X");
                }
            }
        }

        private int offset_y = 0;
        public int Offset_Y
        {
            get { return offset_y; }
            set
            {
                if(value != offset_y)
                {
                    offset_y = value;
                    OnPropertyChanged("Offset_Y");
                }
            }
        }

        private int fps = 60;
        public int Fps
        {
            get { return fps; }
            set
            {
                if(value != fps)
                {
                    fps = value;
                    OnPropertyChanged("Fps");
                }
            }
        }


        //自定义属性改变事件
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

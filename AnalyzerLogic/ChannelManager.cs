using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzerLogic
{
    public class ChannelManager
    {
        /*
         * 准备一个颜色库,按照颜色库配置通道颜色,颜色库为十六进制代码
         */
        private static string[] libColor = { "#f29248", "#339933", "#fe797a", "#4adee8", "#87abdb", "#c591c2", "#a5937b", "#add597", "#d71345" };
        
        public static ObservableCollection<Signal> Signals = new ObservableCollection<Signal>();
        public static ObservableCollection<Channel> Channels = new ObservableCollection<Channel>();
        
        /// <summary>
        /// 增加信道
        /// </summary>
        public static void AddSignal()
        {
            int id = Signals.Count == 0 ? 0 : Signals.Last<Signal>().Id + 1;
            Signal signal = new Signal();
            signal.Id = id;
            Signals.Add(signal);
            AddChannel(signal);
        }
        
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="list">传入数据</param>
        /// <param name="refer">参考数值(参考电压)</param>
        public static void Update(List<byte> list, double refer)
        {
            if (list == null) return;
            byte[] b = list.ToArray();
            foreach (Signal signal in Signals)
            {
                int i = signal.Id;
                if (i == 6)
                {
                    //SDA
                    signal.Values.Add((float)b[0x10] / 85);
                    signal.Init_Value = b[0x10];
                    signal.Value = (float)b[0x10] / 85;
                }
                else if (i == 7)
                {
                    //SCL
                    signal.Values.Add((float)b[0x11] / 85);
                    signal.Init_Value = b[0x11];
                    signal.Value = (float)b[0x11] / 85;
                }
                else
                {
                    signal.Values.Add((float)b[i] / 85);
                    signal.Init_Value = b[i];
                    signal.Value = (float)b[i] / 85;
                }

                //设置缓存清除数据
                if (signal.Values.Count >= 20000)
                {
                    //signal.Init_Values.RemoveRange(0, 5000);
                }
            }
        }

        /// <summary>
        /// 增加通道
        /// </summary>
        public static void AddChannel()
        {
            int id = Channels.Count == 0 ? 0 : Channels.Last<Channel>().Id + 1;
            Channel channel = new Channel();
            channel.Id = id;
            channel.Name = "通道" + (id + 1);
            int colorid = id >= 9 ? id % 9 : id;
            channel.Color = libColor[colorid];
            Channels.Add(channel);
        }

        /// <summary>
        /// 基于信道增加通道
        /// </summary>
        /// <param name="signal"></param>
        public static void AddChannel(Signal signal)
        {
            int id = Channels.Count == 0 ? 0 : Channels.Last<Channel>().Id + 1;
            Channel channel = new Channel(signal);
            channel.Id = id;
            channel.Name = "通道" + (id + 1);
            int colorid = id >= 9 ? id % 9 : id;
            channel.Color = libColor[colorid];
            channel.Values = signal.Values;
            channel.Value = signal.Value;
            channel.Init_Value = signal.Init_Value;
            Channels.Add(channel);
        }
    }
}

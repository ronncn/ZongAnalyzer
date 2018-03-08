using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBLib
{
    public class USBMethod
    {
        //定义连接状态改变事件
        public delegate void enabledChange(object sender, EventArgs e);
        public event enabledChange OnEnabledChange;

        public string debug;
        private USBDriver hid;
        private int hid_id;
        private bool _enabled;
        public bool enabled {
            get { return _enabled; }
            set
            {
                if (value != _enabled)
                {
                    _enabled = value;
                    OnEnabledChange(this, new EventArgs());
                }
            }
        }
        private bool trying_connect;
        private int VID;
        private int PID;
        public ulong Len;
        public byte[] SendBuffer;
        public byte[] RecvBuffer;

        public USBMethod(int VID, int PID)
        {
            hid = new USBDriver();
            this.VID = VID;
            this.PID = PID;
            hid_id = -1;
            _enabled = false;
            trying_connect = false;
            Len = 0;
            RecvBuffer = new byte[64];
            SendBuffer = new byte[64];
            KeepConnect();
        }

        private Task<bool> TryConnect()
        {
            return Task.Run(() =>
            {
                bool en = false;
                long ts_keep_last = DateTime.UtcNow.Ticks;
                while (!en)
                {
                    if (DateTime.UtcNow.Ticks - ts_keep_last > 10000000)
                    {
                        ts_keep_last = DateTime.UtcNow.Ticks;
                        hid.Refresh();
                        hid_id = hid.Connect(VID, PID);
                        en = (hid_id >= 0) ? true : false;
                        debug = en ? "连接成功" : "连接失败";
                    }
                }
                return en;
            });
        }

        private async void KeepConnect()
        {
            if (!enabled && !trying_connect)
            {
                debug = "尝试连接中";
                trying_connect = true;
                enabled = await TryConnect();
                trying_connect = false;
            }
        }


        private bool SetData()
        {
            if (enabled)
            {
                if (hid.CanWrite)
                {
                    hid.Send(hid_id, SendBuffer);
                    return true;
                }
            }
            return false;
        }

        public bool ReadData()
        {
            if (enabled && hid.CanRead)
            {
                //开接收线程
                hid.Recv(hid_id, RecvBuffer);
                return true;
            }
            return false;
        }
    }
}

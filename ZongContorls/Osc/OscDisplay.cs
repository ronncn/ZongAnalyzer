using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ZongContorls.Osc
{
    public partial class OscDisplay: UserControl, INotifyPropertyChanged
    {
        //初始化函数
        public OscDisplay()
        {
            InitializeComponent();
            InitializeTimer();
        }

        //初始化定时器
        private void InitializeTimer()
        {
            /*
             * timer1是控制绘制到画面,重绘画板
             * timer2是缓冲画布,循环不断绘制新的波形(不用于显示)
             */
            timer1.Enabled = false;
            timer1.Interval = 10;
            timer2.Enabled = false;
            timer2.Interval = 10;
        }
        #region 私有属性
                //通道

                private ObservableCollection<ChannelItem> listCH;

                //画布

                private Bitmap backBitmap;                  //背景图片
                private bool isOnPaint = false;             //重绘画布的标记
                private bool isOnBackPaint = false;         //重绘背景的标记
                private bool isShowGrid = true;             //是否显示网格
                private int gridStep = 50;                  //网格步宽

                //数据缓存

                Bitmap bitmap1;
                Bitmap bitmap2;
                Bitmap bitmap3;
                Bitmap bitmapTemp;                          //绘制分析的画布
                Bitmap bitmapWave;                          //绘制波形的画布
                int cursor = 1;                             //游标
                int pointer;                                //指针
                int bmpLock;                                //画布锁

                //属性数据

                private double max;                         //最大值
                private double min;                         //最小值
                private double avg;                         //平均值

                //工具

                private bool isPrintScreen = false;         //是否打印屏幕
        #endregion


        #region 公有属性

            public Point ZeroPoint = new Point(0, 390);

            /// <summary>
            /// 是否显示网格
            /// </summary>
            public bool IsShowGrid
            {
                get { return isShowGrid; }
                set
                {
                    if (value != isShowGrid)
                    {
                        isShowGrid = value;
                        isOnBackPaint = true;
                        base.Invalidate();
                    }
                }
            }

            /// <summary>
            /// 暂停/开始刷新
            /// </summary>
            public bool Switch
            {
                get { return timer1.Enabled; }
                set
                {
                    if (value != timer1.Enabled)
                    {
                        timer1.Enabled = value;
                        OnPropertyChanged("Switch");
                    }
                }
            }

            /// <summary>
            /// 开始后台绘制采集点(后台画)
            /// </summary>
            public bool IsDraw
            {
                get { return timer2.Enabled; }
                set { timer2.Enabled = value; }
            }

            private bool isAnalysis = false;
            /// <summary>
            /// 是否开启分析工具
            /// </summary>
            public bool IsAnalysis
            {
                get { return isAnalysis; }
                set
                {
                    if (value != isAnalysis)
                    {
                        isAnalysis = value;
                        if (isAnalysis)
                            isShowFollow = true;
                    }
                }
            }

            //网格代表的y轴单位
            private int unit_y = 1;
            public int Unit_Y
            {
                get { return unit_y; }
                set
                {
                    if (value != unit_y)
                    {
                        unit_y = value;
                        OnPropertyChanged("Unit_Y");
                        base.Invalidate();
                    }
                }
            }

            //网格代表的x轴单位
            private int unit_x = 500;                   //初始化500毫秒/格子
            public int Unit_X
            {
                get { return unit_x; }
                set
                {
                    if (value != unit_x)
                    {
                        unit_x = value;
                        OnPropertyChanged("Unit_X");
                        base.Invalidate();
                    }
                }
            }

            //刷新FPS
            public int PaintFPS
            {
                get { return 1000 / timer1.Interval; }
                set
                {
                    timer1.Interval = 1000 / value;
                }
            }

            public bool IsPrintScreen
            {
                get { return isPrintScreen; }
                set
                {
                    if (value != isPrintScreen)
                    {
                        isPrintScreen = value;
                        base.Invalidate();
                    }
                }
            }
        
        #endregion


        #region 重绘方法

        protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                //如果开启刷新,方法
                if (bitmap1 == null || isOnPaint)
                {
                    bitmap1 = new Bitmap(this.Width, this.Height);
                    bitmap2 = (Bitmap)bitmap1.Clone();
                    bitmap3 = (Bitmap)bitmap1.Clone();
                    bitmapTemp = (Bitmap)bitmap1.Clone();
                    bitmapWave = (Bitmap)bitmap1.Clone();
                    isOnPaint = false;
                }
                e.Graphics.DrawImage(bitmapWave, 0, 0);
                if (IsAnalysis)
                {
                    e.Graphics.DrawImage(bitmapTemp, 0, 0);
                }
                if (isPrintScreen)
                {
                    e.Graphics.DrawImage(bitmapTemp, 0, 0);
                }
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                base.OnPaintBackground(e);
                //判断背景是否重绘
                if (isOnBackPaint || backBitmap == null)
                {
                    backBitmap = new Bitmap(this.Width, this.Height);
                    Graphics g = Graphics.FromImage(backBitmap);
                    if (ZeroPoint == null)
                    {
                        this.ZeroPoint = new Point(0, this.Height / 2);//设置初始坐标原点
                    }
                    g.TranslateTransform(ZeroPoint.X, ZeroPoint.Y);//更改原点坐标位置
                    g.Clear(this.BackColor);//背景颜色
                    //判断是否打开显示网格标记
                    if (isShowGrid)
                    {
                        Pen pen = new Pen(Color.LightGray, 1);

                        //绘制竖线
                        for (int i = (int)Math.Floor((double)(-this.ZeroPoint.X / gridStep)) * gridStep; i < this.Width - this.ZeroPoint.X; i += gridStep)
                        {
                            g.DrawLine(pen, new Point(i, -this.ZeroPoint.Y), new Point(i, this.Height - this.ZeroPoint.Y));
                        }

                        //绘制横线

                        for (int i = (int)Math.Floor((double)(-this.ZeroPoint.Y / gridStep)) * gridStep; i < this.Height - this.ZeroPoint.Y; i += gridStep)
                        {
                            g.DrawLine(pen, new Point(-this.ZeroPoint.X, i), new Point(this.Width - this.ZeroPoint.X, i));
                        }

                        pen.Dispose();//画笔注销
                    }
                    g.Dispose();//graphice注销
                    isOnBackPaint = false;//关闭标记,禁止重绘
                }
                e.Graphics.DrawImage(backBitmap, 0, 0);//将backBitmap绘制到显示上
            }

            /// <summary>
            /// 尺寸发生改变,重绘背景
            /// </summary>
            /// <param name="e"></param>
            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                isOnBackPaint = true;
                isOnPaint = true;
                base.Invalidate();
            }
            //重写鼠标按下事件
            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);
                //判断分析工具是否开启
                if (IsAnalysis)
                {
                    this.Switch = false;            //暂停画面
                }
            }

            private bool isShowFollow = false;
            //重写鼠标移动事件
            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                if (isShowFollow)
                {
                    this.DrawMouseFollow(e.Location);
                }
            }

            private bool isMouseEnter = false;
            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                isMouseEnter = true;
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                Graphics g = Graphics.FromImage(bitmapTemp);
                g.Clear(Color.Transparent);
                g.Dispose();
                base.Invalidate();
            }


        #endregion


        #region 公开方法
            /// <summary>
            /// 关联通道集合
            /// </summary>
            /// <param name="observableCollection"></param>
            public void BindChannels(ObservableCollection<ChannelItem> observableCollection)
            {
                this.listCH = observableCollection;
            }

            Timer timer3;

            /// <summary>
            ///截取快照
            /// </summary>
            public void PrintScreen()
            {
                //保存快照到目录下
                Bitmap bmp = new Bitmap(bitmapWave.Width + 20, bitmapWave.Height + 20);
                Graphics g = Graphics.FromImage(bmp);
                g.Clear(this.BackColor);
                g.DrawImage(backBitmap, 20, 0);
                g.DrawImage(bitmapWave, 20, 0);
                g.TranslateTransform(bmp.Width, (bmp.Height - 20) / 2);
                int x = 0;
                g.DrawString(x.ToString(), new Font("Arial", 10), Brushes.Gray, new Point(-10, (bmp.Height - 20) / 2 + 2));
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center; //居中
                stringFormat.Alignment = StringAlignment.Far; //右对齐
                g.DrawString("单位:(V/ms)", new Font("Arial", 10), Brushes.Gray, new Point(-10, (bmp.Height - 20) / 2 - 20), stringFormat);
                for (int i = -gridStep; i >= -bmp.Width; i -= gridStep)
                {
                    x -= Unit_X;
                    int off = x.ToString().Length * 4;
                    g.DrawString(x.ToString(), new Font("Arial", 10), Brushes.Gray, new Point(i - off, (bmp.Height - 20) / 2 + 2));
                }
                int y = 0;
                g.DrawString(y.ToString(), new Font("Arial", 10), Brushes.Gray, new Point(-bmp.Width + 6, -8));
                for (int i = gridStep; i < (bmp.Height - 20) / 2; i += gridStep)
                {
                    y -= Unit_Y;
                    g.DrawString(y.ToString(), new Font("Arial", 10), Brushes.Gray, new Point(-bmp.Width + 4, i - 8));
                }
                y = 0;
                for (int i = -gridStep; i > -(bmp.Height - 20) / 2; i -= gridStep)
                {
                    y += Unit_Y;
                    g.DrawString(y.ToString(), new Font("Arial", 10), Brushes.Gray, new Point(-bmp.Width + 6, i - 8));
                }
                g.Dispose();
                string date = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                string path = Directory.GetCurrentDirectory() + "/快照";
                if (!Directory.Exists(path))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(path);
                    directoryInfo.Create();
                }
                bmp.Save(path + "/" + date + ".jpg");
                bmp.Dispose();

                //闪屏
                timer3 = new Timer();
                timer3.Interval = 50;
                timer3.Tick += new EventHandler(Timer3_Tick);
                timer3.Start();
            }

        #endregion
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.DrawBitmap();
            base.Invalidate();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            this.DrawTemp();
        }

        /// <summary>
        /// 绘制到画布上
        /// </summary>
        private void DrawBitmap()
        {
            if (bitmapWave == null) return;
            Graphics gWave = Graphics.FromImage(bitmapWave);
            gWave.Clear(Color.Transparent);
            switch (pointer)
            {
                case 1:
                    bmpLock = 1;
                    gWave.DrawImage(bitmap1, 0, 0);
                    break;
                case 2:
                    bmpLock = 2;
                    gWave.DrawImage(bitmap2, 0, 0);
                    break;
                case 3:
                    bmpLock = 3;
                    gWave.DrawImage(bitmap3, 0, 0);
                    break;
            }
            gWave.Dispose();
        }

        public void DrawTemp()
        {
            if (cursor == bmpLock)
            {
                if (cursor >= 3)
                    cursor = 1;
                else
                    cursor++;
            }
            Graphics gBuffer;
            switch (cursor)
            {
                case 1:
                    gBuffer = Graphics.FromImage(bitmap1);
                    pointer = 1;
                    cursor = 2;
                    break;
                case 2:
                    gBuffer = Graphics.FromImage(bitmap2);
                    pointer = 2;
                    cursor = 3;
                    break;
                case 3:
                    gBuffer = Graphics.FromImage(bitmap3);
                    pointer = 3;
                    cursor = 1;
                    break;
                default:
                    gBuffer = Graphics.FromImage(bitmap1);
                    break;
            }
            gBuffer.TranslateTransform(bitmap1.Width, bitmap1.Height / 2);
            gBuffer.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.DrawChan(gBuffer);
        }
        
        int count = 0;
        /// <summary>
        /// 遍历通道,绘制到graphics
        /// </summary>
        /// <param name="g"></param>
        public void DrawChan(Graphics g)
        {
            g.Clear(Color.Transparent);
            if (listCH.Count == 0) { return; }

            foreach (ChannelItem ch in listCH)
            {
                max = 0;
                min = 1000000;
                avg = 0;
                if (count != 0)
                {
                    Color color = System.Drawing.ColorTranslator.FromHtml(ch.Color);
                    Pen pen = new Pen(color, 1);
                    int num = ch.Init_Values.Count - (500 + 5) * this.Width / gridStep * unit_x / 1000 < 0 ? 0 : ch.Init_Values.Count - (500 + 5) * this.Width / gridStep * unit_x / 1000;
                    int c = 0;
                    for (int i = ch.Init_Values.Count - 1; i > num; i--)
                    {
                        if (max < ch.Init_Values[i])
                            max = ch.Init_Values[i];
                        if(min > ch.Init_Values[i])
                            min = ch.Init_Values[i];
                        avg += ch.Init_Values[i];
                        g.DrawLine(pen, (float)(c * gridStep / unit_x), (float)-ch.Init_Values[i] * gridStep / unit_y - (float)ch.Offset_Y,
                            (float)((c - 1000 / 60) * gridStep / unit_x), (float)-(ch.Init_Values[i - 1]) * gridStep / unit_y - (float)ch.Offset_Y);
                        c -= 1000 / 60;
                    }

                    ch.MaxValue = max;
                    ch.MinValue = min;
                    ch.AvgValue = avg / (ch.Init_Values.Count - 1 - 1 - num);
                    pen.Dispose();
                }
                count = ch.Init_Values.Count();
            }
        }
        
        /// <summary>
        /// 绘制鼠标跟随
        /// </summary>
        /// <param name="pt"></param>
        private void DrawMouseFollow(Point pt)
        {
            if (!Switch)
            {
                //鼠标跟随信息,显示在bitmapTemp的临时画布上
                Graphics gFollow = Graphics.FromImage(bitmapTemp);
                gFollow.Clear(Color.Transparent);
                //绘制水平参考线
                gFollow.DrawLine(Pens.Red, new Point(0, pt.Y), new Point(this.Width, pt.Y));
                //绘制垂直参考线
                gFollow.DrawLine(Pens.Red, new Point(pt.X, 0), new Point(pt.X, this.Height));
                string v = (((float)Unit_Y / (float)gridStep) * (this.Height / 2 - pt.Y)).ToString();
                string ms = (Unit_X / gridStep * -(this.Width - pt.X)).ToString();
                gFollow.DrawString("("+v+"V,"+ms+"ms)", new Font("宋体", 10.5F), Brushes.Red, new Point(pt.X + 5,pt.Y - 20));
                gFollow.Dispose();
                base.Invalidate();
            }
        }


        #region 快照截图功能
        /// <summary>
        /// 快照截图功能,使用定时器,切换黑白背景实现闪屏效果.
        /// </summary>
        int index = 0;
        private void Timer3_Tick(object sender,EventArgs e)
        {
            switch (index)
            {
                case 0:
                    DrawBlack();
                    index++;
                    break;
                case 1:
                    DrawWhite();
                    index = 0;
                    break;
            }
            base.Invalidate();
        }

        private void DrawBlack()
        {
            Graphics gTemp = Graphics.FromImage(bitmapTemp);
            gTemp.Clear(Color.Gray);
            gTemp.Dispose();
            IsPrintScreen = true;
        }
        private void DrawWhite()
        {
            Graphics gTemp = Graphics.FromImage(bitmapTemp);
            gTemp.Clear(Color.Transparent);
            gTemp.Dispose();
            IsPrintScreen = false;
            timer3.Stop();
        }
        #endregion


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

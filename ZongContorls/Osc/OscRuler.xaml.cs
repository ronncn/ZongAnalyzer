using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace ZongContorls.Osc
{
    public enum Axes { Vertical, Horizontal}
    /// <summary>
    /// OscRuler.xaml 的交互逻辑
    /// </summary>
    public partial class OscRuler : UserControl
    {
        public OscRuler()
        {
            InitializeComponent();
        }
        //设置默认方向
        public Axes RulerAxes = Axes.Horizontal;

        //设置起点值
        public double Zero = 0d;

        //设置偏移量
        public double Offset = 0d;

        //设置div像素
        public int Interval = 50;

        //设置单位名字
        public string Unit_Name = "ms";

        //设置单位值
        private double _Unit_Value = 10d;
        public double Unit_Value
        {
            get { return _Unit_Value; }
            set
            {
                if(value != _Unit_Value)
                {
                    _Unit_Value = value;
                    base.InvalidateVisual();
                }
            }
        }
        private RenderTargetBitmap renderBitmap;
        private bool SizeChangeFlag = false;

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (renderBitmap == null || SizeChangeFlag)
            {
                renderBitmap = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                SizeChangeFlag = false;
            }
            base.OnRender(drawingContext);
            this.DrawBitmap();
            drawingContext.DrawImage(renderBitmap,new Rect(new Point(0,0), new Size(this.ActualWidth,this.ActualHeight)));
        }

        private void DrawBitmap()
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            DrawingContext drawingContext = drawingVisual.RenderOpen();

            Pen pen = new Pen(Brushes.Black, 0.1);
            pen.Freeze();  //冻结画笔，这样能加快绘图速度
            switch (RulerAxes)
            {
                case Axes.Horizontal:
                    for (double i = this.Offset; i < this.ActualWidth + this.Offset; i+=10)
                    {
                        if (i % 100 == 0)
                        {
                            drawingContext.DrawLine(pen, new Point(i - this.Offset, this.ActualHeight - 10), new Point(i - this.Offset, this.ActualHeight));
                            drawingContext.DrawLine(pen, new Point(i - this.Offset, this.ActualHeight - 10), new Point(i - this.Offset, this.ActualHeight));
                            drawingContext.DrawLine(pen, new Point(i - this.Offset, this.ActualHeight - 10), new Point(i - this.Offset, this.ActualHeight));
                            string str = "";
                            str = Convert.ToString(this.Zero + (this.Unit_Value * (i / 10))) + Unit_Name;
                            if (this.Zero + (this.Unit_Value * (i / 10)) == 0)
                            {
                                str = Convert.ToString(this.Zero + (this.Unit_Value * (i / 10)));
                            }
                            FormattedText text = new FormattedText(str,
                                    CultureInfo.GetCultureInfo("en-us"),
                                    FlowDirection.LeftToRight,
                                    new Typeface("Arial"),
                                    12, Brushes.Gray);
                            double offset = str.Length * 3;
                            drawingContext.DrawText(text, new Point(i - this.Offset - offset, this.ActualHeight - 25));
                        }
                        else
                        {
                            drawingContext.DrawLine(pen, new Point(i - this.Offset, this.ActualHeight - 5), new Point(i - this.Offset, this.ActualHeight));
                            drawingContext.DrawLine(pen, new Point(i - this.Offset, this.ActualHeight - 5), new Point(i - this.Offset, this.ActualHeight));
                            drawingContext.DrawLine(pen, new Point(i - this.Offset, this.ActualHeight - 5), new Point(i - this.Offset, this.ActualHeight));
                        }
                    }
                    break;
                case Axes.Vertical:
                    for (double i = this.Offset; i < this.ActualHeight + this.Offset; i += 10)
                    {
                        if (i % 100 == 0)
                        {
                            drawingContext.DrawLine(pen, new Point(this.ActualWidth - 10, i - this.Offset), new Point(this.ActualWidth, i - this.Offset));
                            drawingContext.DrawLine(pen, new Point(this.ActualWidth - 10, i - this.Offset), new Point(this.ActualWidth, i - this.Offset));
                            drawingContext.DrawLine(pen, new Point(this.ActualWidth - 10, i - this.Offset), new Point(this.ActualWidth, i - this.Offset));
                            string str = Convert.ToString(this.Zero + (this.Unit_Value * (-i / 10))) + Unit_Name;
                            FormattedText text = new FormattedText(str,
                                    CultureInfo.GetCultureInfo("en-us"),
                                    FlowDirection.LeftToRight,
                                    new Typeface("Arial"),
                                    12, Brushes.Gray);
                            text.TextAlignment = TextAlignment.Right;
                            double offset = str.Length * 3;
                            drawingContext.DrawText(text, new Point(this.ActualWidth - 15, i - this.Offset - 7));
                        }
                        else
                        {
                            drawingContext.DrawLine(pen, new Point(this.ActualWidth - 5, i - this.Offset), new Point(this.ActualWidth, i - this.Offset));
                            drawingContext.DrawLine(pen, new Point(this.ActualWidth - 5, i - this.Offset), new Point(this.ActualWidth, i - this.Offset));
                            drawingContext.DrawLine(pen, new Point(this.ActualWidth - 5, i - this.Offset), new Point(this.ActualWidth, i - this.Offset));
                        }
                    }
                    break;
                default:
                    break;
            }

            drawingContext.Close();
            renderBitmap.Clear();
            renderBitmap.Render(drawingVisual);
        }

        //当空间改变尺寸时候
        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //当尺寸改变的时候,改变标尺的方向
            if (this.ActualWidth >= this.ActualHeight)
            {
                this.RulerAxes = Axes.Horizontal;
            }
            else
            {
                this.RulerAxes = Axes.Vertical;
            }
            //重新绘制
            SizeChangeFlag = true;
            base.InvalidateVisual();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //this.Offset += 10;
            //base.InvalidateVisual();
        }
    }
}

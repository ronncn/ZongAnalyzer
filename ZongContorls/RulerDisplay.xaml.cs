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

namespace ZongContorls
{
    //标尺控件的方向
    public enum XY {X,Y }
    /// <summary>
    /// RulerDisplay.xaml 的交互逻辑
    /// </summary>
    public partial class RulerDisplay : UserControl
    {
        public XY RulerXY = XY.X;//默认水平方向

        public int Zero = 0;

        public int Interval = 50;

        private int _Unit_Value = 10;
        public int Unit_Value
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

        public string Unit_Name = "ms";
        
        public RulerDisplay()
        {
            InitializeComponent();
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //当尺寸改变的时候,改变标尺的方向
            if(this.ActualWidth >= this.ActualHeight)
            {
                this.RulerXY = XY.X;
            }
            else
            {
                this.RulerXY = XY.Y;
            }
            //重新绘制
            base.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            this.DrawBitmap(drawingContext);
        }

        int X_Value = 0;
        int offset;
        /// <summary>
        /// 绘制背景图片
        /// </summary>
        private void DrawBitmap(DrawingContext drawingContext)
        {
            switch (RulerXY)
            {
                case XY.X:
                    this.Zero = (int)this.ActualWidth;
                    X_Value = 0;
                    //绘制0点的位置
                    FormattedText textx = new FormattedText(Convert.ToString(X_Value),
                            CultureInfo.GetCultureInfo("en-us"),
                            FlowDirection.LeftToRight,
                            new Typeface("Arial"),
                            12, Brushes.Gray);
                    offset = X_Value.ToString().Length * 3;
                    drawingContext.DrawText(textx, new Point(Zero - offset, 5));

                    FormattedText textu = new FormattedText("(ms)",
                            CultureInfo.GetCultureInfo("en-us"),
                            FlowDirection.LeftToRight,
                            new Typeface("Arial"),
                            12, Brushes.Gray);
                    drawingContext.DrawText(textu, new Point(this.ActualWidth + 10, 5));

                    X_Value = 0; 
                    //绘制大于0点的位置
                    for (int i = Zero + Interval; i <= (int)this.ActualWidth; i += Interval)
                    {
                        X_Value += Unit_Value;
                        FormattedText text1 = new FormattedText(Convert.ToString(X_Value),
                            CultureInfo.GetCultureInfo("en-us"),
                            FlowDirection.LeftToRight,
                            new Typeface("Arial"),
                            12, Brushes.Gray);
                        offset = X_Value.ToString().Length * 3;
                        drawingContext.DrawText(text1, new Point(i - offset, 5));
                    }

                    X_Value = 0;
                    //绘制小于0点的位置
                    for(int i = Zero - Interval; i > 0; i -= Interval)
                    {
                        X_Value -= Unit_Value;
                        FormattedText text1 = new FormattedText(Convert.ToString(X_Value),
                            CultureInfo.GetCultureInfo("en-us"),
                            FlowDirection.LeftToRight,
                            new Typeface("Arial"),
                            12, Brushes.Gray);
                        offset = X_Value.ToString().Length * 3;
                        drawingContext.DrawText(text1, new Point(i - offset, 5));
                    }
                    
                    break;
                case XY.Y:
                    this.Zero = (int)this.ActualHeight / 2;
                    X_Value = 0;
                    //绘制0点的位置
                    FormattedText texty = new FormattedText(Convert.ToString(X_Value),
                            CultureInfo.GetCultureInfo("en-us"),
                            FlowDirection.LeftToRight,
                            new Typeface("Arial"),
                            12, Brushes.Gray);
                    offset = X_Value.ToString().Length * 2;
                    drawingContext.DrawText(texty, new Point(this.ActualWidth - 10 - offset, Zero-6));

                    FormattedText textv = new FormattedText("(V)",
                            CultureInfo.GetCultureInfo("en-us"),
                            FlowDirection.LeftToRight,
                            new Typeface("Arial"),
                            12, Brushes.Gray);
                    drawingContext.DrawText(textv, new Point(this.ActualWidth - 16, -15));
                    X_Value = 0;
                    //绘制大于0点的位置
                    for (int i = Zero + Interval; i <= (int)this.ActualHeight; i += Interval)
                    {
                        X_Value -= Unit_Value;
                        FormattedText text1 = new FormattedText(Convert.ToString(X_Value),
                            CultureInfo.GetCultureInfo("en-us"),
                            FlowDirection.LeftToRight,
                            new Typeface("Arial"),
                            12, Brushes.Gray);
                        offset = X_Value.ToString().Length * 4;
                        drawingContext.DrawText(text1, new Point(this.ActualWidth - 10 - offset, i-6));
                    }

                    X_Value = 0;
                    //绘制小于0点的位置
                    for (int i = Zero - Interval; i > 0; i -= Interval)
                    {
                        X_Value += Unit_Value;
                        FormattedText text1 = new FormattedText(Convert.ToString(X_Value),
                            CultureInfo.GetCultureInfo("en-us"),
                            FlowDirection.LeftToRight,
                            new Typeface("Arial"),
                            12, Brushes.Gray);
                        offset = X_Value.ToString().Length * 4;
                        drawingContext.DrawText(text1, new Point(this.ActualWidth - 10 - offset, i-6));
                    }
                    break;
                default:
                    break;
            }
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}

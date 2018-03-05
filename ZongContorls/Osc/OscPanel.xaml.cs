using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using AnalyzerLogic;

namespace ZongContorls.Osc
{
    /// <summary>
    /// OscPanel.xaml 的交互逻辑
    /// </summary>
    public partial class OscPanel : UserControl
    {
        public OscPanel()
        {
            InitializeComponent();
            InitializeRuler();//初始化标尺
            this.parameter.DataContext = display;
            /* 
            <local:OscSwiperBtn Canvas.Bottom="0" Canvas.Left="-10" Opacity="0.5"
                                PreviewMouseDown="SwiperBtn_MouseDown"
                                PreviewMouseMove="SwiperBtn_MouseMove"
                                PreviewMouseUp="SwiperBtn_MouseUp"/>
             */
            OscSwiperBtn oscSwiperBtn = new OscSwiperBtn("style_2");
            oscSwiperBtn.SetValue(Canvas.TopProperty, 0d);
            oscSwiperBtn.SetValue(Canvas.RightProperty, 0d);
            oscSwiperBtn.SetValue(OpacityProperty, 0.7d);
            oscSwiperBtn.SetValue(OscSwiperBtn.nameProperty,"1220");
            oscSwiperBtn.SetValue(OscSwiperBtn.colorProperty,"#ff0000");
            oscSwiperBtn.Color = "#ff0000";
            this.SwiperCanvas_2.Children.Add(oscSwiperBtn);
        }
        private SignalListBox _SignalListBox;
        public void SetSignalListBox(SignalListBox signalListBox)
        {
            this._SignalListBox = signalListBox;
            this.display.BindChannels(_SignalListBox.ChannelItems);
        }

        //初始化标尺
        private void InitializeRuler()
        {
            //x轴标尺
            ruler_x.Unit_Name = "ms";
            ruler_x.Unit_Value = display.Unit_X / 10;
            ruler_x.Zero = 0;
            ruler_x.Offset = 0d;

            //y轴标尺
            ruler_y.Unit_Name = "";
            ruler_y.Unit_Value = display.Unit_Y / 10;
            ruler_y.Zero = 0;
            ruler_y.Offset = 0;
        }

        //失去焦点
        private void grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            unFouce.Focus();
        }

        //双击textbox
        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.IsReadOnly = false;
            textBox.Cursor = Cursors.IBeam;
            textBox.Focus();
        }

        //失去焦点禁用
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Cursor = Cursors.SizeWE;
            textBox.IsReadOnly = true;
        }

        Point startPoint;
        double old;
        bool unit_flag = false;
        //单位时间鼠标左键按下
        private void unit_time_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //按下获取鼠标坐标位置
            TextBox textBox = (TextBox)sender;
            startPoint = e.GetPosition(this);
            old = Convert.ToDouble(textBox.Text);
            unit_flag = true;
        }

        private void unit_time_MouseMove(object sender, MouseEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (unit_flag && textBox.Cursor == Cursors.SizeWE)
            {
                Point endPoint = e.GetPosition(this);
                double move = endPoint.X - startPoint.X;
                double res = old + move;
                textBox.Text = res.ToString();
            }
        }

        private void unit_time_MouseUp(object sender, MouseButtonEventArgs e)
        {
            unit_flag = false;
        }

        private void unit_time_x_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            ruler_x.Unit_Value = Convert.ToDouble(textBox.Text)/10;
        }

        private void unit_time_y_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (Convert.ToDouble(textBox.Text) == 0)
            {
                textBox.Text = "1";
            }
            ruler_y.Unit_Value = Convert.ToDouble(textBox.Text) / 10;
        }

        //SwiperBtn
        private bool isMouseDownSwiperBtn = false;

        double start_y;
        double first;
        private void SwiperBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isMouseDownSwiperBtn = true;
            start_y = e.GetPosition(SwiperCanvas_1).X;
            OscSwiperBtn btn = (OscSwiperBtn)sender;
            first = (double)btn.GetValue(Canvas.LeftProperty);
        }

        double end_y;
        private void SwiperBtn_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDownSwiperBtn)
            {
                end_y = e.GetPosition(SwiperCanvas_1).X;
                OscSwiperBtn btn = (OscSwiperBtn)sender;
                double val = first + (end_y - start_y);
                if (val < -10) { val = -10; }
                if (val > SwiperCanvas_1.ActualWidth - 20) { val = SwiperCanvas_1.ActualWidth - 20; }
                btn.SetValue(Canvas.LeftProperty, val);
                e.Handled = true;
            }
        }

        private void SwiperBtn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isMouseDownSwiperBtn = false;
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double d = 0;
            if((ruler_y.ActualHeight/2) % 100 != 0)
            {
                d = ruler_y.ActualHeight / 2 + (100 - (ruler_y.ActualHeight / 2) % 100);
            }
            ruler_y.Offset = -d;
            display.ZeroPoint = new System.Drawing.Point((int)ruler_x.Offset, -(int)ruler_y.Offset);
        }
    }

}

using System;
using System.Collections.Generic;
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
    /// <summary>
    /// SwiperBtn.xaml 的交互逻辑
    /// </summary>
    public partial class SwiperBtn : UserControl
    {
        public SwiperBtn()
        {
            InitializeComponent();
            this.swiperBtn.DataContext = this;
        }


        public int Id
        {
            get { return (int)GetValue(idProperty); }
            set { SetValue(idProperty, value); }
        }

        // Using a DependencyProperty as the backing store for id.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty idProperty =
            DependencyProperty.Register("Id", typeof(int), typeof(SwiperBtn), new PropertyMetadata(0));


        public string Color
        {
            get { return (string)GetValue(colorProperty); }
            set { SetValue(colorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for data_value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty colorProperty =
            DependencyProperty.Register("Color", typeof(string), typeof(SwiperBtn), new PropertyMetadata("#000000"));


        public string Name
        {
            get { return (string)GetValue(nameProperty); }
            set { SetValue(nameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for data_value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty nameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(SwiperBtn), new PropertyMetadata("通道*"));
        
        /// <summary>
        /// 获得指定元素的父元素  
        /// </summary>  
        /// <typeparam name="T">指定页面元素</typeparam>  
        /// <param name="obj"></param>  
        /// <returns></returns>  
        public T GetParentObject<T>(DependencyObject obj) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);

            while (parent != null)
            {
                if (parent is T)
                {
                    return (T)parent;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
    }
}

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
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class ZongTab : UserControl
    {
        private bool isSaveFlag = true;//是否保存标记
        private double conventionWidth = 150;//初始宽度

        private bool isAnalysis;
        public bool IsAnalysis
        {
            get { return isAnalysis; }
            set
            {
                if (value != isAnalysis)
                {
                    isAnalysis = value;
                    foreach (TabItem item in tabControl.Items)
                    {
                        NewWindow win = (NewWindow)item.Content;
                        win.IsAnalysis = value;
                    }
                }
            }
        }

        public int ZongTabSelectedIndex
        {
            get { return tabControl.SelectedIndex; }
        }

        public object ZongTabSelectedItem
        {
            get { return tabControl.SelectedItem; }
        }

        public ZongTab()
        {
            InitializeComponent();
            foreach(TabItem item in tabControl.Items)
            {
                item.Width = conventionWidth;
            }
        }
        
        //关闭选项卡
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (isSaveFlag)
            {
                //获得button的父级tabitem
                TabItem tabItem = GetParentObject<TabItem>((DependencyObject)sender);
                this.CloseItem(tabItem);
            }
        }

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

        /// <summary>
        /// Load
        /// </summary>
        private void TabLoad()
        {
            int criticalCount = (int)(tabControl.ActualWidth / (conventionWidth + 5));
            //平均宽度
            double perWidth = tabControl.ActualWidth / tabControl.Items.Count;
            
            if (tabControl.Items.Count < criticalCount)
            {
                foreach (TabItem item in tabControl.Items)
                {
                    item.Width = conventionWidth;
                }
            }
            else
            {
                foreach (TabItem item in tabControl.Items)
                {
                    item.Width = perWidth - 5;
                }
            }
        }

        /// <summary>
        /// 添加选项卡
        /// </summary>
        public void AddItem()
        {
            TabItem item = new TabItem();
            item.Header = string.Format("窗口{0}", (tabControl.Items.Count + 1));
            tabControl.Items.Add(item);
            item.IsSelected = true;
            NewWindow nwin = new NewWindow();
            item.Content = nwin;

            //初始化标签长度
            TabLoad();
        }

        /// <summary>
        /// 关闭所有标签
        /// </summary>
        public void ClearItem()
        {
            foreach(TabItem item in tabControl.Items)
            {
                this.CloseItem(item);
            }
        }

        public void CloseItem(TabItem item)
        {
            //删除该标签
            tabControl.Items.Remove(item);
            TabLoad();
        }
    }
}

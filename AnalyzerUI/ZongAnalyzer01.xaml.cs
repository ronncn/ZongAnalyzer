using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using USBLib;
using AnalyzerLogic;
using ZongContorls;

namespace AnalyzerUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        DataHandling dataHandling;
        public MainWindow()
        {
            InitializeComponent();
            ListBox_Signal.ItemsSource = ChannelManager.Channels;
            dataHandling = new DataHandling();
            dataHandling.PropertyChanged += new PropertyChangedEventHandler(UsbState_PropertyChanged);
            this.InitTool();
        }

        /// <summary>
        /// 初始化工具箱
        /// </summary>
        private void InitTool()
        {
            this.ToolBar.DataContext = zongTab;
        }

        private void UsbState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (dataHandling.UsbState)
            {
                stateText.Text = "连接成功";
            }
            else
            {
                stateText.Text = "连接失败";
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            zongTab.AddItem();
        }

        private void ListBox_Signal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox_Signal.SelectedIndex == -1)
                return;
            var listbox = (ListBox)sender;
            System.Collections.IList items = (System.Collections.IList)listbox.SelectedItems;
            var collection = items.Cast<Channel>();
            var channels = collection.ToList();

            DragDrop.DoDragDrop(listbox, channels, DragDropEffects.Copy);
            ListBox_Signal.SelectedIndex = -1;
        }

        private void zongTab_Drop(object sender, DragEventArgs e)
        {
            if (zongTab.ZongTabSelectedIndex == -1)
                return;
            List<Channel> dropList = (List<Channel>)e.Data.GetData(typeof(List<Channel>));
            TabItem tab = (TabItem)zongTab.ZongTabSelectedItem;
            NewWindow win = (NewWindow)tab.Content;

            
        }

        /// <summary>
        /// 查找子控件
        /// </summary>
        /// <typeparam name="T">子控件的类型</typeparam>
        /// <param name="obj">要找的是obj的子控件</param>
        /// <param name="name">想找的子控件的Name属性</param>
        /// <returns>目标子控件</returns>
        public T GetChildObject<T>(DependencyObject obj) where T : FrameworkElement
        {
            DependencyObject child = null;

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T)
                {
                    return (T)child;
                }
            }
            return null;
        }
        
    }
}

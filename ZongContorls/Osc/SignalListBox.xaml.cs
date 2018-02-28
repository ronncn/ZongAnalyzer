using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// SignalListBox.xaml 的交互逻辑
    /// </summary>
    public partial class SignalListBox : UserControl
    {
        public SignalListBox()
        {
            InitializeComponent();
            listBox.ItemsSource = Channels;
        }
        
        public ObservableCollection<Channel> Channels;

        private Channel _SelectedChannelItem;
        public Channel SelectedChannelItem
        {
            get { return _SelectedChannelItem; }
            set
            {
                if (value != _SelectedChannelItem)
                {
                    _SelectedChannelItem = value;
                    ValueChanged();
                }
            }
        }

        public delegate void ChangedEventHandler();         //定义委托
        public event ChangedEventHandler ValueChanged;      //定义事件
        
        //关闭信道item
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem listItem = GetParentObject<ListBoxItem>((DependencyObject)sender);

            Channel c = (Channel)(listItem).DataContext;
            this.Channels.Remove(c);
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
        
        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedChannelItem = (Channel)listBox.SelectedItem;
        }

        //隐藏按钮,点击隐藏信道
        private void show_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem listItem = GetParentObject<ListBoxItem>((DependencyObject)sender);
            listItem.IsSelected = true;
            if(listItem.Background != Brushes.Gray)
            {
                listItem.Background = Brushes.Gray;
            }
            else
            {
                listItem.Background = listItem.BorderBrush;
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            AddChannelWindow addChannelWindow = new AddChannelWindow();
            addChannelWindow.Show();
        }
    }
}

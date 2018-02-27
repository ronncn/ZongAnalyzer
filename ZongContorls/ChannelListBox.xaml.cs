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

namespace ZongContorls
{
    /// <summary>
    /// ChannelListBox.xaml 的交互逻辑
    /// </summary>
    public partial class ChannelListBox : UserControl
    {
        public ObservableCollection<ChannelItem> Channels;

        private ChannelItem _SelectedChannelItem;
        public ChannelItem SelectedChannelItem
        {
            get { return _SelectedChannelItem; }
            set
            {
                if(value != _SelectedChannelItem)
                {
                    _SelectedChannelItem = value;
                    ValueChanged();
                }
            }
        }
        
        public delegate void ChangedEventHandler();         //定义委托
        public event ChangedEventHandler ValueChanged;      //定义事件

        public ChannelListBox()
        {
            InitializeComponent();
            //this.InitAddChannelButton();
        }

        public void BindChannels(ObservableCollection<ChannelItem> channels)
        {
            this.Channels = channels;
            listBox.ItemsSource = Channels;
        }

        public void InitAddChannelButton()
        {
            ListBoxItem item = new ListBoxItem();
            item.SetValue(ListBoxItem.StyleProperty, this.Resources["addChannelStyle"]);
            listBox.Items.Add(item);
        }
        
        /// <summary>
        /// 添加通道项
        /// </summary>
        public void AddChannelItem()
        {
            ListBoxItem listBoxItem = new ListBoxItem();

            listBox.Items.Add(listBoxItem);
        }

        public void CloseChannelItem(ListBoxItem item)
        {
            listBox.Items.Remove(item);
        }

        public void ClearChannelItem()
        {
            foreach(ListBoxItem item in listBox.Items)
            {
                this.CloseChannelItem(item);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem listItem = GetParentObject<ListBoxItem>((DependencyObject)sender);
            
            ChannelItem c = (ChannelItem)(listItem).DataContext;
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

        private void btn_AddChannel_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.RemoveAt(listBox.Items.Count - 1);
            this.AddChannelItem();
            this.InitAddChannelButton();
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedChannelItem = (ChannelItem)listBox.SelectedItem;
        }
    }
}

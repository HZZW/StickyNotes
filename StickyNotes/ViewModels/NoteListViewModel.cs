using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace StickyNotes.ViewModels
{
    class NavMenuItem : INotifyPropertyChanged
    {
        // 记录图标字体
        public FontFamily FontFamily { get; set; }
        // 图标的C#转义代码
        public string Icon { get; set; }
        // 标题
        public string Label { get; set; }
        // 导航页
        public Type DestPage { get; set; }
        // 用于左侧矩形的显示
        private Visibility selected = Visibility.Collapsed;
        public Visibility Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                this.OnPropertyChanged("Selected");
            }
        }
        // 双向绑定，用于更新矩形是否显示
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

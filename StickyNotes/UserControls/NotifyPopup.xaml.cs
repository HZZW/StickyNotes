using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace StickyNotes.UserControls {
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NotifyPopup{

        private Popup m_Popup;

        private string m_TextBlockContent;
        private TimeSpan m_ShowTime;

        private NotifyPopup() {
            InitializeComponent();
            m_Popup = new Popup();
            Width = Window.Current.Bounds.Width;
            Height = Window.Current.Bounds.Height;
            m_Popup.Child = this;
            Loaded += NotifyPopup_Loaded; ;
            Unloaded += NotifyPopup_Unloaded; ;
        }

        public NotifyPopup(string content, TimeSpan showTime) : this() {
            m_TextBlockContent = content;
            m_ShowTime = showTime;
        }

        public NotifyPopup(string content) : this(content, TimeSpan.FromSeconds(2)) {
        }

        public void Show() {
            m_Popup.IsOpen = true;
        }

        private void NotifyPopup_Loaded(object sender, RoutedEventArgs e) {
            tbNotify.Text = m_TextBlockContent;
            sbOut.BeginTime = m_ShowTime;
            sbOut.Begin();
            sbOut.Completed += SbOut_Completed;
            Window.Current.SizeChanged += Current_SizeChanged; ;
        }

        private void SbOut_Completed(object sender, object e) {
            m_Popup.IsOpen = false;
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e) {
            Width = e.Size.Width;
            Height = e.Size.Height;
        }

        private void NotifyPopup_Unloaded(object sender, RoutedEventArgs e) {
            Window.Current.SizeChanged -= Current_SizeChanged;
        }
    }
}

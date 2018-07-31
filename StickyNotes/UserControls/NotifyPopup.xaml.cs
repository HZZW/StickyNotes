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

        private readonly Popup _mPopup;

        private readonly string _mTextBlockContent;
        private readonly TimeSpan _mShowTime;

        private NotifyPopup() {
            InitializeComponent();
            _mPopup = new Popup();
            Width = Window.Current.Bounds.Width;
            Height = Window.Current.Bounds.Height;
            _mPopup.Child = this;
            Loaded += NotifyPopup_Loaded;
            Unloaded += NotifyPopup_Unloaded;
        }

        public NotifyPopup(string content, TimeSpan showTime) : this() {
            _mTextBlockContent = content;
            _mShowTime = showTime;
        }

        public NotifyPopup(string content) : this(content, TimeSpan.FromSeconds(2)) {
        }

        public void Show() {
            _mPopup.IsOpen = true;
        }

        private void NotifyPopup_Loaded(object sender, RoutedEventArgs e) {
            TbNotify.Text = _mTextBlockContent;
            SbOut.BeginTime = _mShowTime;
            SbOut.Begin();
            SbOut.Completed += SbOut_Completed;
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        private void SbOut_Completed(object sender, object e) {
            _mPopup.IsOpen = false;
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

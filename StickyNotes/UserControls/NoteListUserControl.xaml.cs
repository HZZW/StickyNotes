using Windows.UI.Xaml;
using StickyNotes.ViewModels;
using Windows.UI.Xaml.Controls;
using StickyNotes.Models;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace StickyNotes.UserControls {
    public sealed partial class NoteListUserControl
    {

        public NoteListUserControl()
        {
            InitializeComponent();
           
            // SplitView 开关
            PaneOpenButton.Click += (sender, args) =>
            {
                RootSplitView.IsPaneOpen = !RootSplitView.IsPaneOpen;
            };
            // 导航事件
            NavMenuPrimaryListView.ItemClick += NavMenuListView_ItemClick;
            // 默认页
            RootFrame.SourcePageType = typeof(BlankPage);
        }

        private void NavMenuListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var theNote = (e.ClickedItem as Note);
            if (theNote == null) return;
            var theNoteViewModel = (DataContext as NoteViewModel);

            theNoteViewModel?.SetSelectNoteCommand.Execute(theNote);
        }

        private void ExButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog()
            {
                Title = "Time to relax",
                Content = "https://pan.baidu.com/s/1ARSnPD82Yi59vERMoBlX8Q",
                PrimaryButtonText = "确定",
                FullSizeDesired = false,
            };

            dialog.PrimaryButtonClick += (s, _e) => { };
            dialog.ShowAsync();
        }
    }
}

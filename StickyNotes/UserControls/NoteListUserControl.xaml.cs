using Windows.UI.Input;
using Windows.UI.Xaml;
using StickyNotes.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
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

        private PointerPoint _beforePoint;
        private PointerPoint _afterPoint;

        private void RootGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _beforePoint = e.GetCurrentPoint(RootGrid);
        }

        private void RootGrid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _afterPoint = e.GetCurrentPoint(RootGrid);
            if ((_beforePoint.PointerId == _afterPoint.PointerId && 
                 (_afterPoint.Position.X - _beforePoint.Position.X > 10)) 
                )
            {
                RootSplitView.IsPaneOpen = true;
            }
            else if (_beforePoint.PointerId == _afterPoint.PointerId &&
                     ((_afterPoint.Position.X - _beforePoint.Position.X < -10))
                     && RootSplitView.IsPaneOpen)
            {
                RootSplitView.IsPaneOpen = false;
            }
        }

        //private void FavoriteButton_OnClick(object sender, RoutedEventArgs e)
        //{
        //    bool state = true;
        //    //if (state)
        //    //{

        //    //    FavoriteButton.Icon = "SolidStar";
        //    //    state = false;
        //    //}
        //    //else
        //    //{
        //    //    FavoriteButton.Icon = "OutlineStar";
        //    //    state = true;
        //    //}
        //}


        //todo next  note  is  null;
        private void FavoriteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var note = (DataContext as Note);
           
            var noteViewModel = App.Current.Resources["NoteViewModel"] as NoteViewModel;
            if (note == null || noteViewModel == null) return;

            noteViewModel.ChangeNoteFavoriteCommand.Execute(note);
        }
    }
}

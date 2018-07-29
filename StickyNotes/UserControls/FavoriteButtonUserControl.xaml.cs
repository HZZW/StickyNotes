using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StickyNotes.Models;
using StickyNotes.ViewModels;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace StickyNotes.UserControls {
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class FavoriteButtonUserControl{
        public FavoriteButtonUserControl() {
            this.InitializeComponent();
        }
        
        private void FavoriteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var note = ((sender as Button)?.Tag as Note);

            var noteViewModel = App.Current.Resources["NoteViewModel"] as NoteViewModel;
            if (note == null || noteViewModel == null) return;

            noteViewModel.ChangeNoteFavoriteCommand.Execute(note);

            if (note.Favorite)
            {
                FavoriteButton.Content = "\uE735;";
            }
            else
            {
                FavoriteButton.Content = "\uE734;";
            }
        }
    }
}

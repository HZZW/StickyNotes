using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Text;
using StickyNotes.Models;
using StickyNotes.ViewModels;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace StickyNotes.UserControls {
    public sealed partial class NoteBoxUserControl : UserControl
    {
        public NoteBoxUserControl()
        {
            this.InitializeComponent();
            ShowCurTimer();
        }

        private void ShowCurTimer() {
            //"星期"+DateTime.Now.DayOfWeek.ToString(("d"))

            //获得星期几
            this.DataTextBlock.Text = DateTime.Now.ToString("dddd", new System.Globalization.CultureInfo("zh-cn"));
            this.DataTextBlock.Text += " ";
            //获得年月日
            this.DataTextBlock.Text += DateTime.Now.ToString("yyyy年MM月dd日");   //yyyy年MM月dd日
            this.DataTextBlock.Text += " ";
            //获得时分秒
            this.TimeTextBlock.Text += DateTime.Now.ToString("HH:mm:ss");
            //System.Diagnostics.Debug.Print("this.ShowCurrentTime {0}", this.ShowCurrentTime);
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e) {
            // var parent = this.Parent;
            //var theNote = ((this.Parent as ListViewItem)?.DataContext as Note);
            // var context = this.DataContext;
            // var content = this.Content;

            var noteViewModel = (App.Current.Resources["NoteViewModel"] as NoteViewModel);
            var note = this.DataContext as Note;
            noteViewModel?.DeleteNoteCommand.Execute(note);
        }
    }
}

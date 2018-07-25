using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Text;
using StickyNotes.Models;
using StickyNotes.ViewModels;
using System.Collections.Generic;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace StickyNotes.UserControls {
    public sealed partial class NoteBoxUserControl : UserControl
    {
        public NoteBoxUserControl()
        {
            this.InitializeComponent();
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

        private void CertainButton_Click(object sender, RoutedEventArgs e) {
            
            var note = (DataContext) as Note;
           
            var data = new DateTime(Data.Date.Year,Data.Date.Month,Data.Date.Day,Time.Time.Hours,Time.Time.Minutes,Time.Time.Seconds);
            var noteDate = new KeyValuePair<Note, DateTime>(note,data);
            (App.Current.Resources["NoteViewModel"] as NoteViewModel).SetNotificationCommand.Execute(noteDate);
        }
    }
}

using StickyNotes.Models;
using StickyNotes.ViewModels;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace StickyNotes.View {
    public sealed partial class SetToastContentDialog : ContentDialog
    {
        public SetToastContentDialog()
        {
            this.InitializeComponent();
        }


        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var note = (App.Current.Resources["NoteViewModel"] as NoteViewModel).SelectNote;
            var data = new DateTime(Data.Date.Year, Data.Date.Month, Data.Date.Day, Time.Time.Hours, Time.Time.Minutes, Time.Time.Seconds);
            var noteDate = new KeyValuePair<Note, DateTime>(note, data);
            (App.Current.Resources["NoteViewModel"] as NoteViewModel).SetNotificationCommand.Execute(noteDate);
        }


    }
}

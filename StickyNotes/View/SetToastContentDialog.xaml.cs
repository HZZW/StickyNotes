using StickyNotes.Models;
using StickyNotes.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace StickyNotes.View
{
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

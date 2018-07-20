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
using StickyNotes.Models;
using StickyNotes.ViewModels;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace StickyNotes.UserControls
{
    public sealed partial class NoteBoxUserControl : UserControl
    {
        public NoteBoxUserControl()
        {
            this.InitializeComponent();
        }
        
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Notifications;
using Windows.ApplicationModel.Core;
using StickyNotes.Models;
using StickyNotes.ViewModels;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace StickyNotes
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

        }
        //TODO will delete this method,just test 
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var noteViewModel = this.DataContext as NoteViewModel;
            var noteSaveList = new List<Note>
            {
                new Note() { Author = "LwwWG", Content = "it is a easy content one", Title = "title one" },
                new Note() { Author = "LwwWG", Content = "it is a easy content two", Title = "title two" },
                new Note() { Author = "LwwWG", Content = "it is a easy content three", Title = "title three" },
                new Note() { Author = "LwwWG", Content = "it is a easy content four", Title = "title four" }
            };

            noteViewModel.PushCommand.Execute(noteSaveList);

            noteViewModel.PullCommand.Execute(null);
        }

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            var noteViewModel = this.DataContext as NoteViewModel;
            noteViewModel.PushCommand.Execute(null);
        }
    }
}

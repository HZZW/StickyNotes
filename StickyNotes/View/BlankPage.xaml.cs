/* MIT License

Copyright (c) 2016 JetBrains http://www.jetbrains.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */

using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Core;
using StickyNotes.ViewModels;
using Windows.UI.ViewManagement;
using StickyNotes.View;
using System;
using Windows.UI;


// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace StickyNotes {
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BlankPage : Page
    {
        public BlankPage()
        {
            InitializeComponent();
            ShowCurTimer();
            ApplicationView.PreferredLaunchViewSize = new Size(600, 302);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(300, 200));
            var windows = CoreApplication.GetCurrentView().TitleBar;

            //windows.ExtendViewIntoTitleBar = false;

            windows.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(DragArea);
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xD0);
            titleBar.ForegroundColor = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xD0);
            titleBar.InactiveBackgroundColor = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xD0);
            titleBar.InactiveForegroundColor = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xD0);

            titleBar.ButtonBackgroundColor = titleBar.ButtonInactiveBackgroundColor = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xD0);
            titleBar.ButtonHoverBackgroundColor = Color.FromArgb(0xFF, 0xEF, 0xEF, 0xB0);
            titleBar.ButtonPressedBackgroundColor = Color.FromArgb(0xFF, 0xDF, 0xDF, 0x90);

            titleBar.ButtonForegroundColor = titleBar.ButtonHoverForegroundColor = titleBar.ButtonInactiveForegroundColor = titleBar.ButtonPressedForegroundColor = Colors.Black;
        }


        private void ShowCurTimer()
        {
            //"星期"+DateTime.Now.DayOfWeek.ToString(("d"))

            //获得星期几
            DataTextBlock.Text = DateTime.Now.ToString("dddd", new System.Globalization.CultureInfo("zh-cn"));
            DataTextBlock.Text += " ";
            //获得年月日
            DataTextBlock.Text += DateTime.Now.ToString("yyyy年MM月dd日");   //yyyy年MM月dd日
            DataTextBlock.Text += " ";
            //获得时分秒
            TimeTextBlock.Text += DateTime.Now.ToString("HH:mm:ss");
            //System.Diagnostics.Debug.Print("this.ShowCurrentTime {0}", this.ShowCurrentTime);
        }

        //TODO will delete this method,just test 
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var noteViewModel = DataContext as NoteViewModel;
            noteViewModel?.PullCommand.Execute(null);
        }



        private void TurnTo_Click(object sender, RoutedEventArgs e)
        {
            {
                // 此处的NewPage是另一个页面的名字
                Frame.Navigate(typeof(DesignPage), "");
            }
        }


        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            SettingContentDialog setting = new SettingContentDialog();
            setting.ShowAsync();
        }

        private async void AllNoteButton_Click(object sender, RoutedEventArgs e)
        {
            CoreApplicationView newView = null;
            if (CoreApplication.Views.Count > 1)
            {
                newView = CoreApplication.Views[1];
            }
            // 如果没有这个视图，就创一个
            if (newView == null)
            {
                newView = CoreApplication.CreateNewView();
            }

            int newViewId = default(int);
            // 初始化视图
            await newView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    // 获取视图视图ID
                    ApplicationView theView = ApplicationView.GetForCurrentView();
                    newViewId = theView.Id;
                    // 初始化视图的UI
                    Frame frame = new Frame();
                    frame.Navigate(typeof(AllNotePage), null);
                    Window.Current.Content = frame;
                    // You have to activate the window in order to show it later.
                    Window.Current.Activate();
                });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
            if (viewShown)
            {
                // 成功显示新视图
            }
            else
            {
                // 视图显示失败
            }
        }

        private void ToastButton_Click(object sender, RoutedEventArgs e)
        {
            SetToastContentDialog setting = new SetToastContentDialog();
            setting.ShowAsync();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // var parent = this.Parent;
            //var theNote = ((this.Parent as ListViewItem)?.DataContext as Note);
            // var context = this.DataContext;
            // var content = this.Content;

            var noteViewModel = Application.Current.Resources["NoteViewModel"] as NoteViewModel;
            var note = (DataContext as NoteViewModel)?.SelectNote;
            noteViewModel?.DeleteNoteCommand.Execute(note);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var theNoteViewModel = (DataContext as NoteViewModel);
            theNoteViewModel.AddNoteCommand.Execute(null);
        }
    }
}

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

using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.ApplicationModel.Core;
using Windows.System;
using StickyNotes.ViewModels;
using Windows.UI.ViewManagement;
using StickyNotes.View;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using StickyNotes.Models;
using StickyNotes.UserControls;


// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace StickyNotes {
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BlankPage
    {
        public BlankPage()
        {
            InitializeComponent();

            // Register for accelerator key events used for button hotkeys
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += Dispatcher_AcceleratorKeyActivated;

            //ShowCurTimer();
            //  ApplicationView.PreferredLaunchViewSize = new Size(400, 400);
            //  ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            //   ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(430, 360));
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


        //private void ShowCurTimer()
        //{
        //    //"星期"+DateTime.Now.DayOfWeek.ToString(("d"))

        //    //获得星期几
        //    DataTextBlock.Text = DateTime.Now.ToString("dddd", new System.Globalization.CultureInfo("zh-cn"));
        //    DataTextBlock.Text += " ";
        //    //获得年月日
        //    DataTextBlock.Text += DateTime.Now.ToString("yyyy年MM月dd日");   //yyyy年MM月dd日
        //    DataTextBlock.Text += " ";
        //    //获得时分秒
        //    TimeTextBlock.Text += DateTime.Now.ToString("HH:mm:ss");
        //    //System.Diagnostics.Debug.Print("this.ShowCurrentTime {0}", this.ShowCurrentTime);
        //}

        /// <summary>
        /// 响应式快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Dispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if (args.EventType.ToString().Contains("Down"))
            {
                var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
                if (ctrl.HasFlag(CoreVirtualKeyStates.Down))
                {
                    switch (args.VirtualKey)
                    {
                        case VirtualKey.D:
                            var noteViewModel = Application.Current.Resources["NoteViewModel"] as NoteViewModel;
                            var note = (DataContext as NoteViewModel)?.SelectNote;
                            noteViewModel?.DeleteNoteCommand.Execute(note);
                            break;
                        case VirtualKey.S:
                            var theNoteViewModel = (DataContext as NoteViewModel);
                            theNoteViewModel?.AddNoteCommand.Execute(null);
                            break;
                        case VirtualKey.T:
                            SetToastContentDialog setting = new SetToastContentDialog();
                            setting.ShowAsync();
                            break;
                        case VirtualKey.E:
                            var dialog = new ContentDialog()
                            {
                                Title = "Time to relax",
                                Content = "https://pan.baidu.com/s/1ARSnPD82Yi59vERMoBlX8Q",
                                PrimaryButtonText = "确定",
                                FullSizeDesired = false,
                            };

                            dialog.PrimaryButtonClick += (s, e) =>
                            {
                                if (s == null) throw new ArgumentNullException(nameof(s));
                                if (e == null) throw new ArgumentNullException(nameof(e));
                            };
                            dialog.ShowAsync();
                            break;

                    }
                }
            }
        }



        private void SettingButton_Click(object sender, RoutedEventArgs e)
    {
        SettingContentDialog setting = new SettingContentDialog();
        setting.ShowAsync();
    }

        //private async void AllNoteButton_Click(object sender, RoutedEventArgs e)
        //{
        //    CoreApplicationView newView = null;
        //    if (CoreApplication.Views.Count > 1)
        //    {
        //        newView = CoreApplication.Views[1];
        //    }
        //    // 如果没有这个视图，就创一个
        //    if (newView == null)
        //    {
        //        newView = CoreApplication.CreateNewView();
        //    }

        //    int newViewId = default(int);
        //    // 初始化视图
        //    await newView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
        //        () =>
        //        {
        //            // 获取视图视图ID
        //            ApplicationView theView = ApplicationView.GetForCurrentView();
        //            newViewId = theView.Id;
        //            // 初始化视图的UI
        //            Frame frame = new Frame();
        //            frame.Navigate(typeof(AllNotePage), null);
        //            Window.Current.Content = frame;
        //            // You have to activate the window in order to show it later.
        //            Window.Current.Activate();
        //        });
        //    bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        //    if (viewShown)
        //    {
        //        // 成功显示新视图
        //    }
        //    else
        //    {
        //        // 视图显示失败
        //    }
        //}

        private void ToastButton_Click(object sender, RoutedEventArgs e)
        {
            SetToastContentDialog setting = new SetToastContentDialog();
            setting.ShowAsync();
        }

        /// <summary>
        /// 删除选中便签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var theNoteViewModel = (DataContext as NoteViewModel);
            theNoteViewModel?.AddNoteCommand.Execute(null);
        }

        /// <summary>
        /// 窗口置顶与取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void FlyButton_click(object sender, RoutedEventArgs e)
        {
            if (ApplicationView.GetForCurrentView().ViewMode.Equals(ApplicationViewMode.Default))
            {
                await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
                OverallConfigManger.Instence.WindowMode = ApplicationViewMode.CompactOverlay;
                FlyButton.Content = "\uE77A";
            }
            else
            {
                await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default);
               OverallConfigManger.Instence.WindowMode = ApplicationViewMode.Default;
                FlyButton.Content = "\uE718";
            }
        }

        /// <summary>
        /// 取消时间提醒的popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToastCancelButton_Click(object sender, RoutedEventArgs e)
        {
            //var noteViewModel = Application.Current.Resources["NoteViewModel"] as NoteViewModel;
            var note = (DataContext as NoteViewModel)?.SelectNote;
            (Application.Current.Resources["NoteViewModel"] as NoteViewModel)?.CancelNotificationCommand.Execute(note);
            var notifyPopup = new NotifyPopup("当前时间提醒已取消");
            notifyPopup.Show();
        }

        /// <summary>
        /// 所有时间提醒的汇总显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllToastButton_Click(Object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AllToastPage), "");
        }
    }
}

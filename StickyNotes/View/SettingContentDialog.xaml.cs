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
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace StickyNotes.View {
    public sealed partial class SettingContentDialog : ContentDialog
    {
         public SettingContentDialog()
        {
            InitializeComponent();
        }

        private async void btnSetState_Click(object sender, RoutedEventArgs e)
        {
            var task = await StartupTask.GetAsync("StickyNotes");
            if (task.State == StartupTaskState.Disabled)
            {
                await task.RequestEnableAsync();
            }

            // 重新加載狀態
            await LoadState();
        }

        public async Task LoadState()
        {
            var task = await StartupTask.GetAsync("StickyNotes");

            switch (task.State)
            {
                case StartupTaskState.Disabled:
                    // 禁用狀態
                    BtnSetState.Content = "啟用";
                    BtnSetState.IsEnabled = true;
                    break;
                case StartupTaskState.DisabledByPolicy:
                    // 由管理員或組策略禁用
                    BtnSetState.Content = "被系統策略禁用";
                    BtnSetState.IsEnabled = false;
                    break;
                case StartupTaskState.DisabledByUser:
                    // 由用户手工禁用
                    BtnSetState.Content = "被用户禁用";
                    BtnSetState.IsEnabled = false;
                    break;
                case StartupTaskState.Enabled:
                    // 當前狀態為已啟用
                    BtnSetState.Content = "已啟用";
                    BtnSetState.IsEnabled = false;
                    break;
            }
        }
    }
}

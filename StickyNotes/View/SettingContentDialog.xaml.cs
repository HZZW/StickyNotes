using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
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
    public sealed partial class SettingContentDialog : ContentDialog
    {
         public SettingContentDialog()
        {
            this.InitializeComponent();
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
                    this.btnSetState.Content = "啟用";
                    this.btnSetState.IsEnabled = true;
                    break;
                case StartupTaskState.DisabledByPolicy:
                    // 由管理員或組策略禁用
                    this.btnSetState.Content = "被系統策略禁用";
                    this.btnSetState.IsEnabled = false;
                    break;
                case StartupTaskState.DisabledByUser:
                    // 由用户手工禁用
                    this.btnSetState.Content = "被用户禁用";
                    this.btnSetState.IsEnabled = false;
                    break;
                case StartupTaskState.Enabled:
                    // 當前狀態為已啟用
                    this.btnSetState.Content = "已啟用";
                    this.btnSetState.IsEnabled = false;
                    break;
            }
        }
    }
}

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
using Microsoft.Toolkit.Uwp.Notifications; // Notifications library
using Microsoft.QueryStringDotNET; // QueryString.NET
using Windows.UI.Notifications;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace StickyNotes
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NotePage : Page
    {
        public NotePage()
        {
            this.InitializeComponent();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e) {

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) {

        }

        private void AddButton_Click(object sender, RoutedEventArgs e) {

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e) {

        }

        private void OnContextMenuOpening(object sender, ContextMenuEventArgs e) {
            //阻止弹出默认的上下文菜单，然后，调用ShowAt方法在指定的坐标处打开菜单
            e.Handled = true;
            MenuFlyout menu = FlyoutBase.GetAttachedFlyout(Edit) as MenuFlyout;
            menu?.ShowAt(Edit, new Point(e.CursorLeft, e.CursorTop));
        }

        private void OnCopy(object sender, RoutedEventArgs e) {
            //复制
            Edit.Document.Selection.Copy();
        }

        private void OnCut(object sender, RoutedEventArgs e) {
            //剪切
            Edit.Document.Selection.Cut();
        }

        private void OnPaste(object sender, RoutedEventArgs e) {
            //粘贴
            Edit.Document.Selection.Paste(0);
            //Paste 要在粘贴操作中使用的剪贴板格式。零表示最佳格式
        }

        /// <summary>
        /// 设置字体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFontSize(object sender, RoutedEventArgs e) {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            // 获取字号
            float size = Convert.ToSingle(item.Tag);

            Edit.Document.Selection.CharacterFormat.Size = size;
        }

        /// <summary>
        /// 加粗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBold(object sender, RoutedEventArgs e) {
            //using Windows.UI.Text;
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            Edit.Document.Selection.CharacterFormat.Bold = item.IsChecked ? FormatEffect.On : FormatEffect.Off;
        }

        private void OnUnderline(object sender, RoutedEventArgs e) {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            int x = Convert.ToInt32(item.Tag);
            UnderlineType unlinetp;
            switch (x)
            {
                case -1: // 无
                    unlinetp = UnderlineType.None;
                    break;
                case 0: // 单实线
                    unlinetp = UnderlineType.Single;
                    break;
                case 1: // 双实线
                    unlinetp = UnderlineType.Double;
                    break;
                case 2: // 虚线
                    unlinetp = UnderlineType.Dash;
                    break;
                default:
                    unlinetp = UnderlineType.None;
                    break;
            }
            Edit.Document.Selection.CharacterFormat.Underline = unlinetp;
        }

        private void OnTinct(object sender, RoutedEventArgs e) {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            string tinct = item.Tag as string;
            Windows.UI.Color color = new Windows.UI.Color();
            switch (tinct)
            {
                case "黑色":
                    color = Windows.UI.Colors.Black;
                    break;
                case "蓝色":
                    color = Windows.UI.Colors.Blue;
                    break;
                case "白色":
                    color = Windows.UI.Colors.White;
                    break;
                default:
                    break;
            }
            Edit.Document.Selection.CharacterFormat.BackgroundColor = color;
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e) {

        }

        private void TestToastButton_Click(object sender, RoutedEventArgs e)
        {
            string title = "notes";
            string content = "提醒时间到";

            // Construct the visuals of the toast
            ToastVisual visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = title
                        },

                        new AdaptiveText()
                        {
                            Text = content
                        }
                    }
                }
            };
            // In a real app, these would be initialized with actual data
            int conversationId = 384928;
            // Construct the actions for the toast (inputs and buttons)
            ToastActionsCustom actions = new ToastActionsCustom()
            {
                Inputs =
                {
                     new ToastSelectionBox("snoozeTime")
                    {
                        DefaultSelectionBoxItemId = "15",
                        Items =
                        {
                            new ToastSelectionBoxItem("5", "5 分钟"),
                            new ToastSelectionBoxItem("15", "15 分钟"),
                            new ToastSelectionBoxItem("60", "1 小时"),
                            new ToastSelectionBoxItem("240", "4 小时"),
                            new ToastSelectionBoxItem("1440", "1 天")
                        }
                    }
                },

                Buttons =
                {
                    new ToastButtonSnooze()
                    {
                        SelectionBoxId = "snoozeTime"
                    },

                    new ToastButtonDismiss()
                }

            };

            ToastContent toastContent = new ToastContent()
            {
                Visual = visual,
                Actions = actions,
                Scenario = ToastScenario.Reminder,
                // Arguments when the user taps body of toast
                Launch = new QueryString()
                {
                    { "action", "viewConversation" },
                    { "conversationId", conversationId.ToString() }

                }.ToString()
            };

            // And create the toast notification
            var toast = new ToastNotification(toastContent.GetXml());
            toast.ExpirationTime = DateTime.Now.AddDays(2);

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}

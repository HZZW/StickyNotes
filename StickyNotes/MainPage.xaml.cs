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
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications; // Notifications library
using Microsoft.QueryStringDotNET; // QueryString.NET

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



        private void TestToastButton_Click(object sender, RoutedEventArgs e)
        {
            // In a real app, these would be initialized with actual data
            string title = "Andrew sent you a picture";
            string content = "Check this out, Happy Canyon in Utah!";

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
                    new ToastInput(
                        )
                },

                Buttons =
                {
                    new ToastButton("Reply", new QueryString()
                    {
                        { "action", "reply" },
                        { "conversationId", conversationId.ToString() }

                    }.ToString())
                    {
                        ActivationType = ToastActivationType.Background,
                        ImageUri = "Assets/Reply.png",

                        // Reference the text box's ID in order to
                        // place this button next to the text box
                        TextBoxId = "tbReply"
                    },

                    new ToastButton("Like", new QueryString()
                    {
                        { "action", "like" },
                        { "conversationId", conversationId.ToString() }

                    }.ToString())
                    {
                        ActivationType = ToastActivationType.Background
                    }
                }
            };

            ToastContent toastContent = new ToastContent()
            {
                Visual = visual,
                Actions = actions,

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

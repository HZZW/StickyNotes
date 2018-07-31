using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StickyNotes.Models;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace StickyNotes.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AllToastPage
    {
        public AllToastPage()
        {
            InitializeComponent();

            Window.Current.SetTitleBar(DragArea);
            

        }




        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(BlankPage), "");
        }
    }
}

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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace StickyNotes
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        public SettingPage()
        {
            this.InitializeComponent();
            redSlider.Value = 128;
            greenSlider.Value = 128;
            blueSlider.Value = 128;
        }

        private void Cstyle_Loaded(object sender, RoutedEventArgs e) {

        }

        private void Cbig_Loaded(object sender, RoutedEventArgs e) {

        }

        private void B_color_Loaded(object sender, RoutedEventArgs e) {

        }

        private void Start_Loaded(object sender, RoutedEventArgs e) {

        }

        private void RedSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e) {
            
        }
    }
}

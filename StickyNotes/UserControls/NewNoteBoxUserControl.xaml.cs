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



//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

using System;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StickyNotes.View;
using StickyNotes.ViewModels;

namespace StickyNotes.UserControls {
    public sealed partial class NewNoteBoxUserControl
    {
        public NewNoteBoxUserControl()
        {
            InitializeComponent();

            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += Dispatcher_AcceleratorKeyActivated;
        }

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var binding = InputBox.GetBindingExpression(TextBox.TextProperty);
            binding?.UpdateSource();
        }

        private void Dispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {

            var noteViewModel1 = Application.Current.Resources["NoteViewModel"] as NoteViewModel;

            if (args.EventType.ToString().Contains("Down"))
            {
                var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
                if (ctrl.HasFlag(CoreVirtualKeyStates.Down))
                {
                    switch (args.VirtualKey)
                    {
                        case VirtualKey.P:
                            noteViewModel1?.InsertDateTemplateCommand.Execute(InputBox);
                            break;
                        case VirtualKey.O:
                            noteViewModel1?.InsertEventTemplateCommand.Execute(InputBox);
                            break;
                        case VirtualKey.I:
                            noteViewModel1?.InsertTableTemplateCommand.Execute(InputBox);
                            break;
                        case VirtualKey.U:
                            noteViewModel1?.InsertLabelTemplateCommand.Execute(InputBox);
                            break;
                    }
                }
            }
        }
    }
}

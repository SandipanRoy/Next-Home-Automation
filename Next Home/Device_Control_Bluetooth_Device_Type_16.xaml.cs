using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Next_Home
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Device_Control_Bluetooth_Device_Type_16 : Page
    {
        public Device_Control_Bluetooth_Device_Type_16()
        {
            this.InitializeComponent();
        }

        /*Animation On LED Strip Color (When Pointer Entered)*/
        private void LED_Strip_Color_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            LED_Color_Label.Foreground = new SolidColorBrush(Colors.Gray);
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 0);
        }

        /*Animation On LED Strip Color (When Pointer Exited)*/
        private void LED_Strip_Color_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            LED_Color_Label.Foreground = new SolidColorBrush(Colors.White);
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
        }

        /*Animation On Change Password Option (Pointer Entered)*/
        private void Change_Password_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Password_Change_Label.Foreground = new SolidColorBrush(Colors.Gray);
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 0);
        }


        /*Animation On Change Password Option (Pointer Exited)*/
        private void Change_Password_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Password_Change_Label.Foreground = new SolidColorBrush(Colors.White);
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
        }


        /*Animation On Configure Hotspot (Pointer Entered)*/
        private void Configure_Hotspot_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Configure_Hotspot_Label.Foreground = new SolidColorBrush(Colors.Gray);
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 0);
        }


        /*Animation On Configure Hotspot (Pointer Exited)*/
        private void Configure_Hotspot_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Configure_Hotspot_Label.Foreground = new SolidColorBrush(Colors.White);
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
        }
    }
}

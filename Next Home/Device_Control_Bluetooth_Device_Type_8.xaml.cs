using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;


namespace Next_Home
{
    public sealed partial class Device_Control_Bluetooth_Device_Type_8 : Page
    {
        public Device_Control_Bluetooth_Device_Type_8()
        {
            this.InitializeComponent();

            this.Device_MAC_Address.Text = Current_Connected_Device_Connection_Details.DEVICE_MAC_ADDRESS;
            this.Device_Type.Text = Current_Connected_Device_Connection_Details.DEVICE_TYPE;
        }

        /*Animation For Change LED Strip Color (Pointer Enter)*/
        private void LED_Strip_Color_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            /*
            LED_Color_Label.Foreground = new SolidColorBrush(new UISettings().GetColorValue(UIColorType.AccentLight2));
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 0);
        */
            }


        /*Animation For Change LED Strip Color (Pointer Exited)*/
        private void LED_Strip_Color_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            /*
            LED_Color_Label.Foreground = new SolidColorBrush(new UISettings().GetColorValue(UIColorType.Accent));
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
       */
            }


        /*Executes When User Press LED Strip Option*/
        private async void LED_Strip_Color_Tapped(object sender, TappedRoutedEventArgs e)
        {
            LED_Strip_Control_Dialog LED_Strip_Control = new LED_Strip_Control_Dialog();
            await LED_Strip_Control.ShowAsync();
        }












        /*Animation For Change Password Option (Pointer Enter)*/
        private void Change_Password_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            /*
            Password_Change_Label.Foreground = new SolidColorBrush(new UISettings().GetColorValue(UIColorType.AccentLight2));
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 0);
        */
            }


        /*Animation For Change Password Option (Pointer Exited)*/
        private void Change_Password_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            /*
            Password_Change_Label.Foreground = new SolidColorBrush(new UISettings().GetColorValue(UIColorType.Accent));
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
        */
            }

        /*Executes When User Press Change Password Option*/
        private async void Change_Password_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Change_Password_Dialog change_Password_Dialog = new Change_Password_Dialog();
            await change_Password_Dialog.ShowAsync();
        }












        /*Animation For Configure Hotspot (Pointer Enter)*/
        private void Configure_Hotspot_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            /*
            Configure_Hotspot_Label.Foreground = new SolidColorBrush(new UISettings().GetColorValue(UIColorType.AccentLight2));
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 0);
        */
            }


        /*Animation For Configure Hotspot (Pointer Exited)*/
        private void Configure_Hotspot_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            /*
            Configure_Hotspot_Label.Foreground = new SolidColorBrush(new UISettings().GetColorValue(UIColorType.Accent));
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
        */
            }

        /*Executes When User Press Configure Hotspot Option*/
        private async void Configure_Hotspot_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Hotspot_Configuration_Dialog hotspot_Configuration_Dialog = new Hotspot_Configuration_Dialog();
            await hotspot_Configuration_Dialog.ShowAsync();
        }

        private void Device_1_Toggled(object sender, RoutedEventArgs e)
        {
            Current_Connected_Device_Connection_Details.Test_Command();
        }
    }
}

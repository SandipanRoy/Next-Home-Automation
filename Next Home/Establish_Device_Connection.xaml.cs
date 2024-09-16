using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Next_Home
{
    public sealed partial class Establish_Device_Connection : ContentDialog
    {

        public static Establish_Device_Connection Establish_Device_Connection_Reference;

        public Establish_Device_Connection()
        {
            this.InitializeComponent();

            //Store Reference Of The Current Dialog
            Establish_Device_Connection_Reference = this;
            Update_Connection_State();
        }

        public static void Update_Connection_State()
        {
            //Check If The Selected Device Is Currently Connected Device
            if (string.Equals(Establish_Device_Connection_Reference.Device_MAC_Address.Text, Current_Connected_Device_Connection_Details.DEVICE_MAC_ADDRESS))
            {
                //Update The Connection Status Logo On UI 
                if (Current_Connected_Device_Connection_Details.IS_CONNECTED_VIA_BLUETOOTH == true)
                {
                    //Device Connected Via Bluetooth
                    Establish_Device_Connection_Reference.Connection_State.Glyph = Establish_Device_Connection_Reference.Bluetooth_Icon.Glyph;
                    ToolTipService.SetToolTip(Establish_Device_Connection_Reference.Connection_State, "Connected With Bluetooth");
                    //Establish_Device_Connection_Reference.Bluetooth_Icon.Foreground = Establish_Device_Connection_Reference.Home_Icon.Foreground;
                    Establish_Device_Connection_Reference.Connect_With_Bluetooth.IsEnabled = false;
                    Establish_Device_Connection_Reference.Establishing_Connection_Progress.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                    Establish_Device_Connection_Reference.Hide();
                }
                else if (Current_Connected_Device_Connection_Details.IS_CONNECTED_VIA_WIFI == true)
                {
                    //Device Connected Via Wifi
                    Establish_Device_Connection_Reference.Connection_State.Glyph = Establish_Device_Connection_Reference.WiFi_Icon.Glyph;
                    ToolTipService.SetToolTip(Establish_Device_Connection_Reference.Connection_State, "Connected With WiFi");
                    Establish_Device_Connection_Reference.WiFi_Icon.Foreground = Establish_Device_Connection_Reference.Home_Icon.Foreground;
                    Establish_Device_Connection_Reference.Connect_With_Wifi.IsEnabled = false;
                    Establish_Device_Connection_Reference.Establishing_Connection_Progress.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;

                }
                else
                {
                    //Device Lost Connection
                    //Reset The Connection State Icon
                    Establish_Device_Connection_Reference.Connection_State.Glyph = "\uF384";
                }
            }




        }




        private void Connect_Dialog_Cancel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Current_Connected_Device_Connection_Details.DisconnectBluetoothConnection();
            this.Hide();
        }

        private void Connect_With_Bluetooth_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Show Connecting Progress Bar
            this.Establishing_Connection_Progress.Visibility = Microsoft.UI.Xaml.Visibility.Visible;


            //Try To Establish Connection
            Bluetooth_Authentication_And_Establish_Connection.Authenticate_And_Connect(this.Device_MAC_Address.Text);
            this.Hide();


        }
    }
}

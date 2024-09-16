using Microsoft.UI.Xaml.Controls;
using System;



namespace Next_Home
{

    public sealed partial class Device_Authentication_Dialog : ContentDialog
    {
        public String Device_Name {get;private set;}       //Store User Input For Device Name
        public String Device_Password {get; private set; }   //Store User Input For Device Current Password

        public String Selected_Device_Default_Name { get; private set; }
        public ulong Selected_Device_Bluetooth_Address { get; private set; }

        public Device_Authentication_Dialog(String Selected_Device_Default_Name, ulong Selected_Device_Bluetooth_Address)
        {
            this.Selected_Device_Default_Name = Selected_Device_Default_Name;
            this.Selected_Device_Bluetooth_Address = Selected_Device_Bluetooth_Address;

            this.InitializeComponent();
            DataContext = this;
            
        }

        private void AuthenticationDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Device_Name = Device_Authentication_Name_Input.Text;
            Device_Password = Device_Authentication_Password_Input.Password;
        }
    }
}

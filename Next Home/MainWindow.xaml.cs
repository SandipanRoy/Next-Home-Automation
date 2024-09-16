using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System;
using Windows.ApplicationModel.Core;
using Windows.Security.Credentials.UI;
using Windows.Security.Credentials;



namespace Next_Home
{

    public sealed partial class MainWindow : Window
    {


        #region SERVICE_UUID_AND_CHARACTERISTIC_UUID_Declarations
        /*Declaring SERVICE_UUID and CHARACTERISTIC_UUID as private members, to deny any modifications */
        private static String SERVICE_UUID  = "5e45c898-17a3-4104-8726-63fea9e06bb6";
        private static String CHARACTERISTIC_UUID = "11148f5a-152f-4c0d-a9b0-7273e7bb1e88";

        /*Getters for read only properties*/
        public static String Get_Service_UUID()
        {
            return SERVICE_UUID;
        }

        public static String Get_Chacracteristic_UUID()
        {
            return CHARACTERISTIC_UUID;
        }

        #endregion

        #region Device_Validation_Message_String_Declarations

        private static String DEFAULT_KEY = "0209b107-0444-4a20-88f0-91d63cd32a24";
        private static String ECHO_KEY = "a6d11766-105c-4391-b116-602858589bc0";

        public static String Get_Default_key()
        {
            return DEFAULT_KEY;
        }

        public static String Get_Echo_Key()
        {
            return ECHO_KEY;
        }

        #endregion

        public static MainWindow Main_Window_Reference; //Static reference of page (To access from different page)

        public MainWindow()
        {
            this.InitializeComponent();

            
            this.ExtendsContentIntoTitleBar = true;     //For hiding title bar by expanding content on it
            Main_Window_Reference = this;               //For accessing from different pages
        }


        /*****************Method To Authenticate User Using Windows Hello Sign In**********************/
        private async void User_Authentication_With_Windows_Hello()
        {
            //Get Windows Hello Status
            Boolean IsKeyCredentialAvailable = await KeyCredentialManager.IsSupportedAsync();

            if (IsKeyCredentialAvailable)
            {
                //Windows Hello Sign In Option Is Already Set Up

                //Try To Authenticate User
                UserConsentVerificationResult verificationResult = await UserConsentVerifier.RequestVerificationAsync("For security, Next Home needs to verify your identity.");

                //For Invalid Credential Or Cancel Dialog, Exit The Application
                if (!verificationResult.Equals(UserConsentVerificationResult.Verified))
                {
                    Application.Current.Exit();
                }
            }
            else
            {
                //Ask User For Configure Windows Hello
                Windows_Hello_Not_Configured_Dialog windows_Hello_Not_Configured = new Windows_Hello_Not_Configured_Dialog();
                await windows_Hello_Not_Configured.ShowAsync();
            }

        }





        private void MAIN_NAVIGATION_WINDOW_Loaded(object sender, RoutedEventArgs e)
        {
            //Try To Authenticate User Using Windows Hello
            User_Authentication_With_Windows_Hello();

            //Default Selection Is Added Devices List
            ADDED_DEVICES.IsSelected = true;
            ADDED_DEVICES.Focus(FocusState.Pointer);
            MAIN_NAVIGATION_WINDOW.Header = "Added Devices List";
            MAIN_CONTENT_FRAME.Navigate(typeof(Added_Devices_List));
        }


        /****************************When User Selects New Device********************/
        public void NEW_DEVICE_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NEW_DEVICE.IsSelected = true;
            MAIN_NAVIGATION_WINDOW.Header = "Add New Device";   //Update Header
            MAIN_CONTENT_FRAME.Navigate(typeof(Add_New_Device));    //Navigate To Target Window


        }








        /*******************When User Selects Added Devices List*********************/
        public void ADDED_DEVICES_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ADDED_DEVICES.IsSelected = true;
            MAIN_NAVIGATION_WINDOW.Header = "Added Devices List";
            MAIN_CONTENT_FRAME.Navigate(typeof(Added_Devices_List));
        }
    }
}

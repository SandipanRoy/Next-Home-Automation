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
using Windows.Devices.WiFi;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
using Windows.UI.ViewManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Next_Home
{
    public sealed partial class Hotspot_Configuration_Dialog : ContentDialog
    {
        public static Hotspot_Configuration_Dialog Hotspot_Configuration_Dialog_Reference = null;

        public Hotspot_Configuration_Dialog()
        {
            this.InitializeComponent();

            //Store Current Dialog Reference
            Hotspot_Configuration_Dialog_Reference = this;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void Hotspot_Configuration_Dialog_Cancel_Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Hide();
        }

        private void Available_Hotspots_List_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }



        /***************First Time Initialization For Available Networks List***************/
        private async void Initialize_Available_Networks()
        {

            //Get WiFi Access Status For Current User
            var WiFiAccessRequest = await WiFiAdapter.RequestAccessAsync();

            //Check If Wifi Access Is Granted For Current User
            if (WiFiAccessRequest.Equals(WiFiAccessStatus.Allowed))
            {
                //WiFi Access Granted For Current User


                //Get List Of Available Wifi Adapters In System
                Windows.Devices.Enumeration.DeviceInformationCollection WiFiAdapterList = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());

                //Check If Any WiFi Adapter Available
                if (WiFiAdapterList.Count > 0)
                {
                    //One Or More WiFi Adapter Available In System

                    //Select The First Wifi Adapter
                    WiFiAdapter wiFiAdapter = await WiFiAdapter.FromIdAsync(WiFiAdapterList[0].Id);

                    //Update UI 
                    foreach (var network in wiFiAdapter.NetworkReport.AvailableNetworks)
                    {
                        Available_Hotspots_List.Items.Add(network.Ssid);
                    }
                }
                else
                {
                    //No WiFi Adapter Available In System

                    //Hide Current Dialog (Hotspot Configuration Dialog)
                    this.Hide();


                    /*Show Error Message Dialog To User
                    Error_Message_Dialog error_Message_Dialog = new Error_Message_Dialog();
                    error_Message_Dialog.Error_Dialog_Message.Text = "No WiFi Adapters found in your system.";
                    await error_Message_Dialog.ShowAsync();*/
                }
            }
            else
            {
                //WiFi Access Not Granted For Current User

                //Close Current Dialog (Hotspot Configuration Dialog)
                this.Hide();


                /*Show Error Dialog To User
                Error_Message_Dialog error_Message_Dialog = new Error_Message_Dialog();
                error_Message_Dialog.Error_Dialog_Message.Text = "WiFi access not granted for current user";
                await error_Message_Dialog.ShowAsync();*/



            }


        }








        private async void Get_Available_Hotspot_List()
        {
            //Get List Of Available Wifi Adapters In System
            Windows.Devices.Enumeration.DeviceInformationCollection WiFiAdapterList = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());

            if (WiFiAdapterList.Count != 0)
            {
                //Select The First Wifi Adapter In The List
                WiFiAdapter wifiAdapter = await WiFiAdapter.FromIdAsync(WiFiAdapterList[0].Id);

                //Scan For All Available Networks
                await wifiAdapter.ScanAsync();

                //Update The List (Combobox In UI)

                //Clear The Combobox First
                Available_Hotspots_List.Items.Clear();

                foreach (Windows.Devices.WiFi.WiFiAvailableNetwork Available_Wifi_Network in wifiAdapter.NetworkReport.AvailableNetworks)
                {
                    //Add Items One By One To Combobox
                    Available_Hotspots_List.Items.Add(Available_Wifi_Network.Ssid);

                    var credential = new PasswordCredential();
                    credential.Password = "abcd12345";
                    try
                    {
                        WiFiConnectionResult result = await wifiAdapter.ConnectAsync(Available_Wifi_Network, WiFiReconnectionKind.Manual, credential);

                        if (result.ConnectionStatus == WiFiConnectionStatus.Success)
                        {

                        }
                        else
                        {

                        }
                    }
                    catch (Exception)
                    {
                    }

                }
            }
        }



        /*Refresh Icon Pointer Enter Animation*/
        private void Refresh_Icon_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Refresh_Icon.Foreground = new SolidColorBrush(new UISettings().GetColorValue(UIColorType.AccentLight3));
        }

        /*Refresh Icon Pointer Exit Animation*/
        private void Refresh_Icon_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Refresh_Icon.Foreground = new SolidColorBrush(new UISettings().GetColorValue(UIColorType.Accent));
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize_Available_Networks();
        }
    }
}

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Linq;
using System.Text;
using Windows.Devices.Bluetooth;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using Windows.Storage;
using Windows.Devices.Enumeration;
using System.Diagnostics;



namespace Next_Home
{

    public sealed partial class Add_New_Device : Page
    {

        public static ulong Device_Bluetooth_ID { get; set; }
        public static String Device_Name { get; set; }

        public Add_New_Device()
        {
            this.InitializeComponent();
        }

        #region Add New Device Using Bluetooth

        private async void NEW_BLUETOOTH_DEVICE_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Reset visibility of connection info bar
            Device_Adition_Info_Bar.IsOpen = false;

            try
            {
                

                /*Show BLE Device Picker Dialog To User*/
                BLE_Device_Picker_Dialog BLE_Device_Picker = new BLE_Device_Picker_Dialog();
                BLE_Device_Picker.XamlRoot = XamlRoot;
                var BLE_Device_Picker_Result = await BLE_Device_Picker.ShowAsync();

                
                if (BLE_Device_Picker_Result == ContentDialogResult.Primary)
                {
                    //Check if Connection status is success
                    if (BLE_Device_Picker.Connection_Status == BLE_Device_Picker_Dialog.CONNECTION_STATUS.SUCCESS)
                    {

                        /*Show Device Authentication Dialog To User*/
                        Device_Authentication_Dialog New_Device_Authentication = new Device_Authentication_Dialog(Device_Name,Device_Bluetooth_ID);
                        New_Device_Authentication.XamlRoot = XamlRoot;
                        ContentDialogResult Authentication_Dialog_Result = await New_Device_Authentication.ShowAsync();


                        /*When Connect Button Is Pressed By User*/
                        if (Authentication_Dialog_Result == ContentDialogResult.Primary)
                        {
                            

                            /*Check If Any Field Is Empty*/
                            if (New_Device_Authentication.Device_Name == "" || New_Device_Authentication.Device_Password == "")
                            {
                                /*If Any Field Is Empty Show Error Message*/
                                throw new User_Input_Exception("Device name or password can't be blank");
                            }
                            else
                            {

                                /******************Show Busy (Connecting State) Progress Bar*****************/
                                Connecting_Progress_Bar.Visibility = Microsoft.UI.Xaml.Visibility.Visible;



                                /*If Device Authentication is successful, add the new device to local system*/

                                var result = await Authenticate_Device_Using_Password(Device_Bluetooth_ID, New_Device_Authentication.Device_Password,New_Device_Authentication.Device_Name);
                                if (result.Item1)
                                {
                                    /*Serialize Object To Write in file*/
                                    XmlSerializer BLE_Device_Object_Writer = new System.Xml.Serialization.XmlSerializer(typeof(Device_Details_Serializer));
                                    var String_Writer = new StringWriter();
                                    BLE_Device_Object_Writer.Serialize(String_Writer,(Device_Details_Serializer) result.Item2);
                                    var Content = String_Writer.ToString();



                                    /*Write Device Details To Application Local Folder*/
                                    Windows.Storage.StorageFolder Local_Folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                                    var File_Writer = await Local_Folder.CreateFileAsync(((Device_Details_Serializer)result.Item2).MAC_Address.Replace(':','_') + ".xml");
                                    await FileIO.WriteTextAsync(File_Writer, Content);


                                    /**Show Device Addition Success Message to user**/
                                    Device_Adition_Info_Bar.Title = "Device added successfully : ";
                                    Device_Adition_Info_Bar.Message = "Next Home Device, ID : " + Device_Bluetooth_ID.ToString() +
                                        ", added succesfully in system. Please refer to 'Added Devices' section to start controlling it.";
                                    Device_Adition_Info_Bar.Severity = InfoBarSeverity.Success;
                                    Device_Adition_Info_Bar.IsOpen = true;
                                }


                            }

                            }
                    }
                    else
                    {
                        /*If BLE Device Picker Dialog returns Connection status as Fail*/
                        throw new Bluetooth_Connection_Exception("Unable to connect !");
                    }
                }

            }
            catch (Exception error)
            {
                if(error.GetType() == typeof (System.Runtime.InteropServices.COMException))
                {
                    /*Update Info Bar With Proper error message*/
                    Device_Adition_Info_Bar.Title = "Information : ";
                    Device_Adition_Info_Bar.Message = "Selected device is already added in system.";
                    Device_Adition_Info_Bar.Severity = InfoBarSeverity.Informational;
                    Device_Adition_Info_Bar.IsOpen = true;
                }
                else
                {
                    /*Update Info Bar With Proper error message*/
                    Device_Adition_Info_Bar.Title = "Error : ";
                    Device_Adition_Info_Bar.Message = error.Message;
                    Device_Adition_Info_Bar.Severity = InfoBarSeverity.Error;
                    Device_Adition_Info_Bar.IsOpen = true;
                }

            }

            finally
            {
                /******************Reset Progress Bar and hide it *****************/
                Connecting_Progress_Bar.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                Connecting_Progress_Bar.ShowError = false;
            }



        }

        #endregion


        #region Device_Authentication_Using_Password

        private async Task<(bool, Device_Details_Serializer)> Authenticate_Device_Using_Password(ulong Bluetooth_ID, String Password, String Device_Name)
        {
            BluetoothLEDevice Selected_Device = null;
            GattCharacteristic characteristic = null;

            try
            {
                //Create BLE Device instace using the seleced device ID
                Selected_Device = await BluetoothLEDevice.FromBluetoothAddressAsync(Bluetooth_ID);

                //Get SERVICE_UUID from MainWindow.Xaml.cs file and validate device connection
                GattDeviceServicesResult BLE_Service = await Selected_Device.GetGattServicesForUuidAsync(new Guid(MainWindow.Get_Service_UUID()));

                if (BLE_Service.Status == GattCommunicationStatus.Success)
                {

                    //Get CHARACTERISTIC_UUID from MainWindow.Xaml.cs file and validate device connection further
                    GattCharacteristicsResult gattCharacteristic = await BLE_Service.Services.Single(service_UUID => service_UUID.Uuid == new Guid(MainWindow.Get_Service_UUID()))
                        .GetCharacteristicsForUuidAsync(new Guid(MainWindow.Get_Chacracteristic_UUID()));

                    if (gattCharacteristic.Status == GattCommunicationStatus.Success)
                    {
                        characteristic = gattCharacteristic.Characteristics.Single(c => c.Uuid == new Guid(MainWindow.Get_Chacracteristic_UUID()));

                        //Read connection properties and validate if REQUIRED properties are there
                        GattCharacteristicProperties properties = characteristic.CharacteristicProperties;

                        if (properties.HasFlag(GattCharacteristicProperties.Read) && properties.HasFlag(GattCharacteristicProperties.Write) && properties.HasFlag(GattCharacteristicProperties.Notify))
                        {
                            
                            //Add descriptor, and register event listner for sending and validating final message string
                            GattCommunicationStatus Connection_Descriptor = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

                            //Write Connection check message to connected device
                            await characteristic.WriteValueAsync(CryptographicBuffer.ConvertStringToBinary(MainWindow.Get_Default_key(), BinaryStringEncoding.Utf8));
                           
                            GattReadResult Echo_Key = await characteristic.ReadValueAsync(BluetoothCacheMode.Uncached);   //Wait and read for predeined Echo Key key from external device
                    
                            if (Echo_Key.Status == GattCommunicationStatus.Success)
                            {
                                /*Store received data (Echo Key) into byte byte array*/
                                DataReader Echo_Key_Reader = DataReader.FromBuffer(Echo_Key.Value);
                                byte[] Echo_Key_Bytes = new byte[Echo_Key_Reader.UnconsumedBufferLength];
                                Echo_Key_Reader.ReadBytes(Echo_Key_Bytes);

                                /*Check if handshake key is valid*/
                                if (Encoding.UTF8.GetString(Echo_Key_Bytes) == MainWindow.Get_Echo_Key())
                                {
                                    /*Validate device using password*/
                                   await characteristic.WriteValueAsync(CryptographicBuffer.ConvertStringToBinary("PASSWORD:" + Password, BinaryStringEncoding.Utf8));
                                  
                                    GattReadResult Password_Validation = await characteristic.ReadValueAsync(BluetoothCacheMode.Uncached);

                                    if(Password_Validation.Status == GattCommunicationStatus.Success)
                                    {
                                        /*Store received data (Password validation message) into byte byte array*/
                                        DataReader Password_Key_Reader = DataReader.FromBuffer(Password_Validation.Value);
                                        byte[] Password_Key_Bytes = new byte[Password_Key_Reader.UnconsumedBufferLength];
                                        Password_Key_Reader.ReadBytes(Password_Key_Bytes);

                                        /*Check for password validation message from external device */
                                        if(Encoding.UTF8.GetString(Password_Key_Bytes) == "PASSWORD_VERIFIED")
                                        {

                                            /*Get Device Type (Number Of Modules supported on system*/
                                            await characteristic.WriteValueAsync(CryptographicBuffer.ConvertStringToBinary("TYPE", BinaryStringEncoding.Utf8));
                                            GattReadResult Device_Type = await characteristic.ReadValueAsync(BluetoothCacheMode.Uncached);

                                            if(Device_Type.Status == GattCommunicationStatus.Success)
                                            {
                                                DataReader Device_Type_Reader = DataReader.FromBuffer(Device_Type.Value);
                                                byte[] Device_Type_Bytes = (new byte[Device_Type_Reader.UnconsumedBufferLength]);
                                                Device_Type_Reader.ReadBytes(Device_Type_Bytes);

                                                /*Make request to get device MAC Address*/
                                                await characteristic.WriteValueAsync(CryptographicBuffer.ConvertStringToBinary("MAC_ADDRESS", BinaryStringEncoding.Utf8));
                                                GattReadResult Device_MAC_Address = await characteristic.ReadValueAsync(BluetoothCacheMode.Uncached);

                                                if(Device_MAC_Address.Status == GattCommunicationStatus.Success)
                                                {
                                                    DataReader Device_MAC_Address_Reader = DataReader.FromBuffer(Device_MAC_Address.Value);
                                                    byte[] Device_MAC_Address_Bytes = new byte [Device_MAC_Address_Reader.UnconsumedBufferLength];
                                                    Device_MAC_Address_Reader.ReadBytes(Device_MAC_Address_Bytes);


                                                    Device_Details_Serializer New_Device_Details = new Device_Details_Serializer();
                                                    New_Device_Details.Name = Device_Name;
                                                    New_Device_Details.MAC_Address = Encoding.UTF8.GetString(Device_MAC_Address_Bytes);
                                                    New_Device_Details.Type = Encoding.UTF8.GetString(Device_Type_Bytes);
                                                    New_Device_Details.Bluetooth_Id = Bluetooth_ID;
                                                    New_Device_Details.Password = Password;

                                                    _ = await Selected_Device.DeviceInformation.Pairing.PairAsync(DevicePairingProtectionLevel.None);
                                                    return (true, New_Device_Details);
                                                }
                                                else
                                                {
                                                    throw new Bluetooth_Connection_Exception("Bluetooth connection error");
                                                }

                                            }
                                            else
                                            {
                                                throw new Bluetooth_Connection_Exception("Bluetooth connection error");
                                            }
                                            
                                        }
                                        else
                                        {
                                            throw new User_Input_Exception("Invalid Password");
                                        }
                                    }
                                    else
                                    {
                                        throw new Bluetooth_Connection_Exception("Bluetooth connection error !");
                                    }
                                    
                                }
                                else
                                {
                                    throw new Bluetooth_Connection_Exception("Unsupported Device !");
                                }
                                
                            }
                            else
                            {
                                throw new Bluetooth_Connection_Exception("Bluetooth connection error !");
                            }
                            
                        }
                        else
                        {
                            throw new Bluetooth_Connection_Exception("Unsupported Device !");
                        }
                        
                    }
                    else
                    {
                        throw new Bluetooth_Connection_Exception("Bluetooth connection error !");
                    }
                    
                }
                else
                {
                    throw new Bluetooth_Connection_Exception("Bluetooth connection error !");
                }
                
            }
            catch(Exception e)
            {
                throw new Bluetooth_Connection_Exception(e.Message);
            }
            finally
            {
                characteristic?.Service.Dispose();
                Selected_Device?.Dispose();
            }
        }
        #endregion
    }
}

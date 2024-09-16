using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Bluetooth;
using System;
using System.Linq;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.Devices.Enumeration;



namespace Next_Home
{
    public sealed partial class BLE_Device_Picker_Dialog : ContentDialog
    {
        #region Global_Variables_Initialization

        //Store the final connection status after validating device connection
        public enum CONNECTION_STATUS { SUCCESS, FAIL };

        public CONNECTION_STATUS Connection_Status = CONNECTION_STATUS.FAIL;   //Default initialization of Connection status is fail 


        /*Observable Collection here used for XAML Data binding (Add new device when found) */
        private ObservableCollection<BLE_Available_Device_Details_View> available_Devices = new ObservableCollection<BLE_Available_Device_Details_View>();

        public ObservableCollection<BLE_Available_Device_Details_View> Available_Devices
        {
            get { return available_Devices; }
            set { available_Devices = value; }
        }

       


        private BluetoothLEDevice Selected_Device;
        private GattCharacteristic characteristic;



        private BluetoothLEAdvertisementWatcher BLEDeviceWatcher;

        public BLE_Available_Device_Details_View Get_Device_Details = null;     //Used to store and use Device details, which is selected by user from the available devices list


        #endregion

        #region Default_Constructor_Initialization

        public BLE_Device_Picker_Dialog()
        {
            DataContext = this;     //Used for XAML Data Binding
            this.InitializeComponent();

            /*Start BLE devices discovery automatically, when this dialog loads */
            start_BLE_Advertisement_Watcher();

            
        }

        #endregion


        #region BLE_Device_Selected_By_User

        private void Select_Device_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Get the details of selected Device
            Get_Device_Details = (BLE_Available_Device_Details_View)e.ClickedItem;

            /*Enable Connect button*/
            this.IsPrimaryButtonEnabled=true;
            this.DefaultButton = ContentDialogButton.Primary;
            
            
        }

        #endregion


        #region Start_BLE_Advertisement_Watcher

        private void start_BLE_Advertisement_Watcher()
        {

            Available_Devices.Clear();  //Clear available devices list values (iff any) before initiating search

            /*Initialize and start BLE Advertisement watcher*/
            BLEDeviceWatcher = new BluetoothLEAdvertisementWatcher();   
            BLEDeviceWatcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromSeconds(0.5);
            BLEDeviceWatcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromSeconds(1);
            BLEDeviceWatcher.ScanningMode = BluetoothLEScanningMode.Active;

            BLEDeviceWatcher.Received += OnAdvertisementReceived;
            BLEDeviceWatcher.Stopped += Stop_Watcher;
            BLEDeviceWatcher.Start();

        }

        #endregion



        #region Add_BLE_Device_To_Observable_Collection

        private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs deviceInfo)
        {

            if (sender == BLEDeviceWatcher)
            {
                //Constraints to check and add proper BLE Devices only in the list
                if (isDeviceAlreadyAdded(deviceInfo.BluetoothAddress) == false && deviceInfo.Advertisement.LocalName != "")
                {

                    /******WINUI 3 Current Bug Fix ***********
                     * Correct code :await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { // My Action here });
                     * Bugfix code : _ = DispatcherQueue.TryEnqueue(() =>{}); */

                    /*We need to use Dispatcher Queue here, because our Observable Collection
                     * is used for data binding (update UI element). Otherwise, program will
                     * throw runtime error during execution */

                    _ = DispatcherQueue.TryEnqueue(() =>
                    {
                        Available_Devices.Add(new BLE_Available_Device_Details_View(deviceInfo.BluetoothAddress, deviceInfo.Advertisement.LocalName));
                    });


                }
            }
        }


        /*Method To Check if found device is already added in List (Observable Collection)*/
        private bool isDeviceAlreadyAdded(ulong id)
        {
            foreach (BLE_Available_Device_Details_View device in Available_Devices)
            {
                if (device.Device_ID == id)
                {
                    return true;
                }
            }
            return false;
        }


        #endregion



        #region Callback_Method_For_BLE_Advertisement_Watcher_Stop

        private void Stop_Watcher(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {
            BLEDeviceWatcher.Stop();
            BLEDeviceWatcher.ScanningMode = BluetoothLEScanningMode.Passive;
            BLEDeviceWatcher.Received -= OnAdvertisementReceived;
        }

        #endregion

        #region Device_Basic_Validation_On_Connect_Button_Click

        private async void BLE_Device_Picker_Dialog_ConnectButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
           
            

            /***For blocking the thread, until current async method is completed***/
            ContentDialogButtonClickDeferral deferral = args.GetDeferral();

            

            //Release BLE Advertisement Watcher as per Microsoft standard guidelines
            BLEDeviceWatcher.Stop();

            try
            {
               
                    Get_Device_Details.Is_Selected = true;
                    List_View.IsHitTestVisible = false;
                    BLE_Devices_Search_Progress_Bar.ShowPaused = true;
                    
                    
                //Constraint, just to avoid any unfavourable circumstances, iff any 
                if (Get_Device_Details != null)
                {
                    //Create BLE Device instace using the seleced device ID
                    Selected_Device = await BluetoothLEDevice.FromBluetoothAddressAsync(Get_Device_Details.Device_ID);

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

                            if(properties.HasFlag(GattCharacteristicProperties.Read) && properties.HasFlag(GattCharacteristicProperties.Write) && properties.HasFlag(GattCharacteristicProperties.Notify))
                            {
                                //Add descriptor, and register event listner for sending and validating final message string
                                GattCommunicationStatus Connection_Descriptor = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

                                //Write Connection check message to connected device
                                await characteristic.WriteValueAsync(CryptographicBuffer.ConvertStringToBinary(MainWindow.Get_Default_key(), BinaryStringEncoding.Utf8));
                                var Echo_Key = await characteristic.ReadValueAsync(BluetoothCacheMode.Uncached);

                                
                                if (Echo_Key.Status == GattCommunicationStatus.Success)
                                {

                                    /*Store received data (Echo Key) into byte byte array*/
                                    DataReader Echo_Key_Reader = DataReader.FromBuffer(Echo_Key.Value);
                                    byte[] Echo_Key_Bytes = new byte[Echo_Key_Reader.UnconsumedBufferLength];
                                    Echo_Key_Reader.ReadBytes(Echo_Key_Bytes);


                                    if (Encoding.UTF8.GetString(Echo_Key_Bytes) == MainWindow.Get_Echo_Key())
                                    {
                                        await Task.Run( () => {
                                            Connection_Status = CONNECTION_STATUS.SUCCESS;
                                            Add_New_Device.Device_Bluetooth_ID = Get_Device_Details.Device_ID;
                                            Add_New_Device.Device_Name = Get_Device_Details.Device_Name;
                                        });

                                    }
                                }
                            }

                        }
                    }
                }


            }
            catch (Exception)
            {
                Connection_Status = CONNECTION_STATUS.FAIL;
            }
            finally
            {
                /*Close current BLE connection*/
                characteristic?.Service.Dispose();

                Selected_Device?.Dispose();
                
                /*Release the thread block*/
                deferral.Complete();
                
            }
            

        }




        #endregion

        

        #region Close_Button_Click_Event_Handler
        private void BLE_Device_Picker_Dialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            BLEDeviceWatcher.Stop();
        }
        #endregion
    }
}

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;


namespace Next_Home
{

    public sealed partial class Added_Devices_List : Page
    {
        public ObservableCollection<Device_Details_Serializer> device_list = new ObservableCollection<Device_Details_Serializer>();  //List for locally stored devices
        public ObservableCollection<Device_Details_Serializer> Device_List
        {
            get { return device_list; }
            set { device_list = value; }
        }


        public Added_Devices_List()
        {
            this.InitializeComponent();
            DataContext = this;
            Display_Local_Devices();
        }


        #region Display all locally stored devices

        public async void Display_Local_Devices()
        {
            _ = await Device_List_DisplayAsync();     //Load All Local Devices
            Local_Devices_List_Loading_Ring.IsActive = false;  //Dispose Loading Animation
        }


        public async Task<bool> Device_List_DisplayAsync()
        {
            /*Method To Load All Local Devices*/

            StorageFolder Local_Storage_Folder = ApplicationData.Current.LocalFolder;   //Get Local Folder For Current Application
            IReadOnlyList<StorageFile> Device_File_List = await Local_Storage_Folder.GetFilesAsync();   //Get All Available Files In Local Folder Asynchronously


            foreach (StorageFile Device in Device_File_List)
            {
                StorageFile Local_Device = await Local_Storage_Folder.GetFileAsync(Device.Name);        //Open File One By One
                string Content = await FileIO.ReadTextAsync(Local_Device);                              //Read Content Of File

                System.Xml.Serialization.XmlSerializer Device_Instance = new System.Xml.Serialization.XmlSerializer(typeof(Device_Details_Serializer));    //Initialize Serializer

                Device_Details_Serializer device;  //Create New Instance Of Device_Details Class TO Deserialize Object
                device = (Device_Details_Serializer)Device_Instance.Deserialize(new StringReader(Content));    //Deserialize Object

                Device_List.Add(new Device_Details_Serializer(device.Name, device.MAC_Address, device.Type));    //Add The Device To List

            }
            return true;    //Representing Function Execution Is Over
        }

        #endregion



        private void Local_Device_List_ItemClickAsync(object sender, ItemClickEventArgs e)
        {
            /*
            //Get Which Item Is Clicked
            var result = (Added_Device_Details_Display)e.ClickedItem;


            /*Show Establish Connection Dialog
            Establish_Device_Connection establish_Device_Connection = new Establish_Device_Connection();

            //Initialize Values To Establish Connection Dialog
            establish_Device_Connection.Device_Name.Text = result.Name;
            establish_Device_Connection.Device_MAC_Address.Text = result.MAC_Address;
            establish_Device_Connection.Device_Type.Text = result.Type;

            Establish_Device_Connection.Update_Connection_State();
            //Show The Dialog To User
            await establish_Device_Connection.ShowAsync(); */

            
            MainWindow.Main_Window_Reference.MAIN_NAVIGATION_WINDOW.Header = "Device Control";
            MainWindow.Main_Window_Reference.MAIN_CONTENT_FRAME.Navigate(typeof(Device_Control_Bluetooth_Device_Type_8));

        }

        #region Display some basic animation on mouse hover

        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            grid.RenderTransform = new CompositeTransform() { CenterX=-2, CenterY=-2 , ScaleX = 1.04, ScaleY = 1.04 };
            
            grid.Background = (Microsoft.UI.Xaml.Media.Brush) Application.Current.Resources["LayerOnMicaBaseAltFillColorSecondaryBrush"];


        }

        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            grid.RenderTransform = new CompositeTransform() { ScaleX = 1, ScaleY = 1 };
            grid.Background = null;
        }
        #endregion
    }
}

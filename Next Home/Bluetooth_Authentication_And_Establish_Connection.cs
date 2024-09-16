using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Bluetooth;
using Windows.Networking.Sockets;
using Windows.Storage;

namespace Next_Home
{
    internal class Bluetooth_Authentication_And_Establish_Connection
    {

        /****************Method To Get Connection Of Device From File*****************/
        public static async Task<Device_Details_Serializer> Load_Connection_ID(string MAC_Address)
        {


            try
            {
                String File_Name = MAC_Address + ".xml";

                //Open Device Specific File To Get Details
                StorageFile loadDevice = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(File_Name);

                //Read Content From File
                string Content = await FileIO.ReadTextAsync(loadDevice);

                //Initialize XML Serializer
                System.Xml.Serialization.XmlSerializer deviceSerializer = new System.Xml.Serialization.XmlSerializer(typeof(Device_Details_Serializer));

                //Deserialize Object
                Device_Details_Serializer device = (Device_Details_Serializer)deviceSerializer.Deserialize(new StringReader(Content));

                return device;

            }
            catch (Exception)
            {
                return null;
            }



        }





        /******************Method To Authenticate Device And Establish Connection**************************/
        public static async void Authenticate_And_Connect(String MAC_Address)
        {
            try
            {

                /*Define Default Key And Echo Key For Identifying Supported Device*/
                String Default_Key = "AKQY1-MBJC4-BJIET-RDRT9-CGCG4";
                String Echo_Key = "AKQY1-MBJC4-BJIE5-DFKE6-GHIE7";


                //Disconnect (if already connected)
                Current_Connected_Device_Connection_Details.DisconnectBluetoothConnection();



                Device_Details_Serializer device_Details = await Load_Connection_ID(MAC_Address);

                /************************Initialize RFComm Service On Selected Device***************************/
                var Btservice = await BluetoothDevice.FromIdAsync(device_Details.Bluetooth_Id.ToString());
                var Rfservice = await Btservice.GetRfcommServicesForIdAsync(RfcommServiceId.SerialPort, BluetoothCacheMode.Uncached);


                /*************Create New Socket For Communication**************/
                var Communication_socket = new StreamSocket();



                /*************Initialize Connection Details On Socket*********************/
                await Communication_socket.ConnectAsync(Rfservice.Services[0].ConnectionHostName, Rfservice.Services[0].ConnectionServiceName);

                /*Initialize StreamWriter And StreamReader For Data Communication*/
                StreamWriter Writer = new StreamWriter(Communication_socket.OutputStream.AsStreamForWrite());
                StreamReader Reader = new StreamReader(Communication_socket.InputStream.AsStreamForRead());


                /*Write Default Key To Device*/
                await Writer.WriteAsync(Default_Key);       //Write Default Key To Device
                await Writer.FlushAsync();                      //Complete Write Operation


                /*Read Echo Key From Device*/
                String Return_Key = Reader.ReadLine();



                /*Write User Input Password To Device*/
                await Writer.WriteAsync(device_Details.Password);
                await Writer.FlushAsync();


                /*Read External Device Password*/
                String Return_Password = await Reader.ReadLineAsync();


                /*Read External Device Type*/
                String Device_Type = await Reader.ReadLineAsync();

                /*If Key Don't Match Then Show Error Message To User*/
                if (Return_Key != Echo_Key)
                {
                    /*Show Error Message Dialog
                    Error_Message_Dialog Password_Or_Key_Error = new Error_Message_Dialog();
                    Password_Or_Key_Error.Title = "Incompitable Device";
                    Password_Or_Key_Error.Error_Dialog_Message.Text = Return_Password;
                    await Password_Or_Key_Error.ShowAsync();
                    //Disconnect (if already connected)
                    Current_Connected_Device_Connection_Details.DisconnectBluetoothConnection();*/
                }

                /*If Password Don't Match Then Show Error Message To User*/
                else if (Return_Password != device_Details.Password)
                {
                    /*Show Error Message Dialog
                    Error_Message_Dialog Password_Or_Key_Error = new Error_Message_Dialog();
                    Password_Or_Key_Error.Title = "Password Mismatch";
                    Password_Or_Key_Error.Error_Dialog_Message.Text = "Password Does Not Match";
                    await Password_Or_Key_Error.ShowAsync();
                    //Disconnect (if already connected)
                    Current_Connected_Device_Connection_Details.DisconnectBluetoothConnection();*/
                }
                else
                {
                    /*
                    Success_Message_Dialog s = new Success_Message_Dialog();
                    s.Success_Dialog_Message.Text = "Connection succeded";
                    await s.ShowAsync();*/

                    //Update Details Of Currently Connected Device
                    Current_Connected_Device_Connection_Details.InitializeBluetoothConnection(Communication_socket,
                    Reader, Writer, MAC_Address, Establish_Device_Connection.Establish_Device_Connection_Reference.Device_Type.Text);
                }




            }
            catch (Exception e)
            {
                //Close Establish Connection Dialog
                Establish_Device_Connection.Establish_Device_Connection_Reference.Hide();

                /*
                //Show Error Dialog
                Error_Message_Dialog Connection_Error_Dialog = new Error_Message_Dialog();
                //Connection_Error_Dialog.Error_Dialog_Message.Text = "Connection Could Not Be Established -5";

                Connection_Error_Dialog.Error_Dialog_Message.Text = e.Message.ToString();
                await Connection_Error_Dialog.ShowAsync(); */
                //Disconnect (if already connected)
                Current_Connected_Device_Connection_Details.DisconnectBluetoothConnection();
            }

        }
    }
}

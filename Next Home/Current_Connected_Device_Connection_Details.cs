using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace Next_Home
{
    internal static class Current_Connected_Device_Connection_Details
    {

        /*Define Connection States*/
        public static Boolean IS_ANY_DEVICE_CONNECTED = false;
        public static Boolean IS_CONNECTED_VIA_BLUETOOTH = false;
        public static Boolean IS_CONNECTED_VIA_WIFI = false;


        /*Define Device Details*/
        public static String DEVICE_MAC_ADDRESS { get; set; }
        public static String DEVICE_TYPE { get; set; }

        public static Boolean TEST_FLAG = true;


        /*Store Bluetooth Communication Instances*/
        public static StreamSocket Bluetooth_Connection_Stream_Socket { get; set; }
        public static StreamReader Bluetooth_Stream_Reader { get; set; }
        public static StreamWriter Bluetooth_Stream_Writer { get; set; }


        /*****************Initialize Bluetooth Connection Details After Successful Connection**********************/
        public static void InitializeBluetoothConnection(StreamSocket Bluetooth_Connection_Stream_Socket,
           StreamReader Bluetooth_Stream_Reader, StreamWriter Bluetooth_Stream_Writer, String MAC_Address, String DEVICE_TYPE)
        {

            //Reconnect (if already connected)
            DisconnectBluetoothConnection();


            //Store Connection References
            Current_Connected_Device_Connection_Details.Bluetooth_Connection_Stream_Socket = Bluetooth_Connection_Stream_Socket;
            Current_Connected_Device_Connection_Details.Bluetooth_Stream_Reader = Bluetooth_Stream_Reader;
            Current_Connected_Device_Connection_Details.Bluetooth_Stream_Writer = Bluetooth_Stream_Writer;
            Current_Connected_Device_Connection_Details.DEVICE_MAC_ADDRESS = MAC_Address;
            Current_Connected_Device_Connection_Details.DEVICE_TYPE = DEVICE_TYPE;

            //Toggle The Flags
            Current_Connected_Device_Connection_Details.IS_ANY_DEVICE_CONNECTED = true;
            Current_Connected_Device_Connection_Details.IS_CONNECTED_VIA_BLUETOOTH = true;

            Establish_Device_Connection.Update_Connection_State();
        }




        /************************Disconnect Bluetooth Connection*******************************/
        public static void DisconnectBluetoothConnection()
        {
            if (Current_Connected_Device_Connection_Details.Bluetooth_Connection_Stream_Socket != null)
            {

                //Close ReadStream
                Current_Connected_Device_Connection_Details.Bluetooth_Stream_Reader.Close();
                //Close Bluetooth WriteStream
                Current_Connected_Device_Connection_Details.Bluetooth_Stream_Writer.Close();

                //Close Connection
                Current_Connected_Device_Connection_Details.Bluetooth_Connection_Stream_Socket.Dispose();
            }


            //Toggle Connection Flags
            Current_Connected_Device_Connection_Details.IS_ANY_DEVICE_CONNECTED = false;
            Current_Connected_Device_Connection_Details.IS_CONNECTED_VIA_BLUETOOTH = false;

        }

        public async static Task<Boolean> Is_Bluetooth_Connection_Alive()
        {
            if (Current_Connected_Device_Connection_Details.Bluetooth_Connection_Stream_Socket != null)
            {
                try
                {
                    await Current_Connected_Device_Connection_Details.Bluetooth_Stream_Writer.WriteAsync("0");
                    await Current_Connected_Device_Connection_Details.Bluetooth_Stream_Writer.WriteAsync("1");
                    var Ping_Result = await Current_Connected_Device_Connection_Details.Bluetooth_Stream_Reader.ReadLineAsync();

                    if (Ping_Result.Equals("TRUE"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }


        public static void Test_Command()
        {
            try
            {

                Current_Connected_Device_Connection_Details.Bluetooth_Stream_Writer.WriteAsync("3");
                Current_Connected_Device_Connection_Details.Bluetooth_Stream_Writer.Flush();

                //Current_Connected_Device_Connection_Details.Bluetooth_Connection_Stream_Socket.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
            }

        }
    }
}

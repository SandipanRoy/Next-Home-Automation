using System;

namespace Next_Home
{
    public class Device_Details_Serializer
    {
        public ulong Bluetooth_Id { get; set; }
        public String MAC_Address { get; set; }
        public String Type { get; set; }
        public String Password { get; set; }
        public String Name { get; set; }

        public Device_Details_Serializer()
        {

        }

        public Device_Details_Serializer(string name, String mAC_Address, String type)
        {
            MAC_Address = mAC_Address;
            Type = type;
            Name = name;
        }
    }
}

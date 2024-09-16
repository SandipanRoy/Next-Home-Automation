using System;

namespace Next_Home
{
    public class Added_Device_Details_Display
    {

        public String Name { get; set; }
        public String Type { get; set; }
        public String MAC_Address { get; set; }



        public Added_Device_Details_Display(String Name, String Type, String MAC_Address)
        {
            this.Name = Name;
            this.Type = Type;
            this.MAC_Address = MAC_Address;
        }
    }
}

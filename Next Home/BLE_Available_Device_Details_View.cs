using System.ComponentModel;

namespace Next_Home
{
    /*This class is used for displaying device details in BLE_Device_Picker_Dialog*/
    public class BLE_Available_Device_Details_View : INotifyPropertyChanged
    {
        public ulong Device_ID { get; set; }
        public string Device_Name { get; set; }
        public bool is_Selected;

        public bool Is_Selected
        {
            get { return is_Selected; }
            set
            {
                is_Selected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Is_Selected)));
            }
        }
       

        public BLE_Available_Device_Details_View(ulong Device_ID, string Device_Name)
        {
           this.Device_ID = Device_ID;
           this.Device_Name = Device_Name;
            Is_Selected = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

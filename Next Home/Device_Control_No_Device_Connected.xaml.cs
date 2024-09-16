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
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Next_Home
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Device_Control_No_Device_Connected : Page
    {
        public Device_Control_No_Device_Connected()
        {
            this.InitializeComponent();
        }


        /****************Method Called When User Choose To Add A New Device*********************/
        private void Add_New_Device_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Navigate To Add New Device Page
            MainWindow.Main_Window_Reference.NEW_DEVICE_Tapped(new Object(), new TappedRoutedEventArgs());
        }



        /*******************Method Called When User Choose To Connect An Added Device********************/
        private void Open_Added_Devices_List_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MainWindow.Main_Window_Reference.ADDED_DEVICES_Tapped(new object(), new TappedRoutedEventArgs());
        }
    }
}

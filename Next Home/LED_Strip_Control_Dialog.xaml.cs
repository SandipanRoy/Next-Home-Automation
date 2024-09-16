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
    public sealed partial class LED_Strip_Control_Dialog : ContentDialog
    {

        public enum Result
        {
            Apply,
            Cancel
        }

        Result ColorDialogResult { get; set; }

        LED_Strip_Control_Dialog LED_Strip_Control_Dialog_Reference { get; set; }
        public LED_Strip_Control_Dialog()
        {
            this.InitializeComponent();
            LED_Strip_Control_Dialog_Reference = this;
        }


        /*Executed When User Press Cancel Button On LED Control Dialog*/
        private void Color_Dialog_Cancel_Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (LED_Strip_Control_Dialog_Reference != null)
            {
                this.Hide();
                ColorDialogResult = Result.Cancel;
            }
        }
    }
}

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
    public sealed partial class Windows_Hello_Not_Configured_Dialog : ContentDialog
    {
        public Windows_Hello_Not_Configured_Dialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //Open Settings App To Configure Windows Hello
            Configure_Windows_Hello();
        }


        /*Method To Open Settings To Configure Windows Hello*/
        public async void Configure_Windows_Hello()
        {
            Boolean result = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:signinoptions"));
        }
    }
}

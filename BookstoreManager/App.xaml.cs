using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BookstoreManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NjI0MjEzQDMyMzAyZTMxMmUzMGtFWUNJaEsydC9CbGpGSE1DdjQxNVVYRkxwNDh0OSt5cmNOcGpOMFdzdEU9");
        }
    }
}

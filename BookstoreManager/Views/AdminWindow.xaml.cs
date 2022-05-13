using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.Windows.Shapes;

namespace BookstoreManager.Views
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            //Main.Content = Pages.DashboardPage;
            Style = (Style)FindResource("WindowStyle");
        }
        private void SfNavigationDrawer_ItemClicked(object sender, Syncfusion.UI.Xaml.NavigationDrawer.NavigationItemClickedEventArgs e)
        {
            switch(e.Item.Name)
            {
                case "NavCustomer":         
                    Main.Content = Pages.ManageCustomerPage;
                    break;
                case "NavBookList":
                    Main.Content = Pages.BookListPage;
                          break;
                case "NavBookType":
                    Main.Content = Pages.BookTypePage;
                    break;
                case "NavDebtReport":
                    Main.Content = Pages.DebtReportPage;
                    break;
                case "NavInvReport":
                    Main.Content = Pages.InventoryReportPage;
                    break;
                case "NavRegulation":
                    Main.Content = Pages.RegulationPage;
                    break;
                case "LogOut":
                    this.Close();
                    break;


            }
        }
    }
}

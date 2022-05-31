using BookstoreManager.ViewModels.ReceiptsViewModels;
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

namespace BookstoreManager.Views.Receipts
{
    /// <summary>
    /// Interaction logic for AddNewCustomer.xaml
    /// </summary>
    public partial class AddNewCustomer : Window
    {
        public AddNewCustomer(ReceiptsViewModel vm)
        {
            AddCustomerModel adVM = new AddCustomerModel(vm);
            this.DataContext = adVM;
            InitializeComponent();
        }
    }
}

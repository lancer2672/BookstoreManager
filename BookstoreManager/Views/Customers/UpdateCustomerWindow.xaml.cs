using BookstoreManager.ViewModels;
using BookstoreManager.ViewModels.Customers;
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

namespace BookstoreManager.Views.Customers
{
    /// <summary>
    /// Interaction logic for UpdateCustomerWindow.xaml
    /// </summary>
    public partial class UpdateCustomerWindow : Window
    {
        public UpdateCustomerWindow(ManageCustomerViewModel vm)
        {
            InitializeComponent();
            UpdateCustomerViewModel updVM = new UpdateCustomerViewModel(vm);
            this.DataContext = updVM;
        }
    }
}

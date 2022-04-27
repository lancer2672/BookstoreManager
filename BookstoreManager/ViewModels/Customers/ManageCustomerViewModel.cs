using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BookstoreManager.Models;
using BookstoreManager.Models.Db;
using BookstoreManager.ViewModels.Customers;

namespace BookstoreManager.ViewModels
{
    public class ManageCustomerViewModel:BaseViewModel
    {
        private ObservableCollection<ViewCustomer> _listManageCustomer;

        public ObservableCollection<ViewCustomer> ListManageCustomer { get { return _listManageCustomer; } set { _listManageCustomer = value; OnPropertyChanged(nameof(_listManageCustomer)); } }

        public ICommand COpenAddCustomerWindow { get; set; }
        public ICommand CDeleteCustomer { get; set; }
        public ICommand CUpdateCustomer { get; set; }
        public ManageCustomerViewModel()
        {
            ListManageCustomer = new ObservableCollection<ViewCustomer>();

            COpenAddCustomerWindow = new RelayCommand<ViewCustomer>((p) => { return true; }, (p) => { OpenAddCustomerWindow(); });

        }
        public void OpenAddCustomerWindow()
        {
            AddCustomerWindow AddWindow = new AddCustomerWindow(this);
            AddWindow.Show();
        }
    }
}

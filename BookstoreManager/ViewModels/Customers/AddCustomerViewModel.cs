using BookstoreManager.Models;
using BookstoreManager.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace BookstoreManager.ViewModels.Customers
{
    public class AddCustomerViewModel:BaseViewModel
    {
        private string _customerName;
        private string _customerAdress;
        private string _customerId;
        private string _customerEmail;
        private string _customerPhoneNumber;
        private string _customerDebt;

        public string CustomerName {  get {   return _customerName;  }  set { _customerName = value;  OnPropertyChanged(nameof(CustomerName)); }  }
        public string CustomerAdress { get { return _customerAdress; } set { _customerAdress = value; OnPropertyChanged(nameof(CustomerAdress)); } }
        public string CustomerId { get { return _customerId; } set { _customerId = value; OnPropertyChanged(nameof(CustomerId)); } }
        public string CustomerEmail { get { return _customerEmail; } set { _customerEmail = value; OnPropertyChanged(nameof(CustomerEmail)); } }
        public string CustomerDebt { get { return _customerDebt; } set { _customerDebt = value; OnPropertyChanged(nameof(CustomerDebt)); } }
        public string CustomerPhoneNumber {  get { return _customerPhoneNumber; }  set { _customerPhoneNumber = value; OnPropertyChanged(nameof(CustomerPhoneNumber));  } }

        public ICommand CAddCustomer { get; set; }

        private ManageCustomerViewModel _customerViewModel;
        public AddCustomerViewModel(ManageCustomerViewModel CustomerVM)
        {

            _customerViewModel = CustomerVM;
            CAddCustomer = new RelayCommand<object>((p) => { return true; }, (p) => { AddCustomer(); });
            
        }
        public void AddCustomer()
        {
            KHACHHANG newCustomer = new KHACHHANG();
            newCustomer.DiaChi = CustomerAdress;
            newCustomer.HoTen = CustomerName;
            newCustomer.Email = CustomerEmail;
            newCustomer.DienThoai = CustomerPhoneNumber;
            newCustomer.TongNo = 0;
            DataProvider.Ins.DB.KHACHHANGs.Add(newCustomer);
            DataProvider.Ins.DB.SaveChanges();
            _customerViewModel.LoadListCustomer();
            RefreshAddCustomerForm();
        }
        public void RefreshAddCustomerForm()
        {
            CustomerName = "";
            CustomerAdress = "";
            CustomerEmail = "";
            CustomerPhoneNumber = "";
            CustomerDebt = "";
        }
    }
}

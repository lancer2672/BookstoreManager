using BookstoreManager.Models;
using BookstoreManager.Models.Db;
using MaterialDesignThemes.Wpf;
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
        private string _customerAddress;
        private string _customerId;
        private string _customerEmail;
        private string _customerPhoneNumber;
        private string _customerDebt;
        private ManageCustomerViewModel _customerViewModel;
        private SnackbarMessageQueue _myMessageQueue;

        public SnackbarMessageQueue MyMessageQueue { get { return _myMessageQueue; } set { _myMessageQueue = value; OnPropertyChanged(nameof(MyMessageQueue)); } }
        public string CustomerName {  get {   return _customerName;  }  set { _customerName = value;  OnPropertyChanged(nameof(CustomerName)); }  }
        public string CustomerAddress { get { return _customerAddress; } set { _customerAddress = value; OnPropertyChanged(nameof(CustomerAddress)); } }
        public string CustomerId { get { return _customerId; } set { _customerId = value; OnPropertyChanged(nameof(CustomerId)); } }
        public string CustomerEmail { get { return _customerEmail; } set { _customerEmail = value; OnPropertyChanged(nameof(CustomerEmail)); } }
        public string CustomerDebt { get { return _customerDebt; } set { _customerDebt = value; OnPropertyChanged(nameof(CustomerDebt)); } }
        public string CustomerPhoneNumber {  get { return _customerPhoneNumber; }  set { _customerPhoneNumber = value; OnPropertyChanged(nameof(CustomerPhoneNumber));  } }

        public ICommand CAddCustomer { get; set; }

      
        public AddCustomerViewModel(ManageCustomerViewModel CustomerVM)
        {

            _customerViewModel = CustomerVM;
            CAddCustomer = new RelayCommand<object>((p) => { return true; }, (p) => { AddCustomer(); });

            MyMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(4000));
            MyMessageQueue.DiscardDuplicates = true;

        }
        public void AddCustomer()
        {
            KHACHHANG newCustomer = new KHACHHANG();
            newCustomer.DiaChi = CustomerAddress;
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
            CustomerAddress = "";
            CustomerEmail = "";
            CustomerPhoneNumber = "";
            CustomerDebt = "";
        }
    }
}

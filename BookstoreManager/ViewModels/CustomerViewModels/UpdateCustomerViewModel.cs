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
    public class UpdateCustomerViewModel:BaseViewModel
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
        public string CustomerName { get { return _customerName; } set { _customerName = value; OnPropertyChanged(nameof(CustomerName)); } }
        public string CustomerAddress { get { return _customerAddress; } set { _customerAddress = value; OnPropertyChanged(nameof(CustomerAddress)); } }
        public string CustomerId { get { return _customerId; } set { _customerId = value; OnPropertyChanged(nameof(CustomerId)); } }
        public string CustomerEmail { get { return _customerEmail; } set { _customerEmail = value; OnPropertyChanged(nameof(CustomerEmail)); } }
        public string CustomerDebt { get { return _customerDebt; } set { _customerDebt = value; OnPropertyChanged(nameof(CustomerDebt)); } }
        public string CustomerPhoneNumber { get { return _customerPhoneNumber; } set { _customerPhoneNumber = value; OnPropertyChanged(nameof(CustomerPhoneNumber)); } }

        public ICommand CUpdateCustomer { get; set; }
    

        public UpdateCustomerViewModel(ManageCustomerViewModel CustomerVM)
        {
            _customerViewModel = CustomerVM;
            LoadUpdWindow();
            CUpdateCustomer = new RelayCommand<object>((p) => { return true; }, (p) => { UpdateCustomer(); });

            MyMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(4000));
            MyMessageQueue.DiscardDuplicates = true;
        }
        public void LoadUpdWindow()
        {
            ViewCustomer updCustomer = _customerViewModel.SelectedCustomer;
            CustomerName = updCustomer.Name;
            CustomerAddress = updCustomer.Address;
            CustomerEmail = updCustomer.Email;
            CustomerPhoneNumber = updCustomer.PhoneNumber;
            CustomerDebt = updCustomer.Debt.ToString();

        }
        public void UpdateCustomer()
        {
            ViewCustomer customer = _customerViewModel.SelectedCustomer;
            KHACHHANG updCustomer = DataProvider.Ins.DB.KHACHHANGs.Where(p => p.MaKhachHang == customer.Id).FirstOrDefault();
            updCustomer.HoTen = CustomerName;
            updCustomer.DiaChi = CustomerAddress;
            updCustomer.DienThoai = customer.PhoneNumber;
            updCustomer.Email = customer.Email;
            updCustomer.TongNo = (decimal)customer.Debt;
            DataProvider.Ins.DB.SaveChanges();
            _customerViewModel.LoadListCustomer();
            RefreshAddCustomerForm();
            MyMessageQueue.Enqueue("Chỉnh sửa khách hàng thành công!");
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

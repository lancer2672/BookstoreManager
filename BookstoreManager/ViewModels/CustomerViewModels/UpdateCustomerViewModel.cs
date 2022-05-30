using BookstoreManager.Models;
using BookstoreManager.Models.Db;
using BookstoreManager.Resources.Utils;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookstoreManager.ViewModels.Customers
{
    public class UpdateCustomerViewModel:BaseViewModel
    {
        private string _customerName;
        private string _customerAddress;
        //private string _customerId;
        private string _customerEmail;
        private string _customerPhoneNumber;
        private decimal _customerDebt;
        private ManageCustomerViewModel _customerViewModel;
        public string CustomerName { get { return _customerName; } set { _customerName = value; OnPropertyChanged(nameof(CustomerName)); } }
        public string CustomerAddress { get { return _customerAddress; } set { _customerAddress = value; OnPropertyChanged(nameof(CustomerAddress)); } }
        //public string CustomerId { get { return _customerId; } set { _customerId = value; OnPropertyChanged(nameof(CustomerId)); } }
        public string CustomerEmail { get { return _customerEmail; } set { _customerEmail = value; OnPropertyChanged(nameof(CustomerEmail)); } }
        public decimal CustomerDebt { get { return _customerDebt; } set { _customerDebt = value; OnPropertyChanged(nameof(CustomerDebt)); } }
        public string CustomerPhoneNumber { get { return _customerPhoneNumber; } set { _customerPhoneNumber = value; OnPropertyChanged(nameof(CustomerPhoneNumber)); } }

        public ICommand CUpdateCustomer { get; set; }
    

        public UpdateCustomerViewModel(ManageCustomerViewModel CustomerVM)
        {
            _customerViewModel = CustomerVM;
            LoadUpdWindow();
            CUpdateCustomer = new RelayCommand<StackPanel>((p) => { return true; }, (p) => { UpdateCustomer(p); });

        }
        public void LoadUpdWindow()
        {
            ViewCustomer updCustomer = _customerViewModel.SelectedCustomer;
            CustomerName = updCustomer.Name;
            CustomerAddress = updCustomer.Address;
            CustomerEmail = updCustomer.Email;
            CustomerPhoneNumber = updCustomer.PhoneNumber;
            CustomerDebt = updCustomer.Debt;
        }
        public void UpdateCustomer(StackPanel t)
        {         
            if (Validator.IsValid(t))
            {
                ViewCustomer customer = _customerViewModel.SelectedCustomer;
                KHACHHANG updCustomer = DataProvider.Ins.DB.KHACHHANGs.Where(p => p.MaKhachHang == customer.Id).FirstOrDefault();
                updCustomer.HoTen = CustomerName;
                updCustomer.DiaChi = CustomerAddress;
                updCustomer.DienThoai = CustomerPhoneNumber;
                updCustomer.Email = CustomerEmail;
                updCustomer.TongNo = CustomerDebt;
                try
                {
                    DataProvider.Ins.DB.SaveChanges();
                }
                catch
                {
                    _customerViewModel.MyMessageQueue.Enqueue("Lỗi. Thông tin khách hàng không hợp lệ");
                    return;
                }
                _customerViewModel.LoadListCustomer();
                RefreshUpdateCustomerForm();
                _customerViewModel.MyMessageQueue.Enqueue("Chỉnh sửa thông tin thành công!");
            }
            else
            {
                _customerViewModel.MyMessageQueue.Enqueue("Lỗi. Thông tin khách hàng không hợp lệ");
            }
        }
        public void RefreshUpdateCustomerForm()
        {
            CustomerName = "";
            CustomerAddress = "";
            CustomerEmail = "";
            CustomerPhoneNumber = "";
            CustomerDebt = 0;
        }
      
    }
}

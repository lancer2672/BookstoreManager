using BookstoreManager.Models;
using BookstoreManager.Models.Db;
using BookstoreManager.Resources;
using BookstoreManager.Resources.Utils;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace BookstoreManager.ViewModels.Customers
{
    public class AddCustomerViewModel:BaseViewModel
    {
        private string _customerName;
        private string _customerAddress;
        private long _customerId;
        private string _customerEmail;
        private string _customerPhoneNumber;
        private decimal _customerDebt;
        private ManageCustomerViewModel _customerViewModel;
  
        public string CustomerName {  get {   return _customerName;  }  set { _customerName = value;  OnPropertyChanged(nameof(CustomerName)); }  }
        public string CustomerAddress { get { return _customerAddress; } set { _customerAddress = value; OnPropertyChanged(nameof(CustomerAddress)); } }
        public long CustomerId { get { return _customerId; } set { _customerId = value; OnPropertyChanged(nameof(CustomerId)); } }
        public string CustomerEmail { get { return _customerEmail; } set { _customerEmail = value; OnPropertyChanged(nameof(CustomerEmail)); } }
        public decimal CustomerDebt { get { return _customerDebt; } set { _customerDebt = value; OnPropertyChanged(nameof(CustomerDebt)); } }
        public string CustomerPhoneNumber {  get { return _customerPhoneNumber; }  set { _customerPhoneNumber = value; OnPropertyChanged(nameof(CustomerPhoneNumber));  } }

        public ICommand CAddCustomer { get; set; }

      
        public AddCustomerViewModel(ManageCustomerViewModel CustomerVM)
        {

            _customerViewModel = CustomerVM;
            CAddCustomer = new RelayCommand<StackPanel>((p) => { return true; }, (p) => { AddCustomer(p); });

        }
        public void AddCustomer(StackPanel p)
        {        
            if (Validator.IsValid(p))
            {

                KHACHHANG newCustomer = new KHACHHANG();
                newCustomer.MaKhachHang = CustomerId;
                newCustomer.DiaChi = CustomerAddress;
                newCustomer.HoTen = CustomerName;
                if (String.IsNullOrEmpty(CustomerEmail) == true)
                {
                    newCustomer.Email = "";
                }
                else
                {
                    newCustomer.Email = CustomerEmail;
                }
                newCustomer.DienThoai = CustomerPhoneNumber;
                newCustomer.TongNo = 0;
                if (IsExist(newCustomer) == false)
                {
                    DataProvider.Ins.DB.KHACHHANGs.Add(newCustomer);
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
                    RefreshAddCustomerForm();
                    _customerViewModel.MyMessageQueue.Enqueue("Thêm khách hàng thành công!");
                    BAOCAOCONGNO newrp = new BAOCAOCONGNO();
                    newrp.MaKhachHang = newCustomer.MaKhachHang;
                    newrp.Thang = DateTime.Now.Month;
                    newrp.Nam = DateTime.Now.Year;
                    newrp.TonDau = 0;
                    newrp.PhatSinh = 0;
                    newrp.TonDau = 0;
                    DataProvider.Ins.DB.BAOCAOCONGNOes.Add(newrp);
                    DataProvider.Ins.DB.SaveChanges();
                }
                else
                {
                    bool? dialogResult = new CustomMessageBox("Khách hàng đã tồn tại. \nBạn có muốn thêm khách hàng khác", MessageType.Info,"Thông Báo", MessageButtons.OkCancel).ShowDialog();
                    if (dialogResult == true)
                    {
                        AddCustomerWindow addCustomerWindow = new AddCustomerWindow(_customerViewModel);
                        addCustomerWindow.ShowDialog();
                    }
                }
            }
            else
            {
                _customerViewModel.MyMessageQueue.Enqueue("Lỗi. Thông tin khách hàng không hợp lệ");
            }
        }
        public void RefreshAddCustomerForm()
        {
            CustomerId = 0;
            CustomerName = "";
            CustomerAddress = "";
            CustomerEmail = "";
            CustomerPhoneNumber = "";
            CustomerDebt = 0;
        }
        public bool IsExist(KHACHHANG NewCustomer)
        {
            List<KHACHHANG> CustomerList = DataProvider.Ins.DB.KHACHHANGs.ToList();
            for(int i=0;i< CustomerList.Count;i++)
            {
                if (CustomerList[i].MaKhachHang == NewCustomer.MaKhachHang)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

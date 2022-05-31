using BookstoreManager.Models;
using BookstoreManager.Models.Db;
using BookstoreManager.Resources.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookstoreManager.ViewModels.ReceiptsViewModels
{
    public class AddCustomerModel : BaseViewModel
    {
        private string _customerName;
        private string _customerAddress;
        private long _customerId;
        private string _customerEmail;
        private string _customerPhoneNumber;
        private decimal _customerDebt;
        private ReceiptsViewModel _receiptViewModel;

        public string CustomerName { get { return _customerName; } set { _customerName = value; OnPropertyChanged(nameof(CustomerName)); } }
        public string CustomerAddress { get { return _customerAddress; } set { _customerAddress = value; OnPropertyChanged(nameof(CustomerAddress)); } }
        public long CustomerId { get { return _customerId; } set { _customerId = value; OnPropertyChanged(nameof(CustomerId)); } }
        public string CustomerEmail { get { return _customerEmail; } set { _customerEmail = value; OnPropertyChanged(nameof(CustomerEmail)); } }
        public decimal CustomerDebt { get { return _customerDebt; } set { _customerDebt = value; OnPropertyChanged(nameof(CustomerDebt)); } }
        public string CustomerPhoneNumber { get { return _customerPhoneNumber; } set { _customerPhoneNumber = value; OnPropertyChanged(nameof(CustomerPhoneNumber)); } }

        public ICommand CAddCustomer { get; set; }


        public AddCustomerModel(ReceiptsViewModel CustomerVM)
        {

            _receiptViewModel = CustomerVM;
            CAddCustomer = new RelayCommand<StackPanel>((p) => { return true; }, (p) => { AddCustomer(p); });

        }
        public void AddCustomer(StackPanel p)
        {
            if (Validator.IsValid(p))
            {

                KHACHHANG newCustomer = new KHACHHANG();
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
                newCustomer.TongNo = CustomerDebt;
                if (IsExist(newCustomer) == false)
                {
                    DataProvider.Ins.DB.KHACHHANGs.Add(newCustomer);
                    try
                    {
                        DataProvider.Ins.DB.SaveChanges();
                    }
                    catch
                    {
                        _receiptViewModel.MyMessageQueue.Enqueue("Lỗi. Thông tin khách hàng không hợp lệ");
                        return;
                    }
                    _receiptViewModel.MyMessageQueue.Enqueue("Thêm khách hàng thành công!");
                    _receiptViewModel.CustomerName = CustomerName;
                    _receiptViewModel.CustomerPhoneNumber = CustomerPhoneNumber;
                    RefreshAddCustomerForm();
                }
                else
                {
                    _receiptViewModel.MyMessageQueue.Enqueue("Lỗi!. Khách hàng đã tồn tại");

                }
            }
            else
            {
                _receiptViewModel.MyMessageQueue.Enqueue("Lỗi. Thông tin khách hàng không hợp lệ");
            }
        }
        public void RefreshAddCustomerForm()
        {
            CustomerName = "";
            CustomerAddress = "";
            CustomerEmail = "";
            CustomerPhoneNumber = "";
            CustomerDebt = 0;
        }
        public bool IsExist(KHACHHANG NewCustomer)
        {
            List<KHACHHANG> CustomerList = DataProvider.Ins.DB.KHACHHANGs.ToList();
            for (int i = 0; i < CustomerList.Count; i++)
            {
                if (NewCustomer.MaKhachHang == CustomerId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

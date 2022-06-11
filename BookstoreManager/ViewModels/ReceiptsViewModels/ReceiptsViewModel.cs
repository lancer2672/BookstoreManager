using BookstoreManager.Models;
using BookstoreManager.Models.Db;
using BookstoreManager.Resources;
using BookstoreManager.Resources.Utils;
using BookstoreManager.ViewModels.Customers;
using BookstoreManager.Views.Receipts;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookstoreManager.ViewModels.ReceiptsViewModels
{
    public class ReceiptsViewModel : BaseViewModel
    {
        private string _customerName;
        private long _customerID;
        private string _customerPhoneNumber;
        private string _customerEmail;
        private string _customerAddress;
        private decimal _customerPaid;
        //private decimal _customerChange;
        //private decimal _receivedMoney;
        private DateTime _date;
        private DateTime _selectedDate;
        private string _searchKey;
        private ObservableCollection<ViewReceipt> _listReceipt;

        public string SearchKey { get { return _searchKey; } set { _searchKey = value; OnPropertyChanged(nameof(SearchKey)); } }
        public long CustomerID { get => _customerID; set { _customerID = value; OnPropertyChanged(nameof(CustomerID)); } }
        public string CustomerName { get => _customerName; set { _customerName = value; OnPropertyChanged(nameof(CustomerName)); } }
        public string CustomerAddress { get => _customerAddress; set { _customerAddress = value; OnPropertyChanged(nameof(CustomerAddress)); } }
        public string CustomerEmail { get => _customerEmail; set { _customerEmail = value; OnPropertyChanged(nameof(CustomerEmail)); } }
        public string CustomerPhoneNumber { get => _customerPhoneNumber; set { _customerPhoneNumber = value; OnPropertyChanged(nameof(CustomerPhoneNumber)); } }
        public DateTime Date { get => _date; set { _date = value; OnPropertyChanged(nameof(Date)); } }
        public DateTime SelectedDate { get => _selectedDate; set { _selectedDate = value; OnPropertyChanged(nameof(SelectedDate)); LoadDataListView(); } }
        public decimal CustomerPaid { get => _customerPaid; set { _customerPaid = value; OnPropertyChanged(nameof(CustomerPaid)); } }
        //public decimal ReceivedMoney { get => _receivedMoney; set { _receivedMoney = value; OnPropertyChanged(nameof(ReceivedMoney)); } }
        //public decimal CustomerChange { get => _customerChange; set { _customerChange = value; OnPropertyChanged(nameof(CustomerChange)); } }
        public ObservableCollection<ViewReceipt> ListReceipt { get => _listReceipt; set { _listReceipt = value; OnPropertyChanged(nameof(ListReceipt)); } }
        public SnackbarMessageQueue MyMessageQueue { get => myMessageQueue; set { myMessageQueue = value; OnPropertyChanged(nameof(MyMessageQueue)); } }
        private SnackbarMessageQueue myMessageQueue;
        public ICommand SaveReceipts { get; set; }
        public ICommand CRefreshData { get; set; }
        public ICommand CRefreshForm { get; set; }
        public ICommand CSearch { get; set; }
        public ICommand CSearchInfo { get; set; }
        public ICommand COpenAddCustomer { get; set; }
        public ReceiptsViewModel()
        {
            MyMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(2000));
            MyMessageQueue.DiscardDuplicates = true;

            Date = DateTime.Now;
            SelectedDate = DateTime.Now;
            ListReceipt = new ObservableCollection<ViewReceipt>();

            CRefreshData = new RelayCommand<object>((p) => { return true; }, (p) => { RefreshData(); });
            CRefreshForm = new RelayCommand<object>((p) => { return true; }, (p) => { RefreshForm(); });
            CSearch = new RelayCommand<object>((p) => { return true; }, (p) => { Search(); });
            SaveReceipts = new RelayCommand<StackPanel>((p) => { return true; }, (p) => { CreateReceipt(p) ; });
            COpenAddCustomer = new RelayCommand<object>((p) => { return true; }, (p) => { OpenAddCustomerWindow(); });
            CSearchInfo = new RelayCommand<object>((p) => { return true; }, (p) => { SearchInfoCustomer(); });


            LoadDataListView();

        }
        public void OpenAddCustomerWindow()
        {
            AddNewCustomer adWindow = new AddNewCustomer(this);
            adWindow.ShowDialog();
        }
        public void SearchInfoCustomer()
        {
            if(CheckExistById(CustomerID) == true)
            {
                KHACHHANG customer  = DataProvider.Ins.DB.KHACHHANGs.Where(t => t.MaKhachHang.Equals(CustomerID)).FirstOrDefault();
                CustomerName = customer.HoTen;
                CustomerPhoneNumber = customer.DienThoai;
                CustomerAddress = customer.DiaChi;
                CustomerEmail = customer.Email;

            }
        }
        public void CreateReceipt(StackPanel p)
        {
            if (Validator.IsValid(p))
            {

                if(CheckExistById(CustomerID) == false)
                {
                    MyMessageQueue.Enqueue("Lỗi. Khách hàng không tồn tại");
                    RefreshForm();
                    return;
                }
                PHIEUTHU newReceipt = new PHIEUTHU();
                newReceipt.NgayLap = DateTime.Now;
                newReceipt.MaKhachHang = CustomerID;
                newReceipt.SoTienThu = CustomerPaid;
                KHACHHANG customer = DataProvider.Ins.DB.KHACHHANGs.Where(t => t.MaKhachHang == CustomerID).FirstOrDefault();
                if ((customer.TongNo - CustomerPaid) >= 0)
                {

                    customer.TongNo -= CustomerPaid;
                    DataProvider.Ins.DB.PHIEUTHUs.Add(newReceipt);
                    try
                    {
                        DataProvider.Ins.DB.SaveChanges();
                    }
                    catch
                    {
                        MyMessageQueue.Enqueue("Lỗi!. Tạo phiếu không thành công");
                        return;
                    }
                    MyMessageQueue.Enqueue("Tạo phiếu thành công");

                    LoadDataListView();
                    RefreshForm();
                }
                else
                {
                    string customerDebt = MoneyConverter.convertMoney(customer.TongNo.ToString());
                    MyMessageQueue.Enqueue("Lỗi!. Số tiền thu phải bé hơn hoặc bằng số tiền nợ" + "< hoặc = " + customerDebt);

                }
            }
            else
            {
               MyMessageQueue.Enqueue("Lỗi. Thông tin không hợp lệ");
            }
        }
        public bool CheckExist(KHACHHANG customer)
        {
            List<KHACHHANG> listCustomer = DataProvider.Ins.DB.KHACHHANGs.ToList();
            KHACHHANG check = DataProvider.Ins.DB.KHACHHANGs.Where(t => t.MaKhachHang == CustomerID).FirstOrDefault();
            if (check != null)
            {
                return true;
            }
            return false;
        }
        public bool CheckExistById(long Id)
        {
            KHACHHANG check = DataProvider.Ins.DB.KHACHHANGs.Where(t => t.MaKhachHang == Id).FirstOrDefault();
            if (check == null)
            {
                return false;
            }
            return true;
        }
        public void Search()
        {
            if (SearchKey != "" && SearchKey != null)
            {
                List<KHACHHANG> listKH = DataProvider.Ins.DB.KHACHHANGs.Where(t => t.HoTen.ToLower().Contains(SearchKey.ToLower())).ToList();
                List<PHIEUTHU> listReceipt = new List<PHIEUTHU>();
                foreach (KHACHHANG item in listKH)
                {
                    List<PHIEUTHU> list = DataProvider.Ins.DB.PHIEUTHUs.
                        Where(t => t.NgayLap.Value.Month == SelectedDate.Month 
                                     && t.NgayLap.Value.Year == SelectedDate.Year
                                     &&t.NgayLap.Value.Day == SelectedDate.Day
                                    && t.MaKhachHang == item.MaKhachHang).ToList();
                    listReceipt.AddRange(list);

                }
                ListReceipt = GetDataFromDb(listReceipt);
            }
            else
            {
                LoadDataListView();
            }
        }
        public void RefreshData()
        {
            SearchKey = "";
            LoadDataListView();
        }
        public void RefreshForm()
        {
            CustomerID = 0;
            CustomerName = "";
            CustomerPhoneNumber = "";
            CustomerAddress = "";
            CustomerEmail = "";
            CustomerPaid = 0;
        }
        public void LoadDataListView()
        {
            List<PHIEUTHU> list = DataProvider.Ins.DB.PHIEUTHUs.Where(t => t.NgayLap.Value.Month == SelectedDate.Month
                                     && t.NgayLap.Value.Year == SelectedDate.Year
                                     && t.NgayLap.Value.Day == SelectedDate.Day).ToList();
            ListReceipt = GetDataFromDb(list);
            if (ListReceipt.Count == 0)
            {
                MyMessageQueue.Enqueue("Không có dữ liệu @@");
            }
        }
        public ObservableCollection<ViewReceipt> GetDataFromDb(List<PHIEUTHU> list)
        {
            ObservableCollection<ViewReceipt> receipts = new ObservableCollection<ViewReceipt>();
            foreach (PHIEUTHU item in list)
            {
                ViewReceipt viewReceipt = new ViewReceipt();
                KHACHHANG customer = DataProvider.Ins.DB.KHACHHANGs.Where(t => t.MaKhachHang == item.MaKhachHang).FirstOrDefault();

                viewReceipt.ReceiptId = item.MaPhieuThu;
                viewReceipt.CustomerName = customer.HoTen;
                viewReceipt.CustomerPhoneNumber = customer.DienThoai;
                viewReceipt.Date = (DateTime)item.NgayLap;
                viewReceipt.CustomerPaid = (decimal)item.SoTienThu;
                viewReceipt.CustomerID = customer.MaKhachHang;
                receipts.Add(viewReceipt);
            }
            return receipts;
        }

    }
}

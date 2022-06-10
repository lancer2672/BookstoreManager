using BookstoreManager.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BookstoreManager.Models;
using BookstoreManager.Models.Db;
using BookstoreManager.Resources.Utils;
using MaterialDesignThemes.Wpf;
using System.Windows.Controls;

namespace BookstoreManager.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public bool IsLoaded = false;
        private int _idcustomer;
        private string _date;
        private decimal _totalmoney;
        private decimal _moneyreceived;
        private decimal _moneyremained;
        private string _namecustomer;
        private string _phonenumber;
        private string _email;
        private string _address;
        private decimal _totaldebt;
        private int _numberbook;
        private ObservableCollection<ViewSellBook> _listBook;
        private ViewBook _selectedlistbook;
        private SACH _selectedbook;
        private THELOAI _selectedcategory;
        private List<SACH> _listSACH;
        private List<THELOAI> _listTHELOAI;
        private List<TACGIA> _listTACGIA;
        private List<CHITIETTACGIA> _listCT_TACGIA;
        private List<KHACHHANG> _listKHACHHANG;
        private SnackbarMessageQueue _myMessageQueue;

        public int IdCustomer { get { return _idcustomer; } set { _idcustomer = value; OnPropertyChanged(nameof(IdCustomer)); LoadInfor(); } }
        public string Date { get { return _date; } set { _date = value; OnPropertyChanged(nameof(Date)); } }
        public decimal TotalMoney { get { return _totalmoney; } set { _totalmoney = value; OnPropertyChanged(nameof(TotalMoney)); } }
        public decimal MoneyReceived { get { return _moneyreceived; } set { _moneyreceived = value; OnPropertyChanged(nameof(MoneyReceived)); LoadMoneyRemained(); } }
        public decimal MoneyRemained { get { return _moneyremained; } set { _moneyremained = value; OnPropertyChanged(nameof(MoneyRemained)); } }
        public string NameCustomer { get { return _namecustomer; } set { _namecustomer = value; OnPropertyChanged(nameof(NameCustomer)); } }
        public string PhoneNumber { get { return _phonenumber; } set { _phonenumber = value; OnPropertyChanged(nameof(PhoneNumber)); } }
        public string Email { get { return _email; } set { _email = value; OnPropertyChanged(nameof(Email)); } }
        public string Address { get { return _address; } set { _address = value; OnPropertyChanged(nameof(Address)); } }
        public decimal TotalDebt { get { return _totaldebt; } set { _totaldebt = value; OnPropertyChanged(nameof(TotalDebt)); } }
        public int NumberBook { get { return _numberbook; } set { _numberbook = value; OnPropertyChanged(nameof(NumberBook)); } }
        public ObservableCollection<ViewSellBook> ListBook { get => _listBook; set { _listBook = value; OnPropertyChanged(nameof(ListBook)); } }
        public ViewBook SelectedListBook { get { return _selectedlistbook; } set { _selectedlistbook = value; OnPropertyChanged(nameof(SelectedListBook)); } }
        public SACH SelectedBook { get { return _selectedbook; } set { _selectedbook = value; OnPropertyChanged(nameof(SelectedBook)); } }
        public THELOAI SelectedCategory { get { return _selectedcategory; } set { _selectedcategory = value; OnPropertyChanged(nameof(SelectedCategory)); LoadBookName(); } }
        public List<SACH> ListSACH { get { return _listSACH; } set { _listSACH = value; OnPropertyChanged(nameof(ListSACH)); } }
        public List<THELOAI> ListTHELOAI { get { return _listTHELOAI; } set { _listTHELOAI = value; OnPropertyChanged(nameof(ListTHELOAI)); } }
        public List<TACGIA> ListTACGIA { get => _listTACGIA; set { _listTACGIA = value; OnPropertyChanged(nameof(ListTACGIA)); } }
        public List<CHITIETTACGIA> ListCT_TACGIA { get => _listCT_TACGIA; set { _listCT_TACGIA = value; OnPropertyChanged(nameof(ListCT_TACGIA)); } }
        public List<KHACHHANG> ListKHACHHANG { get { return _listKHACHHANG; } set { _listKHACHHANG = value; OnPropertyChanged(nameof(ListKHACHHANG)); } }
        public SnackbarMessageQueue MyMessageQueue { get { return _myMessageQueue; } set { _myMessageQueue = value; OnPropertyChanged(nameof(MyMessageQueue)); } }

        public ICommand LoadedWidnowCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand CreateBillCommand { get; set; }
        public MainViewModel()
        {
            ListBook = new ObservableCollection<ViewSellBook>();
            ListSACH = DataProvider.Ins.DB.SACHes.ToList();
            ListTHELOAI = DataProvider.Ins.DB.THELOAIs.ToList();
            ListTACGIA = DataProvider.Ins.DB.TACGIAs.ToList();
            ListCT_TACGIA = DataProvider.Ins.DB.CHITIETTACGIAs.ToList();
            ListKHACHHANG = DataProvider.Ins.DB.KHACHHANGs.ToList();
            DateTime today = DateTime.Now;
            Date = today.ToString("dd/MM/yyyy");

            MyMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(2000))
            {
                DiscardDuplicates = true
            };
            MyMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(2000));
            MyMessageQueue.DiscardDuplicates = true;

            LoadedWidnowCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { LoadWindow(p); });
            AddCommand = new RelayCommand<object>((p) => { return true; }, (p) => { AddBook(); });
            DeleteCommand = new RelayCommand<object>((p) => { return true; }, (p) => { DeleteBook(); });
            CreateBillCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CreateBillBook(); });

            CheckReportDay();
        }
        void CheckReportDay()
        {
            DateTime now = DateTime.Now;
            int daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
            if (DateTime.Now.Day == 10)
            {

                if (IsCreatedReport(now.AddDays(-1)) == false)
                {
                    CreateInvReport(now.AddDays(-1));
                    CreateDebtReport(now.AddDays(-1));
                    MyMessageQueue.Enqueue("Đã tạo báo cáo");
                }
                    
            }
            else
            {
                int day = daysInMonth - DateTime.Now.Day + 1;
                MyMessageQueue.Enqueue("Còn " + day.ToString() + " ngày nữa sẽ tới ngày tạo báo cáo");
            }
        }
        bool IsCreatedReport(DateTime time)
        {
            //  
            List<BAOCAOCONGNO> list = DataProvider.Ins.DB.BAOCAOCONGNOes.Where(t => t.Thang == time.Month && t.Nam == time.Year).ToList();
            return list.Count == 0 ? false : true;
        }

        void CreateDebtReport(DateTime now)
        {
            List<KHACHHANG> customerList = DataProvider.Ins.DB.KHACHHANGs.ToList();
            for (int i = 0; i < customerList.Count; i++)
            {
                long customerId = customerList[i].MaKhachHang;
                BAOCAOCONGNO rp = DataProvider.Ins.DB.BAOCAOCONGNOes.Where(t => t.MaKhachHang == customerId && t.Thang == now.Month && t.Nam == now.Year).FirstOrDefault();
                if (rp != null)
                {
                    rp.PhatSinh = customerList[i].TongNo - rp.TonDau;
                    rp.TonCuoi = customerList[i].TongNo;
                }
                else
                {
                    rp = new BAOCAOCONGNO();
                    rp.Thang = DateTime.Now.Month;
                    rp.Nam = DateTime.Now.Year;
                    rp.MaKhachHang = customerList[i].MaKhachHang;
                    rp.TonDau = GetFirstQuanityDebtRp(customerId);
                    rp.TonCuoi = customerList[i].TongNo;
                    rp.PhatSinh = customerList[i].TongNo - rp.TonDau;
                }
                DataProvider.Ins.DB.BAOCAOCONGNOes.Add(rp);
            }
            try
            {
                DataProvider.Ins.DB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        void CreateInvReport(DateTime now)
        {
           
            List<SACH> bookList = DataProvider.Ins.DB.SACHes.ToList();
            for (int i = 0; i < bookList.Count; i++)
            {
                int bookId = bookList[i].MaSach;
                BAOCAOTON rp = DataProvider.Ins.DB.BAOCAOTONs.Where(t => t.MaSach == bookId && t.Thang == now.Month && t.Nam == now.Year).FirstOrDefault();
                if (rp != null)
                {
                    rp.PhatSinh = bookList[i].SoLuongTon - rp.TonDau;
                    rp.TonCuoi = bookList[i].SoLuongTon;
                }
                else
                {
                    rp = new BAOCAOTON();
                    rp.Thang = DateTime.Now.Month;
                    rp.Nam = DateTime.Now.Year;
                    rp.MaSach = bookList[i].MaSach;
                    rp.TonDau = GetFirstQuanityInvRp(bookList[i].MaSach);
                    rp.TonCuoi = bookList[i].SoLuongTon;
                    rp.PhatSinh = bookList[i].SoLuongTon - rp.TonDau;
                }
                DataProvider.Ins.DB.BAOCAOTONs.Add(rp);
            }
            try
            {
                DataProvider.Ins.DB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        int GetFirstQuanityDebtRp(long customerId)
        {
            var today = DateTime.Today;
            var month = new DateTime(today.Year, today.Month, 1);
            var first = month.AddMonths(-1);
            //var last = month.AddDays(-1);
            BAOCAOTON report = DataProvider.Ins.DB.BAOCAOTONs.Where(t => t.MaSach == customerId && t.Thang == first.Month && t.Nam == first.Year).FirstOrDefault();
            if (report == null)
            {
                return 0;
            }
            return (int)report.TonCuoi;
        }
        int GetFirstQuanityInvRp(int bookId)
        {
            
            var today = DateTime.Today;
            var month = new DateTime(today.Year, today.Month, 1);
            var first = month.AddMonths(-1);
            //var last = month.AddDays(-1);
            BAOCAOTON report = DataProvider.Ins.DB.BAOCAOTONs.Where(t => t.MaSach == bookId && t.Thang == first.Month && t.Nam == first.Year).FirstOrDefault();
            if (report == null)
            {
                return 0;
            }
            return (int)report.TonCuoi;
        }
        public void LoadWindow(Window p)
        {
            //IsLoaded = true;
            //p.Hide();
            //AdminWindow admWindow = new AdminWindow();
            //admWindow.ShowDialog();
            IsLoaded = true;
            if (p == null)
                return;
            p.Hide();
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.ShowDialog();

            if (loginWindow.DataContext == null)
                return;
            var loginVM = loginWindow.DataContext as LoginViewModel;

            if (loginVM.IsLogin)
            {
                AdminWindow adminWindow = new AdminWindow();
                adminWindow.Show();
                MainWindow mainWindow = new MainWindow();
                mainWindow.Close();
                p.Close();
            }
            else
            {
                p.Show();
                loginWindow.Close();
            }
        }
        public int CheckCustomer()
        {
            int i;
            for (i = 0; i < ListKHACHHANG.Count; i++)
            {
                if (IdCustomer == ListKHACHHANG[i].MaKhachHang)
                {
                    return i;
                }
            }
            if (i == ListKHACHHANG.Count)
                i = -1;
            return i;
        }
        public void LoadInfor()
        {
            int i = CheckCustomer();
            if (i == -1)
            {
                PhoneNumber = NameCustomer = "";
                TotalMoney = MoneyReceived = MoneyRemained = TotalDebt = 0;
            }
            else
            {
                if (ListKHACHHANG[i].TongNo == 0)
                    TotalDebt = 0;
                NameCustomer = ListKHACHHANG[i].HoTen;
                Email = ListKHACHHANG[i].Email;
                PhoneNumber = ListKHACHHANG[i].DienThoai;
                Address = ListKHACHHANG[i].DiaChi;
                TotalDebt = (decimal)ListKHACHHANG[i].TongNo;
            }
        }
        public void LoadBookName()
        {
            if (SelectedCategory != null)
            {
                ListSACH.Clear();
                ListSACH = DataProvider.Ins.DB.SACHes.Where(t => t.MaTheLoai == SelectedCategory.MaTheLoai).ToList();
            }
        }
        public string FindCategory(int matheloai, List<THELOAI> listTHELOAI)
        {
            string category = "";
            foreach (var item in listTHELOAI)
            {
                if (matheloai == item.MaTheLoai)
                {
                    category = item.TenTheLoai;
                    break;
                }
            }
            return category;
        }
        public string FindAuthor(int masach, List<TACGIA> listTACGIA, List<CHITIETTACGIA> listCT_TACGIA)
        {
            string author = "";
            int matacgia = 0;
            foreach (var item in listCT_TACGIA)
            {
                if (masach == item.MaSach)
                {
                    matacgia = (int)item.MaTacGia;
                    break;
                }
            }
            foreach (var item in listTACGIA)
            {
                if (matacgia == item.MaTacGia)
                {
                    author = item.HoTen;
                    break;
                }
            }
            return author;
        }
        public int CheckExist(ObservableCollection<ViewSellBook> listbook, int masach)
        {
            int check = -1;
            for (int i = 0; i < listbook.Count; i++)
            {
                if (masach == listbook[i].Id)
                {
                    check = i;
                    break;
                }
            }
            return check;
        }
        public void LoadListBook()
        {
            ObservableCollection<ViewSellBook> NewList = new ObservableCollection<ViewSellBook>();
            foreach (ViewSellBook item in ListBook)
                NewList.Add(item);
            ListBook.Clear();
            foreach (ViewSellBook item in NewList)
                ListBook.Add(item);
        }
        public void LoadMoneyRemained()
        {
            if (TotalMoney != 0)
                MoneyRemained = TotalMoney - MoneyReceived;
        }
        public int FindThamSo(string tenthamso)
        {
            List<THAMSO> ListTHAMSO = DataProvider.Ins.DB.THAMSOes.ToList();
            int i;
            for (i = 0; i < ListTHAMSO.Count; i++)
            {
                if (ListTHAMSO[i].TenThamSo == tenthamso)
                {
                    break;
                }
            }
            return i;
        }
        public void AddBook()
        {
            if(SelectedBook == null)
            {
                MyMessageQueue.Clear();
                MyMessageQueue.Enqueue("Vui lòng chọn sách để thêm!");
                return;
            }
            if (NumberBook == 0)
            {
                MyMessageQueue.Clear();
                MyMessageQueue.Enqueue("Vui lòng chọn số lượng sách!");
            }
            else
            {
                List<THAMSO> ListTHAMSO = DataProvider.Ins.DB.THAMSOes.ToList();
                if ((SelectedBook.SoLuongTon - NumberBook) < ListTHAMSO[FindThamSo("LuongTonToiThieuSauKhiBanSach")].GiaTri)
                {
                    string message = "Không thể thêm vì không đủ lượng tồn tối thiểu (>" + Convert.ToString(ListTHAMSO[FindThamSo("LuongTonToiThieuSauKhiBanSach")].GiaTri) + ") !";
                    MyMessageQueue.Clear();
                    MyMessageQueue.Enqueue(message);
                }
                else
                {
                    SACH newsach = DataProvider.Ins.DB.SACHes.Where(t => t.MaSach == SelectedBook.MaSach).FirstOrDefault();
                    ViewSellBook newBook = new ViewSellBook();
                    newBook.Id = newsach.MaSach;
                    newBook.TitleBook = newsach.TenSach;
                    newBook.Category = FindCategory((int)newsach.MaTheLoai, ListTHELOAI);
                    newBook.NameAuthor = FindAuthor(newsach.MaSach, ListTACGIA, ListCT_TACGIA);
                    newBook.PublishCompany = newsach.NhaXuatBan;
                    newBook.PublishYear = (int)newsach.NamXuatBan;
                    newBook.Number = NumberBook;
                    int tileban = (int)ListTHAMSO[FindThamSo("TiLeGiaBan")].GiaTri;
                    newBook.SellPrice = ((decimal)newsach.GiaNhap * tileban) / 100;

                    if (ListBook.Count == 0)
                    {
                        ListBook.Add(newBook);
                        TotalMoney = newBook.SellPrice * NumberBook;
                        MyMessageQueue.Clear();
                        MyMessageQueue.Enqueue("Thêm sách thành công!");
                    }
                    else
                    {
                        if (CheckExist(ListBook, SelectedBook.MaSach) == -1)
                        {
                            ListBook.Add(newBook);
                            TotalMoney += newBook.SellPrice * NumberBook;
                            MyMessageQueue.Clear();
                            MyMessageQueue.Enqueue("Thêm sách thành công!");
                        }
                        else
                        {
                            ListBook[CheckExist(ListBook, SelectedBook.MaSach)].Number += NumberBook;
                            TotalMoney += newBook.SellPrice * NumberBook;
                            LoadListBook();
                            MyMessageQueue.Clear();
                            MyMessageQueue.Enqueue("Thêm sách thành công!");
                        }
                    }
                    SelectedBook = null;
                    SelectedCategory = null;
                    NumberBook = 0;
                }
            }
        }
        public void DeleteBook()
        {
            if (SelectedListBook != null)
            {
                int location = CheckExist(ListBook, SelectedListBook.Id);
                ListBook.Remove(ListBook[location]);
                LoadListBook();
                MyMessageQueue.Enqueue("Xóa sách thành công!");
                SelectedBook = null;
                SelectedCategory = null;
                NumberBook = 0;
                TotalMoney = 0;
            }
            else
            {
                MyMessageQueue.Clear();
                MyMessageQueue.Enqueue("Vui lòng chọn sách để xóa!");
            }
        }
        public void Clear()
        {
            ListBook.Clear();
            IdCustomer = NumberBook = 0;
            PhoneNumber = NameCustomer = "";
            TotalMoney = MoneyReceived = MoneyRemained = TotalDebt = 0;
            SelectedBook = null;
            SelectedCategory = null;
        }
        public void CreateBillBook()
        {
            List<THAMSO> ListTHAMSO = DataProvider.Ins.DB.THAMSOes.ToList();
            if (MoneyReceived == 0)
            {
                MyMessageQueue.Clear();
                MyMessageQueue.Enqueue("Vui lòng nhập số tiền trả!");
                return;
            }
            if (IdCustomer == 0 || NameCustomer == null || PhoneNumber == null )
            {
                MyMessageQueue.Clear();
                MyMessageQueue.Enqueue("Vui lòng điền đủ thông tin khách hàng!");
                return;
            }
            if((TotalDebt + MoneyRemained) >= ListTHAMSO[FindThamSo("TongNoToiDa")].GiaTri)
            {
                string message = "Không thể lập hóa đơn vì tổng nợ vượt quá tổng nợ tối đa (" + Convert.ToString(ListTHAMSO[FindThamSo("TongNoToiDa")].GiaTri) + ") !";
                MyMessageQueue.Clear();
                MyMessageQueue.Enqueue(message);
                return;
            }
            if (ListBook.Count == 0)
            {
                MyMessageQueue.Clear();
                MyMessageQueue.Enqueue("Vui lòng chọn sách để lập hóa đơn!");
            }
            else
            {
                if (CheckCustomer() == -1)
                {
                    KHACHHANG newcustomer = new KHACHHANG()
                    {
                        MaKhachHang = IdCustomer,
                        HoTen = NameCustomer,
                        DienThoai = PhoneNumber,
                        DiaChi = Address,
                        Email = Email,
                        TongNo = TotalDebt + MoneyRemained
                    };
                    DataProvider.Ins.DB.KHACHHANGs.Add(newcustomer);
                    BAOCAOCONGNO newrp = new BAOCAOCONGNO();
                    newrp.MaKhachHang = IdCustomer;
                    newrp.Thang = DateTime.Now.Month;
                    newrp.Nam = DateTime.Now.Year;
                    newrp.TonDau = newcustomer.TongNo;
                    DataProvider.Ins.DB.BAOCAOCONGNOes.Add(newrp);
                    DataProvider.Ins.DB.SaveChanges();
                    HOADON newbill = new HOADON()
                    {
                        MaKhachHang = IdCustomer,
                        NgayLap = DateTime.Today,
                        TongTien = TotalMoney,
                        SoTienTra = MoneyReceived,
                        SoTienConLai = MoneyRemained
                    };
                    DataProvider.Ins.DB.HOADONs.Add(newbill);

                    foreach (var item in ListBook)
                    {
                        SACH book = DataProvider.Ins.DB.SACHes.Where(t => t.MaSach == item.Id).FirstOrDefault();
                        book.SoLuongTon = book.SoLuongTon - item.Number;
                        DataProvider.Ins.DB.SaveChanges();
                        CHITIETHOADON detailbill = new CHITIETHOADON()
                        {
                            MaHoaDon = newbill.MaHoaDon,
                            MaSach = item.Id,
                            SoLuong = item.Number,
                            DonGia = item.SellPrice,
                            ThanhTien = item.SellPrice * item.Number
                        };
                        DataProvider.Ins.DB.CHITIETHOADONs.Add(detailbill);
                    }
                    DataProvider.Ins.DB.SaveChanges();
                    MyMessageQueue.Clear();
                    MyMessageQueue.Enqueue("Tạo hóa đơn thành công!");
                }
                else
                {
                    KHACHHANG oldcustomer = DataProvider.Ins.DB.KHACHHANGs.Where(t => t.MaKhachHang == IdCustomer).FirstOrDefault();
                    oldcustomer.TongNo += MoneyRemained;
                    HOADON newbill = new HOADON()
                    {
                        MaKhachHang = IdCustomer,
                        NgayLap = DateTime.Today,
                        TongTien = TotalMoney,
                        SoTienTra = MoneyReceived,
                        SoTienConLai = MoneyRemained
                    };
                    DataProvider.Ins.DB.HOADONs.Add(newbill);

                    foreach (var item in ListBook)
                    {
                        SACH book = DataProvider.Ins.DB.SACHes.Where(t => t.MaSach == item.Id).FirstOrDefault();
                        book.SoLuongTon = book.SoLuongTon - item.Number;
                        DataProvider.Ins.DB.SaveChanges();
                        CHITIETHOADON detailbill = new CHITIETHOADON()
                        {
                            MaHoaDon = newbill.MaHoaDon,
                            MaSach = item.Id,
                            SoLuong = item.Number,
                            DonGia = item.SellPrice,
                            ThanhTien = item.SellPrice * item.Number
                        };
                        DataProvider.Ins.DB.CHITIETHOADONs.Add(detailbill);
                    }
                    DataProvider.Ins.DB.SaveChanges();
                    MyMessageQueue.Clear();
                    MyMessageQueue.Enqueue("Tạo hóa đơn thành công!");
                }
                Clear();
            }

        }
    }
}

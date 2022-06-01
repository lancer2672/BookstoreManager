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
        private ObservableCollection<ViewBook> _listBook;
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
        public ObservableCollection<ViewBook> ListBook { get => _listBook; set { _listBook = value; OnPropertyChanged(nameof(ListBook)); } }
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
            ListBook = new ObservableCollection<ViewBook>();
            ListSACH = DataProvider.Ins.DB.SACHes.ToList();
            ListTHELOAI = DataProvider.Ins.DB.THELOAIs.ToList();
            ListTACGIA = DataProvider.Ins.DB.TACGIAs.ToList();
            ListCT_TACGIA = DataProvider.Ins.DB.CHITIETTACGIAs.ToList();
            ListKHACHHANG = DataProvider.Ins.DB.KHACHHANGs.ToList();
            DateTime today = DateTime.Now;
            Date = today.ToString("dd/MM/yyyy");

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
            if (DateTime.Now.Day == DateTime.Now.Day)
            {
                CreateInvReport(now);
                CreateDebtReport(now);
            }
            else
            {
                int day = daysInMonth - DateTime.Now.Day;
                MyMessageQueue.Enqueue("Còn " + day.ToString() + " ngày nữa sẽ tới ngày tạo báo cáo");
            }
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


            //for (i = 0; i < ListKHACHHANG.Count; i++)
            //{
            //    if (IdCustomer == ListKHACHHANG[i].MaKhachHang)
            //    {
            //        if (ListKHACHHANG[i].TongNo == 0)
            //            TotalDebt = 0;
            //        NameCustomer = ListKHACHHANG[i].HoTen;
            //        Email = ListKHACHHANG[i].Email;
            //        PhoneNumber = ListKHACHHANG[i].DienThoai;
            //        Address = ListKHACHHANG[i].DiaChi;
            //        break;
            //    }
            //}
            if (i == -1)
            {
                TotalDebt = 0;
            }
            else
            {
                if (ListKHACHHANG[i].TongNo == 0)
                    TotalDebt = 0;
                NameCustomer = ListKHACHHANG[i].HoTen;
                Email = ListKHACHHANG[i].Email;
                PhoneNumber = ListKHACHHANG[i].DienThoai;
                Address = ListKHACHHANG[i].DiaChi;
            }
        }
        public void LoadBookName()
        {
            ListSACH.Clear();
            ListSACH = DataProvider.Ins.DB.SACHes.Where(t => t.MaTheLoai == SelectedCategory.MaTheLoai).ToList();
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
        public int CheckExist(ObservableCollection<ViewBook> listbook, int masach)
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
            ObservableCollection<ViewBook> NewList = new ObservableCollection<ViewBook>();
            foreach (ViewBook item in ListBook)
                NewList.Add(item);
            ListBook.Clear();
            foreach (ViewBook item in NewList)
                ListBook.Add(item);
        }
        public void LoadMoneyRemained()
        {
            MoneyRemained = TotalMoney - MoneyReceived;
        }
        public void AddBook()
        {
            if (NumberBook == 0)
            {
                MyMessageQueue.Clear();
                MyMessageQueue.Enqueue("Vui lòng chọn số lượng sách!");
            }
            else
            {
                SACH newsach = DataProvider.Ins.DB.SACHes.Where(t => t.MaSach == SelectedBook.MaSach).FirstOrDefault();
                ViewBook newViewBook = new ViewBook();
                newViewBook.Id = newsach.MaSach;
                newViewBook.TitleBook = newsach.TenSach;
                newViewBook.Category = FindCategory((int)newsach.MaTheLoai, ListTHELOAI);
                newViewBook.NameAuthor = FindAuthor(newsach.MaSach, ListTACGIA, ListCT_TACGIA);
                newViewBook.PublishCompany = newsach.NhaXuatBan;
                newViewBook.PublishYear = (int)newsach.NamXuatBan;
                newViewBook.InventoryNumber = NumberBook;
                newViewBook.Price = (decimal)newsach.GiaNhap;
                if (ListBook.Count == 0)
                {
                    ListBook.Add(newViewBook);
                    TotalMoney = newViewBook.Price * NumberBook;
                    MyMessageQueue.Clear();
                    MyMessageQueue.Enqueue("Thêm sách thành công!");
                }
                else
                {
                    if (CheckExist(ListBook, SelectedBook.MaSach) == -1)
                    {
                        ListBook.Add(newViewBook);
                        TotalMoney += newViewBook.Price * NumberBook;
                        MyMessageQueue.Clear();
                        MyMessageQueue.Enqueue("Thêm sách thành công!");
                    }
                    else
                    {
                        ListBook[CheckExist(ListBook, SelectedBook.MaSach)].InventoryNumber += NumberBook;
                        TotalMoney += newViewBook.Price * NumberBook;
                        LoadListBook();
                        MyMessageQueue.Clear();
                        MyMessageQueue.Enqueue("Thêm sách thành công!");
                    }
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
            }
            else
            {
                MyMessageQueue.Clear();
                MyMessageQueue.Enqueue("Vui lòng chọn sách để xóa!");
            }
        }
        public void CreateBillBook()
        {
            if (IdCustomer == 0 || MoneyReceived == 0 || NameCustomer == null || PhoneNumber == null || Email == null || Address == null)
            {
                MyMessageQueue.Clear();
                MyMessageQueue.Enqueue("Vui lòng điền đủ thông tin khách hàng!");
            }
            else
            {
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
                            CHITIETHOADON detailbill = new CHITIETHOADON()
                            {
                                MaHoaDon = newbill.MaHoaDon,
                                MaSach = item.Id,
                                SoLuong = item.InventoryNumber,
                                DonGia = item.Price,
                                ThanhTien = item.Price * item.InventoryNumber
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
                            CHITIETHOADON detailbill = new CHITIETHOADON()
                            {
                                MaHoaDon = newbill.MaHoaDon,
                                MaSach = item.Id,
                                SoLuong = item.InventoryNumber,
                                DonGia = item.Price,
                                ThanhTien = item.Price * item.InventoryNumber
                            };
                            DataProvider.Ins.DB.CHITIETHOADONs.Add(detailbill);
                        }
                        DataProvider.Ins.DB.SaveChanges();
                        MyMessageQueue.Clear();
                        MyMessageQueue.Enqueue("Tạo hóa đơn thành công!");
                    }
                }
            }
        }
    }
}

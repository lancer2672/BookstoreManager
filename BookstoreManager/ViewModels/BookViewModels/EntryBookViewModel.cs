using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BookstoreManager.Models;
using BookstoreManager.Models.Db;
using BookstoreManager.ViewModels.BookViewModels;
using BookstoreManager.Views.BookViews;
using MaterialDesignThemes.Wpf;
using OfficeOpenXml;

namespace BookstoreManager.ViewModels.BookViewModels
{
    public class EntryBookViewModel : BaseViewModel
    {
        private List<SACH> _listSACH;
        private SACH _selectedBook;
        private ObservableCollection<ViewEntryBook> _listBook;
        private List<THELOAI> _listTHELOAI;
        private List<TACGIA> _listTACGIA;
        private List<CHITIETTACGIA> _listCT_TACGIA;
        private List<THAMSO> _listTHAMSO;
        private ViewEntryBook _selectedListBook;
        private string _category;
        private string _author;
        private int _inventorymoney;
        private int _entrynumber;
        private decimal _price;
        private decimal _totalmoneyFirst;
        private decimal _totalmoneySecond;
        private SnackbarMessageQueue _myMessageQueue;

        public List<SACH> ListSACH { get => _listSACH; set { _listSACH = value; OnPropertyChanged(nameof(ListSACH)); } }
        public SACH SelectedBook { get => _selectedBook; set { _selectedBook = value; OnPropertyChanged(nameof(SelectedBook)); LoadInfor(); } }
        public ObservableCollection<ViewEntryBook> ListBook { get => _listBook; set { _listBook = value; OnPropertyChanged(nameof(ListBook)); } }
        public List<THELOAI> ListTHELOAI { get => _listTHELOAI; set { _listTHELOAI = value; OnPropertyChanged(nameof(ListTHELOAI)); } }
        public List<TACGIA> ListTACGIA { get => _listTACGIA; set { _listTACGIA = value; OnPropertyChanged(nameof(ListTACGIA)); } }
        public List<CHITIETTACGIA> ListCT_TACGIA { get => _listCT_TACGIA; set { _listCT_TACGIA = value; OnPropertyChanged(nameof(ListCT_TACGIA)); } }
        public List<THAMSO> ListTHAMSO { get => _listTHAMSO; set { _listTHAMSO = value; OnPropertyChanged(nameof(ListTHAMSO)); } }
        public ViewEntryBook SelectedListBook { get => _selectedListBook; set { _selectedListBook = value; OnPropertyChanged(nameof(SelectedListBook)); } }
        public string Category { get => _category; set { _category = value; OnPropertyChanged(nameof(Category)); } }
        public string Author { get => _author; set { _author = value; OnPropertyChanged(nameof(Author)); } }
        public int InventoryNumber { get => _inventorymoney; set { _inventorymoney = value; OnPropertyChanged(nameof(InventoryNumber)); } }
        public int EntryNumber { get => _entrynumber; set { _entrynumber = value; OnPropertyChanged(nameof(EntryNumber)); } }
        public decimal Price { get => _price; set { _price = value; OnPropertyChanged(nameof(Price)); } }
        public decimal TotalMoneyFirst { get => _totalmoneyFirst; set { _totalmoneyFirst = value; OnPropertyChanged(nameof(TotalMoneyFirst)); } }
        public decimal TotalMoneySecond { get => _totalmoneySecond; set { _totalmoneySecond = value; OnPropertyChanged(nameof(TotalMoneySecond)); } }
        public SnackbarMessageQueue MyMessageQueue { get { return _myMessageQueue; } set { _myMessageQueue = value; OnPropertyChanged(nameof(MyMessageQueue)); } }

        public ICommand AddBookListCommand { get; set; }
        public ICommand DeleteBookListCommand { get; set; }
        public ICommand SaveBookListCommand { get; set; }
        public ICommand COpenAddBookEntryWindow { get; set; }


        public EntryBookViewModel()
        {
            ListSACH = DataProvider.Ins.DB.SACHes.ToList();
            ListBook = new ObservableCollection<ViewEntryBook>();
            ListTHELOAI = DataProvider.Ins.DB.THELOAIs.ToList();
            ListTACGIA = DataProvider.Ins.DB.TACGIAs.ToList();
            ListCT_TACGIA = DataProvider.Ins.DB.CHITIETTACGIAs.ToList();
            ListTHAMSO = DataProvider.Ins.DB.THAMSOes.ToList();

            MyMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(2000));
            MyMessageQueue.DiscardDuplicates = true;

            AddBookListCommand = new RelayCommand<object>((p) => { return true; }, (p) => { AddBookList(); });
            DeleteBookListCommand = new RelayCommand<object>((p) => { return true; }, (p) => { DeleteBookList(); });
            SaveBookListCommand = new RelayCommand<object>((p) => { return true; }, (p) => { SaveBookList(); });
            COpenAddBookEntryWindow = new RelayCommand<object>((p) => { return true; }, (p) => { OpenAddBookEntryWindow(); });
        }
        public string FindCategory(int matheloai)
        {
            List<THELOAI> listTHELOAI = DataProvider.Ins.DB.THELOAIs.ToList();
            string category = "";
            for (int i = 0; i < listTHELOAI.Count; i++)
            {
                if (matheloai == listTHELOAI[i].MaTheLoai)
                {
                    category = listTHELOAI[i].TenTheLoai;
                    break;
                }
            }
            return category;
        }
        public string FindAuthor(int masach)
        {
            List<TACGIA> listTACGIA = DataProvider.Ins.DB.TACGIAs.ToList();
            List<CHITIETTACGIA> listCT_TACGIA = DataProvider.Ins.DB.CHITIETTACGIAs.ToList();
            string author = "";
            int matacgia = 0;
            for (int i = 0; i < listCT_TACGIA.Count; i++)
            {
                if (masach == listCT_TACGIA[i].MaSach)
                {
                    matacgia = (int)listCT_TACGIA[i].MaTacGia;
                    break;
                }
            }
            for (int i = 0; i < listTACGIA.Count; i++)
            {
                if (matacgia == listTACGIA[i].MaTacGia)
                {
                    author = listTACGIA[i].HoTen;
                    break;
                }
            }
            return author;
        }
        public void LoadInfor()
        {
            if (SelectedBook != null)
            {
                Category = FindCategory((int)SelectedBook.MaTheLoai);
                Author = FindAuthor(SelectedBook.MaSach);
                InventoryNumber = (int)SelectedBook.SoLuongTon;
                Price = (decimal)SelectedBook.GiaNhap;
                EntryNumber = 0;
                TotalMoneyFirst = 0;
            }
            string tenthamso = "SoLuongTonToiDaChoPhepNhapSach";
            if (InventoryNumber > ListTHAMSO[FindThamSo(tenthamso)].GiaTri)
            {
                string message = "Số lượng tồn lớn hơn " + Convert.ToString(ListTHAMSO[FindThamSo(tenthamso)].GiaTri) + " nên không thể nhập thêm";
                MyMessageQueue.Clear();
                MyMessageQueue.Enqueue(message);

            }
        }
        public int FindThamSo(string tenthamso)
        {
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
        public int CheckExist()
        {
            int check = -1;
            for (int i = 0; i < ListBook.Count; i++)
            {
                if (SelectedBook.MaSach == ListBook[i].Id)
                {
                    check = i;
                    break;
                }
            }
            return check;
        }
        public void LoadListBook()
        {
            ObservableCollection<ViewEntryBook> NewList = new ObservableCollection<ViewEntryBook>();
            foreach (var item in ListBook)
                NewList.Add(item);
            ListBook.Clear();
            foreach (var item in NewList)
                ListBook.Add(item);
        }
        public void LoadTotalMoneySecond()
        {
            TotalMoneySecond = 0;
            foreach (var item in ListBook)
                TotalMoneySecond += item.TotalPrice;
        }
        public void AddBookList()
        {
            if(SelectedBook==null)
            {
                MyMessageQueue.Clear();
                MyMessageQueue.Enqueue("Vui lòng sách để nhập!");
                return;
            }
            string tenthamso = "SoLuongTonToiDaChoPhepNhapSach";
            if (InventoryNumber > ListTHAMSO[FindThamSo(tenthamso)].GiaTri)
            {
                string message = "Số lượng tồn lớn hơn " + Convert.ToString(ListTHAMSO[FindThamSo(tenthamso)].GiaTri) + " nên không thể nhập thêm";
                MyMessageQueue.Clear();
                MyMessageQueue.Enqueue(message);
                return;
            }
            if (EntryNumber < ListTHAMSO[FindThamSo("SoLuongNhapToiThieu")].GiaTri)
            {
                string message = "Số lượng nhập phải lớn hơn " + Convert.ToString(ListTHAMSO[FindThamSo("SoLuongNhapToiThieu")].GiaTri);
                MyMessageQueue.Clear();
                MyMessageQueue.Enqueue(message);
            }
            else
            {
                if (ListBook.Count == 0)
                {
                    ViewEntryBook newbook = new ViewEntryBook()
                    {
                        Id = SelectedBook.MaSach,
                        TitleBook = SelectedBook.TenSach,
                        Category = Category,
                        NameAuthor = Author,
                        PublishCompany = SelectedBook.NhaXuatBan,
                        InventoryNumber = InventoryNumber,
                        EntryNumber = EntryNumber,
                        Price = Price,
                        TotalPrice = TotalMoneyFirst = Price * EntryNumber
                    };
                    ListBook.Add(newbook);
                    MyMessageQueue.Clear();
                    MyMessageQueue.Enqueue("Thêm sách thành công");
                }
                else
                {
                    int check = CheckExist();
                    if (check != -1)
                    {
                        ListBook[check].EntryNumber += EntryNumber;
                        ListBook[check].TotalPrice += Price * EntryNumber;
                        LoadListBook();
                        MyMessageQueue.Clear();
                        MyMessageQueue.Enqueue("Thêm sách thành công");
                    }
                    else
                    {
                        ViewEntryBook newbook = new ViewEntryBook()
                        {
                            Id = SelectedBook.MaSach,
                            TitleBook = SelectedBook.TenSach,
                            Category = Category,
                            NameAuthor = Author,
                            PublishCompany = SelectedBook.NhaXuatBan,
                            InventoryNumber = InventoryNumber,
                            EntryNumber = EntryNumber,
                            Price = Price,
                            TotalPrice = TotalMoneyFirst = Price * EntryNumber
                        };
                        ListBook.Add(newbook);
                        MyMessageQueue.Clear();
                        MyMessageQueue.Enqueue("Thêm sách thành công");
                    }
                }
                LoadTotalMoneySecond();
            }
        }
        public void DeleteBookList()
        {
            ListBook.Remove(SelectedListBook);
            LoadListBook();
            LoadTotalMoneySecond();
            MyMessageQueue.Clear();
            MyMessageQueue.Enqueue("Xóa sách thành công");
        }
        public void UpdateListSach()
        {
            foreach (var item in ListBook)
            {
                SACH sach = DataProvider.Ins.DB.SACHes.Where(t => t.MaSach == item.Id).FirstOrDefault();
                sach.SoLuongTon += item.EntryNumber;
                DataProvider.Ins.DB.SaveChanges();
            }
        }
        public void SaveBookList()
        {
            PHIEUNHAPSACH newPHIEUNHAP = new PHIEUNHAPSACH()
            {
                NgayLap = DateTime.Today,
                TongTien = TotalMoneySecond
            };
            DataProvider.Ins.DB.PHIEUNHAPSACHes.Add(newPHIEUNHAP);
            foreach (var item in ListBook)
            {
                CHITIETPHIEUNHAPSACH newCP_PhieuNhap = new CHITIETPHIEUNHAPSACH()
                {
                    MaPhieuNhapSach = newPHIEUNHAP.MaPhieuNhapSach,
                    MaSach = item.Id,
                    SoLuong = item.EntryNumber,
                    DonGiaNhap = item.Price,
                    ThanhTien = item.TotalPrice
                };
                DataProvider.Ins.DB.CHITIETPHIEUNHAPSACHes.Add(newCP_PhieuNhap);
            }
            UpdateListSach();
            DataProvider.Ins.DB.SaveChanges();
            ListBook.Clear();
            SelectedBook = null;
            Category = Author = "";
            InventoryNumber = EntryNumber = 0;
            Price = TotalMoneyFirst = TotalMoneySecond = 0;
            MyMessageQueue.Clear();
            MyMessageQueue.Enqueue("Lưu danh sách nhập sách thành công");
        }
        public void OpenAddBookEntryWindow()
        {
            AddBookEntryWindow addBookEntry = new AddBookEntryWindow(this);
            addBookEntry.ShowDialog();
            ListSACH.Clear();
            ListSACH = DataProvider.Ins.DB.SACHes.ToList();

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookstoreManager.Models;
using BookstoreManager.Models.Db;
using BookstoreManager.Resources.Utils;
using MaterialDesignThemes.Wpf;
using System.Windows.Controls;
using System.Windows.Input;


namespace BookstoreManager.ViewModels.BookViewModels
{
    public class AddBookViewModel: BaseViewModel
    {
        private ManageBookViewModel _manageBookViewModel;

        private int _bookid;
        public int BookId { get { return _bookid; } set { _bookid = value; OnPropertyChanged(nameof(BookId)); } }
        
        private string _bookname;
        public string BookName { get { return _bookname; } set { _bookname = value; OnPropertyChanged(nameof(BookName)); } }

        private string _bookcategory;
        public string BookCategory { get { return _bookcategory; } set { _bookcategory = value; OnPropertyChanged(nameof(BookCategory)); } }
        
        private string _bookauthor;
        public string BookAuthor { get { return _bookauthor; } set { _bookauthor = value; OnPropertyChanged(nameof(BookAuthor)); } }
        
        private string _bookpublishcom;
        public string BookPublishCom { get { return _bookpublishcom; } set { _bookpublishcom = value; OnPropertyChanged(nameof(BookPublishCom)); } }
        
        private int _bookpublishyear;
        public int BookPublishYear { get { return _bookpublishyear; } set { _bookpublishyear = value; OnPropertyChanged(nameof(BookPublishYear)); } }
        
        private int _bookinventory;
        public int BookInventory { get { return _bookinventory; } set { _bookinventory = value; OnPropertyChanged(nameof(BookInventory)); } }
        
        private decimal _bookprice;
        public decimal BookPrice { get { return _bookprice; } set { _bookprice = value; OnPropertyChanged(nameof(BookPrice)); } }

        private List<SACH> _listSACAH;
        public List<SACH> ListSACH { get => _listSACAH; set { _listSACAH = value; OnPropertyChanged(nameof(ListSACH)); } }

        private List<THELOAI> _listTHELOAI;
        public List<THELOAI> ListTHELOAI { get => _listTHELOAI; set { _listTHELOAI = value; OnPropertyChanged(nameof(ListTHELOAI)); } }

        private List<TACGIA> _listTACGIA;
        public List<TACGIA> ListTACGIA { get => _listTACGIA; set { _listTACGIA = value; OnPropertyChanged(nameof(ListTACGIA)); } }

        private List<CHITIETTACGIA> _listCT_TACGIA;
        public List<CHITIETTACGIA> ListCT_TACGIA { get => _listCT_TACGIA; set { _listCT_TACGIA = value; OnPropertyChanged(nameof(ListCT_TACGIA)); } }

        private THELOAI _selectedTheLoai;
        public THELOAI SelectedTheLoai { get => _selectedTheLoai; set { _selectedTheLoai = value; OnPropertyChanged(nameof(SelectedTheLoai)); } }

        


        private SnackbarMessageQueue _myMessageQueue;
        public SnackbarMessageQueue MyMessageQueue { get { return _myMessageQueue; } set { _myMessageQueue = value; OnPropertyChanged(nameof(MyMessageQueue)); } }

        public ICommand CAddBook { get; set; }

        public AddBookViewModel(ManageBookViewModel BookVM)
        {
            _manageBookViewModel = BookVM;
            ListSACH = DataProvider.Ins.DB.SACHes.ToList();
            ListTHELOAI = DataProvider.Ins.DB.THELOAIs.ToList();
            ListTACGIA = DataProvider.Ins.DB.TACGIAs.ToList();
            ListCT_TACGIA = DataProvider.Ins.DB.CHITIETTACGIAs.ToList();

            CAddBook = new RelayCommand<StackPanel>((p) => { return true; }, (p) => { AddBook(p); });
        }

        public bool CheckExistID(int bookid, List<SACH> listSACAH)
        {
            bool check = false;
            foreach (SACH book in listSACAH)
            {
                if(bookid == book.MaSach)
                {
                    check = true;
                    break;
                }
            }
            return check;
        }

        public int FindTacGia(string tentacgia)
        {
            int result = -1;
            for (int i = 0; i < ListTACGIA.Count; i++)
            {
                if (ListTACGIA[i].HoTen == tentacgia)
                {
                    result = ListTACGIA[i].MaTacGia;
                    break;
                }
            }
            return result;
        }

        public void ClearAddBookWindow()
        {
            BookId = BookInventory = BookPublishYear = 0;
            BookName = BookCategory = BookAuthor = BookPublishCom = "";
            BookPrice = 0;
        }

        public void AddBook(StackPanel p)
        {
            if(CheckExistID(BookId, ListSACH))
            {
                _manageBookViewModel.MyMessageQueue.Enqueue("Lỗi. Thông tin sách không hợp lệ");
            }
            else
            {
                if (Validator.IsValid(p))
                {
                    SACH newBook = new SACH();
                    //newBook.MaSach = BookId;
                    newBook.TenSach = BookName;
                    newBook.MaTheLoai = SelectedTheLoai.MaTheLoai;
                    newBook.NhaXuatBan = BookPublishCom;
                    newBook.NamXuatBan = BookPublishYear;
                    newBook.SoLuongTon = BookInventory;
                    newBook.GiaNhap = BookPrice;
                    DataProvider.Ins.DB.SACHes.Add(newBook);
                    if (FindTacGia(BookAuthor) == -1)
                    {
                        TACGIA newTACGIA = new TACGIA()
                        {
                            HoTen = BookAuthor
                        };
                        DataProvider.Ins.DB.TACGIAs.Add(newTACGIA);
                        CHITIETTACGIA newCT_TACGIA = new CHITIETTACGIA()
                        {
                            MaSach = newBook.MaSach,
                            MaTacGia = newTACGIA.MaTacGia
                        };
                        DataProvider.Ins.DB.CHITIETTACGIAs.Add(newCT_TACGIA);
                        DataProvider.Ins.DB.SaveChanges();
                    }
                    else
                    {
                        CHITIETTACGIA newCT_TACGIA = new CHITIETTACGIA()
                        {
                            MaSach = newBook.MaSach,
                            MaTacGia = FindTacGia(BookAuthor)
                        };
                        DataProvider.Ins.DB.CHITIETTACGIAs.Add(newCT_TACGIA);
                        DataProvider.Ins.DB.SaveChanges();
                    }
                    try
                    {
                        DataProvider.Ins.DB.SaveChanges();
                    }
                    catch
                    {
                        _manageBookViewModel.MyMessageQueue.Enqueue("Lỗi. Thông tin sách không hợp lệ");
                        return;
                    }
                    _manageBookViewModel.LoadListBook();
                    ClearAddBookWindow();
                    _manageBookViewModel.MyMessageQueue.Enqueue("Thêm sách thành công!");
                    BAOCAOTON report = new BAOCAOTON();
                    DateTime now = DateTime.Now;
                    report.Thang = now.Month;
                    report.Nam = now.Year;
                    report.MaSach = newBook.MaSach;
                    report.TonDau =0;
                    report.PhatSinh = newBook.SoLuongTon;
                    report.TonCuoi = 0;
                    DataProvider.Ins.DB.BAOCAOTONs.Add(report);
                    DataProvider.Ins.DB.SaveChanges();
                }
                else
                {
                    _manageBookViewModel.MyMessageQueue.Enqueue("Lỗi. Thông tin sách không hợp lệ");
                }
            }
        }
    }
}

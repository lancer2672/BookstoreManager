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
    public class EntryBookViewModel:BaseViewModel
    {
        private List<SACH> _listSACH;
        private SACH _selectedBook;
        private ObservableCollection<ViewEntryBook> _listBook;
        private List<THELOAI> _listTHELOAI;
        private List<TACGIA> _listTACGIA;
        private List<CHITIETTACGIA> _listCT_TACGIA;
        private ViewEntryBook _selectedListBook;
        private string _category;
        private string _author;
        private int _inventorymoney;
        private int _entrynumber;
        private decimal _price;
        private decimal _money;
        private SnackbarMessageQueue _myMessageQueue;

        public List<SACH> ListSACH { get => _listSACH; set { _listSACH = value; OnPropertyChanged(nameof(ListSACH)); } }
        public SACH SelectedBook { get => _selectedBook; set { _selectedBook = value; OnPropertyChanged(nameof(SelectedBook)); LoadInfor(); } }
        public ObservableCollection<ViewEntryBook> ListBook { get => _listBook; set { _listBook = value; OnPropertyChanged(nameof(ListBook)); } }
        public List<THELOAI> ListTHELOAI { get => _listTHELOAI; set { _listTHELOAI = value; OnPropertyChanged(nameof(ListTHELOAI)); } }
        public List<TACGIA> ListTACGIA { get => _listTACGIA; set { _listTACGIA = value; OnPropertyChanged(nameof(ListTACGIA)); } }
        public List<CHITIETTACGIA> ListCT_TACGIA { get => _listCT_TACGIA; set { _listCT_TACGIA = value; OnPropertyChanged(nameof(ListCT_TACGIA)); } }
        public ViewEntryBook SelectedListBook { get => _selectedListBook; set { _selectedListBook = value; OnPropertyChanged(nameof(SelectedListBook)); } }
        public string Category { get => _category; set { _category = value; OnPropertyChanged(nameof(Category)); } }
        public string Author { get => _author; set { _author = value; OnPropertyChanged(nameof(Author)); } }
        public int InventoryNumber { get => _inventorymoney; set { _inventorymoney = value; OnPropertyChanged(nameof(InventoryNumber)); } }
        public int EntryNumber { get => _entrynumber; set { _entrynumber = value; OnPropertyChanged(nameof(EntryNumber)); } }
        public decimal Price { get => _price; set { _price = value; OnPropertyChanged(nameof(Price)); } }
        public decimal Money { get => _money; set { _money = value; OnPropertyChanged(nameof(Money)); } }
        public SnackbarMessageQueue MyMessageQueue { get { return _myMessageQueue; } set { _myMessageQueue = value; OnPropertyChanged(nameof(MyMessageQueue)); } }
        
        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        
        public EntryBookViewModel()
        {
            ListSACH = DataProvider.Ins.DB.SACHes.ToList();
            ListBook = new ObservableCollection<ViewEntryBook>();
            ListTHELOAI = DataProvider.Ins.DB.THELOAIs.ToList();
            ListTACGIA = DataProvider.Ins.DB.TACGIAs.ToList();
            ListCT_TACGIA = DataProvider.Ins.DB.CHITIETTACGIAs.ToList();

            //AddCommand = new RelayCommand<object>((p) => { return true; }, (p) => { ImportFileExcel(); });
            //DeleteCommand = new RelayCommand<object>((p) => { return true; }, (p) => { ExportFileExcel(); });
            //SaveCommand = new RelayCommand<object>((p) => { return true; }, (p) => { SearchCustomer(); });
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
        public void LoadInfor()
        {
            Category = FindCategory(SelectedBook.MaSach, ListTHELOAI);
            Author = FindAuthor(SelectedBook.MaSach, ListTACGIA, ListCT_TACGIA);
            InventoryNumber = (int)SelectedBook.SoLuongTon;
            Price = (decimal)SelectedBook.GiaNhap;
        }
    }
}

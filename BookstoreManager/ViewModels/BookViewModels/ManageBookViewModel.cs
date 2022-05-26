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

namespace BookstoreManager.ViewModels.BookViewModels
{
    public class ManageBookViewModel:BaseViewModel
    {
        private ObservableCollection<ViewBook> _listBook;
        public ObservableCollection<ViewBook> ListBook { get => _listBook; set { _listBook = value; OnPropertyChanged(nameof(ListBook)); } }

        private List<THELOAI> _listTHELOAI;
        public List<THELOAI> ListTHELOAI { get => _listTHELOAI; set { _listTHELOAI = value; OnPropertyChanged(nameof(ListTHELOAI));} }

        private List<TACGIA> _listTACGIA;
        public List<TACGIA> ListTACGIA { get => _listTACGIA; set { _listTACGIA = value; OnPropertyChanged(nameof(ListTACGIA)); } }

        private List<CHITIETTACGIA> _listCT_TACGIA;
        public List<CHITIETTACGIA> ListCT_TACGIA { get => _listCT_TACGIA; set { _listCT_TACGIA = value; OnPropertyChanged(nameof(ListCT_TACGIA)); } }

        private ViewBook _selectedBook;
        public ViewBook SelectedBook { get => _selectedBook; set { _selectedBook = value; OnPropertyChanged(nameof(SelectedBook)); } }
        
        private string _searchKey;
        public string SearchKey { get => _searchKey; set { _searchKey = value; OnPropertyChanged(nameof(SearchKey)); } }

        private SnackbarMessageQueue _myMessageQueue;
        public SnackbarMessageQueue MyMessageQueue { get { return _myMessageQueue; } set { _myMessageQueue = value; OnPropertyChanged(nameof(MyMessageQueue)); } }
        private int _searchTypeSelected;
        private List<string> _searchCombobox;
        public int SearchTypeSelected { get { return _searchTypeSelected; } set { _searchTypeSelected = value; OnPropertyChanged(nameof(SearchTypeSelected)); } }
        public List<string> SearchCombobox { get { return _searchCombobox; } set { _searchCombobox = value; OnPropertyChanged(nameof(SearchCombobox)); } }


        public ICommand CSearch { get; set; }
        public ICommand COpenAddBookWindow { get; set; }
        public ICommand COpenUpdateBookWindow { get; set; }

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
        public ObservableCollection<ViewBook> GetViewBookFromList(List<SACH> listSACH)
        {
            ObservableCollection<ViewBook> list = new ObservableCollection<ViewBook>();
            foreach (SACH book in listSACH)
            {
                ViewBook newViewBook = new ViewBook();
                newViewBook.Id = book.MaSach;
                newViewBook.TitleBook = book.TenSach;
                newViewBook.Category = FindCategory((int)book.MaTheLoai, ListTHELOAI);
                newViewBook.NameAuthor = FindAuthor(book.MaSach, ListTACGIA, ListCT_TACGIA);
                newViewBook.PublishCompany = book.NhaXuatBan;
                newViewBook.PublishYear = (int)book.NamXuatBan;
                newViewBook.InventoryNumber = (int)book.SoLuongTon;
                newViewBook.Price = (decimal)book.GiaNhap;
                list.Add(newViewBook);
            }
            return list;
        }
        public void LoadListBook()
        {
            List<SACH> listSACH = DataProvider.Ins.DB.SACHes.ToList();
            ListBook = GetViewBookFromList(listSACH);
        }
        public void SearchCustomer()
        {
            //if (SearchKey != "" && SearchKey != null)
            //{
            //    List<SACH> listSACH_KQ = DataProvider.Ins.DB.SACHes.Where(t => t.TenSach.ToLower().Contains(SearchKey.ToLower())).ToList();
            //    if (listSACH_KQ.Count == 0)
            //        MyMessageQueue.Enqueue("Không có sách theo tên đã nhập!");
                
            //    ListBook = GetViewBookFromList(listSACH_KQ);

            //}
            //else
            //{
            //    LoadListBook();
                
            //}

            List<SACH> Books = new List<SACH>();
            if (SearchKey != "" && SearchKey != null)
            {
                switch (SearchTypeSelected)
                {
                    case 0:
                        Books = DataProvider.Ins.DB.SACHes.Where(t => t.TenSach.ToLower().Contains(SearchKey.ToLower())).ToList();
                        break;
                    case 1:
                        List<THELOAI> TypeBookList = DataProvider.Ins.DB.THELOAIs.Where(t => t.TenTheLoai.ToLower().Contains(SearchKey.ToLower())).ToList();
                        Books = new List<SACH>();
                        foreach (THELOAI item in TypeBookList)
                        {
                            List<SACH> list = DataProvider.Ins.DB.SACHes.Where(t => t.MaTheLoai == item.MaTheLoai).ToList();
                            Books.AddRange(list);
                        }
                        ListBook = GetViewBookFromList(Books);
                        break;
                    case 2:
                        List<TACGIA> Authors = DataProvider.Ins.DB.TACGIAs.Where(t => t.HoTen.ToLower().Contains(SearchKey.ToLower())).ToList();
                        List<CHITIETTACGIA> AuthorDetail = new List<CHITIETTACGIA>();
                        foreach (TACGIA item in Authors)
                        {
                            List<CHITIETTACGIA> List = DataProvider.Ins.DB.CHITIETTACGIAs.Where(t => t.MaTacGia == item.MaTacGia).ToList();
                            AuthorDetail.AddRange(List);
                        }
                        foreach (CHITIETTACGIA item in AuthorDetail)
                        {
                            List<SACH> list = DataProvider.Ins.DB.SACHes.Where(t => t.MaSach == item.MaSach).ToList();
                            Books.AddRange(list);
                        }
                        break;
                }
                ListBook = GetViewBookFromList(Books);
            }
            else
            {
                LoadListBook();
            }
        }
        public void OpenAddBookWindow()
        {
            AddBookWindow addBook = new AddBookWindow(this);
            addBook.ShowDialog();
        }
        public void OpenUpdateBookWindow(ListView listView)
        {
            System.Collections.IList list = listView.SelectedItems;
            if (list.Count == 0)
            {
                return;
            }
            UpdateBookWindow updateBook = new UpdateBookWindow(this);
            updateBook.ShowDialog();

        }

        public ManageBookViewModel()
        {
            ListBook = new ObservableCollection<ViewBook>();
            ListTHELOAI = DataProvider.Ins.DB.THELOAIs.ToList();
            ListTACGIA = DataProvider.Ins.DB.TACGIAs.ToList();
            ListCT_TACGIA = DataProvider.Ins.DB.CHITIETTACGIAs.ToList();
            LoadListBook();

            CSearch = new RelayCommand<ListView>((p) => { return true; }, (p) => { SearchCustomer(); });
            COpenAddBookWindow = new RelayCommand<object>((p) => { return true; }, (p) => { OpenAddBookWindow(); });
            COpenUpdateBookWindow = new RelayCommand<ListView>((p) => { return true; }, (p) => { OpenUpdateBookWindow(p); });

            MyMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(4000));
            MyMessageQueue.DiscardDuplicates = true;

            SearchCombobox = new List<String>() { "Tên Sách", "Thể Loại","Tác Giả" };
            SearchTypeSelected = 0;
        }
    }
}

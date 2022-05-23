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
using BookstoreManager.ViewModels.Customers;
using BookstoreManager.Views.Customers;
using MaterialDesignThemes.Wpf;

namespace BookstoreManager.ViewModels.BookViewModels
{
    public class ManageBookViewModel:BaseViewModel
    {
        private ObservableCollection<ViewBook> _listBook;
        public ObservableCollection<ViewBook> ListBook { get => _listBook; set { _listBook = value; OnPropertyChanged(nameof(ListBook)); } }

        private List<THELOAI> _listTHELOAI;
        public List<THELOAI> ListTHELOAI { get => _listTHELOAI; set { _listTHELOAI = value; OnPropertyChanged(nameof(ListTHELOAI));} }
        
        private ViewCustomer _selectedCustomer;
        public ViewCustomer SelectedCustomer { get => _selectedCustomer; set { _selectedCustomer = value; OnPropertyChanged(nameof(SelectedCustomer)); } }
        
        private string _searchKey;
        public string SearchKey { get => _searchKey; set { _searchKey = value; OnPropertyChanged(nameof(SearchKey)); } }

        private SnackbarMessageQueue _myMessageQueue;
        public SnackbarMessageQueue MyMessageQueue { get { return _myMessageQueue; } set { _myMessageQueue = value; OnPropertyChanged(nameof(MyMessageQueue)); } }
        
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
        public ObservableCollection<ViewBook> GetViewBookFromList(List<SACH> listSACH)
        {
            ObservableCollection<ViewBook> list = new ObservableCollection<ViewBook>();
            foreach (SACH book in listSACH)
            {
                ViewBook newViewBook = new ViewBook();
                newViewBook.Id = book.MaSach;
                newViewBook.TitleBook = book.TenSach;
                newViewBook.Category = FindCategory((int)book.MaTheLoai, ListTHELOAI);
                newViewBook.PublishCompany = book.NhaXuatBan;
                newViewBook.PublishYear = (int)book.NamXuatBan;
                newViewBook.InventoryNumber = (int)book.SoLuongTon;
                newViewBook.Price = (decimal)book.GiaNhap;
                list.Add(newViewBook);
            }
            return list;
        }
        public void LoadListCustomer()
        {
            List<SACH> listSACH = DataProvider.Ins.DB.SACHes.ToList();
            ListBook = GetViewBookFromList(listSACH);
        }

        public ManageBookViewModel()
        {
            ListBook = new ObservableCollection<ViewBook>();
            ListTHELOAI = DataProvider.Ins.DB.THELOAIs.ToList();

            //COpenAddBookWindow = new RelayCommand<object>((p) => { return true; }, (p) => { OpenAddBookWindow(); });
        }
    }
}

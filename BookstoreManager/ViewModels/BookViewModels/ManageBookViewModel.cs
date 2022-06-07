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
    public class ManageBookViewModel:BaseViewModel
    {
        private ObservableCollection<ViewBook> _listBook;
        private List<SACH> _listSACH;
        private List<THELOAI> _listTHELOAI;
        private List<TACGIA> _listTACGIA; 
        private List<CHITIETTACGIA> _listCT_TACGIA;
        private ViewBook _selectedBook;
        private string _searchKey;
        private SnackbarMessageQueue _myMessageQueue;
        private int _searchTypeSelected;
        private List<string> _searchCombobox;

        public ObservableCollection<ViewBook> ListBook { get => _listBook; set { _listBook = value; OnPropertyChanged(nameof(ListBook)); } }
        public List<THELOAI> ListTHELOAI { get => _listTHELOAI; set { _listTHELOAI = value; OnPropertyChanged(nameof(ListTHELOAI));} }
        public List<SACH> ListSACH { get => _listSACH; set { _listSACH = value; OnPropertyChanged(nameof(ListSACH));  } }
        public List<TACGIA> ListTACGIA { get => _listTACGIA; set { _listTACGIA = value; OnPropertyChanged(nameof(ListTACGIA)); } }
        public List<CHITIETTACGIA> ListCT_TACGIA { get => _listCT_TACGIA; set { _listCT_TACGIA = value; OnPropertyChanged(nameof(ListCT_TACGIA)); } }
        public ViewBook SelectedBook { get => _selectedBook; set { _selectedBook = value; OnPropertyChanged(nameof(SelectedBook)); } }
        public string SearchKey { get => _searchKey; set { _searchKey = value; OnPropertyChanged(nameof(SearchKey)); } }
        public SnackbarMessageQueue MyMessageQueue { get { return _myMessageQueue; } set { _myMessageQueue = value; OnPropertyChanged(nameof(MyMessageQueue)); } }
        public int SearchTypeSelected { get { return _searchTypeSelected; } set { _searchTypeSelected = value; OnPropertyChanged(nameof(SearchTypeSelected)); } }
        public List<string> SearchCombobox { get { return _searchCombobox; } set { _searchCombobox = value; OnPropertyChanged(nameof(SearchCombobox)); } }

        public ICommand CRefreshData { get; set; }
        public ICommand CSearch { get; set; }
        public ICommand COpenAddBookWindow { get; set; }
        public ICommand COpenUpdateBookWindow { get; set; }
        public ICommand CImportExcel { get; set; }
        public ICommand CExportExcel { get; set; }


        public ManageBookViewModel()
        {
            ListBook = new ObservableCollection<ViewBook>();
            ListSACH = DataProvider.Ins.DB.SACHes.ToList();
            ListTHELOAI = DataProvider.Ins.DB.THELOAIs.ToList();
            ListTACGIA = DataProvider.Ins.DB.TACGIAs.ToList();
            ListCT_TACGIA = DataProvider.Ins.DB.CHITIETTACGIAs.ToList();
            LoadListBook();

            CImportExcel = new RelayCommand<object>((p) => { return true; }, (p) => { ImportFileExcel(); });
            CExportExcel = new RelayCommand<object>((p) => { return true; }, (p) => { ExportFileExcel(); });
            CSearch = new RelayCommand<ListView>((p) => { return true; }, (p) => { SearchBook(); });
            COpenAddBookWindow = new RelayCommand<object>((p) => { return true; }, (p) => { OpenAddBookWindow(); });
            COpenUpdateBookWindow = new RelayCommand<ListView>((p) => { return true; }, (p) => { OpenUpdateBookWindow(p); });
            CRefreshData = new RelayCommand<object>((p) => { return true; }, (p) => { RefreshData(); });

            MyMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(2000));
            MyMessageQueue.DiscardDuplicates = true;

            SearchCombobox = new List<string>() { "Tên Sách", "Thể Loại", "Tác Giả" };
            SearchTypeSelected = 0;
        }

        public string FindCategory(int matheloai)
        {
            List<THELOAI> listTHELOAI = DataProvider.Ins.DB.THELOAIs.ToList();
            string category = "";
            for(int i = 0; i < listTHELOAI.Count; i++)
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
            //foreach (var item in listCT_TACGIA)
            //{
            //    if (masach == item.MaSach)
            //    {
            //        matacgia = (int)item.MaTacGia;
            //        break;
            //    }
            //}
            //foreach (var item in listTACGIA)
            //{
            //    if (matacgia == item.MaTacGia)
            //    {
            //        author = item.HoTen;
            //        break;
            //    }
            //}
            for (int i = 0; i < ListCT_TACGIA.Count; i++) 
            {
                if (masach == ListCT_TACGIA[i].MaSach)
                {
                    matacgia = (int)ListCT_TACGIA[i].MaTacGia;
                    break;
                }
            }
            for (int i = 0; i < ListTACGIA.Count; i++)
            {
                if (matacgia == ListTACGIA[i].MaTacGia)
                {
                    author = ListTACGIA[i].HoTen;
                    break;
                }
            }
            return author;
        }
        public ObservableCollection<ViewBook> GetViewBookFromList(List<SACH> listSACH)
        {
            ObservableCollection<ViewBook> list = new ObservableCollection<ViewBook>();
            //foreach (SACH book in listSACH)
            //{
            //    ViewBook newViewBook = new ViewBook();
            //    newViewBook.Id = book.MaSach;
            //    newViewBook.TitleBook = book.TenSach;
            //    newViewBook.Category = FindCategory((int)book.MaTheLoai, ListTHELOAI);
            //    newViewBook.NameAuthor = FindAuthor(book.MaSach, ListTACGIA, ListCT_TACGIA);
            //    newViewBook.PublishCompany = book.NhaXuatBan;
            //    newViewBook.PublishYear = (int)book.NamXuatBan;
            //    newViewBook.InventoryNumber = (int)book.SoLuongTon;
            //    newViewBook.Price = (decimal)book.GiaNhap;
            //    list.Add(newViewBook);
            //}
            for (int i = 0; i < listSACH.Count; i++) 
            {
                ViewBook newViewBook = new ViewBook();
                newViewBook.Id = listSACH[i].MaSach;
                newViewBook.TitleBook = listSACH[i].TenSach;
                newViewBook.Category = FindCategory((int)listSACH[i].MaTheLoai);
                newViewBook.NameAuthor = FindAuthor(listSACH[i].MaSach);
                newViewBook.PublishCompany = listSACH[i].NhaXuatBan;
                newViewBook.PublishYear = (int)listSACH[i].NamXuatBan;
                newViewBook.InventoryNumber = (int)listSACH[i].SoLuongTon;
                newViewBook.Price = (decimal)listSACH[i].GiaNhap;
                list.Add(newViewBook);
            }
            return list;
        }
        public void LoadListBook()
        {
            ListBook.Clear();
            List<SACH> listSACH = DataProvider.Ins.DB.SACHes.ToList();
            ListBook = GetViewBookFromList(listSACH);
        }
        public void SearchBook()
        {
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

        public void RefreshData()
        {
            SearchKey = "";
            List<SACH> books = DataProvider.Ins.DB.SACHes.ToList();
            ListBook = GetViewBookFromList(books);
        }

        public bool CheckExistID(string bookname)
        {
            bool check = false;
            //foreach (SACH book in listSACAH)
            //{
            //    if (bookname == book.TenSach)
            //    {
            //        check = true;
            //        break;
            //    }
            //}
            for (int i = 0; i < ListSACH.Count; i++) 
            {
                if (bookname == ListSACH[i].TenSach)
                {
                    check = true;
                    break;
                }
            }
            return check;
        }

        public int FindMatheloai(string theloai)
        {
            int result = -1;
            for (int i = 0; i < ListTHELOAI.Count; i++) 
            {
                if (ListTHELOAI[i].TenTheLoai == theloai)
                {
                    result = ListTHELOAI[i].MaTheLoai;
                    break;
                }
            }
            return result;
        }
        public int FindTacGia(string tentacgia)
        {
            int result = -1;
            for (int i = 0; i < ListTACGIA.Count; i++)
            {
                if ( ListTACGIA[i].HoTen == tentacgia)
                {
                    result = ListTACGIA[i].MaTacGia;
                    break;
                }
            }
            return result;
        }

        public void ImportFileExcel()
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = dialog.FileName;
                AddImportedData(fileName);
            }
        }

        public void AddImportedData(string filename)
        {
            try
            {
                var package = new ExcelPackage(new FileInfo(filename));
                ExcelWorksheet workSheet = package.Workbook.Worksheets[0];

                for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
                {

                    try
                    {
                        // biến j biểu thị cho một column trong file
                        int j = 1;
                        bool check = true;
                        if (CheckExistID(workSheet.Cells[i, 1].Value.ToString()))
                        {
                            SACH sach = DataProvider.Ins.DB.SACHes.Where(t => t.TenSach == workSheet.Cells[i, j].Value.ToString()).FirstOrDefault();
                            //sach.SoLuongTon = Convert.ToInt32(workSheet.Cells[i, 6].Value);
                            //DataProvider.Ins.DB.SaveChanges();
                        }
                        else
                        {
                            SACH newBook = new SACH()
                            {
                                TenSach = workSheet.Cells[i, 1].Value.ToString(),
                                NhaXuatBan = workSheet.Cells[i, 4].Value.ToString(),
                                NamXuatBan = Convert.ToInt32(workSheet.Cells[i, 5].Value),
                                SoLuongTon = Convert.ToInt32(workSheet.Cells[i, 6].Value),
                                GiaNhap = Convert.ToDecimal(workSheet.Cells[i, 7].Value),
                            };
                            if (FindMatheloai(workSheet.Cells[i, 2].Value.ToString()) == -1)
                            {
                                THELOAI newTHELOAI = new THELOAI()
                                {
                                    TenTheLoai = workSheet.Cells[i, 2].Value.ToString()
                                };
                                DataProvider.Ins.DB.THELOAIs.Add(newTHELOAI);
                                DataProvider.Ins.DB.SaveChanges();
                                newBook.MaTheLoai = newTHELOAI.MaTheLoai;
                            }
                            else
                            {
                                newBook.MaTheLoai = FindMatheloai(workSheet.Cells[i, 2].Value.ToString());
                            }
                            DataProvider.Ins.DB.SACHes.Add(newBook);
                            DataProvider.Ins.DB.SaveChanges();
                            if (FindTacGia(workSheet.Cells[i, 3].Value.ToString()) == -1)
                            {
                                TACGIA newTACGIA = new TACGIA()
                                {
                                    HoTen = workSheet.Cells[i, 3].Value.ToString()
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
                                    MaTacGia = FindTacGia(workSheet.Cells[i, 3].Value.ToString())
                                };
                                DataProvider.Ins.DB.CHITIETTACGIAs.Add(newCT_TACGIA);
                                DataProvider.Ins.DB.SaveChanges();
                            }
                        }
                        MyMessageQueue.Enqueue("Thêm dữ liệu từ file excel thành công!");
                    }
                    catch (Exception error)
                    {
                        MyMessageQueue.Enqueue("Lỗi! Không thể nhập liệu từ file excel");

                    }
                }
                LoadListBook();
            }
            catch (Exception ee)
            {
                MyMessageQueue.Enqueue("Lỗi! Không thể nhập liệu từ file Excel");
            }
        }
        public void ExportFileExcel()
        {
            string filePath = "";
            System.Windows.Forms.SaveFileDialog dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.Filter = "Excel files (*.xls or .xlsx)|.xls;*.xlsx";

            // Nếu mở file và chọn nơi lưu file thành công sẽ lưu đường dẫn lại dùng
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePath = dialog.FileName;
            }

            // nếu đường dẫn null hoặc rỗng thì báo không hợp lệ và return hàm
            if (string.IsNullOrEmpty(filePath))
            {
                MyMessageQueue.Enqueue("Lỗi. Đường dẫn không hợp lệ.");
                return;
            }

            try
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    //package.Workbook.Properties.Author = "Admin";
                    //package.Workbook.Properties.Title = "Báo cáo tồn";
                    package.Workbook.Worksheets.Add("Sheet 1");
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                    //add sheet
                    workSheet.Name = "Sheet 1";
                    workSheet.Cells.Style.Font.Size = 12;
                    workSheet.Cells.Style.Font.Name = "Calibri";
                    // Tạo danh sách các column header
                    string[] arrColumnHeader = {
                       "Tên sách",
                       "Thể loại",
                       "Tên tác giả",
                       "Nhà xuất bản",
                       "Năm xuất bản",
                       "Số lượng tồn",
                       "Giá nhập"
                    };

                    var countColHeader = arrColumnHeader.Count();

                    int colIndex = 1;
                    int rowIndex = 2;

                    //tạo các header từ column header đã tạo từ bên trên
                    foreach (var item in arrColumnHeader)
                    {
                        var cell = workSheet.Cells[rowIndex, colIndex];

                        //gán giá trị
                        cell.Value = item;

                        colIndex++;
                    }

                    foreach (var item in ListBook)
                    {
                        colIndex = 1;
                        rowIndex++;
                        
                        workSheet.Cells[rowIndex, colIndex++].Value = item.TitleBook;
                        workSheet.Cells[rowIndex, colIndex++].Value = item.Category;
                        workSheet.Cells[rowIndex, colIndex++].Value = item.NameAuthor;
                        workSheet.Cells[rowIndex, colIndex++].Value = item.PublishCompany;
                        workSheet.Cells[rowIndex, colIndex++].Value = item.PublishYear;
                        workSheet.Cells[rowIndex, colIndex++].Value = item.InventoryNumber;
                        workSheet.Cells[rowIndex, colIndex++].Value = item.Price;
                    }

                    //Lưu file lại
                    Byte[] bin = package.GetAsByteArray();
                    File.WriteAllBytes(filePath, bin);

                }
                MyMessageQueue.Enqueue("Xuất excel thành công!");
            }
            catch (Exception ee)
            {
                MyMessageQueue.Enqueue("Lỗi. Không thể xuất file Excel");
            }
        }
    }
}

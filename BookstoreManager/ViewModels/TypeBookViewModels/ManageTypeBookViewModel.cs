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

namespace BookstoreManager.ViewModels.TypeBookViewModels
{
    public class ManageTypeBookViewModel:BaseViewModel
    {
        private string _searchKey;
        private string _category;
        private List<THELOAI> _listcategory;
        public THELOAI _selectedcategory;
        private SnackbarMessageQueue _myMessageQueue;

        public string SearchKey { get => _searchKey; set { _searchKey = value; OnPropertyChanged(nameof(SearchKey)); } }
        public string Category { get => _category; set { _category = value; OnPropertyChanged(nameof(Category)); } }
        public List<THELOAI> ListCategory { get => _listcategory; set { _listcategory = value; OnPropertyChanged(nameof(ListCategory)); } }
        public THELOAI SelectedCategory { get => _selectedcategory; set { _selectedcategory = value; OnPropertyChanged(nameof(SelectedCategory)); if (SelectedCategory != null) Category = SelectedCategory.TenTheLoai; } }
        public SnackbarMessageQueue MyMessageQueue { get { return _myMessageQueue; } set { _myMessageQueue = value; OnPropertyChanged(nameof(MyMessageQueue)); } }
        public ICommand CRefreshData { get; set; }
        public ICommand CSearch { get; set; }
        public ICommand CAddTypeBook { get; set; }
        public ICommand CUpdateTypeBook { get; set; }
        public ICommand CDeleteTypeBook { get; set; }
        public ICommand CImportExcel { get; set; }
        public ICommand CExportExcel { get; set; }
        public ManageTypeBookViewModel()
        {
            ListCategory = DataProvider.Ins.DB.THELOAIs.ToList();

            CImportExcel = new RelayCommand<object>((p) => { return true; }, (p) => { ImportFileExcel(); });
            CExportExcel = new RelayCommand<object>((p) => { return true; }, (p) => { ExportFileExcel(); });
            CSearch = new RelayCommand<ListView>((p) => { return true; }, (p) => { SearchTypeBook(); });
            CAddTypeBook = new RelayCommand<object>((p) => { return true; }, (p) => { AddTypeBook(); });
            CUpdateTypeBook = new RelayCommand<object>((p) => { return true; }, (p) => { UpdateTypeBook(); });
            CDeleteTypeBook = new RelayCommand<object>((p) => { return true; }, (p) => { DeleteTypeBook(); });
            CRefreshData = new RelayCommand<object>((p) => { return true; }, (p) => { RefreshData(); });

            MyMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(2000));
            MyMessageQueue.DiscardDuplicates = true;
        }
        public void LoadListCategory()
        {
            ListCategory.Clear();
            ListCategory = DataProvider.Ins.DB.THELOAIs.ToList();
        }
        public void SearchTypeBook()
        {
            if (SearchKey != "" && SearchKey != null)
            {
                ListCategory.Clear();
                ListCategory = DataProvider.Ins.DB.THELOAIs.Where(t => t.TenTheLoai.ToLower().Contains(SearchKey.ToLower())).ToList();
            }
            else
            {
                LoadListCategory();
            }
        }
        public void RefreshData()
        {
            LoadListCategory();
            SearchKey = null;
        }
        public void AddTypeBook()
        {
            if (Category == null)
            {
                MyMessageQueue.Clear();
                MyMessageQueue.Enqueue("Vui lòng nhập tên thể loại!");
            }
            else
            {
                THELOAI newCategory = new THELOAI() { TenTheLoai = Category };
                DataProvider.Ins.DB.THELOAIs.Add(newCategory);
                DataProvider.Ins.DB.SaveChanges();
                Category = "";
                LoadListCategory();
                MyMessageQueue.Clear();
                MyMessageQueue.Enqueue("Thêm thể loại thành công");
            }
        }
        public void UpdateTypeBook()
        {
            if (SelectedCategory==null)
            {
                MyMessageQueue.Clear();
                MyMessageQueue.Enqueue("Vui lòng chọn thể loại muốn sửa!");
            }
            else
            {
                THELOAI editCategory = DataProvider.Ins.DB.THELOAIs.Where(t => t.MaTheLoai == SelectedCategory.MaTheLoai).FirstOrDefault();
                editCategory.TenTheLoai=Category;
                DataProvider.Ins.DB.SaveChanges();
                SelectedCategory = null;
                Category = "";
                LoadListCategory();
                MyMessageQueue.Clear();
                MyMessageQueue.Enqueue("Sửa thể loại thành công!");
            }
        }
        public void DeleteTypeBook()
        {
            if (SelectedCategory == null)
            {
                MyMessageQueue.Clear();
                MyMessageQueue.Enqueue("Vui lòng chọn thể loại muốn sửa!");
            }
            else
            {
                List<SACH> ListSACH = DataProvider.Ins.DB.SACHes.Where(t => t.MaTheLoai == SelectedCategory.MaTheLoai).ToList();
                if (ListSACH.Count == 0)
                {
                    THELOAI deleteCategory = DataProvider.Ins.DB.THELOAIs.Where(t => t.MaTheLoai == SelectedCategory.MaTheLoai).FirstOrDefault();
                    DataProvider.Ins.DB.THELOAIs.Remove(deleteCategory);
                    DataProvider.Ins.DB.SaveChanges();
                    SelectedCategory = null;
                    Category = "";
                    LoadListCategory();
                    MyMessageQueue.Clear();
                    MyMessageQueue.Enqueue("Xóa thể loại thành công!");
                }
                else
                {
                    SelectedCategory = null;
                    Category = "";
                    LoadListCategory();
                    MyMessageQueue.Clear();
                    MyMessageQueue.Enqueue("Không thể xóa vì vẫn có sách thuộc thể loại này!");
                }
            }
        }
        public bool CheckCategory(string tentheloai)
        {
            bool check = false;
            List<THELOAI> listTheLoai = DataProvider.Ins.DB.THELOAIs.ToList();
            for(int i = 0; i < listTheLoai.Count; i++)
            {
                if (String.Compare(listTheLoai[i].TenTheLoai, tentheloai, true) == 0)
                {
                    check = true;
                    break;
                }
            }
            return check;
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
                        bool check = true;
                        if (!CheckCategory(workSheet.Cells[i, 1].Value.ToString()))
                        {
                            THELOAI newtheloai = new THELOAI()
                            {
                                TenTheLoai = workSheet.Cells[i, 2].Value.ToString(),
                            };
                            DataProvider.Ins.DB.THELOAIs.Add(newtheloai);
                            DataProvider.Ins.DB.SaveChanges();
                        }
                        //else
                        //{
                            
                        //}
                        MyMessageQueue.Enqueue("Thêm dữ liệu từ file excel thành công!");
                    }
                    catch (Exception error)
                    {
                        MyMessageQueue.Enqueue("Lỗi! Không thể nhập liệu từ file excel");

                    }
                }
                LoadListCategory();
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
                        "STT",
                        "Tên thể loại"
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
                    int stt = 0;
                    foreach (var item in ListCategory)
                    {
                        colIndex = 1;
                        rowIndex++;
                        stt++;

                        workSheet.Cells[rowIndex, colIndex++].Value = stt;
                        workSheet.Cells[rowIndex, colIndex++].Value = item.TenTheLoai;
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

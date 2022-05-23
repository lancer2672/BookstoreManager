﻿using BookstoreManager.Models;
using BookstoreManager.Models.Db;
using MaterialDesignThemes.Wpf;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BookstoreManager.ViewModels.ReportAndStatistic
{
    public class InventoryReportViewModel : BaseViewModel
    {
        private ObservableCollection<InventoryReportItem> _dataListView;
        private string _title;
        private List<int> _listYear;
        private List<int> _listMonth = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        private int _selectedYear;
        private int _selectedMonth;
        private string _searchKey;
        private ObservableCollection<InventoryReportChartModel> _dataChart;
        private Visibility _isVisible;

        private SnackbarMessageQueue _myMessageQueue;

        public string Title { get { return _title; } set { _title = value; OnPropertyChanged(nameof(Title)); } }
        public Visibility IsVisible { get { return _isVisible; } set { _isVisible = value;OnPropertyChanged(nameof(IsVisible)); } }
        public SnackbarMessageQueue MyMessageQueue { get { return _myMessageQueue; } set { _myMessageQueue = value; OnPropertyChanged(nameof(MyMessageQueue)); } }
        public List<int> ListMonth { get => _listMonth; }
        public List<int> ListYear { get => _listYear; set { _listYear = value; OnPropertyChanged(nameof(ListYear)); } }
        public int SelectedMonth { get { return _selectedMonth; } set { _selectedMonth = value; OnPropertyChanged(nameof(SelectedMonth)); } }
        public int SelectedYear { get { return _selectedYear; } set { _selectedYear = value; LoadDataChart(); OnPropertyChanged(nameof(SelectedYear)); } }
        public ObservableCollection<InventoryReportChartModel> DataChart { get { return _dataChart; } set { _dataChart = value; OnPropertyChanged(nameof(DataChart)); } }
        public string SearchKey { get { return _searchKey; } set { _searchKey = value; OnPropertyChanged(nameof(SearchKey)); } }

        public ObservableCollection<InventoryReportItem> DataListView { get { return _dataListView; } set { _dataListView = value; OnPropertyChanged(nameof(DataListView)); } }

        public ICommand CLoadData { get; set; }
        public ICommand CSearch { get; set; }
        public ICommand CImportExcel { get; set; }

        public ICommand CExportExcel { get; set; }


        public InventoryReportViewModel()
        {
            IsVisible = Visibility.Hidden;
            Title = "Báo Cáo Tồn";
            ListYear = new List<int>();
            DataListView = new ObservableCollection<InventoryReportItem>();
            DataChart = new ObservableCollection<InventoryReportChartModel>();

            CLoadData = new RelayCommand<object>((p) => { return true; }, (p) => { LoadDataListView(true); });
            CSearch = new RelayCommand<object>((p) => { return true; }, (p) => { SearchBook(); });
            CImportExcel = new RelayCommand<object>((p) => { return true; }, (p) => { ImportFileExcel(); });
            CExportExcel = new RelayCommand<object>((p) => { return true; }, (p) => { ExportFileExcel(); });

            MyMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(2500));
            MyMessageQueue.DiscardDuplicates = true;

            LoadDataComboBox();
        }
        public void SearchBook()
        {
            if (SearchKey != "" && SearchKey != null)
            {
                List<SACH> BookList = DataProvider.Ins.DB.SACHes.Where(t => t.TenSach.ToLower().Contains(SearchKey.ToLower())).ToList();
                List<BAOCAOTON> InvReport = DataProvider.Ins.DB.BAOCAOTONs.Where(p => p.Thang == SelectedMonth && p.Nam == SelectedYear ).ToList();

                DataListView = GetDataListViewFromDB(InvReport, BookList);
            }
            else
            {
                LoadDataListView(false);
            }
        }
        public void LoadDataComboBox()
        {
            BAOCAOTON BaoCaoTon = DataProvider.Ins.DB.BAOCAOTONs.FirstOrDefault();
            if (BaoCaoTon != null)
            {
                for (int i = (int)BaoCaoTon.Nam; i <= DateTime.Now.Year; i++)
                {
                    ListYear.Add(i);
                }
            }
            else
            {
                ListYear.Add(DateTime.Now.Year);
            }
        }
        public void LoadDataListView(bool IsNotify)
        {
            SearchKey = "";
            List<BAOCAOTON> InvReport = DataProvider.Ins.DB.BAOCAOTONs.Where(p => p.Thang == SelectedMonth && p.Nam == SelectedYear).ToList();
            List<SACH> BookList = DataProvider.Ins.DB.SACHes.ToList();
            if (InvReport.Count !=0 )
            {
                DataListView = GetDataListViewFromDB(InvReport, BookList);
                if(IsNotify)
                {
                MyMessageQueue.Enqueue("Tạo báo cáo thành công!");
                }
                Title = "Báo Cáo Tồn Tháng " + SelectedMonth.ToString() + " Năm " + SelectedYear.ToString();
            }
            else
            {
                if (IsNotify)
                {
                    MyMessageQueue.Enqueue("Không có thông tin");
                }
                DataListView.Clear();
                Title = "Báo Cáo Tồn";
            }
        }
        public ObservableCollection<InventoryReportItem> GetDataListViewFromDB(List<BAOCAOTON> InvReport,List<SACH> BookList)
        {
            ObservableCollection<InventoryReportItem> Data = new ObservableCollection<InventoryReportItem>();
            foreach (BAOCAOTON item in InvReport)
            {
                for (int i = 0; i < BookList.Count; i++)
                {
                    if(BookList[i].MaSach == item.MaSach)
                    {
                        int Indentity = (int)BookList[i].MaTheLoai;
                        THELOAI Type = DataProvider.Ins.DB.THELOAIs.Where(p => p.MaTheLoai == Indentity).SingleOrDefault();
                        InventoryReportItem InvItem = new InventoryReportItem();
                        InvItem.BookId = BookList[i].MaSach;
                        InvItem.ReportId = item.MaBaoCao;
                        InvItem.BookName = BookList[i].TenSach;
                        InvItem.Type = Type.TenTheLoai;
                        InvItem.FirstQuantity = (int)item.TonDau;
                        InvItem.IncurredQuantity = (int)item.PhatSinh;
                        InvItem.EndQuantity = (int)item.TonCuoi;
                        InvItem.Month = (int)item.Thang;
                        InvItem.Year = (int)item.Nam;
                        Data.Add(InvItem);
                    }
                }
            }
            return Data;

        }

        public void LoadDataChart()
        {
            IsVisible = Visibility.Visible;
            for (int i = 1; i <= 12; i++)
            {
                int Quantity = CountBookInMonth(i,SelectedYear);
                InventoryReportChartModel Data = new InventoryReportChartModel(Quantity, i);
                DataChart.Add(Data);
            }
        }
        public int CountBookInMonth(int month,int year)
        {
            int Count = 0;
            List<BAOCAOTON> ListReport = DataProvider.Ins.DB.BAOCAOTONs.Where(p => p.Thang == month && p.Nam == year).ToList();
            for (int i = 0; i < ListReport.Count; i++)
            {
                Count += (int)ListReport[i].TonCuoi;
            }
            return Count;
            
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
                        BAOCAOTON newReport = new BAOCAOTON()
                        {
                            Thang = Convert.ToInt32(workSheet.Cells[i, j++].Value),
                            Nam = Convert.ToInt32(workSheet.Cells[i, j++].Value),
                            MaSach = Convert.ToInt32(workSheet.Cells[i, j++].Value.ToString()),
                            TonDau = Convert.ToInt32(workSheet.Cells[i, j++].Value),
                            PhatSinh = Convert.ToInt32(workSheet.Cells[i, j++].Value),
                            TonCuoi = Convert.ToInt32(workSheet.Cells[i, j++].Value)

                        };
                        DataProvider.Ins.DB.BAOCAOTONs.Add(newReport);
                        DataProvider.Ins.DB.SaveChanges();

                        MyMessageQueue.Enqueue("thêm dữ liệu từ file excel thành công!");

                    }
                    catch (Exception error)
                    {
                        MyMessageQueue.Enqueue("Lỗi! Không thể nhập liệu từ file excel");

                    }
                }
                LoadDataListView(false);
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
                    package.Workbook.Properties.Author = "Admin";
                    package.Workbook.Properties.Title = "Danh sách khách hàng";
                    package.Workbook.Worksheets.Add("Sheet 1");
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                    //add sheet
                    workSheet.Name = "Sheet 1";
                    workSheet.Cells.Style.Font.Size = 12;
                    workSheet.Cells.Style.Font.Name = "Calibri";
                    // Tạo danh sách các column header
                    string[] arrColumnHeader = {
                        "Tháng",
                        "Năm",
                        "Mã Sách",
                        "Tồn Đầu",
                        "Phát Sinh",
                        "Tồn Cuối",
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

                    foreach (var item in DataListView)
                    {
                        colIndex = 1;
                        rowIndex++;

                        workSheet.Cells[rowIndex, colIndex++].Value = item.Month;
                        workSheet.Cells[rowIndex, colIndex++].Value = item.Year;
                        workSheet.Cells[rowIndex, colIndex++].Value = item.BookId;
                        workSheet.Cells[rowIndex, colIndex++].Value = item.FirstQuantity;
                        workSheet.Cells[rowIndex, colIndex++].Value = item.IncurredQuantity;
                        workSheet.Cells[rowIndex, colIndex++].Value = item.EndQuantity;

                    }

                    //Lưu file lại
                    Byte[] bin = package.GetAsByteArray();
                    File.WriteAllBytes(filePath, bin);

                }
                MessageBox.Show("Xuất file thành công");
                MyMessageQueue.Enqueue("Xuất excel thành công!");
            }
            catch (Exception ee)
            {
                MessageBox.Show("Xuất file không thành công");
                MyMessageQueue.Enqueue("Lỗi. Không thể xuất file Excel");
            }
        }
    }
}

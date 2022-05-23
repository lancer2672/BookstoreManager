using BookstoreManager.Models;
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
    public class DebtReportViewModel : BaseViewModel
    {
        private ObservableCollection<DebtReportItem> _dataListView;
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

        public Visibility IsVisible { get { return _isVisible; } set { _isVisible = value; OnPropertyChanged(nameof(IsVisible)); } }
        public SnackbarMessageQueue MyMessageQueue { get { return _myMessageQueue; } set { _myMessageQueue = value; OnPropertyChanged(nameof(MyMessageQueue)); } }
        public List<int> ListMonth { get => _listMonth; }
        public List<int> ListYear { get => _listYear; set { _listYear = value; OnPropertyChanged(nameof(ListYear)); } }
        public int SelectedMonth { get { return _selectedMonth; } set { _selectedMonth = value; OnPropertyChanged(nameof(SelectedMonth)); } }
        public int SelectedYear { get { return _selectedYear; } set { _selectedYear = value; LoadDataChart(); OnPropertyChanged(nameof(SelectedYear)); } }
        public ObservableCollection<InventoryReportChartModel> DataChart { get { return _dataChart; } set { _dataChart = value; OnPropertyChanged(nameof(DataChart)); } }
        public string SearchKey { get { return _searchKey; } set { _searchKey = value; OnPropertyChanged(nameof(SearchKey)); } }

        public ObservableCollection<DebtReportItem> DataListView { get { return _dataListView; } set { _dataListView = value; OnPropertyChanged(nameof(DataListView)); } }

        public ICommand CLoadData { get; set; }
        public ICommand CSearch { get; set; }
        public ICommand CImportExcel { get; set; }

        public ICommand CExportExcel { get; set; }
        public DebtReportViewModel()
        {
            IsVisible = Visibility.Hidden;

            Title = "Báo Cáo Nợ";
            DataListView = new ObservableCollection<DebtReportItem>();
            ListYear = new List<int>();
            DataChart = new ObservableCollection<InventoryReportChartModel>();

            CLoadData = new RelayCommand<object>((p) => { return true; }, (p) => { LoadDataListView(); });
            CSearch = new RelayCommand<object>((p) => { return true; }, (p) => { SearchCustomer(); });
            CImportExcel = new RelayCommand<object>((p) => { return true; }, (p) => { ImportFileExcel(); });
            CExportExcel = new RelayCommand<object>((p) => { return true; }, (p) => { ExportFileExcel(); });


            MyMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(2500));
            MyMessageQueue.DiscardDuplicates = true;

            LoadDataComboBox();
        }
        public void SearchCustomer()
        {
            if (SearchKey != "" && SearchKey != null)
            {
                List<KHACHHANG> CustomerList = DataProvider.Ins.DB.KHACHHANGs.Where(t => t.HoTen.ToLower().Contains(SearchKey.ToLower())).ToList();
                List<BAOCAOCONGNO> InvReport = DataProvider.Ins.DB.BAOCAOCONGNOes.Where(p => p.Thang == SelectedMonth && p.Nam == SelectedYear).ToList();

                DataListView = GetDataListViewFromDB(InvReport, CustomerList);
            }
            else
            {
                LoadDataListView();
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
        public void LoadDataListView()
        {
            SearchKey = "";
            List<BAOCAOCONGNO> InvReport = DataProvider.Ins.DB.BAOCAOCONGNOes.Where(p => p.Thang == SelectedMonth && p.Nam == SelectedYear).ToList();
            List<KHACHHANG> CustomerList = DataProvider.Ins.DB.KHACHHANGs.ToList();
            if (InvReport.Count != 0)
            {
                DataListView = GetDataListViewFromDB(InvReport, CustomerList);
                MyMessageQueue.Enqueue("Tạo báo cáo thành công!");
                Title = "Báo Cáo Nợ Tháng " + SelectedMonth.ToString() + " Năm " + SelectedYear.ToString();
            }
            else
            {
                MyMessageQueue.Enqueue("Không có thông tin");
                DataListView.Clear();
                Title = "Báo Cáo Nợ";
            }
        }
        public ObservableCollection<DebtReportItem> GetDataListViewFromDB(List<BAOCAOCONGNO> DebtReport, List<KHACHHANG> CustomerList)
        {
            ObservableCollection<DebtReportItem> Data = new ObservableCollection<DebtReportItem>();
            foreach (BAOCAOCONGNO item in DebtReport)
            {
                for (int i = 0; i < CustomerList.Count; i++)
                {
                    if (CustomerList[i].MaKhachHang == item.MaKhachHang)
                    {
                     
                        DebtReportItem debtItem = new DebtReportItem();
                        debtItem.Id = CustomerList[i].MaKhachHang;
                        debtItem.CustomerName = CustomerList[i].HoTen;
                        debtItem.PhoneNumber = CustomerList[i].DienThoai; 
                        debtItem.FirstQuantity = (int)item.TonDau;
                        debtItem.IncurredQuantity = (int)item.PhatSinh;
                        debtItem.EndQuantity = (int)item.TonCuoi;
                        Data.Add(debtItem);
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
                int Quantity = CountBookInMonth(i, SelectedYear);
                InventoryReportChartModel Data = new InventoryReportChartModel(Quantity, i);
                DataChart.Add(Data);
            }
        }
        public int CountBookInMonth(int month, int year)
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
                LoadDataListView();
            }
            catch (Exception ee)
            {
                MyMessageQueue.Enqueue("Lỗi! Không thể nhập liệu từ file Excel");
            }
        }
        public void ExportFileExcel()
        {
            //string filePath = "";
            //System.Windows.Forms.SaveFileDialog dialog = new System.Windows.Forms.SaveFileDialog();
            //dialog.Filter = "Excel files (*.xls or .xlsx)|.xls;*.xlsx";

            //// Nếu mở file và chọn nơi lưu file thành công sẽ lưu đường dẫn lại dùng
            //if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    filePath = dialog.FileName;
            //}

            //// nếu đường dẫn null hoặc rỗng thì báo không hợp lệ và return hàm
            //if (string.IsNullOrEmpty(filePath))
            //{
            //    MyMessageQueue.Enqueue("Lỗi. Đường dẫn không hợp lệ.");
            //    return;
            //}

            //try
            //{
            //    using (ExcelPackage package = new ExcelPackage())
            //    {
            //        package.Workbook.Properties.Author = "Admin";
            //        package.Workbook.Properties.Title = "Danh sách khách hàng";
            //        package.Workbook.Worksheets.Add("Sheet 1");
            //        ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
            //        //add sheet
            //        workSheet.Name = "Sheet 1";
            //        workSheet.Cells.Style.Font.Size = 12;
            //        workSheet.Cells.Style.Font.Name = "Calibri";
            //        // Tạo danh sách các column header
            //        string[] arrColumnHeader = {
            //            "Tháng",
            //            "Năm",
            //            "Mã Sách",
            //            "Tồn Đầu",
            //            "Phát Sinh",
            //            "Tồn Cuối",
            //        };

            //        var countColHeader = arrColumnHeader.Count();

            //        int colIndex = 1;
            //        int rowIndex = 2;

            //        //tạo các header từ column header đã tạo từ bên trên
            //        foreach (var item in arrColumnHeader)
            //        {
            //            var cell = workSheet.Cells[rowIndex, colIndex];

            //            //gán giá trị
            //            cell.Value = item;

            //            colIndex++;
            //        }

            //        foreach (var item in DataListView)
            //        {
            //            colIndex = 1;
            //            rowIndex++;

            //            workSheet.Cells[rowIndex, colIndex++].Value = item.Month;
            //            workSheet.Cells[rowIndex, colIndex++].Value = item.Year;
            //            workSheet.Cells[rowIndex, colIndex++].Value = item.BookId;
            //            workSheet.Cells[rowIndex, colIndex++].Value = item.FirstQuantity;
            //            workSheet.Cells[rowIndex, colIndex++].Value = item.IncurredQuantity;
            //            workSheet.Cells[rowIndex, colIndex++].Value = item.EndQuantity;

            //        }

            //        //Lưu file lại
            //        Byte[] bin = package.GetAsByteArray();
            //        File.WriteAllBytes(filePath, bin);

            //    }
            //    MessageBox.Show("Xuất file thành công");
            //    MyMessageQueue.Enqueue("Xuất excel thành công!");
            //}
            //catch (Exception ee)
            //{
            //    MessageBox.Show("Xuất file không thành công");
            //    MyMessageQueue.Enqueue("Lỗi. Không thể xuất file Excel");
            //}
        }
    }
}

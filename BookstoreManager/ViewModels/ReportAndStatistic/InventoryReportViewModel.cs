using BookstoreManager.Models;
using BookstoreManager.Models.Db;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private List<int> _listYear;
        private List<int> _listMonth = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        private int _selectedYear;
        private int _selectedMonth;
        private string _searchKey;
        private ObservableCollection<InventoryReportChartModel> _dataChart;
        private Visibility _isVisible;

        private SnackbarMessageQueue _myMessageQueue;

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
        public InventoryReportViewModel()
        {
            IsVisible = Visibility.Hidden;

            DataListView = new ObservableCollection<InventoryReportItem>();
            ListYear = new List<int>();
            CLoadData = new RelayCommand<object>((p) => { return true; }, (p) => { LoadDataListView(); });
            CSearch = new RelayCommand<object>((p) => { return true; }, (p) => { SearchBook(); });
            DataChart = new ObservableCollection<InventoryReportChartModel>();

            MyMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(4000));
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
            List<BAOCAOTON> InvReport = DataProvider.Ins.DB.BAOCAOTONs.Where(p => p.Thang == SelectedMonth && p.Nam == SelectedYear).ToList();
            List<SACH> BookList = DataProvider.Ins.DB.SACHes.ToList();
            if (InvReport.Count !=0 )
            {
                DataListView = GetDataListViewFromDB(InvReport, BookList);
                MyMessageQueue.Enqueue("Tạo báo cáo thành công!");
            }
            else
            {
                MyMessageQueue.Enqueue("Lỗi! Không có thông tin");
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
                        InvItem.Id = BookList[i].MaSach;
                        InvItem.BookName = BookList[i].TenSach;
                        InvItem.Type = Type.TenTheLoai;
                        InvItem.FirstQuantity = (int)item.TonDau;
                        InvItem.IncurredQuantity = (int)item.PhatSinh;
                        InvItem.EndQuantity = (int)item.TonCuoi;
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
    }
}

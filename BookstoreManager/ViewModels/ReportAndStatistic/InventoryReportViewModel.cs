using BookstoreManager.Models;
using BookstoreManager.Models.Db;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public List<int> ListMonth { get => _listMonth; }
        public string SearchKey { get { return _searchKey; } set { _searchKey = value; OnPropertyChanged(nameof(SearchKey)); } }

        public int SelectedMonth { get { return _selectedMonth; } set { _selectedMonth = value; OnPropertyChanged(nameof(SelectedMonth)); } }
        public int SelectedYear { get { return _selectedYear; } set { _selectedYear = value; OnPropertyChanged(nameof(SelectedYear)); } }
        public List<int> ListYear { get => _listYear; set { _listYear = value; OnPropertyChanged(nameof(ListYear)); } }
        public ObservableCollection<InventoryReportItem> DataListView { get { return _dataListView; } set { _dataListView = value; OnPropertyChanged(nameof(DataListView)); } }

        public ICommand CLoadData { get; set; }
        public ICommand CSearch { get; set; }
        public InventoryReportViewModel()
        {
            DataListView = new ObservableCollection<InventoryReportItem>();
            ListYear = new List<int>();
            CLoadData = new RelayCommand<object>((p) => { return true; }, (p) => { LoadDataListView(); });
            CSearch = new RelayCommand<object>((p) => { return true; }, (p) => { SearchBook(); });

            LoadData();
        }
        public void LoadData()
        {
            LoadDataComboBox();
            LoadDataListView();
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
            if (InvReport != null)
            {
                DataListView = GetDataListViewFromDB(InvReport, BookList);
            }
            else
            {

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
                        InventoryReportItem InvItem = new InventoryReportItem();
                        InvItem.Id = BookList[i].MaSach;
                        InvItem.BookName = BookList[i].TenSach;
                        InvItem.FirstQuantity = (int)item.TonDau;
                        InvItem.IncurredQuantity = (int)item.PhatSinh;
                        InvItem.EndQuantity = (int)item.TonCuoi;
                        Data.Add(InvItem);
                    }
                }
                
            }
            return Data;
        }
    }
}

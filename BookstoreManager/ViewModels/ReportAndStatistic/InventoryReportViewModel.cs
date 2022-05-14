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
        public List<int> ListMonth { get => _listMonth; }
        public int SelectedMonth { get { return _selectedMonth; } set { _selectedMonth = value; OnPropertyChanged(nameof(SelectedMonth)); } }
        public int SelectedYear { get { return _selectedYear; } set { _selectedYear = value; OnPropertyChanged(nameof(SelectedYear)); } }
        public List<int> ListYear { get => _listYear; set { _listYear = value; OnPropertyChanged(nameof(ListYear)); } }
        public ObservableCollection<InventoryReportItem> DataListView { get { return _dataListView; } set { _dataListView = value; OnPropertyChanged(nameof(DataListView)); } }

        public ICommand CLoadData { get; set; }
        public InventoryReportViewModel()
        {
            DataListView = new ObservableCollection<InventoryReportItem>();
            ListYear = new List<int>();
            CLoadData = new RelayCommand<object>((p) => { return true; }, (p) => { LoadDataListView(); });
            LoadData();
        }
        public void LoadData()
        {
            LoadDataComboBox();
            LoadDataListView();
        }
        public void LoadDataComboBox()
        {
            List<BAOCAOTON> listBaoCaoTon = DataProvider.Ins.DB.BAOCAOTONs.ToList();
            foreach (BAOCAOTON item in listBaoCaoTon)
            {
                if(!ListYear.Contains(item.Nam))
                {
                    ListYear.Add(item.Nam);
                }
            }
        }
        public void LoadDataListView()
        {
                BAOCAOTON InvReport = DataProvider.Ins.DB.BAOCAOTONs.Where(p => p.Thang == SelectedMonth && p.Nam == SelectedYear).FirstOrDefault();
                if (InvReport != null)
                { 
                    DataListView = GetDataListViewFromDB(InvReport);
                }
        }
        public ObservableCollection<InventoryReportItem> GetDataListViewFromDB(BAOCAOTON InvReport)
        {
            ObservableCollection<InventoryReportItem> Data = new ObservableCollection<InventoryReportItem>();
            List<SACH> BookList = DataProvider.Ins.DB.SACHes.ToList();
            foreach (SACH book in BookList)
            {
                InventoryReportItem Item = new InventoryReportItem();
                THELOAI Type = DataProvider.Ins.DB.THELOAIs.Where(p => p.MaTheLoai == book.MaTheLoai).FirstOrDefault();
                Item.Type = Type.TenTheLoai; 
                Item.Id = book.MaSach;
                Item.FirstQuantity = (int)InvReport.TonDau;
                Item.IncurredQuantity = (int)InvReport.PhatSinh;
                Item.EndQuantity = (int)InvReport.TonCuoi;
                Item.PublishingHouse = book.NhaXuatBan;
                Item.PublishYear = (int)book.NamXuatBan;
            }
            return Data;
        }
    }
}

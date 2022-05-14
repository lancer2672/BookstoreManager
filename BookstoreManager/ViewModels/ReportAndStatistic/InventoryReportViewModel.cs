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
        private int _selectedYear;
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
            List<SACH> BookList = DataProvider.Ins.DB.SACHes.ToList();
            DataListView = GetDataListViewFromDB(BookList);
        }
        public ObservableCollection<InventoryReportItem> GetDataListViewFromDB(List<SACH> BookList)
        {
            ObservableCollection<InventoryReportItem> Data = new ObservableCollection<InventoryReportItem>();
            foreach (SACH book in BookList)
            {
                InventoryReportItem Item = new InventoryReportItem();
                THELOAI Type = DataProvider.Ins.DB.THELOAIs.Where(p => p.MaTheLoai == book.MaTheLoai).FirstOrDefault();
                if (Type == null)
                {
                    Item.Type = "";
                }
                else
                { 
                    Item.Type = Type.TenTheLoai; 
                }
                Item.Id = book.MaSach;
                Item.Quantity = (int)book.SoLuongTon;
                Item.PublishingHouse = book.NhaXuatBan;
                Item.PublishYear = (int)book.NamXuatBan;
            }
            return Data;
        }
    }
}

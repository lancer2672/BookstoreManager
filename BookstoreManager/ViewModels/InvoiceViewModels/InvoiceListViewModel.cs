using BookstoreManager.Models;
using BookstoreManager.Models.Db;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManager.ViewModels.InvoiceViewModels
{
    public class InvoiceListViewModel:BaseViewModel
    {
        private DateTime _selectedDate;
        private ViewInvoice _selectedInvoice;
        private ObservableCollection<ViewInvoice> _listInvoice;
        private ObservableCollection<ViewInvoiceDetail> _listDetailInvoice;
        public DateTime SelectedDate { get => _selectedDate; set { _selectedDate = value; OnPropertyChanged(nameof(SelectedDate)); LoadDataListView(); } }
        public ViewInvoice SelectedInvoice { get => _selectedInvoice; set { _selectedInvoice = value; OnPropertyChanged(nameof(SelectedInvoice)); LoadListDetailInvoice(); } }
        public ObservableCollection<ViewInvoice> ListInvoice { get => _listInvoice; set { _listInvoice = value; OnPropertyChanged(nameof(ListInvoice)); } }
        public ObservableCollection<ViewInvoiceDetail> ListDetailInvoice { get => _listDetailInvoice; set { _listDetailInvoice = value; OnPropertyChanged(nameof(ListDetailInvoice)); } }

        public InvoiceListViewModel()
        {
            ListInvoice = new ObservableCollection<ViewInvoice>();
            ListDetailInvoice = new ObservableCollection<ViewInvoiceDetail>();
            SelectedDate = DateTime.Now;
        }
        public void LoadDataListView()
        {
            LoadListInvoice();
         
        }
        public void LoadListInvoice()
        {
            ListInvoice.Clear();
            List<HOADON> list = DataProvider.Ins.DB.HOADONs.Where(t => t.NgayLap == SelectedDate).ToList();
            foreach (HOADON item in list)
            {
                KHACHHANG customer = DataProvider.Ins.DB.KHACHHANGs.Where(t => t.MaKhachHang == item.MaKhachHang).FirstOrDefault();
                ViewInvoice newItem = new ViewInvoice();
                newItem.InvoiceId = item.MaHoaDon;
                newItem.CustomerId = (long)item.MaKhachHang;
                newItem.CustomerPaid = (decimal)item.SoTienTra;
                newItem.Rest = (decimal)item.SoTienConLai;
                newItem.CustomerName = customer.HoTen;
                newItem.Total = (decimal)item.TongTien;
                ListInvoice.Add(newItem);
            }
        }
        public void LoadListDetailInvoice()
        {
            if(SelectedInvoice == null)
            {
                ListDetailInvoice.Clear();
            }
            else
            {
                List<CHITIETHOADON> list = DataProvider.Ins.DB.CHITIETHOADONs.Where(t => t.MaHoaDon == SelectedInvoice.InvoiceId).ToList();
                foreach (CHITIETHOADON item in list)
                {
                    SACH book = DataProvider.Ins.DB.SACHes.Where(t => t.MaSach == item.MaSach).FirstOrDefault();
                    ViewInvoiceDetail newItem = new ViewInvoiceDetail();
                    newItem.InvoiceId = SelectedInvoice.InvoiceId;
                    newItem.InvoiceDetailId = item.MaCTHD;
                    newItem.BookId = (int)item.MaSach;
                    newItem.Number = (int)item.SoLuong;
                    newItem.BookName = book.TenSach;
                    newItem.Price = (decimal)item.DonGia;
                    newItem.Total = (decimal)item.ThanhTien;
                    ListDetailInvoice.Add(newItem);
                }
            }
        }
    }
   
}

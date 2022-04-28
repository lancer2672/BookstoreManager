using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using BookstoreManager.Models;
using BookstoreManager.Models.Db;
using BookstoreManager.ViewModels.Customers;

namespace BookstoreManager.ViewModels
{
    public class ManageCustomerViewModel : BaseViewModel
    {
        private ObservableCollection<ViewCustomer> _listCustomer;
        private ViewCustomer _selectedCustomer;
        
        public ObservableCollection<ViewCustomer> ListCustomer { get => _listCustomer; set { _listCustomer = value; OnPropertyChanged(nameof(ListCustomer)); } }

        public ViewCustomer SelectedCustomer { get => _selectedCustomer; set { _selectedCustomer = value; OnPropertyChanged(nameof(SelectedCustomer)); } }
        public ICommand COpenAddCustomerWindow { get; set; }
        public ICommand CDeleteCustomer { get; set; }
        public ICommand CUpdateCustomer { get; set; }
        public ManageCustomerViewModel()
        {
            ListCustomer = new ObservableCollection<ViewCustomer>();

            COpenAddCustomerWindow = new RelayCommand<object>((p) => { return true; }, (p) => { OpenAddCustomerWindow(); });
            CDeleteCustomer = new RelayCommand<ListView>((p) => { return true; }, (p) => { DeleteCustomer(p); });

            LoadListCustomer();
        }

        public void LoadListCustomer()
        {
            List<KHACHHANG> listKHACHHANG = DataProvider.Ins.DB.KHACHHANGs.ToList();
            ListCustomer = GetViewCustomerFromList(listKHACHHANG);
        }
        public ObservableCollection<ViewCustomer> GetViewCustomerFromList(List<KHACHHANG> listKHACHHANG)
        {
            ObservableCollection<ViewCustomer> list = new ObservableCollection<ViewCustomer>();
            int Count = DataProvider.Ins.DB.KHACHHANGs.Count();
            for (int i = 0; i < Count; i++)
            {
                ViewCustomer newCustomer = new ViewCustomer();
                newCustomer.Id = listKHACHHANG[i].MaKhachHang;
                newCustomer.Name = listKHACHHANG[i].HoTen;
                newCustomer.Adress = listKHACHHANG[i].DiaChi;
                newCustomer.Email = listKHACHHANG[i].Email;
                newCustomer.PhoneNumber = listKHACHHANG[i].DienThoai;
                list.Add(newCustomer);
            }
            return list;
        }
        public void OpenAddCustomerWindow()
        {
            AddCustomerWindow AddWindow = new AddCustomerWindow(this);
            AddWindow.Show();
        }
        public void DeleteCustomer(ListView lv)
        {
            System.Collections.IList list = lv.SelectedItems;
            for (int i = 0; i < list.Count; i++)
            {
                int id = (list[i] as ViewCustomer).Id;
                KHACHHANG deletedCustomer = DataProvider.Ins.DB.KHACHHANGs.Where(p => p.MaKhachHang == id).First<KHACHHANG>();
                if (deletedCustomer == null)
                {
                    continue;
                }
                else
                {
                    DataProvider.Ins.DB.KHACHHANGs.Remove(deletedCustomer);
                    DataProvider.Ins.DB.SaveChanges();
                }
            }
            LoadListCustomer();
        }
    }
}

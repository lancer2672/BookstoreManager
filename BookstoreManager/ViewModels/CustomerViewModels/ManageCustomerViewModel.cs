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
using BookstoreManager.Resources;
using BookstoreManager.ViewModels.Customers;
using BookstoreManager.Views.Customers;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using OfficeOpenXml;

namespace BookstoreManager.ViewModels
{
    public class ManageCustomerViewModel : BaseViewModel
    {
        private ObservableCollection<ViewCustomer> _listCustomer;
        private ViewCustomer _selectedCustomer;
        private string _searchKey;

        private SnackbarMessageQueue _myMessageQueue;

        public SnackbarMessageQueue MyMessageQueue { get { return _myMessageQueue; } set { _myMessageQueue = value; OnPropertyChanged(nameof(MyMessageQueue)); } }
        public string SearchKey { get { return _searchKey; } set { _searchKey = value; OnPropertyChanged(nameof(SearchKey)); } }

        public ObservableCollection<ViewCustomer> ListCustomer { get => _listCustomer; set { _listCustomer = value; OnPropertyChanged(nameof(ListCustomer)); } }

        public ViewCustomer SelectedCustomer { get => _selectedCustomer; set { _selectedCustomer = value; OnPropertyChanged(nameof(SelectedCustomer)); } }
        public ICommand COpenAddCustomerWindow { get; set; }
        public ICommand CImportExcel { get; set; }
        public ICommand CExportExcel { get; set; }

        public ICommand CDeleteCustomer { get; set; }
        public ICommand COpenUpdateCustomerWindow { get; set; }
        public ICommand CSearch { get; set; }
        public ManageCustomerViewModel()
        {
            ListCustomer = new ObservableCollection<ViewCustomer>();

            COpenAddCustomerWindow = new RelayCommand<object>((p) => { return true; }, (p) => { OpenAddCustomerWindow(); });
            CDeleteCustomer = new RelayCommand<ListView>((p) => { return true; }, (p) => { DeleteCustomer(p); });
            COpenUpdateCustomerWindow = new RelayCommand<ListView>((p) => { return true; }, (p) => { OpenUpdateWindow(p); });
            CSearch = new RelayCommand<ListView>((p) => { return true; }, (p) => { SearchCustomer(); });
            CImportExcel = new RelayCommand<ListView>((p) => { return true; }, (p) => { ImportFileExcel(); });
            CExportExcel = new RelayCommand<ListView>((p) => { return true; }, (p) => { ExportFileExcel(); });

            MyMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(4000));
            MyMessageQueue.DiscardDuplicates = true;

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
            int Count = listKHACHHANG.Count();
            for (int i = 0; i < Count; i++)
            {
                ViewCustomer newCustomer = new ViewCustomer();
                newCustomer.Id = listKHACHHANG[i].MaKhachHang;
                newCustomer.Name = listKHACHHANG[i].HoTen;
                newCustomer.Address = listKHACHHANG[i].DiaChi;
                newCustomer.Email = listKHACHHANG[i].Email;
                newCustomer.PhoneNumber = listKHACHHANG[i].DienThoai;
                newCustomer.Debt = (decimal)listKHACHHANG[i].TongNo;
                list.Add(newCustomer);
            }
            return list;
        }
        public void OpenAddCustomerWindow()
        {
            AddCustomerWindow AddWindow = new AddCustomerWindow(this);
            AddWindow.ShowDialog();
        }
        public bool CanDeleted(ListView lv)
        {
            bool Delete = false;
            System.Collections.IList list = lv.SelectedItems;
            if(list.Count > 0)
            {
                bool? dialogResult = new CustomMessageBox("Bạn có muốn xóa khách hàng này", MessageType.Info, "Thông Báo", MessageButtons.OkCancel).ShowDialog();
                if (dialogResult == true)
                {
                    Delete = true;
                }
            }
            return Delete;
        }
        public void DeleteCustomer(ListView lv)
        {
            if(CanDeleted(lv))
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
                        try
                        {

                            DataProvider.Ins.DB.KHACHHANGs.Remove(deletedCustomer);
                            DataProvider.Ins.DB.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            MyMessageQueue.Enqueue("Lỗi! Xóa khách hàng không thành công!");
                            return;
                        }
                    }
                }
                LoadListCustomer();
                MyMessageQueue.Enqueue("Xóa khách hàng thành công!");
            }
        }
        public void OpenUpdateWindow(ListView lv)
        {
            System.Collections.IList list = lv.SelectedItems;
            if (list.Count != 1)
            {
                return;
            }
            UpdateCustomerWindow updWindow = new UpdateCustomerWindow(this);
            updWindow.ShowDialog();
        }
        public void SearchCustomer()
        {
            if (SearchKey != "" && SearchKey != null)
            {
                List<KHACHHANG> listKH = DataProvider.Ins.DB.KHACHHANGs.Where(t => t.HoTen.ToLower().Contains(SearchKey.ToLower())).ToList();
                ListCustomer = GetViewCustomerFromList(listKH);
            }
            else
            {
                LoadListCustomer();
            }
        }
        public void ImportFileExcel()
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = dialog.FileName;
                AddImportedCustomer(fileName);
            }
        }
        public void AddImportedCustomer(string filename)
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
                        KHACHHANG newCustomer = new KHACHHANG()
                        {
                            MaKhachHang = Convert.ToInt32(workSheet.Cells[i, j++].Value),
                            HoTen = workSheet.Cells[i, j++].Value.ToString(),
                            DienThoai = workSheet.Cells[i, j++].Value.ToString(),
                            DiaChi = workSheet.Cells[i, j++].Value.ToString(),
                            Email = workSheet.Cells[i, j++].Value.ToString(),
                            TongNo = Convert.ToInt32(workSheet.Cells[i, j++].Value)
                            
                         
                        };
                      
                            DataProvider.Ins.DB.KHACHHANGs.Add(newCustomer);
                            DataProvider.Ins.DB.SaveChanges();

                        MyMessageQueue.Enqueue("thêm dữ liệu từ file excel thành công!");

                    }
                    catch (Exception error)
                    {
                        MyMessageQueue.Enqueue("Lỗi! Không thể nhập liệu từ file excel");

                    }
                }
                LoadListCustomer();
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
                        "Mã khách hàng",
                        "Họ và tên",
                        "Số điện thoại",
                        "Địa chỉ",
                        "Email",
                        "Tổng nợ",
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

                    foreach (var item in ListCustomer)
                    {
                        colIndex = 1;
                        rowIndex++;

                        workSheet.Cells[rowIndex, colIndex++].Value = item.Id;
                        workSheet.Cells[rowIndex, colIndex++].Value = item.Name;
                        workSheet.Cells[rowIndex, colIndex++].Value = item.PhoneNumber;
                        workSheet.Cells[rowIndex, colIndex++].Value = item.Address;
                        workSheet.Cells[rowIndex, colIndex++].Value = item.Email;
                        workSheet.Cells[rowIndex, colIndex++].Value = item.Debt;

                    }

                    //Lưu file lại
                    Byte[] bin = package.GetAsByteArray();
                    File.WriteAllBytes(filePath, bin);

                }
                MessageBox.Show("Xuat file thanh cong");
                MyMessageQueue.Enqueue("Xuất excel thành công!");
            }
            catch (Exception ee)
            {
                MessageBox.Show("Xuat file khong thanh cong");
                MyMessageQueue.Enqueue("Lỗi. Không thể xuất file Excel");
            }
        }
    }
}

using BookstoreManager.Models;
using BookstoreManager.Views.Account;
using MaterialDesignThemes.Wpf;
using BookstoreManager.Resources.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace BookstoreManager.ViewModels.AccountViewModels
{
    public class AccountViewModel : BaseViewModel
    {
        private int _id;
        private string _name;
        private string _address;
        private string _gmail;
        private string _phone;
        private string _source;
        private string _password;
        private string _newPassword;
        private string _rePassword;
        public SnackbarMessageQueue MyMessageQueue { get => myMessageQueue; set { myMessageQueue = value; OnPropertyChanged(nameof(MyMessageQueue)); } }
        private SnackbarMessageQueue myMessageQueue;
        public int Id { get => _id; set => _id = value; }
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        public string Address { get => _address; set { _address = value; OnPropertyChanged(); } }
        public string Gmail { get => _gmail; set { _gmail = value; OnPropertyChanged(); } }
        public string Phone { get => _phone; set { _phone = value; OnPropertyChanged(); } }
        public string Source { get => _source; set { _source = value; OnPropertyChanged(); } }
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }
        public string NewPassword { get => _newPassword; set { _newPassword = value; OnPropertyChanged(); } }
        public string RePassword { get => _rePassword; set { _rePassword = value; OnPropertyChanged(); } }
        public ICommand OpenAccountChange { get; set; }
        public ICommand SaveAccountCommand { get; set; }
        public ICommand SavePictureCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public ICommand NewPasswordChangedCommand { get; set; }
        public ICommand RePasswordChangedCommand { get; set; }
        public ICommand SavePasswordCommand { get; set; }


        public AccountViewModel()
        {
            var select = from s in DataProvider.Ins.DB.TAIKHOANs select s;
            foreach (var data in select)
            {
                Id = 1;
                Name = data.HoTen;
                Address = data.DiaChi;
                Gmail = data.Gmail;
                Phone = data.SoDienThoai;
                Source = data.Avatar;
            }

            OpenAccountChange = new RelayCommand<object>((p) => { return true; }, (p) => { openAccountChangeWindow(p); });
            SaveAccountCommand = new RelayCommand<StackPanel>((p) => { return true; }, (p) => { SaveAccount(p); });
            SavePictureCommand = new RelayCommand<object>((p) => { return true; }, (p) => { SavePicture(p); });
            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { Password = p.Password; });
            NewPasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { NewPassword = p.Password; });
            RePasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { RePassword = p.Password; });
            SavePasswordCommand = new RelayCommand<object>((p) => { return true; }, (p) => { SavePassword(p); });

            MyMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(2000));
            MyMessageQueue.DiscardDuplicates = true;
        }
        void openAccountChangeWindow(object p)
        {
            var window = new AccountChange();
            window.ShowDialog();
        }
        void SaveAccount(StackPanel infoChangeForm)
        {
            if (Resources.Utils.Validator.IsValid(infoChangeForm))
            {
                var change = DataProvider.Ins.DB.TAIKHOANs.SingleOrDefault(x => x.MaTaiKhoan == 1);
                change.HoTen = Name;
                change.DiaChi = Address;
                change.Gmail = Gmail;
                change.SoDienThoai = Phone;
                DataProvider.Ins.DB.SaveChanges();
                MyMessageQueue.Enqueue("Thay đổi thông tin thành công!");
            }

        }
        void SavePicture(object p)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedFileName = ofd.FileName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFileName);
                bitmap.EndInit();
                Source = bitmap.ToString();

                //Lưu hình ảnh vào database
                var imageChange = DataProvider.Ins.DB.TAIKHOANs.SingleOrDefault(x => x.MaTaiKhoan == Id);
                imageChange.Avatar = Source;
                DataProvider.Ins.DB.SaveChanges();
            }
        }
        void SavePassword(object p)
        {
            var choose = DataProvider.Ins.DB.TAIKHOANs.SingleOrDefault(x => x.TenDangNhap == "admin");
            if (string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(RePassword))
            {
                MyMessageQueue.Enqueue("Lỗi. Chưa điền đầy đủ thông tin");
            }
            else
            {
                if (choose.MatKhau == Password)
                {
                    if (NewPassword == RePassword)
                    {
                        var change = DataProvider.Ins.DB.TAIKHOANs.SingleOrDefault(x => x.MatKhau == Password);
                        change.MatKhau = NewPassword;
                        DataProvider.Ins.DB.SaveChanges();
                        MyMessageQueue.Enqueue("Thay đổi mật khẩu thành công");
                    }
                    else
                    {
                        MyMessageQueue.Enqueue("Lỗi. Mật khẩu và xác nhận mật khẩu không trùng khớp");
                    }

                }
                else
                {
                    MyMessageQueue.Enqueue("Lỗi. Mật khẩu cũ không đúng");
                }
            }
        }
    }
}

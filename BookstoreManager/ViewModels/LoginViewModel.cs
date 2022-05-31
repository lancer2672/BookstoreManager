using BookstoreManager.Models;
using BookstoreManager.Resources;
using BookstoreManager.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookstoreManager.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public bool IsLogin { get; set; }
        private string _username;
        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }
        private string _password;


        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }

        public ICommand CloseCommand { get; set; }
        public ICommand LoginCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public object DataProvier { get; private set; }

        public LoginViewModel()
        {
            IsLogin = false;
            Password = "";
            Username = "";


            LoginCommand = new RelayCommand<object>((p) => { return true; }, (p) => { Login(p); });
            PasswordChangedCommand = new RelayCommand<PasswordBox>((p) => { return true; }, (p) => { Password = p.Password; });
        }
        void Login(object p)
        {
            IsLogin = false;
            if (p == null)
                return;
            var accCount = DataProvider.Ins.DB.TAIKHOANs.Where(x => x.TenDangNhap == Username && x.MatKhau == Password).Count();
            if (accCount > 0)
            {
                IsLogin = true;
                AdminWindow adminWindow = new AdminWindow();
                adminWindow.ShowDialog();

            }
            else
            {
                bool? result = new CustomMessageBox("Sai tài khoản hoặc mật khẩu", MessageType.Info, "Thông Báo", MessageButtons.OkCancel).ShowDialog();
            }           
        }
    }
}

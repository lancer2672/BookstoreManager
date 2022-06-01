using BookstoreManager.Models;
using BookstoreManager.Resources;
using BookstoreManager.Views;
using MaterialDesignThemes.Wpf;
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

        public SnackbarMessageQueue MyMessageQueue { get => myMessageQueue; set { myMessageQueue = value; OnPropertyChanged(nameof(MyMessageQueue)); } }
        private SnackbarMessageQueue myMessageQueue;
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }

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
            
            MyMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(2000));
            MyMessageQueue.DiscardDuplicates = true;
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
                Username = "";
                Password = "";

            }
            else
            {
                MyMessageQueue.Enqueue("Sai tài khoản hoặc mật khẩu");

            }           
        }

        void Close()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Close();
        }
    }
}

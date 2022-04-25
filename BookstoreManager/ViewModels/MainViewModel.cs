using BookstoreManager.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BookstoreManager.ViewModels
{
    public class MainViewModel:BaseViewModel
    {
        public bool IsLoaded = false;
        public ICommand LoadedWidnowCommand { get; set; }
        public MainViewModel()
        {
            LoadedWidnowCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                IsLoaded = true;
                p.Hide();
                AdminWindow admWindow = new AdminWindow();
                admWindow.ShowDialog();
            });

        }
    }
}

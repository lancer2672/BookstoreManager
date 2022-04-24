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
        public bool IsLoaded;
        public ICommand LoadedWidnowCommand { get; set; }
        public MainViewModel()
        {
            IsLoaded = false;
        }
    }
}

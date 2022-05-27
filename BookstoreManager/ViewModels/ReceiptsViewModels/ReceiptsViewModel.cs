using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookstoreManager.ViewModels.ReceiptsViewModels
{
    public class ReceiptsViewModel : BaseViewModel
    {
        private string _customerName;
        private string _customerPhoneNumber;
        private decimal _customerPaid;
        private DateTime _date;
        private DateTime _selectedDay;

        public string CustomerName { get => _customerName; set { _customerName = value; OnPropertyChanged(nameof(CustomerName)); } }
        public string CustomerPhoneNumber { get => _customerPhoneNumber; set { _customerPhoneNumber = value; OnPropertyChanged(nameof(_customerPhoneNumber)); } }
        public DateTime Date { get => _date; set { _date = value; OnPropertyChanged(nameof(Date)); } }
        public DateTime SelectedDay { get => _selectedDay; set { _selectedDay = value; OnPropertyChanged(nameof(SelectedDay)); } }
        public decimal CustomerPaid { get => _customerPaid; set { _customerPaid = value; OnPropertyChanged(nameof(CustomerPaid)); } }
        public ICommand SaveReceipts { get; set; }
        public SnackbarMessageQueue MyMessageQueue { get => myMessageQueue; set { myMessageQueue = value; OnPropertyChanged(nameof(MyMessageQueue)); } }
        private SnackbarMessageQueue myMessageQueue;

        public ReceiptsViewModel()
        {
            Date = DateTime.Now;
            SelectedDay = DateTime.Now;
            
            MyMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(2000));
            MyMessageQueue.DiscardDuplicates = true;

        }

    }
}

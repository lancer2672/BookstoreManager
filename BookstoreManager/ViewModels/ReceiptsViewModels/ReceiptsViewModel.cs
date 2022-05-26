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
        private int _receiptsId;
        private int _customerId;
        private DateTime _date;
        private decimal _receiptsMoney;
        public SnackbarMessageQueue MyMessageQueue { get => myMessageQueue; set { myMessageQueue = value; OnPropertyChanged(nameof(MyMessageQueue)); } }
        private SnackbarMessageQueue myMessageQueue;
        public int ReceiptsId { get => _receiptsId; set { _receiptsId = value; OnPropertyChanged(); } }
        public int CustomerId { get => _customerId; set { _customerId = value; OnPropertyChanged(); } }
        public DateTime Date { get => _date; set { _date = value; OnPropertyChanged(); } }
        public decimal ReceiptsMoney{ get => _receiptsMoney; set { _receiptsMoney = value; OnPropertyChanged(); } }
        public ICommand SaveReceipts { get; set; }
        public ReceiptsViewModel()
        {

        }

    }
}

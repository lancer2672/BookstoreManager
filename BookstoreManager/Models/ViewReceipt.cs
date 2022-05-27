using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManager.Models
{
    public class ViewReceipt
    {
        public int ReceiptId { get; set; }
        public DateTime Date { get; set; }
        public decimal CustomerPaid { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerName { get; set; }

    }
}

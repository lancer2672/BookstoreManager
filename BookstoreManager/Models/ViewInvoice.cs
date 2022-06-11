using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManager.Models
{
    public class ViewInvoice
    {
        public long CustomerId { get; set; }
        public int InvoiceId { get; set; }
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public decimal CustomerPaid { get; set; }
        public decimal Rest { get; set; }
        public string CustomerName { get; set; }

    }
    public class ViewInvoiceDetail
    {
        public int InvoiceDetailId { get; set; }
        public int InvoiceId { get; set; }

        public int BookId { get; set; }
        public string BookName { get; set; }
        public int Number { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}

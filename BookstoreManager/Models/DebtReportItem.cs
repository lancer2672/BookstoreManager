using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManager.Models
{
    public class DebtReportItem
    {
        public long CustomerId { get; set; }
        public int ReportId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { set; get; }
        public int FirstQuantity { get; set; }
        public int IncurredQuantity { get; set; }
        public int EndQuantity { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}

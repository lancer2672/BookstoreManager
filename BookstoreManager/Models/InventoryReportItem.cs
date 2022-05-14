using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManager.Models
{
    public class InventoryReportItem
    {
        public int Id { get; set; }
        public string BookName { get; set; }

        public int FirstQuantity { get; set; }
        public int IncurredQuantity { get; set; }
        public int EndQuantity { get; set; }
      

    }
}

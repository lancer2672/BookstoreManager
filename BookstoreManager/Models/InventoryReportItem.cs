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
        public string Type { get; set; }
        public string PublishingHouse { get; set; }
        public int PublishYear { get; set; }
        public int Quantity { get; set; }

    }
}

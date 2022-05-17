using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManager.Models
{
    public class InventoryReportChartModel
    {
        public int Quantity { get; set; }
        public string Month { get; set; }
        public InventoryReportChartModel(int X, int Y)
        {
            Quantity = X;
            Month = "Tháng " + Y.ToString();
        }
    }
}

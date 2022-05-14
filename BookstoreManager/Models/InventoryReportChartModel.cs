using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManager.Models
{
    public class InventoryReportChartModel
    {
        public int Value { get; set; }
        public string Month { get; set; }
        public InventoryReportChartModel(int X, int Y)
        {
            Value = X;
            Month = "Tháng " + Y.ToString();
        }
    }
}

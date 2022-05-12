using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManager.Resources.Utils
{
    static public class MoneyConverter
    {
        static public string convertMoney(string money)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
            string a = double.Parse(money).ToString("#,###", cul.NumberFormat);
            return a;
        }
    }
}

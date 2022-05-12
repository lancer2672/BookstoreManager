using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BookstoreManager.Resources.Utils
{
    public class UIMoneyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN"); // try with "en-US"
            string a = double.Parse(value.ToString()).ToString("#,###", cul.NumberFormat);
            return a;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            decimal result;
            bool convt = decimal.TryParse(value.ToString(), NumberStyles.Currency,
              cul.NumberFormat, out result);
            if (convt)
                return result;
            else return null;
        }
    }
}

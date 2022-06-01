using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BookstoreManager.Resources.Utils
{
    public class NonEmptyRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value.ToString() == "") 
                return new ValidationResult(false, "Không được để trống");
            return ValidationResult.ValidResult;
        }
    }
    public class NameRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {

            if (value.ToString().Any(char.IsDigit) == true)
                return new ValidationResult(false, "Tên không hợp lệ");
            return ValidationResult.ValidResult;
        }
    }
    public class MoneyRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            decimal check;
            string[] moneyArr = value.ToString().Split('.');
            string salary = "";
            for (int i = 0; i < moneyArr.Length; i++)
            {
                salary += moneyArr[i];
            }
            if(salary == "")
            {
                salary = "0";
            }
            if (decimal.TryParse(salary, out check) == true)
            {
                if (check < 0) return new ValidationResult(false, "Số tiền không được nhỏ hơn 0");
                return ValidationResult.ValidResult;
            }
            else
            {
                return new ValidationResult(false, "Số tiền không hợp lệ");
            }
        }
    }
    public class PhoneNumberRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string phonenumber = value.ToString();
            if (phonenumber.Length != 10 || phonenumber.All(char.IsDigit) == false || phonenumber[0] != '0')
            {
                return new ValidationResult(false, "Số điện thoại không hợp lệ");
            }
            return ValidationResult.ValidResult;
        }
    }
    public class EmailRule : ValidationRule
    {
        private static readonly Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value.ToString() == "")
            {
                return ValidationResult.ValidResult;
            }
            if (!regex.IsMatch(value.ToString())) return new ValidationResult(false, "Email không hợp lệ");
            return ValidationResult.ValidResult;
        }
    }
    public class PositiveNumberRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                decimal val = Convert.ToDecimal(value);
                if (val < 0) return new ValidationResult(false, "Giá trị phải lớn hơn 0");
                return ValidationResult.ValidResult;
            }
            catch
            {
                return new ValidationResult(false, "Dữ liệu phải là kiểu số");
            }
        }
    }
}

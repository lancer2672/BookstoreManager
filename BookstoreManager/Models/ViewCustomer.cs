using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManager.Models
{
    public class ViewCustomer
    {
        public int Id { get; set; }
        public string Name { get;set; }
        public string Adress    { get;set; }
        public string PhoneNumber   { get;set; }
        public string Email { get; set; }
        
        public decimal Debt { get; set; } 

    }
}

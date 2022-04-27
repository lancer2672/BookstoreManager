using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BookstoreManager.Views
{
    static class Pages
    {
        public static Page ManageCustomerPagea { get => ListPages[1]; }

        public static List<Page> ListPages = new List<Page>();
        public static Page ManageCustomerPage { get => ListPages[0]; }
        static Pages()
        {
            ListPages.Add(new ManageCustomerPage());
        }
    }
}

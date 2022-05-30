using BookstoreManager.Views.Account;
using BookstoreManager.Views.BookViews;
using BookstoreManager.Views.Regulation;
using BookstoreManager.Views.Report;
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

        public static List<Page> ListPages = new List<Page>();
        public static Page ManageCustomerPage { get => ListPages[0]; }
        public static Page BookListPage { get => ListPages[1]; }
        public static Page BookTypePage { get => ListPages[2]; }
        public static Page DebtReportPage { get => ListPages[3]; }

        public static Page InventoryReportPage { get => ListPages[4]; }

        public static Page RegulationPage { get => ListPages[5]; }
        public static Page AccountPage { get => ListPages[6]; }
        public static Page EntryBookPage { get => ListPages[7]; }

        static Pages()
        {
            ListPages.Add(new ManageCustomerPage());
            ListPages.Add(new BookListPage());
            ListPages.Add(new BookTypePage());
            ListPages.Add(new DebtReportPage());
            ListPages.Add(new InventoryReportPage());
            ListPages.Add(new RegulationPage());
            ListPages.Add(new AccountMain());
            ListPages.Add(new EntryBookPage());
        }
    }
}

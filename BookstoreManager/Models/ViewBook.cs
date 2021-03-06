using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreManager.Models
{
    public class ViewBook
    {
        public int Id { get; set; }
        public string TitleBook { get; set; }
        public string Category { get; set; }
        public string NameAuthor { get; set; }
        public string PublishCompany { get; set; }
        public int PublishYear { get; set; }
        public int InventoryNumber { get; set; }
        public decimal Price { get; set; }
    }
    public class ViewSellBook
    {
        public int Id { get; set; }
        public string TitleBook { get; set; }
        public string Category { get; set; }
        public string NameAuthor { get; set; }
        public string PublishCompany { get; set; }
        public int PublishYear { get; set; }
        public int Number { get; set; }
        public decimal SellPrice { get; set; }
    }
    public class ViewEntryBook
    {
        public int Id { get; set; }
        public string TitleBook { get; set; }
        public string Category { get; set; }
        public string NameAuthor { get; set; }
        public string PublishCompany { get; set; }
        public int PublishYear { get; set; }
        public int InventoryNumber { get; set; }
        public int EntryNumber { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

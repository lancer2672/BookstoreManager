using BookstoreManager.Models;
using BookstoreManager.Resources.Utils;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookstoreManager.ViewModels.RegulationViewModels
{
    public class RegulationViewModel : BaseViewModel
    {
        private int _id;
        private int _minImport;
        private int _maxInventory;
        private int _maxDebt;
        private int _minInventory;
        private int _saleRatio;
        private int _debtAvailable;
        
        public SnackbarMessageQueue MyMessageQueue { get => myMessageQueue; set { myMessageQueue = value; OnPropertyChanged(nameof(MyMessageQueue)); } }
        private SnackbarMessageQueue myMessageQueue;
        

        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public int MinImport { get => _minImport; set { _minImport = value; OnPropertyChanged(); } }
        public int MaxInventory { get => _maxInventory; set { _maxInventory = value; OnPropertyChanged(); } }
        public int MaxDebt { get => _maxDebt; set { _maxDebt = value; OnPropertyChanged(); } }
        public int MinInventory { get => _minInventory; set { _minInventory = value; OnPropertyChanged(); } }
        public int SaleRatio { get => _saleRatio; set { _saleRatio = value; OnPropertyChanged(); } }
        public int DebtAvailable { get => _debtAvailable; set { _debtAvailable = value; OnPropertyChanged(); } }

        public ICommand OpenRegulationChange { get; set; }
        public ICommand SaveRegulationChange { get; set; }

        public RegulationViewModel()
        {
            //var select = from s in DataProvider.Ins.DB.THAMSOes select s;
            //foreach (var data in select)
            //{
            //    Id = 1;
            //    NumberOfTable = (int)data.so_ban;
            //    MinAge = (int)data.tuoi_toi_thieu_nv;
            //    MaxAge = (int)data.tuoi_toi_da_nv;
            //    SpecialOffer = (int)data.loai_san_pham_uu_dai;
            //    SpecialOfferCount = (int)data.count_uu_dai;
            //    SpecialOfferMoney = (decimal)data.muc_tien_nhan_uu_dai;
            //}
            var minImport = DataProvider.Ins.DB.THAMSOes.SingleOrDefault(x => x.MaTS == 1);
            MinImport = (int)minImport.GiaTri;
            var maxInventory = DataProvider.Ins.DB.THAMSOes.SingleOrDefault(x => x.MaTS == 2);
            MaxInventory = (int)maxInventory.GiaTri;
            var maxDebt = DataProvider.Ins.DB.THAMSOes.SingleOrDefault(x => x.MaTS == 3);
            MaxDebt = (int)maxDebt.GiaTri;
            var minInventory = DataProvider.Ins.DB.THAMSOes.SingleOrDefault(x => x.MaTS == 4);
            MinInventory = (int)minInventory.GiaTri;
            var saleRatio = DataProvider.Ins.DB.THAMSOes.SingleOrDefault(x => x.MaTS == 5);
            SaleRatio = (int)saleRatio.GiaTri;
            var debtAvailable = DataProvider.Ins.DB.THAMSOes.SingleOrDefault(x => x.MaTS == 6);
            DebtAvailable = (int)debtAvailable.GiaTri;

            SaveRegulationChange = new RelayCommand<StackPanel>((p) => { return true; }, (p) => { SaveRegulation(p); });
            

            MyMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(4000));
            MyMessageQueue.DiscardDuplicates = true;
        }
        void SaveRegulation(StackPanel regulationChangeForm)
        {
            if (Validator.IsValid(regulationChangeForm))
            {
                var change1 = DataProvider.Ins.DB.THAMSOes.SingleOrDefault(x => x.MaTS == 1);
                change1.GiaTri = MinImport;
                DataProvider.Ins.DB.SaveChanges();
                var change2 = DataProvider.Ins.DB.THAMSOes.SingleOrDefault(x => x.MaTS == 2);
                change2.GiaTri = MaxInventory;
                DataProvider.Ins.DB.SaveChanges();
                var change3 = DataProvider.Ins.DB.THAMSOes.SingleOrDefault(x => x.MaTS == 3);
                change3.GiaTri = MaxDebt;
                DataProvider.Ins.DB.SaveChanges();
                var change4 = DataProvider.Ins.DB.THAMSOes.SingleOrDefault(x => x.MaTS == 4);
                change4.GiaTri = MinInventory;
                DataProvider.Ins.DB.SaveChanges();
                var change5 = DataProvider.Ins.DB.THAMSOes.SingleOrDefault(x => x.MaTS == 5);
                change5.GiaTri = SaleRatio;
                DataProvider.Ins.DB.SaveChanges();
                var change6 = DataProvider.Ins.DB.THAMSOes.SingleOrDefault(x => x.MaTS == 6);
                change6.GiaTri = DebtAvailable;
                DataProvider.Ins.DB.SaveChanges();

                //change.tuoi_toi_thieu_nv = MinAge;
                //change.tuoi_toi_da_nv = MaxAge;
                //change.loai_san_pham_uu_dai = SpecialOffer;
                //change.count_uu_dai = SpecialOfferCount;
                //change.muc_tien_nhan_uu_dai = SpecialOfferMoney;
                //DataProvider.Ins.DB.SaveChanges();
                MyMessageQueue.Enqueue("Lưu quy định thành công!");
            }
            else
            {
                MyMessageQueue.Enqueue("Lỗi. Quy định không hợp lệ");
            }

        }
       
    }
}

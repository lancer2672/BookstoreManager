﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BookstoreManager.ViewModels.BookViewModels;

namespace BookstoreManager.Views.BookViews
{
    /// <summary>
    /// Interaction logic for AddBookWindow.xaml
    /// </summary>
    public partial class AddBookWindow : Window
    {
        public AddBookWindow(ManageBookViewModel BookVM)
        {
            InitializeComponent();
            AddBookViewModel AddBookVM = new AddBookViewModel(BookVM);
            this.DataContext = AddBookVM;
        }

    }
}

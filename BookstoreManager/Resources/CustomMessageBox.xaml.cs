using System;
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

namespace BookstoreManager.Resources
{
    /// <summary>
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Window
    {
        public CustomMessageBox(string Message, MessageType Type,string title, MessageButtons Buttons)
        {
            InitializeComponent();
            txtMessage.Content = Message;
            switch (Type)
            {
                case MessageType.Info:
                    txtTitle.Text = title;
                    break;
                case MessageType.Confirmation:
                    txtTitle.Text = title;
                    break;
                case MessageType.Success:
                    {
                        string defaultColor = "#4527a0";
                        Color bkColor = (Color)ColorConverter.ConvertFromString(defaultColor);
                        changeBackgroundThemeColor(Colors.Green);
                        txtTitle.Text = title;
                    }
                    break;
                case MessageType.Warning:
                    txtTitle.Text = title;
                    break;
                case MessageType.Error:
                    {
                        string defaultColor = "#F44336";
                        Color bkColor = (Color)ColorConverter.ConvertFromString(defaultColor);
                        changeBackgroundThemeColor(bkColor);
                        changeBackgroundThemeColor(Colors.Red);
                        txtTitle.Text = title;
                    }
                    break;
            }
    }
        public void changeBackgroundThemeColor(Color newColor)
        {
            cardHeader.Background = new SolidColorBrush(newColor);
            btnClose.Foreground = new SolidColorBrush(newColor);
            btnYes.Background = new SolidColorBrush(newColor);
            btnNo.Background = new SolidColorBrush(newColor);

            btnOk.Background = new SolidColorBrush(newColor);
            btnCancel.Background = new SolidColorBrush(newColor);
        }
        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
    public enum MessageType
    {
        Info,
        Confirmation,
        Success,
        Warning,
        Error,
    }
    public enum MessageButtons
    {
        OkCancel,
        YesNo,
        Ok,
    }
}

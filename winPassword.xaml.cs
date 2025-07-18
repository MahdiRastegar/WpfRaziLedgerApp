using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
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
using Newtonsoft.Json;

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for winLogin.xaml
    /// </summary>
    public partial class winPassword : Window
    {
        public bool StateOk = false;
        public string password="1";
        public winPassword()
        {
            InitializeComponent();           
        }

        private async void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (txtSubscription.Password.Trim() == "")
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("کاربر گرامی لطفا رمز عبور فعلی را وارد کنید");
                return;
            }
            if (txtSubscription.Password != password)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("رمز عبور فعلی اشتباهست!");
                return;
            }
            if (PasswordText.Password.Trim() == "")
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("کاربر گرامی لطفا رمز عبور جدید را وارد کنید");
                return;
            }            
            if (PasswordText.Password != PasswordText2.Password)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("رمز عبور ها با هم همخوان نیست!");
                return;
            }
            //App.SetKeyPassword(PasswordText.Password);
            StateOk = true;
            Close();
        }
        public static DateTime GetNistTime()
        {
            try
            {
                var myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://google.com");
                myHttpWebRequest.Timeout = 3000;
                var response = myHttpWebRequest.GetResponse();
                string todaysDates = response.Headers["date"];
                return DateTime.ParseExact(todaysDates,
                                           "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                           CultureInfo.InvariantCulture.DateTimeFormat,
                                           DateTimeStyles.AssumeUniversal);
            }
            catch 
            {
                return DateTime.Now;
            }
        }

        private void txtSubscription_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnConfirm_Click(null,null);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            if (password == "1")
            {
                txtSubscription.Password = "1";
                PasswordText.Focus();
            }
            else
                txtSubscription.Focus();
        }

        private void PasswordText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter) 
            {
                btnConfirm.Focus();
            }
        }

        private void PasswordText_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PasswordText2.Focus();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

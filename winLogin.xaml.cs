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
using Microsoft.EntityFrameworkCore;
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Controls.Primitives;

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for winLogin.xaml
    /// </summary>
    public partial class winLogin : Window
    {
        public string password="1";
        public bool NeedChack = false;
        public winLogin()
        {
            InitializeComponent();           
        }

        private async void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (cmbPeriod.SelectedIndex == -1)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("کاربر گرامی لطفا دوره را انتخاب کنید");
                return;
            }
            if (cmbUsers.SelectedIndex == -1)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("کاربر گرامی لطفا کاربر را انتخاب کنید");
                return;
            }
            if (PasswordText.Password == "")
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("لطفا رمز عبور را وارد کنید");
                return;
            }

            using var db = new wpfrazydbContext();
            if (db.UserApps.FirstOrDefault(u => u.UserName == cmbUsers.Text) is UserApp userApp)
            {
                if (PasswordText.Password == userApp.Password)
                {
                    App.Suspended = false;
                    MainWindow.Current.StatusOptions = new StatusOptions()
                    {
                        User = userApp,
                        VD = db.Versions.First().DataBaseRazyDb,
                        VP = db.Versions.First().Application,
                        Date = DateTime.Now.ToPersianDateString(),
                        Period = cmbPeriod.SelectedItem as Period,
                    };
                    var oldContext = MainWindow.Current.statusBar.DataContext;
                    MainWindow.Current.statusBar.DataContext = null;
                    MainWindow.Current.statusBar.DataContext = oldContext;
                    foreach (var item in ((MainWindow.Current.statusBar.Items[0] as StatusBarItem).Content as Grid).GetChildsOfType<TextBlock>())
                    {
                        var binding = item.GetBindingExpression(TextBlock.TextProperty);
                        binding?.UpdateTarget();
                    }
                    Close();
                    return;
                }
            }
            Xceed.Wpf.Toolkit.MessageBox.Show("رمز عبور اشتباهست!");
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
            using var db = new wpfrazydbContext();
            cmbPeriod.ItemsSource = db.Periods.AsNoTracking().ToList();
            cmbUsers.ItemsSource = db.UserApps.AsNoTracking().ToList();
            cmbPeriod.SelectedIndex = 0;
            cmbUsers.SelectedIndex = 0;
            PasswordText.Focus();
        }

        private void PasswordText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter) 
            {
                btnConfirm.Focus();
            }
        }

        private void btnPassword_Click(object sender, RoutedEventArgs e)
        {
            var win = new winPassword();
            win.password = password;
            win.ShowDialog();
            if (win.StateOk == true)
            {
                password = win.PasswordText.Password;
                PasswordText.Password = "";
                PasswordText.Focus();
            } 
        }

        private void cmbUsers_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void cmbUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbPeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbPeriod_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (App.Suspended == true)
                App.Current.Shutdown();
        }
    }
}

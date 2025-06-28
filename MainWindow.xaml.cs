﻿using Stimulsoft.Report;
using Syncfusion.Linq;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfRaziLedgerApp.Interfaces;
using XamlAnimatedGif;

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Current;
        private int _TaxPercent=-1;

        public int TaxPercent
        {
            get { return _TaxPercent; }
            set { _TaxPercent = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
            ribbon.RibbonState = Syncfusion.Windows.Tools.RibbonState.Hide;
            Current = this;
            using var db=new wpfrazydbContext();
            if (db.CodeSettings.FirstOrDefault(i => i.Name == "TaxPercent") is CodeSetting codeSetting)
                TaxPercent = int.Parse(codeSetting.Value);
            var gifImage = new BitmapImage(new Uri("pack://application:,,,/Images/AddDataLarge.gif"));
            XamlAnimatedGif.AnimationBehavior.SetSourceUri(this.gifImage, gifImage.UriSource);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //var M = new winCol();
            //M.ShowDialog();
        }

        private void BtnMoein_Click(object sender, RoutedEventArgs e)
        {
            //var M = new winMoein();
            //M.ShowDialog();
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            var u = "3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679821480865132823066470938446095505822317253594081284811174502841027019385211055596446229489549303819644288109756659334461284756482337867831652712019091456485669234603486104543266482133936072602491412737245870066063155881748815209209628292540917153643678925903600113305305488204665213841469519415116094330572703657595919530921861173819326117931051185480744623799627495673518857527248912279381830119491";
            var y = u.Substring(0, 42);
            var a = "3.1415926535897932384626433832795028841971693993751058209749445923078164062862089469509070248424167755216229746776236362716129014677044253241618530641369520079400145492904783927346278766000080780449455";
            var b = a.Substring(0, 42);

        }
        public IEnumerable<TabItemExt> GetTabControlItems
        {
            get
            {
                return tabcontrol.Items.ToList<TabItemExt>();
            }
        }

        private void rbnCol_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "حساب کل");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "حساب کل" };                
                item.Content = new winCol();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnMoein_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "حساب معین");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "حساب معین" };
                item.Content = new winMoein();
                tabcontrol.Items.Add(item);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!LoadedWin)
                return;
            double newWidth = e.NewSize.Width;
            double newHeight = e.NewSize.Height;
            ribbon.Margin = new Thickness(ribbon.Margin.Left + (newWidth - e.PreviousSize.Width) / 4.33, 0, 0, 0);
            /*var t = ((ribbon.RenderTransform as TransformGroup).Children[3] as TranslateTransform);
            t.X += (newWidth - e.PreviousSize.Width)/3.3333333;*/
        }
        bool LoadedWin = false;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadedWin = true;
            WindowState = WindowState.Maximized;
        }

        private void tabcontrol_TabClosed(object sender, CloseTabEventArgs e)
        {
            (e.TargetTabItem.Content as IDisposable)?.Dispose();
            tabcontrol.Items.Remove(e.TargetTabItem);
        }

        private void tabcontrol_TabClosing(object sender, CancelingRoutedEventArgs e)
        {
            this.Effect = new BlurEffect() { Radius = 4 };
            e.Cancel = !((e.OriginalSource as TabItemExt).Content as ITabForm).CloseForm();
            this.Effect = null;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {

        }

        private void ribbon_RibbonStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(ribbon.RibbonState== Syncfusion.Windows.Tools.RibbonState.Hide)
            {
                row.Height = new GridLength();
            }
            else
                row.Height = new GridLength(197);
        }

        private void rbnGroup_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "گروه تفضیلی");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "گروه تفضیلی" };
                item.Content = new usrGroup();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnPreferential_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "حساب تفضیلی");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "حساب تفضیلی" };                
                item.Content = new usrPreferential();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnAgroup_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "گروه حساب");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "گروه حساب" };
                item.Content = new usrAgroup();
                tabcontrol.Items.Add(item);
            }
        }

        private void tabcontrol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ribbon.RibbonState = Syncfusion.Windows.Tools.RibbonState.Hide;
            row.Height = new GridLength();            
        }

        private void rbnAcType_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "نوع سند");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "نوع سند" };
                item.Content = new usrAcType();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnAcDoc_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "سند حسابداری");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "سند حسابداری" };
                item.Content = new usrAccountDocument();
                tabcontrol.Items.Add(item);
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((tabcontrol.SelectedItem as TabItemExt)?.Content is ITabEdidGrid usrAccountDocument)
            {
                if (usrAccountDocument.DataGridIsFocused && e.Key == Key.Enter)
                {
                    usrAccountDocument.SetEnterToNextCell();
                    e.Handled = true;
                    return;
                }
            }
            if ((tabcontrol.SelectedItem as TabItemExt)?.Content is UserControl userControl)
            {
                if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Keyboard.IsKeyDown(Key.S))
                {
                    Type type = userControl.GetType();

                    MethodInfo method = type.GetMethod("btnConfirm_Click", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (method != null)
                    {
                        e.Handled = true;
                        object[] parameters = new object[] { null, null };

                        method.Invoke(userControl, parameters);
                    }
                }
            }
        }

        private void rbnBank_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "بانک");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "بانک" };
                item.Content = new usrBank();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnRecieveMoney_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "دریافت وجه");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "دریافت وجه" };
                item.Content = new usrRecieveMoney();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnPaymentMoney_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "پرداخت وجه");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "پرداخت وجه" };
                item.Content = new usrPaymentMoney();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnRecieveCheck_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "چک های دریافتی");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "چک های دریافتی" };
                item.Content = new usrRecieveCheck();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnPaymentCheck_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "چک های پرداختی");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "چک های پرداختی" };
                item.Content = new usrPaymentCheck();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnProvince_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "استان");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "استان" };
                item.Content = new usrProvince();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnCity_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "شهر");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "شهر" };
                item.Content = new usrCity();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnPriceGroup_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "گروه قیمت");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "گروه قیمت" };
                item.Content = new usrPriceGroup();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnCustomerGroup_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "گروه مشتریان");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "گروه مشتریان" };
                item.Content = new usrCustomerGroup();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnGroupStorage_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "گروه انبار");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "گروه انبار" };
                item.Content = new usrGroupStorage();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnDefinitionStorage_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "انبار");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "انبار" };
                item.Content = new usrStorage();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnUnit_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "واحد اندازه گیری");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "واحد اندازه گیری" };
                item.Content = new usrUnit();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnGroupCommodity_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "گروه کالا");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "گروه کالا" };
                item.Content = new usrGroupCommodity();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnDefinitionCommodity_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "کالا");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "کالا" };
                item.Content = new usrCommodity();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnCommodityPricingPanel_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "پنل قیمت گذاری کالا");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "پنل قیمت گذاری کالا" };
                item.Content = new usrCommodityPricingPanel();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnCodingReceiptTypes_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "کدینگ انواع رسید");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "کدینگ انواع رسید" };
                item.Content = new usrCodingReceiptTypes();
                tabcontrol.Items.Add(item);
            }
        }
        private void rbnCodingTypesTransfer_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "کدینگ انواع حواله");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "کدینگ انواع حواله" };
                item.Content = new usrCodingTypesTransfer();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnStorageReceipt_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "رسید انبار");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "رسید انبار" };
                item.Content = new usrStorageReceipt();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnStorageTransfer_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "حواله انبار");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "حواله انبار" };
                item.Content = new usrStorageTransfer();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnStorageBetweenTransfer_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "حواله بین انبار");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "حواله بین انبار" };
                item.Content = new usrStorageBetweenTransfer();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnStorageRotation_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "انبارگردانی");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "انبارگردانی" };
                item.Content = new usrStorageRotation();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnNPStorage_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "کسر و اضافات انبار");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "کسر و اضافات انبار" };
                item.Content = new usrNPStorage();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnOrder_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "سفارش");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "سفارش" };
                item.Content = new usrOrder();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnPurchaseInvoice_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "فاکتور خرید");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "فاکتور خرید" };
                item.Content = new usrProductBuy();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnSalesInvoice_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "فاکتور فروش");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "فاکتور فروش" };
                item.Content = new usrProductSell();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnSalesProforma_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "پیش فاکتور فروش");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "پیش فاکتور فروش" };
                item.Content = new usrPreInvoice();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnConfiguration_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "تنظیمات پیکربندی");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "تنظیمات پیکربندی" };
                item.Content = new usrSettingConfig();
                tabcontrol.Items.Add(item);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید از برنامه خارج شوید؟", "خروج", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
        }

        private void rbnBrowseAccounts_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "مرور حساب ها");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "مرور حساب ها" };
                item.Content = new usrBrowseAccounts();
                tabcontrol.Items.Add(item);
            }

            //var report = new StiReport();
            //report.Load("Report.mrt");

            //// فرض بر اینکه ItemsSource گرید از نوع ObservableCollection<Customer> باشد
            //var data = dataGrid.ItemsSource as IEnumerable<Customer>;
            //report.RegBusinessObject("CustomerData", data);  // "CustomerData" باید با نام منبع داده در .mrt مطابقت داشته باشد

            //report.Compile();
            //report.Render();
            //report.ShowWithWpf(); // یا: report.Print(); یا report.ExportDocument(StiExportFormat.Pdf, ...);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Syncfusion.CompoundFile.XlsIO.Native;
using Syncfusion.Data.Extensions;
using Syncfusion.Windows.Controls;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfRaziLedgerApp.Interfaces;
using WpfRaziLedgerApp.Reports.Charts;

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for usrBrowseAccounts.xaml
    /// </summary>
    public partial class usrSellReport : UserControl, ITabForm
    {
        public usrSellReport()
        {
            InitializeComponent();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (dataPager.Source is IEnumerable<SellReport> source && source.Count() > 0&& datagrid.View.Records.Select(r => r.Data as SellReport).ToList() is List<SellReport> sellReports1 && sellReports1.Count>0)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                //switch (GAcClassEntities[0].GetType())
                //{
                //    case Type t when t == typeof(GAcClass):                        

                System.IO.Directory.Delete(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "JSON"), true);
                Thread.Sleep(50);
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "JSON"));

                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };
                string jsonString = JsonSerializer.Serialize(sellReports1, options);
                System.IO.File.WriteAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "JSON", "SellReport.json"), jsonString);
                Mouse.OverrideCursor = null;

                Process process = new Process();
                process.StartInfo.FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "WpfAppEmpty.exe");
                //process.StartInfo.Arguments = $"\"{reportPath}\" \"{outputPdf}\"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.Arguments = MainWindow.StatusOptions.Period.Value.ToString();
                process.Start();
            }            
        }

    public ObservableCollection<SellReport> SellReports { get; set; }
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                using var db = new wpfrazydbContext();

                List<ProductBuyDetail> data1 = null;
                List<ProductSellDetail> data2 = null;

                if (SellReports != null)
                    SellReports.Clear();
                var pc = new PersianCalendar();
                string[] persianMonths = {
    "فروردین", "اردیبهشت", "خرداد",
    "تیر", "مرداد", "شهریور",
    "مهر", "آبان", "آذر",
    "دی", "بهمن", "اسفند"
};

                SellReports = db.ProductSellDetails
                    .Include(psd => psd.FkCommodity)
                    .Include(psd => psd.FkHeader)
                        .ThenInclude(h => h.FkPreferential)
                            .ThenInclude(p => p.FkCity)
                                .ThenInclude(c => c.FkProvince)
                    .AsEnumerable() // از اینجا به بعد PersianCalendar و آرایه ماه‌ها
                    .GroupBy(psd => new
                    {
                        psd.FkCommodity.Code,
                        psd.FkCommodity.Name,
                        Province = psd.FkHeader.FkPreferential.FkCity?.FkProvince?.Name,
                        City = psd.FkHeader.FkPreferential.FkCity?.Name,
                        Month = persianMonths[pc.GetMonth(psd.FkHeader.Date) - 1],
                        Tonnage = psd.FkCommodity.Tonnage
                    })
                    .Select(g => new SellReport
                    {
                        Code = g.Key.Code,
                        Name = g.Key.Name,
                        Count = g.Sum(x => x.Value),
                        Province = g.Key.Province,
                        City = g.Key.City,
                        Month = g.Key.Month,
                        Tonnage = g.Key.Tonnage.HasValue
    ? g.Sum(x => g.Key.Tonnage.Value * x.Value)
    : (decimal?)null,
                        Price = g.Sum(x => (x.Value * x.Fee)-x.Discount)
                    })
                    .ToList().ToObservableCollection();

                    datagrid.SearchHelper.AllowFiltering = true;
                    try
                    {
                        dataPager.Source = null;
                    }
                    catch { }
                    try
                    {
                        dataPager.Source = new ObservableCollection<BuyRemittanceReport>();
                    }
                    catch (Exception ex) { }
                    try
                    {
                        dataPager.Source = SellReports;
                    }
                    catch (Exception ex) { }
                  
                //var options = new JsonSerializerOptions
                //{
                //    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                //    WriteIndented = true
                //};
                //string jsonString = JsonSerializer.Serialize(BrowseAccountsEntities, options);
                //System.IO.File.WriteAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.json"), jsonString);

                
                /*
                System.IO.File.WriteAllLines(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "reportOption.txt"), new string[]
                {
                txtFromDoc.Text == "" ? "اول" : txtFromDoc.Text,
            txtToDoc.Text == "" ? "آخر" : txtToDoc.Text,
             txbCalender.Text == "" ? "ابتدای دوره" : txbCalender.Text,
            txbCalender2.Text == "" ? "انتهای دوره" : txbCalender2.Text
            });*/

                Mouse.OverrideCursor = null;
            }
            catch(Exception ex) 
            {
                Mouse.OverrideCursor = null;
                MessageBox.Show(ex.Message);
            }
        }

        private void Pcw1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            datagrid.AllowFiltering = !datagrid.AllowFiltering;
            if (!datagrid.AllowFiltering)
                datagrid.ClearFilters();
        }
        private void datagrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // ارتفاع سطرهای grid را محاسبه کنید (می‌توانید ارتفاع سطر ثابت فرض کنید)
            double rowHeight = 30; // ارتفاع هر سطر (این مقدار ممکن است بسته به طراحی تغییر کند)

            // ارتفاع موجود در grid را محاسبه کنید
            double availableHeight = (sender as Syncfusion.UI.Xaml.Grid.SfDataGrid).ActualHeight;

            // محاسبه تعداد سطرهایی که در صفحه جا می‌شوند
            int visibleRows = (int)(availableHeight / rowHeight);

            // تنظیم PageSize بر اساس تعداد سطرهای محاسبه شده
            if (visibleRows > 0)
            {
                var sfData = PanelPager.Children.ToList<Syncfusion.UI.Xaml.Controls.DataPager.SfDataPager>().FirstOrDefault(a => a.Visibility == Visibility.Visible);
                sfData.PageSize = visibleRows - 3;
                var g = sfData.Source;
                try
                {
                    sfData.Source = null;
                }
                catch { }
                sfData.Source = g;
            }
        }

        private void datagrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void SearchTermTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (SearchTermTextBox.Text.Trim() == "")
                    datagrid.SearchHelper.ClearSearch();
                else
                    datagrid.SearchHelper.Search(SearchTermTextBox.Text);
            }
            catch (Exception ex)
            {
            }
        }

        private void ClearSearch_MouseEnter(object sender, MouseEventArgs e)
        {
            ClearSearch.Opacity = 1;
        }

        private void ClearSearch_MouseLeave(object sender, MouseEventArgs e)
        {
            ClearSearch.Opacity = .65;
        }

        private void ClearSearch_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SearchTermTextBox.Clear();
        }

        private void Txt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private static readonly Regex _regex = new Regex("[^0-9]"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        public bool CloseForm()
        {            
            var list = MainWindow.Current.GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "گزارش فروش");
            MainWindow.Current.tabcontrol.Items.Remove(item);
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Dispose();
            }));
            return true;
        }

        private void Dispose()
        {
            if (DataContext == null)
                return;
            SellReports.Clear();
            datagrid.Dispose();
            dataPager.Dispose();
            DataContext = null;
            GC.Collect();
        }

        public void SetNull()
        {
            throw new NotImplementedException();
        }


        private void datagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void datagridCol_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            var datagridF = control.SelectedContent as Syncfusion.UI.Xaml.Grid.SfDataGrid;
            if (datagridF != null)
                datagridF.ItemsSource = dataPager.Source;*/
        }

        private void btnChart_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var win = new winChart();
            using var db = new wpfrazydbContext();
            var pc = new PersianCalendar();
            string[] persianMonths = {
    "فروردین", "اردیبهشت", "خرداد",
    "تیر", "مرداد", "شهریور",
    "مهر", "آبان", "آذر",
    "دی", "بهمن", "اسفند"
};

            win.MonthlyTotalSales = db.ProductSellDetails
                .Include(psd => psd.FkHeader)
                .AsEnumerable() // نیاز داریم برای PersianCalendar
                .GroupBy(psd => new
                {
                    Year = pc.GetYear(psd.FkHeader.Date),
                    Month = pc.GetMonth(psd.FkHeader.Date)
                })
                .Select((g, index) => new MonthlyTotalSale
                {
                    Index = persianMonths.IndexOf(persianMonths[g.Key.Month - 1]) + 1,
                    Month = $"{g.Key.Year} {persianMonths[g.Key.Month - 1]}",
                    Price = g.Sum(x => x.Value * x.Fee)/1000
                })
                .OrderBy(x => x.Index)
                .ToList().ToObservableCollection();

            // تناژ کل به تفکیک ماه (شمسی)
            win.MonthlyTonnageTotals = db.ProductSellDetails
    .Include(psd => psd.FkHeader)
    .Include(psd => psd.FkCommodity)
    .AsEnumerable()
    .GroupBy(psd => new
    {
        Year = pc.GetYear(psd.FkHeader.Date),
        Month = pc.GetMonth(psd.FkHeader.Date)
    })
    // فقط ماه‌هایی که حداقل یک قلم دارای تناژ دارند
    .Where(g => g.Any(x => x.FkCommodity.Tonnage.HasValue))
    .OrderBy(g => g.Key.Year)
    .ThenBy(g => g.Key.Month)
    .Select((g, idx) => new MonthlyTotalTonnage
    {
        Index = persianMonths.IndexOf(persianMonths[g.Key.Month - 1]) + 1,
        Month = $"{g.Key.Year} {persianMonths[g.Key.Month - 1]}",
        Tonnage = g.Where(x => x.FkCommodity.Tonnage.HasValue)
                   .Sum(x => (decimal)x.FkCommodity.Tonnage.Value * x.Value)
    })
    .OrderBy(x => x.Index)
    .ToList().ToObservableCollection();

            win.CityTotalTonnages = db.ProductSellDetails
    .Include(d => d.FkCommodity)
    .Include(d => d.FkHeader)
        .ThenInclude(h => h.FkPreferential)
            .ThenInclude(p => p.FkCity)
    .Where(d => d.FkCommodity.Tonnage.HasValue)
    .AsEnumerable() // از اینجا به بعد روی RAM اجرا میشه
    .GroupBy(d => d.FkHeader.FkPreferential.FkCity != null
              ? d.FkHeader.FkPreferential.FkCity.Name
              : "بدون شهر")
    .Select((g, index) => new CityTotalTonnage
    {
        Index = index + 1,
        City = g.Key,
        Tonnage = g.Sum(x => x.FkCommodity.Tonnage.Value * x.Value)
    })
    .ToList().ToObservableCollection();

            win.CommodityTotalTonnages = db.ProductSellDetails
    .Include(d => d.FkCommodity)
    .Include(d => d.FkHeader)
    .Where(d => d.FkCommodity.Tonnage.HasValue) // فقط کالاهایی که ضریب تناژ دارند
    .AsEnumerable() // از اینجا به بعد روی حافظه (برای محاسبه تناژ)
            .GroupBy(d => new
            {
        Month = pc.GetMonth(d.FkHeader.Date),
        CommodityName = d.FkCommodity.Name
    })
    .Select((g, index) => new CommodityTotalTonnage
    {
        Index = persianMonths.IndexOf(persianMonths[g.Key.Month - 1]) + 1,
        Month = persianMonths[g.Key.Month - 1],
        CommodityName = g.Key.CommodityName,
        Tonnage = g.Sum(x => x.FkCommodity.Tonnage.Value * x.Value)
    })
    .OrderBy(x => x.Index)
    .ToList().ToObservableCollection();
            Mouse.OverrideCursor = null;

            win.Show();
        }
    }
}

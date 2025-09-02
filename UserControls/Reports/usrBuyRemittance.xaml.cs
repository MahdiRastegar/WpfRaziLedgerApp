using Microsoft.EntityFrameworkCore;
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

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for usrBrowseAccounts.xaml
    /// </summary>
    public partial class usrBuyRemittance : UserControl, ITabForm
    {
        public usrBuyRemittance()
        {
            InitializeComponent();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {            
            switch (control.SelectedIndex)
            {
                case 0:
                    if (buyRemittanceReports?.Count > 0 && dataPager.Source is IEnumerable<BuyRemittanceReport> source && source.Count() > 0)
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        //switch (GAcClassEntities[0].GetType())
                        //{
                        //    case Type t when t == typeof(GAcClass):                        

                        System.IO.Directory.Delete(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "JSON"),true);
                        Thread.Sleep(50);
                        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "JSON"));

                        var options = new JsonSerializerOptions
                        {
                            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                            WriteIndented = true
                        };
                        string jsonString = JsonSerializer.Serialize(source, options);
                        System.IO.File.WriteAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "JSON", "BuyRemittanceReport.json"), jsonString);
                        Mouse.OverrideCursor = null;
                       
                        Process process = new Process();
                        process.StartInfo.FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "WpfAppEmpty.exe");
                        //process.StartInfo.Arguments = $"\"{reportPath}\" \"{outputPdf}\"";
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.Arguments = MainWindow.StatusOptions.Period.Value.ToString();
                        process.Start();
                    }
                    break;
                case 1:
                    if (datagridِDetails.ItemsSource is Syncfusion.UI.Xaml.Grid.GridPagedCollectionViewWrapper fas)
                    {
                        if (fas.SourceCollection is ObservableCollection<BuyRemittanceDetail> Entities && Entities.Count > 0)
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
                            string jsonString = JsonSerializer.Serialize(Entities, options);
                            System.IO.File.WriteAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "JSON", "BuyRemittanceDetail.json"), jsonString);
                            Mouse.OverrideCursor = null;
                            
                            Process process = new Process();
                            process.StartInfo.FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "WpfAppEmpty.exe");
                            //process.StartInfo.Arguments = $"\"{reportPath}\" \"{outputPdf}\"";
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.Arguments = MainWindow.StatusOptions.Period.Value.ToString();
                            process.Start();
                        }
                    }
                    break;
            }
        }

    public ObservableCollection<BuyRemittanceReport> buyRemittanceReports { get; set; }
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                using var db = new wpfrazydbContext();

                List<ProductBuyDetail> data1 = null;
                List<ProductSellDetail> data2 = null;

                if (buyRemittanceReports != null)
                    buyRemittanceReports.Clear();
                buyRemittanceReports = new ObservableCollection<BuyRemittanceReport>();
                int? buyRemittanceNumber = string.IsNullOrWhiteSpace(txtBuyRemittanceNumber.Text)
    ? (int?)null
    : int.Parse(txtBuyRemittanceNumber.Text);

                if (txtBuyRemittanceNumber.Text == "" && txbCalender.Text == "" && txbCalender2.Text == "")
                {
                    data1 = db.ProductBuyDetails
                        .Include(x => x.FkCommodity)
                        .Include(x => x.FkHeader)
                        .ThenInclude(w => w.FkPreferential).Where(u => u.FkHeader.BuyRemittanceNumber.HasValue && (buyRemittanceNumber == null
                 || u.FkHeader.BuyRemittanceNumber == buyRemittanceNumber))
        .AsNoTracking()
        .ToList();
                    data2 = db.ProductSellDetails
                        .Include(x => x.FkCommodity)
                        .Include(x => x.FkHeader)
                        .ThenInclude(w => w.FkPreferential).Where(u => u.FkHeader.BuyRemittanceNumber.HasValue && (buyRemittanceNumber == null
                 || u.FkHeader.BuyRemittanceNumber == buyRemittanceNumber))
        .AsNoTracking()
        .ToList();
                }
                else
                {
                    long fr = 0;
                    long to = long.MaxValue;
                    DateTime minx = DateTime.MinValue;
                    DateTime max = DateTime.MaxValue;

                    if (txtBuyRemittanceNumber.Text != "")
                        fr = long.Parse(txtBuyRemittanceNumber.Text);
                    if (txbCalender.Text != "")
                        minx = pcw1.SelectedDate.ToDateTime();
                    if (txbCalender2.Text != "")
                        max = pcw2.SelectedDate.ToDateTime();
                    data1 = db.ProductBuyDetails
                        .Include(x => x.FkCommodity)
                        .Include(x => x.FkHeader)
                        .ThenInclude(w => w.FkPreferential).Where(u => u.FkHeader.Date <= max && u.FkHeader.Date >= minx && u.FkHeader.BuyRemittanceNumber.HasValue && (buyRemittanceNumber == null
                 || u.FkHeader.BuyRemittanceNumber == buyRemittanceNumber))
        .AsNoTracking()
        .ToList();
                    data2 = db.ProductSellDetails
                        .Include(x => x.FkCommodity)
                        .Include(x => x.FkHeader)
                        .ThenInclude(w => w.FkPreferential).Where(u => u.FkHeader.Date <= max && u.FkHeader.Date >= minx && u.FkHeader.BuyRemittanceNumber.HasValue && (buyRemittanceNumber == null
                 || u.FkHeader.BuyRemittanceNumber == buyRemittanceNumber))
        .AsNoTracking()
        .ToList();
                }

                var groupedX1 = data1        
         .GroupBy(y => y.FkCommodityId)
        .ToList();
                var groupedX2 = data2
         .GroupBy(y => y.FkCommodityId)
        .ToList();

                // همه Id های کالاها (از خرید و فروش)
                var allCommodityIds = groupedX1.Select(g => g.Key)
                    .Union(groupedX2.Select(g => g.Key))
                    .Distinct()
                    .ToList();

                foreach (var commodityId in allCommodityIds)
                {
                    var buyGroup = groupedX1.FirstOrDefault(g => g.Key == commodityId);
                    var sellGroup = groupedX2.FirstOrDefault(g => g.Key == commodityId);

                    // پیدا کردن اطلاعات کالا از یک نمونه (خرید یا فروش)
                    var commodityInfo = buyGroup?.FirstOrDefault()?.FkCommodity
                                      ?? sellGroup?.FirstOrDefault()?.FkCommodity;

                    // ساخت همه رکوردهای خرید (ممکنه چندتا باشه)
                    var buyDetails = buyGroup?.Select(x => new BuyRemittanceDetail
                    {
                        Id = x.FkCommodityId,
                        Code = x.FkCommodity.Code,
                        Name = x.FkCommodity.Name,
                        Count = x.Value,
                        PreferentialCode = x.FkHeader.FkPreferential.PreferentialCode,
                        PreferentialName = x.FkHeader.FkPreferential.PreferentialName,
                        BuyRemittanceNumber = buyGroup.First().FkHeader.BuyRemittanceNumber,
                        SellOrBuy = "خرید",
                        Date = x.FkHeader.Date
                    }) ?? Enumerable.Empty<BuyRemittanceDetail>();

                    // ساخت همه رکوردهای فروش (ممکنه چندتا باشه)
                    var sellDetails = sellGroup?.Select(x => new BuyRemittanceDetail
                    {
                        Id = x.FkCommodityId,
                        Code = x.FkCommodity.Code,
                        Name = x.FkCommodity.Name,
                        Count = x.Value,
                        PreferentialCode = x.FkHeader.FkPreferential.PreferentialCode,
                        PreferentialName = x.FkHeader.FkPreferential.PreferentialName,
                        BuyRemittanceNumber = sellGroup.First().FkHeader.BuyRemittanceNumber,
                        SellOrBuy = "فروش",
                        Date = x.FkHeader.Date
                    }) ?? Enumerable.Empty<BuyRemittanceDetail>();
                    // ترکیب خرید و فروش + مرتب‌سازی بر اساس تاریخ
                    var allDetailsSorted = buyDetails
                        .Concat(sellDetails)
                        .OrderBy(d => d.Date) // مرتب‌سازی بر اساس تاریخ
                        .ToList();
                    // محاسبه RemainingCount
                    decimal runningCount = 0;
                    foreach (var detail in allDetailsSorted)
                    {
                        if (detail.SellOrBuy == "خرید")
                            runningCount += detail.Count;
                        else if (detail.SellOrBuy == "فروش")
                            runningCount -= detail.Count;

                        detail.RemainingCount = runningCount;
                    }
                    var report = new BuyRemittanceReport
                    {
                        Id = commodityInfo.Id,
                        Code = commodityInfo.Code,
                        Name = commodityInfo.Name,
                        BuySum = buyGroup?.Sum(x => x.Value) ?? 0,  // فرض بر اینکه Quantity تعداد خرید هست
                        SellSum = sellGroup?.Sum(x => x.Value) ?? 0,
                        buyRemittanceDetails = new ObservableCollection<BuyRemittanceDetail>(allDetailsSorted)
                    };

                    buyRemittanceReports.Add(report);
                }

                if (control.SelectedIndex == 0)
                {
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
                        dataPager.Source = buyRemittanceReports;
                    }
                    catch (Exception ex) { }
                    //datagrid.ItemsSource=dataPager.Source;
                }
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

        private void pcw1_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            txbCalender.Text = pcw1.SelectedDate.ToString();

        }

        private void Pcw1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }

        private void pcw2_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            txbCalender2.Text = pcw2.SelectedDate.ToString();

        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            var datagridF = control.SelectedContent as Syncfusion.UI.Xaml.Grid.SfDataGrid;
            datagridF.AllowFiltering = !datagridF.AllowFiltering;
            if (!datagridF.AllowFiltering)
                datagridF.ClearFilters();
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
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "گزارش حواله خرید");
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
            buyRemittanceReports.Clear();
            datagrid.Dispose();
            datagridِDetails.Dispose();
            dataPager.Dispose();
            dataPager5.Dispose();
            DataContext = null;
            GC.Collect();
        }

        public void SetNull()
        {
            throw new NotImplementedException();
        }

        private void control_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (control.SelectedIndex)
            {
                case 0:
                    dataPager5.Visibility = Visibility.Collapsed;
                    dataPager.Visibility = Visibility.Visible;
                    datagrid.SearchHelper.AllowFiltering = true;
                    try
                    {
                        dataPager.Source = null;
                    }
                    catch { }
                    try
                    {
                        dataPager.Source = buyRemittanceReports;
                    }
                    catch { }
                    break;                
                case 1:
                    dataPager.Visibility = Visibility.Collapsed;
                    dataPager5.Visibility = Visibility.Visible;
                    //if (datagridِDetails.ItemsSource is Syncfusion.UI.Xaml.Grid.GridPagedCollectionViewWrapper gridPagedCollectionView)
                    //{
                    //    if(gridPagedCollectionView.Records.Count>0&& !(gridPagedCollectionView.Records[0] is AcDocumentDetail)&& !(dataPager.Source is ObservableCollection<AcDocumentDetail>))
                    //        try
                    //        {
                    dataPager5.Source = null;
                    //        }
                    //        catch { }
                    //}
                    break;
            }
        }

        private void datagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var datagridF = control.SelectedContent as Syncfusion.UI.Xaml.Grid.SfDataGrid;
            if (datagridF.SelectedItem is BuyRemittanceReport buyRemittanceReport)
            {
                datagridF.SearchHelper.AllowFiltering = true;
                try
                {
                    dataPager5.Source = null;
                }
                catch { }
                try
                {
                    dataPager5.Source = buyRemittanceReport.buyRemittanceDetails;
                }
                catch { }
                control.SelectionChanged -= control_SelectionChanged;
                control.SelectedIndex = 1;
                dataPager.Visibility = Visibility.Collapsed;
                dataPager5.Visibility = Visibility.Visible;
                control.SelectionChanged += control_SelectionChanged;
            }
        }

        private void datagridCol_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            var datagridF = control.SelectedContent as Syncfusion.UI.Xaml.Grid.SfDataGrid;
            if (datagridF != null)
                datagridF.ItemsSource = dataPager.Source;*/
        }
    }
}

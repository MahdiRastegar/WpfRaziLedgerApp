using Microsoft.EntityFrameworkCore;
using Syncfusion.Data.Extensions;
using Syncfusion.Windows.Controls;
using Syncfusion.XlsIO.Implementation.PivotAnalysis;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for usrBrowseAccounts.xaml
    /// </summary>
    public partial class usrBillReport : UserControl, ITabForm
    {
        public usrBillReport()
        {
            InitializeComponent();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {            
            switch (control.SelectedIndex)
            {                              
                case 0:
                    if (PreferentialAcReportEntities?.Count > 0 && dataPager4.Source is IEnumerable<PreferentialAcReport> source3 && source3.Count() > 0)
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
                        string jsonString = JsonSerializer.Serialize(source3, options);
                        System.IO.File.WriteAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "JSON", "ReportBill.json"), jsonString);
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
                        if (fas.SourceCollection is ObservableCollection <AcDocumentDetail> Entities&& Entities.Count > 0)
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
                            System.IO.File.WriteAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "JSON", "ReportAcDetail2.json"), jsonString);
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

        public ObservableCollection<PreferentialAcReport> PreferentialAcReportEntities { get; set; }
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                using var db = new wpfrazydbContext();

                List<AcDocumentDetail> data = null;
                DateTime min = DateTime.MinValue;


                if (PreferentialAcReportEntities != null)
                    PreferentialAcReportEntities.Clear();
                PreferentialAcReportEntities = new ObservableCollection<PreferentialAcReport>();

                if (control.SelectedIndex == 0)
                {
                    datagridPreferential.SearchHelper.AllowFiltering = true;
                    try
                    {
                        dataPager4.Source = null;
                    }
                    catch { }
                    try
                    {
                        dataPager4.Source = PreferentialAcReportEntities;
                    }
                    catch (Exception ex) { }
                }

                if (txtMoeinCode.Text == "" && txtPreferentialCode.Text == "" && txbCalender.Text == "" && txbCalender2.Text == "")
                {
                    data = db.AcDocumentDetails
        .Include(x => x.FkAcDocHeader)
        .Include(x => x.FkPreferential)
        .Include(x => x.FkMoein)
            .ThenInclude(m => m.FkCol)
        .AsNoTracking()
        .ToList();
                }
                else
                {
                    int fr = 0;
                    int to = 0;
                    DateTime max = DateTime.MaxValue;

                    if (txtMoeinCode.Text != "")
                        fr = int.Parse(txtMoeinCode.Text);
                    if (txtPreferentialCode.Text != "")
                        to = int.Parse(txtPreferentialCode.Text);
                    if (txbCalender.Text != "")
                        min = pcw1.SelectedDate.ToDateTime();
                    if (txbCalender2.Text != "")
                        max = pcw2.SelectedDate.ToDateTime();

                    // داده اصلی
                    data = db.AcDocumentDetails
        .Include(x => x.FkAcDocHeader)
        .Include(x => x.FkPreferential)
        .Include(x => x.FkMoein)
            .ThenInclude(m => m.FkCol)
        .Where(t =>
        (fr == 0 || t.FkMoein.MoeinCode == fr) &&   // فقط وقتی MoeinCode فیلتر میشه که fr != 0
        (to == 0 || t.FkPreferential.PreferentialCode == to) && // همینطور برای PreferentialCode
        t.FkAcDocHeader.Date <= max &&
        t.FkAcDocHeader.Date >= min)
        .AsNoTracking()
        .ToList();
                }
                // داده‌های قبل از min برای مانده اول
                List<AcDocumentDetail> beforeData = new();
                if (min != DateTime.MinValue)
                {
                    beforeData = db.AcDocumentDetails
                        .Include(x => x.FkAcDocHeader)
                        .Include(x => x.FkPreferential)
                        .Include(x => x.FkMoein)
                            .ThenInclude(m => m.FkCol)
                        .Where(t => t.FkAcDocHeader.Date < min)
                        .AsNoTracking()
                        .ToList();
                }

                // گروه‌بندی: بر اساس ترکیب Preferential و Moein
                var groupedY = data
                    .Where(x => x.FkPreferential != null && x.FkMoein != null)
                    .GroupBy(x => new { PrefId = x.FkPreferential.Id, MoeinId = x.FkMoein.Id })
                    .ToDictionary(g => g.Key, g => g.ToList());

                var beforeGroupedY = beforeData
                    .Where(x => x.FkPreferential != null && x.FkMoein != null)
                    .GroupBy(x => new { PrefId = x.FkPreferential.Id, MoeinId = x.FkMoein.Id })
                    .ToDictionary(g => g.Key, g => g.ToList());

                // همه کلیدها (ترکیب‌های یکتا)
                var allKeysY = groupedY.Keys
                    .Union(beforeGroupedY.Keys)
                    .Distinct()
                    .ToList();

                foreach (var key in allKeysY)
                {
                    var anyRecord = groupedY.ContainsKey(key)
                        ? groupedY[key].First()
                        : beforeGroupedY[key].First();

                    var sumDebtor = groupedY.ContainsKey(key)
                        ? groupedY[key].Sum(x => x.Debtor)
                        : 0;

                    var sumCreditor = groupedY.ContainsKey(key)
                        ? groupedY[key].Sum(x => x.Creditor)
                        : 0;

                    var beforeSum = beforeGroupedY.ContainsKey(key)
                        ? beforeGroupedY[key].Sum(x => x.Debtor - x.Creditor)
                        : 0;
                    var preferential = new PreferentialAcReport
                    {
                        Id = Guid.NewGuid(),
                        FkMoein = anyRecord.FkMoein,
                        FkPreferential = anyRecord.FkPreferential,
                        SumDebtor = sumDebtor,
                        SumCreditor = sumCreditor,
                        BeforeSum = beforeSum,
                        moeinId = anyRecord.FkMoeinId
                    };
                    if (groupedY.ContainsKey(key))
                    {
                        preferential.acDocumentDetails = groupedY[key].OrderBy(y => y.FkAcDocHeader.Date).ToObservableCollection();
                        decimal runningCount = 0;
                        foreach (var item in preferential.acDocumentDetails)
                        {
                            runningCount += (item.Debtor ?? 0) - (item.Creditor ?? 0);
                            if (runningCount >= 0)
                            {
                                item.RunningSum = runningCount.ToString("#,##0");
                                item.Diagnosis = "بد";
                            }
                            else
                            {
                                item.RunningSum = $"{(-runningCount).ToString("#,##0")}";
                                item.Diagnosis = "بس";
                            }
                        }
                        preferential.acDocumentDetails.Insert(0, new AcDocumentDetail()
                        {
                            FkMoein = preferential.FkMoein,
                            FkPreferential = preferential.FkPreferential,
                            Description = "مانده از قبل",
                            Debtor = preferential.BeforeDebtor,
                            Creditor = preferential.BeforeCreditor,
                            RunningSum = (preferential.BeforeDebtor - preferential.BeforeCreditor >= 0) ? (preferential.BeforeDebtor - preferential.BeforeCreditor).Value.ToString("#,##0") :
                    $"{(-(preferential.BeforeDebtor - preferential.BeforeCreditor)).Value.ToString("#,##0")}",
                            Diagnosis = (preferential.BeforeDebtor - preferential.BeforeCreditor >= 0) ? "بد" : "بس"
                        });
                        PreferentialAcReportEntities.Add(preferential);
                    }
                }


                System.IO.File.WriteAllLines(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "reportOption.txt"), new string[]
                    {
             txbCalender.Text == "" ? "ابتدای دوره" : txbCalender.Text,
            txbCalender2.Text == "" ? "انتهای دوره" : txbCalender2.Text
                });

                Mouse.OverrideCursor = null;
            }
            catch (Exception ex)
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
            var sfDataGrid = control.SelectedContent as Syncfusion.UI.Xaml.Grid.SfDataGrid;
            if (sfDataGrid != null)
            {
                try
                {
                    if (SearchTermTextBox.Text.Trim() == "")
                        sfDataGrid.SearchHelper.ClearSearch();
                    else
                        sfDataGrid.SearchHelper.Search(SearchTermTextBox.Text);
                }
                catch (Exception ex)
                {
                }
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
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "صورتحساب");
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
            PreferentialAcReportEntities.Clear();
            datagridPreferential.Dispose();
            datagridِDetails.Dispose();
            dataPager4.Dispose();
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
                    dataPager4.Visibility = Visibility.Visible;
                    datagridPreferential.SearchHelper.AllowFiltering = true;
                    try
                    {
                        dataPager4.Source = null;
                    }
                    catch { }
                    try
                    {
                        dataPager4.Source = PreferentialAcReportEntities;
                    }
                    catch { }
                    break;
                case 1:
                    dataPager4.Visibility = Visibility.Collapsed;
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
            if (datagridF.SelectedItem is PreferentialAcReport baseBrowseAccounts)
            {
                datagridF.SearchHelper.AllowFiltering = true;
                try
                {
                    dataPager5.Source = null;
                }
                catch { }
                try
                {                    
                    dataPager5.Source = baseBrowseAccounts.acDocumentDetails;
                }
                catch { }
                control.SelectionChanged -= control_SelectionChanged;
                control.SelectedIndex = 1;
                dataPager4.Visibility = Visibility.Collapsed;
                dataPager5.Visibility = Visibility.Visible;
                control.SelectionChanged += control_SelectionChanged;
                try
                {
                    datagridِDetails.SortColumnDescriptions.Clear();
                    datagridِDetails.SortColumnDescriptions.Add(new Syncfusion.UI.Xaml.Grid.SortColumnDescription()
                    {
                        ColumnName = "FkAcDocHeader.Date",
                        SortDirection = System.ComponentModel.ListSortDirection.Ascending
                    });
                    datagridِDetails.SortColumnDescriptions.Add(new Syncfusion.UI.Xaml.Grid.SortColumnDescription()
                    {
                        ColumnName = "FkAcDocHeader.NoDoument",
                        SortDirection = System.ComponentModel.ListSortDirection.Ascending
                    });
                } catch { }
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

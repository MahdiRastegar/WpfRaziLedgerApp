using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Syncfusion.CompoundFile.XlsIO.Native;
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
using System.Reflection;
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
    public partial class usrElectronicOO : UserControl, ITabForm
    {
        public usrElectronicOO()
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

        public ObservableCollection<ColElReport> colElReports { get; set; }
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if(cmbAction.SelectedIndex==-1)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("لطفا ماه را انتخاب کنید");
                return;
            }
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                using var db = new wpfrazydbContext();
                PersianCalendar pc = new();

                List<AcDocumentDetail> data = null;
                if (colElReports != null)
                    colElReports.Clear();
                colElReports = new ObservableCollection<ColElReport>();
                // شروع سال 1404
                DateTime start = pc.ToDateTime(MainWindow.StatusOptions.Period.Value, 1, 1, 0, 0, 0, 0);

                // حداقل (با ماه انتخاب شده)
                DateTime min = pc.AddMonths(start, cmbAction.SelectedIndex + 0);

                // حداکثر (ماه بعدی)
                DateTime max = pc.AddMonths(start, cmbAction.SelectedIndex + 1).AddDays(-1);

                // جمع داده‌ها طبق شرایط کاربر
                data = db.AcDocumentDetails
                        .Include(x => x.FkAcDocHeader)
                        .Include(x => x.FkPreferential)
                        .Where(t => t.FkAcDocHeader.Date <= max &&
                                    t.FkAcDocHeader.Date >= min)
                        .Include(u => u.FkMoein)
                            .ThenInclude(w => w.FkCol)
                        .AsNoTracking()
                        .ToList();
                // داده‌های قبل از min
                List<AcDocumentDetail> beforeData = new();                

                // گروه‌بندی داده‌ها
                var grouped = data
                    .Where(y => y.FkMoein != null && y.FkMoein.FkCol != null)
                    .GroupBy(y => y.FkMoein.FkCol.Id)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var beforeGrouped = beforeData
                    .Where(y => y.FkMoein != null && y.FkMoein.FkCol != null)
                    .GroupBy(y => y.FkMoein.FkCol.Id)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // مجموع همه کلیدهای موجود
                var allKeys = grouped.Keys
                    .Union(beforeGrouped.Keys)
                    .Distinct()
                    .ToList();

                // تولید گزارش نهایی
                colElReports.Clear();
                foreach (var colId in allKeys)
                {
                    var anyRecord = grouped.ContainsKey(colId) ? grouped[colId].First() :
                                    beforeGrouped[colId].First();

                    var colCode = anyRecord.FkMoein.FkCol.ColCode;
                    var colName = anyRecord.FkMoein.FkCol.ColName;

                    decimal? sumDebtor = grouped.ContainsKey(colId) ? grouped[colId].Sum(x => x.Debtor) : 0;
                    decimal? sumCreditor = grouped.ContainsKey(colId) ? grouped[colId].Sum(x => x.Creditor) : 0;
                    decimal? beforeSum = beforeGrouped.ContainsKey(colId) ?
                                     beforeGrouped[colId].Sum(x => x.Debtor - x.Creditor) : 0;
                    colElReports.Add(new ColElReport
                    {
                        Date=max,
                        Id = colId,
                        ColCode = colCode,
                        ColName = colName,
                        SumDebtor = (sumDebtor- sumCreditor>=0)? sumDebtor - sumCreditor:0,
                        SumCreditor = (sumCreditor - sumDebtor >= 0) ? sumCreditor - sumDebtor : 0,
                        AgroupId = anyRecord.FkMoein.FkCol.FkGroupId
                        //acDocumentDetails = grouped[colId].ToObservableCollection()
                    });
                }
                colElReports = colElReports.OrderByDescending(w => w.SumDebtor).ToObservableCollection();
                var row = 0;
                colElReports.ForEach(w =>
                {
                    row++;
                    w.Row = row;
                });
                datagrid.SearchHelper.AllowFiltering = true;
                try
                {
                    dataPager.Source = null;
                }
                catch { }
                try
                {
                    dataPager.Source = new ObservableCollection<ColAcReport>();
                }
                catch (Exception ex) { }
                try
                {
                    dataPager.Source = colElReports;
                }
                catch (Exception ex) { }

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
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "خروجی دفتر الکترونیکی");
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
            colElReports.Clear();
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


        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            if (colElReports == null || colElReports.Count == 0)
                return;
            // مسیر پوشه کنار فایل اجرایی
            string exePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string folderPath = System.IO.Path.Combine(exePath, "ExcelElectronicReport");

            // اگر پوشه نبود ساخته بشه
            if (!System.IO.Directory.Exists(folderPath))
                System.IO.Directory.CreateDirectory(folderPath);

            // نام پایه فایل
            string baseFileName = "خروجی دفتر الکترونیکی.xlsx";
            string filePath = System.IO.Path.Combine(folderPath, baseFileName);

            // اگر فایل موجود بود اندیس بخوره
            int counter = 1;
            while (System.IO.File.Exists(filePath))
            {
                string fileName = $"خروجی دفتر الکترونیکی ({counter}).xlsx";
                filePath = System.IO.Path.Combine(folderPath, fileName);
                counter++;
            }

            Mouse.OverrideCursor = Cursors.Wait;
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("گزارش");

                // هدر ستون‌ها
                worksheet.Cell(1, 1).Value = "ردیف";
                worksheet.Cell(1, 2).Value = "تاریخ";
                worksheet.Cell(1, 3).Value = "کد حساب کل";
                worksheet.Cell(1, 4).Value = "عنوان حساب کل";
                worksheet.Cell(1, 5).Value = "مبلغ بدهکار (ریال)";
                worksheet.Cell(1, 6).Value = "مبلغ بستانکار (ریال)";

                int row = 2;
                foreach (var item in colElReports)
                {
                    worksheet.Cell(row, 1).Value = item.Row;
                    worksheet.Cell(row, 2).Value = item.Date.ToString("yyyy/MM/dd");
                    worksheet.Cell(row, 3).Value = item.ColCode;
                    worksheet.Cell(row, 4).Value = item.ColName;
                    worksheet.Cell(row, 5).Value = item.SumDebtor;
                    worksheet.Cell(row, 6).Value = item.SumCreditor;
                    row++;
                }

                // استایل ساده
                worksheet.Columns().AdjustToContents();
                worksheet.Row(1).Style.Font.Bold = true;

                workbook.SaveAs(filePath);
            }
            Mouse.OverrideCursor = null;
            Xceed.Wpf.Toolkit.MessageBox.Show("فابل با موفقیت ذخیره شد");
            // باز کردن فایل بعد از ذخیره
            try
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("فایل باز نشد");
            }

        }

        private void cmAction_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void cmbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

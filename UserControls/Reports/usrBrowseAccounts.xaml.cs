using Microsoft.EntityFrameworkCore;
using Stimulsoft.Report;
using Syncfusion.Data.Extensions;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
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

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for usrBrowseAccounts.xaml
    /// </summary>
    public partial class usrBrowseAccounts : UserControl
    {
        public usrBrowseAccounts()
        {
            InitializeComponent();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (BrowseAccountsEntities?.Count > 0)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var report = new StiReport();
                report.Load(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports","MRT","Report.mrt")); // قالب
                report.RegBusinessObject("GAcClass", BrowseAccountsEntities); // اتصال داده‌ها
                report.Render();
                report.ShowWithWpf();
                Mouse.OverrideCursor = null;
            }
        }

        public ObservableCollection<GAcClass> BrowseAccountsEntities { get; set; }
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            BrowseAccountsEntities = new ObservableCollection<GAcClass>();
            using var db = new wpfrazydbContext();

            List<AcDocumentDetail> data=null;
            if (txtFromDoc.Text == "" && txtToDoc.Text == "" && txbCalender.Text == "" && txbCalender2.Text == "")
            {
                data = db.AcDocumentDetails
    .Include(u => u.FkMoein)
        .ThenInclude(w => w.FkCol)
        .ThenInclude(w => w.FkGroup)
    .AsNoTracking()
    .ToList();
            }
            else
            {
                long fr = 0;
                long to = long.MaxValue;
                DateTime min = DateTime.MinValue;
                DateTime max = DateTime.MaxValue;

                if (txtFromDoc.Text != "")
                    fr = long.Parse(txtFromDoc.Text);
                if (txtToDoc.Text != "")
                    to = long.Parse(txtToDoc.Text);
                if (txbCalender.Text != "")
                    min = pcw1.SelectedDate.ToDateTime();
                if (txbCalender2.Text != "")
                    max = pcw2.SelectedDate.ToDateTime();

                data=db.AcDocumentDetails.Include(u => u.FkAcDocHeader).Where(t => t.FkAcDocHeader.NoDoument >= fr && t.FkAcDocHeader.NoDoument <= to && t.FkAcDocHeader.Date <= max && t.FkAcDocHeader.Date >= min)
   .Include(u => u.FkMoein)
       .ThenInclude(w => w.FkCol)
       .ThenInclude(w => w.FkGroup)
   .AsNoTracking()
   .ToList();
            }

            var grouped = data
    .Where(y => y.FkMoein != null &&
                y.FkMoein.FkCol != null &&
                y.FkMoein.FkCol.FkGroup != null)
     .GroupBy(y => y.FkMoein.FkCol.FkGroup.Id)
    .ToList();
            //var h = db.Moeins.Take(10).ToList();
            //if(count>10)
            //{
            //    for (int i = 0; i < count-10; i++)
            //    {
            //        h.Add(null);
            //    }
            //}
            grouped.ForEach(u => BrowseAccountsEntities.Add(
                new GAcClass()
                {
                    Id=Guid.NewGuid(),
                    GroupCode = u.First().FkMoein.FkCol.FkGroup.GroupCode,
                    GroupName = u.First().FkMoein.FkCol.FkGroup.GroupName,
                    SumDebtor = u.Sum(w => w.Debtor),
                    SumCreditor = u.Sum(w => w.Creditor)
                }));
            datagrid.SearchHelper.AllowFiltering = true;
            dataPager.Source = null;
            dataPager.Source = BrowseAccountsEntities;
            //var options = new JsonSerializerOptions
            //{
            //    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            //    WriteIndented = true
            //};
            //string jsonString = JsonSerializer.Serialize(BrowseAccountsEntities, options);
            //System.IO.File.WriteAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.json"), jsonString);
            Mouse.OverrideCursor = null;
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
            datagrid.AllowFiltering = !datagrid.AllowFiltering;
            if (!datagrid.AllowFiltering)
                datagrid.ClearFilters();
        }
        private void datagrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // ارتفاع سطرهای grid را محاسبه کنید (می‌توانید ارتفاع سطر ثابت فرض کنید)
            double rowHeight = 30; // ارتفاع هر سطر (این مقدار ممکن است بسته به طراحی تغییر کند)

            // ارتفاع موجود در grid را محاسبه کنید
            double availableHeight = datagrid.ActualHeight;

            // محاسبه تعداد سطرهایی که در صفحه جا می‌شوند
            int visibleRows = (int)(availableHeight / rowHeight);

            // تنظیم PageSize بر اساس تعداد سطرهای محاسبه شده
            if (visibleRows > 0)
            {
                dataPager.PageSize = visibleRows - 2;
                var g = dataPager.Source;
                dataPager.Source = null;
                dataPager.Source = g;
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
    }
}

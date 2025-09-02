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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for usrBrowseAccounts.xaml
    /// </summary>
    public partial class usrBrowseAccounts : UserControl, ITabForm
    {
        public usrBrowseAccounts()
        {
            InitializeComponent();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {            
            switch (control.SelectedIndex)
            {
                case 0:
                    if (GAcClassEntities?.Count > 0)
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
                        string jsonString = JsonSerializer.Serialize(GAcClassEntities, options);
                        System.IO.File.WriteAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "JSON", "GAcClass.json"), jsonString);
                        Mouse.OverrideCursor = null;                        

                        Process process = new Process();
                        process.StartInfo.FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "WpfAppEmpty.exe");
                        //process.StartInfo.Arguments = $"\"{reportPath}\" \"{outputPdf}\"";
                        process.StartInfo.UseShellExecute = false;
                        process.Start();
                    }
                    break;                    
                case 1:
                    if (ColAcReportEntities?.Count > 0&& dataPager2.Source is IEnumerable<ColAcReport> source&&source.Count()>0)
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
                        string jsonString = JsonSerializer.Serialize(source, options);
                        System.IO.File.WriteAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "JSON", "ColAcReport.json"), jsonString);
                        Mouse.OverrideCursor = null;
                        
                        Process process = new Process();
                        process.StartInfo.FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "WpfAppEmpty.exe");
                        //process.StartInfo.Arguments = $"\"{reportPath}\" \"{outputPdf}\"";
                        process.StartInfo.UseShellExecute = false;
                        process.Start();
                    }
                    break;                    
                case 2:
                    if (MoeinAcReportEntities?.Count > 0 && dataPager3.Source is IEnumerable<MoeinAcReport> source2 && source2.Count() > 0)
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
                        string jsonString = JsonSerializer.Serialize(source2, options);
                        System.IO.File.WriteAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "JSON", "MoeinAcReport.json"), jsonString);
                        Mouse.OverrideCursor = null;
                        Process process = new Process();
                        process.StartInfo.FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "WpfAppEmpty.exe");
                        //process.StartInfo.Arguments = $"\"{reportPath}\" \"{outputPdf}\"";
                        process.StartInfo.UseShellExecute = false;
                        process.Start();
                    }
                    break;                    
                case 3:
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
                        System.IO.File.WriteAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "JSON", "PreferentialAcReport.json"), jsonString);
                        Mouse.OverrideCursor = null;
                        Process process = new Process();
                        process.StartInfo.FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "WpfAppEmpty.exe");
                        //process.StartInfo.Arguments = $"\"{reportPath}\" \"{outputPdf}\"";
                        process.StartInfo.UseShellExecute = false;
                        process.Start();
                    }
                    break;                    
                case 4:
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
                            System.IO.File.WriteAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "JSON", "AcDocumentDetail.json"), jsonString);
                            Mouse.OverrideCursor = null;
                            Process process = new Process();
                            process.StartInfo.FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "WpfAppEmpty.exe");
                            //process.StartInfo.Arguments = $"\"{reportPath}\" \"{outputPdf}\"";
                            process.StartInfo.UseShellExecute = false;
                            process.Start();
                        }
                    }
                    break;
            }
        }

    public ObservableCollection<GAcClass> GAcClassEntities { get; set; }
        public ObservableCollection<MoeinAcReport> MoeinAcReportEntities { get; set; }
        public ObservableCollection<PreferentialAcReport> PreferentialAcReportEntities { get; set; }
        public ObservableCollection<ColAcReport> ColAcReportEntities { get; set; }
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                using var db = new wpfrazydbContext();

                List<AcDocumentDetail> data = null;

                if (GAcClassEntities != null)
                    GAcClassEntities.Clear();
                GAcClassEntities = new ObservableCollection<GAcClass>();
                if (txtFromDoc.Text == "" && txtToDoc.Text == "" && txbCalender.Text == "" && txbCalender2.Text == "")
                {
                    data = db.AcDocumentDetails
                        .Include(x => x.FkAcDocHeader)
                        .Include(x => x.FkPreferential)
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
                    DateTime minx = DateTime.MinValue;
                    DateTime max = DateTime.MaxValue;

                    if (txtFromDoc.Text != "")
                        fr = long.Parse(txtFromDoc.Text);
                    if (txtToDoc.Text != "")
                        to = long.Parse(txtToDoc.Text);
                    if (txbCalender.Text != "")
                        minx = pcw1.SelectedDate.ToDateTime();
                    if (txbCalender2.Text != "")
                        max = pcw2.SelectedDate.ToDateTime();

                    data = db.AcDocumentDetails.Include(u => u.FkAcDocHeader).Where(t => t.FkAcDocHeader.NoDoument >= fr && t.FkAcDocHeader.NoDoument <= to && t.FkAcDocHeader.Date <= max && t.FkAcDocHeader.Date >= minx)
                        .Include(x => x.FkAcDocHeader)
                        .Include(x => x.FkPreferential)
       .Include(u => u.FkMoein)
           .ThenInclude(w => w.FkCol)
           .ThenInclude(w => w.FkGroup)
       .AsNoTracking()
       .ToList();
                }

                var groupedX = data
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
                groupedX.ForEach(u => GAcClassEntities.Add(
                    new GAcClass()
                    {
                        Id = u.First().FkMoein.FkCol.FkGroup.Id,
                        GroupCode = u.First().FkMoein.FkCol.FkGroup.GroupCode,
                        GroupName = u.First().FkMoein.FkCol.FkGroup.GroupName,
                        SumDebtor = u.Sum(w => w.Debtor),
                        SumCreditor = u.Sum(w => w.Creditor),                        
                        //acDocumentDetails = u.ToObservableCollection()
                    }));
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
                        dataPager.Source = new ObservableCollection<GAcClass>();
                    }
                    catch (Exception ex) { }
                    try
                    {
                        dataPager.Source = GAcClassEntities;
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

                DateTime min = DateTime.MinValue;
                if (ColAcReportEntities != null)
                    ColAcReportEntities.Clear();
                ColAcReportEntities = new ObservableCollection<ColAcReport>();
                if (txtFromDoc.Text == "" && txtToDoc.Text == "" && txbCalender.Text == "" && txbCalender2.Text == "")
                {
                    data = db.AcDocumentDetails
                        .Include(x => x.FkAcDocHeader)
                        .Include(x => x.FkPreferential)
        .Include(u => u.FkMoein)
            .ThenInclude(w => w.FkCol)
        .AsNoTracking()
        .ToList();
                }
                else
                {
                    long fr = 0;
                    long to = long.MaxValue;
                    DateTime max = DateTime.MaxValue;

                    if (txtFromDoc.Text != "")
                        fr = long.Parse(txtFromDoc.Text);
                    if (txtToDoc.Text != "")
                        to = long.Parse(txtToDoc.Text);
                    if (txbCalender.Text != "")
                        min = pcw1.SelectedDate.ToDateTime();
                    if (txbCalender2.Text != "")
                        max = pcw2.SelectedDate.ToDateTime();

                    // جمع داده‌ها طبق شرایط کاربر
                    data = db.AcDocumentDetails
                        .Include(x => x.FkAcDocHeader)
                        .Include(x => x.FkPreferential)
                        .Where(t => t.FkAcDocHeader.NoDoument >= fr &&
                                    t.FkAcDocHeader.NoDoument <= to &&
                                    t.FkAcDocHeader.Date <= max &&
                                    t.FkAcDocHeader.Date >= min)
                        .Include(u => u.FkMoein)
                            .ThenInclude(w => w.FkCol)
                        .AsNoTracking()
                        .ToList();
                }
                // داده‌های قبل از min
                List<AcDocumentDetail> beforeData = new();
                if (min != DateTime.MinValue)
                {
                    beforeData = db.AcDocumentDetails
                        .Include(t => t.FkAcDocHeader)
                        .Include(t => t.FkMoein)
                            .ThenInclude(w => w.FkCol)
                        .Where(t => t.FkAcDocHeader.Date < min)
                        .AsNoTracking()
                        .ToList();
                }

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
                ColAcReportEntities.Clear();

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

                    ColAcReportEntities.Add(new ColAcReport
                    {
                        Id = colId,
                        ColCode = colCode,
                        ColName = colName,
                        SumDebtor = sumDebtor,
                        SumCreditor = sumCreditor,
                        BeforeSum = beforeSum,
                        AgroupId = anyRecord.FkMoein.FkCol.FkGroupId
                        //acDocumentDetails = grouped[colId].ToObservableCollection()
                    });
                }
                if (control.SelectedIndex == 1)
                {
                    datagridCol.SearchHelper.AllowFiltering = true;
                    try
                    {
                        dataPager2.Source = null;
                    }
                    catch { }
                    try
                    {
                        dataPager2.Source = new ObservableCollection<ColAcReport>();
                    }
                    catch (Exception ex) { }
                    try
                    {
                        dataPager2.Source = ColAcReportEntities;
                    }
                    catch (Exception ex) { }
                    //datagridCol.ItemsSource=dataPager.Source;
                }

                if (MoeinAcReportEntities != null)
                    MoeinAcReportEntities.Clear();
                MoeinAcReportEntities = new ObservableCollection<MoeinAcReport>();
                if (txtFromDoc.Text == "" && txtToDoc.Text == "" && txbCalender.Text == "" && txbCalender2.Text == "")
                {
                    data = db.AcDocumentDetails
                        .Include(x => x.FkAcDocHeader)
                        .Include(x => x.FkPreferential)
        .Include(u => u.FkMoein)
            .ThenInclude(w => w.FkCol)
        .AsNoTracking()
        .ToList();
                }
                else
                {
                    long fr = 0;
                    long to = long.MaxValue;
                    DateTime max = DateTime.MaxValue;

                    if (txtFromDoc.Text != "")
                        fr = long.Parse(txtFromDoc.Text);
                    if (txtToDoc.Text != "")
                        to = long.Parse(txtToDoc.Text);
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
                        .Where(t => t.FkAcDocHeader.NoDoument >= fr &&
                                    t.FkAcDocHeader.NoDoument <= to &&
                                    t.FkAcDocHeader.Date <= max &&
                                    t.FkAcDocHeader.Date >= min)
                        .AsNoTracking()
                        .ToList();
                }

                // داده‌های قبل از بازه برای مانده اول
                beforeData = new();
                if (min != DateTime.MinValue)
                {
                    beforeData = db.AcDocumentDetails
                        .Include(x => x.FkAcDocHeader)
                        .Include(x => x.FkMoein)
                            .ThenInclude(m => m.FkCol)
                        .Where(t => t.FkAcDocHeader.Date < min)
                        .AsNoTracking()
                        .ToList();
                }

                // گروه‌بندی داده‌ها بر اساس MoeinId
                grouped = data
                    .Where(y => y.FkMoein != null && y.FkMoein.FkCol != null)
                    .GroupBy(y => y.FkMoein.Id)
                    .ToDictionary(g => g.Key, g => g.ToList());

                beforeGrouped = beforeData
                    .Where(y => y.FkMoein != null && y.FkMoein.FkCol != null)
                    .GroupBy(y => y.FkMoein.Id)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // کلیدهای یکتا از هر دو
                allKeys = grouped.Keys
                    .Union(beforeGrouped.Keys)
                    .Distinct()
                    .ToList();

                foreach (var moeinId in allKeys)
                {
                    var anyRecord = grouped.ContainsKey(moeinId)
                        ? grouped[moeinId].First()
                        : beforeGrouped[moeinId].First();

                    var moeinCode = anyRecord.FkMoein.MoeinCode;
                    var moeinName = anyRecord.FkMoein.MoeinName;
                    var colCode = anyRecord.FkMoein.FkCol.ColCode;
                    var colName = anyRecord.FkMoein.FkCol.ColName;

                    var sumDebtor = grouped.ContainsKey(moeinId)
                        ? grouped[moeinId].Sum(x => x.Debtor)
                        : 0;

                    var sumCreditor = grouped.ContainsKey(moeinId)
                        ? grouped[moeinId].Sum(x => x.Creditor)
                        : 0;

                    var beforeSum = beforeGrouped.ContainsKey(moeinId)
                        ? beforeGrouped[moeinId].Sum(x => x.Debtor - x.Creditor)
                        : 0;

                    MoeinAcReportEntities.Add(new MoeinAcReport
                    {
                        Id = moeinId,
                        MoeinCode = moeinCode,
                        MoeinName = moeinName,
                        ColCode = colCode,
                        ColName = colName,
                        SumDebtor = sumDebtor,
                        SumCreditor = sumCreditor,
                        BeforeSum = beforeSum,
                        colId= anyRecord.FkMoein.FkColId
                        //acDocumentDetails = grouped[moeinId].ToObservableCollection()
                    });
                }
                if (control.SelectedIndex == 2)
                {
                    datagridMoein.SearchHelper.AllowFiltering = true;
                    try
                    {
                        dataPager3.Source = null;
                    }
                    catch { }
                    try
                    {
                        dataPager3.Source = new ObservableCollection<MoeinAcReport>();
                    }
                    catch (Exception ex) { }
                    try
                    {
                        dataPager3.Source = MoeinAcReportEntities;
                    }
                    catch (Exception ex) { }
                    //datagridMoein.ItemsSource=dataPager.Source;
                }


                if (PreferentialAcReportEntities != null)
                    PreferentialAcReportEntities.Clear();
                PreferentialAcReportEntities = new ObservableCollection<PreferentialAcReport>();
                if (txtFromDoc.Text == "" && txtToDoc.Text == "" && txbCalender.Text == "" && txbCalender2.Text == "")
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
                    long fr = 0;
                    long to = long.MaxValue;
                    DateTime max = DateTime.MaxValue;

                    if (txtFromDoc.Text != "")
                        fr = long.Parse(txtFromDoc.Text);
                    if (txtToDoc.Text != "")
                        to = long.Parse(txtToDoc.Text);
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
        .Where(t => t.FkAcDocHeader.NoDoument >= fr &&
                    t.FkAcDocHeader.NoDoument <= to &&
                    t.FkAcDocHeader.Date <= max &&
                    t.FkAcDocHeader.Date >= min)
        .AsNoTracking()
        .ToList();
                }
                // داده‌های قبل از min برای مانده اول
                beforeData = new();
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
                        moeinId=anyRecord.FkMoeinId
                    };
                    if (groupedY.ContainsKey(key))
                        preferential.acDocumentDetails = groupedY[key].OrderBy(y => y.FkAcDocHeader.Date).ToObservableCollection();
                    decimal runningCount = 0;
                    foreach (var item in preferential.acDocumentDetails)
                    {
                        runningCount += (item.Debtor??0)-(item.Creditor??0);
                        if (runningCount >= 0)
                            item.RunningSum = runningCount.ToString("#,##0");
                        else
                            item.RunningSum = $"({(-runningCount).ToString("#,##0")})";
                    }

                    PreferentialAcReportEntities.Add(preferential);
                }

                if (control.SelectedIndex == 3)
                {
                    datagridPreferential.SearchHelper.AllowFiltering = true;
                    try
                    {
                        dataPager4.Source = null;
                    }
                    catch { }

                    try
                    {
                        dataPager4.Source = new ObservableCollection<PreferentialAcReport>();
                    }
                    catch (Exception ex) { }
                    try
                    {
                        dataPager4.Source = PreferentialAcReportEntities;
                    }
                    catch (Exception ex) { }
                    //datagridPreferential.ItemsSource=dataPager.Source;
                }

                System.IO.File.WriteAllLines(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfSimReport", "reportOption.txt"), new string[]
                {
                txtFromDoc.Text == "" ? "اول" : txtFromDoc.Text,
            txtToDoc.Text == "" ? "آخر" : txtToDoc.Text,
             txbCalender.Text == "" ? "ابتدای دوره" : txbCalender.Text,
            txbCalender2.Text == "" ? "انتهای دوره" : txbCalender2.Text
            });

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
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "مرور حساب ها");
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
            GAcClassEntities.Clear();
            ColAcReportEntities.Clear();
            MoeinAcReportEntities.Clear();
            PreferentialAcReportEntities.Clear();
            datagrid.Dispose();
            datagridCol.Dispose();
            datagridMoein.Dispose();
            datagridPreferential.Dispose();
            datagridِDetails.Dispose();
            dataPager.Dispose();
            dataPager2.Dispose();
            dataPager3.Dispose();
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
                    dataPager4.Visibility = Visibility.Collapsed;
                    dataPager2.Visibility = Visibility.Collapsed;
                    dataPager3.Visibility = Visibility.Collapsed;
                    dataPager.Visibility = Visibility.Visible;
                    datagrid.SearchHelper.AllowFiltering = true;
                    try
                    {
                        dataPager.Source = null;
                    }
                    catch { }
                    try
                    {
                        dataPager.Source = GAcClassEntities;
                    }
                    catch { }
                    break;
                case 1:
                    dataPager5.Visibility = Visibility.Collapsed;
                    dataPager4.Visibility = Visibility.Collapsed;
                    dataPager.Visibility = Visibility.Collapsed;
                    dataPager3.Visibility = Visibility.Collapsed;
                    dataPager2.Visibility = Visibility.Visible;
                    datagridCol.SearchHelper.AllowFiltering = true;
                    try
                    {
                        dataPager2.Source = null;
                    }
                    catch { }
                    try
                    {
                        dataPager2.Source = ColAcReportEntities;
                    }
                    catch { }
                    break;
                case 2:
                    dataPager5.Visibility = Visibility.Collapsed;
                    dataPager4.Visibility = Visibility.Collapsed;
                    dataPager.Visibility = Visibility.Collapsed;
                    dataPager2.Visibility = Visibility.Collapsed;
                    dataPager3.Visibility = Visibility.Visible;
                    datagridMoein.SearchHelper.AllowFiltering = true;
                    try
                    {
                        dataPager3.Source = null;
                    }
                    catch { }
                    try
                    {
                        dataPager3.Source = MoeinAcReportEntities;
                    }
                    catch { }
                    break;
                case 3:
                    dataPager5.Visibility = Visibility.Collapsed;
                    dataPager3.Visibility = Visibility.Collapsed;
                    dataPager.Visibility = Visibility.Collapsed;
                    dataPager2.Visibility = Visibility.Collapsed;
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
                case 4:
                    dataPager4.Visibility = Visibility.Collapsed;
                    dataPager3.Visibility = Visibility.Collapsed;
                    dataPager.Visibility = Visibility.Collapsed;
                    dataPager2.Visibility = Visibility.Collapsed;
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
            if (datagridF.SelectedItem is GAcClass gAcClass)
            {
                datagridCol.SearchHelper.AllowFiltering = true;
                try
                {
                    dataPager2.Source = null;
                }
                catch { }
                try
                {
                    dataPager2.Source = new ObservableCollection<ColAcReport>();
                }
                catch (Exception ex) { }
                try
                {
                    dataPager2.Source = ColAcReportEntities.Where(u=>u.AgroupId==gAcClass.Id);
                }
                catch (Exception ex) { }

                control.SelectionChanged -= control_SelectionChanged;
                control.SelectedIndex = 1;
                dataPager4.Visibility = Visibility.Collapsed;
                dataPager3.Visibility = Visibility.Collapsed;
                dataPager2.Visibility = Visibility.Visible;
                dataPager2.Visibility = Visibility.Collapsed;
                dataPager5.Visibility = Visibility.Collapsed;
                control.SelectionChanged += control_SelectionChanged;
            }
            else if (datagridF.SelectedItem is ColAcReport colAcReport)
            {
                datagridMoein.SearchHelper.AllowFiltering = true;
                try
                {
                    dataPager3.Source = null;
                }
                catch { }
                try
                {
                    dataPager3.Source = new ObservableCollection<MoeinAcReport>();
                }
                catch (Exception ex) { }
                try
                {
                    dataPager3.Source = MoeinAcReportEntities.Where(u => u.colId == colAcReport.Id);
                }
                catch (Exception ex) { }

                control.SelectionChanged -= control_SelectionChanged;
                control.SelectedIndex = 2;
                dataPager4.Visibility = Visibility.Collapsed;
                dataPager3.Visibility = Visibility.Visible;
                dataPager.Visibility = Visibility.Collapsed;
                dataPager2.Visibility = Visibility.Collapsed;
                dataPager2.Visibility = Visibility.Collapsed;
                dataPager5.Visibility = Visibility.Collapsed;
                control.SelectionChanged += control_SelectionChanged;
            }
            else if (datagridF.SelectedItem is MoeinAcReport moeinAcReport)
            {
                datagridPreferential.SearchHelper.AllowFiltering = true;
                try
                {
                    dataPager4.Source = null;
                }
                catch { }
                try
                {
                    dataPager4.Source = new ObservableCollection<PreferentialAcReport>();
                }
                catch (Exception ex) { }
                try
                {
                    dataPager4.Source = PreferentialAcReportEntities.Where(u => u.moeinId == moeinAcReport.Id);
                }
                catch (Exception ex) { }

                control.SelectionChanged -= control_SelectionChanged;
                control.SelectedIndex = 3;
                dataPager4.Visibility = Visibility.Visible;
                dataPager3.Visibility = Visibility.Collapsed;
                dataPager2.Visibility = Visibility.Collapsed;
                dataPager2.Visibility = Visibility.Collapsed;
                dataPager.Visibility = Visibility.Collapsed;
                dataPager5.Visibility = Visibility.Collapsed;
                control.SelectionChanged += control_SelectionChanged;
            }
            else if (datagridF.SelectedItem is PreferentialAcReport baseBrowseAccounts)
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
                control.SelectedIndex = 4;
                dataPager4.Visibility = Visibility.Collapsed;
                dataPager3.Visibility = Visibility.Collapsed;
                dataPager.Visibility = Visibility.Collapsed;
                dataPager2.Visibility = Visibility.Collapsed;
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

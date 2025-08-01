using Mahdi.PersianDateControls;
using Microsoft.EntityFrameworkCore;
using PersianCalendarWPF;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.Windows.Controls.Input;
using Syncfusion.Windows.Shared;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfRaziLedgerApp.Interfaces;
using WpfRaziLedgerApp.Windows.toolWindows;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using static ClosedXML.Excel.XLPredefinedFormat;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for winCol.xaml
    /// </summary>
    public partial class usrAccountDocument : UserControl,ITabForm,ITabEdidGrid,IDisposable
    {
        public bool DataGridIsFocused
        {
            get
            {
                return datagrid.IsFocused;
            }
        }
        AcDocumentViewModel acDocumentViewModel;
        List<Mu> mus1 = new List<Mu>();
        List<Mu> mus2 = new List<Mu>();
        public usrAccountDocument()
        {
            AcDocumentDetails = new ObservableCollection<AcDocumentDetail>();
            AcDocumentHeaders = new ObservableCollection<AcDocumentHeader>();
            InitializeComponent();
            acDocumentViewModel = Resources["viewmodel"] as AcDocumentViewModel;
            acDocumentViewModel.AcDocumentDetails.CollectionChanged += AcDocumentDetails_CollectionChanged;
            txbCalender.Text = pcw1.SelectedDate.ToString();
        }

        public void Dispose()
        {
            if (acDocumentViewModel == null)
                return;
            AcDocumentHeaders.Clear();
            AcDocumentDetails.Clear();
            datagridSearch.Dispose();
            dataPager.Dispose();
            DataContext = null;
            acDocumentViewModel.AcDocumentDetails.CollectionChanged -= AcDocumentDetails_CollectionChanged;
            acDocumentViewModel = null;
            GC.Collect();
        }

        Brush brush = null;
        public ObservableCollection<AcDocumentDetail> AcDocumentDetails { get; set; }
        public ObservableCollection<AcDocumentHeader> AcDocumentHeaders { get; set; }
        private void Txt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "\r")
            {
                if ((sender as TextBox).Name == "txtNoDocumen")
                {
                    cmbType.Focus();
                }
                else
                {
                    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                    request.Wrapped = true;
                    (sender as TextBox).MoveFocus(request);
                }
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (btnConfirm.IsFocused)
                    {
                        btnConfirm_Click(null, null);
                    }
                }));
                return;
            }            
            if ((sender as TextBox).Name != "cmbType")
                e.Handled = !IsTextAllowed(e.Text);            
        }
        private static readonly Regex _regex = new Regex("[^0-9]"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        bool AddedMode = true;
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using var db = new wpfrazydbContext();
            var temp = cmbType.SelectedIndex;
            cmbType.ItemsSource = db.DocumentTypes.Where(y => y.IsManual).ToList();
            mus1.Clear();
            mus2.Clear();
            var moeins = db.Moeins.Include("FkCol").ToList();
            var preferentials = db.Preferentials.Include("FkGroup").ToList();
            foreach (var item in moeins)
            {
                AccountSearchClass accountSearchClass = new AccountSearchClass();
                accountSearchClass.Id = item.Id;
                accountSearchClass.Moein = item.MoeinCode.ToString();
                accountSearchClass.MoeinName = item.MoeinName;
                accountSearchClass.ColMoein = $"{item.FkCol.ColCode}{item.MoeinCode}";
                mus1.Add(new Mu()
                {
                    Value = $"{item.FkCol.ColName}",
                    Name = $"{item.FkCol.ColCode}",
                    AdditionalEntity = accountSearchClass
                });
            }
            foreach (var item in preferentials)
            {                
                mus2.Add(new Mu()
                {
                    Id= item.Id,
                    Name = $"{item.PreferentialName}",
                    Value = $"{item.PreferentialCode}",
                    Name2 = item.FkGroup.GroupName
                });
            }
            cmbType.SelectedItem = (cmbType.ItemsSource as List<DocumentType>).Where(y => y.Name == "عمومی").FirstOrDefault();
            if (temp > 0)
                cmbType.SelectedIndex = temp;
            if (AddedMode)
            {               
                AcDocumentDetails = acDocumentViewModel.AcDocumentDetails;
                //AcDocumentDetails.Clear();
                var y = db.AcDocumentHeaders.OrderByDescending(k => k.NoDoument).FirstOrDefault();
                if (y == null)
                {
                    txtSerial.Text = txtNoDocumen.Text = "1";
                }
                else
                {
                    txtNoDocumen.Text = (y.NoDoument + 1).ToString();
                    var yb = db.AcDocumentHeaders.OrderByDescending(k => k.NoDoument).FirstOrDefault();
                    txtSerial.Text = (y.Serial + 1).ToString();
                }
                dataPager.Source = null;
                dataPager.Source = AcDocumentDetails;
            }
            else
            {
                AcDocumentDetails = acDocumentViewModel.AcDocumentDetails;
                AcDocumentDetails.Clear();
                //AcDocumentDetails.Clear();
                var h = db.AcDocumentDetails.Where(u=>u.FkAcDocHeaderId==id).ToList();
                h.ForEach(u => AcDocumentDetails.Add(u));
                RefreshDataGridForSetPersianNumber();
            }
            dataPager.Source = null;
            dataPager.Source = AcDocumentHeaders;
            datagrid.SearchHelper.AllowFiltering = true;
            datagridSearch.SearchHelper.AllowFiltering = true;
            FirstLevelNestedGrid.SearchHelper.AllowFiltering = true;
            cmbType.Focus();
            isCancel = true;
        }

        private static void SetAccountName(wpfrazydbContext db, AcDocumentDetail item2)
        {/*
            var strings = item2.AcCode.Split('-');
            var moein = int.Parse(strings[0]);
            var tafzil = int.Parse(strings[2]);
            item2.AccountName = $"{db.Preferentials.First(i => i.PreferentialCode == tafzil).PreferentialName}-{db.Moeins.First(p => p.MoeinCode == moein).MoeinName}";*/
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            bool haserror = false;
            haserror = GetError();

            if (haserror)
                return;
            using var db = new wpfrazydbContext();
                                    //var c = int.Parse(cmbType.Text);
                                    //var col = db.tGroup.FirstOrDefault(g => g.GroupCode == c);
                                    //if (col == null)
                                    //{
                                    //    Sf_txtDoumentType.ErrorText = "این کد گروه وجود ندارد";
                                    //    Sf_txtDoumentType.HasError = true;
                                    //    return;
                                    //}
                                    //var AcDocumentDetail = db.AcDocumentDetails.Find(id);
            
                                    //var nAcDocumentDetail = db.AcDocumentDetails.FirstOrDefault(g => g.FkGroupId == col.Id && g.AcDocumentDetailName == txtNoDocumen.Text);
                                    //if (AcDocumentDetail?.Id != nAcDocumentDetail?.Id && nAcDocumentDetail != null)
                                    //{
                                    //    Xceed.Wpf.Toolkit.MessageBox.Show("این نام تفضیلی و کد گروه از قبل وجود داشته است!");
                                    //    return;
                                    //}
            var hg=db.DocumentTypes.FirstOrDefault(y=>y.Name==cmbType.Text);
            if(hg==null)
            {
                Sf_txtDoumentType.HasError = true;
                Sf_txtDoumentType.ErrorText = "این نوع سند وجود ندارد!";
                return;
            }
            AcDocumentHeader e_addHeader = null;
            AcDocumentHeader header = null;
            var yx = db.AcDocumentHeaders.OrderByDescending(k => k.Serial).FirstOrDefault();
            string serial = "1";
            if (yx != null)
            {
                serial = (yx.Serial + 1).ToString();
            }
            if (id == Guid.Empty)
            {
                e_addHeader = new AcDocumentHeader()
                {
                    Id = Guid.NewGuid(),
                    Date=pcw1.SelectedDate.ToDateTime(),
                    NoDoument = long.Parse(txtNoDocumen.Text),
                    Serial = long.Parse(serial),
                    FkDocumentType = hg
                };
                DbSet<AcDocumentDetail> details = null;
                int index = 0;
                foreach (var item in AcDocumentDetails)
                {
                    index++;
                    var en = new AcDocumentDetail()
                    {
                        FkMoeinId = item.FkMoein.Id,
                        FkPreferentialId = item.FkPreferential.Id,
                        FkAcDocHeader = e_addHeader,
                        Creditor = item.Creditor,
                        Debtor = item.Debtor,
                        Description = item.Description,
                        Indexer = index,
                        //AccountName = item.AccountName,
                        Id = Guid.NewGuid()
                    };
                    db.AcDocumentDetails.Add(en);
                }
                db.AcDocumentHeaders.Add(e_addHeader);
                if (LoadedFill)
                    AcDocumentHeaders.Add(e_addHeader);
            }
            else
            {
                var h = db.AcDocumentDetails.Where(v => v.FkAcDocHeaderId == id);
                header = AcDocumentHeaders.First(u => u.Id == id);
                foreach (var item in h)
                {
                    db.AcDocumentDetails.Remove(item);
                    header.AcDocumentDetails.Remove(header.AcDocumentDetails.First(x => x.Id == item.Id));
                }
                var e_Edidet = db.AcDocumentHeaders.Include(h => h.AcDocumentDetails)
                    .ThenInclude(d => d.FkPreferential)
                    .Include(h => h.AcDocumentDetails)
                    .ThenInclude(d => d.FkMoein)
                    .ThenInclude(d => d.FkCol).First(a => a.Id == id);
                e_Edidet.NoDoument = header.NoDoument = long.Parse(txtNoDocumen.Text);
                e_Edidet.Date = header.Date = pcw1.SelectedDate.ToDateTime();
                e_Edidet.FkDocumentType = header.FkDocumentType = hg;
                int index = 0;
                foreach (var item in AcDocumentDetails)
                {
                    index++;
                    var en = new AcDocumentDetail()
                    {
                        FkMoeinId = item.FkMoein.Id,
                        FkPreferentialId = item.FkPreferential.Id,
                        FkAcDocHeader = e_Edidet,
                        Creditor = item.Creditor,
                        Debtor = item.Debtor,
                        Description = item.Description,
                        Indexer = index,
                        //AccountName = item.AccountName,
                        Id = Guid.NewGuid()
                    };
                    db.AcDocumentDetails.Add(en);
                    header.AcDocumentDetails.Add(en);
                }
                //e_Edidet.FkGroupId = AcDocumentDetail.FkGroupId = col.Id;
                //e_Edidet.AcDocumentDetailName = AcDocumentDetail.AcDocumentDetailName = txtNoDocumen.Text;
            }
            if (!db.SafeSaveChanges())  return;
            if (header != null)
            {
                int i = 0;
                foreach (var item in header.AcDocumentDetails)
                {
                    item.FkMoein = AcDocumentDetails[i].FkMoein;
                    item.FkPreferential = AcDocumentDetails[i].FkPreferential;
                    i++;
                }
            }
            if(e_addHeader!=null)
            {
                int i = 0;
                foreach (var item in e_addHeader.AcDocumentDetails)
                {
                    item.FkMoein = AcDocumentDetails[i].FkMoein;
                    item.FkPreferential = AcDocumentDetails[i].FkPreferential;
                    i++;
                }
            }
            datagrid.SelectedIndex = -1;
            datagrid.ClearFilters();
            datagrid.SearchHelper.ClearSearch();
            if (AcDocumentDetails.Count > 0)
            {
                datagrid.Dispatcher.BeginInvoke(new Action(() =>
                {
                    AcDocumentDetails.Clear();
                }));
                RefreshDataGridForSetPersianNumber();
            }
            //datagrid.TableSummaryRows.Clear();
            SearchTermTextBox.Text = "";
            if (id == Guid.Empty)
            {
                this.gifImage.Visibility = Visibility.Visible;
                var gifImage = new BitmapImage(new Uri("pack://application:,,,/Images/AddDataLarge.gif"));
                XamlAnimatedGif.AnimationBehavior.SetSourceUri(this.gifImage, gifImage.UriSource);
                var th = new Thread(() =>
                {
                    Thread.Sleep(2570);
                    Dispatcher.Invoke(() =>
                    {
                        searchImage.Visibility = Visibility.Visible;
                        this.gifImage.Visibility = Visibility.Collapsed;
                    });
                });
                th.Start();
                searchImage.Visibility = Visibility.Collapsed;
                Xceed.Wpf.Toolkit.MessageBox.Show("اطلاعات اضافه شد.", "ثبت سند");
                searchImage.Visibility = Visibility.Visible;
                this.gifImage.Visibility = Visibility.Collapsed;
                txtNoDocumen.Text = (long.Parse(txtNoDocumen.Text) + 1).ToString();
                txtSerial.Text = (long.Parse(serial) + 1).ToString();

                cmbType.SelectedIndex = 0;
                cmbType.Focus();
            }
            else
            {                
                Xceed.Wpf.Toolkit.MessageBox.Show("اطلاعات ویرایش شد.", "ویرایش سند");
                btnCancel_Click(null, null);
            }
                            
            isCancel = true;                        
            id = Guid.Empty;
        }
        Guid id = Guid.Empty;
        private bool GetError()
        {
            var haserror = false;
            datagrid.BorderBrush = new  System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#FF808080"));

            if (txtNoDocumen.Text.Trim() == "")
            {
                Sf_txtNoDocumen.HasError = true;
                haserror = true;
            }
            else
                Sf_txtNoDocumen.HasError = false;
            if (cmbType.Text.Trim() == "")
            {
                Sf_txtDoumentType.HasError = true;
                haserror = true;
            }
            else
            {
                Sf_txtDoumentType.HasError = false;
                Sf_txtDoumentType.ErrorText = "";
            }
            if (AcDocumentDetails.Count == 0)//AcDocumentDetails.Any(g => !viewModel.AllCommodities.Any(y => y.CommodityCode == g.CommodityCode)))
            {
                datagrid.BorderBrush = Brushes.Red;
                haserror = true;
            }
            else if (AcDocumentDetails.Any(t => t.ColeMoein == ""|| t.ColeMoein==null || t.PreferentialCode == "" || t.PreferentialCode == null || ((t.Creditor == 0 || t.Creditor == null)&& (t.Debtor == 0||t.Debtor==null))
            || (t.Creditor != 0&&t.Creditor!=null && t.Debtor != 0 && t.Debtor != null)))
            {
                datagrid.BorderBrush = Brushes.Red;
                haserror = true;
            }
            return haserror;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!forceClose && Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید از این فرم خارج شوید؟", "خروج", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
        }


        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            var gr = brush as LinearGradientBrush;
            if (gr != null)
            {
                var gr2 = new LinearGradientBrush();
                foreach (var item in gr.GradientStops)
                {
                    gr2.GradientStops.Add(new GradientStop(item.Color, item.Offset));
                }
                for (var i = 1; i < gr2.GradientStops.Count; i++)
                {
                    gr2.GradientStops[i].Color = ColorToBrushConverter.GetLightOfColor(gr.GradientStops[i].Color, .15f);
                }
                gr2.EndPoint = gr.EndPoint;
                gr2.StartPoint = gr.StartPoint;
                border.Background = gr2;
            }
            else
            {
                border.Background = new SolidColorBrush(ColorToBrushConverter.GetLightOfColor((brush as SolidColorBrush).Color, .15f));
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void border_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Border).Background = brush;
        }

        private void border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border.IsMouseOver)
            {
                Border_MouseEnter(sender, e);
            }
            else
            {
                border.Background = brush;
            }
        }

        private void btnExcelPattern_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExcelPattern", "Commodity.xlsx"));
        }

        private void txtProductID_KeyDown(object sender, KeyEventArgs e)
        {
            /*if (e.Key == Key.F1)
            {
                Border_MouseDown("id",null);
            }*/
        }

        private void txtMu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                Border_MouseDown("mu", null);
            }
        }
        // تعریف توابع مورد نیاز از user32.dll
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        // کلیدهای مجازی
        const byte VK_F2 = 0x71; // کد کلید F2
        const uint KEYEVENTF_KEYUP = 0x0002; // نشان دهنده آزاد کردن کلید
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int X, int Y);
        private void datagrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                if (datagrid.SelectionController.CurrentCellManager?.CurrentCell?.ColumnIndex == 0)
                {
                    dynamic y = null;
                    var element = (datagrid.SelectionController.CurrentCellManager.CurrentCell.Element as GridCell)
                            .Content as FrameworkElement;
                    y = element.DataContext;
                    if (element is TextBlock)
                    {
                        if (datagrid.SelectedIndex == -1)
                        {
                            if (y == null)
                            {
                                bool d = datagrid.GetGridModel().AddNewRowController.AddNew();
                            }
                            y = element.DataContext;
                        }
                        var cell = datagrid.SelectionController.CurrentCellManager.CurrentCell.Element;
                        //var screenPosition = cell.PointToScreen(new System.Windows.Point(0, 0));
                        //SetCursorPos((int)screenPosition.X - 30, (int)screenPosition.Y + 15);
                        //LeftDoubleClick();
                        // شبیه‌سازی فشار دادن کلید F2
                        keybd_event(VK_F2, 0, 0, UIntPtr.Zero); // فشار دادن کلید
                        Thread.Sleep(50); // تاخیر برای شبیه‌سازی فشار دادن
                        keybd_event(VK_F2, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // آزاد کردن کلید
                        var th = new Thread(() =>
                        {
                            Thread.Sleep(10);
                            Dispatcher.Invoke(() =>
                            datagrid_PreviewKeyDown(sender, e));
                        });
                        th.Start();
                        return;
                    }           
                    var win = new winSearch(mus1);
                    win.Closed+=(yf,rs)=>
                    {
                        datagrid.IsHitTestVisible = true;
                    };
                    win.Width = 640;
                    win.datagrid.Columns[0].HeaderText = "کل";
                    win.datagrid.Columns[1].HeaderText = "نام";
                    win.datagrid.Columns[1].Width = 255;
                    win.datagrid.Columns[0].Width = 100;
                    win.datagrid.Columns.Add(new GridTextColumn() {TextAlignment= TextAlignment.Center, HeaderText = "معین", MappingName = "AdditionalEntity.Moein", Width = 100, AllowSorting = true });
                    win.datagrid.Columns.Add(new GridTextColumn() { TextAlignment = TextAlignment.Center, HeaderText = "نام", MappingName = "AdditionalEntity.MoeinName", AllowSorting = true, ColumnSizer= GridLengthUnitType.AutoWithLastColumnFill });
                    win.datagrid.AllowResizingColumns = true;
                    win.Tag = this;
                    win.ParentTextBox = y;
                    win.SearchTermTextBox.Text = "";
                    win.SearchTermTextBox.Select(1, 0);
                    win.Owner = MainWindow.Current;
                    window = win;
                    win.Show();
                    win.Focus();
                    datagrid.IsHitTestVisible = false;
                }
                else if (datagrid.SelectionController.CurrentCellManager?.CurrentCell?.ColumnIndex == 1)
                {
                    dynamic y = null;
                    var element = (datagrid.SelectionController.CurrentCellManager.CurrentCell.Element as GridCell)
                            .Content as FrameworkElement;
                    y = element.DataContext;
                    if (datagrid.SelectedIndex == -1 || element is TextBlock)
                    {
                        if (y == null || y.PreferentialCode != null)
                        {
                            var cell = datagrid.SelectionController.CurrentCellManager.CurrentCell.Element;
                            keybd_event(VK_F2, 0, 0, UIntPtr.Zero); // فشار دادن کلید
                            Thread.Sleep(50); // تاخیر برای شبیه‌سازی فشار دادن
                            keybd_event(VK_F2, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // آزاد کردن کلید
                            var th = new Thread(() =>
                            {
                                Thread.Sleep(10);
                                Dispatcher.Invoke(() =>
                                datagrid_PreviewKeyDown(sender, e));
                            });
                            th.Start();
                            return;
                        }
                    }
                    var win = new winSearch(mus2);
                    win.Closed += (yf, rs) =>
                    {
                        datagrid.IsHitTestVisible = true;
                    };
                    win.datagrid.Columns.Add(new GridTextColumn() {TextAlignment= TextAlignment.Center, HeaderText = "گروه", MappingName = "Name2", Width = 150, AllowSorting = true });
                    win.Width = 640;                    
                    win.Tag = this;
                    win.ParentTextBox = y;
                    win.SearchTermTextBox.Text = "";
                    win.SearchTermTextBox.Select(1, 0);
                    win.Owner = MainWindow.Current;
                    window = win;
                    win.Show();
                    win.Focus();
                    datagrid.IsHitTestVisible = false;
                }
            }           
        }     
        private void datagrid_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {
            isCancel = false;
            CalDebCre();

            if (window == null && datagrid.GetRecordAtRowIndex(e.RowColumnIndex.RowIndex) is AcDocumentDetail AcDocumentDetail)
            {
                if ((CurrentCellText ?? "") != "")
                {
                    if (e.RowColumnIndex.ColumnIndex == 0)
                    {
                        using var db = new wpfrazydbContext();
                        var mu = mus1.Find(t => (t.AdditionalEntity as AccountSearchClass).ColMoein == CurrentCellText);
                        if (mu == null)
                        {

                        }
                        else
                        {
                            var moein = db.Moeins.Include("FkCol").Where(h => h.Id == (mu.AdditionalEntity as AccountSearchClass).Id).First();
                            AcDocumentDetail.FkMoein = moein;
                        }
                    }
                    else if(e.RowColumnIndex.ColumnIndex == 1)
                    {
                        using var db = new wpfrazydbContext();
                        var mu = mus2.Find(t => t.Value == CurrentCellText);
                        if (mu == null)
                        {

                        }
                        else
                        {
                            var preferential = db.Preferentials.Find(mu.Id);
                            AcDocumentDetail.FkPreferential = preferential;
                        }
                    }
                }
            }
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                var th = new Thread(() =>
                {
                    Thread.Sleep(30);
                    Dispatcher.Invoke(new Action(() =>
                    SetEnterToNextCell(this.CurrentRowColumnIndex)));
                });
                th.Start();
            }
        }

        private void btnTransferOfExcel_Click(object sender, RoutedEventArgs e)
        {

        }
        bool forceClose = false;
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CloseForm();
            }         
        }

        private void cmbType_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key ==  Key.Enter)
            {                
                btnConfirm.Focus();
                return;
            }
        }
        bool isCancel = true;
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (AddedMode&&isCancel)
            {
                return;
            }
            if (searchImage.ToolTip.ToString() == "جستجو" && sender != null && Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید از این عملیات انصراف دهید؟", "انصراف", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }
            searchImage.Visibility = Visibility.Visible;
            searchImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Data.png"));
            searchImage.ToolTip = "جستجو";
            GStop1.Color = new Color()
            {
                R = 244,
                G = 248,
                B = 255,
                A = 255
            };            
            searchImage.Opacity = 1;
            using var db = new wpfrazydbContext();
            var y = db.AcDocumentHeaders.OrderByDescending(k => k.NoDoument).FirstOrDefault();
            if (y == null)
            {
                txtSerial.Text = txtNoDocumen.Text = "1";
            }
            else
            {
                txtNoDocumen.Text = (y.NoDoument + 1).ToString();
                var yb = db.AcDocumentHeaders.OrderByDescending(k => k.NoDoument).FirstOrDefault();
                txtSerial.Text = (y.Serial + 1).ToString();
            }
            if (!AddedMode)
            {
                if (id != Guid.Empty)
                {
                    var e_Edidet = db.AcDocumentHeaders.Include(h => h.AcDocumentDetails)
                    .ThenInclude(d => d.FkPreferential)
                    .Include(h => h.AcDocumentDetails)
                    .ThenInclude(d => d.FkMoein)
                    .ThenInclude(d => d.FkCol).First(a => a.Id == id);
                    var header = AcDocumentHeaders.FirstOrDefault(o => o.Id == id);
                    header.AcDocumentDetails.Clear();
                    foreach (var item in e_Edidet.AcDocumentDetails)
                    {
                        header.AcDocumentDetails.Add(item);
                        SetAccountName(db, item);
                    }
                }
                AddedMode = true;                
                column1.Width = new GridLength(225);
                datagrid.AllowEditing = datagrid.AllowDeleting = true;
                datagrid.AddNewRowPosition = Syncfusion.UI.Xaml.Grid.AddNewRowPosition.Bottom;
            }
            datagrid.Visibility = Visibility.Visible;
            datagridSearch.Visibility = Visibility.Collapsed;
            gridConfirm.Visibility = Visibility.Visible;
            cmbType.IsReadOnly = false;
            Sf_txtNoDocumen.HasError = false;
            Sf_txtDoumentType.HasError = false;
            Sf_txtDoumentType.ErrorText = "";
            //txtCodeAcDocumentDetail.Text = (en.AcDocumentDetailCode + 1).ToString();
            
            cmbType.Focus();
            datagrid.SelectedIndex = -1;
            datagrid.ClearFilters();
            //datagrid.TableSummaryRows.Clear();
            datagrid.SearchHelper.ClearSearch();
            SearchTermTextBox.Text = "";
            dataPager.Visibility = Visibility.Collapsed;
            gridDelete.Visibility = Visibility.Hidden;
            borderEdit.Visibility = Visibility.Hidden;
            cmbType.SelectedIndex = 0;
            datagrid.BorderBrush = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#FF808080"));
            if (AcDocumentDetails.Count > 0)
            {
                datagrid.Dispatcher.BeginInvoke(new Action(() =>
                {
                    AcDocumentDetails.Clear();
                }));
                RefreshDataGridForSetPersianNumber();
            }           
            isCancel = true;
            id = Guid.Empty;
        }

        private void datagrid_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            return;
            if (datagrid.SelectedItem != null && !AddedMode)
            {
                gridDelete.Visibility = Visibility.Visible;
                /*var AcDocumentDetail = datagrid.SelectedItem as AcDocumentDetail;
                id = AcDocumentDetail.Id;
                cmbType.TextChanged -= txtDoumentType_TextChanged;
                cmbType.Text = AcDocumentDetail.tGroup.GroupCode.ToString();
                cmbType.TextChanged += txtDoumentType_TextChanged;
                txtSerial.Text = AcDocumentDetail.tGroup.GroupName;
                txtNoDocumen.Text = AcDocumentDetail.AcDocumentDetailName;
                gridDelete.Visibility = Visibility.Visible;
                borderEdit.Visibility = Visibility.Visible;
                cmbType.IsReadOnly = true;
                isCancel = true;
                GetError();*/
            }
            else
                gridDelete.Visibility = Visibility.Collapsed;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (id == Guid.Empty)
                return;
            var header = datagridSearch.SelectedItem as AcDocumentHeader;
            if (header?.FkDocumentType.Name != "عمومی")
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("این سند سیستمی زده شده و قابل حذف نیست!");
                return;
            }
            if (Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید این اطلاعات پاک شود؟", "حذف", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }            
            using var db = new wpfrazydbContext();
            foreach (var item in db.AcDocumentDetails.Where(u => u.FkAcDocHeaderId == id))
            {
                db.AcDocumentDetails.Remove(item);
            }
            db.AcDocumentHeaders.Remove(db.AcDocumentHeaders.Find(id));
            if (!db.SafeSaveChanges())  return;
            try
            {
                AcDocumentHeaders.Remove(AcDocumentHeaders.First(f => f.Id == id));
            }
            catch
            {

            }
            //btnCancel_Click(null, null);
        }
        
        private void SearchTermTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchTermTextBox.Text.Trim() == string.Empty)
            {
                if (FirstLevelNestedGrid.SearchHelper.SearchText.Trim() != "")
                {

                }
                else if (datagrid.Visibility != Visibility.Visible)
                    return;
            }
            try
            {
                if (datagrid.Visibility == Visibility.Visible)
                {
                    datagrid.SearchHelper.Search(SearchTermTextBox.Text);
                }
                else
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    if (InputLanguageManager.Current.CurrentInputLanguage.Name != "fa-IR")
                    {
                        decimal ds = 0;
                        if (decimal.TryParse(SearchTermTextBox.Text.Trim().Replace(",", ""), out ds) && ds >= 0)
                        {

                            int temp = SearchTermTextBox.SelectionStart;
                            SearchTermTextBox.TextChanged -= SearchTermTextBox_TextChanged;
                            SearchTermTextBox.Text = string.Format("{0:#,###}", ds);
                            if (SearchTermTextBox.SelectionStart != temp)
                                SearchTermTextBox.SelectionStart = temp + 1;
                            SearchTermTextBox.TextChanged += SearchTermTextBox_TextChanged;
                        }
                    }
                    datagridSearch.SelectedIndex = -1;
                    if (SearchTermTextBox.Text.Trim() == "")
                    {
                        dataPager.Visibility = Visibility.Visible;
                        datagridSearch.SearchHelper.ClearSearch();
                        FirstLevelNestedGrid.SearchHelper.ClearSearch();
                        var g = dataPager.Source;
                        dataPager.Source = null;
                        dataPager.Source = g;
                    }
                    else
                    {
                        //dataPager.Visibility = Visibility.Collapsed;
                        datagridSearch.SearchHelper.Search("");
                        FirstLevelNestedGrid.SearchHelper.Search(SearchTermTextBox.Text);
                        SetHide_EmptyDetails();
                        //datagridSearch.View.Refresh();

                        //var h2 = FirstLevelNestedGrid.SearchHelper.GetSearchRecords();
                        //var h1 = datagridSearch.SearchHelper.GetSearchRecords();

                        /*foreach (AcDocumentHeader item in datagridSearch.DetailsViewDefinition)
                        {
                            if(item.AcDocumentDetail.Count!=0)
                            {

                            }
                            else
                            {

                            }
                        }*/
                        //datagridSearch.SearchHelper.Search(SearchTermTextBox.Text);
                    }
                }
                if (SearchTermTextBox.Text == "")
                    RefreshDataGridForSetPersianNumber();                
            }
            catch(Exception ex)
            {
            }
            Mouse.OverrideCursor = null;
        }

        private void SetHide_EmptyDetails()
        {
            if (SearchTermTextBox.Text == "")
                return;
            int ir = 0;
            var list = new List<int>();
            foreach (var item in datagridSearch.View?.Records)
            {
                var tt = item.Data as AcDocumentHeader;
                if (!tt.AcDocumentDetails.Any(i => i.Description?.ToLower().Contains(SearchTermTextBox.Text.ToLower())==true ||
                i.Name.ToLower().Contains(SearchTermTextBox.Text.ToLower()) ||
                i.Creditor2?.ToString().ToLower().Contains(SearchTermTextBox.Text.ToLower()) == true ||
                i.ColeMoein.ToLower().Contains(SearchTermTextBox.Text.ToLower()) ||
                i.PreferentialCode.ToLower().Contains(SearchTermTextBox.Text.ToLower()) ||
                i.Debtor2?.ToString().ToLower().Contains(SearchTermTextBox.Text.ToLower()) == true))
                {
                    //datagridSearch.View.Records.Remove(item);
                    list.Add(ir);
                }
                else
                {
                    item.IsExpanded = true;
                    //this.datagrid.ExpandDetailsViewAt(this.datagrid.ResolveToRecordIndex(ir));
                }
                ir++;
            }
            var l = 0;
            for (var i = 0; i < list.Count; i++)
            {
                datagridSearch.View.Records.RemoveAt(list[i] - l);
                l++;
            }

            datagridSearch.ExpandAllDetailsView();
                
        }

        private void txtNoDocumen_TextChanged(object sender, TextChangedEventArgs e)
        {
            isCancel = false;
        }

        private void TxtCodeAcDocumentDetail_TextChanged(object sender, TextChangedEventArgs e)
        {
            isCancel = false;
        }
        public static Window window;
        private void txtDoumentType_TextChanged(object sender, TextChangedEventArgs e)
        {
            isCancel = false;
        }

        private void txtDoumentType_LostFocus(object sender, RoutedEventArgs e)
        {
            
        }

        private void DataPager_PageIndexChanging(object sender, Syncfusion.UI.Xaml.Controls.DataPager.PageIndexChangingEventArgs e)
        {
            var ex = datagrid.View.FilterPredicates;
            
            using var db = new wpfrazydbContext();
            //db.AcDocumentDetails.Where(ex)
            var count = db.AcDocumentDetails.Count();
            var F = db.AcDocumentDetails.OrderBy(d=>d.Id).Skip(10 * e.NewPageIndex).Take(10).ToList();
            int j = 0;
            for (int i = 10 * e.NewPageIndex; i < 10 * (e.NewPageIndex + 1)&&i<count; i++)
            {
                AcDocumentDetails[i] = F[j];
                j++;
            }
        }
        public bool CloseForm()
        {
            if (!isCancel && Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید از این فرم خارج شوید؟", "خروج", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return false;
            }
            forceClose = true;
            var list = MainWindow.Current.GetTabControlItems;
            var item = list.FirstOrDefault(u => u.Header == "سند حسابداری");
            MainWindow.Current.tabcontrol.Items.Remove(item);
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Dispose();
            }));
            return true;
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

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            datagridSearch.AllowFiltering = !datagridSearch.AllowFiltering;
            FirstLevelNestedGrid.AllowFiltering = !FirstLevelNestedGrid.AllowFiltering;
            if (!datagridSearch.AllowFiltering)
                datagridSearch.ClearFilters();
            if (!FirstLevelNestedGrid.AllowFiltering)
                FirstLevelNestedGrid.ClearFilters();
        }

        public void SetNull()
        {
            if(window!=null&&(window as winSearch).ParentTextBox is AcDocumentDetail)
            {
                var y = (window as winSearch).ParentTextBox as AcDocumentDetail;
                //((datagrid.SelectionController.CurrentCellManager.CurrentCell.Element as GridCell).Content as FrameworkElement).DataContext = null;
                //((datagrid.SelectionController.CurrentCellManager.CurrentCell.Element as GridCell).Content as FrameworkElement).DataContext = y;
                var detail = y;
                var v = datagrid.SelectionController.CurrentCellManager.CurrentCell;
                if ((window as winSearch)?.MuText != null)
                {
                    datagrid.Dispatcher.BeginInvoke(new Action(() =>
                    {                        
                        //MMM
                        var th = new Thread(() =>
                        {
                            Thread.Sleep(100);
                            Dispatcher.Invoke(() =>
                            {
                                var i = 1;
                                if (v.ColumnIndex == 1)
                                    i++;
                                if (datagrid.SelectedIndex == -1)
                                {
                                    datagrid.GetAddNewRowController().CommitAddNew();
                                    datagrid.View.Refresh();
                                    datagrid.SelectedIndex = datagrid.GetLastRowIndex() - 1;
                                    if (datagrid.SelectedIndex != -1)
                                        (this.datagrid.SelectionController as GridSelectionController).MoveCurrentCell(new RowColumnIndex(v.RowIndex - 1, v.ColumnIndex + i));
                                }
                                else
                                {
                                    datagrid.View.Refresh();
                                    (this.datagrid.SelectionController as GridSelectionController).MoveCurrentCell(new RowColumnIndex(v.RowIndex, v.ColumnIndex + i));
                                }
                                //MMM
                                datagrid.IsHitTestVisible = true;
                            });
                        });
                        th.Start();
                        //datagrid.SelectCells(datagrid.GetRecordAtRowIndex(datagrid.SelectedIndex-1), datagrid.Columns[1], datagrid.GetRecordAtRowIndex(datagrid.SelectedIndex), datagrid.Columns[2]);
                    }));
                }
            }
            window = null;
        }

        private void pcw1_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            txbCalender.Text = pcw1.SelectedDate.ToString();

        }

        private void Pcw1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }

        private void datagrid_AddNewRowInitiating(object sender, Syncfusion.UI.Xaml.Grid.AddNewRowInitiatingEventArgs e)
        {
            /*
            var h = acDocumentViewModel.AcDocumentDetails.FirstOrDefault(q => q.AcCode == ctext);
            if (h != null)
            {
                (e.NewObject as UtililtyCommodity).CommodityId = h.ID;
                (e.NewObject as UtililtyCommodity).CommodityCode = ctext;
                (e.NewObject as UtililtyCommodity).Discount = 0;
                (e.NewObject as UtililtyCommodity).Tax = h.Tax;
            }*/
        }

        private void searchImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (searchImage.Opacity != .6)
            {
                isCancel = true;
                if (!AddedMode && (searchImage.Source as BitmapImage).UriSource.AbsoluteUri.Contains("dataedit.png"))
                {
                    if (datagridSearch.SelectedItem == null)
                        return;
                    var header = datagridSearch.SelectedItem as AcDocumentHeader;
                    if(header.FkDocumentType.Name!="عمومی")
                    {
                        Xceed.Wpf.Toolkit.MessageBox.Show("این سند سیستمی زده شده و قابل ویرایش نیست!");
                        return;
                    }
                    searchImage.Visibility = Visibility.Visible;
                    searchImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Data.png"));
                    searchImage.ToolTip = "جستجو";
                    GStop1.Color = new Color()
                    {
                        R = 244,
                        G = 248,
                        B = 255,
                        A = 255
                    };
                    searchImage.Opacity = 1;
                    gridDelete.Visibility = Visibility.Collapsed;
                    AcDocumentDetails.Clear();
                    id = header.Id;
                    header.AcDocumentDetails.ForEach(t => AcDocumentDetails.Add(t));
                    cmbType.SelectedItem = (cmbType.ItemsSource as List<DocumentType>).First(u => u.Id == header.FkDocumentType.Id);
                    pcw1.SelectedDate = new PersianCalendarWPF.PersianDate(header.Date);
                    txbCalender.Text = pcw1.SelectedDate.ToString();
                    txtNoDocumen.Text = header.NoDoument.ToString();
                    txtSerial.Text = header.Serial.ToString();
                    datagrid.AllowEditing = datagrid.AllowDeleting = true;
                    datagrid.AddNewRowPosition = Syncfusion.UI.Xaml.Grid.AddNewRowPosition.Bottom;
                    datagrid.Visibility = Visibility.Visible;
                    dataPager.Visibility = Visibility.Collapsed;
                    testsearch.Text = "جستجو...";
                    datagrid.SearchHelper.ClearSearch();
                    SearchTermTextBox.TextChanged-= SearchTermTextBox_TextChanged;
                    SearchTermTextBox.Text = "";
                    SearchTermTextBox.TextChanged+= SearchTermTextBox_TextChanged;
                    datagridSearch.Visibility = Visibility.Collapsed;
                    gridConfirm.Visibility = Visibility.Visible;
                    cmbType.IsReadOnly = false;
                    Sf_txtNoDocumen.HasError = false;
                    Sf_txtDoumentType.HasError = false;
                    Sf_txtDoumentType.ErrorText = "";
                    column1.Width = new GridLength(225);
                    borderEdit.Visibility = Visibility.Visible;
                    RefreshDataGridForSetPersianNumber();
                    datagrid.SelectedIndex = AcDocumentDetails.Count - 1;
                    isCancel = true;
                }
                else
                {
                    if (!isCancel)
                    {
                        if (Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید از این عملیات انصراف دهید؟", "انصراف", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                        {
                            return;
                        }
                    }
                    FillHeaders();
                    if (!AddedMode)
                    {
                        using var db = new wpfrazydbContext();
                        var e_Edidet = db.AcDocumentHeaders.Include(g => g.FkDocumentType)                    
                    .Include(h => h.AcDocumentDetails)
                    .ThenInclude(d => d.FkPreferential)
                    .Include(h => h.AcDocumentDetails)
                    .ThenInclude(d => d.FkMoein)
                    .ThenInclude(d => d.FkCol)
                    .First(f => f.Id == id);
                        var header = AcDocumentHeaders.FirstOrDefault(o => o.Id == id);
                        header.AcDocumentDetails.Clear();
                        e_Edidet.AcDocumentDetails = e_Edidet.AcDocumentDetails
                       .OrderBy(d => d.Indexer)
                       .ToList();
                        foreach (var item in e_Edidet.AcDocumentDetails)
                        {
                            header.AcDocumentDetails.Add(item);
                            SetAccountName(db, item);
                        }
                    }
                    datagridSearch.ClearFilters();
                    datagridSearch.SortColumnDescriptions.Clear();
                    datagridSearch.SortColumnDescriptions.Add(new SortColumnDescription()
                    {
                        ColumnName = "Serial",
                        SortDirection = System.ComponentModel.ListSortDirection.Descending
                    });
                    datagridSearch.SearchHelper.ClearSearch();
                    FirstLevelNestedGrid.SearchHelper.ClearSearch();
                    SearchTermTextBox.Text = "";
                    datagridSearch.SelectedItem = null;
                    var t = dataPager.Source;
                    foreach (var item in t as ObservableCollection<AcDocumentHeader>)
                    {
                        item.RefreshSumColumns();
                    }
                    dataPager.Source = null;
                    datagridSearch.SelectedIndex = 0;
                    borderEdit.Visibility = Visibility.Collapsed;
                    gridDelete.Visibility= Visibility.Visible;
                    datagrid.Visibility = Visibility.Collapsed;
                    datagridSearch.Visibility = Visibility.Visible;
                    dataPager.Visibility = Visibility.Visible;
                    testsearch.Text = "جستجو در جزئیات...";
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        datagridSearch.Visibility = Visibility.Collapsed;
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            datagridSearch.Visibility = Visibility.Visible;
                            dataPager.Source = t;
                        }), DispatcherPriority.Render);
                    }), DispatcherPriority.Render);
                    gridConfirm.Visibility = Visibility.Collapsed;
                    if ((t as ObservableCollection<AcDocumentHeader>).Count == 0)
                        searchImage.Opacity = .6;
                    searchImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/dataedit.png"));
                    searchImage.ToolTip = "ویرایش";
                    GStop1.Color = new Color()
                    {
                        R = 209,
                        G = 226,
                        B = 255,
                        A = 240
                    };
                    column1.Width = new GridLength(0);
                    datagrid.AllowEditing = datagrid.AllowDeleting = false;
                    datagrid.AddNewRowPosition = Syncfusion.UI.Xaml.Grid.AddNewRowPosition.None;
                    AddedMode = false;
                    datagrid.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render,
                new Action(() =>
                {
                    SetHide_EmptyDetails();
                }));
                }
            }
        }

        public bool LoadedFill = false;
        private void FillHeaders()
        {
            if (!LoadedFill)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                using var db = new wpfrazydbContext();
                var documents = db.AcDocumentHeaders
                    .Include(g=>g.FkDocumentType)
                    .Include(h => h.AcDocumentDetails)
                    .ThenInclude(d => d.FkPreferential)
                    .Include(h => h.AcDocumentDetails)
                    .ThenInclude(d => d.FkMoein)
                    .ThenInclude(d => d.FkCol)
                    .AsNoTracking()
                    .ToList();
                foreach (var doc in documents)
                {
                    doc.AcDocumentDetails = doc.AcDocumentDetails
                        .OrderBy(d => d.Indexer)
                        .ToList();

                    foreach (var item2 in doc.AcDocumentDetails)
                    {
                        SetAccountName(db, item2);
                    }
                    AcDocumentHeaders.Add(doc);
                }
                LoadedFill = true;
                Mouse.OverrideCursor = null;
            }
            else
            {
                Mouse.OverrideCursor = Cursors.Wait;
                AcDocumentHeaders.ForEach(y => y.AcDocumentDetails = y.AcDocumentDetails
                   .OrderBy(d => d.Indexer)
                   .ToList());
                Mouse.OverrideCursor = null;
            }
        }

        private void RefreshDataGridForSetPersianNumber()
        {
            var wy = datagrid.Template;
            var uy = datagrid.ItemsSource;
            datagrid.Template = null;
            datagrid.ItemsSource = null;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                datagrid.Template = wy;
                datagrid.ItemsSource = uy;
            }), DispatcherPriority.Render);
        }

        private void datagridSearch_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            if (datagridSearch.SelectedItem != null)
            {
                searchImage.Opacity = 1;
                var header = datagridSearch.SelectedItem as AcDocumentHeader;
                id = header.Id;
            }
            else if (datagrid.Visibility != Visibility.Visible)
                id = Guid.Empty;
        }

        private void datagrid_RowValidated(object sender, RowValidatedEventArgs e)
        {
            var detail = e.RowData as AcDocumentDetail;
            if (datagrid.SelectedIndex!=-1&& detail.ColeMoein == null && detail.PreferentialCode == null && detail.Debtor == null && detail.Creditor == null && detail.Description == null)
            {
                AcDocumentDetails.Remove(detail);
                return;
            }
            var currentCell = datagrid.SelectionController.CurrentCellManager?.CurrentCell;
            if (window != null)
                (window as winSearch).ParentTextBox = detail;
            if (currentCell?.ColumnIndex == 4 && (detail.Debtor ?? 0) != 0)
                detail.Creditor = null;
            if (currentCell?.ColumnIndex == 5 && (detail.Creditor ?? 0) != 0)
                detail.Debtor = null;
        }

        private void AcDocumentDetails_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var detail = AcDocumentDetails.LastOrDefault();
            if (detail == null)
                return;
            if (detail.ColeMoein == null && detail.PreferentialCode == null && detail.Debtor == null && detail.Creditor == null && detail.Description == null)
            {
                datagrid.Dispatcher.BeginInvoke(new Action(() =>
                {
                    AcDocumentDetails.Remove(detail);
                }));
            }
            datagrid.Dispatcher.BeginInvoke(new Action(() =>
            {
                CalDebCre();
            }));
        }

        private void CalDebCre()
        {
            if (datagrid.SelectionController.CurrentCellManager?.CurrentCell?.ColumnIndex >= 4)
            {
                var t = datagrid.ItemsSource;
                datagrid.ItemsSource = null;
                datagrid.ItemsSource = t;
            }
            datagrid.View?.Refresh();
            return;

            var c = AcDocumentDetails.Sum(y => y.Creditor);
            var d = AcDocumentDetails.Sum(y => y.Debtor);
            {
                datagrid.TableSummaryRows[0].SummaryColumns.Add(new GridSummaryColumn() {Name="hfgh", Format = "{Sum:N0}", MappingName = "Debtor", SummaryType = Syncfusion.Data.SummaryType.DoubleAggregate });

            }
            datagrid.TableSummaryRows.Clear();
            var gridSummaryRow = new Syncfusion.UI.Xaml.Grid.GridSummaryRow();            
            var Tafazol = AcDocumentDetails.Sum(y => y.Debtor) - AcDocumentDetails.Sum(y => y.Creditor);
            if (Tafazol != null)
            {
                var sign = Tafazol.Value >= 0 ? "" : "منفی";
                datagrid.TableSummaryRows.Add(new Syncfusion.UI.Xaml.Grid.GridSummaryRow() { Title = $"اختلاف : {string.Format("{0:#,###}", Math.Abs(Tafazol.Value))} {sign}" });
            }
        }

        private void cmbType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            isCancel = false;
        }

        private void datagridSearch_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter) 
            {
                searchImage_PreviewMouseDown(null, null);
            }
        }
        private void PART_AdvancedFilterControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
          
        }
        TextBox textBox1, textBox2;
        DatePicker datePicker1, datePicker2;
        private void PART_AdvancedFilterControl_GotFocus(object sender, RoutedEventArgs e)
        {
            var advance = sender as AdvancedFilterControl;
            if (datePicker1 == null)
            {
                var comboBoxes = advance.GetChildsOfType<ComboBox>();                
                var combo = comboBoxes[1];
                var grid = combo.Parent as Grid;
                grid.Children[0].Visibility = Visibility.Collapsed;
                grid.Children[1].Visibility = Visibility.Visible;
                (grid.Children[1] as TextBox).IsReadOnly = true;
                textBox1 = grid.Children[1] as TextBox;
                (comboBoxes[3].Parent as Grid).Children[0].Visibility = Visibility.Collapsed;
                (comboBoxes[3].Parent as Grid).Children[1].Visibility = Visibility.Visible;
                textBox2 = (comboBoxes[3].Parent as Grid).Children[1] as TextBox;
                ((comboBoxes[3].Parent as Grid).Children[1] as TextBox).IsReadOnly = true;
                datePicker1 = grid.Children[2] as DatePicker;
                datePicker2 = ((comboBoxes[3].Parent as Grid).Children[2]) as DatePicker;
                (MyPopupS.Parent as Grid).Children.Remove(MyPopupS);
                MyPopupS.Visibility = Visibility.Visible;
                grid.Children.Add(MyPopupS);
                (MyPopupE.Parent as Grid).Children.Remove(MyPopupE);
                MyPopupE.Visibility = Visibility.Visible;
                (comboBoxes[3].Parent as Grid).Children.Add(MyPopupE);
            }
            if (datePicker1?.IsMouseOver == true)
            {
                //grid.Children.RemoveAt(2);
                FieldInfo fieldInfo = typeof(DatePicker).GetField("_popUp", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                (fieldInfo.GetValue(datePicker1) as Popup).IsOpen = false;
                if (textBox1.Text == null || textBox1.Text == "" || persianCalendar.SelectedDate.ToDateTime() == System.DateTime.Today)
                {
                    persianCalendar.SelectedDate = new Mahdi.PersianDate(System.DateTime.Today.AddDays(-1));
                    persianCalendar.SelectedDate = new Mahdi.PersianDate(System.DateTime.Today);
                }
                MyPopupS.IsOpen = true;
            }
            if (datePicker2?.IsMouseOver == true)
            {
                //grid.Children.RemoveAt(2);
                FieldInfo fieldInfo = typeof(DatePicker).GetField("_popUp", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                (fieldInfo.GetValue(datePicker2) as Popup).IsOpen = false;
                if (textBox2.Text == null || textBox2.Text == "" || persianCalendarE.SelectedDate.ToDateTime() == System.DateTime.Today)
                {
                    persianCalendarE.SelectedDate = new Mahdi.PersianDate(System.DateTime.Today.AddDays(-1));
                    persianCalendarE.SelectedDate = new Mahdi.PersianDate(System.DateTime.Today);
                }
                MyPopupE.IsOpen = true;
            }
        }

        GridFilterControl gridFilterControl;
        private void PART_AdvancedFilterControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(textBox1!=null)
            {
                textBox1.Text = string.Empty;
                textBox1.TextChanged += TextBox1_TextChanged;
                textBox2.Text = string.Empty;
                textBox2.TextChanged += TextBox2_TextChanged; ;
            }
            datePicker1 = datePicker2 = null;
            //textBox1 = null;
            //textBox2 = null;
            var advance = sender as AdvancedFilterControl;            
            advance.Tag = true;
            FieldInfo fieldInfo = typeof(AdvancedFilterControl).GetField("gridFilterCtrl", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            gridFilterControl = (GridFilterControl)fieldInfo.GetValue(advance);
        }

        private void TextBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            var h = textBox2.Text.Split('/');
            textBox2.TextChanged -= TextBox2_TextChanged;
            textBox2.Text = $"{h[2]}/{h[1]}/{h[0]}";
        }

        private void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            var h = textBox1.Text.Split('/');
            textBox1.TextChanged -= TextBox1_TextChanged;
            textBox1.Text = $"{h[2]}/{h[1]}/{h[0]}";
        }
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        // تعریف ثابت‌ها
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;  // برای نگه داشتن کلیک راست
        private const int MOUSEEVENTF_RIGHTUP = 0x10;    // برای رها کردن کلیک راست

        // تابع برای ارسال رویداد موس از کتابخانه user32.dll
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        static void LeftDoubleClick()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);

            System.Threading.Thread.Sleep(50);

            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
        static void RightClick()
        {
            // کلیک راست را نگه دارید
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);

            // کلیک راست را رها کنید
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }

        private void persianCalendar_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            MyPopupS.IsOpen = false;
            datePicker1.SelectedDate = persianCalendar.SelectedDate.ToDateTime();
            var persian = new System.Globalization.PersianCalendar();
            var h = $"{persian.GetYear(datePicker1.SelectedDate.Value)}/{persian.GetMonth(datePicker1.SelectedDate.Value)}/{persian.GetDayOfMonth(datePicker1.SelectedDate.Value)}";
            ((MyPopupS.Parent as Grid).Children[1] as TextBox).Text = h;
        }

        private void persianCalendarE_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            if (MyPopupE.IsOpen)
                datagrid.Dispatcher.BeginInvoke(new Action(async () =>
                {
                    await Task.Delay(80);
                    MyPopupE.IsOpen = false;
                }));
            datePicker2.SelectedDate = persianCalendarE.SelectedDate.ToDateTime();
            var persian = new System.Globalization.PersianCalendar();
            var h = $"{persian.GetYear(datePicker2.SelectedDate.Value)}/{persian.GetMonth(datePicker2.SelectedDate.Value)}/{persian.GetDayOfMonth(datePicker2.SelectedDate.Value)}";
            ((MyPopupE.Parent as Grid).Children[1] as TextBox).Text = h;
        }
        bool rl1, rl2 = false;

        private void persianCalendarE_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            rl2 = true;
            RightClick();
        }
        RowColumnIndex CurrentRowColumnIndex;
    
        private void datagrid_CurrentCellBeginEdit(object sender, CurrentCellBeginEditEventArgs e)
        {
            if (SearchTermTextBox.Text != "")
            {
                datagrid.SearchHelper.ClearSearch();
                SearchTermTextBox.Text = "";
            }
            CurrentRowColumnIndex = e.RowColumnIndex;
            CurrentCellText = "";
        }
        string CurrentCellText;
        private void datagrid_CurrentCellValueChanged(object sender, CurrentCellValueChangedEventArgs e)
        {
            var textBox = (datagrid.SelectionController.CurrentCellManager?.CurrentCell.Element as GridCell).Content as TextBox;
            if (textBox.Text != "" && e.Record is AcDocumentDetail detail && detail.GetType().GetProperty(e.Column.MappingName).GetValue(detail)?.ToString() != textBox.Text && !Keyboard.IsKeyDown(Key.Enter))
                CurrentCellText = textBox.Text;
        }

        private void datagrid_RowValidating(object sender, RowValidatingEventArgs e)
        {
            if (e.RowData is AcDocumentDetail detail)
            {
                var dataColumn = datagrid.SelectionController.CurrentCellManager?.CurrentCell;
                var textBox = (dataColumn.Element as GridCell).Content as TextBox;
                if (textBox == null)
                    return;
                var u = textBox.Text == "" ? CurrentCellText : textBox.Text;
                if (dataColumn.ColumnIndex == 0)
                {
                    var mu = mus1.Find(t => (t.AdditionalEntity as AccountSearchClass).ColMoein == u);
                    if (mu == null)
                    {
                        e.IsValid = false;
                        e.ErrorMessages.Add("ColeMoein", "چنین کل و معینی وجود ندارد!");
                    }
                }
                else if (dataColumn.ColumnIndex == 1)
                {
                    var mu = mus2.Find(t => t.Value == u);
                    if (mu == null)
                    {
                        e.IsValid = false;
                        e.ErrorMessages.Add("PreferentialCode", "چنین تفضیلی وجود ندارد!");
                    }
                }
            }
        }

        private void datagridSearch_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var t = datagridSearch.GetChildByName<Grid>("PART_GroupDropAreaGrid");
            if (t == null) return;
            var textBlock = (t.Children[0] as Grid).Children[1] as TextBlock;
            textBlock.Foreground = Brushes.DarkBlue;
            textBlock.FontWeight= FontWeights.Bold;
        }

        private void datagridSearch_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // ارتفاع سطرهای grid را محاسبه کنید (می‌توانید ارتفاع سطر ثابت فرض کنید)
            double rowHeight = 30; // ارتفاع هر سطر (این مقدار ممکن است بسته به طراحی تغییر کند)

            // ارتفاع موجود در grid را محاسبه کنید
            double availableHeight = datagridSearch.ActualHeight;

            // محاسبه تعداد سطرهایی که در صفحه جا می‌شوند
            int visibleRows = (int)(availableHeight / rowHeight);

            // تنظیم PageSize بر اساس تعداد سطرهای محاسبه شده
            if (visibleRows > 0)
            {
                var y = dataPager.PageSize;
                dataPager.PageSize = visibleRows - 4;
                if (dataPager.PageSize != y)
                {
                    var g = dataPager.Source;
                    dataPager.Source = null;
                    dataPager.Source = g;
                    dataPager.Visibility = Visibility.Visible;
                    datagridSearch.SearchHelper.ClearSearch();
                    SearchTermTextBox.Text = "";
                }
            }
        }

        private void persianCalendarE_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!rl2)
                e.Handled = true;
            rl2 = false;
        }

        private void dataPager_PageIndexChanged(object sender, Syncfusion.UI.Xaml.Controls.DataPager.PageIndexChangedEventArgs e)
        {
            if (SearchTermTextBox.Text.Trim() != string.Empty)
                datagridSearch.ExpandAllDetailsView();
        }

        private void persianCalendar_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!rl1)
                e.Handled = true;
            rl1 = false;
        }

        private void persianCalendar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            rl1 = true;
            RightClick();
        }

        public void SetEnterToNextCell(RowColumnIndex? rowColumn = null)
        {
            var dataGrid = datagrid;

            // پیدا کردن سطر و ستون فعلی
            var currentCell = datagrid.SelectionController.CurrentCellManager?.CurrentCell;
            if (currentCell != null)
            {
                int currentRowIndex = rowColumn == null ? currentCell.RowIndex : rowColumn.Value.RowIndex;
                int currentColumnIndex = rowColumn == null ? currentCell.ColumnIndex : rowColumn.Value.ColumnIndex;

                // افزایش اندیس ستون
                currentColumnIndex++;

                // اگر به انتهای ستون‌ها رسیدیم، به سطر بعد بروید
                if (currentColumnIndex >= dataGrid.Columns.Count)
                {
                    currentColumnIndex = 0; // به اولین ستون برگردید
                    currentRowIndex++; // به سطر بعد بروید
                }

                // اگر به انتهای سطرها رسیدیم، به اولین سطر برگردید
                if (currentRowIndex >= AcDocumentDetails.Count + 2)
                {
                    currentRowIndex = 0; // به اولین سطر برگردید
                }

                //Updates the PressedRowColumnIndex value in the GridBaseSelectionController.
                try
                {
                    if (currentColumnIndex == 2)
                        (this.datagrid.SelectionController as GridSelectionController).MoveCurrentCell(new RowColumnIndex(currentRowIndex, currentColumnIndex + 1));
                    else if (currentColumnIndex == 5 && ((datagrid.GetRecordAtRowIndex(currentRowIndex) as AcDocumentDetail)?.Debtor ?? 0) != 0)
                        (this.datagrid.SelectionController as GridSelectionController).MoveCurrentCell(new RowColumnIndex(currentRowIndex + 1, 0));
                    else
                        (this.datagrid.SelectionController as GridSelectionController).MoveCurrentCell(new RowColumnIndex(currentRowIndex, currentColumnIndex));
                }
                catch { }
            }
        }
    }
}

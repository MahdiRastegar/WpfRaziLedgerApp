using Microsoft.EntityFrameworkCore;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfRaziLedgerApp.Interfaces;
using WpfRaziLedgerApp.Windows.toolWindows;

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for winCol.xaml
    /// </summary>
    public partial class usrCommodityPricingPanel : UserControl,ITabForm
    {
        public usrCommodityPricingPanel()
        {
            CommodityPricingPanels = new ObservableCollection<CommodityPricingPanel>();
            InitializeComponent();
            txbCalender.Text = pcw1.SelectedDate.ToString();
            isCancel = true;
        }
        Brush brush = null;
        public ObservableCollection<CommodityPricingPanel> CommodityPricingPanels { get; set; }
        private void Txt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "\r")
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                request.Wrapped = true;
                (sender as TextBox).MoveFocus(request);
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (btnConfirm.IsFocused)
                    {
                        btnConfirm_Click(null, null);
                    }
                }));
                return;
            }        
            if ((sender as TextBox).Name != "txtCommodityName"&& (sender as TextBox).Name != "txtWebSite" && (sender as TextBox).Name != "txtEmail" && (sender as TextBox).Name != "txtAddress" && (sender as TextBox).Name != "txtDescription")
                e.Handled = !IsTextAllowed(e.Text);            
        }
        private static readonly Regex _regex = new Regex("[^0-9]"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CommodityPricingPanels = new ObservableCollection<CommodityPricingPanel>();
            using var db = new wpfrazydbContext();
            var count = db.CommodityPricingPanels.Count();
            var h = db.CommodityPricingPanels.Include(i=>i.FkPriceGroup).Include("FkCommodity").AsNoTracking().ToList();
            //var h = db.Commodities.Take(10).ToList();
            //if(count>10)
            //{
            //    for (int i = 0; i < count-10; i++)
            //    {
            //        h.Add(null);
            //    }
            //}
            h.ForEach(u => CommodityPricingPanels.Add(u));
            datagrid.SearchHelper.AllowFiltering = true;
            txtCommodity.Focus();
            dataPager.Source = null;
            dataPager.Source = CommodityPricingPanels;
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            bool haserror = false;
            haserror = GetError();

            if (haserror)
                return;
            using var db = new wpfrazydbContext();                    
            CommodityPricingPanel e_add = null;
            var PriceGroupcode = int.Parse(txtPriceGroup.Text);
            var Commoditycode = int.Parse(txtCommodity.Text);
            var priceGroup = db.PriceGroups.First(t => t.GroupCode == PriceGroupcode);
            var commodity = db.Commodities.First(t => t.Code == Commoditycode);
            if (id == Guid.Empty)
            {
                datagrid.SortColumnDescriptions.Clear();
                e_add = new CommodityPricingPanel()
                {
                    Id = Guid.NewGuid(),
                    FkCommodity = commodity,
                    FkPriceGroup = priceGroup,
                    Fee = decimal.Parse(txtInvoiceDiscount.Text.Replace(",", "")),
                    Date = pcw1.SelectedDate.ToDateTime()
                };
                db.CommodityPricingPanels.Add(e_add);
                CommodityPricingPanels.Add(e_add);
            }
            else
            {                
                var e_Edidet = CommodityPricingPanels.FirstOrDefault(a => a.Id == id);
                var e_Edidet2 = db.CommodityPricingPanels.FirstOrDefault(a => a.Id == id);
                e_Edidet.FkCommodity = commodity;
                e_Edidet.FkPriceGroup = priceGroup;
                e_Edidet2.Date = e_Edidet.Date = pcw1.SelectedDate.ToDateTime();
                e_Edidet2.Fee = e_Edidet.Fee = decimal.Parse(txtInvoiceDiscount.Text.Replace(",", ""));
            }
            if (!db.SafeSaveChanges())  return;
            if (id == Guid.Empty)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("اطلاعات اضافه شد.", "ثبت پنل قیمت گذاری کالا");
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("اطلاعات ویرایش شد.", "ویرایش پنل قیمت گذاری کالا");
            }
            txtCommodity.IsReadOnly = false;
            btnCancel_Click(null, null);
            datagrid.SelectedIndex = -1;
            datagrid.ClearFilters();
            datagrid.SearchHelper.ClearSearch();
            SearchTermTextBox.Text = "";            
            isCancel = true;            
            gridDelete.Visibility = Visibility.Hidden;
            borderEdit.Visibility = Visibility.Hidden;
            if (id != Guid.Empty)
                txtCommodity.Focus();
            else
            {
                txtCommodity.Focus();
                dataPager.MoveToLastPage();
            }

            id = Guid.Empty;
        }
        Guid id = Guid.Empty;
        private bool GetError()
        {
            var haserror = false;
            if (txtCommodity.Text.Trim() == "")
            {
                Sf_txtCommodity.HasError = true;
                haserror = true;
            }
            else
                Sf_txtCommodity.HasError = false;            
            if (txtPriceGroup.Text.Trim() == "")
            {
                Sf_txtPriceGroup.HasError = true;
                haserror = true;
            }
            else
            {
                Sf_txtPriceGroup.HasError = false;
                Sf_txtPriceGroup.ErrorText = "";
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
//                Border_MouseDown("mu", null);
            }
        }

        private void datagrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {


        }

        private void datagrid_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {

        }

        private void btnTransferOfExcel_Click(object sender, RoutedEventArgs e)
        {

        }
        bool forceClose = false;
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.Key == Key.F1)
            {
                if (txtCommodity.IsFocused && !txtCommodity.IsReadOnly)
                {
                    if (window != null)
                        return;
                    /*Point relativePoint = y.TransformToAncestor(this)
                              .Transform(new Point(this.Left+Width, this.Top-Height));*/
                    isCancel = false;
                    Point relativePoint = new Point(MainWindow.Current.Left + Width - 500, MainWindow.Current.Top + 50);
                    if (MainWindow.Current.WindowState == WindowState.Maximized)
                        relativePoint = txtCommodity.TransformToAncestor(this)
                              .Transform(new Point(530, 0));
                    using var db = new wpfrazydbContext();
                    var list = db.Commodities.ToList().Select(r => new Mu() { Name = r.Name, Value = r.Code.ToString() }).ToList();
                    var win = new winSearch(list);
                    win.Tag = this;
                    win.ParentTextBox = txtCommodity;
                    win.SearchTermTextBox.Text = txtCommodity.Text;
                    win.SearchTermTextBox.Select(1, 0);
                    win.Owner = MainWindow.Current;
                    //win.Left = relativePoint.X - 60;
                    //win.Top = relativePoint.Y + 95;
                    window = win;
                    win.Show(); 
                    win.Focus();
                }
                else if (txtPriceGroup.IsFocused && !txtPriceGroup.IsReadOnly)
                {
                    if (window != null)
                        return;
                    /*Point relativePoint = y.TransformToAncestor(this)
                              .Transform(new Point(this.Left+Width, this.Top-Height));*/
                    isCancel = false;
                    Point relativePoint = new Point(MainWindow.Current.Left + Width - 500, MainWindow.Current.Top + 50);
                    if (MainWindow.Current.WindowState == WindowState.Maximized)
                        relativePoint = txtPriceGroup.TransformToAncestor(this)
                              .Transform(new Point(530, 0));
                    using var db = new wpfrazydbContext();
                    var list = db.PriceGroups.ToList().Select(r => new Mu() { Name = r.GroupName, Value = r.GroupCode.ToString() }).ToList();
                    var win = new winSearch(list);
                    win.Tag = this;
                    win.ParentTextBox = txtPriceGroup;
                    win.SearchTermTextBox.Text = txtPriceGroup.Text;
                    win.SearchTermTextBox.Select(1, 0);
                    win.Owner = MainWindow.Current;
                    //win.Left = relativePoint.X - 60;
                    //win.Top = relativePoint.Y + 95;
                    window = win;
                    win.Show();
                    win.Focus();
                }
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
        private bool _iscancel=false;

        public bool isCancel
        {
            get 
            {
                return _iscancel; 
            }
            set
            {
                _iscancel = value;
                if (gridContainer != null)
                {
                    gridContainer.Opacity = .6;
                    gridContainer.IsEnabled = false;
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (isCancel&& sender != null&&id== Guid.Empty)
            {
                gridContainer.Opacity = 1;
                gridContainer.IsEnabled = true;
                return;
            }
            if (!isCancel && sender != null && Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید از این عملیات انصراف دهید؟", "انصراف", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }
            txtCommodity.IsReadOnly = false;
            txtCommodityName.Text = "";
            txtCommodity.Text = "";
            txtInvoiceDiscount.Text = "0";
            Sf_txtCommodityName.HasError = false;
            Sf_txtCommodity.HasError = false;
            Sf_txtCommodity.ErrorText = "";
            //txtCommodity.Text = (en.Code + 1).ToString();
            Sf_txtPriceGroup.HasError = false;
            txtPriceGroup.Text = "";
            txtPriceGroupName.Text = "";

            txtCommodity.Focus();
            datagrid.SelectedIndex = -1;
            datagrid.ClearFilters();
            datagrid.SearchHelper.ClearSearch();
            SearchTermTextBox.Text = "";
            gridDelete.Visibility = Visibility.Hidden;
            borderEdit.Visibility = Visibility.Hidden;
            txtCommodity.TextChanged -= TxtGroup_TextChanged;
            txtCommodityName.Text = txtCommodity.Text;
            txtCommodity.TextChanged += TxtGroup_TextChanged;
            isCancel = true;
            if (sender != null)
            {
                if (id == Guid.Empty)
                {
                    gridContainer.Opacity = 1;
                    gridContainer.IsEnabled = true;
                }
                id = Guid.Empty;
            }
        }

        private void datagrid_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            if (isCancel && datagrid.SelectedItem != null)
            {
                var commodityPricingPanel = datagrid.SelectedItem as CommodityPricingPanel;
                id = commodityPricingPanel.Id;
                txtCommodity.TextChanged -= TxtGroup_TextChanged;
                txtCommodity.Text = commodityPricingPanel.FkCommodity.Code.ToString();
                txtCommodity.TextChanged += TxtGroup_TextChanged;
                txtCommodityName.Text = commodityPricingPanel.FkCommodity.Name;
                txtPriceGroup.Text = commodityPricingPanel.FkPriceGroup.GroupCode.ToString();
                txtPriceGroupName.Text = commodityPricingPanel.FkPriceGroup.GroupName.ToString();
                pcw1.SelectedDate = new PersianCalendarWPF.PersianDate(commodityPricingPanel.Date);
                txbCalender.Text = pcw1.SelectedDate.ToString();
                if (commodityPricingPanel.Fee.HasValue)
                    txtInvoiceDiscount.Text = commodityPricingPanel.Fee.Value.ToString();
                else
                    txtInvoiceDiscount.Text = "0";

                gridDelete.Visibility = Visibility.Visible;
                borderEdit.Visibility = Visibility.Visible;
                txtCommodity.IsReadOnly = true;
                txtCommodity.IsReadOnly = true;
                isCancel = true;
                GetError();
                gridContainer.Opacity = 1;
                gridContainer.IsEnabled = true;
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (datagrid.SelectedItem == null)
                return;
            if (Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید این اطلاعات پاک شود؟", "حذف", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }
            using var db = new wpfrazydbContext();
            var commodityPricingPanel = db.CommodityPricingPanels.Find(id);
            //if (db.AcDocument_Detail.Any(y => y.FkCommodityId == Commodity.Id))
            //{
            //    Xceed.Wpf.Toolkit.MessageBox.Show("قبلا با این کالا سند حسابداری زده شده است و قابل حذف نیست!");
            //    return;
            //}
            
            db.CommodityPricingPanels.Remove(commodityPricingPanel);
            if (!db.SafeSaveChanges())  return;
            id = Guid.Empty;
            CommodityPricingPanels.Remove((datagrid.SelectedItem as CommodityPricingPanel));            
            btnCancel_Click(null, null);
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
            catch(Exception ex)
            {
            }
        }

        private void txtCommodityName_TextChanged(object sender, TextChangedEventArgs e)
        {
            isCancel = false;
        }

        private void TxtCodeCommodity_TextChanged(object sender, TextChangedEventArgs e)
        {
            isCancel = false;
        }
        public static Window window;
        private void TxtGroup_TextChanged(object sender, TextChangedEventArgs e)
        {
            isCancel = false;
        }

        private void TxtGroup_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtCommodity.IsReadOnly)
                return;
            if (txtCommodity.Text == "")
            {
                txtCommodity.Text = string.Empty;
                txtCommodityName.Text = string.Empty;
                return;
            }
            using var db = new wpfrazydbContext();
            var code = int.Parse(txtCommodity.Text);
            var mu = db.Commodities.FirstOrDefault(t => t.Code == code);
            if (mu == null)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("چنین کد کالا وجود ندارد!");
                txtCommodity.Text = txtCommodityName.Text = string.Empty;
            }
            else
            {
                txtCommodityName.Text = mu.Name;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    txtPriceGroup.Focus();
                }));
            }
        }

        private void DataPager_PageIndexChanging(object sender, Syncfusion.UI.Xaml.Controls.DataPager.PageIndexChangingEventArgs e)
        {
            var ex = datagrid.View.FilterPredicates;
            
            using var db = new wpfrazydbContext();
            //db.Commodities.Where(ex)
            var count = db.CommodityPricingPanels.Count();
            var F = db.CommodityPricingPanels.OrderBy(d=>d.Id).Skip(10 * e.NewPageIndex).Take(10).ToList();
            int j = 0;
            for (int i = 10 * e.NewPageIndex; i < 10 * (e.NewPageIndex + 1)&&i<count; i++)
            {
                CommodityPricingPanels[i] = F[j];
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
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "پنل قیمت گذاری کالا");
            MainWindow.Current.tabcontrol.Items.Remove(item);
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
            datagrid.AllowFiltering = !datagrid.AllowFiltering;
            if (!datagrid.AllowFiltering)
                datagrid.ClearFilters();
        }

        public void SetNull()
        {
            bool pc=false;
            if ((window as winSearch).ParentTextBox == txtPriceGroup)
            {
                pc = true;
            }
            window = null;

            try
            {
                using var db = new wpfrazydbContext();
                if (!pc)
                {
                    var g = int.Parse(txtCommodity.Text);

                    var y = db.Commodities.FirstOrDefault(gs => gs.Code == g);
                    if (y != null)
                        Dispatcher.BeginInvoke(new Action(async () =>
                        {
                            await Task.Delay(50);
                            txtPriceGroup.Focus();
                        }));
                }
                else
                {
                    var g = int.Parse(txtPriceGroup.Text);

                    var y = db.PriceGroups.FirstOrDefault(gs => gs.GroupCode == g);
                    if (y != null)
                        Dispatcher.BeginInvoke(new Action(async () =>
                        {
                            await Task.Delay(50);
                            btnConfirm.Focus();
                        }));
                }
            }
            catch { }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {/*
            using var db = new wpfrazydbContext();    
            foreach (var item in db.Commodities.ToList())
            {
                foreach (var item2 in db.MoeinGroup.Where(t => t.FkGroupId == item.FkGroupId))
                {
                    db.Account.Add(new Account()
                    {
                        Id = Guid.NewGuid(),
                        fk_ColId = item2.Moein.fk_ColId,
                        FkCommodityId = item.Id,
                        FkMoeinId = item2.FkMoeinId
                    });
                }
            }
            if (!db.SafeSaveChanges())  return;*/
        }

        private void txtEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            InputLanguageManager.Current.CurrentInputLanguage = new CultureInfo("en-US");
        }

        private void txtWebSite_GotFocus(object sender, RoutedEventArgs e)
        {
            InputLanguageManager.Current.CurrentInputLanguage = new CultureInfo("en-US");
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
                dataPager.PageSize = visibleRows-2;
                var g = dataPager.Source;
                dataPager.Source = null;
                dataPager.Source = g;
            }
        }

        private void txtCommodity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "\r")
            {
                txtCommodityName.Focus();

                return;
            }
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void txtPriceGroup_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "\r")
            {
                btnConfirm.Focus();
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (btnConfirm.IsFocused)
                    {
                        btnConfirm_Click(null, null);
                    }
                }));
                return;
            }
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void txtPriceGroup_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtPriceGroup.Text == "")
            {
                txtPriceGroup.Text = string.Empty;
                txtPriceGroupName.Text = string.Empty;
                return;
            }
            using var db = new wpfrazydbContext();
            var code=int.Parse(txtPriceGroup.Text);
            var mu = db.PriceGroups.FirstOrDefault(t =>  t.GroupCode == code);
            if (mu == null)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("چنین گروه قیمت وجود ندارد!");
                txtPriceGroup.Text = txtPriceGroupName.Text = string.Empty;
            }
            else
            {
                txtPriceGroupName.Text = mu.GroupName;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    txtInvoiceDiscount.Focus();
                    txtInvoiceDiscount.SelectAll();
                }));
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

        private void txtInvoiceDiscount_TextChanged(object sender, TextChangedEventArgs e)
        {
            isCancel = false;
            var textbox = sender as TextBox;
            decimal ds = 0;
            if (decimal.TryParse(textbox.Text.Trim().Replace(",", ""), out ds) && ds >= 0)
            {

                int temp = textbox.SelectionStart;
                textbox.TextChanged -= txtInvoiceDiscount_TextChanged;
                textbox.Text = string.Format("{0:#,###}", ds);
                if (textbox.SelectionStart != temp)
                    textbox.SelectionStart = temp + 1;
                if (textbox.Text == "")
                    textbox.Text = "0";
                textbox.TextChanged += txtInvoiceDiscount_TextChanged;
            }
        }
        private void txtInvoiceDiscount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var txt = sender as TextBox;

            if (e.Text == "\r")
            {
                btnConfirm.Focus();
                return;
            }
            e.Handled = !IsTextAllowed(e.Text);
            if (txt.Text == "")
                txt.Text = "0";
        }

        private void txtInvoiceDiscount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
                return;
            }
        }

        private void txtInvoiceDiscount_LostFocus(object sender, RoutedEventArgs e)
        {
            var txt = sender as TextBox;
            if (txt.Text == "")
                txt.Text = "0";            
        }
    }
}

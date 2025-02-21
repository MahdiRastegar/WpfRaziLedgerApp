using Microsoft.EntityFrameworkCore;
using Syncfusion.Windows.Tools.Controls;
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
    public partial class usrPreferential : UserControl,ITabForm
    {
        public usrPreferential()
        {
            Preferentials = new ObservableCollection<Preferential>();
            InitializeComponent();
            isCancel = true;
        }
        Brush brush = null;
        public ObservableCollection<Preferential> Preferentials { get; set; }
        private void Txt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "\r")
            {
                if ((sender as TextBox).Name == "txtPreferentialName")
                {
                    btnConfirm.Focus();
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
            if ((sender as TextBox).Name != "txtPreferentialName"&& (sender as TextBox).Name != "txtWebSite" && (sender as TextBox).Name != "txtEmail" && (sender as TextBox).Name != "txtAddress" && (sender as TextBox).Name != "txtDescription")
                e.Handled = !IsTextAllowed(e.Text);            
        }
        private static readonly Regex _regex = new Regex("[^0-9]"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Preferentials = new ObservableCollection<Preferential>();
            using var db = new wpfrazydbContext();
            cmbProvince.ItemsSource = db.Provinces.AsNoTracking().ToList();            

            var count = db.Preferentials.Count();
            var h = db.Preferentials.Include(j=>j.FkCity).Include(d=>d.FkCity.FkProvince).Include("FkGroup").AsNoTracking().ToList();
            //var h = db.Preferentials.Take(10).ToList();
            //if(count>10)
            //{
            //    for (int i = 0; i < count-10; i++)
            //    {
            //        h.Add(null);
            //    }
            //}
            h.ForEach(u => Preferentials.Add(u));
            datagrid.SearchHelper.AllowFiltering = true;
            txtGroup.Focus();
            dataPager.Source = null;
            dataPager.Source = Preferentials;
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            bool haserror = false;
            haserror = GetError();

            if (haserror)
                return;
            using var db = new wpfrazydbContext();
            var c = int.Parse(txtGroup.Text);
            var col = db.TGroups.FirstOrDefault(g => g.GroupCode == c);
            if (col == null)
            {
                Sf_txtGroup.ErrorText = "این کد گروه وجود ندارد";
                Sf_txtGroup.HasError = true;
                return;
            }
            var i = int.Parse(txtCodePreferential.Text);
            var preferential = db.Preferentials.Find(id);
            if (id == Guid.Empty)
            {
                if (db.Preferentials.Any(u => u.PreferentialCode == i))
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("کد تفضیلی تکراریست!");
                    return;
                }
            }
            else
            {
                if (i != preferential.PreferentialCode && db.Preferentials.Any(u => u.PreferentialCode == i))
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("کد تفضیلی تکراریست!");
                    return;
                }
            }
            //var mpreferential = db.Preferentials.FirstOrDefault(g => g.FkGroupId == col.Id && g.PreferentialCode == i);
            //if (preferential?.Id != mpreferential?.Id && mpreferential != null)
            //{
            //    Xceed.Wpf.Toolkit.MessageBox.Show("این کد تفضیلی و کد گروه از قبل وجود داشته است!");
            //    return;
            //}    
            var npreferential = db.Preferentials.FirstOrDefault(g => g.FkGroupId == col.Id && g.PreferentialName == txtPreferentialName.Text);
            if (preferential?.Id != npreferential?.Id && npreferential != null)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("این نام تفضیلی و کد گروه از قبل وجود داشته است!");
                return;
            }

            Preferential e_add = null;
            if (id == Guid.Empty)
            {
                datagrid.SortColumnDescriptions.Clear();
                e_add = new Preferential()
                {
                    Id = Guid.NewGuid(),
                    PreferentialCode = i,
                    FkGroupId = col.Id,
                    PreferentialName = txtPreferentialName.Text,
                    Mobile = txtMobile.Text,
                    Phone1 = txtPhone1.Text,
                    Phone2 = txtPhone2.Text,
                    Phone3 = txtPhone3.Text,
                    WebSite = txtWebSite.Text,
                    Email = txtEmail.Text,
                    Address = txtAddress.Text,
                    Description = txtDescription.Text,
                    FkCityId = (cmbCity.SelectedItem as City)?.Id
                };
                db.Preferentials.Add(e_add);
                Preferentials.Add(e_add);
            }
            else
            {                
                var e_Edidet = Preferentials.FirstOrDefault(a => a.Id == id);
                e_Edidet.FkGroupId = preferential.FkGroupId = col.Id;
                e_Edidet.PreferentialCode = preferential.PreferentialCode = i;
                e_Edidet.PreferentialName = preferential.PreferentialName = txtPreferentialName.Text;
                e_Edidet.FkGroup.GroupName = txtGroupName.Text;

                e_Edidet.Mobile = preferential.Mobile = txtMobile.Text;
                e_Edidet.Phone1 = preferential.Phone1 = txtPhone1.Text;
                e_Edidet.Phone2 = preferential.Phone2 = txtPhone2.Text;
                e_Edidet.Phone3 = preferential.Phone3 = txtPhone3.Text;
                e_Edidet.WebSite = preferential.WebSite = txtWebSite.Text;
                e_Edidet.Email = preferential.Email = txtEmail.Text;
                e_Edidet.Address = preferential.Address = txtAddress.Text;
                e_Edidet.Description = preferential.Description = txtDescription.Text;
                preferential.FkCityId = (cmbCity.SelectedItem as City)?.Id;
                e_Edidet.FkCity = cmbCity.SelectedItem as City;
                if (e_Edidet.FkCity != null)
                    e_Edidet.FkCity.FkProvince = cmbProvince.SelectedItem as Province;
            }
            if (!db.SafeSaveChanges())  return;
            if (id == Guid.Empty)
            {
                e_add.FkCity = cmbCity.SelectedItem as City;
                Xceed.Wpf.Toolkit.MessageBox.Show("اطلاعات اضافه شد.", "ثبت تفضیلی");
                txtCodePreferential.Text = (e_add.PreferentialCode + 1).ToString();
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("اطلاعات ویرایش شد.", "ویرایش تفضیلی");
                btnCancel_Click(null, null);
            }
            datagrid.SelectedIndex = -1;
            datagrid.ClearFilters();
            datagrid.SearchHelper.ClearSearch();
            SearchTermTextBox.Text = "";            
            ClearMore();
            txtCodePreferential.IsReadOnly = false;
            txtGroup.IsReadOnly = false;
            txtPreferentialName.Text = "";
            isCancel = true;            
            gridDelete.Visibility = Visibility.Hidden;
            borderEdit.Visibility = Visibility.Hidden;
            if (id != Guid.Empty)
                txtGroup.Focus();
            else
            {
                txtPreferentialName.Focus();
                dataPager.MoveToLastPage();
            }

            id = Guid.Empty;
        }
        Guid id = Guid.Empty;
        private bool GetError()
        {
            var haserror = false;
            if (txtCodePreferential.Text.Trim() == "")
            {
                Sf_txtCodePreferential.HasError = true;
                haserror = true;
            }
            else
                Sf_txtCodePreferential.HasError = false;
            if (txtPreferentialName.Text.Trim() == "")
            {
                Sf_txtPreferentialName.HasError = true;
                haserror = true;
            }
            else
                Sf_txtPreferentialName.HasError = false;
            if (txtGroup.Text.Trim() == "")
            {
                Sf_txtGroup.HasError = true;
                haserror = true;
            }
            else
            {
                Sf_txtGroup.HasError = false;
                Sf_txtGroup.ErrorText = "";
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
            if (e.Key == Key.Escape)
            {
                CloseForm();
            }
            else if (e.Key == Key.F1 && txtGroup.IsFocused && !txtGroup.IsReadOnly)
            {
                if (window != null)
                    return;
                /*Point relativePoint = y.TransformToAncestor(this)
                          .Transform(new Point(this.Left+Width, this.Top-Height));*/
                isCancel = false;
                Point relativePoint = new Point(MainWindow.Current.Left + Width - 500, MainWindow.Current.Top + 50);
                if (MainWindow.Current.WindowState == WindowState.Maximized)
                    relativePoint = txtGroup.TransformToAncestor(this)
                          .Transform(new Point(530, 0));
                using var db = new wpfrazydbContext();
                var list = db.TGroups.ToList().Select(r => new Mu() { Name = r.GroupName, Value = r.GroupCode.ToString() }).ToList();
                var win = new winSearch(list);
                win.Tag = this;
                win.ParentTextBox = txtGroup;
                win.SearchTermTextBox.Text = txtGroup.Text;
                win.SearchTermTextBox.Select(1, 0);
                win.Owner = MainWindow.Current;
                //win.Left = relativePoint.X - 60;
                //win.Top = relativePoint.Y + 95;
                window = win;
                win.Show(); win.Focus();
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

                gridContainer.Opacity = .6;
                gridContainer.IsEnabled = false;
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
            txtCodePreferential.IsReadOnly = false;
            txtGroup.IsReadOnly = false;
            txtPreferentialName.Text = "";
            Sf_txtPreferentialName.HasError = false;
            Sf_txtCodePreferential.HasError = false;
            Sf_txtGroup.HasError = false;
            Sf_txtGroup.ErrorText = "";
            ClearMore();
            //txtCodePreferential.Text = (en.PreferentialCode + 1).ToString();

            txtGroup.Focus();
            datagrid.SelectedIndex = -1;
            datagrid.ClearFilters();
            datagrid.SearchHelper.ClearSearch();
            SearchTermTextBox.Text = "";
            gridDelete.Visibility = Visibility.Hidden;
            borderEdit.Visibility = Visibility.Hidden;
            txtGroup.TextChanged -= TxtGroup_TextChanged;
            txtGroupName.Text = txtGroup.Text = txtCodePreferential.Text = "";
            txtGroup.TextChanged += TxtGroup_TextChanged;
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

        private void ClearMore()
        {
            txtMobile.Text = "";
            txtPhone1.Text = "";
            txtPhone2.Text = "";
            txtPhone3.Text = "";
            txtWebSite.Text = "";
            txtEmail.Text = "";
            txtAddress.Text = "";
            txtDescription.Text = "";
            cmbCity.SelectedItem = null;
            cmbProvince.SelectedItem = null;
        }

        private void datagrid_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            if (isCancel&&datagrid.SelectedItem!=null) 
            {
                var preferential = datagrid.SelectedItem as Preferential;
                id = preferential.Id;
                txtGroup.TextChanged -= TxtGroup_TextChanged;
                txtGroup.Text = preferential.FkGroup.GroupCode.ToString();
                txtGroup.TextChanged += TxtGroup_TextChanged;
                txtGroupName.Text = preferential.FkGroup.GroupName;
                txtPreferentialName.Text = preferential.PreferentialName;
                txtCodePreferential.Text = preferential.PreferentialCode.ToString();

                txtMobile.Text = preferential.Mobile;
                txtPhone1.Text = preferential.Phone1;
                txtPhone2.Text = preferential.Phone2;
                txtPhone3.Text = preferential.Phone3;
                txtWebSite.Text = preferential.WebSite;
                txtEmail.Text = preferential.Email;
                txtAddress.Text = preferential.Address;
                txtDescription.Text = preferential.Description;
                if (preferential.FkCity != null)
                {
                    cmbCity.SelectionChanged -= cmbProvince_SelectionChanged;
                    cmbProvince.SelectionChanged -= cmbProvince_SelectionChanged;
                    cmbProvince.SelectedItem = (cmbProvince.ItemsSource as List<Province>).First(u => u.Id == preferential.FkCity?.FkProvince.Id);
                    var id = (cmbProvince.SelectedItem as Province).Id;
                    using var db = new wpfrazydbContext();
                    cmbCity.ItemsSource = db.Cities.AsNoTracking().Where(y => y.FkProvinceId == id).ToList();
                    cmbCity.SelectedItem = (cmbCity.ItemsSource as List<City>).First(u => u.Id == preferential.FkCity.Id);
                    cmbCity.SelectionChanged += cmbProvince_SelectionChanged;
                    cmbProvince.SelectionChanged += cmbProvince_SelectionChanged;
                }
                else
                {
                    cmbCity.SelectedItem = null;
                    cmbProvince.SelectedItem = null;
                }
                gridDelete.Visibility = Visibility.Visible;
                borderEdit.Visibility = Visibility.Visible;
                txtCodePreferential.IsReadOnly = true;
                txtGroup.IsReadOnly = true;
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
            var preferential = db.Preferentials.Find(id);
            if (db.AcDocumentDetails.Any(y => y.FkPreferentialId == preferential.Id))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("قبلا با این تفضیلی سند حسابداری زده شده است و قابل حذف نیست!");
                return;
            }
            var listPreferentials = new List<string>();
            foreach (var item in db.CodeSettings.Where(s => s.Name.Contains("PreferentialCode")).ToList())
            {
                var code = db.CodeSettings.FirstOrDefault(j => j.Name == item.Name);
                if (code != null)
                {
                    listPreferentials.Add(item.Value);
                }
            }
            if (listPreferentials.Contains(preferential.PreferentialCode.ToString()))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("در تنظیمات پیکربندی از این تفضیل استفاده شده است و قابل حذف نیست!");
                return;
            }
            db.Preferentials.Remove(preferential);
            if (!db.SafeSaveChanges())  return;
            Preferentials.Remove((datagrid.SelectedItem as Preferential));            
            btnCancel_Click(null, null);
            id = Guid.Empty;
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

        private void txtPreferentialName_TextChanged(object sender, TextChangedEventArgs e)
        {
            isCancel = false;
        }

        private void TxtCodePreferential_TextChanged(object sender, TextChangedEventArgs e)
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
            if (!isCancel && !txtCodePreferential.IsReadOnly)
                try
                {
                    using var db = new wpfrazydbContext();
                    var g = int.Parse(txtGroup.Text);
                    var group = db.TGroups.FirstOrDefault(gs => gs.GroupCode == g);
                    txtGroupName.Text = group.GroupName;
                    try
                    {
                        txtCodePreferential.Text = (db.Preferentials.Include("FkGroup").Where(u => u.FkGroup.GroupCode == g).Max(y => y.PreferentialCode) + 1).ToString();
                    }
                    catch
                    {
                        txtCodePreferential.Text = ((group.GroupCode * 10000) + 1).ToString();
                    }
                }
                catch
                {
                    txtCodePreferential.Text = "1";
                    txtGroupName.Text = "";
                }
        }

        private void DataPager_PageIndexChanging(object sender, Syncfusion.UI.Xaml.Controls.DataPager.PageIndexChangingEventArgs e)
        {
            var ex = datagrid.View.FilterPredicates;
            
            using var db = new wpfrazydbContext();
            //db.Preferentials.Where(ex)
            var count = db.Preferentials.Count();
            var F = db.Preferentials.OrderBy(d=>d.Id).Skip(10 * e.NewPageIndex).Take(10).ToList();
            int j = 0;
            for (int i = 10 * e.NewPageIndex; i < 10 * (e.NewPageIndex + 1)&&i<count; i++)
            {
                Preferentials[i] = F[j];
                j++;
            }
        }
        public bool CloseForm()
        {
            if (Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید از این فرم خارج شوید؟", "خروج", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return false;
            }
            forceClose = true;
            var list = MainWindow.Current.GetTabControlItems;
            var item = list.FirstOrDefault(u => u.Header == "حساب تفضیلی");
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
            window = null;

            try
            {
                using var db = new wpfrazydbContext();
                var g = int.Parse(txtGroup.Text);

                var y = db.TGroups.FirstOrDefault(gs => gs.GroupCode == g);
                if (y != null)
                    Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        await Task.Delay(50);
                        txtPreferentialName.Focus();
                    }));
            }
            catch { }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {/*
            using var db = new wpfrazydbContext();    
            foreach (var item in db.Preferentials.ToList())
            {
                foreach (var item2 in db.MoeinsGroup.Where(t => t.FkGroupId == item.FkGroupId))
                {
                    db.Account.Add(new Account()
                    {
                        Id = Guid.NewGuid(),
                        FkColId = item2.Moein.FkColId,
                        FkPreferentialId = item.Id,
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

        private void txtGroup_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "\r")
            {
                txtPreferentialName.Focus();

                return;
            }
            e.Handled = !IsTextAllowed(e.Text);
        }
        private void cmbProvince_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBoxAdv;
            isCancel = false;
            switch (cmb.Name)
            {
                case "cmbProvince":
                    if (cmb.SelectedIndex != -1)
                    {
                        cmbCity.Focus();
                        using var db = new wpfrazydbContext();
                        var id = (cmbProvince.SelectedItem as Province).Id;
                        cmbCity.ItemsSource = db.Cities.Include(y=>y.FkProvince).AsNoTracking().Where(y=>y.FkProvinceId==id).ToList();
                    }
                    break;
                case "cmbCity":
                    if (cmb.SelectedIndex != -1)
                    {
                        txtPhone1.Focus();
                    }
                    break;

            }
        }

        private void cmbProvince_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var cmb = sender as ComboBoxAdv;
            if (e.Key == Key.Enter)
            {
                switch (cmb.Name)
                {
                    case "cmbProvince":
                        Dispatcher.BeginInvoke(new Action(async () =>
                        {
                            await Task.Delay(50);
                            cmbCity.Focus();
                            using var db = new wpfrazydbContext();
                            var id = (cmbProvince.SelectedItem as Province).Id;
                            cmbCity.ItemsSource = db.Cities.AsNoTracking().Where(y => y.FkProvinceId == id).ToList();
                        }));
                        break;
                    case "cmbCity":
                        Dispatcher.BeginInvoke(new Action(async () =>
                        {
                            await Task.Delay(50);
                            txtPhone1.Focus();
                        }));
                        break;

                }
                return;
            }
            cmb.SelectedIndex = -1;
        }

        private void cmbProvince_LostFocus(object sender, RoutedEventArgs e)
        {
            var cmb=sender as ComboBoxAdv;
            if (cmb.SelectedIndex == -1)
                cmb.Text = "";
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using Point = System.Windows.Point;

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for winCol.xaml
    /// </summary>
    public partial class winMoein : UserControl,ITabForm
    {
        public winMoein()
        {
            Moeins = new ObservableCollection<Moein>();
            InitializeComponent();
            isCancel = true;
        }
        Brush brush = null;
        public ObservableCollection<Moein> Moeins { get; set; }
        private void Txt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "\r")
            {
                if ((sender as TextBox).Name == "txtCol")
                {
                    txtMoeinName.Focus();
                    return;
                }
                else if ((sender as TextBox).Name == "txtMoeinName")
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
            if ((sender as TextBox).Name != "txtMoeinName")
                e.Handled = !IsTextAllowed(e.Text);            
        }
        private static readonly Regex _regex = new Regex("[^0-9]"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Moeins = new ObservableCollection<Moein>();
            using var db = new wpfrazydbContext();
            var count = db.Moeins.Count();
            var h = db.Moeins.Include(y=>y.FkCol).AsNoTracking().ToList();
            //var h = db.Moeins.Take(10).ToList();
            //if(count>10)
            //{
            //    for (int i = 0; i < count-10; i++)
            //    {
            //        h.Add(null);
            //    }
            //}
            h.ForEach(u => Moeins.Add(u));
            datagrid.SearchHelper.AllowFiltering = true;
            //checkListBox.ItemsSource = db.TGroups.ToList();
            txtCol.Focus();
            dataPager.Source = null;
            dataPager.Source = Moeins;
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            bool haserror = false;
            haserror = GetError();

            if (haserror)
                return;
            using var db = new wpfrazydbContext();
            var c = int.Parse(txtCol.Text);
            var col = db.Cols.FirstOrDefault(g => g.ColCode == c);
            if (col == null)
            {
                Sf_txtCol.ErrorText = "این کد کل وجود ندارد";
                Sf_txtCol.HasError = true;
                return;
            }
            var i = int.Parse(txtCodeMoein.Text);
            var moein = db.Moeins.Find(id);
            //var mmoein = db.Moeins.FirstOrDefault(g => g.FkColId == col.Id && g.MoeinCode == i);
            //if (moein?.Id != mmoein?.Id && mmoein != null)
            //{
            //    Xceed.Wpf.Toolkit.MessageBox.Show("این کد معین و کد کل از قبل وجود داشته است!");
            //    return;
            //}    
            var nmoein = db.Moeins.FirstOrDefault(g => g.FkColId == col.Id && g.MoeinName == txtMoeinName.Text);
            if (moein?.Id != nmoein?.Id && nmoein != null)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("این نام معین و کد کل از قبل وجود داشته است!");
                return;
            }
            Moein e_add = null;
            if (id == Guid.Empty)
            {
                datagrid.SortColumnDescriptions.Clear();
                e_add = new Moein()
                {
                    Id = Guid.NewGuid(),
                    MoeinCode = i,
                    FkColId = col.Id,
                    MoeinName = txtMoeinName.Text
                };
                db.Moeins.Add(e_add);
                Moeins.Add(e_add);
                //foreach (var item in checkListBox.SelectedItems)//GroupDeleted
                //{
                //    var group = item as TGroup;
                //    db.MoeinsGroup.Add(new MoeinGroup()
                //    {
                //        Id = Guid.NewGuid(),
                //        Moein = e_add,
                //        FkGroupId = group.Id
                //    });
                //}                
            }
            else
            {                
                var e_Edidet = Moeins.FirstOrDefault(a => a.Id == id);
                e_Edidet.FkColId = moein.FkColId = col.Id;
                e_Edidet.MoeinName = moein.MoeinName = txtMoeinName.Text;
                //db.MoeinsGroup.RemoveRange(db.MoeinsGroup.Where(j => j.FkMoeinId == id));//GroupDeleted
                //foreach (var item in checkListBox.SelectedItems)
                //{
                //    var group = item as TGroup;
                //    db.MoeinsGroup.Add(new MoeinGroup()
                //    {
                //        Id = Guid.NewGuid(),
                //        FkMoeinId = id,
                //        FkGroupId = group.Id
                //    });
                //}
            }
            if (!db.SafeSaveChanges())  return;
            if (id == Guid.Empty)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("اطلاعات اضافه شد.", "ثبت معین");
                txtCodeMoein.Text = (e_add.MoeinCode + 1).ToString();
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("اطلاعات ویرایش شد.", "ویرایش معین");
                btnCancel_Click(null, null);
            }
            datagrid.SelectedIndex = -1;
            datagrid.ClearFilters();
            datagrid.SearchHelper.ClearSearch();
            SearchTermTextBox.Text = "";                            
            txtCodeMoein.IsReadOnly = false;
            txtCol.IsReadOnly = false;
            txtMoeinName.Text = "";
            isCancel = true;            
            gridDelete.Visibility = Visibility.Hidden;
            borderEdit.Visibility = Visibility.Hidden;
            if (id != Guid.Empty)
                txtCol.Focus();
            else
            {
                txtMoeinName.Focus();
                dataPager.MoveToLastPage();
            }
            id = Guid.Empty;
        }
        Guid id = Guid.Empty;
        private bool GetError()
        {
            var haserror = false;
            if (txtCodeMoein.Text.Trim() == "")
            {
                Sf_txtCodeMoein.HasError = true;
                haserror = true;
            }
            else
                Sf_txtCodeMoein.HasError = false;
            if (txtMoeinName.Text.Trim() == "")
            {
                Sf_txtMoeinName.HasError = true;
                haserror = true;
            }
            else
                Sf_txtMoeinName.HasError = false;
            if (txtCol.Text.Trim() == "")
            {
                Sf_txtCol.HasError = true;
                haserror = true;
            }
            else
            {
                Sf_txtCol.HasError = false;
                Sf_txtCol.ErrorText = "";
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
            else if (e.Key == Key.F1 && txtCol.IsFocused && !txtCol.IsReadOnly)
            {
                if (window != null)
                    return;
                /*Point relativePoint = y.TransformToAncestor(this)
                          .Transform(new Point(this.Left+Width, this.Top-Height));*/
                isCancel = false;
                Point relativePoint = new Point(MainWindow.Current.Left + Width - 500, MainWindow.Current.Top + 50);
                if (MainWindow.Current.WindowState == WindowState.Maximized)
                    relativePoint = txtCol.TransformToAncestor(this)
                          .Transform(new Point(530, 0));
                using var db = new wpfrazydbContext();
                var list = db.Cols.ToList().Select(r => new Mu() { Name = r.ColName, Value = r.ColCode.ToString() }).ToList();
                var win = new winSearch(list);
                win.Tag = this;
                win.ParentTextBox = txtCol;
                win.SearchTermTextBox.Text = txtCol.Text;
                win.SearchTermTextBox.Select(1, 0);
                win.Owner = MainWindow.Current;
                //win.Left = relativePoint.X - 60;
                //win.Top = relativePoint.Y + 95;
                window = win;
                win.Show();
                win.Focus();
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
        private bool _iscancel = false;

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
            if (isCancel && sender != null && id == Guid.Empty)
            {
                gridContainer.Opacity = 1;
                gridContainer.IsEnabled = true;
                return;
            }
            if (!isCancel && sender != null && Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید از این عملیات انصراف دهید؟", "انصراف", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }
            txtCodeMoein.IsReadOnly = false;
            txtCol.IsReadOnly = false;
            txtMoeinName.Text = "";
            Sf_txtMoeinName.HasError = false;
            Sf_txtCodeMoein.HasError = false;
            Sf_txtCol.HasError = false;
            Sf_txtCol.ErrorText = "";
            //txtCodeMoein.Text = (en.MoeinCode + 1).ToString();

            txtCol.Focus();
            datagrid.SelectedIndex = -1;
            datagrid.ClearFilters();
            datagrid.SearchHelper.ClearSearch();
            SearchTermTextBox.Text = "";
            gridDelete.Visibility = Visibility.Hidden;
            borderEdit.Visibility = Visibility.Hidden;
            txtCol.TextChanged -= TxtCol_TextChanged;
            txtColName.Text = txtCol.Text = txtCodeMoein.Text = "";
            txtCol.TextChanged += TxtCol_TextChanged;
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
            if (isCancel&&datagrid.SelectedItem!=null) 
            {
                var moein = datagrid.SelectedItem as Moein;
                id = moein.Id;
                txtCol.TextChanged -= TxtCol_TextChanged;
                txtCol.Text = moein.FkCol.ColCode.ToString();
                txtCol.TextChanged += TxtCol_TextChanged;
                txtColName.Text = moein.FkCol.ColName;
                txtMoeinName.Text = moein.MoeinName;
                txtCodeMoein.Text = moein.MoeinCode.ToString();
                /*checkListBox.SelectedItems.Clear();//GroupDeleted
                using var db = new wpfrazydbContext();
                foreach (MoeinGroup mo in db.MoeinsGroup.Where(t => t.FkMoeinId == id).ToList())
                    checkListBox.SelectedItems.Add((checkListBox.ItemsSource as List<TGroup>).Find(u => u.Id == mo.FkGroupId));*/
                gridDelete.Visibility = Visibility.Visible;
                borderEdit.Visibility = Visibility.Visible;
                txtCodeMoein.IsReadOnly = true;
                txtCol.IsReadOnly = true;
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
            var moein = db.Moeins.Include(m => m.FkCol).FirstOrDefault(m => m.Id == id);

            if (db.AcDocumentDetails.Any(y => y.FkMoeinId == moein.Id))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("قبلا با این معین سند حسابداری زده شده است و قابل حذف نیست!");
                return;
            }
            var listColMoeins = new List<string>();
            foreach (var item in db.CodeSettings.Where(s=>s.Name.Contains("ColCode")).ToList())
            {
                var name = item.Name.Replace("ColCode", "MoeinCode");
                var moeinh = db.CodeSettings.FirstOrDefault(j => j.Name == name);
                if(moeinh!=null)
                {
                    listColMoeins.Add(item.Value + moeinh.Value);
                }
            }
            if (listColMoeins.Contains(moein.FkCol.ColCode.ToString() + moein.MoeinCode.ToString()))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("در تنظیمات پیکربندی از این معین و کل استفاده شده است و قابل حذف نیست!");
                return;
            }
            db.Moeins.Remove(db.Moeins.Find(id));
            if (!db.SafeSaveChanges())  return;
            Moeins.Remove((datagrid.SelectedItem as Moein));            
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

        private void txtMoeinName_TextChanged(object sender, TextChangedEventArgs e)
        {
            isCancel = false;
        }

        private void TxtCodeMoein_TextChanged(object sender, TextChangedEventArgs e)
        {
            isCancel = false;
        }
        public static Window window;
        private void TxtCol_TextChanged(object sender, TextChangedEventArgs e)
        {
            isCancel = false;
        }

        private void TxtCol_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isCancel && (!txtCodeMoein.IsReadOnly || borderEdit.Visibility != Visibility.Visible))
            {
                using var db = new wpfrazydbContext();
                var g = 0;
                try
                {
                    g = int.Parse(txtCol.Text);

                    txtColName.Text = db.Cols.FirstOrDefault(gs => gs.ColCode == g).ColName;
                }
                catch
                {
                    txtCodeMoein.Text = "";
                    txtColName.Text = "";
                    return;
                }
                try
                {
                    txtCodeMoein.Text = (db.Moeins.Include("FkCol").Where(u => u.FkCol.ColCode == g).Max(y => y.MoeinCode) + 1).ToString();
                }
                catch
                {
                    txtCodeMoein.Text = "1";
                }
            }
        }

        private void DataPager_PageIndexChanging(object sender, Syncfusion.UI.Xaml.Controls.DataPager.PageIndexChangingEventArgs e)
        {
            var ex = datagrid.View.FilterPredicates;
            
            using var db = new wpfrazydbContext();
            //db.Moeins.Where(ex)
            var count = db.Moeins.Count();
            var F = db.Moeins.OrderBy(d=>d.Id).Skip(10 * e.NewPageIndex).Take(10).ToList();
            int j = 0;
            for (int i = 10 * e.NewPageIndex; i < 10 * (e.NewPageIndex + 1)&&i<count; i++)
            {
                Moeins[i] = F[j];
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
            var item = list.FirstOrDefault(u => u.Header == "حساب معین");
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
                var g = int.Parse(txtCol.Text);

                var y = db.Cols.FirstOrDefault(gs => gs.ColCode == g);
                if (y != null)
                    Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        await Task.Delay(50);
                        txtMoeinName.Focus();
                    }));
            }
            catch { }
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
    }
}

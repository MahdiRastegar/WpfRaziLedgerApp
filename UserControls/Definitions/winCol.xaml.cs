﻿using Microsoft.EntityFrameworkCore;
using Syncfusion.Data.Extensions;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for winCol.xaml
    /// </summary>
    public partial class winCol : UserControl,ITabForm
    {
        public winCol()
        {
            InitializeComponent();            
            isCancel = true;
        }
        Brush brush = null;

        private void Txt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "\r")
            {
                /*
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                request.Wrapped = true;
                (sender as TextBox).MoveFocus(request);*/
                cmbType.Focus();
                cmbType.IsDropDownOpen = true;
                return;
            }
            /*
            if ((sender as TextBox).Name == "txtVra" || (sender as TextBox).Name == "txtDis")
                e.Handled = !IsTextAllowed(e.Text);*/
        }
        private static readonly Regex _regex = new Regex("[^0-9]"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using var db=new wpfrazydbContext();
            var M = db.Cols.Include(t=>t.FkGroup).AsNoTracking().ToList();            
            datagrid.ItemsSource = M;
            datagrid.SearchHelper.AllowFiltering = true;
            txtGroup.Focus();
            datagrid.SortColumnDescriptions.Clear();
            datagrid.SortColumnDescriptions.Add(new Syncfusion.UI.Xaml.Grid.SortColumnDescription()
            {
                ColumnName = "ColCode",
                SortDirection = System.ComponentModel.ListSortDirection.Ascending
            });
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            bool haserror = GetError();

            if (haserror)
                return;
            using var db = new wpfrazydbContext();
            var g = int.Parse(txtGroup.Text);
            var group = db.Agroups.FirstOrDefault(h => h.GroupCode == g);
            if (group == null)
            {
                Sf_txtGreoup.ErrorText = "این کد گروه وجود ندارد";
                Sf_txtGreoup.HasError = true;
                return;
            }


            var i = int.Parse(txtCol.Text);
            var col = db.Cols.FirstOrDefault(h => h.ColCode == i);
            if (col == null)
            {
                datagrid.SortColumnDescriptions.Clear();
                db.Cols.Add(new Col()
                {
                    Id = Guid.NewGuid(),
                    ColCode = i,
                    ColName = txtColName.Text,
                    Type = (byte)cmbType.SelectedIndex,
                    Action = (byte)cmbAction.SelectedIndex,
                    FkGroupId = group.Id
                });
            }
            else
            {
                col.Type = (byte)cmbType.SelectedIndex;
                col.ColName = txtColName.Text;
                col.Action = (byte)cmbAction.SelectedIndex;
            }
            if (!db.SafeSaveChanges())  return;
            var M = db.Cols.Include(w=>w.FkGroup).ToList();
            datagrid.ItemsSource = M;
            if (col == null)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("اطلاعات اضافه شد.", "ثبت کل");                
                isCancel = false;
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("اطلاعات ویرایش شد.", "ویرایش کل");
                btnCancel_Click(null, null);
                isCancel = true;
            }

            txtColName.Text = "";
            if (cmbAction.SelectedIndex == 0)
                cmbAction.SelectedIndex = 1;
            else
                cmbAction.SelectedIndex = 0;

            if (cmbType.SelectedIndex < 2)
                cmbType.SelectedIndex = 2;
            else
                cmbType.SelectedIndex = 0;
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                //await Task.Delay(2000);
                cmbAction.SelectedItem = null;
                cmbType.SelectedItem = null;
            }));
            datagrid.SelectedIndex = -1;
            datagrid.ClearFilters();
            gridDelete.Visibility = Visibility.Hidden;
            borderEdit.Visibility = Visibility.Hidden;
            if (col != null)
                txtGroup.Focus();
            else
            {
                txtCol.Text = (i + 1).ToString();
                txtColName.Focus();
            }
        }

        private bool GetError()
        {
            bool haserror = false;
            if (txtColName.Text.Trim() == "")
            {
                Sf_txtVra.HasError = true;
                haserror = true;
            }
            else
                Sf_txtVra.HasError = false;
            if (cmbType.SelectedIndex == -1)
            {
                Sf_txtName.HasError = true;
                haserror = true;
            }
            else
                Sf_txtName.HasError = false;
            if (cmbAction.SelectedIndex == -1)
            {
                Sf_Actin.HasError = true;
                haserror = true;
            }
            else
                Sf_Actin.HasError = false;
            if (txtGroup.Text.Trim() == "")
            {
                Sf_txtGreoup.HasError = true;
                haserror = true;
            }
            else
            {
                Sf_txtGreoup.HasError = false;
                Sf_txtGreoup.ErrorText = "";
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
        public static System.Windows.Window window;
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.Key == Key.F1 && txtGroup.IsFocused && !txtGroup.IsReadOnly)
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
                var list = db.Agroups.ToList().Select(r => new Mu() { Name = r.GroupName, Value = r.GroupCode.ToString() }).ToList();
                var win = new winSearch(list);
                win.Tag = this;
                win.ParentTextBox = txtGroup;
                win.SearchTermTextBox.Text = txtGroup.Text;
                win.SearchTermTextBox.Select(1, 0);
                win.Owner = MainWindow.Current;
                //win.Left = relativePoint.X - 60;
                //win.Top = relativePoint.Y + 95;
                window = win;
                win.Show();win.Focus();
            }
        }

        private void cmbType_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                cmbAction.IsDropDownOpen = true;
                cmbAction.Focus();
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
            if (isCancel && sender != null && datagrid.SelectedIndex==-1)
            {
                gridContainer.Opacity = 1;
                gridContainer.IsEnabled = true;
                txtGroup.Focus();
                return;
            }
            if (!isCancel && sender != null && Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید از این عملیات انصراف دهید؟", "انصراف", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }
            txtCol.Text = txtGroupName.Text = txtGroup.Text = txtColName.Text = txtColName.Text = "";
            Sf_Actin.HasError = false;
            Sf_txtVra.HasError = false;
            Sf_txtName.HasError = false;
            txtGroup.IsReadOnly = false;
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                await Task.Delay(50);                
                isCancel = true;
            }));
            if (sender != null)
            {
                if (datagrid.SelectedIndex == -1)
                {
                    gridContainer.Opacity = 1;
                    gridContainer.IsEnabled = true;
                }
                txtGroup.Focus();
            }

            datagrid.SelectedIndex = -1;
            datagrid.ClearFilters();
            gridDelete.Visibility = Visibility.Hidden;
            borderEdit.Visibility = Visibility.Hidden;
        }

        private void cmbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isCancel = false;
        }

        private void txtColName_TextChanged(object sender, TextChangedEventArgs e)
        {
            isCancel = false;
        }

        private void datagrid_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            if (isCancel&&datagrid.SelectedItem!=null) 
            {
                var col = datagrid.SelectedItem as Col;
                txtGroup.Text = col.FkGroup.GroupCode.ToString();
                txtGroupName.Text = col.FkGroup.GroupName;
                cmbAction.SelectedIndex = (int)col.Action;
                txtCol.Text = col.ColCode.ToString();
                txtGroup.IsReadOnly = true;
                txtColName.Text = col.ColName;
                cmbType.SelectedIndex = (int)col.Type;
                gridDelete.Visibility = Visibility.Visible;
                isCancel = true;
                borderEdit.Visibility = Visibility.Visible;
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
            db.Cols.Remove(db.Cols.Find((datagrid.SelectedItem as Col).Id));
            if (!db.SafeSaveChanges())  return;
            (datagrid.ItemsSource as List<Col>).Remove((datagrid.SelectedItem as Col));
            var u = datagrid.ItemsSource;
            datagrid.ItemsSource = null;
            datagrid.ItemsSource = u;
            btnCancel_Click(null, null);
            txtGroup.Focus();

            if (cmbAction.SelectedIndex == 0)
                cmbAction.SelectedIndex = 1;
            else
                cmbAction.SelectedIndex = 0;

            if (cmbType.SelectedIndex < 2)
                cmbType.SelectedIndex = 2;
            else
                cmbType.SelectedIndex = 0;
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                //await Task.Delay(2000);
                cmbAction.SelectedItem = null;
                cmbType.SelectedItem = null;
            }));
        }

        private void SearchTermTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchTermTextBox.Text.Trim() == "")
                datagrid.SearchHelper.ClearSearch();
            else
                datagrid.SearchHelper.Search(SearchTermTextBox.Text);
        }

        public bool CloseForm()
        {
            if (!isCancel && Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید از این فرم خارج شوید؟", "خروج", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return false;
            }
            forceClose = true;
            var list = MainWindow.Current.GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "حساب کل");
            MainWindow.Current.tabcontrol.Items.Remove(item);
            return true;
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            datagrid.AllowFiltering = !datagrid.AllowFiltering;
            if (!datagrid.AllowFiltering)
                datagrid.ClearFilters();
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {

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

        public void SetNull()
        {
            window = null;

            try
            {
                using var db = new wpfrazydbContext();
                var g = int.Parse(txtGroup.Text);

                var y = db.Agroups.FirstOrDefault(gs => gs.GroupCode == g);
                if (y != null)
                    Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        await Task.Delay(50);
                        txtColName.Focus();
                    }));
            }
            catch { }
        }

        private void cmAction_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnConfirm.Focus();
                return;
            }
        }

        private void txtGroup_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isCancel && (!txtCol.IsReadOnly||borderEdit.Visibility!= Visibility.Visible))
                try
                {
                    using var db = new wpfrazydbContext();
                    var g = int.Parse(txtGroup.Text);
                    var group = db.Agroups.FirstOrDefault(gs => gs.GroupCode == g);
                    txtGroupName.Text = group.GroupName;
                    try
                    {
                        txtCol.Text = (db.Cols.Where(u => u.FkGroup.GroupCode == g).Max(y => y.ColCode) + 1).ToString();
                    }
                    catch
                    {
                        txtCol.Text = ((group.GroupCode * 10) + 1).ToString();
                    }
                }
                catch
                {
                    txtGroupName.Text = "";
                    //txtCodePreferential.Text = "1";
                }
        }

        private void txtGroup_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "\r")
            {
                /*
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                request.Wrapped = true;
                (sender as TextBox).MoveFocus(request);*/
                txtColName.Focus();
                return;
            }
        }
    }
}

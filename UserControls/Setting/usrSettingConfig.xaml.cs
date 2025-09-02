using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.TextInputLayout;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;
using WpfRaziLedgerApp.Interfaces;
using WpfRaziLedgerApp.Windows.toolWindows;
using Syncfusion.Data.Extensions;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for usrSettingConfig.xaml
    /// </summary>
    public partial class usrSettingConfig : UserControl,ITabForm
    {
        public usrSettingConfig()
        {
            InitializeComponent();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
        }
        public bool CloseForm()
        {
            //if (Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید از این فرم خارج شوید؟", "خروج", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            //{
            //    return false;
            //}
            var list = MainWindow.Current.GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "چارچوب سیستم");
            MainWindow.Current.tabcontrol.Items.Remove(item);            
            return true;
        }

        public void SetNull()
        {

        }

        List<Mu> mus1 = new List<Mu>();
        List<Mu> mus2 = new List<Mu>();
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            using var db = new wpfrazydbContext();
            mus1.Clear();
            mus2.Clear();
            var moeins = db.Moeins.Include(t => t.FkCol).ToList();
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
                    Id = item.Id,
                    Value = $"{item.FkCol.ColCode}",
                    Name = $"{item.FkCol.ColName}",
                    AdditionalEntity = accountSearchClass
                });
            }
            foreach (var item in preferentials)
            {
                mus2.Add(new Mu()
                {
                    Id = item.Id,
                    Name = $"{item.PreferentialName}",
                    Value = $"{item.PreferentialCode}",
                    Name2 = item.FkGroup.GroupName
                });
            }
            if (db.CodeSettings.Any(t => t.Name == "MoeinCodeCheckRecieve"))
            {
                imgReceive.Visibility = Visibility.Visible;
            }
            if (db.CodeSettings.Any(t => t.Name == "MoeinCodeCheckPayment"))
            {
                imgPayment.Visibility = Visibility.Visible;
            }
            if (db.CodeSettings.Any(t => t.Name == "TaxPercent"))
            {
                imgTaxPercent.Visibility = Visibility.Visible;
            }
        }
        private GroupBox SettingDefinitionGroupBox(winSettingCode win, wpfrazydbContext db, bool exist, string name, string str1, string str2, string str3)
        {
            var groupBox = new GroupBox() { Header = name };
            var stackPanel = new DockPanel();
            groupBox.Content = stackPanel;

            var keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add(str1, exist ? db.CodeSettings.First(i => i.Name == str1).Value : "");
            keyValuePairs.Add(str2, exist ? db.CodeSettings.First(i => i.Name == str2).Value : "");

            var textInputLayout = new SfTextInputLayout() { Tag = keyValuePairs, Hint = "کد کل و معین ", Width = 175 };
            var textBox = new TextBox() { Text = exist ? keyValuePairs.ElementAt(0).Value + keyValuePairs.ElementAt(1).Value : "", Tag = true };
            textInputLayout.InputView = textBox;
            if (exist)
            {
                var mu = mus1.Find(t => (t.AdditionalEntity as AccountSearchClass).ColMoein == textBox.Text);
                textInputLayout.HelperText = (mu.AdditionalEntity as AccountSearchClass).MoeinName;
            }
            textBox.PreviewKeyDown += (s1, e1) =>
            {
                if (e1.Key == Key.F1)
                {
                    win.childWindow = ShowSearchMoein(s1, win);
                }
                else if (e1.Key == Key.Enter)
                {
                    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                    request.Wrapped = true;
                    (s1 as TextBox).MoveFocus(request);
                }
            };
            textBox.LostFocus += (s1, e1) =>
            {
                var txt = s1 as TextBox;
                var sfTextInput = txt.GetParentOfType<SfTextInputLayout>();
                if (txt.Text == "")
                {
                    sfTextInput.HelperText = string.Empty;
                    return;
                }
                var mu = mus1.Find(t => (t.AdditionalEntity as AccountSearchClass).ColMoein == txt.Text);
                if (mu == null)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("چنین کل و معینی وجود ندارد!");
                    sfTextInput.HelperText = txt.Text = string.Empty;
                }
                else
                {
                    txt.Tag = mu;
                    sfTextInput.HelperText = (mu.AdditionalEntity as AccountSearchClass).MoeinName;
                    keyValuePairs = sfTextInput.Tag as Dictionary<string, string>;
                    keyValuePairs[keyValuePairs.ElementAt(0).Key] = mu.Value;
                    keyValuePairs[keyValuePairs.ElementAt(1).Key] = (mu.AdditionalEntity as AccountSearchClass).Moein;
                }
            };
            stackPanel.Children.Add(textInputLayout);

            textInputLayout = new SfTextInputLayout() { Tag = str3, Hint = "کد تفضیل", Margin = new Thickness(10, 0, 10, 0) };
            textBox = new TextBox() { Text = exist ? db.CodeSettings.First(i => i.Name == str3).Value : "", Tag = true };
            textInputLayout.InputView = textBox;
            if (exist)
            {
                var mu = mus2.Find(t => t.Value == textBox.Text);
                textInputLayout.HelperText = mu.Name;
            }
            textBox.PreviewKeyDown += (s1, e1) =>
            {
                if (e1.Key == Key.F1)
                {
                    win.childWindow = ShowSearchPreferential(s1, win);
                }
                else if (e1.Key == Key.Enter)
                {
                    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                    request.Wrapped = true;
                    (s1 as TextBox).MoveFocus(request);
                }
            };
            textBox.LostFocus += (s1, e1) =>
            {
                var txt = s1 as TextBox;
                var sfTextInput = txt.GetParentOfType<SfTextInputLayout>();
                if (txt.Text == "")
                {
                    sfTextInput.HelperText = string.Empty;
                    return;
                }
                var mu = mus2.Find(t => t.Value == txt.Text);
                if (mu == null)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("چنین تفضیلی وجود ندارد!");
                    sfTextInput.HelperText = txt.Text = string.Empty;
                }
                else
                {
                    txt.Tag = mu;
                    sfTextInput.HelperText = mu.Name;
                }
            };
            stackPanel.Children.Add(textInputLayout);
            return groupBox;
        }
        private GroupBox SettingDefinitionGroupBoxPayment(winSettingCode win, wpfrazydbContext db, bool exist, string name, string str1, string str2, string str3)
        {
            var groupBox = new GroupBox() { Header = name };
            var stackPanel = new DockPanel();
            groupBox.Content = stackPanel;

            var keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add(str1, exist ? db.CodeSettings.First(i => i.Name == str1).Value : "");
            keyValuePairs.Add(str2, exist ? db.CodeSettings.First(i => i.Name == str2).Value : "");

            var textInputLayout = new SfTextInputLayout()
            {
                Tag = keyValuePairs,
                Hint = (str1 == "ColCodeCheckPayment" ? "کد کل و معین اسناد پرداختنی "
                : "کد کل و معین "),
                Width = 175
            };
            if (str1 == "ColCodeCheckPayment")
                textInputLayout.Width = 190;
            var textBox = new TextBox() { Text = exist ? keyValuePairs.ElementAt(0).Value + keyValuePairs.ElementAt(1).Value : "", Tag = true };
            textInputLayout.InputView = textBox;
            if (exist)
            {
                var mu = mus1.Find(t => (t.AdditionalEntity as AccountSearchClass).ColMoein == textBox.Text);
                textInputLayout.HelperText = (mu.AdditionalEntity as AccountSearchClass).MoeinName;
            }
            textBox.PreviewKeyDown += (s1, e1) =>
            {
                if (e1.Key == Key.F1)
                {
                    win.childWindow = ShowSearchMoein(s1, win);
                }
                else if (e1.Key == Key.Enter)
                {
                    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                    request.Wrapped = true;
                    (s1 as TextBox).MoveFocus(request);
                }
            };
            textBox.LostFocus += (s1, e1) =>
            {
                var txt = s1 as TextBox;
                var sfTextInput = txt.GetParentOfType<SfTextInputLayout>();
                if (txt.Text == "")
                {
                    sfTextInput.HelperText = string.Empty;
                    return;
                }
                var mu = mus1.Find(t => (t.AdditionalEntity as AccountSearchClass).ColMoein == txt.Text);
                if (mu == null)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("چنین کل و معینی وجود ندارد!");
                    sfTextInput.HelperText = txt.Text = string.Empty;
                }
                else
                {
                    txt.Tag = mu;
                    sfTextInput.HelperText = (mu.AdditionalEntity as AccountSearchClass).MoeinName;
                    keyValuePairs = sfTextInput.Tag as Dictionary<string, string>;
                    keyValuePairs[keyValuePairs.ElementAt(0).Key] = mu.Value;
                    keyValuePairs[keyValuePairs.ElementAt(1).Key] = (mu.AdditionalEntity as AccountSearchClass).Moein;
                }
            };
            stackPanel.Children.Add(textInputLayout);
            if (str3 != null)
            {
                textInputLayout = new SfTextInputLayout() { Tag = str3, Hint = "کد تفضیل", Margin = new Thickness(10, 0, 10, 0) };
                textBox = new TextBox() { Text = exist ? db.CodeSettings.First(i => i.Name == str3).Value : "", Tag = true };
                textInputLayout.InputView = textBox;
                if (exist)
                {
                    var mu = mus2.Find(t => t.Value == textBox.Text);
                    textInputLayout.HelperText = mu.Name;
                }
                textBox.PreviewKeyDown += (s1, e1) =>
                {
                    if (e1.Key == Key.F1)
                    {
                        win.childWindow = ShowSearchPreferential(s1, win);
                    }
                    else if (e1.Key == Key.Enter)
                    {
                        TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                        request.Wrapped = true;
                        (s1 as TextBox).MoveFocus(request);
                    }
                };
                textBox.LostFocus += (s1, e1) =>
                {
                    var txt = s1 as TextBox;
                    var sfTextInput = txt.GetParentOfType<SfTextInputLayout>();
                    if (txt.Text == "")
                    {
                        sfTextInput.HelperText = string.Empty;
                        return;
                    }
                    var mu = mus2.Find(t => t.Value == txt.Text);
                    if (mu == null)
                    {
                        Xceed.Wpf.Toolkit.MessageBox.Show("چنین تفضیلی وجود ندارد!");
                        sfTextInput.HelperText = txt.Text = string.Empty;
                    }
                    else
                    {
                        txt.Tag = mu;
                        sfTextInput.HelperText = mu.Name;
                    }
                };
                stackPanel.Children.Add(textInputLayout);
            }
            else
            {
                stackPanel.HorizontalAlignment = HorizontalAlignment.Left;
            }
            return groupBox;
        }
        private void btnReceive_Click(object sender, RoutedEventArgs e)
        {
            var win = new winSettingCode() { Width = 460 };
            win.grid.Width = 435;
            using var db = new wpfrazydbContext();
            var exist = false;
            if (imgReceive.Visibility == Visibility.Visible)
            {
                exist = true;
            }
            GroupBox groupBox = SettingDefinitionGroupBox(win, db, exist, "نوع وجه چک", "ColCodeCheckRecieve", "MoeinCodeCheckRecieve", "PreferentialCodeCheckRecieve");
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                groupBox.GetChildOfType<TextBox>().Focus();
            }), DispatcherPriority.Render);
            win.stack.Children.Add(groupBox);
            var groupBox2 = SettingDefinitionGroupBox(win, db, exist, "نوع وجه نقد", "ColCodeMoneyRecieve", "MoeinCodeMoneyRecieve", "PreferentialCodeMoneyRecieve");
            win.stack.Children.Add(groupBox2);

            groupBox2 = SettingDefinitionGroupBox(win, db, exist, "نوع وجه تخفیف", "ColCodeDiscountRecieve", "MoeinCodeDiscountRecieve", "PreferentialCodeDiscountRecieve");
            win.stack.Children.Add(groupBox2);

            var keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("ColCodeTransferLCheckRecieve", exist ? db.CodeSettings.First(i => i.Name == "ColCodeTransferLCheckRecieve").Value : "");
            keyValuePairs.Add("MoeinCodeTransferLCheckRecieve", exist ? db.CodeSettings.First(i => i.Name == "MoeinCodeTransferLCheckRecieve").Value : "");

            var textInputLayout = new SfTextInputLayout() { Tag = keyValuePairs, Hint = "کد کل و معین چکهای واگذار شده به بانک ", Margin = new Thickness(0, 30, 0, 0) };
            var textBox = new TextBox() { Text = exist ? keyValuePairs.ElementAt(0).Value + keyValuePairs.ElementAt(1).Value : "", Tag = true };
            textBox.Loaded += (sf, ef) =>
            {
                (sf as TextBox).Focus();
            };
            textInputLayout.InputView = textBox;
            if (exist)
            {
                var mu = mus1.Find(t => (t.AdditionalEntity as AccountSearchClass).ColMoein == textBox.Text);
                textInputLayout.HelperText = (mu.AdditionalEntity as AccountSearchClass).MoeinName;
            }
            textBox.PreviewKeyDown += (s1, e1) =>
            {
                if (e1.Key == Key.F1)
                {
                    win.childWindow = ShowSearchMoein(s1, win);
                }
                else if (e1.Key == Key.Enter)
                {
                    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                    request.Wrapped = true;
                    (s1 as TextBox).MoveFocus(request);
                }
            };
            textBox.LostFocus += (s1, e1) =>
            {
                var txt = s1 as TextBox;
                var sfTextInput = txt.GetParentOfType<SfTextInputLayout>();
                if (txt.Text == "")
                {
                    sfTextInput.HelperText = string.Empty;
                    return;
                }
                var mu = mus1.Find(t => (t.AdditionalEntity as AccountSearchClass).ColMoein == txt.Text);
                if (mu == null)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("چنین کل و معینی وجود ندارد!");
                    sfTextInput.HelperText = txt.Text = string.Empty;
                }
                else
                {
                    txt.Tag = mu;
                    sfTextInput.HelperText = (mu.AdditionalEntity as AccountSearchClass).MoeinName;
                    keyValuePairs = sfTextInput.Tag as Dictionary<string, string>;
                    keyValuePairs[keyValuePairs.ElementAt(0).Key] = mu.Value;
                    keyValuePairs[keyValuePairs.ElementAt(1).Key] = (mu.AdditionalEntity as AccountSearchClass).Moein;
                }
            };
            win.stack.Children.Add(textInputLayout);

            keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("ColCodeDoneLCheckRecieve", exist ? db.CodeSettings.First(i => i.Name == "ColCodeDoneLCheckRecieve").Value : "");
            keyValuePairs.Add("MoeinCodeDoneLCheckRecieve", exist ? db.CodeSettings.First(i => i.Name == "MoeinCodeDoneLCheckRecieve").Value : "");
            textInputLayout = new SfTextInputLayout() { Tag = keyValuePairs, Hint = "کد کل و معین حساب های بانکی " };
            textBox = new TextBox() { Text = exist ? keyValuePairs.ElementAt(0).Value + keyValuePairs.ElementAt(1).Value : "", Tag = true };
            textInputLayout.InputView = textBox;
            if (exist)
            {
                var mu = mus1.Find(t => (t.AdditionalEntity as AccountSearchClass).ColMoein == textBox.Text);
                textInputLayout.HelperText = (mu.AdditionalEntity as AccountSearchClass).MoeinName;
            }
            textBox.PreviewKeyDown += (s1, e1) =>
            {
                if (e1.Key == Key.F1)
                {
                    win.childWindow = ShowSearchMoein(s1, win);
                }
                else if (e1.Key == Key.Enter)
                {
                    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                    request.Wrapped = true;
                    (s1 as TextBox).MoveFocus(request);
                }
            };
            textBox.LostFocus += (s1, e1) =>
            {
                var txt = s1 as TextBox;
                var sfTextInput = txt.GetParentOfType<SfTextInputLayout>();
                if (txt.Text == "")
                {
                    sfTextInput.HelperText = string.Empty;
                    return;
                }
                var mu = mus1.Find(t => (t.AdditionalEntity as AccountSearchClass).ColMoein == txt.Text);
                if (mu == null)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("چنین کل و معینی وجود ندارد!");
                    sfTextInput.HelperText = txt.Text = string.Empty;
                }
                else
                {
                    txt.Tag = mu;
                    sfTextInput.HelperText = (mu.AdditionalEntity as AccountSearchClass).MoeinName;
                    keyValuePairs = sfTextInput.Tag as Dictionary<string, string>;
                    keyValuePairs[keyValuePairs.ElementAt(0).Key] = mu.Value;
                    keyValuePairs[keyValuePairs.ElementAt(1).Key] = (mu.AdditionalEntity as AccountSearchClass).Moein;
                }
            };
            win.stack.Children.Add(textInputLayout);

            var result = win.ShowDialog();
            if (result == true)
                imgReceive.Visibility = Visibility.Visible;
        }
        public static Window window;
        private winSearch ShowSearchMoein(dynamic y, Window owner = null)
        {
            var win = new winSearch(mus1);
            win.Closed += (yf, rs) =>
            {
            };
            win.Width = 640;
            win.datagrid.Columns[0].HeaderText = "نام";
            win.datagrid.Columns[1].HeaderText = "کل";
            win.datagrid.Columns[0].Width = 255;
            win.datagrid.Columns[1].Width = 100;
            win.datagrid.Columns.MoveTo(0, 1);
            win.datagrid.Columns.Add(new GridTextColumn() { TextAlignment = TextAlignment.Center, HeaderText = "معین", MappingName = "AdditionalEntity.Moein", Width = 100, AllowSorting = true });
            win.datagrid.Columns.Add(new GridTextColumn() { TextAlignment = TextAlignment.Center, HeaderText = "نام", MappingName = "AdditionalEntity.MoeinName", AllowSorting = true, ColumnSizer = GridLengthUnitType.AutoWithLastColumnFill });
            win.datagrid.AllowResizingColumns = true;
            if (owner == null)
                win.Tag = this;
            else
                win.Tag = owner;
            if (owner == null)
                owner = MainWindow.Current;
            win.ParentTextBox = y;
            win.SearchTermTextBox.Text = "";
            win.SearchTermTextBox.Select(1, 0);
            win.Owner = MainWindow.Current;
            window = win;
            win.Show();
            win.Focus();
            return win;
        }
        private winSearch ShowSearchPreferential(dynamic y, Window owner = null)
        {
            var win = new winSearch(mus2);
            win.Closed += (yf, rs) =>
            {
            };
            win.datagrid.Columns.Add(new GridTextColumn() { TextAlignment = TextAlignment.Center, HeaderText = "گروه", MappingName = "Name2", Width = 150, AllowSorting = true });
            win.Width = 640;
            if (owner == null)
                win.Tag = this;
            else
                win.Tag = owner;
            if (owner == null)
                owner = MainWindow.Current;
            win.ParentTextBox = y;
            win.SearchTermTextBox.Text = "";
            win.SearchTermTextBox.Select(1, 0);
            win.Owner = MainWindow.Current;
            window = win;
            win.Show();
            win.Focus();
            return win;
        }

        private void btnPayment_Click(object sender, RoutedEventArgs e)
        {
            var win = new winSettingCode() { Width = 460 };
            win.grid.Width = 435;
            using var db = new wpfrazydbContext();
            var exist = false;
            if (imgPayment.Visibility == Visibility.Visible)
            {
                exist = true;
            }
            GroupBox groupBox = SettingDefinitionGroupBoxPayment(win, db, exist, "نوع وجه چک", "ColCodeCheckPayment", "MoeinCodeCheckPayment", null);
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                groupBox.GetChildOfType<TextBox>().Focus();
            }), DispatcherPriority.Render);
            win.stack.Children.Add(groupBox);
            var groupBox2 = SettingDefinitionGroupBoxPayment(win, db, exist, "نوع وجه نقد", "ColCodeMoneyPayment", "MoeinCodeMoneyPayment", "PreferentialCodeMoneyPayment");
            win.stack.Children.Add(groupBox2);

            groupBox2 = SettingDefinitionGroupBoxPayment(win, db, exist, "نوع وجه تخفیف", "ColCodeDiscountPayment", "MoeinCodeDiscountPayment", "PreferentialCodeDiscountPayment");
            win.stack.Children.Add(groupBox2);

            var result = win.ShowDialog();
            if (result == true)
                imgPayment.Visibility = Visibility.Visible;
        }

        private void btnTaxPercent_Click(object sender, RoutedEventArgs e)
        {
             var win = new winSettingCode() { Width = 460 };
            win.grid.Width = 435;
            using var db = new wpfrazydbContext();
            //var exist = false;
            //if (db.CodeSettings.Any(t => t.Name == "MoeinCodeTransferLCheckPayment"))
            //{
            //    exist = true;
            //}
            var exist = false;
            if (imgTaxPercent.Visibility == Visibility.Visible)
            {
                exist = true;
            }
            GroupBox groupBox = SettingDefinitionGroupBoxTax(win, db, exist, "درصد مالیات", "TaxPercent");
            win.stack.Children.Add(groupBox);

            var result = win.ShowDialog();
            if (result == true)
                imgTaxPercent.Visibility = Visibility.Visible;
        }
        private GroupBox SettingDefinitionGroupBoxTax(winSettingCode win, wpfrazydbContext db, bool exist, string name, string str1)
        {
            var groupBox = new GroupBox() { Header = name };
            var stackPanel = new DockPanel();
            groupBox.Content = stackPanel;

            var keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add(str1, exist ? db.CodeSettings.First(i => i.Name == str1).Value : "");

            var textInputLayout = new SfTextInputLayout()
            {
                Tag = keyValuePairs,
                Hint = name,
                Width = 175
            };

            var textBox = new TextBox() { Text = exist ? keyValuePairs.ElementAt(0).Value : "", Tag = true };
            textInputLayout.InputView = textBox;

            textBox.LostFocus += (s1, e1) =>
            {
                var txt = s1 as TextBox;
                var sfTextInput = txt.GetParentOfType<SfTextInputLayout>();
                if (txt.Text == "")
                {
                    return;
                }
                int x = -1;
                var b = int.TryParse(txt.Text, out x);
                if (!b)
                {
                    txt.Text = "";
                    return;
                }

                if (x == -1)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("عددی وارد نشده!");
                    txt.Text = "";
                }
                else if (x < 0 || x > 100)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("عدد وارد شده اشتباهست!");
                    txt.Text = "";
                }
            };
            textBox.PreviewKeyDown += (s1, e1) =>
            {
                if (e1.Key == Key.Enter)
                {
                    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                    request.Wrapped = true;
                    (s1 as TextBox).MoveFocus(request);
                }
            };
            stackPanel.Children.Add(textInputLayout);

            return groupBox;
        }

        private void btnOther_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

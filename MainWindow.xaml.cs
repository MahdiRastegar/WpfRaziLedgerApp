using Microsoft.EntityFrameworkCore;
using Syncfusion.CompoundFile.XlsIO.Native;
using Syncfusion.Linq;
using Syncfusion.UI.Xaml.TextInputLayout;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfRaziLedgerApp.Interfaces;
using XamlAnimatedGif;

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Current;
        private int _TaxPercent=-1;
        private DispatcherTimer timer;

        public int TaxPercent
        {
            get { return _TaxPercent; }
            set { _TaxPercent = value; }
        }
        public static StatusOptions StatusOptions { get; set; }
        public static bool ViewFormLeftRigth = true;
        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            SetNextTick();
            Hide();            
            ribbon.RibbonState = Syncfusion.Windows.Tools.RibbonState.Hide;
            Current = this;
            try
            {
                using var db = new wpfrazydbContext();
                if (db.CodeSettings.FirstOrDefault(i => i.Name == "TaxPercent") is CodeSetting codeSetting)
                    TaxPercent = int.Parse(codeSetting.Value);
            }
            catch (Exception ex) 
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, "خطای اتصال به دیتابیس", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            //var gifImage = new BitmapImage(new Uri("pack://application:,,,/Images/AddDataLarge.gif"));
            //XamlAnimatedGif.AnimationBehavior.SetSourceUri(this.gifImage, gifImage.UriSource);
            ClockText.Text = DateTime.Now.ToString("HH:mm");
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            ClockText.Text = DateTime.Now.ToString("HH:mm");
            txbDate.Text = DateTime.Now.ToPersianDateString();
            SetNextTick(); // بعد از آپدیت، زمان تیک بعدی را مجدد تنظیم می‌کنیم
        }

        private void SetNextTick()
        {
            var now = DateTime.Now;
            var nextMinute = now.AddMinutes(1).AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
            timer.Interval = nextMinute - now; // فاصله تا دقیقه بعد
            timer.Start();
        }
        public void LoadUser(Guid userGroupId)
        {
            ApplyPermissions(userGroupId);
        }
        public Dictionary<RibbonButton, Guid> keyValuePairs = new Dictionary<RibbonButton, Guid>();
        private void ApplyPermissions(Guid userGroupId)
        {
            // 1. دریافت دسترسی‌ها از دیتابیس
            var allowedIds = GetPermissionIdsForGroup(userGroupId); // List<Guid>
            if(allowedIds.Count == 0) return;

            int g = 0;
            // 2. مرور کل آیتم‌های Ribbon
            foreach (var tab in ribbon.Items.OfType<RibbonTab>())
            {
                bool hasVisibleChild = false;

                foreach (var bar in tab.Items.OfType<RibbonBar>())
                {
                    UIElement? item2=null;
                    foreach (var item in bar.Items.OfType<UIElement>())
                    {
                        if (item is RibbonButton btn && btn.Label != null)
                        {
                            var ribbonId = GetRibbonItemIdByName(btn.Label,tab.Caption); // گرفتن Id از نام دکمه
                            bool canAccess = allowedIds.Contains(ribbonId);

                            btn.Visibility = canAccess ? Visibility.Visible : Visibility.Collapsed;
                            if(btn.Visibility == Visibility.Visible) 
                            {                                
                                if (btn.Label != "تفضیلی" || g == 0)
                                    keyValuePairs.Add(btn, ribbonId);
                                if (btn.Label == "تفضیلی")
                                    g++;
                            }
                            if (btn.Visibility == Visibility.Collapsed && item2 is RibbonSeparator separator)
                                separator.Visibility = Visibility.Collapsed;

                            if (canAccess)
                                hasVisibleChild = true;
                        }
                        item2 = item;
                    }

                    // اگر هیچ دکمه‌ای در این Bar قابل دسترسی نبود، خودش رو پنهان کن
                    bar.Visibility = bar.Items.OfType<RibbonButton>().Any(b => b.Visibility == Visibility.Visible)
                        ? Visibility.Visible : Visibility.Collapsed;
                    if(bar.Visibility== Visibility.Visible) 
                    {
                        var visibleButtons = bar.Items.OfType<RibbonButton>().Where(btn => btn.Visibility == Visibility.Visible).ToList();

                        // تغییر اندازه bar بر اساس تعداد دکمه‌های قابل مشاهده
                        bar.Width -= (bar.Items.OfType<RibbonButton>().Count() - visibleButtons.Count()) * 15;
                    }
                }

                // اگر هیچ آیتمی در تب قابل نمایش نیست، تب هم پنهان شه
                tab.Visibility = hasVisibleChild ? Visibility.Visible : Visibility.Collapsed;
            }
            if (rbnDoshboardSetting.Visibility == Visibility.Visible)
                rbnPreview.Visibility = Visibility.Visible;
            //rbnDoshboard.Visibility = 
            if (rbnPreview.Visibility != Visibility.Visible)
                borderMiz.Visibility = Visibility.Collapsed;            
            //rbnConfiguration2.Visibility = Visibility.Visible;
        }
        private List<Guid> GetPermissionIdsForGroup(Guid groupId)
        {
            using (var context = new wpfrazydbContext())
            {
                return context.Permissions
                              .Where(p => p.FkUserGroupId == groupId && p.CanAccess)
                              .Select(p => p.FkRibbonItemId)
                              .ToList();
            }
        }

        private Guid GetRibbonItemIdByName(string buttonName,string category)
        {
            using (var context = new wpfrazydbContext())
            {
                var item = context.RibbonItemMains.FirstOrDefault(r => r.DisplayName == buttonName && r.Category.Contains(category));
                return item?.Id ?? Guid.Empty;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //var M = new winCol();
            //M.ShowDialog();
        }

        private void BtnMoein_Click(object sender, RoutedEventArgs e)
        {
            //var M = new winMoein();
            //M.ShowDialog();
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            var u = "3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679821480865132823066470938446095505822317253594081284811174502841027019385211055596446229489549303819644288109756659334461284756482337867831652712019091456485669234603486104543266482133936072602491412737245870066063155881748815209209628292540917153643678925903600113305305488204665213841469519415116094330572703657595919530921861173819326117931051185480744623799627495673518857527248912279381830119491";
            var y = u.Substring(0, 42);
            var a = "3.1415926535897932384626433832795028841971693993751058209749445923078164062862089469509070248424167755216229746776236362716129014677044253241618530641369520079400145492904783927346278766000080780449455";
            var b = a.Substring(0, 42);

        }
        public IEnumerable<TabItemExt> GetTabControlItems
        {
            get
            {
                return tabcontrol.Items.ToList<TabItemExt>();
            }
        }

        private void rbnCol_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "حساب کل");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("حساب کل", new winCol(),sender , "Definitions/col.png");
        }

        private void rbnMoein_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "حساب معین");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("حساب معین", new winMoein(),sender , "Definitions/moeinPng.png");
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!LoadedWin)
                return;
            double newWidth = e.NewSize.Width;
            double newHeight = e.NewSize.Height;
            ribbon.Margin = new Thickness(ribbon.Margin.Left + (newWidth - e.PreviousSize.Width) / 4.33, 0, 0, 0);
            /*var t = ((ribbon.RenderTransform as TransformGroup).Children[3] as TranslateTransform);
            t.X += (newWidth - e.PreviousSize.Width)/3.3333333;*/
        }
        bool LoadedWin = false;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadedWin = true;
            WindowState = WindowState.Maximized;
            if (rbnPreview.Visibility == Visibility.Visible)
            {
                if (StatusOptions.User.ribbonFirst_Dash == true)
                {
                    ribbon.Items.Remove(rbnPreview);
                    ribbon.Items.Insert(0,rbnPreview);
                    borderMiz.Margin = new Thickness(0, 0, 60, 0);
                    return;
                }

                // گرفتن مختصات تب نسبت به کل صفحه (Screen)
                var screenPoint =630- (SystemParameters.PrimaryScreenWidth - rbnPreview.PointToScreen(new System.Windows.Point(0, 0)).X);

                borderMiz.Margin=new Thickness(0,0,632-screenPoint,0);
            }
        }

        private void tabcontrol_TabClosed(object sender, CloseTabEventArgs e)
        {
            ((e.TargetTabItem.Content as Grid).Children[0] as IDisposable)?.Dispose();
            tabcontrol.Items.Remove(e.TargetTabItem);
            /*if (DeskWindow.OpenTabs.FirstOrDefault(t => (t.Tag as Dictionary<RibbonButton, UserControl>).First().Value == (e.TargetTabItem.Content as Grid).Children[0]) is Border border)
                DeskWindow.OpenTabs.Remove(border);*/
        }

        private void tabcontrol_TabClosing(object sender, CancelingRoutedEventArgs e)
        {
            var n = (e.OriginalSource as TabItemExt).Content;
            (n as FrameworkElement).Effect = new BlurEffect() { Radius = 4 };
            if (n is ITabForm tab)
                e.Cancel = !tab.CloseForm();
            else
                e.Cancel = !((n as Grid).Children[0] as ITabForm).CloseForm();
            (n as FrameworkElement).Effect = null;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {

        }

        private void ribbon_RibbonStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //if (ribbon.SelectedItem is RibbonTab tab && tab.Name == "rbnPreview")
            //{
            //    ribbon.RibbonStateChanged -= ribbon_RibbonStateChanged;
            //    ribbon.RibbonState = Syncfusion.Windows.Tools.RibbonState.Hide;
            //    ribbon.RibbonStateChanged += ribbon_RibbonStateChanged;
            //    row.Height = new GridLength();
            //    return;
            //}
            if(ribbon.RibbonState== Syncfusion.Windows.Tools.RibbonState.Hide)
            {
                row.Height = new GridLength();
            }
            else
                row.Height = new GridLength(160);
        }

        private void rbnGroup_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "گروه تفضیلی");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("گروه تفضیلی", new usrGroup(),sender , "Definitions/preferentialGroup.png");
        }

        private void rbnPreferential_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "حساب تفضیلی");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("حساب تفضیلی", new usrPreferential(),sender , "Definitions/preferential.jpg");
        }

        private void rbnAgroup_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "گروه حساب");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("گروه حساب", new usrAgroup(),sender , "Definitions/agroup.png");
        }

        private void tabcontrol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ribbon.RibbonState = Syncfusion.Windows.Tools.RibbonState.Hide;
            row.Height = new GridLength();            
        }

        private void rbnAcType_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "نوع سند");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("نوع سند", new usrAcType(),sender , "Definitions/acTypecopy.png");
        }

        private void rbnAcDoc_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "سند حسابداری");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("سند حسابداری", new usrAccountDocument(),sender , "Definitions/acDoc.png");
        }
        const byte VK_F2 = 0x71;
        const uint KEYEVENTF_KEYUP = 0x0002;
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && (tabcontrol.SelectedItem as TabItemExt)?.Content is Grid grid2d && grid2d.Children[0] is ITabForm tabForm)
            {
                tabForm.CloseForm();
            }
            else if (e.Key == Key.F5 && borderMiz.Visibility == Visibility.Visible)
            {
                var map = BuildRibbonButtonMap();
                var desk = new DeskWindow(StatusOptions.User.Id, map);
                desk.Show();
            }
            else if ((tabcontrol.SelectedItem as TabItemExt)?.Content is Grid grid2 && grid2.Children[0] is ITabEdidGrid usrAccountDocument)
            {
                if ((usrAccountDocument.DataGridIsFocused || (Keyboard.FocusedElement as FrameworkElement)?.GetParentOfType<Syncfusion.UI.Xaml.Grid.SfDataGrid>() is Syncfusion.UI.Xaml.Grid.SfDataGrid) && e.Key == Key.Enter)
                {
                    usrAccountDocument.SetEnterToNextCell();
                    e.Handled = true;
                    return;
                }
            }
            if ((tabcontrol.SelectedItem as TabItemExt)?.Content is Grid grid && grid.Children[0] is UserControl userControl)
            {
                if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Keyboard.IsKeyDown(Key.S))
                {
                    Type type = userControl.GetType();
                    var btnSave = userControl.FindName("btnConfirm") as FrameworkElement;
                    if (btnSave != null && (btnSave.Parent as Grid).Visibility == Visibility.Collapsed)
                        return;
                    MethodInfo method = type.GetMethod("btnConfirm_Click", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (method != null)
                    {
                        e.Handled = true;
                        object[] parameters = new object[] { null, null };

                        method.Invoke(userControl, parameters);
                    }
                    return;
                }
                if(Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.N))
                {
                    var focused = Keyboard.FocusedElement;
                    var sfDataGrid = focused as Syncfusion.UI.Xaml.Grid.SfDataGrid
                 ?? (focused as FrameworkElement)?.GetParentOfType<Syncfusion.UI.Xaml.Grid.SfDataGrid>();
                    if (sfDataGrid?.SelectionController.CurrentCellManager?.CurrentCell is Syncfusion.UI.Xaml.Grid.DataColumn column && GetClick(column.GridColumn.MappingName) is string str0)
                    {
                        Type type = GetType();
                        MethodInfo method = type.GetMethod(str0, BindingFlags.NonPublic | BindingFlags.Instance);

                        if (method != null)
                        {
                            e.Handled = true;
                            var rbn = FindName(str0.Replace("_Click", "")) as FrameworkElement;
                            rbn.Tag = column.GridColumn;
                            var my = new MyPublisher()
                            {
                                eventNav = new EventNav(rbn as RibbonButton, "")
                            };
                            object[] parameters = new object[] { my, null };

                            Mouse.OverrideCursor = Cursors.Wait;
                            method.Invoke(this, parameters);
                            my.MyEvent += (s1, e1) =>
                            {
                                //textBox.Text = (e1 as EventNav).Message;
                                dynamic y = null;
                                var element = (sfDataGrid.SelectionController.CurrentCellManager.CurrentCell.Element as Syncfusion.UI.Xaml.Grid.GridCell)
                                        .Content as FrameworkElement;
                                y = element.DataContext;
                                if (sfDataGrid.SelectedIndex == -1 || element is TextBlock)
                                {
                                    sfDataGrid.GetParentOfType<UserControl>().Tag = (e1 as EventNav).Message;                                    
                                }                                
                            };

                            Mouse.OverrideCursor = null;
                        }
                    }
                    else if (focused is TextBox textBox&&textBox.GetParentOfType<SfTextInputLayout>() is SfTextInputLayout sfTextInputLayout &&sfTextInputLayout.Tag is string str)
                    {
                        Type type = GetType();
                        MethodInfo method = type.GetMethod(str, BindingFlags.NonPublic | BindingFlags.Instance);

                        if (method != null)
                        {
                            e.Handled = true;
                            var rbn = FindName(str.Replace("_Click", "")) as FrameworkElement;
                            rbn.Tag = textBox;
                            var my = new MyPublisher()
                            {
                                eventNav = new EventNav(rbn as RibbonButton, "")
                            };
                            object[] parameters = new object[] { my, null };

                            Mouse.OverrideCursor = Cursors.Wait;
                            method.Invoke(this, parameters);
                            my.MyEvent += (s1, e1) =>
                            {
                                textBox.Text = (e1 as EventNav).Message;
                                // Raise کردن LostFocus
                                textBox.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));
                            };
                            
                            Mouse.OverrideCursor = null;
                        }
                    }

                }
            }
        }
        public string GetClick(string str)
        { 
            switch(str) 
            {
                case "CommodityCode":
                    return "rbnDefinitionCommodity_Click";
            }
            return null;
        }
        private void rbnBank_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "بانک");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("بانک", new usrBank(),sender , "Definitions/bank2.png");
        }

        private void rbnRecieveMoney_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "دریافت وجه");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("دریافت وجه", new usrRecieveMoney(),sender , "Definitions/recieveMoney4.png");
        }

        private void rbnPaymentMoney_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "پرداخت وجه");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("پرداخت وجه", new usrPaymentMoney(),sender , "Definitions/paymentMoney3.png");
        }

        private void rbnRecieveCheck_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "چک های دریافتی");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("چک های دریافتی", new usrRecieveCheck(), sender , "Definitions/recieveCheck.png");
        }

        private void rbnPaymentCheck_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "چک های پرداختی");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("چک های پرداختی", new usrPaymentCheck(), sender , "Definitions/paymentCheck.png");
        }

        private void rbnProvince_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "استان");
            if (item != null)
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            else
                AddTabWithTriangle("استان", new usrProvince(),sender , "Commerce/province.jpg");
        }
    
        private void AddTabWithTriangle(string header, UserControl userControl, object sender,string imagename,bool isTringle=true)
        {
            // ایجاد تب
            var headerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0),
            };

            var icon = new Image
            {
                Source = imagename == "" ? null : new BitmapImage(new Uri("pack://application:,,,/Images/" + imagename)),
                Width = 23,
                Height = 23,
                Margin = new Thickness(0, 0, 5, 0), // فاصله بین عکس و متن
            };
            if (imagename == "")
                icon.Visibility = Visibility.Collapsed;
            var title = new TextBlock
            {
                Text = header,
                VerticalAlignment = VerticalAlignment.Center
            };

            headerPanel.Children.Add(icon);
            headerPanel.Children.Add(title);

            var item = new TabItemExt { Header = headerPanel,Tag=header };

            // Container برای overlay کردن محتوا و مثلث
            var container = new Grid();
            RibbonButton tabItemExt = null;

            // اضافه کردن محتوا (UserControl)
            container.Children.Add(userControl);
            if (isTringle)
            {
                using var db = new wpfrazydbContext();

                if (header == "حساب تفضیلی")
                    header = "تفضیلی";
                if (header == "حساب کل")
                    header = "کل";
                if (header == "حساب معین")
                    header = "معین";
                if (header == "سند حسابداری")
                    header = "سند حسابداری";
                if (sender is MyPublisher publisher)
                {
                    tabItemExt = publisher.eventNav.MyRibbonButton;
                }
                else
                    tabItemExt = sender as RibbonButton;
                var per = db.Permissions.Include(t => t.FkRibbonItem).FirstOrDefault(u => u.FkUserGroupId == StatusOptions.User.FkUserGroupId && u.FkRibbonItem.DisplayName == header && u.FkRibbonItem.Category.Contains(((tabItemExt.Parent as RibbonBar).Parent as RibbonTab).Caption));
                var btnSave = userControl.FindName("btnConfirm") as FrameworkElement;
                var btnDelete = userControl.FindName("btnDelete") as FrameworkElement;
                if (btnSave != null)
                {
                    (btnSave.Parent as Grid).IsVisibleChanged += btnSave_IsVisibleChanged;
                }
                if (btnDelete != null)
                {
                    (btnDelete.Parent as Grid).IsVisibleChanged += btnDelete_IsVisibleChanged;
                }
                if (per.CanInsert == false)
                {
                    if (btnSave != null)
                        (btnSave.Parent as Grid).Visibility = Visibility.Collapsed;
                }

                // ایجاد مثلث آبی
                var triangle = CreateGreenTriangle(per.CanInsert == true);
                triangle.Tag = per;
                triangle.VerticalAlignment = VerticalAlignment.Top;
                triangle.HorizontalAlignment = HorizontalAlignment.Left;
                Panel.SetZIndex(triangle, 1);
                container.Children.Add(triangle);

                // وصل کردن واکنش به borderEdit داخل UserControl (اگر وجود داشته باشد)
                var borderEditField = userControl.FindName("borderEdit") as FrameworkElement;
                if (borderEditField != null)
                {
                    borderEditField.IsVisibleChanged += (s, e) =>
                    {
                        triangle.Visibility = borderEditField.Visibility == Visibility.Visible
                            ? Visibility.Collapsed
                            : Visibility.Visible;
                        if (borderEditField.Visibility == Visibility.Visible)
                        {
                            ((btnSave.Parent as Grid).Children[1] as Image).Source = new BitmapImage(new Uri("pack://application:,,,/Images/Save - Copy.png"));
                            if (per.CanModify == true)
                                (btnSave.Parent as Grid).Visibility = Visibility.Visible;
                            else
                                (btnSave.Parent as Grid).Visibility = Visibility.Collapsed;
                        }
                        else if (borderEditField.Visibility != Visibility.Visible)
                        {
                            ((btnSave.Parent as Grid).Children[1] as Image).Source = new BitmapImage(new Uri("pack://application:,,,/Images/Save.png"));
                            if (per.CanInsert == true)
                                (btnSave.Parent as Grid).Visibility = Visibility.Visible;
                            else
                                (btnSave.Parent as Grid).Visibility = Visibility.Collapsed;
                        }
                    };
                }
                else
                    triangle.Visibility = Visibility.Collapsed;
                var datagridSearchField = userControl.FindName("datagridSearch") as FrameworkElement;
                if (datagridSearchField != null)
                {
                    datagridSearchField.IsVisibleChanged += (s, e) =>
                    {
                        if (datagridSearchField.Visibility == Visibility.Visible)
                        {
                            triangle.Visibility = Visibility.Collapsed;
                        }
                        else
                            triangle.Visibility = borderEditField.Visibility == Visibility.Visible
                                ? Visibility.Collapsed
                                : Visibility.Visible;
                    };
                }
                if (tabItemExt.Tag != null)
                {
                    //(userControl as IEventTools).MyEventh += MainWindow_MyEventh;
                    userControl.Tag = sender;
                }
            }
            // اضافه به تب‌ها
            item.Content = container;
            tabcontrol.Items.Add(item);

            /*if (DeskWindow.OpenTabs.FirstOrDefault(t => (t.Tag as Dictionary<RibbonButton, UserControl>).First().Key == tabItemExt) is Border border)
            {
                (border.Tag as Dictionary<RibbonButton, UserControl>)[tabItemExt] = userControl;
                DeskWindow.CurrentTab = border;
            }*/
        }

        private void btnDelete_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Grid grid && (((grid.DataContext as UserControl).Parent as Grid).Children[1] as Path).Tag is Permission permission)
            {
                if (grid.Visibility == Visibility.Visible)
                {
                    if (permission.CanDelete != true)
                    {
                        grid.IsVisibleChanged -= btnDelete_IsVisibleChanged;
                        grid.Visibility= Visibility.Collapsed;
                        grid.IsVisibleChanged += btnDelete_IsVisibleChanged;
                    }
                }

           }
        }
        private void btnSave_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Grid grid && (((grid.DataContext as UserControl).Parent as Grid).Children[1] as Path).Tag is Permission permission)
            {
                if (grid.Visibility == Visibility.Visible)
                {
                    if (permission.CanInsert != true&&(grid.DataContext as UserControl).FindName("borderEdit") is Border border&&border.Visibility!= Visibility.Visible)
                    {
                        grid.IsVisibleChanged -= btnSave_IsVisibleChanged;
                        grid.Visibility = Visibility.Collapsed;
                        grid.IsVisibleChanged += btnSave_IsVisibleChanged;
                    }
                }

            }
        }
        private FrameworkElement CreateGreenTriangle(bool green=true)
        {
            // براش برای پر کردن مثلث (حتماً باید SolidColorBrush باشه تا رنگش انیمیت بشه)
            var fillBrush = green == true ? new SolidColorBrush(Colors.Green): new SolidColorBrush(Colors.Gray);
            var leafPath = Geometry.Parse(
    "M 10,0 C 15,5 20,15 10,20 C 0,15 5,5 10,0 Z"
);

            var leaf = new System.Windows.Shapes.Path
            {
                Opacity = .7,
                Data = leafPath,
                Fill = fillBrush,
                Stroke = green == true ? Brushes.DarkGreen : Brushes.DarkGray,
                StrokeThickness = 1,
                ToolTip = "جدید" + (green ? "" : " - عدم دسترسی"),
                Width = 20,
                Height = 20,
                Margin = new Thickness(1),
                Stretch = Stretch.Fill
            };
    //        var triangle = new System.Windows.Shapes.Polygon
    //        {
    //            Points = new PointCollection
    //{
    //    new Point(10,0),
    //    new Point(12,7),
    //    new Point(20,7),
    //    new Point(13.5,12),
    //    new Point(16,20),
    //    new Point(10,15),
    //    new Point(4,20),
    //    new Point(6.5,12),
    //    new Point(0,7),
    //    new Point(8,7)
    //},
    //            Fill = fillBrush,
    //            Stroke = green == true ? Brushes.DarkGreen : Brushes.DarkGray,
    //            StrokeThickness = 1,
    //            ToolTip = "جدید" + (green ? "" : " - عدم دسترسی"),
    //            Width = 20,
    //            Height = 20,
    //            Margin = new Thickness(1),
    //            Stretch = Stretch.Fill   // تا در همان سایز فیت شود
    //        };

            // انیمیشن رنگ (از سبز معمولی به سبز روشن‌تر و برگشت)
            var animation = new ColorAnimation
            {                
                From = green ? Colors.GreenYellow : Colors.MistyRose,
                To = green ? Colors.LimeGreen : Colors.Crimson,
                Duration = TimeSpan.FromSeconds(0.6),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            // شروع انیمیشن بدون Storyboard
            fillBrush.BeginAnimation(SolidColorBrush.ColorProperty, animation);

            return leaf;
        }
       
        private void rbnCity_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "شهر");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("شهر", new usrCity(),sender , "Commerce/province.jpg");
        }

        private void rbnPriceGroup_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "گروه قیمت");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("گروه قیمت", new usrPriceGroup(),sender , "Commerce/priceGroup.png");
        }

        private void rbnCustomerGroup_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "گروه مشتریان");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("گروه مشتریان", new usrCustomerGroup(),sender , "Commerce/customerGroup.png");
        }

        private void rbnGroupStorage_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "گروه انبار");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("گروه انبار", new usrGroupStorage(),sender , "Commerce/groupStorage.jpg");
        }

        private void rbnDefinitionStorage_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "انبار");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("انبار", new usrStorage(),sender , "Commerce/DefinitionStorage.png");
        }

        private void rbnUnit_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "واحد اندازه گیری");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("واحد اندازه گیری", new usrUnit(),sender , "Commerce/unit.jpg");
        }

        private void rbnGroupCommodity_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "گروه کالا");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else
                AddTabWithTriangle("گروه کالا", new usrGroupCommodity(),sender , "Commerce/groupCommodity.jpg");
        }

        private void rbnDefinitionCommodity_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "کالا");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("کالا", new usrCommodity(),sender , "Commerce/commodity.png");
        }

        private void rbnCommodityPricingPanel_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "پنل قیمت گذاری کالا");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("پنل قیمت گذاری کالا", new usrCommodityPricingPanel(),sender , "Commerce/commodityPricingPanel.jpg");
        }

        private void rbnCodingReceiptTypes_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "انواع رسید");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("انواع رسید", new usrCodingReceiptTypes(),sender , "Commerce/receiptTypes.jpg");
        }
        private void rbnCodingTypesTransfer_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "انواع حواله");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("انواع حواله", new usrCodingTypesTransfer(),sender , "Commerce/receiptTypes.jpg");
        }

        private void rbnStorageReceipt_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "رسید انبار");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("رسید انبار", new usrStorageReceipt(),sender , "Commerce/storageReceipt.jpg");
        }

        private void rbnStorageTransfer_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "حواله انبار");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("حواله انبار", new usrStorageTransfer(),sender , "Commerce/storageReceipt.jpg");
        }

        private void rbnStorageBetweenTransfer_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "حواله بین انبار");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("حواله بین انبار", new usrStorageBetweenTransfer(),sender , "Commerce/storageBetweenTransfer.jpg");
        }

        private void rbnStorageRotation_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "انبارگردانی");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("انبارگردانی", new usrStorageRotation(),sender , "Commerce/StorageRotation.jpg");
        }

        private void rbnNPStorage_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "کسر و اضافات انبار");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else
            {
                item = new TabItemExt() { Header = "کسر و اضافات انبار" };
                item.Content = new usrNPStorage();
                tabcontrol.Items.Add(item);
            }
        }

        private void rbnOrder_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "سفارش");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }    
            else
                AddTabWithTriangle("سفارش", new usrOrder(),sender , "Commerce2/order.jpg");
        }

        private void rbnPurchaseInvoice_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "فاکتور خرید");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("فاکتور خرید", new usrProductBuy(),sender , "Commerce2/purchaseInvoice.jpg");
        }

        private void rbnSalesInvoice_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "فاکتور فروش");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("فاکتور فروش", new usrProductSell(),sender , "Commerce2/salesInvoice.jpg");
        }

        private void rbnSalesProforma_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "پیش فاکتور فروش");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("پیش فاکتور فروش", new usrPreInvoice(),sender , "Commerce2/salesProforma.jpg");
        }

        private void rbnConfiguration_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "مالی");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else
                AddTabWithTriangle("مالی", new usrSettingConfig(), sender , "Tools/configuration.png", false);            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Effect = new BlurEffect() { Radius = 5 };
            if (Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید از برنامه بازرگانی رازی خارج شوید؟", "خروج", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
                this.Effect = null;
                return;
            }
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                await Task.Delay(500);
                Environment.Exit(0);
            }));
        }

        private void rbnBrowseAccounts_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "مرور حساب ها");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else
                AddTabWithTriangle("مرور حساب ها", new usrBrowseAccounts(), sender , "reports/BrowseAccounts.jpg",false);            

            //var report = new StiReport();
            //report.Load("Report.mrt");

            //// فرض بر اینکه ItemsSource گرید از نوع ObservableCollection<Customer> باشد
            //var data = dataGrid.ItemsSource as IEnumerable<Customer>;
            //report.RegBusinessObject("CustomerData", data);  // "CustomerData" باید با نام منبع داده در .mrt مطابقت داشته باشد

            //report.Compile();
            //report.Render();
            //report.ShowWithWpf(); // یا: report.Print(); یا report.ExportDocument(StiExportFormat.Pdf, ...);
        }

        private void rbnUser_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "کاربر");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("کاربر", new usrUser(),sender , "Tools/User1.png");
        }

        private void rbnUserGroup_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "گروه کاربر");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else            
                AddTabWithTriangle("گروه کاربر", new usrUserGroup(),sender , "Tools/UserGroup1.png");
        }

        private void rbnPermissionManager_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "سطح دسترسی");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else
                AddTabWithTriangle("سطح دسترسی", new usrPermissionManager(), sender , "Tools/Premession1.png",false);            
        }

        private void rbnConfiguration2_Click(object sender, RoutedEventArgs e)
        {
            if (ViewFormLeftRigth)
            {
                if (Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید نمایش فرم از بالا به پایین باشد و برنامه دوباره اجرا شود؟", "نحوه نمایش فرم", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    System.IO.File.WriteAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Setting.txt"), "false");
                    App.Current.Shutdown();
                    string jsonArg = JsonSerializer.Serialize(WpfRaziLedgerApp.MainWindow.StatusOptions);

                    // توجه: چون بعضی کاراکترها ممکنه برای command line مناسب نباشن، پیشنهاد می‌شه encode کنی:
                    string encodedArg = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(jsonArg));

                    // Start process with encoded argument
                    Process.Start(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfRaziLedgerApp.exe"), encodedArg);
                }
            }
            else
            {
                if (Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید نمایش فرم از چپ به راست باشد و برنامه دوباره اجرا شود؟", "نحوه نمایش فرم", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    System.IO.File.WriteAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Setting.txt"), "true");
                    App.Current.Shutdown();
                    string jsonArg = JsonSerializer.Serialize(WpfRaziLedgerApp.MainWindow.StatusOptions);

                    // توجه: چون بعضی کاراکترها ممکنه برای command line مناسب نباشن، پیشنهاد می‌شه encode کنی:
                    string encodedArg = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(jsonArg));

                    // Start process with encoded argument
                    Process.Start(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfRaziLedgerApp.exe"), encodedArg);
                }
            }
        }

        private void rbnBuyRemittance_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "گزارش حواله خرید");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else
                AddTabWithTriangle("گزارش حواله خرید", new usrBuyRemittance(), sender , "reports/BuyRemittance.png",false);            
        }

        private void rbnSellReport_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "گزارش فروش");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else
                AddTabWithTriangle("گزارش فروش", new usrSellReport(), sender , "reports/SellReport.png", false);
        }

        private void rbnSupport_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "پشتیبان");
            if (item != null)
            {
                {tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender;}
            }
            else
                AddTabWithTriangle("پشتیبان", new BackupRestoreControl(), sender , "Tools/backup.png", false);
        }
        private Dictionary<Guid, RibbonButton> BuildRibbonButtonMap()
        {
            var map = new Dictionary<Guid, RibbonButton>();

            // فرض: keyValuePairs قبلاً داری (RibbonButton -> RibbonItemId)
            foreach (var kv in keyValuePairs)
            {
                map[kv.Value] = kv.Key;
            }

            return map;
        }
        private void rbnDoshboardSetting_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "میز کار");
            if (item != null)
            {
                { tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender; }
            }
            else
            {
                var map = BuildRibbonButtonMap();
                var settings = new DeskSettingsWindow(StatusOptions.User.Id, map);
                AddTabWithTriangle("میز کار", settings, sender, "Tools/DoshboardSetting2.png", false);
            }
        }

        private async void rbnPreview_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (e.LeftButton != MouseButtonState.Pressed)
            //    return;
            //ribbon.Visibility= Visibility.Collapsed;
            //var map = BuildRibbonButtonMap();
            //var desk = new DeskWindow(StatusOptions.User.Id, map);
            //desk.Show();
            //await Dispatcher.BeginInvoke((Action)(async () =>
            //{
            //    await Task.Delay(100);
            //    ribbon.Visibility= Visibility.Visible;
            //    ribbon.RibbonStateChanged -= ribbon_RibbonStateChanged;
            //    ribbon.RibbonState = Syncfusion.Windows.Tools.RibbonState.Hide;
            //    ribbon.RibbonStateChanged += ribbon_RibbonStateChanged;
            //    row.Height = new GridLength(); 
            //}));
        }
        private void borderMiz_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;
            var map = BuildRibbonButtonMap();
            var desk = new DeskWindow(StatusOptions.User.Id, map);
            desk.Show();
        }

        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (ribbon.RibbonState == Syncfusion.Windows.Tools.RibbonState.Adorner)
                if (e.Delta > 0)
                {
                    // چرخ موس به سمت بالا حرکت کرده
                    try
                    {
                        ribbon.SelectedIndex -= 1;
                    }
                    catch { }
                }
                else if (e.Delta < 0)
                {
                    try
                    {
                        ribbon.SelectedIndex += 1;
                    }
                    catch { }
                    // چرخ موس به سمت پایین حرکت کرده
                }
        }

        private void rbnClosingtemporaryaccounts_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rbnIssuanceofopeningdocument_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rbnIssuanceofclosingdocument_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rbnBill_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "صورتحساب");
            if (item != null)
            {
                { tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender; }
            }
            else
                AddTabWithTriangle("صورتحساب", new usrBillReport(), sender, "", false);
        }

        private void rbnCardex_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rbnCommerce_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rbnStoage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rbnElectronicOO_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "خروجی دفتر الکترونیکی");
            if (item != null)
            {
                { tabcontrol.SelectedItem = item; if (sender is MyPublisher publisher) ((item.Content as Grid).Children[0] as FrameworkElement).Tag = sender; }
            }
            else
                AddTabWithTriangle("خروجی دفتر الکترونیکی", new usrElectronicOO(), sender, "", false);
        }

        private void rbnPrint_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rbnDate_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

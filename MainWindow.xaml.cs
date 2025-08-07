using Microsoft.EntityFrameworkCore;
using Syncfusion.Linq;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
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
            var gifImage = new BitmapImage(new Uri("pack://application:,,,/Images/AddDataLarge.gif"));
            XamlAnimatedGif.AnimationBehavior.SetSourceUri(this.gifImage, gifImage.UriSource);
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
        private void ApplyPermissions(Guid userGroupId)
        {
            // 1. دریافت دسترسی‌ها از دیتابیس
            var allowedIds = GetPermissionIdsForGroup(userGroupId); // List<Guid>
            if(allowedIds.Count == 0) return;

            // 2. مرور کل آیتم‌های Ribbon
            foreach (var tab in ribbon.Items.OfType<RibbonTab>())
            {
                bool hasVisibleChild = false;

                foreach (var bar in tab.Items.OfType<RibbonBar>())
                {
                    foreach (var item in bar.Items.OfType<UIElement>())
                    {
                        if (item is RibbonButton btn && btn.Label != null)
                        {
                            var ribbonId = GetRibbonItemIdByName(btn.Label); // گرفتن Id از نام دکمه
                            bool canAccess = allowedIds.Contains(ribbonId);

                            btn.Visibility = canAccess ? Visibility.Visible : Visibility.Collapsed;

                            if (canAccess)
                                hasVisibleChild = true;
                        }
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

        private Guid GetRibbonItemIdByName(string buttonName)
        {
            using (var context = new wpfrazydbContext())
            {
                var item = context.RibbonItems.FirstOrDefault(r => r.DisplayName == buttonName || r.Category == buttonName);
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
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("حساب کل", new winCol(),sender as RibbonButton, "Definitions/col.png");
        }

        private void rbnMoein_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "حساب معین");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("حساب معین", new winMoein(),sender as RibbonButton, "Definitions/moeinPng.png");
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
        }

        private void tabcontrol_TabClosed(object sender, CloseTabEventArgs e)
        {
            (e.TargetTabItem.Content as IDisposable)?.Dispose();
            tabcontrol.Items.Remove(e.TargetTabItem);
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
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("گروه تفضیلی", new usrGroup(),sender as RibbonButton, "Definitions/preferentialGroup.png");
        }

        private void rbnPreferential_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "حساب تفضیلی");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("حساب تفضیلی", new usrPreferential(),sender as RibbonButton, "Definitions/preferential.jpg");
        }

        private void rbnAgroup_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "گروه حساب");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("گروه حساب", new usrAgroup(),sender as RibbonButton, "Definitions/agroup.png");
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
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("نوع سند", new usrAcType(),sender as RibbonButton, "Definitions/acTypecopy.png");
        }

        private void rbnAcDoc_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "سند حسابداری");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("سند حسابداری", new usrAccountDocument(),sender as RibbonButton, "Definitions/acDoc.png");
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((tabcontrol.SelectedItem as TabItemExt)?.Content is ITabEdidGrid usrAccountDocument)
            {
                if (usrAccountDocument.DataGridIsFocused && e.Key == Key.Enter)
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
                }
            }
        }

        private void rbnBank_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "بانک");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("بانک", new usrBank(),sender as RibbonButton, "Definitions/bank copy.png");
        }

        private void rbnRecieveMoney_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "دریافت وجه");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("دریافت وجه", new usrRecieveMoney(),sender as RibbonButton, "Definitions/recieveMoney.png");
        }

        private void rbnPaymentMoney_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "پرداخت وجه");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("پرداخت وجه", new usrPaymentMoney(),sender as RibbonButton, "Definitions/recieveMoney.png");
        }

        private void rbnRecieveCheck_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "چک های دریافتی");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("چک های دریافتی", new usrRecieveCheck(), sender as RibbonButton, "Definitions/recieveCheck copy.png");
        }

        private void rbnPaymentCheck_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "چک های پرداختی");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("چک های پرداختی", new usrPaymentCheck(), sender as RibbonButton, "Definitions/recieveCheck copy.png");
        }

        private void rbnProvince_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "استان");
            if (item != null)
                tabcontrol.SelectedItem = item;
            else
                AddTabWithTriangle("استان", new usrProvince(),sender as RibbonButton, "Commerce/province.jpg");
        }
    
        private void AddTabWithTriangle(string header, UserControl userControl, RibbonButton tabItemExt,string imagename="print.png")
        {
            // ایجاد تب
            var headerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0),
            };

            var icon = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Images/"+ imagename)),
                Width = 23,
                Height = 23,
                Margin = new Thickness(0, 0, 5, 0), // فاصله بین عکس و متن
            };

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

            // اضافه کردن محتوا (UserControl)
            container.Children.Add(userControl);
            using var db=new wpfrazydbContext();
            
            if(header=="حساب تفضیلی")
                header="تفضیلی";
            if (header == "حساب کل")
                header = "کل";
            if (header == "حساب معین")
                header = "معین";
            if (header == "سند حسابداری")
                header = " سند حسابداری";
            var per = db.Permissions.Include(t => t.FkRibbonItem).FirstOrDefault(u => u.FkUserGroupId == StatusOptions.User.FkUserGroupId && u.FkRibbonItem.DisplayName == header && ((tabItemExt.Parent as RibbonBar).Parent as RibbonTab).Caption == u.FkRibbonItem.Category);
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

            // ایجاد مثلث سبز
            var triangle = CreateGreenTriangle(per.CanInsert==true);
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
                        if (per.CanModify == true)
                            (btnSave.Parent as Grid).Visibility = Visibility.Visible;
                        else
                            (btnSave.Parent as Grid).Visibility = Visibility.Collapsed;
                    }
                    else if (borderEditField.Visibility != Visibility.Visible)
                    {
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

            // اضافه به تب‌ها
            item.Content = container;
            tabcontrol.Items.Add(item);
            tabcontrol.SelectedItem = item;
        }

        private void btnDelete_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Grid grid && (((grid.DataContext as UserControl).Parent as Grid).Children[1] as Polygon).Tag is Permission permission)
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
            if (sender is Grid grid && (((grid.DataContext as UserControl).Parent as Grid).Children[1] as Polygon).Tag is Permission permission)
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

            var triangle = new System.Windows.Shapes.Polygon
            {
                Points = new PointCollection { new Point(0, 0), new Point(22, 0), new Point(0, 22) },
                Fill = fillBrush,
                Stroke = green == true ? Brushes.DarkGreen : Brushes.DarkGray,
                StrokeThickness = 1,
                ToolTip = "جدید"+(green?"": " - عدم دسترسی"),
                Width = 20,
                Height = 20,
                Margin = new Thickness(1)
            };

            // انیمیشن رنگ (از سبز معمولی به سبز روشن‌تر و برگشت)
            var animation = new ColorAnimation
            {
                From = green ? Colors.LimeGreen : Colors.MistyRose,
                To = green ? Colors.LightGreen : Colors.Crimson,
                Duration = TimeSpan.FromSeconds(0.6),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            // شروع انیمیشن بدون Storyboard
            fillBrush.BeginAnimation(SolidColorBrush.ColorProperty, animation);

            return triangle;
        }
       
        private void rbnCity_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "شهر");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("شهر", new usrCity(),sender as RibbonButton, "Commerce/province.jpg");
        }

        private void rbnPriceGroup_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "گروه قیمت");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("گروه قیمت", new usrPriceGroup(),sender as RibbonButton, "Commerce/priceGroup.png");
        }

        private void rbnCustomerGroup_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "گروه مشتریان");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("گروه مشتریان", new usrCustomerGroup(),sender as RibbonButton, "Commerce/customerGroup.png");
        }

        private void rbnGroupStorage_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "گروه انبار");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("گروه انبار", new usrGroupStorage(),sender as RibbonButton, "Commerce/groupStorage.jpg");
        }

        private void rbnDefinitionStorage_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "انبار");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("انبار", new usrStorage(),sender as RibbonButton, "Commerce/DefinitionStorage.png");
        }

        private void rbnUnit_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "واحد اندازه گیری");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("واحد اندازه گیری", new usrUnit(),sender as RibbonButton, "Commerce/unit.jpg");
        }

        private void rbnGroupCommodity_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "گروه کالا");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
                AddTabWithTriangle("گروه کالا", new usrGroupCommodity(),sender as RibbonButton, "Commerce/groupCommodity.jpg");
        }

        private void rbnDefinitionCommodity_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "کالا");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("کالا", new usrCommodity(),sender as RibbonButton, "Commerce/commodity.png");
        }

        private void rbnCommodityPricingPanel_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "پنل قیمت گذاری کالا");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("پنل قیمت گذاری کالا", new usrCommodityPricingPanel(),sender as RibbonButton, "Commerce/commodityPricingPanel.jpg");
        }

        private void rbnCodingReceiptTypes_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "کدینگ انواع رسید");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("کدینگ انواع رسید", new usrCodingReceiptTypes(),sender as RibbonButton, "Commerce/receiptTypes.jpg");
        }
        private void rbnCodingTypesTransfer_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "کدینگ انواع حواله");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("کدینگ انواع حواله", new usrCodingTypesTransfer(),sender as RibbonButton, "Commerce/receiptTypes.jpg");
        }

        private void rbnStorageReceipt_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "رسید انبار");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("رسید انبار", new usrStorageReceipt(),sender as RibbonButton, "Commerce/storageReceipt.jpg");
        }

        private void rbnStorageTransfer_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "حواله انبار");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("حواله انبار", new usrStorageTransfer(),sender as RibbonButton, "Commerce/storageReceipt.jpg");
        }

        private void rbnStorageBetweenTransfer_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "حواله بین انبار");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("حواله بین انبار", new usrStorageBetweenTransfer(),sender as RibbonButton, "Commerce/storageBetweenTransfer.jpg");
        }

        private void rbnStorageRotation_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "انبارگردانی");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("انبارگردانی", new usrStorageRotation(),sender as RibbonButton, "Commerce/StorageRotation.jpg");
        }

        private void rbnNPStorage_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "کسر و اضافات انبار");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
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
                tabcontrol.SelectedItem = item;
            }    
            else
                AddTabWithTriangle("سفارش", new usrOrder(),sender as RibbonButton, "Commerce2/order.jpg");
        }

        private void rbnPurchaseInvoice_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "فاکتور خرید");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("فاکتور خرید", new usrProductBuy(),sender as RibbonButton, "Commerce2/purchaseInvoice.jpg");
        }

        private void rbnSalesInvoice_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "فاکتور فروش");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("فاکتور فروش", new usrProductSell(),sender as RibbonButton, "Commerce2/salesInvoice.jpg");
        }

        private void rbnSalesProforma_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "پیش فاکتور فروش");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("پیش فاکتور فروش", new usrPreInvoice(),sender as RibbonButton, "Commerce2/salesProforma.jpg");
        }

        private void rbnConfiguration_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "تنظیمات پیکربندی");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "تنظیمات پیکربندی" };
                item.Content = new usrSettingConfig();
                tabcontrol.Items.Add(item);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Effect = new BlurEffect() { Radius = 5 };
            if (Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید از برنامه خارج شوید؟", "خروج", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
                this.Effect = null;
            }
        }

        private void rbnBrowseAccounts_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "مرور حساب ها");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "مرور حساب ها" };
                item.Content = new usrBrowseAccounts();
                tabcontrol.Items.Add(item);
            }

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
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("کاربر", new usrUser(),sender as RibbonButton, "Tools/User.png");
        }

        private void rbnUserGroup_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "گروه کاربر");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else            
                AddTabWithTriangle("گروه کاربر", new usrUserGroup(),sender as RibbonButton, "Tools/UserGroup.png");
        }

        private void rbnPermissionManager_Click(object sender, RoutedEventArgs e)
        {
            var list = GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Header == "سطح دسترسی");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "سطح دسترسی" };
                item.Content = new usrPermissionManager();
                tabcontrol.Items.Add(item);
            }
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
            var item = list.FirstOrDefault(y => y.Header == "گزارش حواله خرید");
            if (item != null)
            {
                tabcontrol.SelectedItem = item;
            }
            else
            {
                item = new TabItemExt() { Header = "گزارش حواله خرید" };
                item.Content = new usrBuyRemittance();
                tabcontrol.Items.Add(item);
            }
        }
    }
}

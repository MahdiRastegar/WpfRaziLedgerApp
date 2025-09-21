using Syncfusion.Windows.Tools.Controls;
using System;
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
using WpfRaziLedgerApp.Interfaces;
using WpfRaziLedgerApp.RazyDb;

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for DeskSettingsWindow.xaml
    /// </summary>
    public partial class DeskSettingsWindow : UserControl, ITabForm
    {
        private readonly Guid _currentUserId;
        private readonly Dictionary<Guid, RibbonButton> _ribbonMap;

        public DeskSettingsWindow(Guid FkuserId, Dictionary<Guid, RibbonButton> ribbonMap)
        {
            InitializeComponent();
            _currentUserId = FkuserId;
            _ribbonMap = ribbonMap;
            Loaded += DeskSettingsWindow_Loaded;
        }

        private void DeskSettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTopIcons();
            LoadDeskItems();
        }

        private void LoadTopIcons()
        {
            TopIconPanel.Children.Clear();

            foreach (var kv in _ribbonMap)
            {
                var rb = kv.Value;
                var ribbonId = kv.Key;

                var img = new Image
                {
                    Width = 48,
                    Height = 48,
                    Stretch = System.Windows.Media.Stretch.Uniform,
                    Tag = ribbonId,
                    Margin = new Thickness(5)
                };

                if (rb.LargeIcon != null)
                    img.Source = rb.LargeIcon;

                ToolTipService.SetToolTip(img, rb.Label);

                img.MouseMove += TopIcon_MouseMove;

                var border = new Border
                {
                    Width = 64,
                    Height = 64,
                    Margin = new Thickness(5),
                    Background = Brushes.Transparent,
                    Child = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        Children =
        {
            img,
            //new TextBlock
            //{
            //    Text = rb.Label,
            //    FontSize = 10,
            //    HorizontalAlignment = HorizontalAlignment.Center,
            //    TextAlignment = TextAlignment.Center
            //}
        }
                    }
                };

                // استایل Hover
                var style = new Style(typeof(Border));
                var hoverTrigger = new Trigger
                {
                    Property = Border.IsMouseOverProperty,
                    Value = true
                };
                hoverTrigger.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.LightGray));
                hoverTrigger.Setters.Add(new Setter(Border.EffectProperty,
                    new System.Windows.Media.Effects.DropShadowEffect
                    {
                        BlurRadius = 25,
                        ShadowDepth = 0,
                        Color = Colors.DeepSkyBlue,   // می‌تونی Colors.White هم بذاری
                        Opacity = 1.0
                    }));
                style.Triggers.Add(hoverTrigger);

                border.Style = style;

                TopIconPanel.Children.Add(border);
            }
        }

        private void LoadDeskItems()
        {
            PreviewGrid.Children.Clear();

            using var db = new wpfrazydbContext();
            var items = db.UserDesktopItems.Where(u => u.FkuserId == _currentUserId).ToList();

            foreach (var item in items)
            {
                //var id = db.RibbonItems.FirstOrDefault(r => r.Id == item.FkribbonItemId);
                //var ribbonId = id.fkRbMainId;
                if (_ribbonMap.TryGetValue(item.FKRibbonItemMainId, out var rb))
                {
                    var panel = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    var img = new Image
                    {
                        Width = 48,
                        Height = 48,
                        Stretch = Stretch.Uniform,
                        Tag = item.FKRibbonItemMainId
                    };

                    img.Source = rb.LargeIcon;
                    img.MouseMove += TopIcon_MouseMove;

                    var text = new TextBlock
                    {
                        Text = rb.Label,
                        Margin = new Thickness(0, 10, 0, 10),
                        Foreground = Brushes.White,
                        FontSize = 15,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        TextAlignment = TextAlignment.Center
                    };

                    // اضافه به StackPanel
                    panel.Children.Add(img);
                    panel.Children.Add(text);

                    // کانتکست منو برای حذف
                    var ctx = new ContextMenu();
                    var deleteItem = new MenuItem { Header = "حذف از میز کار" };
                    deleteItem.Click += (s, ev) => RemoveDeskItem(item.FKRibbonItemMainId, item.RowIndex, item.ColIndex);
                    ctx.Items.Add(deleteItem);
                    panel.ContextMenu = ctx;

                    // اضافه به Grid
                    Grid.SetRow(panel, item.RowIndex);
                    Grid.SetColumn(panel, item.ColIndex);
                    PreviewGrid.Children.Add(panel);
                }
            }
        }
        private void RemoveDeskItem(Guid ribbonItemId, byte row, byte col)
        {
            using var db = new wpfrazydbContext();
            var entity = db.UserDesktopItems
                           .FirstOrDefault(u => u.FkuserId == _currentUserId
                                             && u.FKRibbonItemMainId == ribbonItemId
                                             && u.RowIndex == row
                                             && u.ColIndex == col);
            if (entity != null)
            {
                db.UserDesktopItems.Remove(entity);
                db.SaveChanges();
            }

            LoadDeskItems();
        }
        private void TopIcon_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is Image img && img.Tag is Guid ribbonId)
            {
                DragDrop.DoDragDrop(img, ribbonId.ToString(), DragDropEffects.Copy);
            }
        }

        private void PreviewGrid_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.StringFormat))
                {
                    var s = e.Data.GetData(DataFormats.StringFormat) as string;
                    if (Guid.TryParse(s, out var ribbonId))
                    {
                        // موقعیت موس نسبت به Grid
                        var pos = e.GetPosition(PreviewGrid);

                        // نزدیک‌ترین خانه Grid
                        var cell = GetNearestCell(pos);
                        if (cell != null)
                        {
                            int row = cell.Value.row;
                            int col = cell.Value.col;

                            //MessageBox.Show($"Drop شد: Row={row}, Col={col}, RibbonId={ribbonId}");

                            // ذخیره در دیتابیس
                            SaveDeskItem(ribbonId, (byte)row, (byte)col);

                            // دوباره بارگذاری آیکون‌ها
                            LoadDeskItems();
                        }
                        else
                        {
                            MessageBox.Show("مختصات معتبر نیست!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در Drop: " + ex.Message);
            }
        }

        private (int row, int col)? GetNearestCell(Point pos)
        {
            double cellW = PreviewGrid.ActualWidth / 5.0; // 5 ستون
            double cellH = PreviewGrid.ActualHeight / 3.0; // 3 سطر

            if (cellW <= 0 || cellH <= 0)
                return null;

            int col = Math.Min(4, Math.Max(0, (int)(pos.X / cellW)));
            int row = Math.Min(2, Math.Max(0, (int)(pos.Y / cellH)));

            return (row, col);
        }

        private void SaveDeskItem(Guid ribbonId, byte row, byte col)
        {
            using var db = new wpfrazydbContext();
            //var id = db.RibbonItems.FirstOrDefault(r => r.fkRbMainId == ribbonId);
            //ribbonId = id.Id;
            var item = db.UserDesktopItems.FirstOrDefault(u => u.FkuserId == _currentUserId && u.FKRibbonItemMainId == ribbonId);
            if (item == null)
            {
                item = new UserDesktopItem
                {
                    Id = Guid.NewGuid(),
                    FkuserId = _currentUserId,
                    FKRibbonItemMainId = ribbonId,
                    RowIndex = row,
                    ColIndex = col
                };
                db.UserDesktopItems.Add(item);
            }
            else
            {
                item.RowIndex = row;
                item.ColIndex = col;
            }
            db.SaveChanges();
        }

        public bool CloseForm()
        {
            var list = MainWindow.Current.GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "میز کار");
            MainWindow.Current.tabcontrol.Items.Remove(item);
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Dispose();
            }));
            return true;
        }

        private void Dispose()
        {
            _ribbonMap.Clear();
        }

        public void SetNull()
        {
            throw new NotImplementedException();
        }

        private void PreviewGrid_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;

            e.Handled = true;
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void checkbox_Checked(object sender, RoutedEventArgs e)
        {
            using var db = new wpfrazydbContext();
            if(checkbox.IsChecked == true)
            {
                db.UserApps.First(q => MainWindow.StatusOptions.User.Id == q.Id).ShowMainMenu_Dash = true;
                MainWindow.StatusOptions.User.ShowMainMenu_Dash = true;
                db.SafeSaveChanges();
            }
            else
            {
                db.UserApps.First(q => MainWindow.StatusOptions.User.Id == q.Id).ShowMainMenu_Dash = false;
                MainWindow.StatusOptions.User.ShowMainMenu_Dash = false;
                db.SafeSaveChanges();
            }
        }

        private void cmbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using var db = new wpfrazydbContext();
            if (cmbAction.SelectedIndex == 0)
            {
                db.UserApps.First(q => MainWindow.StatusOptions.User.Id == q.Id).ribbonFirst_Dash = true;
                MainWindow.StatusOptions.User.ribbonFirst_Dash = true;
                MainWindow.Current.ribbon.Items.Remove(MainWindow.Current.rbnPreview);
                MainWindow.Current.ribbon.Items.Insert(0, MainWindow.Current.rbnPreview);
                MainWindow.Current.borderMiz.Margin = new Thickness(0, 0, 60, 0);
                db.SafeSaveChanges();
            }
            else if (cmbAction.SelectedIndex == 1)
            {
                db.UserApps.First(q => MainWindow.StatusOptions.User.Id == q.Id).ribbonFirst_Dash = false;
                MainWindow.StatusOptions.User.ribbonFirst_Dash = false;
                MainWindow.Current.ribbon.Items.Remove(MainWindow.Current.rbnPreview);
                MainWindow.Current.ribbon.Items.Add(MainWindow.Current.rbnPreview);
                Dispatcher.BeginInvoke(async ()=>
                { 
                    await Task.Delay(20);
                    var screenPoint = 630 - (SystemParameters.PrimaryScreenWidth - MainWindow.Current.rbnPreview.PointToScreen(new System.Windows.Point(0, 0)).X);

                    MainWindow.Current.borderMiz.Margin = new Thickness(0, 0, 632 - screenPoint, 0);
                });
                db.SafeSaveChanges();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindow.StatusOptions.User.ribbonFirst_Dash == null || !MainWindow.StatusOptions.User.ribbonFirst_Dash.Value)
            {
                cmbAction.SelectedIndex = 1;
            }
            else
                cmbAction.SelectedIndex = 0;

            if (MainWindow.StatusOptions.User.ShowMainMenu_Dash == null || MainWindow.StatusOptions.User.ShowMainMenu_Dash.Value)
            {
                checkbox.IsChecked = true;
            }
            else
                checkbox.IsChecked = false;
        }
    }
}

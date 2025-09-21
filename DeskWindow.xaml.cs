using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using WpfRaziLedgerApp.RazyDb;
using WpfRaziLedgerApp;
using Syncfusion.Windows.Tools.Controls;
using System.Threading.Tasks;
using System.Collections;
namespace WpfRaziLedgerApp
{
    public partial class DeskWindow : Window
    {
        //public static Border CurrentTab;
        //public static List<Border> OpenTabs=new List<Border>();

        private Guid currentUserId;
        // mapping loaded icons on desk: key is UI element (Image)
        private Dictionary<Image, Guid> imageToRibbonItem = new();

        // a dictionary to map FkribbonItemId -> RibbonButton control in main ribbon (you must fill this)
        private Dictionary<Guid, RibbonButton> ribbonButtonByItemId;

        public DeskWindow(Guid FkuserId, Dictionary<Guid, RibbonButton> ribbonMap)
        {
            InitializeComponent();
            Closed += DeskWindow_Closed;
            currentUserId = FkuserId;
            ribbonButtonByItemId = ribbonMap;
            Loaded += DeskWindow_Loaded;
            gridtop.Opacity = 0;
        }

        private void DeskWindow_Closed(object? sender, EventArgs e)
        {
            ribbonButtonByItemId.Clear();
            imageToRibbonItem.Clear();
        }

        private void DeskWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDeskItems();
            StartOpenAnimation();
            if (MainWindow.StatusOptions.User.ShowMainMenu_Dash == null || MainWindow.StatusOptions.User.ShowMainMenu_Dash.Value)
            {

            }
            else
                gridtop.Visibility  = Visibility.Hidden;
        }

        private void StartOpenAnimation()
        {
            var sb = (Storyboard)FindResource("OpenDeskStoryboard");
            sb.Completed -= OpenSb_Completed;
            sb.Completed += OpenSb_Completed;
            sb.Begin();
        }

        private void OpenSb_Completed(object? sender, EventArgs e)
        {
            // بعد از کامل شدن انیمیشن، چیزی لازم نیست
        }

        private void StartCloseAnimationAndClose()
        {
            closed = true;
            var sb = (Storyboard)FindResource("CloseDeskStoryboard");
            sb.Completed -= CloseSb_Completed;
            sb.Completed += CloseSb_Completed;
            sb.Begin();
        }

        private void CloseSb_Completed(object? sender, EventArgs e)
        {
            Close();
        }

        private void LoadDeskItems(string? category=null)
        {
            DeskGrid.Children.Clear();

            using var db = new wpfrazydbContext();
            List<UserDesktopItem> items = null;
            List<RibbonItem> ribbonItems = null;
            if (category == null)
            {
                items = db.UserDesktopItems.Where(u => u.FkuserId == currentUserId).ToList();
                foreach (var item in items)
                {
                    item.Id = db.RibbonItemMains.FirstOrDefault(r => item.FKRibbonItemMainId == r.Id).Id;
                }
            }
            else
                items = db.RibbonItems
    .Where(u => u.Category.Contains(category))
    .ToList()
    .Select((q, indexA) => new UserDesktopItem()
    {
        Id = q.fkRbMainId,
        FkuserId = currentUserId,
        FKRibbonItemMainId = q.Id,
        RowIndex = (byte)(indexA / 5), // چون 5 ستون دارید
        ColIndex = (byte)(indexA % 5)  // ستون باقیمانده تقسیم
    })
    .ToList();
            //if (category == "انبار")
            //    for (int index = 0; index < items.Count; index++)
            //    {
            //        // مکان جدید با یک خانه عقب
            //        int newIndex = index - 1;

            //        if (newIndex < 0)
            //            newIndex = items.Count - 1; // اگر اولین بود، بره آخر جدول

            //        items[index].RowIndex = (byte)(newIndex / 5);
            //        items[index].ColIndex = (byte)(newIndex % 5);
            //    }
           
            foreach (var item in items)
            {
                if (ribbonButtonByItemId.TryGetValue(item.Id, out var rb))
                {
                    //if (rb.Label == "گروه قیمت")
                    //    continue;
                    // ساخت StackPanel (آیکون + متن)
                    var panel = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    // آیکون
                    var img = new Image
                    {
                        Width = 48,
                        Height = 48,
                        Stretch = Stretch.Uniform
                    };

                    img.Source = rb.LargeIcon;

                    // متن زیر آیکون
                    var text = new TextBlock
                    {
                        Text = rb.Label,
                        Margin = new Thickness(0, 10, 0, 10),
                        Foreground = Brushes.White,
                        FontSize = 15,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        TextAlignment = TextAlignment.Center,
                        TextWrapping = TextWrapping.Wrap
                    };

                    // اضافه کردن آیکون و متن به پنل
                    panel.Children.Add(img);
                    panel.Children.Add(text);

                    var border = new Border
                    {
                        Cursor = Cursors.Hand,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        //Margin = new Thickness(5),
                        Background = Brushes.Transparent,
                        Child = new StackPanel
                        {
                            Orientation = Orientation.Vertical,
                            Children =
                                    {
                                        panel
                                    }
                        }
                    };

                    // وقتی کاربر روی پنل کلیک کرد
                    border.MouseLeftButtonUp += (s, e) =>
                    {
                        /*CurrentTab = s as Border;
                        if (!OpenTabs.Contains(CurrentTab))
                            OpenTabs.Add(CurrentTab);*/
                        // همون رفتار RibbonButton اصلی رو صدا بزن
                        Dispatcher.BeginInvoke(new Action(() =>
                        rb.RaiseEvent(new RoutedEventArgs(RibbonButton.ClickEvent, rb))));
                        
                        // بستن DeskWindow
                        this.Close();
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
                            Color = Colors.Khaki,   // می‌تونی Colors.White هم بذاری
                            Opacity = 1.0
                        }));
                    style.Triggers.Add(hoverTrigger);

                    border.Style = style;


                    // اضافه به Grid
                    Grid.SetRow(border, item.RowIndex);
                    Grid.SetColumn(border, item.ColIndex);

                    DeskGrid.Children.Add(border);
                    border.Tag = new Dictionary<RibbonButton, UserControl>() { { rb, null } };
                }
            }
        }


        private Image CreateDeskIcon(string iconPath, string label)
        {
            var img = new Image
            {
                Width = 48,
                Height = 48,
                Stretch = System.Windows.Media.Stretch.Uniform
            };

            try
            {
                // assume iconPath is relative like "/Images/..."; create uri
                var uri = new Uri(iconPath, UriKind.RelativeOrAbsolute);
                var bmp = new BitmapImage(uri);
                img.Source = bmp;
            }
            catch
            {
                // fallback
            }

            // clickable
            img.MouseLeftButtonUp += Img_MouseLeftButtonUp;
            // allow tooltip
            ToolTipService.SetToolTip(img, label);
            return img;
        }

        private void Img_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image img && imageToRibbonItem.TryGetValue(img, out var FkribbonItemId))
            {
                // find the RibbonButton and raise its click
                if (ribbonButtonByItemId.TryGetValue(FkribbonItemId, out var rb))
                {
                    rb.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                }
                this.Close();
            }
        }

        // place centered in grid cell
        private void PlaceImageInCell(Image img, int row, int col)
        {
            // create container to center image
            var panel = new Grid();
            panel.SetValue(Grid.RowProperty, row);
            panel.SetValue(Grid.ColumnProperty, col);
            panel.HorizontalAlignment = HorizontalAlignment.Center;
            panel.VerticalAlignment = VerticalAlignment.Center;

            panel.Children.Add(img);
            DeskGrid.Children.Add(panel);

            // enable dragging from desk (optional: to remove or move)
            img.MouseMove += Img_MouseMove;
            img.MouseLeftButtonDown += Img_MouseLeftButtonDown;
            img.MouseLeftButtonUp += Img_MouseLeftButtonUpForDrag;
        }

        Point dragStart;
        bool isDragging = false;
        Image draggingImage;
        private void Img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragStart = e.GetPosition(this);
            draggingImage = sender as Image;
        }

        private void Img_MouseLeftButtonUpForDrag(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                // drop to nearest cell
                var pos = e.GetPosition(DeskBorder);
                var cell = GetNearestCellFromPoint(pos);
                if (cell != null && draggingImage != null && imageToRibbonItem.TryGetValue(draggingImage, out var ribbonId))
                {
                    // move image to new cell
                    RemoveImageFromGrid(draggingImage);
                    PlaceImageInCell(draggingImage, cell.Value.row, cell.Value.col);
                    SaveDeskItemPosition(ribbonId, (byte)cell.Value.row, (byte)cell.Value.col);
                }
            }
        }

        private void Img_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggingImage == null) return;
            if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
            {
                var p = e.GetPosition(this);
                if ((p - dragStart).Length > 4)
                {
                    isDragging = true;
                    DragDrop.DoDragDrop(draggingImage, imageToRibbonItem[draggingImage].ToString(), DragDropEffects.Move);
                }
            }
        }

        // helper: remove image's parent panel from DeskGrid
        private void RemoveImageFromGrid(Image img)
        {
            var parent = img.Parent as Grid;
            if (parent != null)
            {
                DeskGrid.Children.Remove(parent);
            }
        }

        private (int row, int col)? GetNearestCellFromPoint(Point p)
        {
            // DeskGrid actual size:
            var totalW = DeskGrid.ActualWidth;
            var totalH = DeskGrid.ActualHeight;
            if (totalW == 0 || totalH == 0) return null;
            double cellW = totalW / 5.0;
            double cellH = totalH / 3.0;
            int col = Math.Min(4, Math.Max(0, (int)(p.X / cellW)));
            int row = Math.Min(2, Math.Max(0, (int)(p.Y / cellH)));
            return (row, col);
        }

        private void SaveDeskItemPosition(Guid FkribbonItemId, byte row, byte col)
        {
            //using var db = new wpfrazydbContext();
            //var item = db.UserDesktopItems.FirstOrDefault(u => u.FkuserId == currentUserId && u.FkribbonItemId == FkribbonItemId);
            //if (item == null)
            //{
            //    item = new UserDesktopItem { Id = Guid.NewGuid(), FkuserId = currentUserId, FkribbonItemId = FkribbonItemId, RowIndex = row, ColIndex = col };
            //    db.UserDesktopItems.Add(item);
            //}
            //else
            //{
            //    item.RowIndex = row; item.ColIndex = col;
            //}
            //db.SaveChanges();
        }
        bool closed = false;
        // clicking outside closes:
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (closed)
                return;
            var pos = e.GetPosition(DeskBorder);
            if (pos.X < 0 || pos.Y < 0 || pos.X > DeskBorder.ActualWidth || pos.Y > DeskBorder.ActualHeight)
            {
                if ((rect2.Tag as StackPanel)?.IsMouseOver == true||imageMiz.IsMouseOver||imageMizSetting.IsMouseOver)
                    return;
                pos = e.GetPosition(rect);
                if ((!sd&& rect.Visibility== Visibility.Hidden) ||(pos.X < 0 || pos.Y < 0 || pos.X > rect.ActualWidth || pos.Y > rect.ActualHeight))
                    StartCloseAnimationAndClose();
            }
        }

        // also on ESC
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (closed)
                return;
            if (rect2.Visibility==Visibility && e.Key == Key.Left)
            {
                StackPanel stack = null;
                try
                {
                    stack = gridtop.Children[Grid.GetColumn(rect2) + 3] as StackPanel;
                }
                catch
                { }
                if (stack != null)
                    SelectStack(stack);
            }
            else if (rect2.Visibility == Visibility && e.Key == Key.Right)
            {
                StackPanel stack = null;
                try
                {
                    stack = gridtop.Children[Grid.GetColumn(rect2) + 1] as StackPanel;
                }
                catch
                { }
                if (stack != null)
                    SelectStack(stack);
            }
            else if (e.Key == Key.Escape) 
                StartCloseAnimationAndClose();
        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            var stack = sender as StackPanel;
            if (rect2.Visibility == Visibility.Visible && Grid.GetColumn(stack) == Grid.GetColumn(rect2))
                return;
            rect.Visibility = Visibility.Visible;
            Grid.SetColumn(rect, Grid.GetColumn(stack));
        }

        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            var stack = sender as StackPanel;
            rect.Visibility = Visibility.Hidden;
        }
        bool sd = false;
        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var stack = sender as StackPanel;
            sd = true;
            SelectStack(stack);
            Dispatcher.Invoke(async () =>
            {
                await Task.Delay(20);
                sd = false;
                rect.Visibility = Visibility.Hidden;
            });
        }

        private void SelectStack(StackPanel? stack)
        {
            imageMiz.Visibility = Visibility.Visible;
            rect2.Visibility = Visibility.Visible;
            rect2.Tag = stack;
            Grid.SetColumn(rect2, Grid.GetColumn(stack));
            LoadDeskItems((stack.Children[1] as TextBlock).Text);
        }

        private void imageMizSetting_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed&&!closed) 
            {
                closed = true;
                StartCloseAnimationAndClose();
                Dispatcher.BeginInvoke(new Action(async () =>
                {
                    await Task.Delay(250);
                    var rb = MainWindow.Current.keyValuePairs.First(q => q.Key.Label == "میز کار").Key;
                    rb.RaiseEvent(new RoutedEventArgs(RibbonButton.ClickEvent, rb));
                }));
            }
        }

        private void imageMiz_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Dispatcher.Invoke(async () =>
                {
                    await Task.Delay(250);
                    imageMiz.Visibility = Visibility.Collapsed;
                    rect2.Visibility = Visibility.Hidden;
                    LoadDeskItems();
                });
            }
        }
    }
}
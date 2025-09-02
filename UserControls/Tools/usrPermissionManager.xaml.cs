using Syncfusion.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace WpfRaziLedgerApp
{
    /// <summary>
    /// Interaction logic for usrPermissionManager.xaml
    /// </summary>
    public partial class usrPermissionManager : UserControl,ITabForm
    {
        private Guid selectedGroupId;

        public usrPermissionManager()
        {
            InitializeComponent();
            LoadGroups();
        }

        private void LoadGroups()
        {
            using var db = new wpfrazydbContext();
            var groups = db.UserGroups.ToList();
            cmbUserGroup.ItemsSource = groups;
        }

        private void cmbUserGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbUserGroup.SelectedItem is not UserGroup group) return;

            selectedGroupId = group.Id;
            LoadTreePermissions();
        }

        private void LoadTreePermissions()
        {
            using var db = new wpfrazydbContext();
            var ribbonItems = db.RibbonItems.ToList();
            var existingPermissions = db.Permissions
                                        .Where(p => p.FkUserGroupId == selectedGroupId)
                                        .ToList();

            // ساخت درخت
            var categories = ribbonItems
    .GroupBy(x => x.Category)
    .Select(g =>
    {
        var children = g.Select(item =>
        {
            var permission = existingPermissions.FirstOrDefault(p => p.FkRibbonItemId == item.Id);
            return new RibbonPermissionNode
            {
                RibbonItemId = item.Id,
                Name = item.DisplayName,
                Category = item.Category,
                CanAccess = permission != null,
                CanDelete = permission?.CanDelete ?? false,
                CanModify = permission?.CanModify ?? false,
                CanInsert = permission?.CanInsert ?? false,
            };
        }).ToList();

        // اتصال پدر به فرزندان
        var parent = new RibbonPermissionNode
        {
            Name = g.Key,
            Category = g.Key,
            CanAccess = null,
            Children = new ObservableCollection<RibbonPermissionNode>(children)
        };

        foreach (var child in children)
        {
            child.Parent = parent;
            child.IsPermissionIconsVisible = false;
        }

        // بررسی وضعیت اولیه
        parent.EvaluateCanAccessFromChildren();
        
        return parent;
    }).ToList();

            treePermissions.ItemsSource = categories;
        }

        private List<RibbonPermissionNode> GetAllNodes(ObservableCollection<RibbonPermissionNode> nodes)
        {
            var list = new List<RibbonPermissionNode>();
            foreach (var node in nodes)
            {
                list.Add(node);
                list.AddRange(GetAllNodes(node.Children));
            }
            return list;
        }

        bool forceClose = false;
        bool isCancel = true;
        public bool CloseForm()
        {
            if (!isCancel && Xceed.Wpf.Toolkit.MessageBox.Show("آیا می خواهید از این فرم خارج شوید؟", "خروج", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return false;
            }
            forceClose = true;
            var list = MainWindow.Current.GetTabControlItems;
            var item = list.FirstOrDefault(y => y.Tag?.ToString() == "سطح دسترسی");
            MainWindow.Current.tabcontrol.Items.Remove(item);
            return true;
        }

        public void SetNull()
        {
            
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGroupId == Guid.Empty)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("لطفاً یک گروه کاربری انتخاب کنید.","سطح دسترسی", MessageBoxButton.OK,MessageBoxImage.Warning);
                return;
            }

            using var db = new wpfrazydbContext();

            var allNodes = GetAllNodes(treePermissions.ItemsSource.ToList<RibbonPermissionNode>().ToObservableCollection());

            // پاک‌سازی دسترسی‌های قبلی
            var oldPermissions = db.Permissions.Where(p => p.FkUserGroupId == selectedGroupId);
            db.Permissions.RemoveRange(oldPermissions);
            db.SafeSaveChanges();

            // ایجاد رکوردهای جدید
            foreach (var node in allNodes.Where(n => n.CanAccess == true))
            {
                if (node.RibbonItemId != null)
                {
                    var permission = new Permission
                    {
                        Id = Guid.NewGuid(),
                        CanAccess = true,
                        FkUserGroupId = selectedGroupId,
                        CanInsert=node.CanInsert,
                        CanDelete=node.CanDelete,
                        CanModify=node.CanModify,
                        FkRibbonItemId = node.RibbonItemId ?? Guid.Empty
                    };
                    db.Permissions.Add(permission);
                }
            }
            if (db.SafeSaveChanges())
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("سطح دسترسی با موفقیت ذخیره شد.");
                isCancel = true;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            isCancel = false;
        }
        private void InsertIcon_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.DataContext is RibbonPermissionNode node)
                node.CanInsert = !node.CanInsert;
        }

        private void DeleteIcon_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.DataContext is RibbonPermissionNode node)
                node.CanDelete = !node.CanDelete;
        }

        private void ModifyIcon_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.DataContext is RibbonPermissionNode node)
                node.CanModify = !node.CanModify;
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            isCancel = false;
            return;
            if (sender is CheckBox checkBox && checkBox.IsChecked == false && checkBox.DataContext is RibbonPermissionNode permissionNode)// && permissionNode.Children.Any(d => d.CanAccess == true))
            {
                permissionNode.Children.ForEach(y=>y.CanAccess = false);
            }
        }
    }
}

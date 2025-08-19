using Syncfusion.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace WpfRaziLedgerApp
{
    public class RibbonPermissionNode : INotifyPropertyChanged
    {
        public Guid? RibbonItemId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }

        private bool? _canAccess;
        public bool? CanAccess
        {
            get => _canAccess;
            set
            {
                if (_canAccess != value)
                {
                    _canAccess = value;
                    OnPropertyChanged(nameof(CanAccess));
                    if (Parent == null)
                    {                        
                        _IsPermissionIconsVisible = true;
                        if (value == true)
                        {
                            Children.ForEach(c =>
                            {
                                c._canAccess = true;
                                if(!Enter)
                                c.CanInsert = c.CanDelete = c.CanModify = true;
                    c.OnPropertyChanged(nameof(CanAccess));
                    c.OnPropertyChanged(nameof(IsPermissionIconsVisible));
                                });
                        }
                        else if(!Enter)
                        {
                            Children.ForEach(c =>
                            {
                                c._canAccess = false;
                    c.OnPropertyChanged(nameof(CanAccess));
                    c.OnPropertyChanged(nameof(IsPermissionIconsVisible));
                            });
                        }
                    }
                    else
                        _IsPermissionIconsVisible = false;
                    OnPropertyChanged(nameof(IsPermissionIconsVisible));
                    if (value == true)
                    {
                        CanInsert = true;
                        CanDelete = true;
                        CanModify = true;
                        _canAccess = true;
                    }
                    else
                    {
                        CanInsert = false;
                        CanDelete = false;
                        CanModify = false;
                        _canAccess = false;
                    }
                    Enter = false;
                    Parent?.EvaluateCanAccessFromChildren();                    
                }
            }
        }
        bool Enter = false;
        private bool _canInsert;
        public bool CanInsert
        {
            get => _canInsert;
            set { _canInsert = value; OnPropertyChanged(nameof(CanInsert)); }
        }

        private bool _canDelete;
        public bool CanDelete
        {
            get => _canDelete;
            set { _canDelete = value; OnPropertyChanged(nameof(CanDelete)); }
        }

        private bool _canModify;
        public bool CanModify
        {
            get => _canModify;
            set { _canModify = value; OnPropertyChanged(nameof(CanModify)); }
        }
        public RibbonPermissionNode Parent { get; set; }

        public void EvaluateCanAccessFromChildren()
        {
            if (Children.Count == 0) return;
            Enter = true;
            var allChecked = Children.All(c => c.CanAccess == true);
            var noneChecked = Children.All(c => c.CanAccess == false);

            if (allChecked)
                CanAccess = true;
            else if (noneChecked)
            {
                CanAccess = false;
                OnPropertyChanged(nameof(CanAccess));
            }
            else
                CanAccess = null;
        }
        public bool _IsPermissionIconsVisible;
        public bool IsPermissionIconsVisible
        {
            get
            {
                if (_IsPermissionIconsVisible == true)
                    return false;
                return CanAccess == true;
            }
            set
            {
                _IsPermissionIconsVisible = value;
                OnPropertyChanged(nameof(IsPermissionIconsVisible));
            }
        }

        public ObservableCollection<RibbonPermissionNode> Children { get; set; } = new();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

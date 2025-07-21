using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

                    if (value.HasValue)
                    {
                        foreach (var child in Children)
                            child.CanAccess = value;
                    }

                    Parent?.EvaluateCanAccessFromChildren();
                }
            }
        }
        public RibbonPermissionNode Parent { get; set; }

        public void EvaluateCanAccessFromChildren()
        {
            if (Children.Count == 0) return;

            var allChecked = Children.All(c => c.CanAccess == true);
            var noneChecked = Children.All(c => c.CanAccess == false);

            if (allChecked)
                CanAccess = true;
            else if (noneChecked)
                CanAccess = false;
            else
                CanAccess = null;
        }

        public ObservableCollection<RibbonPermissionNode> Children { get; set; } = new();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

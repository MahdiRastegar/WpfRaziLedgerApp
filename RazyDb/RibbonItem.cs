using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class RibbonItem
    {
        public RibbonItem()
        {
            Permissions = new HashSet<Permission>();
        }

        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Category { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; }
    }
}

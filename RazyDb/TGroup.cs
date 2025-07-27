using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class TGroup
    {
        public TGroup()
        {
            Preferentials = new HashSet<Preferential>();
        }

        public Guid Id { get; set; }
        public int GroupCode { get; set; }
        public string GroupName { get; set; }
        public bool? PermissionView { get; set; }

        public virtual ICollection<Preferential> Preferentials { get; set; }
    }
}

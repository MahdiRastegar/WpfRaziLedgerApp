using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class GroupStorage
    {
        public GroupStorage()
        {
            Storages = new HashSet<Storage>();
        }

        public Guid Id { get; set; }
        public int GroupCode { get; set; }
        public string GroupName { get; set; }

        public virtual ICollection<Storage> Storages { get; set; }
    }
}

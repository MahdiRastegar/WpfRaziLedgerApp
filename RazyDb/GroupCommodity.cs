using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class GroupCommodity
    {
        public GroupCommodity()
        {
            Commodities = new HashSet<Commodity>();
        }

        public Guid Id { get; set; }
        public int GroupCode { get; set; }
        public string GroupName { get; set; }

        public virtual ICollection<Commodity> Commodities { get; set; }
    }
}

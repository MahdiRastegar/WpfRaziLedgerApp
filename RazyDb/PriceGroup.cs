using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class PriceGroup
    {
        public PriceGroup()
        {
            CommodityPricingPanels = new HashSet<CommodityPricingPanel>();
            CustomerGroups = new HashSet<CustomerGroup>();
        }

        public Guid Id { get; set; }
        public int GroupCode { get; set; }
        public string GroupName { get; set; }

        public virtual ICollection<CommodityPricingPanel> CommodityPricingPanels { get; set; }
        public virtual ICollection<CustomerGroup> CustomerGroups { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class CommodityPricingPanel
    {
        public Guid Id { get; set; }
        public Guid FkPriceGroupId { get; set; }
        public Guid FkCommodityId { get; set; }
        public DateTime Date { get; set; }
        public decimal? Fee { get; set; }
        public Guid? FkPeriodId { get; set; }

        public virtual Commodity FkCommodity { get; set; }
        public virtual Period FkPeriod { get; set; }
        public virtual PriceGroup FkPriceGroup { get; set; }
    }
}

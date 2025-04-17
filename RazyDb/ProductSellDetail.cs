using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class ProductSellDetail
    {
        public Guid Id { get; set; }
        public Guid FkCommodityId { get; set; }
        public decimal Value { get; set; }
        public Guid FkHeaderId { get; set; }
        public int Indexer { get; set; }
        public decimal Fee { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxPercent { get; set; }
        public bool? IsTax { get; set; }

        public virtual Commodity FkCommodity { get; set; }
        public virtual ProductSellHeader FkHeader { get; set; }
    }
}

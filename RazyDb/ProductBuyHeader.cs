using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class ProductBuyHeader
    {
        public ProductBuyHeader()
        {
            ProductBuyDetails = new HashSet<ProductBuyDetail>();
        }

        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public long Serial { get; set; }
        public long? OrderNumber { get; set; }
        public long InvoiceNumber { get; set; }
        public string WayBillNumber { get; set; }
        public string CarPlate { get; set; }
        public string CarType { get; set; }
        public Guid FkPreferentialId { get; set; }
        public decimal? InvoiceDiscount { get; set; }
        public decimal? ShippingCost { get; set; }
        public string Description { get; set; }
        public decimal SumDiscount { get; set; }

        public virtual Preferential FkPreferential { get; set; }
        public virtual ICollection<ProductBuyDetail> ProductBuyDetails { get; set; }
    }
}

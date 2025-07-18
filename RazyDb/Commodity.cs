using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class Commodity
    {
        public Commodity()
        {
            CommodityPricingPanels = new HashSet<CommodityPricingPanel>();
            NpstorageDetails = new HashSet<NpstorageDetail>();
            OrderDetails = new HashSet<OrderDetail>();
            PreInvoiceDetails = new HashSet<PreInvoiceDetail>();
            ProductBuyDetails = new HashSet<ProductBuyDetail>();
            ProductSellDetails = new HashSet<ProductSellDetail>();
            StorageReceiptDetails = new HashSet<StorageReceiptDetail>();
            StorageRotationDetails = new HashSet<StorageRotationDetail>();
            StorageTransferDetails = new HashSet<StorageTransferDetail>();
        }

        public Guid Id { get; set; }
        public int Code { get; set; }
        public Guid FkGroupId { get; set; }
        public string Name { get; set; }
        public Guid FkUnitId { get; set; }
        public bool? Taxable { get; set; }
        public Guid? FkPeriodId { get; set; }

        public virtual GroupCommodity FkGroup { get; set; }
        public virtual Period FkPeriod { get; set; }
        public virtual Unit FkUnit { get; set; }
        public virtual ICollection<CommodityPricingPanel> CommodityPricingPanels { get; set; }
        public virtual ICollection<NpstorageDetail> NpstorageDetails { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<PreInvoiceDetail> PreInvoiceDetails { get; set; }
        public virtual ICollection<ProductBuyDetail> ProductBuyDetails { get; set; }
        public virtual ICollection<ProductSellDetail> ProductSellDetails { get; set; }
        public virtual ICollection<StorageReceiptDetail> StorageReceiptDetails { get; set; }
        public virtual ICollection<StorageRotationDetail> StorageRotationDetails { get; set; }
        public virtual ICollection<StorageTransferDetail> StorageTransferDetails { get; set; }
    }
}

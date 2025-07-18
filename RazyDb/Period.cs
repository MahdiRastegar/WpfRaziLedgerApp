using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class Period
    {
        public Period()
        {
            AcDocumentHeaders = new HashSet<AcDocumentHeader>();
            CheckPaymentEvents = new HashSet<CheckPaymentEvent>();
            CheckRecieveEvents = new HashSet<CheckRecieveEvent>();
            Commodities = new HashSet<Commodity>();
            CommodityPricingPanels = new HashSet<CommodityPricingPanel>();
            OrderHeaders = new HashSet<OrderHeader>();
            PaymentMoneyHeaders = new HashSet<PaymentMoneyHeader>();
            PreInvoiceHeaders = new HashSet<PreInvoiceHeader>();
            ProductBuyHeaders = new HashSet<ProductBuyHeader>();
            ProductSellHeaders = new HashSet<ProductSellHeader>();
            RecieveMoneyHeaders = new HashSet<RecieveMoneyHeader>();
            StorageReceiptHeaders = new HashSet<StorageReceiptHeader>();
            StorageRotationHeaders = new HashSet<StorageRotationHeader>();
            StorageTransferHeaders = new HashSet<StorageTransferHeader>();
            Storages = new HashSet<Storage>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }

        public virtual ICollection<AcDocumentHeader> AcDocumentHeaders { get; set; }
        public virtual ICollection<CheckPaymentEvent> CheckPaymentEvents { get; set; }
        public virtual ICollection<CheckRecieveEvent> CheckRecieveEvents { get; set; }
        public virtual ICollection<Commodity> Commodities { get; set; }
        public virtual ICollection<CommodityPricingPanel> CommodityPricingPanels { get; set; }
        public virtual ICollection<OrderHeader> OrderHeaders { get; set; }
        public virtual ICollection<PaymentMoneyHeader> PaymentMoneyHeaders { get; set; }
        public virtual ICollection<PreInvoiceHeader> PreInvoiceHeaders { get; set; }
        public virtual ICollection<ProductBuyHeader> ProductBuyHeaders { get; set; }
        public virtual ICollection<ProductSellHeader> ProductSellHeaders { get; set; }
        public virtual ICollection<RecieveMoneyHeader> RecieveMoneyHeaders { get; set; }
        public virtual ICollection<StorageReceiptHeader> StorageReceiptHeaders { get; set; }
        public virtual ICollection<StorageRotationHeader> StorageRotationHeaders { get; set; }
        public virtual ICollection<StorageTransferHeader> StorageTransferHeaders { get; set; }
        public virtual ICollection<Storage> Storages { get; set; }
    }
}

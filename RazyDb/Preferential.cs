using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class Preferential
    {
        public Preferential()
        {
            AcDocumentDetails = new HashSet<AcDocumentDetail>();
            CheckPaymentEvents = new HashSet<CheckPaymentEvent>();
            CheckRecieveEvents = new HashSet<CheckRecieveEvent>();
            MoneyType666s = new HashSet<MoneyType666>();
            OrderHeaders = new HashSet<OrderHeader>();
            PaymentMoneyDetails = new HashSet<PaymentMoneyDetail>();
            PaymentMoneyHeaders = new HashSet<PaymentMoneyHeader>();
            PreInvoiceHeaders = new HashSet<PreInvoiceHeader>();
            ProductBuyHeaders = new HashSet<ProductBuyHeader>();
            ProductSellHeaderFkPreferentialIdDriverNavigations = new HashSet<ProductSellHeader>();
            ProductSellHeaderFkPreferentialIdFreightNavigations = new HashSet<ProductSellHeader>();
            ProductSellHeaderFkPreferentialIdPersonnelNavigations = new HashSet<ProductSellHeader>();
            ProductSellHeaderFkPreferentialIdReceiverNavigations = new HashSet<ProductSellHeader>();
            ProductSellHeaderFkPreferentials = new HashSet<ProductSellHeader>();
            RecieveMoneyDetails = new HashSet<RecieveMoneyDetail>();
            RecieveMoneyHeaders = new HashSet<RecieveMoneyHeader>();
        }

        public Guid Id { get; set; }
        public int PreferentialCode { get; set; }
        public Guid FkGroupId { get; set; }
        public string PreferentialName { get; set; }
        public string Mobile { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Phone3 { get; set; }
        public string WebSite { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public Guid? FkCityId { get; set; }

        public virtual City FkCity { get; set; }
        public virtual TGroup FkGroup { get; set; }
        public virtual ICollection<AcDocumentDetail> AcDocumentDetails { get; set; }
        public virtual ICollection<CheckPaymentEvent> CheckPaymentEvents { get; set; }
        public virtual ICollection<CheckRecieveEvent> CheckRecieveEvents { get; set; }
        public virtual ICollection<MoneyType666> MoneyType666s { get; set; }
        public virtual ICollection<OrderHeader> OrderHeaders { get; set; }
        public virtual ICollection<PaymentMoneyDetail> PaymentMoneyDetails { get; set; }
        public virtual ICollection<PaymentMoneyHeader> PaymentMoneyHeaders { get; set; }
        public virtual ICollection<PreInvoiceHeader> PreInvoiceHeaders { get; set; }
        public virtual ICollection<ProductBuyHeader> ProductBuyHeaders { get; set; }
        public virtual ICollection<ProductSellHeader> ProductSellHeaderFkPreferentialIdDriverNavigations { get; set; }
        public virtual ICollection<ProductSellHeader> ProductSellHeaderFkPreferentialIdFreightNavigations { get; set; }
        public virtual ICollection<ProductSellHeader> ProductSellHeaderFkPreferentialIdPersonnelNavigations { get; set; }
        public virtual ICollection<ProductSellHeader> ProductSellHeaderFkPreferentialIdReceiverNavigations { get; set; }
        public virtual ICollection<ProductSellHeader> ProductSellHeaderFkPreferentials { get; set; }
        public virtual ICollection<RecieveMoneyDetail> RecieveMoneyDetails { get; set; }
        public virtual ICollection<RecieveMoneyHeader> RecieveMoneyHeaders { get; set; }
    }
}

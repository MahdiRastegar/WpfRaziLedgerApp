using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class Moein
    {
        public Moein()
        {
            AcDocumentDetails = new HashSet<AcDocumentDetail>();
            CheckPaymentEvents = new HashSet<CheckPaymentEvent>();
            CheckRecieveEvents = new HashSet<CheckRecieveEvent>();
            MoneyType666s = new HashSet<MoneyType666>();
            PaymentMoneyDetails = new HashSet<PaymentMoneyDetail>();
            PaymentMoneyHeaders = new HashSet<PaymentMoneyHeader>();
            RecieveMoneyDetails = new HashSet<RecieveMoneyDetail>();
            RecieveMoneyHeaders = new HashSet<RecieveMoneyHeader>();
        }

        public Guid Id { get; set; }
        public int MoeinCode { get; set; }
        public Guid FkColId { get; set; }
        public string MoeinName { get; set; }

        public virtual Col FkCol { get; set; }
        public virtual ICollection<AcDocumentDetail> AcDocumentDetails { get; set; }
        public virtual ICollection<CheckPaymentEvent> CheckPaymentEvents { get; set; }
        public virtual ICollection<CheckRecieveEvent> CheckRecieveEvents { get; set; }
        public virtual ICollection<MoneyType666> MoneyType666s { get; set; }
        public virtual ICollection<PaymentMoneyDetail> PaymentMoneyDetails { get; set; }
        public virtual ICollection<PaymentMoneyHeader> PaymentMoneyHeaders { get; set; }
        public virtual ICollection<RecieveMoneyDetail> RecieveMoneyDetails { get; set; }
        public virtual ICollection<RecieveMoneyHeader> RecieveMoneyHeaders { get; set; }
    }
}

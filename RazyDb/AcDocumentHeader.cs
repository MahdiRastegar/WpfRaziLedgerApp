using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class AcDocumentHeader
    {
        public AcDocumentHeader()
        {
            AcDocumentDetails = new HashSet<AcDocumentDetail>();
            CheckPaymentEvents = new HashSet<CheckPaymentEvent>();
            CheckRecieveEvents = new HashSet<CheckRecieveEvent>();
            PaymentMoneyHeaders = new HashSet<PaymentMoneyHeader>();
            RecieveMoneyHeaders = new HashSet<RecieveMoneyHeader>();
        }

        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public long Serial { get; set; }
        public long NoDoument { get; set; }
        public Guid FkDocumentTypeId { get; set; }

        public virtual DocumentType FkDocumentType { get; set; }
        public virtual ICollection<AcDocumentDetail> AcDocumentDetails { get; set; }
        public virtual ICollection<CheckPaymentEvent> CheckPaymentEvents { get; set; }
        public virtual ICollection<CheckRecieveEvent> CheckRecieveEvents { get; set; }
        public virtual ICollection<PaymentMoneyHeader> PaymentMoneyHeaders { get; set; }
        public virtual ICollection<RecieveMoneyHeader> RecieveMoneyHeaders { get; set; }
    }
}

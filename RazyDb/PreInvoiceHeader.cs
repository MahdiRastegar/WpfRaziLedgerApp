using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class PreInvoiceHeader
    {
        public PreInvoiceHeader()
        {
            PreInvoiceDetails = new HashSet<PreInvoiceDetail>();
        }

        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public long Serial { get; set; }
        public Guid FkPreferentialId { get; set; }
        public decimal? InvoiceDiscount { get; set; }
        public string Description { get; set; }
        public decimal SumDiscount { get; set; }
        public Guid? FkPeriodId { get; set; }

        public virtual Period FkPeriod { get; set; }
        public virtual Preferential FkPreferential { get; set; }
        public virtual ICollection<PreInvoiceDetail> PreInvoiceDetails { get; set; }
    }
}

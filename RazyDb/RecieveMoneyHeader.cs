using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class RecieveMoneyHeader
    {
        public RecieveMoneyHeader()
        {
            RecieveMoneyDetails = new HashSet<RecieveMoneyDetail>();
        }

        public Guid Id { get; set; }
        public int ReceiptNumber { get; set; }
        public DateTime Date { get; set; }
        public Guid? FkPreferentialId { get; set; }
        public Guid? FkMoeinId { get; set; }
        public string Description { get; set; }

        public virtual Moein FkMoein { get; set; }
        public virtual Preferential FkPreferential { get; set; }
        public virtual ICollection<RecieveMoneyDetail> RecieveMoneyDetails { get; set; }
    }
}

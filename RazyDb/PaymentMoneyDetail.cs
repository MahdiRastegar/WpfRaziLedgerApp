using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class PaymentMoneyDetail
    {
        public PaymentMoneyDetail()
        {
            CheckPaymentEvents = new HashSet<CheckPaymentEvent>();
        }

        public int Indexer { get; set; }
        public Guid Id { get; set; }
        public Guid FkHeaderId { get; set; }
        public decimal Price { get; set; }
        public string BranchName { get; set; }
        public string Number { get; set; }
        public Guid? FkBank { get; set; }
        public DateTime? Date { get; set; }
        public byte MoneyType { get; set; }
        public Guid FkPreferentialId { get; set; }
        public Guid FkMoeinId { get; set; }
        public string SayadiNumber { get; set; }
        public bool? Registered { get; set; }

        public virtual Bank FkBankNavigation { get; set; }
        public virtual PaymentMoneyHeader FkHeader { get; set; }
        public virtual Moein FkMoein { get; set; }
        public virtual Preferential FkPreferential { get; set; }
        public virtual ICollection<CheckPaymentEvent> CheckPaymentEvents { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class Bank
    {
        public Bank()
        {
            PaymentMoneyDetails = new HashSet<PaymentMoneyDetail>();
            RecieveMoneyDetails = new HashSet<RecieveMoneyDetail>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<PaymentMoneyDetail> PaymentMoneyDetails { get; set; }
        public virtual ICollection<RecieveMoneyDetail> RecieveMoneyDetails { get; set; }
    }
}

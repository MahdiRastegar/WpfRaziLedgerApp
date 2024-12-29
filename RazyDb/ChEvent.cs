using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class ChEvent
    {
        public ChEvent()
        {
            CheckPaymentEvents = new HashSet<CheckPaymentEvent>();
            CheckRecieveEvents = new HashSet<CheckRecieveEvent>();
        }

        public Guid Id { get; set; }
        public byte ChEventCode { get; set; }
        public string Name { get; set; }
        public bool ForMoney { get; set; }
        public bool ForPayment { get; set; }

        public virtual ICollection<CheckPaymentEvent> CheckPaymentEvents { get; set; }
        public virtual ICollection<CheckRecieveEvent> CheckRecieveEvents { get; set; }
    }
}

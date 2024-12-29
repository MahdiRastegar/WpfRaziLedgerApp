using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class CheckRecieveEvent
    {
        public int Indexer { get; set; }
        public Guid Id { get; set; }
        public Guid FkDetaiId { get; set; }
        public Guid? FkAcId { get; set; }
        public Guid FkChEventId { get; set; }
        public DateTime EventDate { get; set; }
        public Guid FkPreferentialId { get; set; }
        public Guid FkMoeinId { get; set; }
        public string Description { get; set; }

        public virtual AcDocumentHeader FkAc { get; set; }
        public virtual ChEvent FkChEvent { get; set; }
        public virtual RecieveMoneyDetail FkDetai { get; set; }
        public virtual Moein FkMoein { get; set; }
        public virtual Preferential FkPreferential { get; set; }
    }
}

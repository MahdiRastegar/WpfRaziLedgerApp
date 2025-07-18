using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WpfRaziLedgerApp;
using WpfRaziLedgerApp;
using WpfRaziLedgerApp;
using WpfRaziLedgerApp;
using WpfRaziLedgerApp;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class AcDocumentDetail
    {
        public int Indexer { get; set; }
        public Guid Id { get; set; }
        public string Description { get; set; }
        public decimal? Debtor { get; set; }
        public decimal? Creditor { get; set; }
        public Guid FkAcDocHeaderId { get; set; }
        public Guid? FkPreferentialId { get; set; }
        public Guid? FkMoeinId { get; set; }
        [JsonIgnore]
        public virtual AcDocumentHeader FkAcDocHeader { get; set; }
        [JsonIgnore]
        public virtual Moein FkMoein { get; set; }
        [JsonIgnore]
        public virtual Preferential FkPreferential { get; set; }
    }
}

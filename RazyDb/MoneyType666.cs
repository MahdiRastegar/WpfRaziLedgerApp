using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class MoneyType666
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte Type { get; set; }
        public bool? RequiredNumber { get; set; }
        public bool? RequiredBank { get; set; }
        public bool? RequiredDate { get; set; }
        public Guid? FkPreferentialId { get; set; }
        public Guid? FkMoeinId { get; set; }

        public virtual Moein FkMoein { get; set; }
        public virtual Preferential FkPreferential { get; set; }
    }
}

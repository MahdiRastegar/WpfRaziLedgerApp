using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class Unit
    {
        public Unit()
        {
            Commodities = new HashSet<Commodity>();
        }

        public Guid Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Commodity> Commodities { get; set; }
    }
}

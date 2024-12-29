using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class City
    {
        public City()
        {
            Preferentials = new HashSet<Preferential>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid FkProvinceId { get; set; }

        public virtual Province FkProvince { get; set; }
        public virtual ICollection<Preferential> Preferentials { get; set; }
    }
}

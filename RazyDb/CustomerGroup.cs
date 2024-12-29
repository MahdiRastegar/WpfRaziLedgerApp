using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class CustomerGroup
    {
        public Guid Id { get; set; }
        public int CustomerGroupCode { get; set; }
        public Guid FkGroupId { get; set; }
        public string CustomerGroupName { get; set; }

        public virtual PriceGroup FkGroup { get; set; }
    }
}

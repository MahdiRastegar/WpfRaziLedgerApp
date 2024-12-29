using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class OrderHeader
    {
        public OrderHeader()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public long Serial { get; set; }
        public long NoDoument { get; set; }
        public Guid FkPreferentialId { get; set; }
        public string Description { get; set; }

        public virtual Preferential FkPreferential { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}

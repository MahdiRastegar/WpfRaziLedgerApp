using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class NpstorageHeader
    {
        public NpstorageHeader()
        {
            NpstorageDetails = new HashSet<NpstorageDetail>();
        }

        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public long Serial { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }

        public virtual ICollection<NpstorageDetail> NpstorageDetails { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class StorageRotationHeader
    {
        public StorageRotationHeader()
        {
            StorageRotationDetails = new HashSet<StorageRotationDetail>();
        }

        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public long Serial { get; set; }
        public string Description { get; set; }
        public Guid? FkPeriodId { get; set; }

        public virtual Period FkPeriod { get; set; }
        public virtual ICollection<StorageRotationDetail> StorageRotationDetails { get; set; }
    }
}

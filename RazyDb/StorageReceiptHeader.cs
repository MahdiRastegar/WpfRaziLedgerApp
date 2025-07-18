using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class StorageReceiptHeader
    {
        public StorageReceiptHeader()
        {
            StorageReceiptDetails = new HashSet<StorageReceiptDetail>();
        }

        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public long Serial { get; set; }
        public long NoDoument { get; set; }
        public Guid FkStorageId { get; set; }
        public Guid FkCodingReceiptTypesId { get; set; }
        public string Description { get; set; }
        public Guid? FkPeriodId { get; set; }

        public virtual CodingReceiptType FkCodingReceiptTypes { get; set; }
        public virtual Period FkPeriod { get; set; }
        public virtual Storage FkStorage { get; set; }
        public virtual ICollection<StorageReceiptDetail> StorageReceiptDetails { get; set; }
    }
}

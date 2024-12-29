using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class StorageTransferHeader
    {
        public StorageTransferHeader()
        {
            StorageTransferDetails = new HashSet<StorageTransferDetail>();
        }

        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public long Serial { get; set; }
        public long NoDoument { get; set; }
        public Guid FkStorageId { get; set; }
        public Guid FkCodingTypesTransferId { get; set; }
        public string Description { get; set; }

        public virtual CodingTypesTransfer FkCodingTypesTransfer { get; set; }
        public virtual Storage FkStorage { get; set; }
        public virtual ICollection<StorageTransferDetail> StorageTransferDetails { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class CodingTypesTransfer
    {
        public CodingTypesTransfer()
        {
            StorageTransferHeaders = new HashSet<StorageTransferHeader>();
        }

        public Guid Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public bool? IsDefault { get; set; }

        public virtual ICollection<StorageTransferHeader> StorageTransferHeaders { get; set; }
    }
}

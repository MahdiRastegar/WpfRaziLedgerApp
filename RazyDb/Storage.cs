using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class Storage
    {
        public Storage()
        {
            StorageReceiptHeaders = new HashSet<StorageReceiptHeader>();
            StorageTransferHeaders = new HashSet<StorageTransferHeader>();
        }

        public Guid Id { get; set; }
        public int StorageCode { get; set; }
        public Guid FkGroupId { get; set; }
        public string StorageName { get; set; }

        public virtual GroupStorage FkGroup { get; set; }
        public virtual ICollection<StorageReceiptHeader> StorageReceiptHeaders { get; set; }
        public virtual ICollection<StorageTransferHeader> StorageTransferHeaders { get; set; }
    }
}

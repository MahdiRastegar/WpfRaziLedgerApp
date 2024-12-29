using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class CodingReceiptType
    {
        public CodingReceiptType()
        {
            StorageReceiptHeaders = new HashSet<StorageReceiptHeader>();
        }

        public Guid Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public bool? IsDefault { get; set; }

        public virtual ICollection<StorageReceiptHeader> StorageReceiptHeaders { get; set; }
    }
}

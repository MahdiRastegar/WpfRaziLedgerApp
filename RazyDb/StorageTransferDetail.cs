using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class StorageTransferDetail
    {
        public Guid Id { get; set; }
        public Guid FkCommodityId { get; set; }
        public decimal Value { get; set; }
        public Guid FkHeaderId { get; set; }
        public int Indexer { get; set; }

        public virtual Commodity FkCommodity { get; set; }
        public virtual StorageTransferHeader FkHeader { get; set; }
    }
}

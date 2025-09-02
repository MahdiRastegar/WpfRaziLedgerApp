using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp.RazyDb
{
    public partial class UserDesktopItem
    {
        public Guid Id { get; set; }
        public Guid FkuserId { get; set; }
        public Guid FkribbonItemId { get; set; }
        public byte RowIndex { get; set; }
        public byte ColIndex { get; set; }
    }
}

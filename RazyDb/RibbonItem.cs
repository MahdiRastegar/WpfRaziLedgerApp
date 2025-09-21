using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class RibbonItem
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Category { get; set; }
        public RibbonItemMain fkRbMain { get; set; }
        public Guid fkRbMainId { get; set; }
    }
}

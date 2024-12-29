using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class Agroup
    {
        public Agroup()
        {
            Cols = new HashSet<Col>();
        }

        public Guid Id { get; set; }
        public int GroupCode { get; set; }
        public string GroupName { get; set; }

        public virtual ICollection<Col> Cols { get; set; }
    }
}

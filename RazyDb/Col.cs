using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class Col
    {
        public Col()
        {
            Moeins = new HashSet<Moein>();
        }

        public Guid Id { get; set; }
        public int ColCode { get; set; }
        public string ColName { get; set; }
        public byte? Type { get; set; }
        public byte? Action { get; set; }
        public Guid FkGroupId { get; set; }

        public virtual Agroup FkGroup { get; set; }
        public virtual ICollection<Moein> Moeins { get; set; }
    }
}

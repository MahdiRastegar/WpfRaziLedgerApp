using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public class RibbonItemMain
    {
        public RibbonItemMain()
        {
            Permissions = new HashSet<Permission>();
        }

        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Category { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; }
        public virtual ICollection<RibbonItem> fkRbMains { get; set; }
    }
}

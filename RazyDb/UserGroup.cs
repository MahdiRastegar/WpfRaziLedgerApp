using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class UserGroup
    {
        public UserGroup()
        {
            Permissions = new HashSet<Permission>();
            UserApps = new HashSet<UserApp>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; }
        public virtual ICollection<UserApp> UserApps { get; set; }
    }
}

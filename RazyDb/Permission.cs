using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class Permission
    {
        public Guid Id { get; set; }
        public bool CanAccess { get; set; }
        public Guid FkRibbonItemId { get; set; }
        public Guid FkUserGroupId { get; set; }

        public virtual RibbonItem FkRibbonItem { get; set; }
        public virtual UserGroup FkUserGroup { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Security;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class Permission: IPermissionUser
    {
        public Guid Id { get; set; }
        public bool CanAccess { get; set; }
        //is of IPermissionUser
        public bool? CanInsert { get; set; }
        //is of IPermissionUser
        public bool? CanDelete { get; set; }
        //is of IPermissionUser
        public bool? CanModify { get; set; }
        public Guid FkRibbonItemId { get; set; }
        public Guid FkUserGroupId { get; set; }

        public virtual RibbonItemMain FkRibbonItem { get; set; }
        public virtual UserGroup FkUserGroup { get; set; }
    }
}

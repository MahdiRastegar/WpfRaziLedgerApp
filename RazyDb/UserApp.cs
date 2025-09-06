using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class UserApp
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Guid FkUserGroupId { get; set; }
        public bool? ribbonFirst_Dash { get; set; }
        public bool? ShowMainMenu_Dash { get; set; }

        public virtual UserGroup FkUserGroup { get; set; }
    }
}

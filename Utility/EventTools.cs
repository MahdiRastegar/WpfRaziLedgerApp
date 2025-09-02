using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfRaziLedgerApp.Utility
{
    public class EventTools : EventArgs
    {
        public UserControl UserName { get; set; }
        public string Value { get; set; }

        public EventTools(UserControl userName, string value)
        {
            UserName = userName;
            Value = value;
        }
    }
}

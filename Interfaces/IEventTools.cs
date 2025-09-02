using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfRaziLedgerApp.Utility;

namespace WpfRaziLedgerApp
{
    public interface IEventTools
    {
        event EventHandler<EventTools> MyEventh;
        EventTools eventTools { set; get;}
    }
}

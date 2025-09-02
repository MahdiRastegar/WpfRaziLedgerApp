using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public class EventNav : EventArgs
    {
        public EventNav(RibbonButton ribbonButton, string message) 
        {
            Message = message;
            MyRibbonButton = ribbonButton;
        }
        public RibbonButton MyRibbonButton{ get; set; }
        public string Message { get; set; }
    }
}

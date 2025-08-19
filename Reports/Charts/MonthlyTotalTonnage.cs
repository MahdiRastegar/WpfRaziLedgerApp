using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    //تناژ کل به تفکیک ماه
    public class MonthlyTotalTonnage
    {
        public int Index { get; set; }
        //ماه
        public string Month { get; set; }
        //تناژ کل در این ماه
        public decimal? Tonnage { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public class MonthlyTotalSale
    {
        public int Index { get; set; }
        //ماه
        public string Month { get; set; }
        //کل فروش کالاها در این ماه
        public decimal? Price { get; set; }
    }
}

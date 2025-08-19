using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp.Reports.Charts
{
    public class CityTotalTonnage
    {
        public int Index { get; set; }
        //شهر
        public string City { get; set; }
        //تناژ کل در این شهر
        public decimal? Tonnage { get; set; }
    }
}

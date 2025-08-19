using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public class CommodityTotalTonnage
    {
        public int Index { get; set; }
        //ماه
        public string Month { get; set; }
        //نام کالا
        public string CommodityName { get; set; }
        //تناژ کل در این ماه و برای این کالا
        public decimal? Tonnage { get; set; }
        public string NameMounth
        {
            get
            {
                var spaces = new string(' ', Math.Max(0, (Month.Length - CommodityName.Length) / 2));
                return $"{Month}\n{CommodityName}";
            }
        }
    }
}

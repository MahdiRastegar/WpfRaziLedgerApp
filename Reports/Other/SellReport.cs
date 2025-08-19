using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public class SellReport
    {
        public SellReport()
        {
            
        }

        public Guid Id { get; set; }
        //کد کالا
        public int Code { get; set; }
        //نام کالا
        public string Name { get; set; }
        //تعداد
        public decimal Count { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Month { get; set; }
        public decimal? Price { get; set; }
        public decimal? Tonnage { get; set; }   
    }
}

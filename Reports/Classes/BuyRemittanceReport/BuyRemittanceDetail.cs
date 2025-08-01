using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public class BuyRemittanceDetail
    {
        //آی دی کالا
        public Guid Id { get; set; }
        //کد کالا
        public int Code { get; set; }
        //نام کالا
        public string Name { get; set; }
        //تعداد خرید یا فروش
        public decimal Count { get; set; }
        //کد تفضیلی خریدار یا فروشنده
        public int PreferentialCode { get; set; }
        // خرید یا فروش
        public string SellOrBuy { get; set; }
        [JsonIgnore]
        public DateTime Date { get; set; }
    }
}

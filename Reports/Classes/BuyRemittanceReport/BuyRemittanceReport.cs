using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public class BuyRemittanceReport
    {
        public Guid Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public decimal BuySum { get; set; }
        public decimal SellSum { get; set; }
         public decimal RemainingCount
        {
            get
            {
                return BuySum - SellSum;
            }
        }
        [JsonIgnore]
        public ObservableCollection<BuyRemittanceDetail> buyRemittanceDetails { get; set; }
    }
}

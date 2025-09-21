using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public class ColElReport
    {      
        public int Row {  get; set; }
        public DateTime Date { get; set; }
        public string DateString
        {
            get
            {
                return Date.ToPersianDateString();
            }
        }
        public Guid Id { get; set; }
        public int ColCode { get; set; }
        public string ColName { get; set; }
        public decimal? SumDebtor { get; set; }
        public decimal? SumCreditor { get; set; }        
        [JsonIgnore]
        public Guid AgroupId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public class GAcClass
    {      
        public Guid Id { get; set; }        
        public int GroupCode { get; set; }
        public string GroupName { get; set; }
        public decimal? SumDebtor { get; set; }
        public decimal? SumCreditor { get; set; }
        public decimal? RemainingDebtor 
        {
            get
            {
                if (SumDebtor - SumCreditor > 0)
                    return SumDebtor - SumCreditor;
                return 0;
            }
        }
        public decimal? RemainingCreditor
        {
            get
            {
                if (SumCreditor - SumDebtor > 0)
                    return SumCreditor - SumDebtor;
                return 0;
            }
        }
    }
}

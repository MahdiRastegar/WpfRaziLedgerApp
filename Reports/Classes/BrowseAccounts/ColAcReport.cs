using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public class ColAcReport
    {      
        public Guid Id { get; set; }
        public int ColCode { get; set; }
        public string ColName { get; set; }
        public decimal? BeforeSum { get; set; }
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
        public decimal? BeforeDebtor
        {
            get
            {
                if (BeforeSum > 0)
                    return BeforeSum;
                return 0;
            }
        }
        public decimal? BeforeCreditor
        {
            get
            {
                if (BeforeSum < 0)
                    return -BeforeSum;
                return 0;
            }
        }
    }
}

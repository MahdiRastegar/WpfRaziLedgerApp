using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public partial class AcDocumentHeader
    {
        public void RefreshSumColumns()
        {
            _SumCreditor = _SumDebtor = null;
        }
        Nullable<decimal> _SumDebtor;
        [NotMapped]
        public Nullable<decimal> SumDebtor
        {
            get
            {
                if (_SumDebtor == null)
                    _SumDebtor = AcDocumentDetails.Sum(x => x.Debtor);
                return _SumDebtor;
            }
            set
            {
                _SumDebtor = value;
            }
        }
        Nullable<decimal> _SumCreditor;
        [NotMapped]
        public Nullable<decimal> SumCreditor
        {
            get
            {
                if (_SumCreditor == null)
                    _SumCreditor = AcDocumentDetails.Sum(x => x.Creditor);
                return _SumCreditor;
            }
            set
            {
                _SumCreditor = value;
            }
        }
        public Nullable<decimal> Difference
        {
            get
            {
                return SumDebtor - SumCreditor;
            }
        }
    }
}

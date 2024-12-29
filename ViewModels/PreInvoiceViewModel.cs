using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public class PreInvoiceViewModel
    {
        public PreInvoiceViewModel()
        {
            PreInvoice_Details = new ObservableCollection<PreInvoiceDetail>();
        }
        public ObservableCollection<PreInvoiceDetail>  PreInvoice_Details { get; set; }
    }
}

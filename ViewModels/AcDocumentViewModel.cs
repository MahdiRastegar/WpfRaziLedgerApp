using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfRaziLedgerApp;

namespace WpfRaziLedgerApp
{
    public class AcDocumentViewModel
    {
        public AcDocumentViewModel()
        {
            AcDocumentDetails = new ObservableCollection<AcDocumentDetail>();
        }
        public ObservableCollection<AcDocumentDetail> AcDocumentDetails {  get; set; }
    }
}

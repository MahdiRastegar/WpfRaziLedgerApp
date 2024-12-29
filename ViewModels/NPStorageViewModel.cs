using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public class NPStorageViewModel
    {
        public NPStorageViewModel()
        {
            NPStorage_Details = new ObservableCollection<NpstorageDetail>();
        }
        public ObservableCollection<NpstorageDetail>  NPStorage_Details { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public class StorageTransferViewModel
    {
        public StorageTransferViewModel()
        {
            StorageTransfer_Details = new ObservableCollection<StorageTransferDetail>();
        }
        public ObservableCollection<StorageTransferDetail>  StorageTransfer_Details { get; set; }
    }
}

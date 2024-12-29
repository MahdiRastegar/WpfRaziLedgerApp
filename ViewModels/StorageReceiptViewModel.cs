using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public class StorageReceiptViewModel
    {
        public StorageReceiptViewModel()
        {
            StorageReceiptDetails = new ObservableCollection<StorageReceiptDetail>();
        }
        public ObservableCollection<StorageReceiptDetail>  StorageReceiptDetails { get; set; }
    }
}

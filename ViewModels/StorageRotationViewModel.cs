using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public class StorageRotationViewModel
    {
        public StorageRotationViewModel()
        {
            StorageRotation_Details = new ObservableCollection<StorageRotationDetail>();
        }
        public ObservableCollection<StorageRotationDetail>  StorageRotation_Details { get; set; }
    }
}

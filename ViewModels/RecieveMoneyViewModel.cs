using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfRaziLedgerApp;

namespace WpfRaziLedgerApp
{
    public class RecieveMoneyViewModel
    {
        public RecieveMoneyViewModel()
        {
            Banks = new ObservableCollection<Bank>();
            recieveMoney_Details = new ObservableCollection<RecieveMoneyDetail>();
        }
        public ObservableCollection<Bank> Banks {  get; set; }
        public ObservableCollection<RecieveMoneyDetail> recieveMoney_Details {  get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfRaziLedgerApp;

namespace WpfRaziLedgerApp
{
    public class PaymentMoneyViewModel
    {
        public PaymentMoneyViewModel()
        {
            Banks = new ObservableCollection<Bank>();
            paymentMoney_Details = new ObservableCollection<PaymentMoneyDetail>();
        }
        public ObservableCollection<Bank> Banks {  get; set; }
        public ObservableCollection<PaymentMoneyDetail> paymentMoney_Details {  get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public class OrderViewModel
    {
        public OrderViewModel()
        {
            Order_Details = new ObservableCollection<OrderDetail>();
        }
        public ObservableCollection<OrderDetail>  Order_Details { get; set; }
    }
}

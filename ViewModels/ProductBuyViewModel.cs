using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public class ProductBuyViewModel
    {
        public ProductBuyViewModel()
        {
            ProductBuy_Details = new ObservableCollection<ProductBuyDetail>();
        }
        public ObservableCollection<ProductBuyDetail>  ProductBuy_Details { get; set; }
    }
}

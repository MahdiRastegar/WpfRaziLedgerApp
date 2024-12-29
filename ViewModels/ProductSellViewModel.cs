using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public class ProductSellViewModel
    {
        public ProductSellViewModel()
        {
            ProductSell_Details = new ObservableCollection<ProductSellDetail>();
        }
        public ObservableCollection<ProductSellDetail>  ProductSell_Details { get; set; }
    }
}

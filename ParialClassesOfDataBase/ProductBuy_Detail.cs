using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace WpfRaziLedgerApp
{
    public partial class ProductBuyDetail : IDataErrorInfo
    {
        private string _CommodityName;
        [NotMapped]
public string CommodityName
        {
            get
            {
                if (FkCommodity == null)
                {
                    _CommodityName = "";
                    return _CommodityName;
                }
                _CommodityName = FkCommodity.Name;
                return _CommodityName;
            }
            set { _CommodityName = value; }
        }
        private int _CommodityCode;
        [NotMapped]
        public int CommodityCode
        {
            get
            {
                if (FkCommodity == null)
                {
                    //_CommodityCode = 0;
                    return _CommodityCode;
                }
                _CommodityCode = FkCommodity.Code;
                return _CommodityCode;
            }
            set
            {
                _CommodityCode = value;
                if (FkCommodity != null)
                {
                    TaxPercent2 = (decimal)-.4;
                }
            }
        }
        private decimal _TaxPercent2 = (decimal)-.4;
        [NotMapped]
        public decimal TaxPercent2
        {
            get
            {
                if (FkCommodity == null)
                {
                }
                else if (_TaxPercent2 == (decimal)-.4)
                {
                    if (TaxPercent == 0)
                    {
                        if (FkCommodity.Taxable == true)
                            _TaxPercent2 = MainWindow.Current.TaxPercent;
                        else
                            _TaxPercent2 = 0;
                    }
                    else
                        _TaxPercent2 = TaxPercent;
                }
                TaxPercent = _TaxPercent2;
                return _TaxPercent2;
            }
            set
            {
                _TaxPercent2 = value;
            }
        }
        public decimal SumNextDiscount
        {
            get
            {
                return (Value * Fee) - Discount;
            }
        }
        public decimal Tax
        {
            get
            {
                return SumNextDiscount * TaxPercent / 100;
            }
        }
        public decimal Sum
        {
            get
            {
                return SumNextDiscount + Tax;
            }
        }
        [Display(AutoGenerateField = false)]
        public string Error
        {
            get
            {
                if (_Errors.Count > 0)
                    return "اطلاعات در گرید به شکل درست وارد نشده";
                return string.Empty;
            }
        }
        private List<string> _Errors = new List<string>();
        public void ClearErrors()
        {
            _Errors.Clear();
        }
        public string this[string columnName]
        {
            get
            {
                if (FkCommodity == null)
                    return string.Empty;
                switch (columnName)
                {
                    case "Value":
                        if (Value==0)
                        {
                            _Errors.AddUniqueItem("Value");
                            return "مقدار را وارد کنید!";
                        }
                        _Errors.Remove("Value");
                        return string.Empty;
                    case "Fee":
                        if (Fee==0)
                        {
                            _Errors.AddUniqueItem("Fee");
                            return "فی را وارد کنید!";
                        }
                        _Errors.Remove("Fee");
                        return string.Empty;
                }

                return string.Empty;
            }
        }
    }
}

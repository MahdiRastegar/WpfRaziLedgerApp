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
    public partial class ProductSellDetail : IDataErrorInfo
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
            set { _CommodityCode = value; }
        }
        [NotMapped]
        bool? _IsTax2
        {
            get => IsTax;
            set => IsTax = value;
        }
        [NotMapped]
        public bool? IsTax2
        {
            get
            {
                if (FkCommodity == null)
                {
                }
                else if (_IsTax2 == null)
                {
                    _IsTax2 = FkCommodity.Taxable;
                }
                if (_IsTax2 == false)
                    TaxPercent = 0;
                else
                    TaxPercent = MainWindow.Current.TaxPercent == -1 ? 10 : MainWindow.Current.TaxPercent;
                return _IsTax2;
            }
            set { _IsTax2 = value; }
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

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
    public partial class NpstorageDetail
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
    }
}

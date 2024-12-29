﻿using Syncfusion.Windows.Shared;
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
    public partial class RecieveMoneyDetail:IDataErrorInfo
    {

        private string _Name;
        [NotMapped]
        public string Name
        {
            get
            {
                if (FkPreferential == null || FkMoein == null)
                {
                    _Name = null;
                    return _Name;
                }
                _Name = $"{FkPreferential.PreferentialName}-{FkMoein.MoeinName}";
                return _Name;
            }
            set { _Name = value; }
        }
        private string _ColeMoein;
        [NotMapped]
        public string ColeMoein
        {
            get
            {
                if (FkMoein == null)
                {
                    _ColeMoein = null;
                    return _ColeMoein;
                }
                _ColeMoein = $"{FkMoein.FkCol.ColCode}{FkMoein.MoeinCode}";
                return _ColeMoein;
            }
            set { _ColeMoein = value; }
        }
        private string _PreferentialCode;
        [NotMapped]
        public string PreferentialCode
        {
            get
            {
                if (FkPreferential == null)
                {
                    _PreferentialCode = null;
                    return _PreferentialCode;
                }
                _PreferentialCode = $"{FkPreferential.PreferentialCode}";
                return _PreferentialCode;
            }
            set { _PreferentialCode = value; }
        }

        [NotMapped]
        public string DateString
        {
            get 
            {
                if (Date != null)
                {
                    return Date.Value.ToPersianDateString();
                }
                return null; 
            }
            set
            {
                if (value.Count() == 8)
                {
                    try
                    {
                        Date = $"{value.Substring(0,4)}/{value.Substring(4, 2)}/{value.Substring(6, 2)}".ToDateTimeOfString();
                    }
                    catch { }
                }
            }
        }

        [NotMapped]
        public string GetMoneyType
        {
            get
            {
                switch (MoneyType)
                {
                    case 0:
                        return "1-نقد";
                    case 1:
                        return "2-چک";
                    case 2:
                        return "3-تخفیف";
                    case 3:
                        return "4-سایر";
                }
                return null;
            }
            set { }                
        }
        [NotMapped]
        public string Price2
        {
            get
            {
                if (Price == 0)
                    return null;
                return (Price as decimal?).ToComma();
            }
            set { }
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
                if (MoneyType == 255)
                    return string.Empty;
                switch (columnName)
                {
                    case "ColeMoein":
                        if (ColeMoein == "" || ColeMoein == null)
                        {
                            _Errors.AddUniqueItem("ColeMoein");
                            return "کد کل و معین را وارد کنید!";
                        }
                        _Errors.Remove("ColeMoein");
                        return string.Empty;
                    case "PreferentialCode":
                        if (PreferentialCode == "" || PreferentialCode == null)
                        {
                            _Errors.AddUniqueItem("PreferentialCode");
                            return "کد تفضیلی را وارد کنید!";
                        }
                        _Errors.Remove("PreferentialCode");
                        return string.Empty;
                }
                if (columnName.Equals("Price"))
                {
                    if (Price == 0)
                    {
                        _Errors.AddUniqueItem("Price");
                        return "مبلغ نمی تواند صفر باشد!";
                    }
                    _Errors.Remove("Price");
                    return string.Empty;
                }
                switch (MoneyType)
                {
                    case 0:
                        break;
                    case 1:
                        switch (columnName)
                        {
                            case "DateString":
                                if (Date == null)
                                {
                                    _Errors.AddUniqueItem("DateString");
                                    return "تاریخ را وارد کنید!";
                                }
                                _Errors.Remove("DateString");
                                return string.Empty;
                            case "Bank":
                                if (FkBank == null)
                                {
                                    _Errors.AddUniqueItem("Bank");
                                    return "نام بانک را وارد کنید!";
                                }
                                _Errors.Remove("Bank");
                                return string.Empty;
                            case "Number":
                                _Errors.Remove("Number3");
                                if (Number == "" || Number == null)
                                {
                                    _Errors.AddUniqueItem("Number1");
                                    return "شماره چک را وارد کنید!";
                                }
                                _Errors.Remove("Number1");
                                return string.Empty;
                        }
                        break;
                    case 2:
                        break;
                    case 3:
                        _Errors.Remove("Number1");
                        if (columnName == "Number" && (Number == "" || Number == null))
                        {
                            _Errors.AddUniqueItem("Number3");
                            return "شماره را وارد کنید!";
                        }
                        _Errors.Remove("Number3");
                        return string.Empty;
                }

                return string.Empty;
            }
        }
    }
}

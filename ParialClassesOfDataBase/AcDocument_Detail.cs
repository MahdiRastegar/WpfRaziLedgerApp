using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public partial class AcDocumentDetail
    {        
        private string _Name;
        [NotMapped]
        public string Name
        { 
            get
            {
                if (FkPreferential == null && FkMoein == null)
                {
                    _Name = null;
                    return _Name;
                }
                _Name = $"{FkMoein?.MoeinName}-{FkPreferential?.PreferentialName}";
                if(_Name.StartsWith("-"))
                    _Name = _Name.Substring(1);
                if(_Name.EndsWith("-"))
                    _Name=_Name.Substring(0, _Name.Length - 2);
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
        //_PreferentialName = $"{Preferential.PreferentialName}-{Account.Moein.MoeinName}";
        [NotMapped]
        public string Debtor2
        {
            get
            {
                if (Debtor == null)
                    return null;
                return Debtor.ToComma();
            }
            set { }
        }
        [NotMapped]
        public string Creditor2
        {
            get
            {
                if (Creditor == null)
                    return null;
                return Creditor.ToComma();
            }
            set { }
        }
        public string HeaderDate
        {
            get
            {
                return FkAcDocHeader.Date.ToPersianDateString();
            }
        }
        public long HeaderNoDoument
        {
            get
            {
                return FkAcDocHeader.NoDoument;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public partial class RecieveMoneyHeader
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
    }
}

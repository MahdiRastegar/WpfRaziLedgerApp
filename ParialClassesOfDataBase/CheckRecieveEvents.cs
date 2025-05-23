﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public partial class CheckRecieveEvent
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
    }
}

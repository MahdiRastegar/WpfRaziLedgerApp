using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public class PreferentialAcReport
    {
        [JsonIgnore]
        public ObservableCollection<AcDocumentDetail> acDocumentDetails { get; set; }

        public Guid Id { get; set; }
        [JsonIgnore]
        public Moein FkMoein { get; set; }
        [JsonIgnore]
        public Preferential FkPreferential { get; set; }
        private string _Name;
        public string Name
        {
            get
            {
                if (FkPreferential == null && FkMoein == null)
                {
                    _Name = null;
                    return _Name;
                }
                _Name = $"{FkPreferential?.PreferentialName}-{FkMoein?.MoeinName}";
                if (_Name.StartsWith("-"))
                    _Name = _Name.Substring(1);
                if (_Name.EndsWith("-"))
                    _Name = _Name.Substring(0, _Name.Length - 2);
                return _Name;
            }
            set { _Name = value; }
        }
        private string _ColeMoein;
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
        public decimal? BeforeSum { get; set; }
        public decimal? SumDebtor { get; set; }
        public decimal? SumCreditor { get; set; }
        public decimal? RemainingDebtor
        {
            get
            {
                if (SumDebtor - SumCreditor > 0)
                    return SumDebtor - SumCreditor;
                return 0;
            }
        }
        public decimal? RemainingCreditor
        {
            get
            {
                if (SumCreditor - SumDebtor > 0)
                    return SumCreditor - SumDebtor;
                return 0;
            }
        }
        public decimal? BeforeDebtor
        {
            get
            {
                if (BeforeSum > 0)
                    return BeforeSum;
                return 0;
            }
        }
        public decimal? BeforeCreditor
        {
            get
            {
                if (BeforeSum < 0)
                    return -BeforeSum;
                return 0;
            }
        }
        [JsonIgnore]
        public Guid? moeinId { get; set; }
    }
}

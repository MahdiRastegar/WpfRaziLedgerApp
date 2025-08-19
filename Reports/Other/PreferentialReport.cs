using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfRaziLedgerApp
{
    public class PreferentialReport
    {
        public PreferentialReport()
        {
            
        }

        public Guid Id { get; set; }
        public int PreferentialCode { get; set; }
        public string PreferentialName { get; set; }
        public virtual string GroupName { get; set; }
        public virtual string ProvinceName { get; set; }
        public virtual string CityName { get; set; }
        public string Mobile { get; set; }
        public string PostalCode { get; set; }
        public string Address { get; set; }


        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Phone3 { get; set; }
        public string WebSite { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string EconomicCode { get; set; }
        public string NationalCode { get; set; }
        public string RegistrationNumber { get; set; }
        public byte? AccountType { get; set; }        
    }
}

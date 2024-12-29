using System;
using System.Collections.Generic;

#nullable disable

namespace WpfRaziLedgerApp
{
    public partial class DocumentType
    {
        public DocumentType()
        {
            AcDocumentHeaders = new HashSet<AcDocumentHeader>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsManual { get; set; }

        public virtual ICollection<AcDocumentHeader> AcDocumentHeaders { get; set; }
    }
}

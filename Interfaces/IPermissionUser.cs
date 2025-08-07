namespace WpfRaziLedgerApp
{
    public interface IPermissionUser
    {
        public bool? CanInsert { get; set; }
        public bool? CanDelete { get; set; }
        public bool? CanModify { get; set; }
    }
}
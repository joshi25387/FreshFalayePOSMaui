namespace FreshFalaye.Pos.Shared.Models
{
    public class LocalSetting : BaseEntity
    {        
        public Guid BranchId { get; set; }
        public Guid PosDeviceId { get; set; }
        public string BranchCode { get; set; } = null!;
        public string PosDeviceCode { get; set; } = null!;

        public int LastBillSequence { get; set; } = 0;

        public DateTime? LastSyncAt { get; set; }
    }
}

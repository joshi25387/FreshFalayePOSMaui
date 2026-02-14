namespace FreshFalaye.Pos.Shared.Models
{
    public class SyncState
    {
        public int Id { get; set; }
        public long LastSaleVersion { get; set; } = 0;
    }
}

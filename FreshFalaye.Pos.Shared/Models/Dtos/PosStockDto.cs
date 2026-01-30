namespace FreshFalaye.Pos.Shared.Models.Dtos
{
    public class PosStockDto
    {
        public Guid ProductId { get; set; }
        public decimal Qty { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}

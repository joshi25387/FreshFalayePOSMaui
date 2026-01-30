using System.ComponentModel.DataAnnotations;

namespace FreshFalaye.Pos.Shared.Models
{
    public class LocalSaleItem
    {
        [Key]
        public long LocalSaleItemId { get; set; }

        public long LocalSaleId { get; set; }   // FK to LocalSale

        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string UnitCode { get; set; } = null!;

        public decimal Qty { get; set; }
        public decimal Mrp { get; set; }
        public decimal Discount { get; set; }
        public decimal Rate { get; set; }
        public decimal GstPercent { get; set; }

        public decimal Amount { get; set; }
        public decimal GstAmount { get; set; }
        public decimal LineTotal { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace FreshFalaye.Pos.Shared.Models
{
    public class CartItem
    {
        [Key]
        public Guid ProductId { get; set; }
        public Guid UnitId { get; set; }
        public string ProductName { get; set; } = null!;
        public string UnitCode { get; set; } = null!;
        public bool DecimalAllowed { get; set; }
        public bool PrimaryUnitSelected { get; set; } = false;
        public decimal UnitConversionRatio { get; set; } = 1;

        public decimal Qty { get; set; }
        public decimal Mrp { get; set; }
        public decimal Discount { get; set; }
        public decimal Rate { get; set; }
        public decimal GstPercent { get; set; }

        public decimal Amount => Qty * Rate;
        public decimal GstAmount => Amount * GstPercent / 100;
        public decimal LineTotal => Amount + GstAmount;
        public bool UseWeighingScale { get; set; }
        public List<UnitDto> AvailableUnits { get; set; }
    }

    public class UnitDto
    {
        public Guid Id { get; set; }
        public string UnitCode { get; set; }
    }
}

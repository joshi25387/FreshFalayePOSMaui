using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshFalaye.Pos.Shared.Models
{
    public class LocalProduct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid ProductId { get; set; }
        public Guid SyncId { get; set; }
        public string ProductName { get; set; } = null!;
        public Guid ProductGroupId { get; set; }
        public LocalProductGroup? ProductGroup { get; set; }        
        public string UnitCode { get; set; } = null!;
        public bool DecimalAllowed { get; set; }
        public decimal Mrp { get; set; }
        public decimal Discount { get; set; }
        public decimal SalePrice { get; set; }
        public decimal GstPercent { get; set; }
        public string? ImagePath { get; set; }
        public bool IsActive { get; set; }
        public bool UseWeighingScale { get; set; }

        [NotMapped]
        public string? Base64Image { get; set; }
    }
}

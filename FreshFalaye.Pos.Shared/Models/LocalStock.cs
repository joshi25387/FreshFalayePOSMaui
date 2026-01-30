using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshFalaye.Pos.Shared.Models
{
    public class LocalStock
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid ProductId { get; set; }
        public decimal Qty { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}

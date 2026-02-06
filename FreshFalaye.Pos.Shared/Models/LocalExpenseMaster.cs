using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshFalaye.Pos.Shared.Models
{
    public class LocalExpenseMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public Guid SyncId { get; set; }

        // Sale / Purchase
        public string ExpenseType { get; set; } = null!;

        public string ExpenseName { get; set; } = null!;

        // # or %
        public string RateType { get; set; } = null!;

        public decimal Rate { get; set; }
        public string AddDeduct { get; set; }

        // Self / Customer
        public string Bearer { get; set; } = null!;

        public bool IsTaxable { get; set; }

        public bool IsActive { get; set; } = true;

    }
}

using System.ComponentModel.DataAnnotations;

namespace FreshFalaye.Pos.Shared.Models
{
    public class LocalSaleExpense
    {
        [Key]
        public long LocalSaleExpenseId { get; set; }

        public long LocalSaleId { get; set; }

        public Guid ExpenseId { get; set; }
        public string ExpenseName { get; set; } = null!;

        public string RateType { get; set; } = null!; // # or %
        public decimal Rate { get; set; }

        public decimal Amount { get; set; }   // Final calculated amount

        public string Bearer { get; set; } = null!; // Self / Customer
    }
}

using System.ComponentModel.DataAnnotations;

namespace FreshFalaye.Pos.Shared.Models
{
    public class LocalSale
    {
        [Key]
        public long LocalSaleId { get; set; }   // Local identity

        public Guid SyncId { get; set; } = Guid.NewGuid();

        // 🔹 Bill identity
        public string BillNo { get; set; } = null!;
        public DateTime SaleDate { get; set; } = DateTime.Now;

        // 🔹 Customer details
        public string? CustomerMobile { get; set; }
        public string? CustomerName { get; set; }

        // 🔹 Payment
        public string PaymentMode { get; set; } = "Cash";

        // 🔹 Totals
        public decimal ProductSubTotal { get; set; }
        public decimal GstTotal { get; set; }
        public decimal ExpenseTotal { get; set; }
        public decimal GrandTotal { get; set; }

        // 🔹 Sync
        public bool IsSynced { get; set; } = false;

        // 🔴 UPLOAD TRACKING
        public bool IsUploaded { get; set; } = false;
        public DateTime? UploadedAt { get; set; }
        public long SyncVersion { get; set; }

        public ICollection<LocalSaleItem> Items { get; set; }
        = new List<LocalSaleItem>();

        public ICollection<LocalSaleExpense> Expenses { get; set; }
            = new List<LocalSaleExpense>();
    }
}

namespace FreshFalaye.Pos.Shared.Models.Dtos
{
    public class SaleUploadDto
    {
        public Guid SyncId { get; set; }
        public Guid BranchId { get; set; }

        public string BillNo { get; set; } = null!;
        public DateTime SaleDate { get; set; }
        public string? CustomerMobile { get; set; }
        public string? CustomerName { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GstTotal { get; set; }
        public decimal GrandTotal { get; set; }

        public string PaymentMode { get; set; } = "";

        public List<SaleItemUploadDto> Items { get; set; } = new();
        public List<SaleExpenseUploadDto> Expenses { get; set; } = new();
    }

    public class SaleItemUploadDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string UnitCode { get; set; }
        public decimal Qty { get; set; }
        public decimal Mrp { get; set; }
        public decimal Discount { get; set; }
        public decimal Rate { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class SaleExpenseUploadDto
    {
        public Guid ExpenseId { get; set; }
        public string ExpenseName { get; set; } = "";
        public decimal Amount { get; set; }
        public string RateType { get; set; } = null!; // # or %
        public decimal Rate { get; set; }
        public string AddDeduct { get; set; }
        public string Bearer { get; set; }

    }

}

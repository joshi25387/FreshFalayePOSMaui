using System.ComponentModel.DataAnnotations;

namespace FreshFalaye.Pos.Shared.Models
{
    public class LocalStoreSettings
    {
        [Key]
        public int Id { get; set; } = 1;

        public string StoreName { get; set; } = "";
        public string Address { get; set; } = "";
        public string GstIn { get; set; } = "";
        public string Phone { get; set; } = "";
        public string BillPrefix { get; set; } = "";
        public string POSCounterNo { get; set; } = "";        
        public string POSSalesman { get; set; } = "";        
        public string ReceiptLine1 { get; set; }
        public string ReceiptLine2 { get; set; }
        public string ReceiptLine3 { get; set; }
        public string ReceiptLine4 { get; set; }
        public string ReceiptLine5 { get; set; }
        public string ReceiptLine6 { get; set; }
        public string UpiId { get; set; }
        public string UpiMerchantName { get; set; }
    }

}

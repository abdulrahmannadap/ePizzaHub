namespace ePizzaHub.Models
{
    public class ReceiptModel
    {
        public string TransactionId { get; set; }
        public string Currency { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

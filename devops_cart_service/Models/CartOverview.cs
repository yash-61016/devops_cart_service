namespace devops_cart_service.Models
{
    public class CartOverview
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsCheckedOut { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

    }
}

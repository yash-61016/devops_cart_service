namespace devops_cart_service.Models.Dto
{
    public class CartOverviewUpdateDto
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsCheckedOut { get; set; }
    }
}
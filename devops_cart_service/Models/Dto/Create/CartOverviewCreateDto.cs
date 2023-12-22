namespace devops_cart_service.Models.Dto
{
    public class CartOverviewCreateDto
    {
        public int UserId { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
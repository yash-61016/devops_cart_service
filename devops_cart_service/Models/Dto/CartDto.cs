namespace devops_cart_service.Models.Dto
{
    public class CartDto
    {
        public required CartOverviewDto CartOverview { get; set; }
        public IEnumerable<CartProductDto>? CartProducts { get; set; }
    }
}
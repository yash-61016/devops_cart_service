namespace devops_cart_service.Models.Dto
{
    public class CartUpdateDto
    {
        public required CartOverviewUpdateDto CartOverview { get; set; }
        public required IEnumerable<CartProductUpdateDto> CartProducts { get; set; }
    }
}
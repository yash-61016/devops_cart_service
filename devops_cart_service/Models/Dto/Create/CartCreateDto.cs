namespace devops_cart_service.Models.Dto
{
    public class CartCreateDto
    {
        public required CartOverviewCreateDto CartOverview { get; set; }
        public required List<CartProductCreateDto> CartProducts { get; set; }
    }
}
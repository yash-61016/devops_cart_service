namespace devops_cart_service.Models
{
    public class Cart
    {
        public required CartOverview CartOverview { get; set; }
        public required IEnumerable<CartProduct> CartProducts { get; set; }
    }
}
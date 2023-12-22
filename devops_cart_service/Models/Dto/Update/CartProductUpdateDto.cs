namespace devops_cart_service.Models.Dto
{
    public class CartProductUpdateDto
    {
        public int CartProductId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public bool IsDeleted { get; set; }
    }
}
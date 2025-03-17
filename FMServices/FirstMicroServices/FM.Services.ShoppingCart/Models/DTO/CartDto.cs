namespace FM.Services.ShoppingCart.Models.DTO
{
    public class CartDto
    {
        public CartHeaderDto cartHeader { get; set; }
        public IEnumerable<CartDetailsDto> cartDetails { get; set; }
    }
}

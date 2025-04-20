namespace GadgetShopApi.Models
{
    public class InventoryModelDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int AvailableQty { get; set; }
        public int ReorderPoint { get; set; }
    }
}

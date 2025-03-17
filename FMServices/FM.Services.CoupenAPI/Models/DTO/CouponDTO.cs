namespace FM.Services.CoupenAPI.Models.DTO
{
    public class CouponDTO
    {
        public int CouponId { get; set; }
        public string CoupenCode { get; set; }
        public double DiscountAmount { get; set; }
        public int MinAmount { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace FM.Services.CoupenAPI.Models
{
    public class CouponModel
    {
        [Key]
        public int CouponId { get; set; }
        [Required]
        public string CoupenCode { get; set; }
        [Required]
        public double DiscountAmount { get; set; }
        public int MinAmount { get; set; }
    }
}

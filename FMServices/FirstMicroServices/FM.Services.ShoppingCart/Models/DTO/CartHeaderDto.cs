﻿using System.ComponentModel.DataAnnotations.Schema;

namespace FM.Services.ShoppingCart.Models.DTO
{
    public class CartHeaderDto
    {
        public int CartHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? CouponCode { get; set; }
        public double? Discount { get; set; }
        public double? CartTotal { get; set; }
    }
}

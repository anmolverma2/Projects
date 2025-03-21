﻿using System.ComponentModel.DataAnnotations;

namespace FM.Web.Models
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }        
        public double Price { get; set; }
        public int Count { get; set; } = 1;
    }
}

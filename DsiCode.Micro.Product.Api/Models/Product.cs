﻿using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace DsiCode.Micro.Product.Api.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        [Range(1,100)]

        public double Price { get; set; }
        public string Description { get; set; }
        public string CategoryName  { get; set; }
        public String ImageUrl { get; set; }
        public string ImageLocalPath { get; set; }
    }
}

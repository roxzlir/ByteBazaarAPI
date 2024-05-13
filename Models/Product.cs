﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ByteBazaarAPI.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [StringLength(30)]
        public string Title { get; set; }
        [StringLength(250)]
        public string Description { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        [Required]
        [ForeignKey("Category")]
        public int FkCategoryId { get; set; }
        public Category Category { get; set; }
        public int Quantity { get; set; }

    }
}

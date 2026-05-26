using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketAutomation.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Barcode { get; set; } = string.Empty;

        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!;

        public int? SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchasePrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SalePrice { get; set; }

        public decimal StockQuantity { get; set; } // decimal for weight/kg

        public int MinimumStockLevel { get; set; } = 10;

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsFavorite { get; set; } = false;

        [StringLength(500)]
        public string? ImagePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

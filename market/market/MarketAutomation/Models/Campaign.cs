using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketAutomation.Models
{
    public class Campaign
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string CampaignName { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Kampanya Tipi: 3_AL_2_ODE, YUZDE_INDIRIM, TUTAR_INDIRIMI vs.
        [Required, StringLength(50)]
        public string CampaignType { get; set; } = string.Empty;

        public int? ProductId { get; set; } // Belirli bir ürün için geçerliyse
        [ForeignKey("ProductId")]
        public Product? TargetProduct { get; set; }

        public int? CategoryId { get; set; } // Belirli bir kategori için geçerliyse
        [ForeignKey("CategoryId")]
        public Category? TargetCategory { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountValue { get; set; } // %20 için 20, 10TL için 10.

        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

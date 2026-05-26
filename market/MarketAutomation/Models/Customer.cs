using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketAutomation.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(300)]
        public string? Address { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DebtBalance { get; set; } = 0; // Veresiye Borcu

        public int Points { get; set; } = 0; // Müşteri Sadakat Puanı

        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketAutomation.Models
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string ReceiptNumber { get; set; } = string.Empty; // Fiş/Fatura Numarası

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GrandTotal { get; set; }

        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Cash"; // Cash, CreditCard, Mixed, Debt (Veresiye)

        [Column(TypeName = "decimal(18,2)")]
        public decimal CashPaid { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CardPaid { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.Now;

        public int UserId { get; set; } // Kasiyer
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        public int? CustomerId { get; set; } // Opsiyonel (Veresiye veya Puan kartı için)
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
    }
}

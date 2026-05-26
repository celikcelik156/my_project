using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketAutomation.Models
{
    public class ActivityLog
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string ActionType { get; set; } = string.Empty; // LOGIN, SALE_CANCEL, DELETE_PRODUCT vs.

        public string? Description { get; set; }
        
        public DateTime LogDate { get; set; } = DateTime.Now;

        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}

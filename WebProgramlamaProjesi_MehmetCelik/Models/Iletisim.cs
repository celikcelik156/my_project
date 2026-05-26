using System;
using System.ComponentModel.DataAnnotations;

namespace WebProgramlamaProjesi_MehmetCelik.Models
{
    public class Iletisim
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad gereklidir")]
        [StringLength(100)]
        public string Ad { get; set; }

        [Required(ErrorMessage = "E-posta gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Telefon { get; set; }

        [Required(ErrorMessage = "Mesaj gereklidir")]
        public string Mesaj { get; set; }

        public DateTime OlusturmaTarihi { get; set; }

        public bool Okundu { get; set; }
    }
}


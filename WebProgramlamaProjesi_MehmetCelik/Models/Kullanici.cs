using System.ComponentModel.DataAnnotations;

namespace WebProgramlamaProjesi_MehmetCelik.Models
{
    public class Kullanici
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı gereklidir")]
        [StringLength(50)]
        public string KullaniciAdi { get; set; }

        [Required(ErrorMessage = "E-posta gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir")]
        [StringLength(255)]
        public string Sifre { get; set; }

        [StringLength(100)]
        public string AdSoyad { get; set; }

        public bool Admin { get; set; }

        public bool Aktif { get; set; }
    }
}


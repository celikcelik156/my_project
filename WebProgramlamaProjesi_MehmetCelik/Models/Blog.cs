using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace WebProgramlamaProjesi_MehmetCelik.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık gereklidir")]
        [StringLength(200)]
        public string Baslik { get; set; }

        [Required(ErrorMessage = "İçerik gereklidir")]
        public string Icerik { get; set; }

        [StringLength(500)]
        public string ResimYolu { get; set; }

        [NotMapped]
        public HttpPostedFileBase ResimDosyasi { get; set; }

        public DateTime OlusturmaTarihi { get; set; }

        public DateTime? GuncellemeTarihi { get; set; }

        public int? KategoriId { get; set; }
        public virtual Kategori Kategori { get; set; }

        [StringLength(100)]
        public string Yazar { get; set; }

        public int GoruntulenmeSayisi { get; set; }

        public bool Aktif { get; set; }
    }
}


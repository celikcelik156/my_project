using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebProgramlamaProjesi_MehmetCelik.Models
{
    public class Kategori
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı gereklidir")]
        [StringLength(100)]
        public string Ad { get; set; }

        [StringLength(500)]
        public string Aciklama { get; set; }

        public virtual ICollection<Blog> Bloglar { get; set; }

        public Kategori()
        {
            Bloglar = new HashSet<Blog>();
        }
    }
}


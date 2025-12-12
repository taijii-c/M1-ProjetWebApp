using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace M1_ProjetWebApp.Models
{
    public class Article
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le titre est obligatoire")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le contenu est obligatoire")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Date de publication")]
        [DataType(DataType.DateTime)]
        public DateTime PublishedDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Auteur")]
        public string AuthorId { get; set; } = string.Empty;

        [ForeignKey("AuthorId")]
        public IdentityUser? Author { get; set; }
    }
}

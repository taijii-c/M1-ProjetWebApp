using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace M1_ProjetWebApp.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le contenu du commentaire est obligatoire")]
        [StringLength(1000, ErrorMessage = "Le commentaire ne peut pas dépasser 1000 caractères")]
        [Display(Name = "Commentaire")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Date de publication")]
        public DateTime PublishedDate { get; set; } = DateTime.Now;

        // Relations
        [Required]
        public int ArticleId { get; set; }

        [ForeignKey("ArticleId")]
        public Article? Article { get; set; }

        [Required]
        [Display(Name = "Auteur")]
        public string AuthorId { get; set; } = string.Empty;

        [ForeignKey("AuthorId")]
        public IdentityUser? Author { get; set; }
    }
}

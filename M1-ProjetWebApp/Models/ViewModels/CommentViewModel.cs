using System.ComponentModel.DataAnnotations;

namespace M1_ProjetWebApp.Models.ViewModels
{
    public class CommentViewModel
    {
        [Required(ErrorMessage = "Le commentaire est obligatoire")]
        [StringLength(1000, ErrorMessage = "Le commentaire ne peut pas dépasser 1000 caractères")]
        [Display(Name = "Votre commentaire")]
        public string Content { get; set; } = string.Empty;

        public int ArticleId { get; set; }
    }
}

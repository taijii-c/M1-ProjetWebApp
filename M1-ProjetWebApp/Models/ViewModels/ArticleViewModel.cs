using System.ComponentModel.DataAnnotations;

namespace M1_ProjetWebApp.Models.ViewModels
{
    public class ArticleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le titre est obligatoire")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le contenu est obligatoire")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; } = string.Empty;

        public DateTime PublishedDate { get; set; }
        public string? AuthorId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace M1_ProjetWebApp.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le titre est obligatoire")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "La description est obligatoire")]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Date de réalisation")]
        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; } = DateTime.Now;

        [Display(Name = "Lien GitHub")]
        [Url(ErrorMessage = "Veuillez entrer une URL valide")]
        [StringLength(500)]
        public string? GithubUrl { get; set; }

        [Display(Name = "Lien Live")]
        [Url(ErrorMessage = "Veuillez entrer une URL valide")]
        [StringLength(500)]
        public string? LiveUrl { get; set; }

        [Display(Name = "Image du projet")]
        [StringLength(500)]
        public string? ImagePath { get; set; }
    }
}

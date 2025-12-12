using M1_ProjetWebApp.Models;

namespace M1_ProjetWebApp.Models.ViewModels
{
    public class ArticleDetailsViewModel
    {
        public Article Article { get; set; } = new();
        public CommentViewModel NewComment { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
    }
}

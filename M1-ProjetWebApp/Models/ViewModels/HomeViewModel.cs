using M1_ProjetWebApp.Models;

namespace M1_ProjetWebApp.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Project> LatestProjects { get; set; } = new();
        public List<Article> LatestArticles { get; set; } = new();
    }
}

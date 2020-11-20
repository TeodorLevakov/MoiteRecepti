namespace MoiteRecepti.Web.Controllers
{
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using MoiteRecepti.Data;
    using MoiteRecepti.Data.Common.Repositories;
    using MoiteRecepti.Data.Models;
    using MoiteRecepti.Services.Data;
    using MoiteRecepti.Web.ViewModels;
    using MoiteRecepti.Web.ViewModels.Home;

    public class HomeController : BaseController
    {
        private readonly IGetCountsService countsServise;

        public HomeController(IGetCountsService countsServise)
        {
            this.countsServise = countsServise;
        }

        public IActionResult Index()
        {
            var counts = this.countsServise.GetCounts();

            var viewModel = new IndexViewModel
            {
                CategoriesCount = counts.CategoriesCount,
                ImagesCount = counts.ImagesCount,
                IngredientsCount = counts.IngredientsCount,
                RecipesCount = counts.RecipesCount,
            };

            return this.View(viewModel);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}

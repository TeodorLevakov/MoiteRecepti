using Microsoft.AspNetCore.Mvc;
using MoiteRecepti.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoiteRecepti.Web.Controllers
{
    public class GatherRecipeController : BaseController
    {
        private readonly IGotvachBGScraper gotvachBGScraper;

        public GatherRecipeController(IGotvachBGScraper gotvachBGScraper)
        {
            this.gotvachBGScraper = gotvachBGScraper;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public async Task<IActionResult> Add()
        {
            await this.gotvachBGScraper.PopulateDBWithRecipesAsync(1000);

            return this.Redirect("/");
        }
    }
}

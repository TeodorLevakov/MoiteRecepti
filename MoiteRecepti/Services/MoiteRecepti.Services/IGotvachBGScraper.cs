namespace MoiteRecepti.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public interface IGotvachBGScraper
    {
        Task PopulateDBWithRecipesAsync(int recipeCount);
    }
}

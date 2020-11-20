namespace MoiteRecepti.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using MoiteRecepti.Web.ViewModels.Recipes;

    public interface IRecipeService
    {
        void Create(CreateRecipeInputModel input);
    }
}

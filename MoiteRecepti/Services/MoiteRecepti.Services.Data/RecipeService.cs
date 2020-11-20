namespace MoiteRecepti.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using MoiteRecepti.Data.Common.Repositories;
    using MoiteRecepti.Data.Models;
    using MoiteRecepti.Web.ViewModels.Recipes;

    public class RecipeService : IRecipeService
    {
        private readonly IDeletableEntityRepository<Recipe> recipeRepository;
        private readonly IDeletableEntityRepository<Ingredient> ingredientRepository;

        public RecipeService(IDeletableEntityRepository<Recipe> recipeRepository,
                            IDeletableEntityRepository<Ingredient> ingredientRepository)
        {
            this.recipeRepository = recipeRepository;
            this.ingredientRepository = ingredientRepository;
        }

        public void Create(CreateRecipeInputModel input)
        {
            var recipe = new Recipe 
            {
                CategoryId = input.CategoryId,
                CookingTime = TimeSpan.FromMinutes(input.CookingTime),
                Instructions = input.Instructions,
                Name = input.Name,
                PortionsCount = input.PortionsCount,
                PreparatioTime = TimeSpan.FromMinutes(input.PreparatioTime),
            };
        }
    }
}

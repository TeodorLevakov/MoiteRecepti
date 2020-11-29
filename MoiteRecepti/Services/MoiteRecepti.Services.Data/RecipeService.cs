namespace MoiteRecepti.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using MoiteRecepti.Data.Common.Repositories;
    using MoiteRecepti.Data.Models;
    using MoiteRecepti.Services.Mapping;
    using MoiteRecepti.Web.ViewModels.Recipes;

    public class RecipeService : IRecipeService
    {
        private readonly IDeletableEntityRepository<Recipe> recipesRepository;
        private readonly IDeletableEntityRepository<Ingredient> ingredientsRepository;

        public RecipeService(IDeletableEntityRepository<Recipe> recipesRepository,
                            IDeletableEntityRepository<Ingredient> ingredientsRepository)
        {
            this.recipesRepository = recipesRepository;
            this.ingredientsRepository = ingredientsRepository;
        }

        public async Task CreateAsync(CreateRecipeInputModel input, string userId)
        {
            var recipe = new Recipe 
            {
                CategoryId = input.CategoryId,
                CookingTime = TimeSpan.FromMinutes(input.CookingTime),
                Instructions = input.Instructions,
                Name = input.Name,
                PortionsCount = input.PortionsCount,
                PreparatioTime = TimeSpan.FromMinutes(input.PreparatioTime),
                AddedByUserId = userId,
            };

            foreach (var item in input.Ingredients)
            {
                var ingredient = this.ingredientsRepository.All().FirstOrDefault(x => x.Name == item.IngredientName);

                if (ingredient == null)
                {
                    ingredient = new Ingredient { Name = item.IngredientName};
                }

                recipe.Ingredients.Add(new RecipeIngredient
                {
                    Ingredient = ingredient,
                    Quantity = item.Quantity,
                });
            }

            await this.recipesRepository.AddAsync(recipe);
            await this.recipesRepository.SaveChangesAsync();
        }

        public IEnumerable<RecipeInListViewModel> GetAll(int page, int itemsPerPage = 12)
        {
            var recipes = this.recipesRepository.AllAsNoTracking()
                .OrderByDescending(x => x.Id)
                .Skip((page-1) * itemsPerPage)
                .Take(itemsPerPage)
                .To<RecipeInListViewModel>()
                //.Select(x => new RecipeInListViewModel
                //{
                //    Id = x.Id,
                //    Name = x.Name,
                //    CategoryName = x.Category.Name,
                //    CategoryId = x.CategoryId,
                //    ImageUrl =
                //            x.Images.FirstOrDefault().RemoteImageUrl != null ?
                //            x.Images.FirstOrDefault().RemoteImageUrl :
                //            "images/recipes" + x.Images.FirstOrDefault().Id + "." + x.Images.FirstOrDefault().Extension,
                //})
                .ToList();

            return recipes;

        }

        public int GetCount()
        {

            return this.recipesRepository.All().Count();
        }
    }
}

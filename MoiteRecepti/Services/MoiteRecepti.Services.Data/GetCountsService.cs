namespace MoiteRecepti.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MoiteRecepti.Data.Common.Repositories;
    using MoiteRecepti.Data.Models;
    using MoiteRecepti.Services.Data.DTOs;
    using MoiteRecepti.Web.ViewModels.Home;

    public class GetCountsService : IGetCountsService
    {
        private readonly IDeletableEntityRepository<Category> categoryRepository;
        private readonly IRepository<Image> imageRepository;
        private readonly IDeletableEntityRepository<Ingredient> ingredientRepository;
        private readonly IDeletableEntityRepository<Recipe> recipesRepository;

        public GetCountsService(
            IDeletableEntityRepository<Category> categoryRepository,
            IRepository<Image> imageRepository,
            IDeletableEntityRepository<Ingredient> ingredientRepository,
            IDeletableEntityRepository<Recipe> recipesRepository)
        {
            this.categoryRepository = categoryRepository;
            this.imageRepository = imageRepository;
            this.ingredientRepository = ingredientRepository;
            this.recipesRepository = recipesRepository;
        }

        public CountsDTO GetCounts()
        {
            var data = new CountsDTO
            {
                CategoriesCount = this.categoryRepository.All().Count(),
                RecipesCount = this.recipesRepository.All().Count(),
                ImagesCount = this.imageRepository.All().Count(),
                IngredientsCount = this.ingredientRepository.All().Count(),
            };

            return data;
        }
    }
}

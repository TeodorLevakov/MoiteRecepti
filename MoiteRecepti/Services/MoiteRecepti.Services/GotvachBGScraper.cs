namespace MoiteRecepti.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    using AngleSharp;
    using MoiteRecepti.Data.Common.Repositories;
    using MoiteRecepti.Data.Models;
    using MoiteRecepti.Services.Services;

    public class GotvachBGScraper : IGotvachBGScraper
    {
        private readonly IConfiguration config;
        private readonly IBrowsingContext context;

        private readonly IDeletableEntityRepository<Category> categoriesRepository;
        private readonly IDeletableEntityRepository<Ingredient> ingredientsRepository;
        private readonly IDeletableEntityRepository<Recipe> recipiesRepository;
        private readonly IRepository<RecipeIngredient> recipeIngredientsRepository;
        private readonly IRepository<Image> imagesRepository;

        public GotvachBGScraper(IDeletableEntityRepository<Category> categoriesRepository,
                                IDeletableEntityRepository<Ingredient> ingredientsRepository,
                                IDeletableEntityRepository<Recipe> recipiesRepository,
                                IRepository<RecipeIngredient> recipeIngredientsRepository,
                                IRepository<Image> imagesRepository)
        {
            this.categoriesRepository = categoriesRepository;
            this.ingredientsRepository = ingredientsRepository;
            this.recipiesRepository = recipiesRepository;
            this.recipeIngredientsRepository = recipeIngredientsRepository;
            this.imagesRepository = imagesRepository;

            this.config = Configuration.Default.WithDefaultLoader();
            this.context = BrowsingContext.New(this.config);
        }

        public async Task PopulateDBWithRecipesAsync(int recipeCount)
        {
            var concurentBag = new ConcurrentBag<GotvachBgDTO>();

            Parallel.For(1, recipeCount, (i) =>
            {
                try
                {
                    var recipe = this.GetRecipe(i);
                    concurentBag.Add(recipe);
                }
                catch
                {
                }
            });

            foreach (var recipe in concurentBag)
            {
                var categoryId = await this.GetOrCreateCategoryAsync(recipe.CategoryName);
                
                var recipeExists = this.recipiesRepository
                    .AllAsNoTracking()
                    .Any(x => x.Name == recipe.RecipeName);

                if (recipeExists)
                {
                    continue;
                }

                var newRecipe = new Recipe
                {
                    Name = recipe.RecipeName,
                    Instructions = recipe.Instructions,
                    PreparatioTime = recipe.PreparationTime,
                    CookingTime = recipe.CookingTime,
                    PortionsCount = recipe.PortionsCount,
                    OriginalUrl = recipe.OriginalUrl,
                    CategoryId = categoryId,
                };

                await this.recipiesRepository.AddAsync(newRecipe);
                await this.recipiesRepository.SaveChangesAsync();

                foreach (var item in recipe.Ingredients)
                {
                    var ingr = item.Split("-");

                    if (ingr.Length < 2)
                    {
                        continue;
                    }

                    var ingridientId = await this.GetOrCreateIngridientsAsync(ingr[0].Trim());
                    var qty = ingr[1].Trim();

                    var recipeIngridient = new RecipeIngredient
                    {
                        IngredientId = ingridientId,
                        RecipeId = newRecipe.Id,
                        Quantity = qty,
                    };

                    await this.recipeIngredientsRepository.AddAsync(recipeIngridient);
                    await this.recipeIngredientsRepository.SaveChangesAsync();
                }

                var imag = new Image
                {
                    Extension = recipe.Extension,
                    RecipeId = newRecipe.Id,
                };

                await this.imagesRepository.AddAsync(imag);
                await this.imagesRepository.SaveChangesAsync();
            }

        }

        private async Task<int> GetOrCreateIngridientsAsync(string name)
        {
                var ingridient = this.ingredientsRepository
                    .AllAsNoTracking()
                    .FirstOrDefault(x => x.Name == name);

                if (ingridient == null)
                {
                    ingridient = new Ingredient
                    {
                        Name = name,
                    };

                    await this.ingredientsRepository.AddAsync(ingridient);
                    await this.ingredientsRepository.SaveChangesAsync();
                }

                  return ingridient.Id;
        }

        private async Task<int> GetOrCreateCategoryAsync(string categoryName)
        {
            var category = this.categoriesRepository
                                .AllAsNoTracking()
                                .FirstOrDefault(x => x.Name == categoryName);

            if (category == null)
            {
                category = new Category()
                {
                    Name = categoryName,
                };
                await this.categoriesRepository.AddAsync(category);
                await this.categoriesRepository.SaveChangesAsync();
            }

            return category.Id;
        }

        private GotvachBgDTO GetRecipe(int id)
        {
            var document = this.context.OpenAsync($"https://recepti.gotvach.bg/r-{id}")
                .GetAwaiter()
                .GetResult();

            if (document.StatusCode == HttpStatusCode.NotFound ||
                document.DocumentElement.OuterHtml.Contains("Тази страница не е намерена."))
            {
                throw new InvalidOperationException();
            }

            var resipe = new GotvachBgDTO();

            var recipeNameAndCat = document
                .QuerySelectorAll("#recEntity > div.breadcrumb")
                .Select(x => x.TextContent)
                .FirstOrDefault()
                .Split(" »")
                .Reverse()
                .ToArray();

            resipe.CategoryName = recipeNameAndCat[1];
            resipe.RecipeName = recipeNameAndCat[0];

            var instructions = document.QuerySelectorAll(".text > p")
                .Select(x => x.TextContent)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var item in instructions)
            {
                sb.AppendLine(item);
            }

            resipe.Instructions = sb.ToString().TrimEnd();

            var timing = document.QuerySelectorAll(".mbox > .feat.small");

            if (timing.Length > 0)
            {
                var prepTime = timing[0]
                    .TextContent
                    .Replace("Приготвяне", string.Empty)
                    .Replace(" мин.", string.Empty);

                var totalMin = int.Parse(prepTime);

                resipe.PreparationTime = TimeSpan.FromMinutes(totalMin);
            }

            if (timing.Length > 1)
            {
                var cookTime = timing[1]
                    .TextContent
                    .Replace("Готвене", string.Empty)
                    .Replace(" мин.", string.Empty);

                var totalMin = int.Parse(cookTime);

                resipe.CookingTime = TimeSpan.FromMinutes(totalMin);
            }

            var portionsCount = document.QuerySelectorAll(".mbox > .feat > span").LastOrDefault().TextContent;

            resipe.PortionsCount = int.Parse(portionsCount);
            
            resipe.OriginalUrl = document.QuerySelector("#newsGal > div.image > img").GetAttribute("src");

            var elements = document.QuerySelectorAll(".products > ul > li");

            foreach (var item in elements)
            {
                resipe.Ingredients.Add(item.TextContent);
            }

            return resipe;
        }
    }
}

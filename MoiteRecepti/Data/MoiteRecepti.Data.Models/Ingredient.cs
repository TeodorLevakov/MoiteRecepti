namespace MoiteRecepti.Data.Models
{
    using System.Collections.Generic;

    using MoiteRecepti.Data.Common.Models;

    public class Ingredient : BaseDeletableModel<int>
    {
        public Ingredient()
        {
            this.Reciepes = new HashSet<RecipeIngredient>();
        }

        public string Name { get; set; }

        public virtual ICollection<RecipeIngredient> Reciepes { get; set; }
    }
}

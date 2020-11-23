namespace MoiteRecepti.Services.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class GotvachBgDTO
    {
        public GotvachBgDTO()
        {
            this.Ingredients = new List<string>();
        }

        public string CategoryName { get; set; }

        public string RecipeName { get; set; }

        public string Instructions { get; set; }

        public TimeSpan PreparationTime { get; set; }

        public TimeSpan CookingTime { get; set; }

        public int PortionsCount { get; set; }

        public string OriginalUrl { get; set; }

        public string Extension { get; set; }

        public List<string> Ingredients { get; set; }

    }
}

using System;

namespace MoiteRecepti.Web.ViewModels
{
    public class PagingViewModel
    {
        public int PageNumber { get; set; }

        public bool HasPreviosPage => this.PageNumber > 1;

        public bool HasNextPage => this.PageNumber < this.PageCount;

        public int PreviosPageNumber => this.PageNumber - 1;

        public int NextPageNumber => this.PageNumber + 1;

        public int PageCount => (int)Math.Ceiling((double)this.RecipesCount / this.ItemPerPage);

        public int RecipesCount { get; set; }

        public int ItemPerPage { get; set; }
    }
}
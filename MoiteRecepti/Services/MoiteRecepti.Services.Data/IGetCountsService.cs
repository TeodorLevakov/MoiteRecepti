namespace MoiteRecepti.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using MoiteRecepti.Services.Data.DTOs;
    using MoiteRecepti.Web.ViewModels.Home;

    public interface IGetCountsService
    {
        CountsDTO GetCounts();
    }
}

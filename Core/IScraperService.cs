using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebScraperMF.Models;

namespace WebScraperMF.Core
{
    public interface IScraperService
    {
        Task<List<Product>> SearchProductAsync(string searchTerm);

    }
}

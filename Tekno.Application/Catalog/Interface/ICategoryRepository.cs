using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Domain.Catalog;

namespace Tekno.Application.Catalog.Interface
{
    public interface ICategoryRepository
    {
       Task<List<Category>> GetAllCategoriesAsync();
    }
}


using System.ComponentModel.DataAnnotations;

namespace Tekno.Api.Models.Catalog.Admin
{
    public class DeleteCategoryApiDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "ID must be greater than 0.")]
        public int Id { get; set; }
    }
}
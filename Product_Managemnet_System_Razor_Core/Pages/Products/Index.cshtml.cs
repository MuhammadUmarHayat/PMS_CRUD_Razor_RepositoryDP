using Books_Management_System.Data;
using Books_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Books_Management_System.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly ProductRepository _repository;

        public IndexModel(ProductRepository repository)
        {
            _repository = repository;
        }

        public List<Product> Products { get; set; } = new();

        public async Task OnGetAsync()
        {
            Products = await _repository.GetAllAsync();
        }
    }
}

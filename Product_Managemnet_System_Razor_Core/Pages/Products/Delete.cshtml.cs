using Books_Management_System.Data;
using Books_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Books_Management_System.Pages.Products
{
    public class DeleteModel : PageModel
    {
        private readonly ProductRepository _repository;
        private readonly ProductImageService _images;

        public DeleteModel(
            ProductRepository repository,
            ProductImageService images)
        {
            _repository = repository;
            _images = images;
        }

        public Product Product { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            Product = product;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            bool deleted = await _repository.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            _images.Delete(product.image);

            TempData["Success"] = "Product deleted successfully.";

            return RedirectToPage("Index");
        }
    }
}

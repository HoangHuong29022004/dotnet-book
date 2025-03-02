using BlazorApp.Data;  // Make sure you have this namespace for the context and models
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    // Get a product by its ID
    public async Task<Product> GetProductByIdAsync(int id)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id); // Get the product by Id
    }

    // Get all products
    public async Task<List<Product>> GetProductsAsync()
    {
        return await _context.Products.ToListAsync(); // Get all products
    }

    // Add a new product
    public async Task AddProductAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync(); // Save changes to the database
    }

    // Update an existing product
    public async Task UpdateProductAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(); // Save changes to the database
    }

    // Delete a product
    public async Task DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(); // Save changes to the database
        }
    }
}
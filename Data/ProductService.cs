using BlazorApp.Data;  // Make sure you have this namespace for the context and models
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using System;

namespace BlazorApp.Data
{
    public class ProductService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private const long MaxFileSize = 10L * 1024L * 1024L; // 10MB
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };

        public ProductService(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // Get a product by its ID
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        // Get all products
        public async Task<List<Product>> GetProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        private bool IsImageValid(IBrowserFile file)
        {
            if (file == null) return false;
            if (file.Size > MaxFileSize) return false;

            var extension = Path.GetExtension(file.Name).ToLowerInvariant();
            return AllowedExtensions.Contains(extension);
        }

        private async Task<string> SaveImageAsync(IBrowserFile imageFile)
        {
            if (!IsImageValid(imageFile))
            {
                throw new Exception("Invalid image file. Please use JPG or PNG files under 10MB.");
            }

            try
            {
                var uploadPath = Path.Combine(_environment.WebRootPath, "images");
                Directory.CreateDirectory(uploadPath);

                var extension = Path.GetExtension(imageFile.Name);
                var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                await using var fileStream = new FileStream(filePath, FileMode.Create);
                await using var stream = imageFile.OpenReadStream(MaxFileSize);
                await stream.CopyToAsync(fileStream);

                return $"/images/{fileName}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save image: {ex.Message}");
            }
        }

        private void DeleteImage(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            try
            {
                var fullPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning - Failed to delete image: {ex.Message}");
            }
        }

        // Add a new product
        public async Task<Product> AddProductAsync(Product product, IBrowserFile? imageFile)
        {
            string? imagePath = null;
            
            try
            {
                if (imageFile != null)
                {
                    if (!IsImageValid(imageFile))
                    {
                        throw new Exception("Invalid image file. Please use JPG or PNG files under 10MB.");
                    }
                    imagePath = await SaveImageAsync(imageFile);
                    product.Hinhanh = imagePath;
                }

                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                
                return product;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(imagePath))
                {
                    DeleteImage(imagePath);
                }
                throw new Exception($"Failed to add product: {ex.Message}");
            }
        }

        // Update an existing product
        public async Task UpdateProductAsync(Product product, IBrowserFile? imageFile)
        {
            var oldImagePath = product.Hinhanh;
            string? newImagePath = null;

            try
            {
                if (imageFile != null)
                {
                    if (!IsImageValid(imageFile))
                    {
                        throw new Exception("Invalid image file. Please use JPG or PNG files under 10MB.");
                    }
                    newImagePath = await SaveImageAsync(imageFile);
                    product.Hinhanh = newImagePath;
                }

                _context.Entry(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(oldImagePath) && !string.IsNullOrEmpty(newImagePath))
                {
                    DeleteImage(oldImagePath);
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(newImagePath))
                {
                    DeleteImage(newImagePath);
                    product.Hinhanh = oldImagePath;
                }
                throw new Exception($"Failed to update product: {ex.Message}");
            }
        }

        // Delete a product
        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                try
                {
                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();
                    DeleteImage(product.Hinhanh);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to delete product: {ex.Message}");
                }
            }
        }
    }
}
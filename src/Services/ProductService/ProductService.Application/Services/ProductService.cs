using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Repositories;

namespace ProductService.Application.Services;
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        var productDtos = new List<ProductDto>();

        foreach (var product in products)
        {
            productDtos.Add(MapToDto(product));
        }

        return productDtos;
    }

    public async Task<ProductDto> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return null;

        return MapToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> FilterProductsAsync(ProductFilterDto filterDto)
    {
        var criteria = new ProductFilterCriteria
        {
            Name = filterDto.Name,
            Category = filterDto.Category,
            MinPrice = filterDto.MinPrice,
            MaxPrice = filterDto.MaxPrice,
            MinStock = filterDto.MinStock,
            MaxStock = filterDto.MaxStock
        };

        var products = await _productRepository.FilterAsync(criteria);
        var productDtos = new List<ProductDto>();

        foreach (var product in products)
        {
            productDtos.Add(MapToDto(product));
        }

        return productDtos;
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto productDto)
    {
        var product = new Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            Category = productDto.Category,
            ImageUrl = productDto.ImageUrl,
            Price = productDto.Price,
            Stock = productDto.Stock
        };

        var id = await _productRepository.CreateAsync(product);
        product.Id = id;

        return MapToDto(product);
    }

    public async Task<bool> UpdateProductAsync(UpdateProductDto productDto)
    {
        var existingProduct = await _productRepository.GetByIdAsync(productDto.Id);
        if (existingProduct == null)
            return false;

        var product = new Product
        {
            Id = productDto.Id,
            Name = productDto.Name,
            Description = productDto.Description,
            Category = productDto.Category,
            ImageUrl = productDto.ImageUrl,
            Price = productDto.Price,
            Stock = productDto.Stock
        };

        return await _productRepository.UpdateAsync(product);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        return await _productRepository.DeleteAsync(id);
    }

    public async Task<bool> UpdateProductStockAsync(int id, int quantityChange)
    {
        return await _productRepository.UpdateStockAsync(id, quantityChange);
    }

    private ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            ImageUrl = product.ImageUrl,
            Price = product.Price,
            Stock = product.Stock
        };
    }
}


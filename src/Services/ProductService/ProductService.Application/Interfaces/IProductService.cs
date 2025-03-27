using ProductService.Application.DTOs;

namespace ProductService.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();

    Task<ProductDto> GetProductByIdAsync(int id);

    Task<IEnumerable<ProductDto>> FilterProductsAsync(ProductFilterDto filterDto);

    Task<ProductDto> CreateProductAsync(CreateProductDto productDto);

    Task<bool> UpdateProductAsync(UpdateProductDto productDto);

    Task<bool> DeleteProductAsync(int id);

    Task<bool> UpdateProductStockAsync(int id, int quantityChange);
}

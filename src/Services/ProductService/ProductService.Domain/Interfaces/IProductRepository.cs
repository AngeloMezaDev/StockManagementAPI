using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;

namespace ProductService.Domain.Interfaces;
public interface IProductRepository
{
    Task<IEnumerable<Entities.Product>> GetAllAsync();

    Task<Entities.Product> GetByIdAsync(int id);

    Task<IEnumerable<Entities.Product>> FilterAsync(ProductFilterCriteria criteria);

    Task<int> CreateAsync(Product product);

    Task<bool> UpdateAsync(Product product);

    Task<bool> DeleteAsync(int id);

    Task<bool> UpdateStockAsync(int id, int quantityChange);
}


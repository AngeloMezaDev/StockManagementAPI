using TransactionService.Application.DTOs;

namespace TransactionService.Application.Interfaces;
public interface IProductServiceClient
{
    Task<ProductInfoDto> GetProductAsync(int productId);

    Task<bool> UpdateProductStockAsync(int productId, int quantityChange);
}


using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProductService.Domain.Interfaces;

namespace ProductService.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(IConfiguration configuration, ILogger<ProductRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("Key 'DefaultConnection' not found");
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    return await connection.QueryAsync<Product>("SELECT * FROM Products");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all products");
                throw;
            }
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var response = await connection.QueryFirstOrDefaultAsync<Product>(
                        "SELECT * FROM Products WHERE Id = @Id", new { Id = id });
                    // Cambiado: simplemente devolver null en lugar de lanzar excepción
                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID {ProductId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Product>> FilterAsync(ProductFilterCriteria criteria)
        {
            try
            {
                var query = "SELECT * FROM Products WHERE 1=1";
                var parameters = new DynamicParameters();

                if (!string.IsNullOrEmpty(criteria.Name))
                {
                    query += " AND Name LIKE @Name";
                    parameters.Add("Name", $"%{criteria.Name}%");
                }

                if (!string.IsNullOrEmpty(criteria.Category))
                {
                    query += " AND Category = @Category";
                    parameters.Add("Category", criteria.Category);
                }

                if (criteria.MinPrice.HasValue)
                {
                    query += " AND Price >= @MinPrice";
                    parameters.Add("MinPrice", criteria.MinPrice.Value);
                }

                if (criteria.MaxPrice.HasValue)
                {
                    query += " AND Price <= @MaxPrice";
                    parameters.Add("MaxPrice", criteria.MaxPrice.Value);
                }

                if (criteria.MinStock.HasValue)
                {
                    query += " AND Stock >= @MinStock";
                    parameters.Add("MinStock", criteria.MinStock.Value);
                }

                if (criteria.MaxStock.HasValue)
                {
                    query += " AND Stock <= @MaxStock";
                    parameters.Add("MaxStock", criteria.MaxStock.Value);
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    return await connection.QueryAsync<Product>(query, parameters);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering products");
                throw;
            }
        }

        public async Task<int> CreateAsync(Product product)
        {
            try
            {
                var query = @"
                    INSERT INTO Products (Name, Description, Category, ImageUrl, Price, Stock)
                    VALUES (@Name, @Description, @Category, @ImageUrl, @Price, @Stock);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    return await connection.QuerySingleAsync<int>(query, product);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product {ProductName}", product.Name);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            try
            {
                var query = @"
                    UPDATE Products 
                    SET Name = @Name, 
                        Description = @Description, 
                        Category = @Category, 
                        ImageUrl = @ImageUrl, 
                        Price = @Price, 
                        Stock = @Stock
                    WHERE Id = @Id";

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var result = await connection.ExecuteAsync(query, product);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID {ProductId}", product.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Primero verifica si el producto tiene transacciones relacionadas
                    var checkQuery = "SELECT COUNT(1) FROM Transactions WHERE ProductId = @Id";
                    var hasTransactions = await connection.ExecuteScalarAsync<int>(checkQuery, new { Id = id }) > 0;

                    if (hasTransactions)
                    {
                        // TODO: Lo recomendable es añadir una columa de estado para eliminar el producto
                        // De momento Lanzar una excepción descriptiva
                        throw new InvalidOperationException($"Cannot delete product {id} because it has related transactions");
                    }

                    var deleteProductQuery = "DELETE FROM Products WHERE Id = @Id";
                    var result = await connection.ExecuteAsync(deleteProductQuery, new { Id = id });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el producto con ID {ProductId}", id);
                throw;
            }
        }

        public async Task<bool> UpdateStockAsync(int id, int quantityChange)
        {
            try
            {
                var query = @"
                    UPDATE Products 
                    SET Stock = Stock + @QuantityChange
                    WHERE Id = @Id AND (Stock + @QuantityChange) >= 0";

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var result = await connection.ExecuteAsync(query, new { Id = id, QuantityChange = quantityChange });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock for product with ID {ProductId}", id);
                throw;
            }
        }
    }
}
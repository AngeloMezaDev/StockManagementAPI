using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Exceptions;
using TransactionService.Domain.Interfaces;
using TransactionService.Domain.Repositories;

namespace TransactionService.Infrastructure.Repositories;
public class TransactionRepository : ITransactionRepository
{
    private readonly string _connectionString;
    private readonly ILogger<TransactionRepository> _logger;

    public TransactionRepository(IConfiguration configuration, ILogger<TransactionRepository> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("Key 'DefaultConnection' not found");
        _logger = logger;
    }

    public async Task<IEnumerable<Transaction>> GetAllAsync()
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<Transaction>("SELECT * FROM Transactions ORDER BY Date DESC");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all transactions");
            throw;
        }
    }

    public async Task<Transaction> GetByIdAsync(int id)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var response = await connection.QueryFirstOrDefaultAsync<Transaction>(
                    "SELECT * FROM Transactions WHERE Id = @Id", new { Id = id });

                // Simply return null when no transaction is found
                return response;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database error retrieving transaction with ID {TransactionId}", id);
            throw new DatabaseException($"Error retrieving transaction with ID {id}", ex);
        }
    }

    public async Task<IEnumerable<Transaction>> FilterAsync(TransactionFilterCriteria criteria)
    {
        try
        {
            var query = "SELECT * FROM Transactions WHERE 1=1";
            var parameters = new DynamicParameters();

            if (criteria.StartDate.HasValue)
            {
                query += " AND Date >= @StartDate";
                parameters.Add("StartDate", criteria.StartDate.Value);
            }

            if (criteria.EndDate.HasValue)
            {
                query += " AND Date <= @EndDate";
                parameters.Add("EndDate", criteria.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(criteria.TransactionType))
            {
                query += " AND Type = @TransactionType";
                parameters.Add("TransactionType", criteria.TransactionType);
            }

            if (criteria.ProductId.HasValue)
            {
                query += " AND ProductId = @ProductId";
                parameters.Add("ProductId", criteria.ProductId.Value);
            }

            query += " ORDER BY Date DESC";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<Transaction>(query, parameters);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering transactions");
            throw;
        }
    }

    public async Task<IEnumerable<Transaction>> GetByProductIdAsync(int productId)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<Transaction>(
                    "SELECT * FROM Transactions WHERE ProductId = @ProductId ORDER BY Date DESC",
                    new { ProductId = productId });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions for product with ID {ProductId}", productId);
            throw;
        }
    }

    public async Task<int> CreateAsync(Transaction transaction)
    {
        try
        {
            var query = @"
                INSERT INTO Transactions (Date, Type, ProductId, Quantity, UnitPrice, TotalPrice, Details)
                VALUES (@Date, @Type, @ProductId, @Quantity, @UnitPrice, @TotalPrice, @Details);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QuerySingleAsync<int>(query, transaction);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transaction for product ID {ProductId}", transaction.ProductId);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(Transaction transaction)
    {
        try
        {
            var query = @"
                UPDATE Transactions 
                SET Date = @Date, 
                    Type = @Type, 
                    ProductId = @ProductId, 
                    Quantity = @Quantity, 
                    UnitPrice = @UnitPrice, 
                    TotalPrice = @TotalPrice, 
                    Details = @Details
                WHERE Id = @Id";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.ExecuteAsync(query, transaction);
                return result > 0;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating transaction with ID {TransactionId}", transaction.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var query = "DELETE FROM Transactions WHERE Id = @Id";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.ExecuteAsync(query, new { Id = id });
                return result > 0;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting transaction with ID {TransactionId}", id);
            throw;
        }
    }
}
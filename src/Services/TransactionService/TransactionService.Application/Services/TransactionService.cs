using TransactionService.Application.DTOs;
using TransactionService.Application.Interfaces;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransactionService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace TransactionService.Application.Services;
public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IProductServiceClient _productServiceClient;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(
        ITransactionRepository transactionRepository,
        IProductServiceClient productServiceClient,
        ILogger<TransactionService> logger)
    {
        _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        _productServiceClient = productServiceClient ?? throw new ArgumentNullException(nameof(productServiceClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync()
    {
        try
        {
            var transactions = await _transactionRepository.GetAllAsync();
            var transactionDtos = new List<TransactionDto>();

            foreach (var transaction in transactions)
            {
                if (transaction == null) continue;

                try
                {
                    var productInfo = await _productServiceClient.GetProductAsync(transaction.ProductId);
                    var transactionDto = MapToDto(transaction);
                    transactionDto.ProductName = productInfo?.Name ?? string.Empty;
                    transactionDtos.Add(transactionDto);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving product info for transaction ID {TransactionId}", transaction.Id);
                    var transactionDto = MapToDto(transaction);
                    transactionDto.ProductName = "[Product info unavailable]";
                    transactionDtos.Add(transactionDto);
                }
            }

            return transactionDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all transactions");
            throw;
        }
    }

    public async Task<TransactionDto> GetTransactionByIdAsync(int id)
    {
        try
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null)
                return null;

            try
            {
                var productInfo = await _productServiceClient.GetProductAsync(transaction.ProductId);
                var transactionDto = MapToDto(transaction);
                transactionDto.ProductName = productInfo?.Name ?? string.Empty;
                return transactionDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product info for transaction ID {TransactionId}", id);
                var transactionDto = MapToDto(transaction);
                transactionDto.ProductName = "[Product info unavailable]";
                return transactionDto;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transaction with ID {TransactionId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<TransactionDto>> FilterTransactionsAsync(TransactionFilterDto filterDto)
    {
        if (filterDto == null)
            throw new ArgumentNullException(nameof(filterDto));

        try
        {
            var criteria = new TransactionFilterCriteria
            {
                StartDate = filterDto.StartDate,
                EndDate = filterDto.EndDate,
                TransactionType = filterDto.TransactionType,
                ProductId = filterDto.ProductId
            };

            var transactions = await _transactionRepository.FilterAsync(criteria);
            var transactionDtos = new List<TransactionDto>();

            foreach (var transaction in transactions)
            {
                if (transaction == null) continue;

                try
                {
                    var productInfo = await _productServiceClient.GetProductAsync(transaction.ProductId);
                    var transactionDto = MapToDto(transaction);
                    transactionDto.ProductName = productInfo?.Name ?? string.Empty;
                    transactionDtos.Add(transactionDto);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving product info for transaction ID {TransactionId}", transaction.Id);
                    var transactionDto = MapToDto(transaction);
                    transactionDto.ProductName = "[Product info unavailable]";
                    transactionDtos.Add(transactionDto);
                }
            }

            return transactionDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering transactions with criteria: {Criteria}",
                System.Text.Json.JsonSerializer.Serialize(filterDto));
            throw;
        }
    }

    public async Task<IEnumerable<TransactionDto>> GetTransactionsByProductIdAsync(int productId)
    {
        try
        {
            var transactions = await _transactionRepository.GetByProductIdAsync(productId);
            var transactionDtos = new List<TransactionDto>();

            string productName = string.Empty;
            try
            {
                var productInfo = await _productServiceClient.GetProductAsync(productId);
                productName = productInfo?.Name ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product info for product ID {ProductId}", productId);
                productName = "[Product info unavailable]";
            }

            foreach (var transaction in transactions)
            {
                if (transaction == null) continue;

                var transactionDto = MapToDto(transaction);
                transactionDto.ProductName = productName;
                transactionDtos.Add(transactionDto);
            }

            return transactionDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions for product ID {ProductId}", productId);
            throw;
        }
    }

    public async Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto transactionDto)
    {
        if (transactionDto == null)
            throw new ArgumentNullException(nameof(transactionDto));

        // Validate transaction type before proceeding
        if (transactionDto.Type != TransactionTypeDto.Purchase && transactionDto.Type != TransactionTypeDto.Sale)
        {
            throw new ArgumentException("Transaction type must be either 'Purchase' or 'Sale'.", nameof(transactionDto.Type));
        }

        try
        {
            // Get product details
            ProductInfoDto productInfo;
            try
            {
                productInfo = await _productServiceClient.GetProductAsync(transactionDto.ProductId);
                if (productInfo == null)
                    throw new Exception($"Product with ID {transactionDto.ProductId} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID {ProductId}", transactionDto.ProductId);
                throw new Exception($"Failed to retrieve product information: {ex.Message}", ex);
            }

            // If it's a sale, verify stock
            if (transactionDto.Type == TransactionTypeDto.Sale)
            {
                if (productInfo.Stock < transactionDto.Quantity)
                    throw new Exception($"Insufficient stock. Available: {productInfo.Stock}, Requested: {transactionDto.Quantity}");
            }

            // Create transaction
            var transaction = new Transaction
            {
                Date = DateTime.Now,
                Type = (TransactionType)transactionDto.Type,
                ProductId = transactionDto.ProductId,
                Quantity = transactionDto.Quantity,
                UnitPrice = transactionDto.UnitPrice,
                TotalPrice = transactionDto.Quantity * transactionDto.UnitPrice,
                Details = transactionDto.Details ?? string.Empty
            };

            int id;
            try
            {
                id = await _transactionRepository.CreateAsync(transaction);
                transaction.Id = id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction for product {ProductId}", transactionDto.ProductId);
                throw new Exception("Failed to create transaction in database", ex);
            }

            // Update product stock
            try
            {
                int quantityChange = transactionDto.Type == TransactionTypeDto.Purchase
                    ? transactionDto.Quantity
                    : -transactionDto.Quantity;
                bool stockUpdateResult = await _productServiceClient.UpdateProductStockAsync(
                    transactionDto.ProductId, quantityChange);
                if (!stockUpdateResult)
                {
                    // Try to rollback transaction if stock update fails
                    try
                    {
                        await _transactionRepository.DeleteAsync(id);
                    }
                    catch (Exception deleteEx)
                    {
                        _logger.LogError(deleteEx, "Failed to rollback transaction {TransactionId} after stock update failure", id);
                    }
                    throw new Exception("Failed to update product stock");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock for product {ProductId}", transactionDto.ProductId);
                // Try to rollback transaction
                try
                {
                    await _transactionRepository.DeleteAsync(id);
                }
                catch (Exception deleteEx)
                {
                    _logger.LogError(deleteEx, "Failed to rollback transaction {TransactionId} after stock update error", id);
                }
                throw new Exception("Failed to update product stock", ex);
            }

            var result = MapToDto(transaction);
            result.ProductName = productInfo.Name ?? string.Empty;
            return result;
        }
        catch (Exception ex) when (!(ex is ArgumentNullException) && !(ex is ArgumentException))
        {
            _logger.LogError(ex, "Error creating transaction");
            throw;
        }
    }

    public async Task<bool> UpdateTransactionAsync(UpdateTransactionDto transactionDto)
    {
        if (transactionDto == null)
            throw new ArgumentNullException(nameof(transactionDto));

        // Validate transaction type before proceeding
        if (transactionDto.Type != TransactionTypeDto.Purchase && transactionDto.Type != TransactionTypeDto.Sale)
        {
            throw new ArgumentException("Transaction type must be either 'Purchase' or 'Sale'.", nameof(transactionDto.Type));
        }

        try
        {
            Transaction existingTransaction;
            try
            {
                existingTransaction = await _transactionRepository.GetByIdAsync(transactionDto.Id);
                if (existingTransaction == null)
                    return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving existing transaction {TransactionId}", transactionDto.Id);
                throw new Exception($"Failed to retrieve existing transaction: {ex.Message}", ex);
            }

            // Calculate the stock change that needs to be reverted
            int oldQuantityChange = existingTransaction.Type == TransactionType.Purchase
                ? existingTransaction.Quantity
                : -existingTransaction.Quantity;

            // Calculate the new stock change
            int newQuantityChange = transactionDto.Type == TransactionTypeDto.Purchase
                ? transactionDto.Quantity
                : -transactionDto.Quantity;

            try
            {
                // If the product ID has changed, revert the old product's stock and update the new one
                if (existingTransaction.ProductId != transactionDto.ProductId)
                {
                    // Revert the stock change for the old product
                    await _productServiceClient.UpdateProductStockAsync(
                        existingTransaction.ProductId, -oldQuantityChange);

                    // Apply the stock change to the new product
                    var productInfo = await _productServiceClient.GetProductAsync(transactionDto.ProductId);
                    if (productInfo == null)
                        return false;

                    // For a sale, verify the new product has enough stock
                    if (transactionDto.Type == TransactionTypeDto.Sale && productInfo.Stock < transactionDto.Quantity)
                        return false;

                    await _productServiceClient.UpdateProductStockAsync(
                        transactionDto.ProductId, newQuantityChange);
                }
                else
                {
                    // Same product, just update the difference in quantity
                    int netStockChange = newQuantityChange - oldQuantityChange;

                    // If it's a sale and we're increasing the quantity, check stock
                    if (existingTransaction.Type == TransactionType.Sale && netStockChange < 0)
                    {
                        var productInfo = await _productServiceClient.GetProductAsync(transactionDto.ProductId);
                        if (productInfo == null || productInfo.Stock < Math.Abs(netStockChange))
                            return false;
                    }

                    await _productServiceClient.UpdateProductStockAsync(
                        transactionDto.ProductId, netStockChange);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product stock during transaction update {TransactionId}", transactionDto.Id);
                throw new Exception("Failed to update product stock", ex);
            }

            var transaction = new Transaction
            {
                Id = transactionDto.Id,
                Date = transactionDto.Date,
                Type = (TransactionType)transactionDto.Type, 
                ProductId = transactionDto.ProductId,
                Quantity = transactionDto.Quantity,
                UnitPrice = transactionDto.UnitPrice,
                TotalPrice = transactionDto.Quantity * transactionDto.UnitPrice,
                Details = transactionDto.Details ?? string.Empty
            };

            try
            {
                return await _transactionRepository.UpdateAsync(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating transaction {TransactionId} in database", transactionDto.Id);

                // Try to revert stock changes
                try
                {
                    if (existingTransaction.ProductId != transactionDto.ProductId)
                    {
                        // Revert the new product's stock update
                        await _productServiceClient.UpdateProductStockAsync(
                            transactionDto.ProductId, -newQuantityChange);

                        // Restore the old product's stock
                        await _productServiceClient.UpdateProductStockAsync(
                            existingTransaction.ProductId, oldQuantityChange);
                    }
                    else
                    {
                        // Revert the stock change
                        int netStockChange = newQuantityChange - oldQuantityChange;
                        await _productServiceClient.UpdateProductStockAsync(
                            transactionDto.ProductId, -netStockChange);
                    }
                }
                catch (Exception revertEx)
                {
                    _logger.LogError(revertEx, "Failed to revert stock changes after transaction update failure {TransactionId}", transactionDto.Id);
                }

                throw new Exception("Failed to update transaction in database", ex);
            }
        }
        catch (Exception ex) when (!(ex is ArgumentNullException) && !(ex is ArgumentException))
        {
            _logger.LogError(ex, "Error updating transaction {TransactionId}", transactionDto?.Id);
            throw;
        }
    }
    public async Task<bool> DeleteTransactionAsync(int id)
    {
        try
        {
            Transaction existingTransaction;
            try
            {
                existingTransaction = await _transactionRepository.GetByIdAsync(id);
                if (existingTransaction == null)
                    return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transaction {TransactionId} for deletion", id);
                throw new Exception($"Failed to retrieve transaction: {ex.Message}", ex);
            }

            // Revert the stock change
            int quantityChange = existingTransaction.Type == TransactionType.Purchase
                ? -existingTransaction.Quantity
                : existingTransaction.Quantity;

            try
            {
                await _productServiceClient.UpdateProductStockAsync(
                    existingTransaction.ProductId, quantityChange);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reverting stock changes for product {ProductId} when deleting transaction {TransactionId}",
                    existingTransaction.ProductId, id);
                throw new Exception("Failed to revert product stock", ex);
            }

            try
            {
                return await _transactionRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting transaction {TransactionId} from database", id);

                // Try to restore stock changes
                try
                {
                    await _productServiceClient.UpdateProductStockAsync(
                        existingTransaction.ProductId, -quantityChange);
                }
                catch (Exception revertEx)
                {
                    _logger.LogError(revertEx, "Failed to restore stock changes after transaction deletion failure {TransactionId}", id);
                }

                throw new Exception("Failed to delete transaction from database", ex);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting transaction {TransactionId}", id);
            throw;
        }
    }

    private TransactionDto MapToDto(Transaction transaction)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction));

        try
        {
            return new TransactionDto
            {
                Id = transaction.Id,
                Date = transaction.Date,
                Type = (TransactionTypeDto)transaction.Type,
                ProductId = transaction.ProductId,
                ProductName = string.Empty, // Initialize with empty string instead of null
                Quantity = transaction.Quantity,
                UnitPrice = transaction.UnitPrice,
                TotalPrice = transaction.TotalPrice,
                Details = transaction.Details ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error mapping transaction {TransactionId} to DTO", transaction.Id);
            throw new Exception("Failed to map transaction to DTO", ex);
        }
    }
}
using TransactionService.Domain.Entities;
using TransactionService.Domain.Repositories;

namespace TransactionService.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetAllAsync();

    Task<Transaction> GetByIdAsync(int id);

    Task<IEnumerable<Transaction>> FilterAsync(TransactionFilterCriteria criteria);

    Task<IEnumerable<Transaction>> GetByProductIdAsync(int productId);

    Task<int> CreateAsync(Transaction transaction);

    Task<bool> UpdateAsync(Transaction transaction);

    Task<bool> DeleteAsync(int id);
}


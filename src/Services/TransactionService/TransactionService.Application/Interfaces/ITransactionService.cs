using TransactionService.Application.DTOs;

namespace TransactionService.Application.Interfaces;
public interface ITransactionService
{
    Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync();

    Task<TransactionDto> GetTransactionByIdAsync(int id);

    Task<IEnumerable<TransactionDto>> FilterTransactionsAsync(TransactionFilterDto filterDto);

    Task<IEnumerable<TransactionDto>> GetTransactionsByProductIdAsync(int productId);

    Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto transactionDto);

    Task<bool> UpdateTransactionAsync(UpdateTransactionDto transactionDto);

    Task<bool> DeleteTransactionAsync(int id);
}


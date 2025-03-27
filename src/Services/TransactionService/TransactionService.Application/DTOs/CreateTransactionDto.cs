namespace TransactionService.Application.DTOs;
public class CreateTransactionDto
{
    public TransactionTypeDto Type { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public string Details { get; set; } = string.Empty;
}


namespace TransactionService.Application.DTOs;
public class UpdateTransactionDto
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public TransactionTypeDto Type { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public string Details { get; set; } = string.Empty;
}


namespace TransactionService.Application.DTOs;
public class TransactionDto
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public TransactionTypeDto Type { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public string Details { get; set; } = string.Empty;
}

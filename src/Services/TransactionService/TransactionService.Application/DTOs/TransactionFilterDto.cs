namespace TransactionService.Application.DTOs;
public class TransactionFilterDto
{
    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string TransactionType { get; set; } = string.Empty;

    public int? ProductId { get; set; }
}


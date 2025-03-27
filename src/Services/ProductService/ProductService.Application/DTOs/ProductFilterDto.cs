namespace ProductService.Application.DTOs;
public class ProductFilterDto
{
    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public int? MinStock { get; set; }

    public int? MaxStock { get; set; }
}

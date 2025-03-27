namespace ProductService.Domain.Repositories;
public class ProductFilterCriteria
{
    public string Name { get; set; }

    public string Category { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public int? MinStock { get; set; }

    public int? MaxStock { get; set; }
}

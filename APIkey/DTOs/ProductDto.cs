namespace APIkey.DTOs;

public class ProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal? UnitPrice { get; set; }
    public int? UnitsInStock { get; set; }
}

public class ProductsSummaryDto
{
    public int TotalProducts { get; set; }
    public List<ProductDto> Products { get; set; } = new();
}

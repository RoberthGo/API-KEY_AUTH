using System.ComponentModel.DataAnnotations;

namespace APIkey.DTOs;

public class ProductUpsertDto
{
    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    [StringLength(40, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 40 caracteres.")]
    public string ProductName { get; set; } = string.Empty;

    [Range(0.01, 100000.00, ErrorMessage = "El precio unitario debe estar entre 0.01 y 100,000.00.")]
    public decimal? UnitPrice { get; set; }

    [Range(0, 32767, ErrorMessage = "El stock debe ser un número entero mayor o igual a 0.")]
    public int? UnitsInStock { get; set; }

    public bool Discontinued { get; set; } = false;
}

﻿namespace ProductService.Application.DTOs;
public class UpdateProductDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Stock { get; set; }
}


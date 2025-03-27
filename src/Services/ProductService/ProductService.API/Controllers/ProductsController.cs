using Microsoft.AspNetCore.Mvc;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound(new { Message = $"Product with ID {id} not found." });

            return Ok(product);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> Filter([FromQuery] ProductFilterDto filter)
        {
            var products = await _productService.FilterProductsAsync(filter);
            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdProduct = await _productService.CreateProductAsync(productDto);
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto productDto)
        {
            if (id != productDto.Id)
                return BadRequest(new { Message = "ID in URL does not match ID in request body." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.UpdateProductAsync(productDto);
            if (!result)
                return NotFound(new { Message = $"Product with ID {id} not found." });

            return Ok(new { Message = "Product updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
                return NotFound(new { Message = $"Product with ID {id} not found." });

            return Ok(new { Message = "Product deleted successfully." });
        }

        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] ProductStockUpdateDto updateDto)
        {
            if (id != updateDto.ProductId)
                return BadRequest(new { Message = "ID in URL does not match ID in request body." });

            var result = await _productService.UpdateProductStockAsync(id, updateDto.Quantity);
            if (!result)
                return NotFound(new { Message = $"Product with ID {id} not found or stock update would result in negative stock." });

            return Ok(new { Message = "Product stock updated successfully." });
        }
    }
}
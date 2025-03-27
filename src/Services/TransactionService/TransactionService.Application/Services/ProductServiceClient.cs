using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TransactionService.Application.DTOs;
using TransactionService.Application.Interfaces;

namespace TransactionService.Application.Services
{
    public class ProductServiceClient : IProductServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly ILogger<ProductServiceClient> _logger;

        public ProductServiceClient(HttpClient httpClient, IConfiguration configuration, ILogger<ProductServiceClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _baseUrl = configuration["ProductServiceUrl"]
                ?? throw new InvalidOperationException("Not found the key 'ProductServiceUrl'.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ProductInfoDto> GetProductAsync(int productId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/Products/{productId}");
                _logger.LogInformation($"baseUrl: {_baseUrl}");
                if (response.IsSuccessStatusCode)
                {
                    var responseService = await response.Content.ReadFromJsonAsync<ProductInfoDto>();
                    if (responseService == null)
                    {
                        _logger.LogWarning("Product service returned null for product ID {ProductId}", productId);
                        return new ProductInfoDto();
                    }
                    return responseService;
                }

                _logger.LogWarning("Product service returned status code {StatusCode} for product ID {ProductId}",
                    (int)response.StatusCode, productId);
                return new ProductInfoDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product with ID {ProductId}", productId);
                throw;
            }
        }

        public async Task<bool> UpdateProductStockAsync(int productId, int quantityChange)
        {
            try
            {
                var updateDto = new
                {
                    ProductId = productId,
                    Quantity = quantityChange
                };
                _logger.LogInformation($"baseUrl: {_baseUrl}");

                var response = await _httpClient.PatchAsJsonAsync(
                    $"{_baseUrl}/api/Products/{productId}/stock", updateDto);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to update stock for product ID {ProductId}. Status code: {StatusCode}",
                        productId, (int)response.StatusCode);
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock for product with ID {ProductId}", productId);
                throw;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kitbox_app.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://kitbox.msrl.be"; // 🔄 URL de production
        // Pour le développement local, utilisez : "https://localhost:7156"

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        // 🔧 Méthodes pour consommer votre API Kitbox

        // GET tous les cabinets
        public async Task<List<CabinetDto>?> GetCabinetsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/cabinets");
                response.EnsureSuccessStatusCode();
                
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<CabinetDto>>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des cabinets : {ex.Message}");
                return null;
            }
        }

        // GET tous les lockers
        public async Task<List<LockerDto>?> GetLockersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/lockers");
                response.EnsureSuccessStatusCode();
                
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<LockerDto>>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des lockers : {ex.Message}");
                return null;
            }
        }

        // GET commande complète
        public async Task<CustomerOrderFullDto?> GetFullOrderAsync(int orderId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/customerorders/{orderId}/full");
                response.EnsureSuccessStatusCode();
                
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<CustomerOrderFullDto>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération de la commande {orderId} : {ex.Message}");
                return null;
            }
        }

        // POST nouvelle commande
        public async Task<CustomerOrder?> CreateOrderAsync(CustomerOrderCreateDto order)
        {
            try
            {
                var json = JsonSerializer.Serialize(order);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("/api/customerorders", content);
                response.EnsureSuccessStatusCode();
                
                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<CustomerOrder>(responseJson, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la création de la commande : {ex.Message}");
                return null;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    // 📦 DTOs correspondants à votre API
    public class CabinetDto
    {
        public int Id { get; set; }
        public int IdOrder { get; set; }
        public decimal Price { get; set; }
        public string? Dimensions { get; set; }
        public string? Reference { get; set; }
        public List<LockerDto> Lockers { get; set; } = new();
    }

    public class LockerDto
    {
        public int Id { get; set; }
        public string? Reference { get; set; }
        public string? Code { get; set; }
        public string? Dimensions { get; set; }
        public List<LockerStockDto> LockerStocks { get; set; } = new();
    }

    public class LockerStockDto
    {
        public int Id { get; set; }
        public int QuantityNeeded { get; set; }
        public StockDto Stock { get; set; } = null!;
    }

    public class StockDto
    {
        public int Id { get; set; }
        public string? Reference { get; set; }
        public string? Code { get; set; }
        public string? Dimensions { get; set; }
        public int Quantity { get; set; }
        public StockStatus Status { get; set; } // Utilise l'enum de votre API
        public string? Location { get; set; }
        public SupplierDto Supplier { get; set; } = null!;
    }

    public class SupplierDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Contact { get; set; }
        public string? Address { get; set; }
    }

    public class CustomerOrderFullDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal DepositAmount { get; set; }
        public string? Tel { get; set; }
        public string? Mail { get; set; }
        public List<CabinetDto> Cabinets { get; set; } = new();
    }

    public class CustomerOrderCreateDto
    {
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal DepositAmount { get; set; }
        public string? Tel { get; set; }
        public string? Mail { get; set; }
    }

    public class CustomerOrder
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal DepositAmount { get; set; }
        public string? Tel { get; set; }
        public string? Mail { get; set; }
    }

    // 🔹 Enum StockStatus (copié depuis votre API)
    public enum StockStatus
    {
        AVAILABLE,    // disponible
        DELIVERY,     // en_livraison
        OUT_OF_STOCK  // rupture
    }
}
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using KitBoxDesigner.Models;

namespace KitBoxDesigner.Services
{
    public class KitboxApiService : IStockService, IPartService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://kitbox.msrl.be";

        public KitboxApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        // 📊 Implémentation IStockService
        public async Task<List<StockItem>> GetAllStockItemsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/stocks");
                response.EnsureSuccessStatusCode();
                
                var json = await response.Content.ReadAsStringAsync();
                var apiStocks = JsonSerializer.Deserialize<List<ApiStockDto>>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                return apiStocks?.Select(ConvertToStockItem).ToList() ?? new List<StockItem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetAllStockItemsAsync : {ex.Message}");
                return new List<StockItem>();
            }
        }

        public async Task<StockItem?> GetStockItemAsync(string partCode)
        {
            try
            {
                var allStocks = await GetAllStockItemsAsync();
                return allStocks.FirstOrDefault(s => s.Part.Code == partCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetStockItemAsync : {ex.Message}");
                return null;
            }
        }

        public async Task<StockItem?> GetStockItemByCodeAsync(string partCode)
        {
            return await GetStockItemAsync(partCode);
        }

        public async Task<List<StockItem>> GetLowStockItemsAsync()
        {
            var allStocks = await GetAllStockItemsAsync();
            return allStocks.Where(s => s.CurrentStock > 0 && s.CurrentStock < s.ReorderPoint).ToList();
        }

        public async Task<List<StockItem>> GetOutOfStockItemsAsync()
        {
            var allStocks = await GetAllStockItemsAsync();
            return allStocks.Where(s => s.CurrentStock == 0).ToList();
        }

        public async Task<List<StockItem>> GetItemsNeedingReorderAsync()
        {
            var allStocks = await GetAllStockItemsAsync();
            return allStocks.Where(s => s.CurrentStock <= s.ReorderPoint).ToList();
        }

        public async Task<bool> UpdateStockAsync(string partCode, int newQuantity, string reason = "Manual Update")
        {
            try
            {
                var updateDto = new { quantity = newQuantity, reason };
                var json = JsonSerializer.Serialize(updateDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"/api/stocks/code/{partCode}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur UpdateStockAsync : {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AddStockAsync(string partCode, int quantity, string reason = "Delivery")
        {
            var stockItem = await GetStockItemAsync(partCode);
            if (stockItem == null) return false;
            
            return await UpdateStockAsync(partCode, stockItem.CurrentStock + quantity, reason);
        }

        public async Task<bool> RemoveStockAsync(string partCode, int quantity, string reason = "Sale")
        {
            var stockItem = await GetStockItemAsync(partCode);
            if (stockItem == null) return false;
            
            var newQuantity = Math.Max(0, stockItem.CurrentStock - quantity);
            return await UpdateStockAsync(partCode, newQuantity, reason);
        }

        public async Task<bool> ReserveStockAsync(string partCode, int quantity)
        {
            // TODO: Implémenter selon votre logique de réservation
            return await RemoveStockAsync(partCode, quantity, "Reserved");
        }

        public async Task<bool> ReleaseReservedStockAsync(string partCode, int quantity)
        {
            // TODO: Implémenter selon votre logique de libération
            return await AddStockAsync(partCode, quantity, "Released");
        }

        public async Task<Dictionary<string, StockStatus>> CheckStockAvailabilityAsync(List<PartRequirement> requirements)
        {
            var result = new Dictionary<string, StockStatus>();
            
            foreach (var req in requirements)
            {
                var stock = await GetStockItemAsync(req.PartCode);
                if (stock != null)
                {
                    if (stock.AvailableStock >= req.Quantity)
                        result[req.PartCode] = StockStatus.InStock;
                    else if (stock.AvailableStock > 0)
                        result[req.PartCode] = StockStatus.LowStock;
                    else
                        result[req.PartCode] = StockStatus.OutOfStock;
                }
                else
                {
                    result[req.PartCode] = StockStatus.OutOfStock;
                }
            }
            
            return result;
        }

        public async Task<List<StockMovement>> GetStockMovementsAsync(string partCode, int daysBack = 30)
        {
            // TODO: Implémenter si vous avez une API pour les mouvements
            await Task.Delay(10);
            return new List<StockMovement>();
        }

        public async Task<List<StockAlert>> GetStockAlertsAsync()
        {
            var stocks = await GetAllStockItemsAsync();
            var alerts = new List<StockAlert>();
            
            foreach (var stock in stocks.Where(s => s.CurrentStock < s.ReorderPoint))
            {
                alerts.Add(new StockAlert
                {
                    Id = Guid.NewGuid(),
                    PartCode = stock.Part.Code,
                    // Utiliser les propriétés disponibles dans StockAlert
                    Message = $"Stock faible pour {stock.Part.Reference}: {stock.CurrentStock} restant(s)"
                });
            }
            
            return alerts;
        }

        public async Task<Dictionary<string, DateTime>> GetEstimatedDeliveryDatesAsync(List<PartRequirement> requirements)
        {
            // TODO: Implémenter selon votre logique de livraison
            await Task.Delay(10);
            var result = new Dictionary<string, DateTime>();
            foreach (var req in requirements)
            {
                result[req.PartCode] = DateTime.Now.AddDays(7); // 7 jours par défaut
            }
            return result;
        }

        public async Task<StockSummary> GetStockSummaryAsync()
        {
            var stocks = await GetAllStockItemsAsync();
            
            return new StockSummary
            {
                TotalParts = stocks.Count,
                InStockParts = stocks.Count(s => s.CurrentStock > s.ReorderPoint),
                LowStockParts = stocks.Count(s => s.CurrentStock > 0 && s.CurrentStock <= s.ReorderPoint),
                OutOfStockParts = stocks.Count(s => s.CurrentStock == 0),
                PartsNeedingReorder = stocks.Count(s => s.CurrentStock <= s.ReorderPoint),
                TotalValue = stocks.Sum(s => s.CurrentStock * s.Part.Price),
                ActiveAlerts = stocks.Count(s => s.CurrentStock <= s.ReorderPoint)
            };
        }

        public async Task<bool> CheckAvailabilityAsync(string partCode, int requiredQuantity)
        {
            var stock = await GetStockItemAsync(partCode);
            return stock != null && stock.AvailableStock >= requiredQuantity;
        }

        // 🔧 Implémentation IPartService
        public async Task<List<Part>> GetAllPartsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/stocks");
                response.EnsureSuccessStatusCode();
                
                var json = await response.Content.ReadAsStringAsync();
                var apiStocks = JsonSerializer.Deserialize<List<ApiStockDto>>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                return apiStocks?.Select(ConvertToPart).ToList() ?? new List<Part>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetAllPartsAsync : {ex.Message}");
                return new List<Part>();
            }
        }

        public async Task<Part?> GetPartByCodeAsync(string code)
        {
            var allParts = await GetAllPartsAsync();
            return allParts.FirstOrDefault(p => p.Code == code);
        }

        public async Task<List<Part>> GetPartsByCategoryAsync(PartCategory category)
        {
            var allParts = await GetAllPartsAsync();
            return allParts.Where(p => p.Category == category).ToList();
        }

        public async Task<List<Part>> SearchPartsAsync(string searchTerm)
        {
            var allParts = await GetAllPartsAsync();
            return allParts.Where(p => 
                p.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.Reference.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        public async Task<List<Part>> GetLowStockPartsAsync()
        {
            var allParts = await GetAllPartsAsync();
            return allParts.Where(p => p.StockQuantity > 0 && p.StockQuantity <= p.MinimumStock).ToList();
        }

        public async Task<List<Part>> GetOutOfStockPartsAsync()
        {
            var allParts = await GetAllPartsAsync();
            return allParts.Where(p => p.StockQuantity == 0).ToList();
        }

        public async Task<List<int>> GetAvailableDimensionsAsync(PartCategory category)
        {
            // TODO: Implémenter selon votre logique métier
            await Task.Delay(10);
            return category switch
            {
                PartCategory.VerticalBatten => new List<int> { 32, 64 },
                PartCategory.Door => new List<int> { 32, 64 },
                PartCategory.PanelHorizontal => new List<int> { 32, 64 },
                _ => new List<int> { 32, 64 }
            };
        }

        public async Task<Dictionary<string, bool>> CheckPartsAvailabilityAsync(List<PartRequirement> requirements)
        {
            var result = new Dictionary<string, bool>();
            
            foreach (var req in requirements)
            {
                var isAvailable = await CheckAvailabilityAsync(req.PartCode, req.Quantity);
                result[req.PartCode] = isAvailable;
            }
            
            return result;
        }

        public async Task<List<PartRequirement>> GetRequiredPartsAsync(CabinetConfiguration configuration)
        {
            // Utiliser la méthode existante de CabinetConfiguration
            return configuration.GetRequiredParts();
        }

        public async Task<bool> UpdatePartStockAsync(string partCode, int newQuantity)
        {
            return await UpdateStockAsync(partCode, newQuantity, "Part stock update");
        }

        public async Task<bool> ReservePartsAsync(List<PartRequirement> requirements)
        {
            var success = true;
            foreach (var req in requirements)
            {
                var result = await ReserveStockAsync(req.PartCode, req.Quantity);
                if (!result) success = false;
            }
            return success;
        }

        public async Task<bool> ReleasePartsAsync(List<PartRequirement> requirements)
        {
            var success = true;
            foreach (var req in requirements)
            {
                var result = await ReleaseReservedStockAsync(req.PartCode, req.Quantity);
                if (!result) success = false;
            }
            return success;
        }

        // 🔄 Méthodes de conversion
        private StockItem ConvertToStockItem(ApiStockDto apiStock)
        {
            var part = ConvertToPart(apiStock);
            
            return new StockItem
            {
                Part = part,
                CurrentStock = apiStock.Quantity,
                ReservedStock = 0, // TODO: adapter selon votre logique
                ReorderPoint = 10,
                MaximumStockLevel = 100,
                LastUpdated = DateTime.Now,
                Supplier = "API Supplier",
                WarehouseLocation = apiStock.Location ?? "Unknown"
            };
        }

        private Part ConvertToPart(ApiStockDto apiStock)
        {
            return new Part
            {
                Code = apiStock.Code ?? apiStock.Reference ?? "Unknown",
                Reference = apiStock.Reference ?? "Unknown Part",
                Category = DetermineCategoryEnum(apiStock.Reference),
                Dimensions = apiStock.Dimensions ?? "Unknown",
                Price = apiStock.PriceSupplier1,
                StockQuantity = apiStock.Quantity,
                MinimumStock = 10,
                DeliveryDelay = ParseDeliveryDelay(apiStock.DelaySupplier1)
            };
        }

        private PartCategory DetermineCategoryEnum(string? reference)
        {
            if (string.IsNullOrEmpty(reference)) return PartCategory.VerticalBatten;
            
            var refLower = reference.ToLower();
            if (refLower.Contains("panel") && refLower.Contains("horizontal"))
                return PartCategory.PanelHorizontal;
            if (refLower.Contains("panel") && refLower.Contains("vertical"))
                return PartCategory.PanelVertical;
            if (refLower.Contains("panel") && refLower.Contains("back"))
                return PartCategory.PanelBack;
            if (refLower.Contains("door"))
                return PartCategory.Door;
            if (refLower.Contains("crossbar") && refLower.Contains("left"))
                return PartCategory.CrossbarLeft;
            if (refLower.Contains("crossbar") && refLower.Contains("right"))
                return PartCategory.CrossbarRight;
            if (refLower.Contains("crossbar") && refLower.Contains("front"))
                return PartCategory.CrossbarFront;
            if (refLower.Contains("crossbar") && refLower.Contains("back"))
                return PartCategory.CrossbarBack;
            if (refLower.Contains("angle") || refLower.Contains("iron"))
                return PartCategory.AngleIron;
            if (refLower.Contains("coupelles"))
                return PartCategory.Coupelles;
            
            return PartCategory.VerticalBatten; // Par défaut
        }

        private int ParseDeliveryDelay(string? delay)
        {
            if (string.IsNullOrEmpty(delay)) return 7;
            
            // Essayer d'extraire un nombre du string de délai
            if (int.TryParse(delay, out int days))
                return days;
            
            return 7; // Délai par défaut
        }

        public void Dispose() => _httpClient?.Dispose();
    }

    // 📦 DTOs pour l'API
    public class ApiStockDto
    {
        public int Id { get; set; }
        public string? Reference { get; set; }
        public string? Code { get; set; }
        public string? Dimensions { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; } = "AVAILABLE";
        public string? Location { get; set; }
        public decimal PriceSupplier1 { get; set; }
        public string? DelaySupplier1 { get; set; }
    }
}
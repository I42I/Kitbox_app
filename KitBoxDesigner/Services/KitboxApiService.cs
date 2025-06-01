using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions; // Added for parsing dimensions
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

        private async Task<List<ApiStockDto>?> GetApiStockDtosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/stocks");
                response.EnsureSuccessStatusCode();
                
                var json = await response.Content.ReadAsStringAsync();
                // Add null check for json before deserializing
                if (string.IsNullOrEmpty(json))
                {
                    Console.WriteLine("API returned empty or null JSON string for stocks.");
                    return new List<ApiStockDto>();
                }
                return JsonSerializer.Deserialize<List<ApiStockDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching or deserializing stock DTOs: {ex.Message}");
                return new List<ApiStockDto>(); // Return empty list on error
            }
        }

        // 📊 Implémentation IStockService
        public async Task<List<StockItem>> GetAllStockItemsAsync()
        {
            var apiStocks = await GetApiStockDtosAsync();
            return apiStocks?.Select(ConvertToStockItem).ToList() ?? new List<StockItem>();
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
            var apiStocks = await GetApiStockDtosAsync();
            return apiStocks?.Select(ConvertToPart).ToList() ?? new List<Part>();
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
            return await Task.FromResult(configuration.GetRequiredParts());
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
                // Assuming ReleaseReservedStockAsync handles the logic of "releasing" a part
                // This might mean incrementing available stock or decrementing a reserved counter.
                // The current implementation of ReleaseReservedStockAsync calls AddStockAsync.
                bool result = await ReleaseReservedStockAsync(req.PartCode, req.Quantity);
                if (!result)
                {
                    // Log or handle partial failure if necessary
                    success = false;
                }
            }
            return success;
        }

        public async Task<Part?> FindPartBySpecificationAsync(PartSpecification spec)
        {
            var allParts = await GetAllPartsAsync(); // This fetches all parts, could be optimized by category if API supports it

            var potentialParts = allParts.Where(p => p.Category == spec.Category).ToList();

            foreach (var part in potentialParts)
            {
                bool match = true;

                // Match Color
                if (!string.IsNullOrEmpty(spec.Color))
                {
                    // Normalize color comparison if necessary, e.g., case-insensitive
                    // The Part.Color is determined by DetermineColor method which might need alignment
                    if (string.IsNullOrWhiteSpace(part.Color) || !part.Color.Equals(spec.Color, StringComparison.OrdinalIgnoreCase))
                    {
                        match = false;
                    }
                }
                
                // Match TypeAttribute (e.g. for AngleIron color codes like BL, NR)
                if (match && !string.IsNullOrEmpty(spec.TypeAttribute))
                {
                    // This relies on Part.Code or Part.Reference containing the TypeAttribute.
                    // Example: AngleIron code "COR35BL" contains "BL".
                    // This check might need to be more sophisticated based on how TypeAttribute is stored or derived.
                    if (string.IsNullOrWhiteSpace(part.Code) || !part.Code.Contains(spec.TypeAttribute, StringComparison.OrdinalIgnoreCase))
                    {
                         // Also check reference if code doesn't match
                        if(string.IsNullOrWhiteSpace(part.Reference) || !part.Reference.Contains(spec.TypeAttribute, StringComparison.OrdinalIgnoreCase))
                        {
                            match = false;
                        }
                    }
                }

                // Match Dimensions (Height, Width, Depth)
                // This requires parsing the part.Dimensions string (e.g., "32x60x5 cm", "50 cm", "50x30 cm")
                // and comparing with spec.Height, spec.Width, spec.Depth.
                if (match && (spec.Height.HasValue || spec.Width.HasValue || spec.Depth.HasValue))
                {
                    if (string.IsNullOrWhiteSpace(part.Dimensions))
                    {
                        match = false; // Part has no dimensions specified, cannot match
                    }
                    else
                    {
                        var parsedDims = ParseDimensionsString(part.Dimensions);

                        if (spec.Height.HasValue && (!parsedDims.Height.HasValue || parsedDims.Height.Value != spec.Height.Value))
                            match = false;
                        if (spec.Width.HasValue && (!parsedDims.Width.HasValue || parsedDims.Width.Value != spec.Width.Value))
                            match = false;
                        if (spec.Depth.HasValue && (!parsedDims.Depth.HasValue || parsedDims.Depth.Value != spec.Depth.Value))
                            match = false;
                    }
                }

                if (match)
                {
                    return part; // Found a matching part
                }
            }

            return null; // No part found matching the specification
        }

        private (int? Height, int? Width, int? Depth) ParseDimensionsString(string dimString)
        {
            if (string.IsNullOrWhiteSpace(dimString))
                return (null, null, null);

            // Normalize: remove "cm", spaces, and use 'x' as a consistent separator.
            dimString = dimString.ToLowerInvariant().Replace("cm", "").Replace(" ", "");
            
            // Try to match patterns like HxWxD, HxW, WxD, or just H/W/D
            // This is a simplified parser. A more robust one would be needed for varied formats.
            // Example formats: "52x32x30", "32x60", "60", "L:60 H:30 P:20"

            int? h = null, w = null, d = null;
            
            var parts = dimString.Split(new[] { 'x', '*', '-', '/' }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(pVal => int.TryParse(pVal.Trim(), out int val) ? (int?)val : null)
                               .Where(pVal => pVal.HasValue)
                               .Select(pVal => pVal.Value)
                               .ToList();

            // This heuristic assumes order or context based on PartCategory, which is not available here directly.
            // A common convention is H x W x D or W x D x H or just H for vertical items.
            // For simplicity, if 3 values, assume HxWxD or WxDxH (often ambiguous without context)
            // If 2 values, often HxW or WxD
            // If 1 value, often H (for vertical items like battens) or W or D
            
            // This is a very basic interpretation and likely needs to be much smarter,
            // possibly by knowing the typical dimension order for certain part categories
            // or by having more structured dimension data from the API.

            if (parts.Count == 3)
            {
                // Assuming HxWxD for now, or WxDxH. This is ambiguous.
                // Let's assume it could be H, W, D in order if 3 parts.
                // Or it could be that the API provides them in a specific order.
                // For now, let's try to be flexible or make an assumption.
                // If the reference often contains "hauteur", "largeur", "profondeur", that could guide.
                // Without more info, this is a guess.
                // Let's assume for now: if 3 parts, it's H, W, D or W, D, H.
                // If the spec has H, W, D, we need to find a part that matches all three.
                // The order in part.Dimensions might not be fixed.
                
                // A more robust way: if the API could return dimensions as structured data:
                // { "height": 50, "width": 30, "depth": 20 }

                // Simplified: if 3 numbers, assign to H, W, D if they are not already assigned.
                // This won't work well if the order is not guaranteed or if some are zero.
                // For now, we'll assume if 3 numbers are present, they correspond to H, W, D in some order
                // and the matching logic in FindPartBySpecificationAsync will check spec.H against parsed H, etc.
                // This means the ParseDimensionsString should try to identify which number is H, W, D.

                // Let's try a regex approach for "HxWxD" or "H x W x D"
                var regex3D = new Regex(@"(\d+)\s*x\s*(\d+)\s*x\s*(\d+)"); // e.g. 50x30x20
                var match3D = regex3D.Match(dimString);
                if (match3D.Success)
                {
                    h = int.Parse(match3D.Groups[1].Value);
                    w = int.Parse(match3D.Groups[2].Value);
                    d = int.Parse(match3D.Groups[3].Value);
                    return (h, w, d);
                }

                // Regex for "HxW" or "H x W"
                var regex2D_HW = new Regex(@"(\d+)\s*x\s*(\d+)"); // e.g. 50x30
                var match2D_HW = regex2D_HW.Match(dimString);
                if (match2D_HW.Success && parts.Count == 2)
                {
                    // Ambiguous: could be HxW or WxD.
                    // If category is Panel, it's often HxW. If crossbar, WxD or HxL.
                    // For now, assume H then W if two parts.
                    h = int.Parse(match2D_HW.Groups[1].Value);
                    w = int.Parse(match2D_HW.Groups[2].Value);
                    return (h, w, null);
                }
                
                // Regex for single dimension
                var regex1D = new Regex(@"^(\d+)$"); // e.g. 50
                var match1D = regex1D.Match(dimString);
                if (match1D.Success && parts.Count == 1)
                {
                    // Highly ambiguous. Could be H, W, or D.
                    // If it's a Tasseau (VerticalBatten), it's likely Height.
                    // If it's a Traverse (Crossbar), it's likely a Length (Width or Depth of locker).
                    // For now, we can't reliably assign it without category context here.
                    // The calling function FindPartBySpecificationAsync will have to be smart.
                    // Let's return it as 'h' for now if only one dimension.
                    h = int.Parse(match1D.Groups[1].Value);
                    return (h, null, null);
                }
                
                // Fallback: if parts list has values, assign them in order H, W, D
                // This is a weak fallback.
                if (parts.Count > 0) h = parts[0];
                if (parts.Count > 1) w = parts[1];
                if (parts.Count > 2) d = parts[2];
                
                return (h,w,d);
            }
            // If only one or two numbers, it's harder.
            // Example: "TASSEAU 52 CM" -> Height is 52. "TRAVERSE 32 CM" -> Length is 32.
            // The part.Reference often gives clues.
            // For now, this parser is very basic.
            if (parts.Count == 1) h = parts[0]; // Assume height if only one dimension
            if (parts.Count == 2) { h = parts[0]; w = parts[1]; } // Assume H, W
            if (parts.Count >= 3) { h = parts[0]; w = parts[1]; d = parts[2]; } // Assume H, W, D

            return (h, w, d);
        }

        private StockItem ConvertToStockItem(ApiStockDto apiStock)
        {
            var part = ConvertToPart(apiStock);
            
            return new StockItem
            {
                Part = part,
                CurrentStock = apiStock.Quantity,
                ReservedStock = 0, // TODO: Implement reservation logic based on API capabilities or local tracking
                ReorderPoint = part.MinimumStock, // Use MinimumStock from Part, which should be configured
                MaximumStockLevel = part.MinimumStock * 5, // Example: Max stock is 5x min stock, configure as needed
                LastUpdated = DateTime.Now, // TODO: API should ideally provide this
                Supplier = "Dynamic (API)", // Supplier determined by best price/delay in ConvertToPart
                WarehouseLocation = apiStock.Location ?? "Unknown"
                // TODO: Add other relevant fields from StockItem model if they can be derived or fetched
            };
        }

        private Part ConvertToPart(ApiStockDto apiStock)
        {
            decimal price1 = apiStock.PriceSupplier1; // No change needed here as it's a non-nullable decimal in ApiStockDto
            int delay1 = ParseDeliveryDelay(apiStock.DelaySupplier1);
            bool supplier1Valid = price1 > 0;

            decimal price2 = apiStock.PriceSupplier2; // No change needed here
            int delay2 = ParseDeliveryDelay(apiStock.DelaySupplier2);
            bool supplier2Valid = price2 > 0;

            decimal bestPrice = 0;
            int bestDelay = int.MaxValue;
            // string chosenSupplier = "N/A"; // Optional: if you want to store which supplier was chosen

            if (supplier1Valid && supplier2Valid)
            {
                if (price1 < price2)
                {
                    bestPrice = price1;
                    bestDelay = delay1;
                }
                else if (price2 < price1)
                {
                    bestPrice = price2;
                    bestDelay = delay2;
                }
                else // Prices are identical
                {
                    if (delay1 <= delay2)
                    {
                        bestPrice = price1;
                        bestDelay = delay1;
                    }
                    else
                    {
                        bestPrice = price2;
                        bestDelay = delay2;
                    }
                }
            }
            else if (supplier1Valid)
            {
                bestPrice = price1;
                bestDelay = delay1;
            }
            else if (supplier2Valid)
            {
                bestPrice = price2;
                bestDelay = delay2;
            }
            // If neither supplier is valid, bestPrice remains 0 and bestDelay int.MaxValue

            var (parsedHeight, parsedWidth, parsedDepth) = ParseDimensionsString(apiStock.Dimensions ?? string.Empty); // Handle possible null for Dimensions

            return new Part
            {
                Reference = apiStock.Reference ?? "N/A",
                Code = apiStock.Code ?? "N/A",
                Dimensions = apiStock.Dimensions ?? "N/A",
                Height = parsedHeight,
                Width = parsedWidth,
                Depth = parsedDepth,
                Price = bestPrice,
                DeliveryDelay = bestDelay == int.MaxValue ? 0 : bestDelay,                StockQuantity = apiStock.Quantity,
                MinimumStock = 10,                Category = DetermineCategoryEnum(apiStock.Reference, apiStock.Code),
                Color = DetermineColor(apiStock.Code, apiStock.Reference),
                PriceSupplier1 = apiStock.PriceSupplier1,                DelaySupplier1 = apiStock.DelaySupplier1,                PriceSupplier2 = apiStock.PriceSupplier2,                DelaySupplier2 = apiStock.DelaySupplier2            };
        }

        private string? DetermineColor(string? code, string? reference)
        {
            if (!string.IsNullOrEmpty(code))
            {
                if (code.EndsWith("BL", StringComparison.OrdinalIgnoreCase)) return "White";
                if (code.EndsWith("NR", StringComparison.OrdinalIgnoreCase)) return "Black";
                if (code.EndsWith("VE", StringComparison.OrdinalIgnoreCase)) return "Glass";
                if (code.EndsWith("BR", StringComparison.OrdinalIgnoreCase)) return "Brown"; // Assuming BR for Brown
                if (code.EndsWith("GL", StringComparison.OrdinalIgnoreCase)) return "Galvanized";
                // Add more color codes as needed from your catalogue
            }
            
            if (!string.IsNullOrEmpty(reference))
            {
                var refLower = reference.ToLowerInvariant();
                if (refLower.Contains("white")) return "White";
                if (refLower.Contains("black")) return "Black";
                if (refLower.Contains("glass")) return "Glass";
                if (refLower.Contains("brown")) return "Brown";
                if (refLower.Contains("galvanized")) return "Galvanized";
                // Add more keywords if necessary
            }
            return null; // Default if no color can be determined
        }

        private PartCategory DetermineCategoryEnum(string? reference, string? code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                if (code.StartsWith("TAS", StringComparison.OrdinalIgnoreCase)) return PartCategory.VerticalBatten;
                if (code.StartsWith("TRG", StringComparison.OrdinalIgnoreCase)) return PartCategory.CrossbarLeft; 
                if (code.StartsWith("TRD", StringComparison.OrdinalIgnoreCase)) return PartCategory.CrossbarRight; 
                if (code.StartsWith("TRR", StringComparison.OrdinalIgnoreCase)) return PartCategory.CrossbarBack;
                if (code.StartsWith("TRF", StringComparison.OrdinalIgnoreCase)) return PartCategory.CrossbarFront;
                if (code.StartsWith("PAR", StringComparison.OrdinalIgnoreCase)) return PartCategory.PanelBack;
                if (code.StartsWith("PAL", StringComparison.OrdinalIgnoreCase)) return PartCategory.PanelVertical; 
                if (code.StartsWith("PAH", StringComparison.OrdinalIgnoreCase)) return PartCategory.PanelHorizontal;
                if (code.StartsWith("POR", StringComparison.OrdinalIgnoreCase)) return PartCategory.Door;
                if (code.StartsWith("COR", StringComparison.OrdinalIgnoreCase)) return PartCategory.AngleIron;
                if (code.StartsWith("COUPEL", StringComparison.OrdinalIgnoreCase)) return PartCategory.Coupelles;
                // Add other categories based on your catalogue/codes
            }
            
            if (!string.IsNullOrEmpty(reference))
            {
                var refLower = reference.ToLowerInvariant();
                if (refLower.Contains("vertical batten") || refLower.Contains("tasseau vertic")) return PartCategory.VerticalBatten;
                if (refLower.Contains("panel") && (refLower.Contains("horizontal") || refLower.Contains("top") || refLower.Contains("bottom") || refLower.Contains("fond") || refLower.Contains("dessus"))) return PartCategory.PanelHorizontal;
                if (refLower.Contains("panel") && (refLower.Contains("vertical") || refLower.Contains("side") || refLower.Contains("latéral") || refLower.Contains("côté"))) return PartCategory.PanelVertical;
                if (refLower.Contains("panel") && (refLower.Contains("back") || refLower.Contains("arrière"))) return PartCategory.PanelBack;
                if (refLower.Contains("door") || refLower.Contains("porte")) return PartCategory.Door;
                if (refLower.Contains("crossbar") && (refLower.Contains("left") || refLower.Contains("gauche"))) return PartCategory.CrossbarLeft;
                if (refLower.Contains("crossbar") && (refLower.Contains("right") || refLower.Contains("droit"))) return PartCategory.CrossbarRight;
                if (refLower.Contains("crossbar") && (refLower.Contains("front") || refLower.Contains("avant"))) return PartCategory.CrossbarFront;
                if (refLower.Contains("crossbar") && (refLower.Contains("back") || refLower.Contains("arrière"))) return PartCategory.CrossbarBack;
                if (refLower.Contains("angle iron") || refLower.Contains("cornière")) return PartCategory.AngleIron;
                if (refLower.Contains("coupelles") || refLower.Contains("feet") || refLower.Contains("pied")) return PartCategory.Coupelles;
                // Add other keywords
            }
            
            return PartCategory.Unknown; // Ensure PartCategory.Unknown exists
        }

        private int ParseDeliveryDelay(string? delayString)
        {
            if (string.IsNullOrWhiteSpace(delayString)) return int.MaxValue; // Represents unknown or very long delay
            
            // Try to extract number of days, e.g., "10 days", "Approx 5j"
            var match = System.Text.RegularExpressions.Regex.Match(delayString, @"\d+");
            if (match.Success && int.TryParse(match.Value, out int days))
            {
                return days;
            }
            
            // Fallback for simple integer strings if Regex fails or not specific format
            if (int.TryParse(delayString, out days))
            {
                return days;
            }
            
            return int.MaxValue; // Default if parsing fails
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
        public decimal PriceSupplier2 { get; set; } // Added for second supplier
        public string? DelaySupplier2 { get; set; } // Added for second supplier
    }
}
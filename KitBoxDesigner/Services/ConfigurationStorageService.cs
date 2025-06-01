using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using KitBoxDesigner.Models;

namespace KitBoxDesigner.Services
{
    /// <summary>
    /// Service for saving and loading cabinet configurations to/from local storage
    /// </summary>
    public class ConfigurationStorageService
    {
        private readonly string _storageDirectory;
        private readonly JsonSerializerOptions _jsonOptions;

        public ConfigurationStorageService()
        {
            _storageDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KitBoxDesigner", "Configurations");
            
            // Create directory if it doesn't exist
            Directory.CreateDirectory(_storageDirectory);

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            };
        }

        /// <summary>
        /// Save a cabinet configuration to file
        /// </summary>
        public async Task SaveConfigurationAsync(CabinetConfiguration configuration, string? customName = null)
        {
            try
            {
                var fileName = !string.IsNullOrEmpty(customName) 
                    ? $"{SanitizeFileName(customName)}.json"
                    : $"Cabinet_Config_{configuration.Id:N}.json";

                var filePath = Path.Combine(_storageDirectory, fileName);
                var json = JsonSerializer.Serialize(configuration, _jsonOptions);
                
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save configuration: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Load a cabinet configuration from file
        /// </summary>
        public async Task<CabinetConfiguration?> LoadConfigurationAsync(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_storageDirectory, fileName);
                
                if (!File.Exists(filePath))
                    return null;

                var json = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<CabinetConfiguration>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load configuration: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get list of all saved configurations
        /// </summary>
        public async Task<List<SavedConfiguration>> GetSavedConfigurationsAsync()
        {
            try
            {
                var savedConfigs = new List<SavedConfiguration>();
                var files = Directory.GetFiles(_storageDirectory, "*.json");

                foreach (var file in files)
                {
                    try
                    {
                        var fileInfo = new FileInfo(file);
                        var json = await File.ReadAllTextAsync(file);
                        var config = JsonSerializer.Deserialize<CabinetConfiguration>(json, _jsonOptions);

                        if (config != null)
                        {
                            savedConfigs.Add(new SavedConfiguration
                            {
                                FileName = fileInfo.Name,
                                DisplayName = Path.GetFileNameWithoutExtension(fileInfo.Name),
                                SavedDate = fileInfo.LastWriteTime,
                                CompartmentCount = config.CompartmentCount,
                                Dimensions = config.FormattedDimensions,
                                Configuration = config
                            });
                        }
                    }
                    catch
                    {
                        // Skip corrupted files
                        continue;
                    }
                }

                return savedConfigs.OrderByDescending(c => c.SavedDate).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get saved configurations: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Delete a saved configuration
        /// </summary>
        public async Task DeleteConfigurationAsync(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_storageDirectory, fileName);
                
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to delete configuration: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Export configuration to a custom location
        /// </summary>
        public async Task ExportConfigurationAsync(CabinetConfiguration configuration, string exportPath)
        {
            try
            {
                var json = JsonSerializer.Serialize(configuration, _jsonOptions);
                await File.WriteAllTextAsync(exportPath, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to export configuration: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Import configuration from a custom location
        /// </summary>
        public async Task<CabinetConfiguration?> ImportConfigurationAsync(string importPath)
        {
            try
            {
                if (!File.Exists(importPath))
                    return null;

                var json = await File.ReadAllTextAsync(importPath);
                return JsonSerializer.Deserialize<CabinetConfiguration>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to import configuration: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Sanitize filename to remove invalid characters
        /// </summary>
        private static string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());
            return string.IsNullOrWhiteSpace(sanitized) ? "Unnamed_Configuration" : sanitized;
        }
    }

    /// <summary>
    /// Represents a saved configuration entry
    /// </summary>
    public class SavedConfiguration
    {
        public string FileName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public DateTime SavedDate { get; set; }
        public int CompartmentCount { get; set; }
        public string Dimensions { get; set; } = string.Empty;
        public CabinetConfiguration? Configuration { get; set; }
        
        public string FormattedSaveDate => SavedDate.ToString("yyyy-MM-dd HH:mm");
        public string Summary => $"{CompartmentCount} compartments - {Dimensions}";
    }
}

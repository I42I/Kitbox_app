using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using System.Runtime.CompilerServices;
using KitBoxDesigner.Models;
using KitBoxDesigner.Services;

namespace KitBoxDesigner.ViewModels
{
    /// <summary>
    /// View model for the cabinet configurator with step-by-step wizard
    /// </summary>
    public class ConfiguratorViewModel : ViewModelBase
    {
        private readonly IPartService _partService;
        private readonly IStockService _stockService;
        private readonly IPriceCalculatorService _priceCalculatorService;
        private readonly ConfigurationStorageService _configurationStorageService;
        private readonly Action<CabinetConfiguration>? _onCompleteConfiguration;

        // Configuration properties
        private CabinetConfiguration _configuration;
        private int _currentStep;
        private bool _isLoading;
        private string _errorMessage;        // Step management
        private readonly List<string> _steps = new List<string>
        {
            "Basic Options",
            "Dimensions", 
            "Accessories",
            "Review"
        };

        // Available options
        private ObservableCollection<double> _availableWidths;
        private ObservableCollection<double> _availableHeights;
        private ObservableCollection<double> _availableDepths;
        private ObservableCollection<Part> _availableDoors;
        private ObservableCollection<Part> _availablePanels;        // Commands
        public ICommand NextStepCommand { get; }
        public ICommand PreviousStepCommand { get; }
        public ICommand SaveConfigurationCommand { get; }
        public ICommand LoadConfigurationCommand { get; }
        public ICommand ResetConfigurationCommand { get; }
        public ICommand DeleteConfigurationCommand { get; }
        public ICommand RefreshSavedConfigurationsCommand { get; }
        public ICommand CompleteConfigurationCommand { get; }

        public ConfiguratorViewModel(
            IPartService partService,
            IStockService stockService,
            IPriceCalculatorService priceCalculatorService,
            ConfigurationStorageService configurationStorageService,
            Action<CabinetConfiguration>? onCompleteConfiguration = null)
        {
            _partService = partService ?? throw new ArgumentNullException(nameof(partService));
            _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
            _priceCalculatorService = priceCalculatorService ?? throw new ArgumentNullException(nameof(priceCalculatorService));
            _configurationStorageService = configurationStorageService ?? throw new ArgumentNullException(nameof(configurationStorageService));
            _onCompleteConfiguration = onCompleteConfiguration;            // Initialize configuration
            _configuration = new CabinetConfiguration
            {
                Width = 60,  // Default 60cm width
                Height = 200, // Default 200cm height  
                Depth = 58   // Default 58cm depth
            };
            _currentStep = 0;
            _isLoading = false;
            _errorMessage = string.Empty;

            // Initialize collections
            _availableWidths = new ObservableCollection<double>();
            _availableHeights = new ObservableCollection<double>();
            _availableDepths = new ObservableCollection<double>();
            _availableDoors = new ObservableCollection<Part>();
            _availablePanels = new ObservableCollection<Part>();            // Initialize commands - using simple commands without reactive CanExecute to avoid threading issues
            NextStepCommand = new SimpleCommand(NextStep);
            PreviousStepCommand = new SimpleCommand(PreviousStep);
            SaveConfigurationCommand = new SimpleAsyncCommand(SaveConfigurationAsync);
            LoadConfigurationCommand = new SimpleAsyncCommand(LoadConfigurationAsync);
            ResetConfigurationCommand = new SimpleCommand(ResetConfiguration);
            DeleteConfigurationCommand = new SimpleCommand(() => { /* TODO: Implement */ });            RefreshSavedConfigurationsCommand = new SimpleCommand(() => { /* TODO: Implement */ });
            CompleteConfigurationCommand = new SimpleCommand(CompleteConfiguration);

            // Initialize compartments with default count
            UpdateCompartments();

            // Load initial data on UI thread
            RunOnUIThread(() =>
            {
                _ = Task.Run(async () => 
                {
                    try
                    {
                        await LoadAvailableOptionsAsync();
                    }
                    catch (Exception ex)
                    {
                        RunOnUIThread(() => ErrorMessage = $"Failed to load initial data: {ex.Message}");
                    }
                });
            });
        }

        #region Properties

        /// <summary>
        /// Current cabinet configuration
        /// </summary>
        public CabinetConfiguration Configuration
        {
            get => _configuration;
            set => SafeRaiseAndSetIfChanged(ref _configuration, value);
        }        /// <summary>
        /// Current step index in the configuration wizard
        /// </summary>
        public int CurrentStep
        {
            get => _currentStep;            set
            {
                if (SafeRaiseAndSetIfChanged(ref _currentStep, value))
                {
                    RunOnUIThread(() =>
                    {
                        SafeRaisePropertyChanged(nameof(CurrentStepName));
                        SafeRaisePropertyChanged(nameof(IsFirstStep));
                        SafeRaisePropertyChanged(nameof(IsLastStep));
                        SafeRaisePropertyChanged(nameof(CanGoPrevious));
                        SafeRaisePropertyChanged(nameof(CanGoNext));
                        SafeRaisePropertyChanged(nameof(IsCurrentStepValid));
                        SafeRaisePropertyChanged(nameof(IsStep1Active));
                        SafeRaisePropertyChanged(nameof(IsStep1Completed));
                        SafeRaisePropertyChanged(nameof(IsStep2Active));
                        SafeRaisePropertyChanged(nameof(IsStep2Completed));
                        SafeRaisePropertyChanged(nameof(IsStep3Active));
                        SafeRaisePropertyChanged(nameof(IsStep3Completed));
                        SafeRaisePropertyChanged(nameof(IsStep4Active));
                        SafeRaisePropertyChanged(nameof(IsStep4Completed));
                    });
                }
            }
        }

        /// <summary>
        /// Name of the current step
        /// </summary>
        public string CurrentStepName => _currentStep >= 0 && _currentStep < _steps.Count ? _steps[_currentStep] : "Unknown";

        /// <summary>
        /// Whether we are on the first step
        /// </summary>
        public bool IsFirstStep => _currentStep <= 0;

        /// <summary>
        /// Whether we are on the last step
        /// </summary>
        public bool IsLastStep => _currentStep >= _steps.Count - 1;

        /// <summary>
        /// Whether we can go to the previous step
        /// </summary>
        public bool CanGoPrevious => !IsFirstStep && !IsLoading;

        /// <summary>
        /// Whether we can go to the next step
        /// </summary>
        public bool CanGoNext => !IsLastStep && IsCurrentStepValid && !IsLoading;        /// <summary>
        /// Whether the current step is valid and we can proceed
        /// </summary>
        public bool IsCurrentStepValid
        {
            get
            {
                if (_configuration == null) return false;

                switch (_currentStep)
                {
                    case 0: // Basic Options
                        return !string.IsNullOrWhiteSpace(SelectedColor);
                    case 1: // Dimensions
                        return _configuration.Width > 0 && _configuration.Height > 0 && _configuration.Depth > 0;
                    case 2: // Accessories
                        return true; // Accessories are optional
                    case 3: // Review
                        return true; // Review step is always valid
                    default:
                        return false;
                }
            }
        }/// <summary>
        /// Whether the configurator is currently loading data
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SafeRaiseAndSetIfChanged(ref _isLoading, value))
                {
                    RunOnUIThread(() =>
                    {
                        SafeRaisePropertyChanged(nameof(CanGoPrevious));
                        SafeRaisePropertyChanged(nameof(CanGoNext));
                    });
                }
            }
        }

        /// <summary>
        /// Current error message, if any
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SafeRaiseAndSetIfChanged(ref _errorMessage, value);
        }

        /// <summary>
        /// Available widths for cabinet configuration
        /// </summary>
        public ObservableCollection<double> AvailableWidths
        {
            get => _availableWidths;
            set => SafeRaiseAndSetIfChanged(ref _availableWidths, value);
        }

        /// <summary>
        /// Available heights for cabinet configuration
        /// </summary>
        public ObservableCollection<double> AvailableHeights
        {
            get => _availableHeights;
            set => SafeRaiseAndSetIfChanged(ref _availableHeights, value);
        }

        /// <summary>
        /// Available depths for cabinet configuration
        /// </summary>
        public ObservableCollection<double> AvailableDepths
        {
            get => _availableDepths;
            set => SafeRaiseAndSetIfChanged(ref _availableDepths, value);
        }

        /// <summary>
        /// Available doors for cabinet configuration
        /// </summary>
        public ObservableCollection<Part> AvailableDoors
        {
            get => _availableDoors;
            set => SafeRaiseAndSetIfChanged(ref _availableDoors, value);
        }

        /// <summary>
        /// Available panels for cabinet configuration
        /// </summary>
        public ObservableCollection<Part> AvailablePanels
        {        get => _availablePanels;
            set => SafeRaiseAndSetIfChanged(ref _availablePanels, value);
        }

        /// <summary>
        /// Selected width for the cabinet
        /// </summary>
        public int SelectedWidth
        {
            get => _configuration?.Width ?? 0;
            set
            {
                if (_configuration != null && _configuration.Width != value)
                {
                    _configuration.Width = value;
                    UpdateConfigurationCompartments(); // Update compartments when width changes
                    RunOnUIThread(() =>
                    {
                        SafeRaisePropertyChanged(nameof(SelectedWidth));
                        SafeRaisePropertyChanged(nameof(IsCurrentStepValid));
                        SafeRaisePropertyChanged(nameof(CanGoNext));
                    });
                }
            }
        }

        /// <summary>
        /// Selected height for the cabinet
        /// </summary>
        public int SelectedHeight
        {
            get => _configuration?.Height ?? 0;
            set
            {
                if (_configuration != null && _configuration.Height != value)
                {
                    _configuration.Height = value;
                    RunOnUIThread(() =>
                    {
                        SafeRaisePropertyChanged(nameof(SelectedHeight));
                        SafeRaisePropertyChanged(nameof(IsCurrentStepValid));
                        SafeRaisePropertyChanged(nameof(CanGoNext));
                    });
                }
            }
        }        /// <summary>
        /// Selected depth for the cabinet
        /// </summary>
        public int SelectedDepth
        {
            get => _configuration?.Depth ?? 0;
            set
            {
                if (_configuration != null && _configuration.Depth != value)
                {
                    _configuration.Depth = value;
                    UpdateConfigurationCompartments(); // Update compartments when depth changes
                    RunOnUIThread(() =>
                    {
                        SafeRaisePropertyChanged(nameof(SelectedDepth));
                        SafeRaisePropertyChanged(nameof(IsCurrentStepValid));
                        SafeRaisePropertyChanged(nameof(CanGoNext));
                    });
                }
            }
        }

        // Additional properties expected by the XAML view
        
        /// <summary>
        /// Configuration name for saving
        /// </summary>
        public string ConfigurationName { get; set; } = string.Empty;
        
        /// <summary>
        /// Collection of saved configurations
        /// </summary>
        public ObservableCollection<SavedConfiguration> SavedConfigurations { get; } = new();
        
        /// <summary>
        /// Selected saved configuration
        /// </summary>
        public SavedConfiguration? SelectedSavedConfiguration { get; set; }
          /// <summary>
        /// Step navigation properties
        /// </summary>
        public bool IsStep1Active => CurrentStep == 0;
        public bool IsStep1Completed => CurrentStep > 0;
        public bool IsStep2Active => CurrentStep == 1;
        public bool IsStep2Completed => CurrentStep > 1;
        public bool IsStep3Active => CurrentStep == 2;
        public bool IsStep3Completed => CurrentStep > 2;
        public bool IsStep4Active => CurrentStep == 3;
        public bool IsStep4Completed => false;
        
        /// <summary>
        /// Color selection properties
        /// </summary>
        public string SelectedColor { get; set; } = "Brown";
        public bool IsBrownSelected => SelectedColor == "Brown";
        public bool IsWhiteSelected => SelectedColor == "White";
        
        /// <summary>
        /// Number of compartments
        /// </summary>
        private int _numberOfCompartments = 1;
        
        public int NumberOfCompartments
        {
            get => _numberOfCompartments;
            set
            {
                if (SafeRaiseAndSetIfChanged(ref _numberOfCompartments, value))
                {
                    UpdateCompartments();
                    RunOnUIThread(() =>
                    {
                        SafeRaisePropertyChanged(nameof(NumberOfCompartments));
                        SafeRaisePropertyChanged(nameof(CompartmentHeights));
                        SafeRaisePropertyChanged(nameof(IsCurrentStepValid));
                        SafeRaisePropertyChanged(nameof(CanGoNext));
                    });
                }
            }
        }

        /// <summary>
        /// Dimension input as text
        /// </summary>
        public string WidthText
        { 
            get => SelectedWidth > 0 ? SelectedWidth.ToString() : string.Empty; 
            set 
            { 
                if (string.IsNullOrWhiteSpace(value))
                {
                    SelectedWidth = 0;
                }
                else if (int.TryParse(value, out int w) && w > 0) 
                {
                    SelectedWidth = w;
                }
                RunOnUIThread(() => 
                {
                    SafeRaisePropertyChanged(nameof(WidthText));
                    SafeRaisePropertyChanged(nameof(FormattedGlobalDimensions));
                    SafeRaisePropertyChanged(nameof(FormattedDimensions));
                });
            }
        }
        public string HeightText 
        { 
            get => SelectedHeight > 0 ? SelectedHeight.ToString() : string.Empty; 
            set 
            { 
                if (string.IsNullOrWhiteSpace(value))
                {
                    SelectedHeight = 0;
                }
                else if (int.TryParse(value, out int h) && h > 0) 
                {
                    SelectedHeight = h;
                }
                RunOnUIThread(() => 
                {
                    SafeRaisePropertyChanged(nameof(HeightText));
                    SafeRaisePropertyChanged(nameof(FormattedDimensions));
                });
            }
        }
        public string DepthText 
        { 
            get => SelectedDepth > 0 ? SelectedDepth.ToString() : string.Empty; 
            set 
            { 
                if (string.IsNullOrWhiteSpace(value))
                {
                    SelectedDepth = 0;
                }
                else if (int.TryParse(value, out int d) && d > 0) 
                {
                    SelectedDepth = d;
                }
                RunOnUIThread(() => 
                {
                    SafeRaisePropertyChanged(nameof(DepthText));
                    SafeRaisePropertyChanged(nameof(FormattedGlobalDimensions));
                    SafeRaisePropertyChanged(nameof(FormattedDimensions));
                });
            }
        }
          /// <summary>
        /// Dimension validation
        /// </summary>
        public string DimensionValidationMessage => string.Empty;
        public bool HasDimensionValidationError => false;
        
        /// <summary>
        /// Individual compartment heights
        /// </summary>
        private ObservableCollection<CompartmentHeightModel> _compartmentHeights = new();
        
        public ObservableCollection<CompartmentHeightModel> CompartmentHeights
        {
            get => _compartmentHeights;
            set => SafeRaiseAndSetIfChanged(ref _compartmentHeights, value);
        }
        
        /// <summary>
        /// Accessory selections
        /// </summary>
        public bool IncludeDoors { get; set; } = true;
        public bool IncludeLighting { get; set; } = false;
        
        /// <summary>
        /// Corner iron information
        /// </summary>
        public string CornerIronInfo => "4 corner irons required";        /// <summary>
        /// Formatted display properties
        /// </summary>
        public string FormattedDimensions => $"{WidthText} × {HeightText} × {DepthText} cm";
        public string FormattedGlobalDimensions => $"Width: {WidthText} cm, Depth: {DepthText} cm";
        public string SelectedAccessories => "Doors";
        public string EstimatedPrice => "€150.00";
        
        /// <summary>
        /// Update compartments based on count and synchronize with configuration
        /// </summary>
        private void UpdateCompartments()
        {
            // Clear existing compartment heights
            CompartmentHeights.Clear();
            
            // Create new compartment height models
            for (int i = 0; i < NumberOfCompartments; i++)
            {
                CompartmentHeights.Add(new CompartmentHeightModel 
                { 
                    Index = i + 1, 
                    Height = 50,  // Default height
                    OnHeightChanged = () => UpdateConfigurationCompartments()
                });
            }
            
            // Update the configuration compartments
            UpdateConfigurationCompartments();
        }
        
        /// <summary>
        /// Update the CabinetConfiguration compartments list based on current settings
        /// </summary>
        private void UpdateConfigurationCompartments()
        {
            if (_configuration != null)
            {
                _configuration.Compartments.Clear();
                
                for (int i = 0; i < CompartmentHeights.Count; i++)
                {
                    _configuration.Compartments.Add(new Compartment
                    {
                        Position = i,
                        Width = SelectedWidth,
                        Depth = SelectedDepth,
                        Height = CompartmentHeights[i].Height,
                        HasDoor = IncludeDoors,
                        DoorColor = DoorColor.White
                    });
                }
            }
        }
        public string CabinetColor => SelectedColor;        public string StatusMessage => ErrorMessage;
        
        /// <summary>
        /// Visualization properties
        /// </summary>
        public ObservableCollection<CompartmentViewModel> VisualizationCompartments { get; } = new();
        
        /// <summary>
        /// Configuration validation
        /// </summary>
        public bool IsConfigurationValid => IsCurrentStepValid;        // Additional commands expected by XAML
        public ICommand SaveAsConfigurationCommand => SaveConfigurationCommand;

        #endregion

        #region Methods

        /// <summary>
        /// Move to the next step in the configuration wizard
        /// </summary>
        private void NextStep()
        {
            try
            {
                if (CanGoNext)
                {
                    CurrentStep++;
                }
            }
            catch (Exception ex)
            {
                RunOnUIThread(() => ErrorMessage = $"Error moving to next step: {ex.Message}");
            }
        }

        /// <summary>
        /// Move to the previous step in the configuration wizard
        /// </summary>
        private void PreviousStep()
        {
            try
            {
                if (CanGoPrevious)
                {
                    CurrentStep--;
                }
            }
            catch (Exception ex)
            {
                RunOnUIThread(() => ErrorMessage = $"Error moving to previous step: {ex.Message}");
            }
        }

        /// <summary>
        /// Save the current configuration
        /// </summary>
        private async Task SaveConfigurationAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                await _configurationStorageService.SaveConfigurationAsync(_configuration);
                
                RunOnUIThread(() => ErrorMessage = "Configuration saved successfully!");
            }
            catch (Exception ex)
            {
                RunOnUIThread(() => ErrorMessage = $"Error saving configuration: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Load a saved configuration
        /// </summary>
        private async Task LoadConfigurationAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var savedConfigs = await _configurationStorageService.GetSavedConfigurationsAsync();
                if (savedConfigs?.Any() == true)
                {
                    var latestConfig = savedConfigs.OrderByDescending(c => c.SavedDate).FirstOrDefault();
                    if (latestConfig?.Configuration != null)
                    {
                        RunOnUIThread(() => 
                        {
                            Configuration = latestConfig.Configuration;
                            ErrorMessage = "Configuration loaded successfully!";
                        });
                    }
                }
                else
                {
                    RunOnUIThread(() => ErrorMessage = "No saved configurations found.");
                }
            }
            catch (Exception ex)
            {
                RunOnUIThread(() => ErrorMessage = $"Error loading configuration: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Reset the configuration to default values
        /// </summary>
        private void ResetConfiguration()
        {
            try
            {
                Configuration = new CabinetConfiguration();
                CurrentStep = 0;
                ErrorMessage = "Configuration reset to defaults.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error resetting configuration: {ex.Message}";
            }
        }

        /// <summary>
        /// Complete the configuration and navigate to order completion
        /// </summary>
        private void CompleteConfiguration()
        {
            try
            {
                if (_configuration == null)
                {
                    ErrorMessage = "No configuration to complete.";
                    return;
                }

                if (!IsConfigurationValid)
                {
                    ErrorMessage = "Configuration is not valid. Please check all required fields.";
                    return;
                }

                // Update the configuration with final settings
                UpdateConfigurationCompartments();

                // Invoke the navigation callback if provided
                _onCompleteConfiguration?.Invoke(_configuration);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error completing configuration: {ex.Message}";
            }
        }

        /// <summary>
        /// Load available options for configuration
        /// </summary>
        private async Task LoadAvailableOptionsAsync()
        {
            try
            {
                RunOnUIThread(() => IsLoading = true);

                // Load available dimensions
                var standardWidths = new[] { 30.0, 40.0, 50.0, 60.0, 80.0, 100.0, 120.0 };
                var standardHeights = new[] { 70.0, 80.0, 90.0, 100.0, 110.0, 120.0 };
                var standardDepths = new[] { 30.0, 35.0, 40.0, 45.0, 50.0, 60.0 };                // Load parts
                var allParts = await _partService.GetAllPartsAsync();
                var doors = allParts?.Where(p => p.Category == PartCategory.Door) ?? Enumerable.Empty<Part>();
                var panels = allParts?.Where(p => p.Category == PartCategory.PanelVertical || p.Category == PartCategory.PanelBack) ?? Enumerable.Empty<Part>();

                // Update UI on main thread
                RunOnUIThread(() =>
                {
                    try
                    {
                        AvailableWidths.Clear();
                        foreach (var width in standardWidths)
                            AvailableWidths.Add(width);

                        AvailableHeights.Clear();
                        foreach (var height in standardHeights)
                            AvailableHeights.Add(height);

                        AvailableDepths.Clear();
                        foreach (var depth in standardDepths)
                            AvailableDepths.Add(depth);

                        AvailableDoors.Clear();
                        foreach (var door in doors)
                            AvailableDoors.Add(door);

                        AvailablePanels.Clear();
                        foreach (var panel in panels)
                            AvailablePanels.Add(panel);

                        ErrorMessage = "Available options loaded successfully.";
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = $"Error updating UI with available options: {ex.Message}";
                    }
                });
            }
            catch (Exception ex)
            {
                RunOnUIThread(() => ErrorMessage = $"Error loading available options: {ex.Message}");
            }
            finally
            {
                RunOnUIThread(() => IsLoading = false);
            }        }

#endregion
    }

    /// <summary>
    /// Simple view model for compartment visualization
    /// </summary>
    public class CompartmentViewModel
    {
        public int Width { get; set; } = 50;
        public int Height { get; set; } = 30;
    }

    /// <summary>
    /// Model for individual compartment height management
    /// </summary>
    public class CompartmentHeightModel : ViewModelBase
    {
        private int _height = 50;
        
        public int Index { get; set; }
        
        public int Height
        {
            get => _height;
            set
            {
                if (SafeRaiseAndSetIfChanged(ref _height, value))
                {
                    OnHeightChanged?.Invoke();
                }
            }
        }
        
        public string HeightText
        {
            get => Height.ToString();
            set
            {
                if (int.TryParse(value, out int h) && h > 0)
                {
                    Height = h;
                }
            }
        }
        
        public Action? OnHeightChanged { get; set; }
        
        public string DisplayName => $"Compartment {Index}";
    }
}

using System;
using System.Windows.Input;
using ReactiveUI;
using KitBoxDesigner.Services;

namespace KitBoxDesigner.ViewModels
{
    /// <summary>
    /// View model for the tabbed stock management interface
    /// </summary>
    public class StockManagementViewModel : ViewModelBase
    {
        private readonly IPartService _partService;
        private readonly IStockService _stockService;
          private ViewModelBase _currentTabContent;
        private int _selectedTabIndex;
        private StockCheckerViewModel _stockCheckerViewModel;public StockManagementViewModel(IPartService partService, IStockService stockService)
        {
            _partService = partService ?? throw new ArgumentNullException(nameof(partService));
            _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
              // Initialize tab view models - now inventory tab shows stock checker
            _stockCheckerViewModel = new StockCheckerViewModel(_stockService, _partService);
            
            // Start with stock checker in the "Inventaire" tab
            _selectedTabIndex = 0;
            _currentTabContent = _stockCheckerViewModel;
            
            // Commands
            SelectTabCommand = new SimpleCommand<int>(SelectTab);
        }

        /// <summary>
        /// Command to select a tab
        /// </summary>
        public ICommand SelectTabCommand { get; }

        /// <summary>
        /// Currently selected tab index
        /// </summary>
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (this.SafeRaiseAndSetIfChanged(ref _selectedTabIndex, value))
                {
                    UpdateCurrentTabContent();
                }
            }
        }

        /// <summary>
        /// Current tab content view model
        /// </summary>
        public ViewModelBase CurrentTabContent
        {
            get => _currentTabContent;
            private set => this.SafeRaiseAndSetIfChanged(ref _currentTabContent, value);
        }        /// <summary>
        /// Stock checker view model for direct binding
        /// </summary>
        public StockCheckerViewModel StockCheckerViewModel => _stockCheckerViewModel;

        /// <summary>
        /// Select a specific tab
        /// </summary>
        private void SelectTab(int tabIndex)
        {
            SelectedTabIndex = tabIndex;
        }        /// <summary>
        /// Update the current tab content based on selected index
        /// </summary>
        private void UpdateCurrentTabContent()
        {
            CurrentTabContent = _selectedTabIndex switch
            {
                0 => _stockCheckerViewModel, // Inventaire tab now shows stock checker
                _ => _stockCheckerViewModel
            };
        }
    }
}

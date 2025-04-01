using Kitbox_avalonia.Services;

namespace Kitbox_avalonia.ViewModels
{
    public class StockManagementViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public StockManagementViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }
}

using Kitbox_avalonia.Services;

namespace Kitbox_avalonia.ViewModels
{
    public class SupplierManagementViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public SupplierManagementViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }
}

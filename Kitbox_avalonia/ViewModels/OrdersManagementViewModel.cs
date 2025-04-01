using Kitbox_avalonia.Services;

namespace Kitbox_avalonia.ViewModels
{
    public class OrdersManagementViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public OrdersManagementViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }
}

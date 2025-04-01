using Kitbox_avalonia.Services;

namespace Kitbox_avalonia.ViewModels
{
    public class OrdersViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public OrdersViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }
}

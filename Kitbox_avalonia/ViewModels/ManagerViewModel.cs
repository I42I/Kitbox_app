using Kitbox_avalonia.Services;

namespace Kitbox_avalonia.ViewModels
{
    public class ManagerViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public ManagerViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }
}

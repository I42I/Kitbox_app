using Kitbox_avalonia.Services;

namespace Kitbox_avalonia.ViewModels
{
    public class ConfigViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public ConfigViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }
}

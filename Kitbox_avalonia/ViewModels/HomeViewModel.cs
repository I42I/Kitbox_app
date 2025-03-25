using Kitbox_avalonia.Services;
using ReactiveUI;
using System.Windows.Input;

namespace Kitbox_avalonia.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public ICommand CreateCabinetCommand { get; }
        public ICommand ViewOrdersCommand { get; }
        public ICommand LoginCommand { get; }

        public HomeViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            
            CreateCabinetCommand = ReactiveCommand.Create(CreateCabinet);
            ViewOrdersCommand = ReactiveCommand.Create(ViewOrders);
            LoginCommand = ReactiveCommand.Create(Login);
        }

        private void CreateCabinet()
        {
            _navigationService.Navigate<ConfigViewModel>();
        }

        private void ViewOrders()
        {
            _navigationService.Navigate<OrdersViewModel>();
        }

        private void Login()
        {
            _navigationService.Navigate<LoginViewModel>();
        }
    }
}
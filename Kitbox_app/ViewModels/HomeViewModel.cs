using CommunityToolkit.Mvvm.Input;
using Kitbox_app.Services;

namespace Kitbox_app.ViewModels;

public class HomeViewModel : ViewModelBase
{
    private readonly INavigationService _navigation;

    public IRelayCommand GoToCreateCabinet { get; }
    public IRelayCommand GoToOrders { get; }
    public IRelayCommand GoToLogin { get; }

    public HomeViewModel(INavigationService navigation)
    {
        _navigation = navigation;

        GoToCreateCabinet = new RelayCommand(() =>
            _navigation.NavigateTo(new CreateCabinetViewModel(_navigation)));

        GoToOrders = new RelayCommand(() =>
            _navigation.NavigateTo(new OrdersViewModel(_navigation)));

        GoToLogin = new RelayCommand(() =>
            _navigation.NavigateTo(new LoginViewModel(_navigation)));
    }
}

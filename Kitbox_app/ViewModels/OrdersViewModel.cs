using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Kitbox_app.Services;

namespace Kitbox_app.ViewModels;

public partial class OrdersViewModel : ViewModelBase // 🔄 hérite de ViewModelBase
{
    private readonly INavigationService _navigation;

    public ObservableCollection<string> Orders { get; } = new()
    {
        "Order #1001 - 3 Lockers",
        "Order #1002 - 1 Cabinet",
        "Order #1003 - 5 Lockers"
    };

    public IRelayCommand BackCommand { get; }

    public OrdersViewModel(INavigationService navigation)
    {
        _navigation = navigation;

        BackCommand = new RelayCommand(() =>
            _navigation.NavigateTo(new HomeViewModel(_navigation)));
    }
}

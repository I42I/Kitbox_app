using CommunityToolkit.Mvvm.ComponentModel;
using Kitbox_app.Services;

namespace Kitbox_app.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public INavigationService Navigation { get; }

    [ObservableProperty]
    private ViewModelBase currentViewModel;

    public MainWindowViewModel()
    {
        Navigation = new NavigationService(this);
        CurrentViewModel = new HomeViewModel(Navigation);
    }
}

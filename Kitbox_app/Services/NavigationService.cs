using Kitbox_app.ViewModels;

namespace Kitbox_app.Services;

public class NavigationService : INavigationService
{
    private readonly MainWindowViewModel _mainWindowViewModel;

    public NavigationService(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
    }

    public void NavigateTo(ViewModelBase viewModel)
    {
        // Important : avertir de la modification pour que l'interface se mette à jour
        _mainWindowViewModel.CurrentViewModel = viewModel;
    }
}

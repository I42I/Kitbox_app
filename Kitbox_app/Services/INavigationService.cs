using Kitbox_app.ViewModels;

namespace Kitbox_app.Services;

public interface INavigationService
{
    void NavigateTo(ViewModelBase viewModel);
}

using System.Reactive;
using ReactiveUI;
using Avalonia.Controls;
using Kitbox_app.Views;

namespace Kitbox_app.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";
    
    public ReactiveCommand<Window, Unit> AddBoxCommand { get; }

    public MainWindowViewModel()
    {
        AddBoxCommand = ReactiveCommand.Create<Window>(OpenAddBoxDialog);
    }

    private async void OpenAddBoxDialog(Window owner)
    {
        var dialog = new AddBoxView();
        await dialog.ShowDialog(owner);
    }
}

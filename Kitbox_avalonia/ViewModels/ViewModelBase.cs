using Avalonia;
using ReactiveUI;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Kitbox_avalonia.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        // Add common ViewModel functionality here
    }
    
    public interface INavigationAware
    {
        void OnNavigatedTo(object? parameter);
        void OnNavigatedFrom();
    }
}
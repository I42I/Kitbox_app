using Avalonia.Controls;
using Kitbox_avalonia.Services;
using ReactiveUI;
using System.Reactive;
using System.Windows.Input;

namespace Kitbox_avalonia.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        
        private string _username = string.Empty;
        public string Username 
        { 
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }
        
        private string _password = string.Empty;
        public string Password 
        { 
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ICommand BackCommand { get; }

        public LoginViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            LoginCommand = ReactiveCommand.Create(Login);
            BackCommand = ReactiveCommand.Create(GoBack);
        }

        private void Login()
        {
            if (Username == "admin" && Password == "admin")
            {
                // In Avalonia, we'll need a different approach for message boxes
                // For now, we'll just navigate
                _navigationService.Navigate<ManagerViewModel>();
            }
        }

        private void GoBack()
        {
            _navigationService.Navigate<HomeViewModel>();
        }
    }
}
using KitBoxDesigner.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using KitBoxDesigner.Models;

namespace KitBoxDesigner.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IKitboxApiService _kitboxApiService;

        public LoginViewModel(IAuthenticationService authenticationService, IKitboxApiService kitboxApiService)
        {
            _authenticationService = authenticationService;
            _kitboxApiService = kitboxApiService;
        }

        [RelayCommand]
        private void LoginAsAdmin()
        {
            // Direct admin login for demo purposes (no password required)
            _authenticationService.LoginAsAdmin();
        }
    }
}

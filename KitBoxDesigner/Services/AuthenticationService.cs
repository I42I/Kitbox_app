// New file: KitBoxDesigner/Services/AuthenticationService.cs
using System;
using ReactiveUI;

namespace KitBoxDesigner.Services
{
    public interface IAuthenticationService
    {
        bool IsAdmin { get; }
        void LoginAsAdmin();
        void Logout();
        event Action? AuthenticationStateChanged;
    }

    public class AuthenticationService : ReactiveObject, IAuthenticationService
    {
        private bool _isAdmin;
        public bool IsAdmin
        {
            get => _isAdmin;
            private set => this.RaiseAndSetIfChanged(ref _isAdmin, value);
        }

        public event Action? AuthenticationStateChanged;

        public void LoginAsAdmin()
        {
            IsAdmin = true;
            AuthenticationStateChanged?.Invoke();
        }

        public void Logout()
        {
            IsAdmin = false;
            AuthenticationStateChanged?.Invoke();
        }
    }
}

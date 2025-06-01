// New file: KitBoxDesigner/Services/AuthenticationService.cs
using System;
using ReactiveUI;

namespace KitBoxDesigner.Services
{
    public interface IAuthenticationService
    {
        bool IsAdmin { get; }
        string CurrentUser { get; }
        bool LoginAsAdmin(string username, string password);
        void Logout();
        event Action? AuthenticationStateChanged;
    }

    public class AuthenticationService : ReactiveObject, IAuthenticationService
    {
        private bool _isAdmin;
        private string _currentUser = "Guest";

        public bool IsAdmin
        {
            get => _isAdmin;
            private set => this.RaiseAndSetIfChanged(ref _isAdmin, value);
        }

        public string CurrentUser
        {
            get => _currentUser;
            private set => this.RaiseAndSetIfChanged(ref _currentUser, value);
        }

        public event Action? AuthenticationStateChanged;

        public bool LoginAsAdmin(string username, string password)
        {
            // Simple password check - in production, use proper authentication
            if (password == "admin")
            {
                IsAdmin = true;
                CurrentUser = string.IsNullOrWhiteSpace(username) ? "Admin" : username;
                AuthenticationStateChanged?.Invoke();
                return true;
            }
            return false;
        }

        public void Logout()
        {
            IsAdmin = false;
            CurrentUser = "Guest";
            AuthenticationStateChanged?.Invoke();
        }
    }
}

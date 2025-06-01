using System;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace KitBoxDesigner.Services
{
    public interface IAuthenticationService
    {
        bool IsAdmin { get; }
        void LoginAsAdmin();
        void Logout();
        event Action? AuthenticationStateChanged;
    }

    public partial class AuthenticationService : ObservableObject, IAuthenticationService
    {
        [ObservableProperty]
        private bool _isAdmin;

        public event Action? AuthenticationStateChanged;

        public void LoginAsAdmin()
        {
            IsAdmin = true;
            RaiseAuthenticationStateChanged();
        }

        public void Logout()
        {
            IsAdmin = false;
            RaiseAuthenticationStateChanged();
        }

        private void RaiseAuthenticationStateChanged()
        {
            // Ensure the event is raised on the UI thread
            if (Dispatcher.UIThread.CheckAccess())
            {
                AuthenticationStateChanged?.Invoke();
            }
            else
            {
                Dispatcher.UIThread.Post(() => AuthenticationStateChanged?.Invoke());
            }
        }
    }
}

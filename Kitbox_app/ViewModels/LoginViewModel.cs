using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kitbox_app.Services;
using System;

namespace Kitbox_app.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly INavigationService _navigation;

    public LoginViewModel(INavigationService navigation)
    {
        _navigation = navigation;
    }

    // Champs observables générés automatiquement
    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    // Commande pour revenir à l’accueil
    [RelayCommand]
    private void Back()
    {
        _navigation.NavigateTo(new HomeViewModel(_navigation));
    }

    // Commande de connexion
    [RelayCommand]
    private void Login()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            Console.WriteLine("❌ Veuillez remplir tous les champs.");
            return;
        }

        Console.WriteLine($"🔐 Tentative de connexion avec {Email} / {Password}");
        // TODO : implémenter vérification réelle
    }
}

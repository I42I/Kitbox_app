using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kitbox_app.Models;
using Kitbox_app.Services;
using System.Collections.ObjectModel;

namespace Kitbox_app.ViewModels;

public partial class CreateCabinetViewModel : ViewModelBase
{
    private readonly INavigationService _navigation;

    [ObservableProperty]
    private string selectedHeight = "32";

    [ObservableProperty]
    private string selectedWidth = "62";

    [ObservableProperty]
    private string selectedDepth = "50";

    [ObservableProperty]
    private ObservableCollection<Locker> lockers = new();

    public bool CanAddLocker => Lockers.Count < 7;

    public CreateCabinetViewModel(INavigationService navigation)
    {
        _navigation = navigation;
    }

    // ✅ Commandes pour sélectionner hauteur/largeur/profondeur
    [RelayCommand]
    private void SelectHeight(string value) => SelectedHeight = value;

    [RelayCommand]
    private void SelectWidth(string value) => SelectedWidth = value;

    [RelayCommand]
    private void SelectDepth(string value) => SelectedDepth = value;

    // ✅ Commande pour ajouter un locker
    [RelayCommand]
    private void AddLocker()
    {
        if (CanAddLocker)
        {
            Lockers.Add(new Locker
            {
                Height = SelectedHeight,
                Width = SelectedWidth,
                Depth = SelectedDepth
            });
            OnPropertyChanged(nameof(CanAddLocker));
        }
    }

    // ✅ Commande pour supprimer un locker
    [RelayCommand]
    private void RemoveLocker(Locker locker)
    {
        Lockers.Remove(locker);
        OnPropertyChanged(nameof(CanAddLocker));
    }

    // ✅ Commande pour revenir à l'accueil
    [RelayCommand]
    private void GoBack()
    {
        _navigation.NavigateTo(new HomeViewModel(_navigation));
    }
}

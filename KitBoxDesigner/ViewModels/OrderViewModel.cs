using KitBoxDesigner.Services; // For IKitboxApiService
using ReactiveUI;
using System.Collections.ObjectModel; // For ObservableCollection if displaying list of orders
using KitBoxDesigner.Models; // For Order or related models

namespace KitBoxDesigner.ViewModels
{
    public class OrderViewModel : ViewModelBase
    {
        private readonly IKitboxApiService _kitboxApiService;

        // Example property for a list of orders
        // private ObservableCollection<Order> _orders = new ObservableCollection<Order>();
        // public ObservableCollection<Order> Orders
        // {
        //     get => _orders;
        //     set => this.RaiseAndSetIfChanged(ref _orders, value);
        // }

        public OrderViewModel(IKitboxApiService kitboxApiService)
        {
            _kitboxApiService = kitboxApiService;
            // Load orders or initialize other properties here
            // LoadOrdersAsync();
        }

        // Example method to load orders
        // private async void LoadOrdersAsync()
        // {
        //     // var ordersFromApi = await _kitboxApiService.GetOrdersAsync(); // Assuming such a method exists
        //     // Orders = new ObservableCollection<Order>(ordersFromApi);
        // }
    }
}
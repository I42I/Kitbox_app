using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KitBoxDesigner.Models;

namespace KitBoxDesigner.Services
{    /// <summary>
    /// Service for managing customer orders (in-memory implementation)
    /// </summary>
    public class CustomerOrderService : ICustomerOrderService
    {
        private readonly List<CustomerOrder> _orders = new();
        private int _nextId = 1;

        public CustomerOrderService()
        {
            // Add some sample data for testing
            InitializeSampleData();
        }

        public async Task<int> SaveCustomerOrderAsync(CabinetConfiguration configuration, string customerName, string customerEmail, string customerPhone, string customerAddress)
        {
            await Task.Delay(10); // Simulate async operation
            
            var order = new CustomerOrder
            {
                Id = _nextId++,
                CustomerName = customerName,
                CustomerEmail = customerEmail,
                CustomerPhone = customerPhone,
                CustomerAddress = customerAddress,
                Configuration = configuration,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                TotalPrice = 0, // Will be calculated later
                DepositPaid = 0,
                Notes = ""
            };

            _orders.Add(order);
            return order.Id;
        }        public async Task<List<CustomerOrder>> GetCustomerOrdersAsync()
        {
            await Task.Delay(10); // Simulate async operation
            return _orders.ToList();
        }

        public async Task<List<CustomerOrder>> GetAllOrdersAsync()
        {
            await Task.Delay(10); // Simulate async operation
            return _orders.ToList();
        }

        public async Task<List<CustomerOrder>> GetCustomerOrdersByNameAsync(string customerName)
        {
            await Task.Delay(10); // Simulate async operation
            return _orders.Where(o => o.CustomerName.Contains(customerName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            await Task.Delay(10); // Simulate async operation
            
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                order.Status = status;
            }
        }        public async Task<CustomerOrder?> GetOrderByIdAsync(int orderId)
        {
            await Task.Delay(10); // Simulate async operation
            return _orders.FirstOrDefault(o => o.Id == orderId);
        }        private void InitializeSampleData()
        {
            var sampleConfiguration = new CabinetConfiguration
            {
                Width = 800,
                Height = 1200,
                Depth = 600,
                Compartments = new List<Compartment>
                {
                    new Compartment { Width = 400, Height = 400, Depth = 600, Position = 0 },
                    new Compartment { Width = 400, Height = 800, Depth = 600, Position = 1 }
                }
            };

            // Sample order 1
            _orders.Add(new CustomerOrder
            {
                Id = _nextId++,
                CustomerName = "John Smith",
                CustomerEmail = "john.smith@email.com",
                CustomerPhone = "+1-555-0123",
                CustomerAddress = "123 Main St, Anytown, USA",
                Configuration = sampleConfiguration,
                OrderDate = DateTime.Now.AddDays(-5),
                Status = OrderStatus.Pending,
                TotalPrice = 850.00m,
                DepositPaid = 0,
                Notes = "Standard kitchen cabinet configuration"
            });

            // Sample order 2  
            _orders.Add(new CustomerOrder
            {
                Id = _nextId++,
                CustomerName = "Sarah Johnson",
                CustomerEmail = "sarah.j@email.com",
                CustomerPhone = "+1-555-0456",
                CustomerAddress = "456 Oak Ave, Springfield, USA",
                Configuration = sampleConfiguration,
                OrderDate = DateTime.Now.AddDays(-3),
                Status = OrderStatus.Confirmed,
                TotalPrice = 1200.00m,
                DepositPaid = 300.00m,
                Notes = "Custom office storage solution"
            });

            // Sample order 3
            _orders.Add(new CustomerOrder
            {
                Id = _nextId++,
                CustomerName = "Mike Brown",
                CustomerEmail = "mike.brown@company.com",
                CustomerPhone = "+1-555-0789",
                CustomerAddress = "789 Pine St, Metro City, USA",
                Configuration = sampleConfiguration,
                OrderDate = DateTime.Now.AddDays(-1),
                Status = OrderStatus.InProduction,
                TotalPrice = 950.00m,
                DepositPaid = 475.00m,
                Notes = "Garage organization system"
            });
        }
    }
}

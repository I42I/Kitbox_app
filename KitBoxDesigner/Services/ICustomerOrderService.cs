using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KitBoxDesigner.Models;

namespace KitBoxDesigner.Services
{
    /// <summary>
    /// Interface for managing customer orders
    /// </summary>
    public interface ICustomerOrderService
    {
        /// <summary>
        /// Save a completed cabinet configuration as a customer order
        /// </summary>
        Task<int> SaveCustomerOrderAsync(CabinetConfiguration configuration, string customerName, string customerEmail, string customerPhone, string customerAddress);
          /// <summary>
        /// Get all customer orders
        /// </summary>
        Task<List<CustomerOrder>> GetCustomerOrdersAsync();
        
        /// <summary>
        /// Get all customer orders for admin management
        /// </summary>
        Task<List<CustomerOrder>> GetAllOrdersAsync();
        
        /// <summary>
        /// Get customer orders by customer name
        /// </summary>
        Task<List<CustomerOrder>> GetCustomerOrdersByNameAsync(string customerName);
        
        /// <summary>
        /// Update order status
        /// </summary>
        Task UpdateOrderStatusAsync(int orderId, OrderStatus status);
        
        /// <summary>
        /// Get order by ID
        /// </summary>
        Task<CustomerOrder?> GetOrderByIdAsync(int orderId);
    }

    /// <summary>
    /// Customer order model
    /// </summary>
    public class CustomerOrder
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = "";
        public string CustomerEmail { get; set; } = "";
        public string CustomerPhone { get; set; } = "";
        public string CustomerAddress { get; set; } = "";
        public CabinetConfiguration Configuration { get; set; } = new();
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DepositPaid { get; set; }
        public string Notes { get; set; } = "";
    }

    /// <summary>
    /// Order status enumeration
    /// </summary>
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        InProduction,
        ReadyForDelivery,
        Delivered,
        Cancelled
    }
}

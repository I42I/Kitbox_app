# Kitbox Application Suite

## Complete Digital Solution for Modular Cabinet Design and Management

The **Kitbox Application Suite** is a comprehensive digital solution developed for modernizing cabinet ordering and inventory management processes. This project transforms a traditional paper-based system into an advanced digital workflow using cutting-edge .NET technologies.

---

## 🏗️ Project Architecture

The solution consists of two main components:

### 1. **KitBoxDesigner** - Advanced Desktop Application
A sophisticated **Avalonia UI** desktop application providing:
- **Interactive Cabinet Configuration**: Visual wizard for designing custom cabinets
- **Real-time Stock Management**: Live inventory tracking with low-stock alerts
- **Price Calculator**: Automatic pricing based on component requirements
- **Order Validation**: Business rule enforcement (max 7 lockers, compatibility checks)
- **Multi-platform Support**: Windows, macOS, and Linux compatibility

### 2. **KitboxAPI** - Production Backend
A robust **ASP.NET Core 9.0** Web API featuring:
- **RESTful Architecture**: Clean separation of concerns with DTOs
- **MariaDB Integration**: Production database with Entity Framework Core
- **Docker Deployment**: Containerized with Traefik reverse proxy
- **HTTPS/SSL**: Let's Encrypt certificates for secure communication
- **Comprehensive CRUD**: Full lifecycle management for all entities

---

## 🎯 Business Requirements Addressed

The solution addresses **Kitbox Company's** specific needs:

### Cabinet Composition Rules
- **Maximum 7 lockers** per cabinet
- **Height calculation**: Sum of locker heights + 4cm for angle irons
- **Compatibility validation**: Ensuring proper locker combinations

### Stock Management
- **Multi-supplier support** with automatic price comparison
- **Minimum stock tracking** based on sales history
- **Automatic reordering** when stock falls below thresholds
- **Delivery time optimization** for supplier selection

### Order Processing
- **Deposit management** for incomplete stock situations
- **Order status tracking** (RESERVED, PURCHASED, PAID)
- **Component availability checking** before order confirmation

---

## 🔧 Technical Stack

### Frontend (KitBoxDesigner)
- **Framework**: Avalonia UI 11.2.7 with Fluent Design
- **Architecture**: MVVM pattern with ReactiveUI
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection
- **Data Binding**: CommunityToolkit.Mvvm for reactive ViewModels
- **Graphics**: SkiaSharp for cross-platform rendering

### Backend (KitboxAPI)
- **Framework**: ASP.NET Core 9.0 Web API
- **Database**: MariaDB 11.5 with MySql.EntityFrameworkCore provider
- **Architecture**: Layered architecture with Repository pattern
- **Documentation**: Swagger/OpenAPI for API exploration
- **Serialization**: System.Text.Json with enum-to-string conversion

### Infrastructure
- **Containerization**: Docker with multi-stage builds
- **Reverse Proxy**: Traefik with automatic SSL certificate management
- **Database**: MariaDB with connection pooling and retry logic
- **CORS**: Configured for cross-origin desktop application support

---

## 📊 Database Schema

### Core Entities
| Entity | Purpose | Key Relationships |
|--------|---------|-------------------|
| `CustomerOrder` | Client orders with status tracking | → `Cabinet` (1:N) |
| `Cabinet` | Complete cabinet configurations | → `Locker` (1:N) |
| `Locker` | Individual compartments with dimensions | → `LockerStock` (1:N) |
| `Stock` | Inventory items with supplier pricing | → `SupplierOrder` (N:1) |
| `Supplier` | Vendor information and contact details | → `SupplierOrder` (1:N) |

### Business Logic Implementation
- **Enum Status Management**: `OrderStatus` and `StockStatus` with string conversion
- **Foreign Key Constraints**: Maintaining data integrity across relationships
- **Automatic Calculations**: Price computation based on component requirements

---

## 🚀 Deployment Architecture

### Production Environment
```
https://kitbox.msrl.be
├── Traefik (Reverse Proxy + SSL)
├── KitboxAPI (Docker Container)
└── MariaDB (Docker Container)
```

### Development Workflow
```
KitBoxDesigner ←→ KitboxApiService ←→ REST API ←→ MariaDB
    (Desktop)         (HTTP Client)      (Backend)     (Database)
```

---

## 🔌 API Integration

The **KitboxApiService** class provides seamless integration between the desktop application and the production API:

### Service Implementation
- **Dual Interface Support**: Implements both `IStockService` and `IPartService`
- **Automatic Conversion**: Transforms API DTOs to application models
- **Error Handling**: Robust exception management with retry logic
- **Real-time Data**: Live stock levels and part availability

### Key Features
```csharp
// Real-time stock checking
var availability = await apiService.CheckStockAvailabilityAsync(requirements);

// Automatic part categorization
var parts = await apiService.GetPartsByCategoryAsync(PartCategory.Door);

// Order validation with business rules
var requiredParts = configuration.GetRequiredParts();
```

---

## 📈 Key Improvements Over Legacy System

### From Paper to Digital
- **Manual Forms** → **Interactive Configuration Wizard**
- **Error-prone Calculations** → **Automatic Validation & Pricing**
- **Inventory Guesswork** → **Real-time Stock Tracking**
- **Manual Supplier Management** → **Automated Price Comparison**

### Performance Benefits
- **Instant Availability Checking**: No more stockroom visits
- **Automatic Reordering**: Prevents stockouts
- **Error Reduction**: Business rule validation prevents invalid configurations
- **Customer Experience**: Immediate order confirmation or deposit calculation

---

## 🎨 User Experience

### Cabinet Configuration Wizard
1. **Select Dimensions**: Choose cabinet width, depth, and maximum height
2. **Add Lockers**: Configure individual compartments with doors/colors
3. **Validate Design**: Automatic compatibility and dimension checking
4. **Calculate Price**: Real-time pricing with component breakdown
5. **Check Availability**: Stock verification with delivery estimates

### Inventory Management
- **Dashboard Overview**: Total parts, low stock alerts, reorder notifications
- **Search & Filter**: Find parts by category, code, or availability status
- **Stock Movements**: Track inventory changes with audit trail
- **Supplier Integration**: Automatic ordering based on stock levels

---

## 🔄 Future Extensibility

The architecture supports planned enhancements:
- **Additional Elements**: Shelves and drawers (plugin-ready architecture)
- **Advanced Analytics**: Sales reporting and stock optimization
- **Mobile Support**: Avalonia's cross-platform capabilities
- **API Expansion**: Additional endpoints for enhanced functionality

---

## 📋 Development Setup

### Prerequisites
- **.NET 9.0 SDK**
- **Docker & Docker Compose**

### Quick Start
```bash
# Clone the repository
git clone [repository-url]

# Start the backend (Docker)
cd kitboxAPI
docker-compose up -d

# Run the desktop application
cd ../KitBoxDesigner
dotnet run

# Access API documentation
# https://kitbox.msrl.be/swagger
```

---

## 🎓 Academic Context

This project was developed as part of an **Electrical Engineering** program with a focus on **Computer Science & Electronics**. It demonstrates:

- **Software Architecture**: Clean separation of concerns and scalable design
- **Database Design**: Normalized schema with proper relationships
- **API Development**: RESTful services with comprehensive documentation
- **UI/UX Design**: Modern desktop application with intuitive workflows
- **DevOps Practices**: Containerization and production deployment

---

## ✨ Result

The **Kitbox Application Suite** successfully modernizes the cabinet ordering process, providing:
- **Seamless Integration** between desktop and cloud infrastructure
- **Business Rule Enforcement** preventing configuration errors
- **Real-time Inventory Management** with automatic supplier integration
- **Professional User Experience** replacing paper-based workflows
- **Scalable Architecture** ready for future business expansion

This solution represents a complete digital transformation, bridging traditional manufacturing processes with modern software development practices.
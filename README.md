# 🧱 Kitbox – Modular Cabinet Configurator

> Academic software project developed at **ECAM Brussels Engineering School** as part of the third-year **Bachelor in Electrical Engineering (Electronics & Computer Science track)**.

---

## 🎓 Academic Context

This project was conducted within the framework of software engineering education. The goal is to develop an interactive application that allows users to **configure a customizable modular cabinet**, using standardized components and respecting industrial design logic and constraints.

---

## 👥 Project Team

### Student Developers

| Full Name               | Student ID |
|-------------------------|------------|
| Niall Scafe             | 22272      |
| Issakha Yaya Libis      | 21252      |
| Bruno Masureel          | 23375      |
| Louis Pierre            | 23317      |
| Omar Chokayri           | 22379      |
| Simon Delecluse         | 22158      |

### Academic Supervisors

- **Prof. André Lorge**
- **Prof. Shabani Luleta**

---

## 🎯 Learning Objectives

- Build a robust and modular software architecture using the **MVVM** pattern.
- Design a clean and user-friendly interface for product configuration.
- Implement backend services via a RESTful **.NET Web API**.
- Manage application states, navigation, user interactions and data integrity.

---

## 🛠️ Technologies Used

| Component       | Technology                   |
|----------------|------------------------------|
| Frontend        | **Avalonia UI** (.NET C#)    |
| Backend         | **ASP.NET Core Web API**     |
| Database ORM    | **Entity Framework Core**    |
| Database Engine | **MariaDB / MySQL**          |
| Architecture    | **MVVM (Model-View-ViewModel)** |
| Version Control | **Git** + **GitHub**         |
| IDEs            | Visual Studio / JetBrains Rider |

---

## 📸 Application Preview

| Home Screen               | Cabinet Configuration View     |
|---------------------------|-------------------------------|
| ![Home](docs/screenshot_home.png) | ![CreateCabinet](docs/screenshot_cabinet.png) |

---

## 📦 Key Features

- 🏠 **Home Screen** with navigation to:
  - "Create a Cabinet"
  - "View My Orders"
  - "Log In"
  
- 🧱 **Modular Cabinet Builder**:
  - Dimension selection (Height, Width, Depth) using ComboBoxes
  - Dynamic addition of lockers (up to 7 lockers)
  - Display of the added lockers list with delete buttons

- 🧾 **Order History View** *(in progress)*

- 🔐 **User Authentication** *(planned)*

---

## 📁 Repository Structure

```bash
Kitbox/
├── Kitbox_app/             # Avalonia UI Frontend
├── KitboxAPI/              # ASP.NET Core Backend API
├── Models/                 # Shared Models (DTOs / Entities)
├── README.md               # This file
└── docs/                   # Screenshots and documentation

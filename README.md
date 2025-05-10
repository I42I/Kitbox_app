# Kitbox_app

Frontend de l’application **Kitbox** développé en **.NET 9** avec **AvaloniaUI** en architecture MVVM.

## 🎯 Objectif

Ce projet permet de configurer une armoire personnalisée (casier par casier) via une interface graphique intuitive, reliée à un backend API (`KitboxAPI`).

## ✅ Fonctionnalités actuelles

- 🎨 Interface d'accueil fonctionnelle avec design propre
- 📋 Menu déroulant pour la sélection des dimensions des casiers (lockers)
- ➕ Ajout de casiers avec ComboBox
- 🧭 Navigation MVVM en place via `INavigationService`
- 🔄 Liaison prête avec l’API backend

## 🚧 À venir

- 💾 Sauvegarde des armoires configurées
- 📦 Intégration avec la base de données via l’API
- 🧾 Vue des commandes (ViewOrders)
- 🔐 Vue de connexion (LoginView)
- 🖼️ Aperçu visuel 3D de l’armoire

## 📁 Structure du projet

```bash
Kitbox_app/
├── Views/
├── ViewModels/
├── Services/
├── Models/
├── App.axaml
├── Program.cs
└── ViewLocator.cs

# Backend – KitboxAPI

## Explication du backend – Projet KitboxAPI

Le backend du projet **KitboxAPI** a été développé en **ASP.NET Core Web API** (.NET 8) dans le cadre d’un projet académique en génie électrique (orientation informatique & électronique). Il constitue la partie serveur du système de configuration d’armoires modulaires, utilisée par une interface frontend en Avalonia.

---

### Objectif principal

L’API permet de :

- Gérer l’**authentification des utilisateurs**
- Créer des **armoires personnalisées** à partir de casiers
- Suivre les **commandes clients**
- Gérer le **stock des casiers**
- Lancer des **commandes fournisseurs** en cas de rupture

---

### Dossiers et structure du backend

- `Controllers/` : contient tous les **points d’entrée de l’API** REST (7 controllers)
- `Models/` : contient les **entités de base de données** (7 modèles)
- `Dtos/` : objets de transfert de données (partage frontend/backend)
- `Data/` : contexte Entity Framework et gestion de la base MariaDB

---

### Fonctionnalités principales par controller

| Contrôleur                  | Rôle de l’API                                      |
|-----------------------------|---------------------------------------------------|
| CabinetsController          | Gérer la composition des armoires                |
| CustomerOrdersController    | Créer et consulter les commandes clients         |
| LockersController           | Lister les dimensions de casiers disponibles     |
| LockersStockController      | Gérer les quantités disponibles pour chaque casier |
| StocksController            | Suivi global des mouvements de stock             |
| SupplierOrderController     | Création des commandes vers les fournisseurs     |
| SuppliersController         | Gestion des fournisseurs                         |

---

### Entités de la base de données

| Entité           | Description                                                        |
|------------------|---------------------------------------------------------------------|
| Cabinet          | Représente une armoire, composée de casiers                        |
| CustumerOrder    | Commande passée par un client (avec une faute à corriger)          |
| Locker           | Casiers disponibles (hauteur, largeur, profondeur)                 |
| LockerStock      | Quantité de chaque type de casier en stock                         |
| Stock            | Registre des entrées/sorties de stock                              |
| Supplier         | Informations sur les fournisseurs                                  |
| SupplierOrder    | Commandes vers les fournisseurs                                    |

---

### Fonctionnement global

1. Un utilisateur configure une armoire via le frontend.
2. Les casiers choisis sont envoyés à l’API (`POST /api/cabinets`).
3. L’armoire est liée à une commande client (`POST /api/customerorders`).
4. L’API vérifie la disponibilité des casiers en stock.
5. Si le stock est insuffisant, une commande fournisseur est automatiquement générée.

---

### Technologies utilisées

- **ASP.NET Core Web API (.NET 8)**
- **Entity Framework Core**
- **MariaDB** pour la base de données relationnelle
- **JWT** pour l’authentification sécurisée
- Architecture **MVC** + séparation des responsabilités

---

### Améliorations identifiées

- Correction du nom `CustumerOrder.cs` → `CustomerOrder.cs`
- Ajout d’un système de rôles utilisateurs (admin, client)
- Intégration d’un système de **validation métier** (stock, commandes invalides…)
- Documentation via **Swagger**
- Sécurité avancée (hash des mots de passe, middleware global d’erreur)
- Nettoyage du code en déplaçant la logique métier dans des services

---

### Résultat

Ce backend permet de gérer efficacement tout le processus de configuration, de commande, de gestion de stock et d’approvisionnement d’armoires personnalisées dans un contexte modulaire. Il constitue la couche serveur essentielle du projet Kitbox, avec une architecture claire, extensible, et connectée au frontend Avalonia via des appels REST.
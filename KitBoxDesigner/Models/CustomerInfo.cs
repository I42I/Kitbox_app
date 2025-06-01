using System;
using System.ComponentModel.DataAnnotations;

namespace KitBoxDesigner.Models
{
    /// <summary>
    /// Model for customer information during order completion
    /// </summary>
    public class CustomerInfo
    {
        [Required(ErrorMessage = "Le nom du client est requis")]
        [MinLength(2, ErrorMessage = "Le nom doit contenir au moins 2 caractères")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Le numéro de téléphone est requis")]
        [Phone(ErrorMessage = "Format de téléphone invalide")]
        public string Phone { get; set; } = "";

        [Required(ErrorMessage = "L'adresse est requise")]
        [MinLength(5, ErrorMessage = "L'adresse doit contenir au moins 5 caractères")]
        public string Address { get; set; } = "";

        [Range(0, double.MaxValue, ErrorMessage = "Le montant du dépôt doit être positif")]
        public decimal DepositAmount { get; set; } = 0;

        public string Notes { get; set; } = "";
    }
}

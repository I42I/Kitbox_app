using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kitbox_app
{
    /// <summary>
    /// Logique d'interaction pour SupplierManagementPage.xaml
    /// </summary>
    public partial class SupplierManagementPage : Page
    {
        private List<Supplier> suppliers = new List<Supplier>();

        public SupplierManagementPage()
        {
            InitializeComponent();
            LoadSuppliers();
        }

        // Load Suppliers into the ListBox
        private void LoadSuppliers()
        {
            SupplierList.Items.Clear();
            foreach (var supplier in suppliers)
            {
                SupplierList.Items.Add($"{supplier.Name} - {supplier.Price}€ - {supplier.DeliveryTime} jours");
            }
        }

        // Add a New Supplier
        private void AddSupplier_Click(object sender, RoutedEventArgs e)
        {
            string name = SupplierNameInput.Text;
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(SupplierPriceInput.Text) || string.IsNullOrEmpty(SupplierDelayInput.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (decimal.TryParse(SupplierPriceInput.Text, out decimal price) && int.TryParse(SupplierDelayInput.Text, out int delay))
            {
                suppliers.Add(new Supplier { Name = name, Price = price, DeliveryTime = delay });
                LoadSuppliers();
                ClearFields();
            }
            else
            {
                MessageBox.Show("Prix ou délai invalide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Edit a Selected Supplier
        private void EditSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (SupplierList.SelectedIndex < 0)
            {
                MessageBox.Show("Veuillez sélectionner un fournisseur.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string name = SupplierNameInput.Text;
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(SupplierPriceInput.Text) || string.IsNullOrEmpty(SupplierDelayInput.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (decimal.TryParse(SupplierPriceInput.Text, out decimal price) && int.TryParse(SupplierDelayInput.Text, out int delay))
            {
                suppliers[SupplierList.SelectedIndex] = new Supplier { Name = name, Price = price, DeliveryTime = delay };
                LoadSuppliers();
                ClearFields();
            }
            else
            {
                MessageBox.Show("Prix ou délai invalide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Delete a Selected Supplier
        private void DeleteSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (SupplierList.SelectedIndex < 0)
            {
                MessageBox.Show("Veuillez sélectionner un fournisseur à supprimer.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            suppliers.RemoveAt(SupplierList.SelectedIndex);
            LoadSuppliers();
            ClearFields();
        }

        // Refresh the Supplier List
        private void RefreshSuppliers_Click(object sender, RoutedEventArgs e)
        {
            LoadSuppliers();
            MessageBox.Show("Liste des fournisseurs mise à jour !");
        }

        // Clear Input Fields
        private void ClearFields()
        {
            SupplierNameInput.Clear();
            SupplierPriceInput.Clear();
            SupplierDelayInput.Clear();
        }

        // Return to Manager Menu
        private void BackToManager_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ManagerPage());
        }
    }

    // Supplier Class
    public class Supplier
    {
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public int DeliveryTime { get; set; }
    }
}

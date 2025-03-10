using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kitbox_app.Pages
{
    public partial class ConfigPage : Page
    {
        private int currentLockerIndex = 0;
        private List<Locker> lockers = new List<Locker>();
        private const double MAX_HEIGHT = 333.33;

        public ConfigPage()
        {
            InitializeComponent();
            InitializeLockers(1);
        }

        // Initialisation des casiers avec valeurs par défaut
        private void InitializeLockers(int count)
        {
            lockers.Clear();
            int firstWidth = 32;
            int firstDepth = 32;

            for (int i = 0; i < count; i++)
            {
                if (i == 0) // Premier casier
                {
                    lockers.Add(new Locker { Height = 32, Width = 32, Depth = 32, Color = "Blanc", HasDoors = false });
                }
                else // Autres casiers
                {
                    lockers.Add(new Locker { Height = 32, Width = firstWidth, Depth = firstDepth, Color = "Blanc", HasDoors = false });
                }
            }
            currentLockerIndex = 0;
            UpdateLockerDisplay();
        }

        // Met à jour l'affichage du casier actuel
        private void UpdateLockerDisplay()
        {
            if (currentLockerIndex >= lockers.Count) return;

            var locker = lockers[currentLockerIndex];
            LockerTitle.Text = $"Casier {currentLockerIndex + 1}";
            HeightInput.SelectedItem = locker.Height.ToString();
            ColorInput.SelectedItem = locker.Color;
            HasDoors.IsChecked = locker.HasDoors;

            if (currentLockerIndex == 0) // Premier casier
            {
                WidthInput.SelectedItem = locker.Width.ToString();
                DepthInput.SelectedItem = locker.Depth.ToString();
                WidthInput.IsEnabled = true;
                DepthInput.IsEnabled = true;
            }
            else // Autres casiers
            {
                WidthInput.SelectedItem = lockers[0].Width.ToString();
                DepthInput.SelectedItem = lockers[0].Depth.ToString();
                WidthInput.IsEnabled = false;
                DepthInput.IsEnabled = false;
            }
        }

        // Modification du nombre de casiers
        private void LockerCount_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (LockerCount.SelectedItem is ComboBoxItem selectedItem && int.TryParse(selectedItem.Content.ToString(), out int count))
            {
                if (CalculateTotalHeight(count) > MAX_HEIGHT)
                {
                    MessageBox.Show("Impossible d'ajouter plus de casiers : hauteur totale dépassée !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                InitializeLockers(count);
            }
        }

        // Gestion du carrousel : Casier précédent
        private void PreviousLocker_Click(object sender, RoutedEventArgs e)
        {
            if (currentLockerIndex > 0)
            {
                currentLockerIndex--;
                UpdateLockerDisplay();
            }
        }

        // Gestion du carrousel : Casier suivant
        private void NextLocker_Click(object sender, RoutedEventArgs e)
        {
            if (currentLockerIndex < lockers.Count - 1)
            {
                currentLockerIndex++;
                UpdateLockerDisplay();
            }
        }

        // Gestion de la hauteur
        private void HeightInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentLockerIndex >= lockers.Count) return;

            if (HeightInput.SelectedItem is ComboBoxItem selectedItem && int.TryParse(selectedItem.Content.ToString(), out int newHeight))
            {
                lockers[currentLockerIndex].Height = newHeight;

                if (CalculateTotalHeight(lockers.Count) > MAX_HEIGHT)
                {
                    MessageBox.Show("Commande impossible : hauteur totale dépassée (max : 333,33 cm) !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    lockers[currentLockerIndex].Height = 32; // Réinitialisation
                    HeightInput.SelectedItem = "32";
                }
            }
        }

        // Calcul de la hauteur totale
        private double CalculateTotalHeight(int count)
        {
            return lockers.Take(count).Sum(locker => locker.Height);
        }

        // Ajout au panier
        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (CalculateTotalHeight(lockers.Count) > MAX_HEIGHT)
            {
                MessageBox.Show("Impossible d'ajouter au panier : hauteur totale dépassée !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Configuration ajoutée au panier !");
        }

        // Retour au menu principal
        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomePage());
        }
    }

    // Classe Locker pour stocker les informations de chaque casier
    public class Locker
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int Depth { get; set; }
        public string? Color { get; set; }
        public bool HasDoors { get; set; }
    }
}

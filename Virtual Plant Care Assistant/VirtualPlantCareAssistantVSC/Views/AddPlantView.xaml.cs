using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using VirtualPlantCareAssistantVSC.Utilities;
using MySql.Data.MySqlClient;

namespace VirtualPlantCareAssistantVSC.Views
{
    public partial class AddPlantView : Page
    {
        private string _selectedImagePath = "";

        public AddPlantView()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new DashboardView());
        }

        private void ChooseImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                _selectedImagePath = dlg.FileName;
                PlantImagePreview.Source = new BitmapImage(new Uri(_selectedImagePath));
                ChooseImageButton.Visibility = Visibility.Collapsed;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            string species = SpeciesTextBox.Text.Trim();
            string notes = NotesTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter a plant name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(FrequencyTextBox.Text, out int waterFreq) || waterFreq <= 0)
            {
                MessageBox.Show("Please enter a valid watering frequency (number of days).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Fertilization frequency — default to 30 days if left blank
            int fertilizationFreq = 30;
            if (!string.IsNullOrWhiteSpace(FertilizationFrequencyTextBox.Text))
            {
                if (!int.TryParse(FertilizationFrequencyTextBox.Text, out fertilizationFreq) || fertilizationFreq <= 0)
                {
                    MessageBox.Show("Please enter a valid fertilization frequency (number of days).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            string imagePathToSave = _selectedImagePath;
            if (string.IsNullOrEmpty(imagePathToSave))
            {
                string exeDir = AppDomain.CurrentDomain.BaseDirectory;
                imagePathToSave = Path.GetFullPath(Path.Combine(exeDir, @"Assets\Images\default_plant.png"));
            }

            try
            {
                using (var conn = Data.DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO Plant (UserId, Name, Species, WateringFrequencyDays, FertilizationFrequencyDays, ImagePath) 
                                     VALUES (@userId, @name, @species, @waterFreq, @fertilizationFreq, @imagePath)";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", UserSession.CurrentUser.UserId);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@species", species);
                        cmd.Parameters.AddWithValue("@waterFreq", waterFreq);
                        cmd.Parameters.AddWithValue("@fertilizationFreq", fertilizationFreq);
                        cmd.Parameters.AddWithValue("@imagePath", imagePathToSave);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show($"'{name}' added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService?.Navigate(new DashboardView());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save plant: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

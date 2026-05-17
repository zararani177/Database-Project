using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using VirtualPlantCareAssistantVSC.Data;
using VirtualPlantCareAssistantVSC.Models;
using VirtualPlantCareAssistantVSC.Utilities;

namespace VirtualPlantCareAssistantVSC.Views
{
    public partial class DashboardView : Page
    {
        public ObservableCollection<Plant> Plants { get; set; } = new ObservableCollection<Plant>();
        private System.Collections.Generic.List<Plant> _allPlants = new System.Collections.Generic.List<Plant>();

        public DashboardView()
        {
            InitializeComponent();
            if (UserSession.IsLoggedIn)
                WelcomeTextBlock.Text = $"Welcome, {UserSession.CurrentUser.Name}!";
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (UserSession.IsLoggedIn)
                SeedDefaultPlants();
            LoadPlants();
        }

        private void SeedDefaultPlants()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string checkQuery = "SELECT COUNT(*) FROM Plant WHERE UserId = @userId";
                    using (var checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@userId", UserSession.CurrentUser.UserId);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0) return;
                    }

                    string defaultImage = "pack://application:,,,/Assets/Images/default_plant.png";
                    string insertQuery = @"
                        INSERT INTO Plant (UserId, Name, Species, WateringFrequencyDays, LastWatered, FertilizationFrequencyDays, LastFertilized, ImagePath)
                        VALUES
                        (@uid, 'Fernie',  'Boston Fern',        3,  @w1, 30, @f1, @img),
                        (@uid, 'Spike',   'Cactus',            14,  @w2, 90, @f2, @img),
                        (@uid, 'Monstie', 'Monstera Deliciosa',  7,  @w3, 21, @f3, @img);";

                    using (var cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", UserSession.CurrentUser.UserId);
                        cmd.Parameters.AddWithValue("@w1", DateTime.Now.AddDays(-2));
                        cmd.Parameters.AddWithValue("@w2", DateTime.Now.AddDays(-13));
                        cmd.Parameters.AddWithValue("@w3", DateTime.Now.AddDays(-6));
                        cmd.Parameters.AddWithValue("@f1", DateTime.Now.AddDays(-25));
                        cmd.Parameters.AddWithValue("@f2", DateTime.Now.AddDays(-80));
                        cmd.Parameters.AddWithValue("@f3", DateTime.Now.AddDays(-20));
                        cmd.Parameters.AddWithValue("@img", defaultImage);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Seed Failed: " + ex.Message);
            }
        }

        private void LoadPlants()
        {
            _allPlants.Clear();
            if (!UserSession.IsLoggedIn) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Plant WHERE UserId = @userId";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", UserSession.CurrentUser.UserId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int plantId = reader.GetInt32("PlantId");
                                string name = reader.GetString("Name");
                                string species = reader.IsDBNull(reader.GetOrdinal("Species")) ? "" : reader.GetString("Species");
                                int waterFreq = reader.GetInt32("WateringFrequencyDays");
                                DateTime? lastWatered = reader.IsDBNull(reader.GetOrdinal("LastWatered")) ? (DateTime?)null : reader.GetDateTime("LastWatered");

                                int fertilizationFreq = 30;
                                if (!reader.IsDBNull(reader.GetOrdinal("FertilizationFrequencyDays")))
                                    fertilizationFreq = reader.GetInt32("FertilizationFrequencyDays");

                                DateTime? lastFertilized = reader.IsDBNull(reader.GetOrdinal("LastFertilized")) ? (DateTime?)null : reader.GetDateTime("LastFertilized");
                                string imagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath")) ? "" : reader.GetString("ImagePath");

                                _allPlants.Add(new Plant(plantId, UserSession.CurrentUser.UserId, name, species,
                                    waterFreq, lastWatered, fertilizationFreq, lastFertilized, imagePath));
                            }
                        }
                    }
                }
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load plants: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (SearchTextBox == null) return;
            
            string filter = SearchTextBox.Text.ToLower().Trim();
            Plants.Clear();

            foreach (var plant in _allPlants)
            {
                if (string.IsNullOrWhiteSpace(filter) || 
                    plant.Name.ToLower().Contains(filter) || 
                    (plant.Species != null && plant.Species.ToLower().Contains(filter)))
                {
                    Plants.Add(plant);
                }
            }
            
            PlantsItemsControl.ItemsSource = null;
            PlantsItemsControl.ItemsSource = Plants;
        }

        // --- Navigation ---
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            UserSession.Logout();
            NavigationService?.Navigate(new LoginView());
        }

        private void AddPlantButton_Click(object sender, RoutedEventArgs e) =>
            NavigationService?.Navigate(new AddPlantView());

        private void CareScheduleButton_Click(object sender, RoutedEventArgs e) =>
            NavigationService?.Navigate(new CareScheduleView());

        private void RemindersButton_Click(object sender, RoutedEventArgs e) =>
            NavigationService?.Navigate(new ReminderView());

        private void CareHistoryButton_Click(object sender, RoutedEventArgs e) =>
            NavigationService?.Navigate(new CareHistoryView());

        // --- Plant Card Click → Detail ---
        private void PlantCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Plant plant)
                NavigationService?.Navigate(new PlantDetailView(plant));
        }

        // --- Water ---
        private void WaterButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn && btn.Tag is int plantId)) return;
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE Plant SET LastWatered = @now WHERE PlantId = @plantId AND UserId = @userId";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@now", DateTime.Now);
                        cmd.Parameters.AddWithValue("@plantId", plantId);
                        cmd.Parameters.AddWithValue("@userId", UserSession.CurrentUser.UserId);
                        cmd.ExecuteNonQuery();
                    }
                }
                DatabaseHelper.LogCareEvent(plantId, UserSession.CurrentUser.UserId, "Watering");
                LoadPlants();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to water plant: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- Fertilize ---
        private void FertilizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn && btn.Tag is int plantId)) return;
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE Plant SET LastFertilized = @now WHERE PlantId = @plantId AND UserId = @userId";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@now", DateTime.Now);
                        cmd.Parameters.AddWithValue("@plantId", plantId);
                        cmd.Parameters.AddWithValue("@userId", UserSession.CurrentUser.UserId);
                        cmd.ExecuteNonQuery();
                    }
                }
                DatabaseHelper.LogCareEvent(plantId, UserSession.CurrentUser.UserId, "Fertilization");
                LoadPlants();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to fertilize plant: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- Delete ---
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn && btn.Tag is int plantId)) return;
            var result = MessageBox.Show("Are you sure you want to delete this plant?", "Confirm Delete",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Plant WHERE PlantId = @plantId AND UserId = @userId";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@plantId", plantId);
                        cmd.Parameters.AddWithValue("@userId", UserSession.CurrentUser.UserId);
                        cmd.ExecuteNonQuery();
                    }
                }
                LoadPlants();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete plant: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

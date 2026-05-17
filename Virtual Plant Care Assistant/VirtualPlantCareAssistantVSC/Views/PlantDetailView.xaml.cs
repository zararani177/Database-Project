using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VirtualPlantCareAssistantVSC.Data;
using VirtualPlantCareAssistantVSC.Models;
using VirtualPlantCareAssistantVSC.Utilities;

namespace VirtualPlantCareAssistantVSC.Views
{
    public partial class PlantDetailView : Page
    {
        private Plant _plant;

        public PlantDetailView(Plant plant)
        {
            InitializeComponent();
            _plant = plant;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            BindPlantData();
            LoadCareLog();
        }

        private void BindPlantData()
        {
            // Hero image
            try
            {
                HeroImage.Source = new BitmapImage(new Uri(_plant.ImagePath, UriKind.RelativeOrAbsolute));
            }
            catch { /* use null / blank if image fails */ }

            PlantNameText.Text = _plant.Name;
            PlantSpeciesText.Text = _plant.Species;

            // Watering card
            WaterFreqText.Text = $"Every {_plant.WateringFrequencyDays} day(s)";
            LastWateredText.Text = _plant.LastWatered.HasValue ? _plant.LastWatered.Value.ToString("MMM dd, yyyy") : "Never";
            NextWateringText.Text = _plant.NextWateringDate.HasValue ? _plant.NextWateringDate.Value.ToString("MMM dd, yyyy") : "—";
            WateringStatusText.Text = _plant.WateringStatus;
            WateringStatusBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_plant.WateringStatusColor));

            // Fertilization card
            FertilizationFreqText.Text = $"Every {_plant.FertilizationFrequencyDays} day(s)";
            LastFertilizedText.Text = _plant.LastFertilized.HasValue ? _plant.LastFertilized.Value.ToString("MMM dd, yyyy") : "Never";
            NextFertilizationText.Text = _plant.NextFertilizationDate.HasValue ? _plant.NextFertilizationDate.Value.ToString("MMM dd, yyyy") : "—";
            FertilizationStatusText.Text = _plant.FertilizationStatus;
            FertilizationStatusBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_plant.FertilizationStatusColor));
        }

        private void LoadCareLog()
        {
            var entries = new List<CareLogEntry>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT LogId, CareType, CaredAt, Notes FROM CareLog
                                     WHERE PlantId = @plantId
                                     ORDER BY CaredAt DESC LIMIT 15";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@plantId", _plant.PlantId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                entries.Add(new CareLogEntry
                                {
                                    LogId = reader.GetInt32("LogId"),
                                    PlantId = _plant.PlantId,
                                    PlantName = _plant.Name,
                                    CareType = reader.GetString("CareType"),
                                    CaredAt = reader.GetDateTime("CaredAt"),
                                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? "" : reader.GetString("Notes")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Care log load failed: " + ex.Message);
            }

            if (entries.Count == 0)
            {
                CareLogGrid.Visibility = Visibility.Collapsed;
                NoLogText.Visibility = Visibility.Visible;
            }
            else
            {
                CareLogGrid.ItemsSource = entries;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new DashboardView());
        }

        private void WaterNow_Click(object sender, RoutedEventArgs e)
        {
            PerformCare("Watering", "LastWatered");
        }

        private void FertilizeNow_Click(object sender, RoutedEventArgs e)
        {
            PerformCare("Fertilization", "LastFertilized");
        }

        private void PerformCare(string careType, string columnName)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = $"UPDATE Plant SET {columnName} = @now WHERE PlantId = @plantId AND UserId = @userId";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@now", DateTime.Now);
                        cmd.Parameters.AddWithValue("@plantId", _plant.PlantId);
                        cmd.Parameters.AddWithValue("@userId", UserSession.CurrentUser.UserId);
                        cmd.ExecuteNonQuery();
                    }
                }

                DatabaseHelper.LogCareEvent(_plant.PlantId, UserSession.CurrentUser.UserId, careType);

                // Refresh local plant state
                if (careType == "Watering")
                    _plant.LastWatered = DateTime.Now;
                else
                    _plant.LastFertilized = DateTime.Now;

                BindPlantData();
                LoadCareLog();

                MessageBox.Show($"{_plant.Name} has been {careType.ToLower()}d! ✅", "Done",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

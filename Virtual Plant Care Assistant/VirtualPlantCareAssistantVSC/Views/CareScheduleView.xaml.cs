using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VirtualPlantCareAssistantVSC.Data;
using VirtualPlantCareAssistantVSC.Models;
using VirtualPlantCareAssistantVSC.Utilities;

namespace VirtualPlantCareAssistantVSC.Views
{
    public partial class CareScheduleView : Page
    {
        public CareScheduleView()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSchedule();
        }

        private void LoadSchedule()
        {
            var items = new List<ScheduleItem>();
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
                                string img = reader.IsDBNull(reader.GetOrdinal("ImagePath")) ? "" : reader.GetString("ImagePath");

                                int waterFreq = reader.GetInt32("WateringFrequencyDays");
                                DateTime? lastWatered = reader.IsDBNull(reader.GetOrdinal("LastWatered")) ? (DateTime?)null : reader.GetDateTime("LastWatered");

                                int fertFreq = reader.IsDBNull(reader.GetOrdinal("FertilizationFrequencyDays")) ? 30 : reader.GetInt32("FertilizationFrequencyDays");
                                DateTime? lastFertilized = reader.IsDBNull(reader.GetOrdinal("LastFertilized")) ? (DateTime?)null : reader.GetDateTime("LastFertilized");

                                // Watering task
                                DateTime waterDue = lastWatered.HasValue
                                    ? lastWatered.Value.AddDays(waterFreq)
                                    : DateTime.Today;
                                if ((waterDue.Date - DateTime.Today).TotalDays <= 14)
                                {
                                    items.Add(new ScheduleItem
                                    {
                                        PlantId = plantId, PlantName = name, ImagePath = img,
                                        CareType = "Watering", DueDate = waterDue.Date
                                    });
                                }

                                // Fertilization task
                                DateTime fertDue = lastFertilized.HasValue
                                    ? lastFertilized.Value.AddDays(fertFreq)
                                    : DateTime.Today;
                                if ((fertDue.Date - DateTime.Today).TotalDays <= 14)
                                {
                                    items.Add(new ScheduleItem
                                    {
                                        PlantId = plantId, PlantName = name, ImagePath = img,
                                        CareType = "Fertilization", DueDate = fertDue.Date
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load schedule: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Sort: overdue first, then by date
            var sorted = items.OrderBy(i => i.DueDate).ToList();

            if (sorted.Count == 0)
            {
                NoScheduleText.Visibility = Visibility.Visible;
                ScheduleItemsControl.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoScheduleText.Visibility = Visibility.Collapsed;
                ScheduleItemsControl.Visibility = Visibility.Visible;
                ScheduleItemsControl.ItemsSource = sorted;
            }
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e) =>
            NavigationService?.Navigate(new DashboardView());
        private void RemindersButton_Click(object sender, RoutedEventArgs e) =>
            NavigationService?.Navigate(new ReminderView());
        private void CareHistoryButton_Click(object sender, RoutedEventArgs e) =>
            NavigationService?.Navigate(new CareHistoryView());

        private void BackButton_Click(object sender, RoutedEventArgs e) =>
            NavigationService?.Navigate(new DashboardView());
    }
}

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VirtualPlantCareAssistantVSC.Data;
using VirtualPlantCareAssistantVSC.Models;
using VirtualPlantCareAssistantVSC.Utilities;

namespace VirtualPlantCareAssistantVSC.Views
{
    public partial class ReminderView : Page
    {
        public ReminderView()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadReminders();
        }

        private void LoadReminders()
        {
            var reminders = new List<ReminderItem>();
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

                                // Check watering
                                DateTime waterDue = lastWatered.HasValue
                                    ? lastWatered.Value.AddDays(waterFreq).Date
                                    : DateTime.Today;
                                int waterDays = (int)(DateTime.Today - waterDue).TotalDays;
                                if (waterDays >= 0)
                                {
                                    reminders.Add(new ReminderItem
                                    {
                                        PlantId = plantId, PlantName = name, ImagePath = img,
                                        CareType = "Watering", DaysOverdue = waterDays
                                    });
                                }

                                // Check fertilization
                                DateTime fertDue = lastFertilized.HasValue
                                    ? lastFertilized.Value.AddDays(fertFreq).Date
                                    : DateTime.Today;
                                int fertDays = (int)(DateTime.Today - fertDue).TotalDays;
                                if (fertDays >= 0)
                                {
                                    reminders.Add(new ReminderItem
                                    {
                                        PlantId = plantId, PlantName = name, ImagePath = img,
                                        CareType = "Fertilization", DaysOverdue = fertDays
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load reminders: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Sort: most overdue first
            reminders.Sort((a, b) => b.DaysOverdue.CompareTo(a.DaysOverdue));

            if (reminders.Count == 0)
            {
                RemindersItemsControl.Visibility = Visibility.Collapsed;
                AllGoodPanel.Visibility = Visibility.Visible;
            }
            else
            {
                RemindersItemsControl.Visibility = Visibility.Visible;
                AllGoodPanel.Visibility = Visibility.Collapsed;
                RemindersItemsControl.ItemsSource = reminders;
            }
        }

        private void ReminderAction_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn && btn.Tag is ReminderItem item)) return;

            string columnName = item.CareType == "Watering" ? "LastWatered" : "LastFertilized";
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = $"UPDATE Plant SET {columnName} = @now WHERE PlantId = @plantId AND UserId = @userId";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@now", DateTime.Now);
                        cmd.Parameters.AddWithValue("@plantId", item.PlantId);
                        cmd.Parameters.AddWithValue("@userId", UserSession.CurrentUser.UserId);
                        cmd.ExecuteNonQuery();
                    }
                }
                DatabaseHelper.LogCareEvent(item.PlantId, UserSession.CurrentUser.UserId, item.CareType);
                MessageBox.Show($"✅ {item.PlantName} has been {item.CareType.ToLower()}d!", "Done",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                LoadReminders(); // Refresh list
            }
            catch (Exception ex)
            {
                MessageBox.Show("Action failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e) =>
            NavigationService?.Navigate(new DashboardView());
        private void CareScheduleButton_Click(object sender, RoutedEventArgs e) =>
            NavigationService?.Navigate(new CareScheduleView());
        private void CareHistoryButton_Click(object sender, RoutedEventArgs e) =>
            NavigationService?.Navigate(new CareHistoryView());

        private void BackButton_Click(object sender, RoutedEventArgs e) =>
            NavigationService?.Navigate(new DashboardView());
    }
}

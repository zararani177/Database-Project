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
    public partial class CareHistoryView : Page
    {
        private List<CareLogEntry> _allEntries = new List<CareLogEntry>();

        public CareHistoryView()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadHistory();
        }

        private void LoadHistory()
        {
            _allEntries.Clear();
            if (!UserSession.IsLoggedIn) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT cl.LogId, cl.PlantId, p.Name AS PlantName, cl.CareType, cl.CaredAt, cl.Notes
                        FROM CareLog cl
                        JOIN Plant p ON cl.PlantId = p.PlantId
                        WHERE cl.UserId = @userId
                        ORDER BY cl.CaredAt DESC";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", UserSession.CurrentUser.UserId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _allEntries.Add(new CareLogEntry
                                {
                                    LogId = reader.GetInt32("LogId"),
                                    PlantId = reader.GetInt32("PlantId"),
                                    PlantName = reader.GetString("PlantName"),
                                    UserId = UserSession.CurrentUser.UserId,
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
                MessageBox.Show("Failed to load care history: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (FilterComboBox == null || HistoryGrid == null) return;
            string filter = (FilterComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "All Events";
            List<CareLogEntry> filtered;

            if (filter == "Watering Only")
                filtered = _allEntries.Where(e => e.CareType == "Watering").ToList();
            else if (filter == "Fertilization Only")
                filtered = _allEntries.Where(e => e.CareType == "Fertilization").ToList();
            else
                filtered = _allEntries;

            if (filtered.Count == 0)
            {
                HistoryGrid.Visibility = Visibility.Collapsed;
                NoHistoryText.Visibility = Visibility.Visible;
            }
            else
            {
                HistoryGrid.Visibility = Visibility.Visible;
                NoHistoryText.Visibility = Visibility.Collapsed;
                HistoryGrid.ItemsSource = filtered;
            }
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilter();

        private void DashboardButton_Click(object sender, RoutedEventArgs e) =>
            NavigationService?.Navigate(new DashboardView());
        private void CareScheduleButton_Click(object sender, RoutedEventArgs e) =>
            NavigationService?.Navigate(new CareScheduleView());
        private void RemindersButton_Click(object sender, RoutedEventArgs e) =>
            NavigationService?.Navigate(new ReminderView());

        private void BackButton_Click(object sender, RoutedEventArgs e) =>
            NavigationService?.Navigate(new DashboardView());
    }
}

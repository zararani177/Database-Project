using System;
using MySql.Data.MySqlClient;

namespace VirtualPlantCareAssistantVSC.Data
{
    public class DatabaseHelper
    {
        private static string serverConnectionString = "Server=localhost;Port=3306;Uid=root;Pwd=YOUR_PASSWORD;";
        private static string connectionString = "Server=localhost;Port=3306;Database=VirtualPlantCareAssistantVSC1_1;Uid=root;Pwd=YOUR_PASSWORD;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public static bool TestConnection()
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection Failed: " + ex.Message);
                return false;
            }
        }

        public static void InitializeDatabase()
        {
            try
            {
                using (MySqlConnection serverConn = new MySqlConnection(serverConnectionString))
                {
                    serverConn.Open();
                    using (var cmd = new MySqlCommand("CREATE DATABASE IF NOT EXISTS VirtualPlantCareAssistantVSC1_1;", serverConn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    // User table
                    string createUserTable = @"
                        CREATE TABLE IF NOT EXISTS User (
                            UserId INT AUTO_INCREMENT PRIMARY KEY,
                            Name VARCHAR(100) NOT NULL,
                            Email VARCHAR(150) UNIQUE NOT NULL,
                            Password VARCHAR(255) NOT NULL
                        );";
                    using (var cmd = new MySqlCommand(createUserTable, conn))
                        cmd.ExecuteNonQuery();

                    // Plant table (full schema including fertilization)
                    string createPlantTable = @"
                        CREATE TABLE IF NOT EXISTS Plant (
                            PlantId INT AUTO_INCREMENT PRIMARY KEY,
                            UserId INT NOT NULL,
                            Name VARCHAR(100) NOT NULL,
                            Species VARCHAR(100),
                            WateringFrequencyDays INT DEFAULT 7,
                            LastWatered DATETIME,
                            FertilizationFrequencyDays INT DEFAULT 30,
                            LastFertilized DATETIME,
                            ImagePath VARCHAR(255),
                            FOREIGN KEY (UserId) REFERENCES User(UserId) ON DELETE CASCADE
                        );";
                    using (var cmd = new MySqlCommand(createPlantTable, conn))
                        cmd.ExecuteNonQuery();

                    // CareLog table
                    string createCareLogTable = @"
                        CREATE TABLE IF NOT EXISTS CareLog (
                            LogId INT AUTO_INCREMENT PRIMARY KEY,
                            PlantId INT NOT NULL,
                            UserId INT NOT NULL,
                            CareType VARCHAR(50) NOT NULL,
                            CaredAt DATETIME NOT NULL,
                            Notes VARCHAR(255),
                            FOREIGN KEY (PlantId) REFERENCES Plant(PlantId) ON DELETE CASCADE
                        );";
                    using (var cmd = new MySqlCommand(createCareLogTable, conn))
                        cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Database Initialization Failed: " + ex.Message);
            }
        }

        public static void LogCareEvent(int plantId, int userId, string careType, string notes = "")
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO CareLog (PlantId, UserId, CareType, CaredAt, Notes) 
                                     VALUES (@plantId, @userId, @careType, @now, @notes)";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@plantId", plantId);
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@careType", careType);
                        cmd.Parameters.AddWithValue("@now", DateTime.Now);
                        cmd.Parameters.AddWithValue("@notes", notes ?? "");
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CareLog insert failed: " + ex.Message);
            }
        }
    }
}

using System.Windows;
using System.Windows.Controls;

namespace VirtualPlantCareAssistantVSC.Views
{
    public partial class LoginView : Page
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both email and password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var conn = Data.DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT UserId, Name, Email FROM User WHERE Email = @email AND Password = @password";
                    using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int id = reader.GetInt32("UserId");
                                string name = reader.GetString("Name");
                                string dbEmail = reader.GetString("Email");

                                // Create session and navigate
                                Utilities.UserSession.Login(new Models.User(id, name, dbEmail));
                                NavigationService?.Navigate(new DashboardView());
                            }
                            else
                            {
                                MessageBox.Show("Invalid email or password. Please try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Database connection error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new SignUpView());
        }
    }
}

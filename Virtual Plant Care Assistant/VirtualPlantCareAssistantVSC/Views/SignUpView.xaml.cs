using System.Windows;
using System.Windows.Controls;
using VirtualPlantCareAssistantVSC.Utilities;

namespace VirtualPlantCareAssistantVSC.Views
{
    public partial class SignUpView : Page
    {
        public SignUpView()
        {
            InitializeComponent();
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var conn = Data.DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    
                    // Check if email already exists
                    string checkQuery = "SELECT COUNT(*) FROM User WHERE Email = @email";
                    using (var checkCmd = new MySql.Data.MySqlClient.MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@email", email);
                        int count = System.Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            MessageBox.Show("This email is already registered. Please log in.", "Account Exists", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }

                    // Insert new user
                    string insertQuery = "INSERT INTO User (Name, Email, Password) VALUES (@name, @email, @password); SELECT LAST_INSERT_ID();";
                    using (var insertCmd = new MySql.Data.MySqlClient.MySqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@name", name);
                        insertCmd.Parameters.AddWithValue("@email", email);
                        insertCmd.Parameters.AddWithValue("@password", password);

                        int newUserId = System.Convert.ToInt32(insertCmd.ExecuteScalar());

                        // Automatically log them in after registration
                        UserSession.Login(new Models.User(newUserId, name, email));
                        NavigationService?.Navigate(new DashboardView());
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Database connection error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new LoginView());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new LoginView());
        }
    }
}

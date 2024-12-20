using System;
using System.Data.SQLite;
using System.Configuration;
using System.Windows;
using InventoryManagementSystem;

namespace project_ex
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Event handler for Username TextBox GotFocus event
        private void UsernameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Text == "Enter Username")
            {
                UsernameTextBox.Clear();
            }
        }

        // Event handler for Username TextBox LostFocus event
        private void UsernameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
            {
                UsernameTextBox.Text = "Enter Username";
            }
        }

        // Event handler for PasswordBox GotFocus event
        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password == "Enter Password")
            {
                PasswordBox.Clear();
            }
        }

        // Event handler for PasswordBox LostFocus event
        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                PasswordBox.Password = "Enter Password";
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim(); // Trim to remove whitespace
            string password = PasswordBox.Password.Trim(); // PasswordBox is used for password input

            try
            {
                // Early check for empty username or password
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Username or password cannot be empty.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Retrieve connection string from App.config
                string connectionString = ConfigurationManager.ConnectionStrings["InventoryDB"]?.ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("The connection string 'InventoryDB' is missing or not found in App.config.");
                }

                // Using SQLiteConnection to connect to SQLite database
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();

                    // Query to validate user credentials (assuming password is hashed in DB)
                    string query = "SELECT Role FROM Users WHERE Username = @Username AND PasswordHash = @PasswordHash";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@PasswordHash", password); // Here, consider hashing the password before comparing

                        // Execute the query
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            string role = result.ToString();
                            MessageBox.Show($"Login successful! Welcome, {role}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Proceed to dashboard
                            Dashboard dashboard = new Dashboard(role);
                            dashboard.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (SQLiteException sqlEx)
            {
                MessageBox.Show($"Database Error: {sqlEx.Message}", "SQL Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

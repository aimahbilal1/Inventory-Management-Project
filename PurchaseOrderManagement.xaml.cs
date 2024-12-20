using System;
using System.Windows;
using System.Windows.Controls;
using InventoryManagementSystem;

namespace InventoryManagement
{
    public partial class PurchaseOrderManagementWindow : Window
    {
        public PurchaseOrderManagementWindow()
        {
            InitializeComponent();
        }

        // Method to handle order creation
        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            string supplier = ((ComboBoxItem)supplierComboBox.SelectedItem)?.Content.ToString();
            string orderStatus = ((ComboBoxItem)orderStatusComboBox.SelectedItem)?.Content.ToString();

            if (string.IsNullOrEmpty(supplier) || string.IsNullOrEmpty(orderStatus))
            {
                MessageBox.Show("Please select a supplier and order status.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Simulate saving the order (you can link to a database here)
            MessageBox.Show($"Order Created: Supplier - {supplier}, Status - {orderStatus}", "Order Created", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide the current window
            this.Hide();

            // Create a new instance of the Dashboard window
            Dashboard dashboardWindow = new Dashboard("userRole"); // Pass the appropriate argument for the role

            // Show the Dashboard window
            dashboardWindow.Show();
        }

        // Method to track order status
        private void TrackOrderButton_Click(object sender, RoutedEventArgs e)
        {
            string orderID = orderIDTextBox.Text;
            if (string.IsNullOrEmpty(orderID))
            {
                MessageBox.Show("Please enter a valid Order ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Simulate order tracking (you can fetch real data from your database here)
            string orderStatus = "Pending"; // Example status
            MessageBox.Show($"Order ID {orderID} Status: {orderStatus}", "Order Status", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

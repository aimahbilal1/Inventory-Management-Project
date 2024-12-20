using System;
using System.Data.SQLite;
using System.Windows;
using InventoryManagement;
using SalesOrderManagementApp;


namespace InventoryManagementSystem
{
    public partial class Dashboard : Window
    {
        private string userRole;

        public Dashboard(string role="defaultRole")
        {
            if (string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Error: Invalid role.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Prevent loading the Dashboard if role is invalid
            }

            InitializeComponent();
            userRole = role;
            LoadDashboardData();
        }
        private void OpenPurchaseOrderManagement(object sender, RoutedEventArgs e)
        {
            var window = new PurchaseOrderManagementWindow();
            window.Show();
        }
        private void LoadDashboardData()
        {
            try
            {
                string connectionString = "Data Source=C:\\\\Users\\\\noora\\\\source\\\\repos\\\\project ex\\\\project ex\\\\project ex\\\\InventoryManagement.db;Version=3;";

                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();

                    // Load total products
                    TotalProductsTextBlock.Text = ExecuteScalarQuery(conn, "SELECT COUNT(*) FROM Products") ?? "0";

                    // Load total suppliers
                    TotalSuppliersTextBlock.Text = ExecuteScalarQuery(conn, "SELECT COUNT(*) FROM Suppliers") ?? "0";

                    // Load low stock products
                    LowStockTextBlock.Text = ExecuteScalarQuery(conn, "SELECT COUNT(*) FROM Products WHERE Quantity < 10") ?? "0";

                    // Load sales summary (example placeholder)
                    SalesSummaryTextBlock.Text = ExecuteScalarQuery(conn, "SELECT SUM(TotalAmount) FROM SalesOrders WHERE OrderDate >= date('now', '-7 days')") ?? "0";
                }
            }
            catch (SQLiteException sqlEx)
            {
                MessageBox.Show($"SQLite Error: {sqlEx.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"General Error: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string ExecuteScalarQuery(SQLiteConnection conn, string query)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                var result = cmd.ExecuteScalar();
                return result?.ToString();
            }
        }

        private void NavigateToWindow(Window window)
        {
            window.Show();
            this.Close();
        }

        private void ManageProductsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToWindow(new ProductWindow());
        }

        private void InventoryTrackingButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToWindow(new InventoryTrackingWindow());
        }

        private void PurchaseOrderManagementButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToWindow(new PurchaseOrderManagementWindow());
        }

        private void OpenSalesOrderManagement_Click(object sender, RoutedEventArgs e)
        {
            SalesOrderManagement salesOrderWindow = new SalesOrderManagement();
            salesOrderWindow.Show();
        }


        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToWindow(new ReportsWindow());
        }
    }
}

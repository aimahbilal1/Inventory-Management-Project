using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Data.SQLite;
using System.Windows.Input;

namespace InventoryManagementSystem
{
    public partial class ProductWindow : Window
    {
        private string connectionString = new SQLiteConnectionStringBuilder
        {
            DataSource = "C:\\Users\\noora\\source\\repos\\project ex\\project ex\\project ex\\InventoryManagement.db",
            Version = 3,
            DefaultTimeout = 30
        }.ToString();

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the Dashboard
            Dashboard dashboardWindow = new Dashboard();
            dashboardWindow.Show();
            this.Close(); // Close the current Product window
        }

        public ProductWindow()
        {
            InitializeComponent();
            LoadCategories();
            LoadProducts();
        }

        private void LoadCategories()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT CategoryID, CategoryName FROM Categories";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        var categories = new List<KeyValuePair<int, string>>();

                        while (reader.Read())
                        {
                            int categoryID = Convert.ToInt32(reader["CategoryID"]);
                            string categoryName = reader["CategoryName"].ToString();
                            categories.Add(new KeyValuePair<int, string>(categoryID, categoryName));
                        }

                        // Bind data to ComboBox
                        CategoryComboBox.ItemsSource = categories;
                        CategoryComboBox.DisplayMemberPath = "Value";
                        CategoryComboBox.SelectedValuePath = "Key";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM Products";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        var productList = new List<Product>();

                        while (reader.Read())
                        {
                            productList.Add(new Product
                            {
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                Name = reader["Name"].ToString(),
                                SKU = reader["SKU"].ToString(),
                                CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                                Barcode = reader["Barcode"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                            });
                        }

                        ProductListView.ItemsSource = productList;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string productName = ProductNameTextBox.Text.Trim();
                string sku = SKUTextBox.Text.Trim();
                string barcode = BarcodeTextBox.Text.Trim();
                int quantity = int.Parse(QuantityTextBox.Text.Trim());
                decimal unitPrice = decimal.Parse(UnitPriceTextBox.Text.Trim());

                if (CategoryComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Please select a valid category.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(CategoryComboBox.SelectedValue.ToString(), out int categoryID))
                {
                    MessageBox.Show("Invalid category selection.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();

                    string query = "INSERT INTO Products (Name, SKU, CategoryID, Quantity, UnitPrice, Barcode, CreatedAt, UpdatedAt) " +
                                   "VALUES (@Name, @SKU, @CategoryID, @Quantity, @UnitPrice, @Barcode, @CreatedAt, @UpdatedAt)";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", productName);
                        cmd.Parameters.AddWithValue("@SKU", sku);
                        cmd.Parameters.AddWithValue("@CategoryID", categoryID);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                        cmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                        cmd.Parameters.AddWithValue("@Barcode", barcode);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Product added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Method to handle RemoveProductButton click event
        private void RemoveProductButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ProductListView.SelectedItem == null)
                {
                    MessageBox.Show("Please select a product to remove.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Product selectedProduct = (Product)ProductListView.SelectedItem;

                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();

                    string query = "DELETE FROM Products WHERE ProductID = @ProductID";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductID", selectedProduct.ProductID);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Product removed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Method to handle UpdateProductButton click event
        private void UpdateProductButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ProductListView.SelectedItem == null)
                {
                    MessageBox.Show("Please select a product to update.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Product selectedProduct = (Product)ProductListView.SelectedItem;
                string productName = ProductNameTextBox.Text.Trim();
                string sku = SKUTextBox.Text.Trim();
                string barcode = BarcodeTextBox.Text.Trim();
                int quantity = int.Parse(QuantityTextBox.Text.Trim());
                decimal unitPrice = decimal.Parse(UnitPriceTextBox.Text.Trim());

                if (CategoryComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Please select a valid category.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(CategoryComboBox.SelectedValue.ToString(), out int categoryID))
                {
                    MessageBox.Show("Invalid category selection.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();

                    string query = "UPDATE Products SET Name = @Name, SKU = @SKU, CategoryID = @CategoryID, Quantity = @Quantity, UnitPrice = @UnitPrice, Barcode = @Barcode, UpdatedAt = @UpdatedAt WHERE ProductID = @ProductID";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", productName);
                        cmd.Parameters.AddWithValue("@SKU", sku);
                        cmd.Parameters.AddWithValue("@CategoryID", categoryID);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                        cmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                        cmd.Parameters.AddWithValue("@Barcode", barcode);
                        cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@ProductID", selectedProduct.ProductID);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Product updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Method to handle SearchTextBox key-up event
        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                string searchQuery = SearchTextBox.Text.Trim();

                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM Products WHERE Name LIKE @Search OR SKU LIKE @Search";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Search", "%" + searchQuery + "%");

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            var productList = new List<Product>();
                            while (reader.Read())
                            {
                                productList.Add(new Product
                                {
                                    ProductID = Convert.ToInt32(reader["ProductID"]),
                                    Name = reader["Name"].ToString(),
                                    SKU = reader["SKU"].ToString(),
                                    CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                                    Barcode = reader["Barcode"].ToString(),
                                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                    UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                                });
                            }

                            ProductListView.ItemsSource = productList;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public int CategoryID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Barcode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

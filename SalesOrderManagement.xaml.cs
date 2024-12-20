using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace SalesOrderManagementApp
{
    public partial class SalesOrderManagement : Window
    {
        private string connectionString = new SQLiteConnectionStringBuilder
        {
            DataSource = "C:\\Users\\noora\\source\\repos\\project ex\\project ex\\project ex\\InventoryManagement.db",
            Version = 3,
            DefaultTimeout = 30
        }.ToString(); // Connection string for SQLite

        private List<OrderItem> OrderItems = new List<OrderItem>(); // To hold the order items

        public SalesOrderManagement()
        {
            InitializeComponent();
            LoadProducts(); // Load products into combo box when the window is loaded
        }

        // OrderItem class to hold product details
        private class OrderItem
        {
            public string Name { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice => Quantity * UnitPrice; // Calculate total price per item
        }

        // Load product details into the ComboBox (cmbProduct)
        private void LoadProducts()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Name FROM Products"; // Using 'Name' as the correct column name
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    SQLiteDataReader reader = command.ExecuteReader();

                    if (!reader.HasRows) // Check if there are any products
                    {
                        MessageBox.Show("No products found in the database.");
                    }

                    while (reader.Read())
                    {
                        cmbProduct.Items.Add(reader["Name"].ToString()); // Adding product names to ComboBox
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading product details: " + ex.Message);
            }
        }


        // Event handler for Add Product button click
        // Event handler for Add Product button click
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            string Name = cmbProduct.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(Name))
            {
                MessageBox.Show("Please select a product.");
                return;
            }

            // Add logic to get price for the selected product
            decimal Unitprice = GetProductPrice(Name);
            if (Unitprice == 0)
            {
                MessageBox.Show("Error retrieving product price.");
                return;
            }

            // Ask the user for quantity using InputBox
            string quantityInput = Microsoft.VisualBasic.Interaction.InputBox("Enter quantity:", "Product Quantity", "1");

            if (int.TryParse(quantityInput, out int quantity) && quantity > 0)
            {
                // Add item to the order
                AddOrderItem(Name, quantity, Unitprice);
                CalculateGrandTotal();
            }
            else
            {
                MessageBox.Show("Please enter a valid quantity.");
            }
        }

        // New event handler for TextChanged event
        private void txtDiscount_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateGrandTotal(); // Call CalculateGrandTotal when text changes
        }


        // Method to get product price from the database
        private decimal GetProductPrice(string Name)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Price FROM Products WHERE Name = @ProductName";
                    SQLiteCommand command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@ProductName", Name);
                    var result = command.ExecuteScalar();
                    return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving product price: " + ex.Message);
                return 0;
            }
        }

        // Add order item to the list and update the DataGrid
        private void AddOrderItem(string Name, int quantity, decimal price)
        {
            OrderItem newItem = new OrderItem
            {
                Name = Name,
                Quantity = quantity,
                UnitPrice = price
            };

            OrderItems.Add(newItem);
            dgOrderDetails.ItemsSource = null;
            dgOrderDetails.ItemsSource = OrderItems;  // Rebind to update the DataGrid
        }

        // Calculate the grand total of the order
        private void CalculateGrandTotal()
        {
            decimal subtotal = 0;
            foreach (var item in OrderItems)
            {
                subtotal += item.TotalPrice;
            }

            decimal discount = string.IsNullOrEmpty(txtDiscount.Text) ? 0 : Convert.ToDecimal(txtDiscount.Text);
            decimal salesTax = 0; // Logic for sales tax can be added as per your requirement

            txtSubtotal.Text = subtotal.ToString("F2");
            txtSalesTax.Text = salesTax.ToString("F2");
            txtTotalAmount.Text = (subtotal - discount + salesTax).ToString("F2");
        }

        // Event handler for Remove Item button click
        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (dgOrderDetails.SelectedItem is OrderItem selectedItem)
            {
                OrderItems.Remove(selectedItem);
                dgOrderDetails.ItemsSource = null;
                dgOrderDetails.ItemsSource = OrderItems;  // Rebind to update the DataGrid
                CalculateGrandTotal();
            }
            else
            {
                MessageBox.Show("Please select an item to remove.");
            }
        }

        // Event handler for Place Order button click
        private void PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrderItems.Count == 0)
            {
                MessageBox.Show("No items in the order.");
                return;
            }

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO SalesOrders (CustomerName, TotalAmount) VALUES (@CustomerName, @TotalAmount)";
                    SQLiteCommand command = new SQLiteCommand(query, connection);

                    command.Parameters.AddWithValue("@CustomerName", txtCustomerName.Text);
                    command.Parameters.AddWithValue("@TotalAmount", Convert.ToDecimal(txtTotalAmount.Text));
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Order placed successfully.");
                ClearOrder(); // Clear the order after placing it
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error placing the order: " + ex.Message);
            }
        }

        // Event handler for Cancel button click
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ClearOrder(); // Clear all order data when cancelling
        }

        // Clear the order details
        private void ClearOrder()
        {
            OrderItems.Clear();
            dgOrderDetails.ItemsSource = null;
            txtCustomerName.Clear();
            txtDiscount.Clear();
            txtSalesTax.Clear();
            txtSubtotal.Clear();
            txtTotalAmount.Clear();
        }

        // Event handler for Back Button click
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Close the Sales Order Management window
        }

        // Event handler for Search Customer button click
        private void SearchCustomer_Click(object sender, RoutedEventArgs e)
        {
            // Logic for searching customer details based on customer name, phone, or email
            string customerName = txtCustomerName.Text;
            string phoneNumber = txtPhoneNumber.Text;
            string email = txtEmailAddress.Text;

            // Logic to search customers and display results (or handle errors if no customer found)
            MessageBox.Show($"Searching for customer: {customerName} | {phoneNumber} | {email}");
        }
    }

}

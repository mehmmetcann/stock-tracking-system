using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockTrackingSystem
{
    public partial class FrmProduct : Form
    {
        public FrmProduct()
        {
            InitializeComponent();
        }

        string connectionString = @"Server=localhost\SQLEXPRESS;Database=StockDb;Trusted_Connection=True;";

        void CategoryComboBoxFill()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlDataAdapter da = new SqlDataAdapter("SELECT CategoryId, CategoryName FROM Categories", connection);

            DataTable dt = new DataTable();
            da.Fill(dt);

            cmbCategory.DisplayMember = "CategoryName";
            cmbCategory.ValueMember = "CategoryId";
            cmbCategory.DataSource = dt;
        }

        void ProductList()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            SqlDataAdapter da = new SqlDataAdapter(
                "SELECT P.ProductId, P.ProductName, P.UnitPrice, P.Stock, P.CategoryId, C.CategoryName " +
                "FROM Products P " +
                "INNER JOIN Categories C ON P.CategoryId = C.CategoryId",
                connection
            );

            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridViewProducts.DataSource = dt;

            dataGridViewProducts.Columns["CategoryId"].Visible = false;

            dataGridViewProducts.Columns["CategoryName"].HeaderText = "Category";
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductName.Text) ||
                string.IsNullOrWhiteSpace(txtPrice.Text) ||
                string.IsNullOrWhiteSpace(txtStock.Text))
            {
                MessageBox.Show("Please fill all fields!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand(
                "INSERT INTO Products (ProductName, UnitPrice, Stock, CategoryId) VALUES (@p1, @p2, @p3, @p4)",
                connection
            );

            command.Parameters.AddWithValue("@p1", txtProductName.Text);
            command.Parameters.AddWithValue("@p2", decimal.Parse(txtPrice.Text));
            command.Parameters.AddWithValue("@p3", int.Parse(txtStock.Text));
            command.Parameters.AddWithValue("@p4", cmbCategory.SelectedValue);

            command.ExecuteNonQuery();
            connection.Close();

            MessageBox.Show("Product added successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ProductList();

            txtProductId.Clear();
            txtProductName.Clear();
            txtPrice.Clear();
            txtStock.Clear();
            cmbCategory.SelectedIndex = 0;

            txtProductName.Focus();
        }

        private void FrmProduct_Load(object sender, EventArgs e)
        {
            CategoryComboBoxFill();
            ProductList();
        }

        private void dataGridViewProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int selectedRow = dataGridViewProducts.SelectedCells[0].RowIndex;

            txtProductId.Text = dataGridViewProducts.Rows[selectedRow].Cells["ProductId"].Value.ToString();
            txtProductName.Text = dataGridViewProducts.Rows[selectedRow].Cells["ProductName"].Value.ToString();
            txtPrice.Text = dataGridViewProducts.Rows[selectedRow].Cells["UnitPrice"].Value.ToString();
            txtStock.Text = dataGridViewProducts.Rows[selectedRow].Cells["Stock"].Value.ToString();

            cmbCategory.SelectedValue = dataGridViewProducts.Rows[selectedRow].Cells["CategoryId"].Value.ToString();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductId.Text))
            {
                MessageBox.Show("Please select a product from the table!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand(
                "UPDATE Products SET ProductName=@p1, UnitPrice=@p2, Stock=@p3, CategoryId=@p4 WHERE ProductId=@p5",
                connection
            );

            command.Parameters.AddWithValue("@p1", txtProductName.Text);
            command.Parameters.AddWithValue("@p2", decimal.Parse(txtPrice.Text));
            command.Parameters.AddWithValue("@p3", int.Parse(txtStock.Text));
            command.Parameters.AddWithValue("@p4", cmbCategory.SelectedValue);
            command.Parameters.AddWithValue("@p5", int.Parse(txtProductId.Text));

            command.ExecuteNonQuery();
            connection.Close();

            MessageBox.Show("Product updated successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ProductList();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductId.Text))
            {
                MessageBox.Show("Please select a product from the table!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this product?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.No)
                return;

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand("DELETE FROM Products WHERE ProductId=@p1", connection);
            command.Parameters.AddWithValue("@p1", int.Parse(txtProductId.Text));

            command.ExecuteNonQuery();
            connection.Close();

            MessageBox.Show("Product deleted successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ProductList();

            txtProductId.Clear();
            txtProductName.Clear();
            txtPrice.Clear();
            txtStock.Clear();
            cmbCategory.SelectedIndex = 0;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            SqlDataAdapter da = new SqlDataAdapter(
                "SELECT P.ProductId, P.ProductName, P.UnitPrice, P.Stock, P.CategoryId, C.CategoryName " +
                "FROM Products P " +
                "INNER JOIN Categories C ON P.CategoryId = C.CategoryId " +
                "WHERE P.ProductName LIKE @p1",
                connection
            );

            da.SelectCommand.Parameters.AddWithValue("@p1", "%" + txtSearch.Text + "%");

            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridViewProducts.DataSource = dt;

            dataGridViewProducts.Columns["CategoryId"].Visible = false;
            dataGridViewProducts.Columns["CategoryName"].HeaderText = "Category";
        }

        private void btnList_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            ProductList();
        }
    }
}

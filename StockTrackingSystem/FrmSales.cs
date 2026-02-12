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
    public partial class FrmSales : Form
    {
        public FrmSales()
        {
            InitializeComponent();
        }

        string connectionString = @"Server=localhost\SQLEXPRESS;Database=StockDb;Trusted_Connection=True;";

        void CustomerComboBoxFill()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            SqlDataAdapter da = new SqlDataAdapter("SELECT CustomerId, CustomerName FROM Customers",connection);

            DataTable dt = new DataTable();
            da.Fill(dt);

            cmbCustomer.DisplayMember = "CustomerName";
            cmbCustomer.ValueMember = "CustomerId";
            cmbCustomer.DataSource = dt;
        }

        void SalesList()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            SqlDataAdapter da = new SqlDataAdapter(
                @"SELECT 
            S.SaleId,
            C.CustomerName,
            S.SaleDate,
            S.TotalAmount
          FROM Sales S
          INNER JOIN Customers C ON S.CustomerId = C.CustomerId",
                connection
            );

            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridViewSales.DataSource = dt;
        }

        private void FrmSales_Load(object sender, EventArgs e)
        {
            CustomerComboBoxFill();
            SalesList();
        }

   
        private void dataGridViewSales_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int selectedRow = dataGridViewSales.SelectedCells[0].RowIndex;

            txtSaleId.Text = dataGridViewSales.Rows[selectedRow].Cells["SaleId"].Value.ToString();

            cmbCustomer.Text = dataGridViewSales.Rows[selectedRow].Cells["CustomerName"].Value.ToString();

            object saleDateValue = dataGridViewSales.Rows[selectedRow].Cells["SaleDate"].Value;

            if (saleDateValue != DBNull.Value)
                dtpSaleDate.Value = Convert.ToDateTime(saleDateValue);

            object totalValue = dataGridViewSales.Rows[selectedRow].Cells["TotalAmount"].Value;

            if (totalValue != DBNull.Value)
                txtTotalAmount.Text = totalValue.ToString();
            else
                txtTotalAmount.Clear();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cmbCustomer.SelectedValue == null)
            {
                MessageBox.Show("Please select a customer!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTotalAmount.Text))
            {
                MessageBox.Show("Total amount cannot be empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal totalAmount;

            if (!decimal.TryParse(txtTotalAmount.Text, out totalAmount))
            {
                MessageBox.Show("Please enter a valid total amount!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand(
                "INSERT INTO Sales (CustomerId, SaleDate, TotalAmount) VALUES (@p1, @p2, @p3)",
                connection
            );

            command.Parameters.AddWithValue("@p1", cmbCustomer.SelectedValue);
            command.Parameters.AddWithValue("@p2", dtpSaleDate.Value);
            command.Parameters.AddWithValue("@p3", totalAmount);

            command.ExecuteNonQuery();
            connection.Close();

            MessageBox.Show("Sale added successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            SalesList();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSaleId.Text))
            {
                MessageBox.Show("Please select a sale from the table!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbCustomer.SelectedValue == null)
            {
                MessageBox.Show("Please select a customer!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTotalAmount.Text))
            {
                MessageBox.Show("Total amount cannot be empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal totalAmount;

            if (!decimal.TryParse(txtTotalAmount.Text, out totalAmount))
            {
                MessageBox.Show("Please enter a valid total amount!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand(
                "UPDATE Sales SET CustomerId=@p1, SaleDate=@p2, TotalAmount=@p3 WHERE SaleId=@p4",
                connection
            );

            command.Parameters.AddWithValue("@p1", cmbCustomer.SelectedValue);
            command.Parameters.AddWithValue("@p2", dtpSaleDate.Value);
            command.Parameters.AddWithValue("@p3", totalAmount);
            command.Parameters.AddWithValue("@p4", int.Parse(txtSaleId.Text));

            command.ExecuteNonQuery();
            connection.Close();

            MessageBox.Show("Sale updated successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            SalesList();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSaleId.Text))
            {
                MessageBox.Show("Please select a sale from the table!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this sale?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.No)
                return;

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand(
                "DELETE FROM Sales WHERE SaleId=@p1",
                connection
            );

            command.Parameters.AddWithValue("@p1", int.Parse(txtSaleId.Text));

            command.ExecuteNonQuery();
            connection.Close();

            MessageBox.Show("Sale deleted successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            SalesList();

            txtSaleId.Clear();
            txtTotalAmount.Clear();
        }

        private void btnList_Click(object sender, EventArgs e)
        {
            txtSaleId.Clear();
            txtTotalAmount.Clear();
            txtSearch.Clear();

            CustomerComboBoxFill();
            SalesList();

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            SqlDataAdapter da = new SqlDataAdapter(
                @"SELECT 
            S.SaleId,
            C.CustomerName,
            S.SaleDate,
            S.TotalAmount
          FROM Sales S
          INNER JOIN Customers C ON S.CustomerId = C.CustomerId
          WHERE C.CustomerName LIKE @p1",
                connection
            );

            da.SelectCommand.Parameters.AddWithValue("@p1", "%" + txtSearch.Text + "%");

            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridViewSales.DataSource = dt;
        }
    }
}

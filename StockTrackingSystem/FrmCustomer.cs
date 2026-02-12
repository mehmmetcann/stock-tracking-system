using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace StockTrackingSystem
{
    public partial class FrmCustomer : Form
    {
        public FrmCustomer()
        {
            InitializeComponent();
        }

        string connectionString = @"Server=localhost\SQLEXPRESS;Database=StockDb;Trusted_Connection=True;";

        void CustomerList()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            SqlDataAdapter da = new SqlDataAdapter(
                "SELECT CustomerId, CustomerName, CustomerSurname, Phone FROM Customers",
                connection
            );

            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridViewCustomers.DataSource = dt;
        }

        private void FrmCustomer_Load(object sender, EventArgs e)
        {
            CustomerList();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("Customer name cannot be empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCustomerSurname.Text))
            {
                MessageBox.Show("Customer surname cannot be empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!mskPhone.MaskFull)
            {
                MessageBox.Show("Please enter a valid phone number!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand(
                "INSERT INTO Customers (CustomerName, CustomerSurname, Phone) VALUES (@p1, @p2, @p3)",
                connection
            );

            command.Parameters.AddWithValue("@p1", txtCustomerName.Text);
            command.Parameters.AddWithValue("@p2", txtCustomerSurname.Text);
            command.Parameters.AddWithValue("@p3", mskPhone.Text);

            command.ExecuteNonQuery();
            connection.Close();

            MessageBox.Show("Customer added successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            CustomerList();
        }

        private void btnList_Click(object sender, EventArgs e)
        {
            txtCustomerId.Clear();
            txtCustomerName.Clear();
            txtCustomerSurname.Clear();
            mskPhone.Clear();
            txtSearch.Clear();

            CustomerList();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomerId.Text))
            {
                MessageBox.Show("Please select a customer from the table!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("Customer name cannot be empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCustomerSurname.Text))
            {
                MessageBox.Show("Customer surname cannot be empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!mskPhone.MaskFull)
            {
                MessageBox.Show("Please enter a valid phone number!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand(
                "UPDATE Customers SET CustomerName=@p1, CustomerSurname=@p2, Phone=@p3 WHERE CustomerId=@p4",
                connection
            );

            command.Parameters.AddWithValue("@p1", txtCustomerName.Text);
            command.Parameters.AddWithValue("@p2", txtCustomerSurname.Text);
            command.Parameters.AddWithValue("@p3", mskPhone.Text);
            command.Parameters.AddWithValue("@p4", int.Parse(txtCustomerId.Text));

            command.ExecuteNonQuery();
            connection.Close();

            MessageBox.Show("Customer updated successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            CustomerList();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomerId.Text))
            {
                MessageBox.Show("Please select a customer from the table!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this customer?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.No)
                return;

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand(
                "DELETE FROM Customers WHERE CustomerId=@p1",
                connection
            );

            command.Parameters.AddWithValue("@p1", int.Parse(txtCustomerId.Text));

            command.ExecuteNonQuery();
            connection.Close();

            MessageBox.Show("Customer deleted successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            CustomerList();

            txtCustomerId.Clear();
            txtCustomerName.Clear();
            txtCustomerSurname.Clear();
            mskPhone.Clear();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            SqlDataAdapter da = new SqlDataAdapter(
                "SELECT CustomerId, CustomerName, CustomerSurname, Phone FROM Customers WHERE CustomerName LIKE @p1 OR CustomerSurname LIKE @p1",
                connection
            );

            da.SelectCommand.Parameters.AddWithValue("@p1", "%" + txtSearch.Text + "%");

            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridViewCustomers.DataSource = dt;
        }

        private void dataGridViewCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int selectedRow = dataGridViewCustomers.SelectedCells[0].RowIndex;

            txtCustomerId.Text = dataGridViewCustomers.Rows[selectedRow].Cells["CustomerId"].Value.ToString();
            txtCustomerName.Text = dataGridViewCustomers.Rows[selectedRow].Cells["CustomerName"].Value.ToString();
            txtCustomerSurname.Text = dataGridViewCustomers.Rows[selectedRow].Cells["CustomerSurname"].Value.ToString();
            mskPhone.Text = dataGridViewCustomers.Rows[selectedRow].Cells["Phone"].Value.ToString();
        }

        
    }
}

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
    public partial class FrmSaleDetails : Form
    {
        public FrmSaleDetails()
        {
            InitializeComponent();
        }

        string connectionString = @"Server=localhost\SQLEXPRESS;Database=StockDb;Trusted_Connection=True;";

        void SaleComboBoxFill()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            SqlDataAdapter da = new SqlDataAdapter(
                @"SELECT 
                    S.SaleId,
                    C.CustomerName + ' (SaleId: ' + CAST(S.SaleId AS varchar) + ')' AS SaleInfo
                  FROM Sales S
                  INNER JOIN Customers C ON S.CustomerId = C.CustomerId",
                connection
            );

            DataTable dt = new DataTable();
            da.Fill(dt);

            cmbSale.DisplayMember = "SaleInfo";
            cmbSale.ValueMember = "SaleId";
            cmbSale.DataSource = dt;
        }

        void ProductComboBoxFill()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            SqlDataAdapter da = new SqlDataAdapter(
                "SELECT ProductId, ProductName FROM Products",
                connection
            );

            DataTable dt = new DataTable();
            da.Fill(dt);

            cmbProduct.DisplayMember = "ProductName";
            cmbProduct.ValueMember = "ProductId";
            cmbProduct.DataSource = dt;
        }

        void SaleDetailsList()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            SqlDataAdapter da = new SqlDataAdapter(
                @"SELECT 
                    SD.SaleDetailId,
                    SD.SaleId,
                    C.CustomerName,
                    SD.ProductId,
                    P.ProductName,
                    SD.Quantity,
                    SD.UnitPrice,
                    SD.LineTotal
                  FROM SaleDetails SD
                  INNER JOIN Sales S ON SD.SaleId = S.SaleId
                  INNER JOIN Customers C ON S.CustomerId = C.CustomerId
                  INNER JOIN Products P ON SD.ProductId = P.ProductId",
                connection
            );

            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridViewSaleDetails.DataSource = dt;
        }

        void ClearFields()
        {
            txtSaleDetailId.Clear();
            txtQuantity.Clear();
            txtUnitPrice.Clear();
            txtLineTotal.Clear();
            txtSearch.Clear();
        }

        void CalculateLineTotal()
        {
            if (int.TryParse(txtQuantity.Text, out int qty) && decimal.TryParse(txtUnitPrice.Text, out decimal price))
            {
                txtLineTotal.Text = (qty * price).ToString("0.00");
            }
            else
            {
                txtLineTotal.Clear();
            }
        }


        private void FrmSaleDetails_Load(object sender, EventArgs e)
        {
            SaleComboBoxFill();
            ProductComboBoxFill();
            SaleDetailsList();
        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            CalculateLineTotal();
        }

        private void txtUnitPrice_TextChanged(object sender, EventArgs e)
        {
            CalculateLineTotal();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cmbSale.SelectedValue == null || cmbProduct.SelectedValue == null)
            {
                MessageBox.Show("Please select sale and product!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int qty) || qty <= 0)
            {
                MessageBox.Show("Quantity must be a positive number!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtUnitPrice.Text, out decimal unitPrice) || unitPrice <= 0)
            {
                MessageBox.Show("Unit price must be a positive number!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal lineTotal = qty * unitPrice;

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand(
                @"INSERT INTO SaleDetails (SaleId, ProductId, Quantity, UnitPrice, LineTotal)
                  VALUES (@p1, @p2, @p3, @p4, @p5)",
                connection
            );

            command.Parameters.AddWithValue("@p1", cmbSale.SelectedValue);
            command.Parameters.AddWithValue("@p2", cmbProduct.SelectedValue);
            command.Parameters.AddWithValue("@p3", qty);
            command.Parameters.AddWithValue("@p4", unitPrice);
            command.Parameters.AddWithValue("@p5", lineTotal);

            command.ExecuteNonQuery();
            connection.Close();

            MessageBox.Show("Sale detail added successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            SaleDetailsList();
            ClearFields();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSaleDetailId.Text))
            {
                MessageBox.Show("Please select a sale detail from table!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int qty) || qty <= 0)
            {
                MessageBox.Show("Quantity must be a positive number!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtUnitPrice.Text, out decimal unitPrice) || unitPrice <= 0)
            {
                MessageBox.Show("Unit price must be a positive number!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal lineTotal = qty * unitPrice;

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand(
                @"UPDATE SaleDetails 
                  SET SaleId=@p1, ProductId=@p2, Quantity=@p3, UnitPrice=@p4, LineTotal=@p5
                  WHERE SaleDetailId=@p6",
                connection
            );

            command.Parameters.AddWithValue("@p1", cmbSale.SelectedValue);
            command.Parameters.AddWithValue("@p2", cmbProduct.SelectedValue);
            command.Parameters.AddWithValue("@p3", qty);
            command.Parameters.AddWithValue("@p4", unitPrice);
            command.Parameters.AddWithValue("@p5", lineTotal);
            command.Parameters.AddWithValue("@p6", int.Parse(txtSaleDetailId.Text));

            command.ExecuteNonQuery();
            connection.Close();

            MessageBox.Show("Sale detail updated successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            SaleDetailsList();
            ClearFields();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSaleDetailId.Text))
            {
                MessageBox.Show("Please select a sale detail from table!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this sale detail?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.No)
                return;

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand(
                "DELETE FROM SaleDetails WHERE SaleDetailId=@p1",
                connection
            );

            command.Parameters.AddWithValue("@p1", int.Parse(txtSaleDetailId.Text));

            command.ExecuteNonQuery();
            connection.Close();

            MessageBox.Show("Sale detail deleted successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            SaleDetailsList();
            ClearFields();
        }

        private void btnList_Click(object sender, EventArgs e)
        {
            ClearFields();
            SaleDetailsList();
        }

        private void btnList_Click_1(object sender, EventArgs e)
        {
            ClearFields();
            SaleDetailsList();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            SqlDataAdapter da = new SqlDataAdapter(
                @"SELECT 
                    SD.SaleDetailId,
                    SD.SaleId,
                    C.CustomerName,
                    SD.ProductId,
                    P.ProductName,
                    SD.Quantity,
                    SD.UnitPrice,
                    SD.LineTotal
                  FROM SaleDetails SD
                  INNER JOIN Sales S ON SD.SaleId = S.SaleId
                  INNER JOIN Customers C ON S.CustomerId = C.CustomerId
                  INNER JOIN Products P ON SD.ProductId = P.ProductId
                  WHERE P.ProductName LIKE @p1",
                connection
            );

            da.SelectCommand.Parameters.AddWithValue("@p1", "%" + txtSearch.Text + "%");

            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridViewSaleDetails.DataSource = dt;
        }

        private void dataGridViewSaleDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int selectedRow = dataGridViewSaleDetails.SelectedCells[0].RowIndex;

            txtSaleDetailId.Text = dataGridViewSaleDetails.Rows[selectedRow].Cells["SaleDetailId"].Value.ToString();

            cmbSale.SelectedValue = dataGridViewSaleDetails.Rows[selectedRow].Cells["SaleId"].Value;
            cmbProduct.SelectedValue = dataGridViewSaleDetails.Rows[selectedRow].Cells["ProductId"].Value;

            txtQuantity.Text = dataGridViewSaleDetails.Rows[selectedRow].Cells["Quantity"].Value.ToString();
            txtUnitPrice.Text = dataGridViewSaleDetails.Rows[selectedRow].Cells["UnitPrice"].Value.ToString();
            txtLineTotal.Text = dataGridViewSaleDetails.Rows[selectedRow].Cells["LineTotal"].Value.ToString();
        }
    }
}

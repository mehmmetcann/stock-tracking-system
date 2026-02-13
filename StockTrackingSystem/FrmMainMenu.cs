using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockTrackingSystem
{
    public partial class FrmMainMenu : Form
    {
        public FrmMainMenu()
        {
            InitializeComponent();
        }

        private void FrmMainMenu_Load(object sender, EventArgs e)
        {

        }

        void OpenForm(Form frm)
        {
            panelContent.Controls.Clear();

            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            frm.AutoScroll = false;

            panelContent.Controls.Add(frm);
            frm.Show();
        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            lblTop.Text = "Products";
            OpenForm(new FrmProduct());
        }

        private void btnCategories_Click(object sender, EventArgs e)
        {
            lblTop.Text = "Categories";
            OpenForm(new FrmCategory());
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            lblTop.Text = "Customers";
            OpenForm(new FrmCustomer());
        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            lblTop.Text = "Sales";
            OpenForm(new FrmSales());
        }

        private void btnSaleDetails_Click(object sender, EventArgs e)
        {
            lblTop.Text = "Sale Details";
            OpenForm(new FrmSaleDetails());
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
        "Do you want to exit the application?",
        "Exit",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question
        );

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}

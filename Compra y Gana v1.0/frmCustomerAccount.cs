using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compra_y_Gana_v1._0
{
    public partial class frmCustomerAccount : Form
    {
        public Customer customer { get; set; }
        private int MonthPeriod;

        public frmCustomerAccount(Customer customer)
        {
            InitializeComponent();
            FillTextBoxSince(customer);
            this.customer = customer;

            var account = BLL.AccountServices.FindById(customer.CustomerID);
            txtAccumulatedPoints.Text = account.CurrentPointsBalance.ToString();
            txtCashEquivalents.Text = (account.CurrentPointsBalance * Properties.Settings.Default.PointValueCash).ToString("C4");
        }        

        private void frmCustomerAccount_Load(object sender, EventArgs e)
        {
            cbxPeriod.SelectedIndex = 0;
            txtCurrentMonth.Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month);
        }

        private void GetAccountTransactionsByPeriod(int MonthPeriod)
        {
            try
            {
                ICollection<Transaction> transactions = BLL.TransactionServices.GetAccountTransactions(customer.CustomerID, MonthPeriod);

                foreach (Transaction transaction in transactions)
                {
                    string TransactionType = TranslateTransactionType(transaction.TransactionType);
                    string[] row1 = { $"{transaction.Description}", TransactionType, $"{transaction.Amount.ToString("C2")}" };
                    lvCustomerTransactions.Items.Add(transaction.TransactionDate.ToString()).SubItems.AddRange(row1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"Error del sistema {ex.GetType().Name}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string TranslateTransactionType(TransactionType transactionType)
        {
            switch (transactionType)
            {
                case Models.TransactionType.Purchase:
                    return "Compra";
                case Models.TransactionType.Expense:
                    return "Gasto";
                case Models.TransactionType.Withdrawal:
                    return "Retiro";
                case Models.TransactionType.Adjustment:
                    return "Ajuste";
                default:
                    throw new TypeAccessException("Tipo de transacción desconocida");
            }
        }

        private void cbPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxPeriod.SelectedIndex == 0)
            {
                MonthPeriod = DateTime.Now.Month;
                lvCustomerTransactions.Items.Clear();
                GetAccountTransactionsByPeriod(MonthPeriod);
                txtPeriodo.Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month);
            }
            else
            {
                MonthPeriod = DateTime.Now.Month - 1;
                lvCustomerTransactions.Items.Clear();
                GetAccountTransactionsByPeriod(MonthPeriod);
                txtPeriodo.Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month - 1);
            }
        }

        private void FillTextBoxSince(Customer customer)
        {
            txtCustomerAddress.Text = customer.Address;
            txtEmail.Text = customer.Email;
            txtCustomerFullname.Text = customer.ToString();
            txtAccountNumber.Text = customer.CustomerID.ToString();
        }

        private void frmCustomerAccount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}

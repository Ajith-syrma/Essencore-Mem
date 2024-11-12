using System;
using System.Windows.Forms;

namespace Essencore
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }


        private void button1_click(object sender, EventArgs e)
        {
            string emp_id = employee_id.Text;
            string Password = password.Text;
            int empIdint;

            if (int.TryParse(emp_id,out empIdint) && !string.IsNullOrEmpty(emp_id) && Password == "123") 
            {
                try
                {
                    this.Hide();


                    frmBarcode barcodeForm = new frmBarcode(emp_id);
                    barcodeForm.ShowDialog();

                    Application.Exit();

                }

                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");                
                }
 
            }
            else
            {

                MessageBox.Show("Employee Id or Password incorrect. Please try again.");
                employee_id.Text = string.Empty;
                password.Text = string.Empty;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
 
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}

using System.Drawing;
using System.IO;
using System;
using System.Text;
using System.Runtime.InteropServices;
using BarTender;
using System.Globalization;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Forms;


namespace Essencore
{
    public partial class frmBarcode : Form
    {
        private StringBuilder barcodeData = new StringBuilder();
        DbConnection getConn = new DbConnection();
        private System.Windows.Forms.Timer blinkTimer;
        private bool isBlinking;
        private Color originalColor;
        private string emp_id;

        public frmBarcode(string emp_id)
        {
            InitializeComponent();
            DisplayWeekNumber();
            this.emp_id = emp_id;
            lbluserid.Text = this.emp_id;

            this.KeyPreview = true;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
        }


        private void DisplayWeekNumber()
        {
            //----Get the current date----//
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);


            label10.Text = $"DATE: {currentDate}";
            label10.Font = new Font("Showcard Gothic", 12f);

            //----Get the current week number ----//
            DateTime currentweekdate = DateTime.Now;
            CultureInfo culture = CultureInfo.CurrentCulture;
            int weekNumber = culture.Calendar.GetWeekOfYear(
                currentweekdate,
                CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday);
            lblWeekNumber.Text = $"Week Number: {weekNumber}";
            lblWeekNumber.Font = new Font("Showcard Gothic", 12f);
        }


        //----Db coonection for serial and product number without duplicates----//
        private void ProcessBarcode(string barcode, int labelid, string emp_id, string Work_Orderno)
        {
            if (cmbProductType.SelectedIndex != 0 && cmbWorkOrderNo.SelectedValue.ToString() != "Select")
            {
                var bcode = getConn.DbConnect(barcode, labelid, emp_id, Work_Orderno);

                if (bcode.duplicate != "Duplicate" && bcode.duplicate != "NotFound")
                {

                    rtbInstruction.Text = "Print Started";
                    rtbInstruction.Font = new Font("Showcard Gothic", 12f);
                    rtbInstruction.BackColor = Color.LightGoldenrodYellow;
                    DataBindings();
                    printLabelBarcode(lblProductNo.Text.ToString(), bcode);


                    rtbInstruction.BackColor = Color.Empty;
                    txtCustomerSerialNo.Text = bcode.ToString();
                }
                else if (bcode.duplicate == "Duplicate")
                {

                    txtPCBSerialNo.BackColor = Color.OrangeRed;

                    blinkTimer = new System.Windows.Forms.Timer
                    {
                        Interval = 500 // Set the interval to 500 milliseconds (0.5 seconds)
                    };
                    blinkTimer.Tick += (s, args) => BlinkTextBox();
                    blinkTimer.Start();
                    rtbInstruction.Text = "Duplicate";
                    rtbInstruction.BackColor = Color.OrangeRed;
                    rtbInstruction.Font = new Font("Showcard Gothic", 12f);
                }
                else if (bcode.duplicate == "NotFound")
                {
                    rtbInstruction.Text = "PCB Serial Not Found!" + Environment.NewLine + "or Product Type Mismatch!"
                                                  + Environment.NewLine + "Please Enter or Select valid value";
                    rtbInstruction.BackColor = Color.Gray;
                    rtbInstruction.Font = new Font("Showcard Gothic", 12f);
                }
            }
            else if (cmbProductType.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select the Product Type");
            }
            else if (cmbWorkOrderNo.SelectedValue.ToString() == "Select")
            {
                MessageBox.Show("Please select the WorkOrder Number ");
            }


        }

        private void BlinkTextBox()
        {
            if (isBlinking)
            {
                txtPCBSerialNo.BackColor = Color.OrangeRed;
            }
            else
            {
                txtPCBSerialNo.BackColor = Color.Empty;

            }
            if (txtPCBSerialNo.Text == string.Empty)
            {
                blinkTimer.Stop();
                txtPCBSerialNo.BackColor = Color.Empty;
                txtDescription.Text = string.Empty;
            }

            isBlinking = !isBlinking;
        }


        private void btnClear_Click_1(object sender, EventArgs e)
        {
            txtCustomerSerialNo.Text = string.Empty;
            txtPCBSerialNo.Focus();
            txtPCBSerialNo.Text = string.Empty;
            txtCustomerPartNo.Text = string.Empty;
            cmbWorkOrderNo.Text = "Select";
            txtDescription.Text = string.Empty;
            cmbProductType.SelectedIndex = 0;
            rtbInstruction.Text = string.Empty;
            rtbInstruction.BackColor = Color.Empty;
            txtPCBSerialNo.BackColor = Color.Empty;
        }


        public void printLabelBarcode(string productno, BarcodeDetails barcode_details)
        {

            string labelFormatPath = @"D:\Essencore_updated.btw";


            var Model = string.IsNullOrEmpty(barcode_details.Model) ? string.Empty : barcode_details.Model;
            var cus_no = string.IsNullOrEmpty(barcode_details.CustomerSerialNo) ? string.Empty : barcode_details.CustomerSerialNo;
            var Voltage = string.IsNullOrEmpty(barcode_details.Voltage) ? string.Empty : barcode_details.Voltage;
            var Rank = string.IsNullOrEmpty(barcode_details.Rank1) ? string.Empty : barcode_details.Rank1;
            var Country = string.IsNullOrEmpty(barcode_details.Country) ? string.Empty : barcode_details.Country;
            var DDR = string.IsNullOrEmpty(barcode_details.DDR) ? string.Empty : barcode_details.DDR;
            var Density = string.IsNullOrEmpty(barcode_details.Density) ? string.Empty : barcode_details.Density;
            var DIMM = string.IsNullOrEmpty(barcode_details.DIMM) ? string.Empty : barcode_details.DIMM;
            var DTR = string.IsNullOrEmpty(barcode_details.DTR) ? string.Empty : barcode_details.DTR;
            var Latency = string.IsNullOrEmpty(barcode_details.Latency) ? string.Empty : barcode_details.Latency;
            var YearWeek = GetSerialWeek(DateTime.Now);
            var combinedSerialNumber = $"{DDR} {DIMM} {Density} {DTR} {Rank}";
            var QR = $"{Model}  {YearWeek} {cus_no}";


            if (Model != string.Empty && cus_no != string.Empty)
            {

                var externalValues = new Dictionary<string, string>
        {
            {"SerialNumber", cus_no  },
            {"Model", Model },
            {"DDR",combinedSerialNumber },
           // {"Density",Density },
          //  {"DIMM",DIMM },
            //{"DTR",DTR },
            {"Latency",Latency },
          //  {"Rank",Rank },
            {"Country",Country },
            {"YEARWEEK",YearWeek },
            {"QR",QR },

        };

                PrintLabel(labelFormatPath, externalValues);
                txtPCBSerialNo.Text = string.Empty;
                txtPCBSerialNo.Focus();
            }
            else
            {
                MessageBox.Show("Database not connected. Please check with admin");
            }

        }


        public void PrintLabel(string labelFormatPath, Dictionary<string, string> values)
        {

            BarTender.Application btApp = null;
            BarTender.Format btFormat = null;


            try
            {

                btApp = new BarTender.Application();
                btFormat = btApp.Formats.Open(labelFormatPath, false, "");

                foreach (var param in values)
                {
                    btFormat.SetNamedSubStringValue(param.Key, param.Value);
                }

                btFormat.PrintOut(false, false);
                btFormat.Close(BtSaveOptions.btDoNotSaveChanges);

                rtbInstruction.Text = "Print Successfully Completed";
                rtbInstruction.Font = new Font("Showcard Gothic", 12f);
                rtbInstruction.BackColor = Color.Gray;

            }
            catch (COMException comEx)
            {
                MessageBox.Show("COM Error: " + comEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                // Quit the BarTender application
                if (btApp != null)
                {
                    btApp.Quit(BtSaveOptions.btDoNotSaveChanges);
                    Marshal.ReleaseComObject(btApp);
                }

                if (btFormat != null)
                {
                    Marshal.ReleaseComObject(btFormat);
                }
            }


        }

        private void txtPCBSerialNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Process the barcode
                //  ProcessBarcode(barcodeData.ToString());
                int labelid = Convert.ToInt32(cmbProductType.SelectedValue);
                string Work_Orderno = cmbWorkOrderNo.SelectedValue.ToString().Trim();
                ProcessBarcode(txtPCBSerialNo.Text, labelid, this.emp_id, Work_Orderno);

                barcodeData.Clear();
            }
            else
            {
                // Append the keystroke to the barcode data
                barcodeData.Append((char)e.KeyValue);
            }
        }

        public void DataBindings()
        {
            try
            {

                string productNo = lblProductNo.Text.ToString();
                int labelid = Convert.ToInt32(cmbProductType.SelectedValue);
                var barcodedetails = getConn.GetBarcodeDetails(labelid);
    

                // Column design changes 
                dgvBarcodeDetails.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
  
                dgvBarcodeDetails.ColumnHeadersDefaultCellStyle.BackColor = Color.LightBlue;
             

                foreach (DataGridViewColumn column in dgvBarcodeDetails.Columns)
                {
                    column.HeaderText = column.HeaderText.ToUpper();
                }

                dgvBarcodeDetails.Refresh();

                //Value 


                dgvBarcodeDetails.DataSource = barcodedetails;
                dgvBarcodeDetails.Columns["Model"].Width = 250;
                dgvBarcodeDetails.Columns["CustomerSerialNo"].Width = 250;
                dgvBarcodeDetails.Columns["PCBSerialNo"].Width = 250;
                dgvBarcodeDetails.Columns["SyrmaSGSPartno"].Visible = false;
                dgvBarcodeDetails.Columns["WorkOrderNo"].Visible = false;
                dgvBarcodeDetails.Columns["CustomerPartNo"].Visible = false;
                dgvBarcodeDetails.Columns["Bar_Description"].Visible = false;
                dgvBarcodeDetails.Columns["WeekDetails"].Visible = false;
                dgvBarcodeDetails.Columns["ProductNo"].Visible = false;
                dgvBarcodeDetails.Columns["Country"].Visible = false;
                dgvBarcodeDetails.Columns["Density"].Visible = false;
                dgvBarcodeDetails.Columns["DDR"].Visible = false;
                dgvBarcodeDetails.Columns["DIMM"].Visible = false;
                dgvBarcodeDetails.Columns["DTR"].Visible = false;
                dgvBarcodeDetails.Columns["Latency"].Visible = false;
                dgvBarcodeDetails.Columns["Rank1"].Visible = false;
                dgvBarcodeDetails.Columns["Voltage"].Visible = false;


            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void frmBarcode_Load(object sender, EventArgs e)
        {
            try
            {
                txtPCBSerialNo.Focus();
                cmbProductType.Items.Insert(0, "Select ProductType");

                var lstLabelDetails = getConn.GetLabels();
                cmbProductType.DataSource = lstLabelDetails;
                cmbProductType.DisplayMember = "labelname";
                cmbProductType.ValueMember = "labelmasterid";
                cmbProductType.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database not connected.");
            }

        }

        private void cmbProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbProductType.SelectedIndex != 0)
                {
                    int labelid = Convert.ToInt32(cmbProductType.SelectedValue.ToString());
                    var listNos = getConn.getWorkOrderDetails(labelid);
                    if (listNos != null && listNos.Count > 0)
                    {
                        cmbWorkOrderNo.DataSource = listNos;
                    }
                }
                else
                {
                    //txtSyrmaPartNo.Text = string.Empty;
                }
                if (isBlinking)
                    blinkTimer.Stop();
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message.ToString()); }
        }

        public void getFGDetails()
        {
            int labelid = Convert.ToInt32(cmbProductType.SelectedValue);
            var productdetails = getConn.GetProductDetails(labelid, cmbWorkOrderNo.SelectedValue.ToString());
            if (productdetails.WorkOrderNo != null)
            {
                //txtWorkorderNo.Text = productdetails.WorkOrderNo;
                txtCustomerPartNo.Text = productdetails.CustomerPartNo;
                txtDescription.Text = productdetails.Bar_Description;
                lblProductNo.Text = productdetails.Model;
                DataBindings();
                txtPCBSerialNo.Focus();
            }
            else
            {
                dgvBarcodeDetails.Columns.Clear();
                //txtWorkorderNo.Text = string.Empty;
                txtCustomerPartNo.Text = string.Empty;
                txtDescription.Text = string.Empty;
                lblProductNo.Text = string.Empty;
            }
        }

        static string GetSerialWeek(DateTime date)
        {
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;

            // Get the week number
            Calendar calendar = cultureInfo.Calendar;
            //int weekNumber = calendar.GetWeekOfYear(date);

            CultureInfo culture = CultureInfo.CurrentCulture;
            int weekNumber = culture.Calendar.GetWeekOfYear(
                date,
                CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday);

            // Get the year
            int year = date.Year;

            // Format the output as "YYWW"
            string formattedOutput = $"{year % 100:D2}{weekNumber:D2}";

            return formattedOutput; // Return the formatted string
        }

        private void cmbWorkOrderNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbWorkOrderNo.SelectedValue.ToString() != "Select" && cmbWorkOrderNo.Text != string.Empty)
                    getFGDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            //Application.Exit();
            this.Close();
        }
    }

}














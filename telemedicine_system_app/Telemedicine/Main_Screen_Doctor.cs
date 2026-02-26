using System;
using System.Drawing;
using System.Windows.Forms;
using Telemedicine.Controller;

namespace Telemedicine
{
    public partial class Main_Screen_Doctor : Form
    {
        Query controller;

        public int UserId { get; private set; }

        public Main_Screen_Doctor(int userId)
        {
            InitializeComponent();
            controller = new Query(ConnectionString.ConnStr); 
            UserId = userId;
        }

        public string FullName
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        public string DateOfBirth
        {
            get { return textBox2.Text; }
            set { textBox2.Text = value; }
        }

        public string Specialty
        {
            get { return textBox3.Text; }
            set { textBox3.Text = value; }
        }

        public int Experience
        {
            get { return Convert.ToInt32(textBox4.Text); }
            set { textBox4.Text = value.ToString(); }
        }

        public string WorkPlace
        {
            get { return textBox5.Text; }
            set { textBox5.Text = value; }
        }

        public decimal HourlyRate
        {
            get { return decimal.TryParse(textBox6.Text, out decimal val) ? val : 0m; }
            set { textBox6.Text = value.ToString("0.00"); }
        }

        public string Email
        {
            get { return textBox7.Text; }
            set { textBox7.Text = value; }
        }

        private void Main_Screen_Doctor_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = controller.Show_Appointments_list_for_Doctor(UserId);
            dataGridView2.DataSource = controller.Show_Patients_List(UserId);
            label14.Font = new Font(label1.Font, label1.Font.Style | FontStyle.Bold);
            label15.Text = " ";
            label16.Text = " ";
            label17.Text = " ";

            int totalPatients = controller.Total_patient(UserId);
            textBox8.Text = totalPatients.ToString();

            int totalAppoint = controller.Total_appointments(UserId);
            textBox9.Text = totalAppoint.ToString();

            int totalMinutes = controller.Total_minutes(UserId);
            int hours = totalMinutes / 60;
            int minutes = totalMinutes % 60;
            textBox10.Text = $"{hours} h {minutes} m";

            int rating = controller.Rating(UserId);
            textBox11.Text = rating.ToString();
        }

        int currentAppointmentId = 0;
        string selectedPatientName = string.Empty;

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dataGridView2.Columns[e.ColumnIndex].Name == "full_name")
            {
                selectedPatientName = dataGridView2.Rows[e.RowIndex].Cells["full_name"].Value.ToString();
                
                label14.Font = new Font(label1.Font, label1.Font.Style | FontStyle.Regular);
                label15.Text = "Diagnosis";
                label15.Font = new Font(label1.Font, label1.Font.Style | FontStyle.Bold);
                dataGridView2.DataSource = controller.Show_Patient_Diagnosis(UserId, selectedPatientName);
            }

            if(dataGridView2.Columns[e.ColumnIndex].Name == "appointment_id")
            {
                currentAppointmentId = (int)dataGridView2.Rows[e.RowIndex].Cells["appointment_id"].Value;

                bool hasP = controller.HasPrescription(currentAppointmentId);
                bool hasV = controller.HasVitals(currentAppointmentId);

                label16.Visible = false;
                label17.Visible = false;

                if (hasP && hasV)
                {
                    label15.Font = new Font(label1.Font, label1.Font.Style | FontStyle.Regular);
                    label16.Text = "Prescription";
                    label16.Visible = true;
                    label16.Font = new Font(label1.Font, label1.Font.Style | FontStyle.Bold);
                    label17.Text = "Vital signs";
                    label17.Visible = true;
                    dataGridView2.DataSource = controller.Show_Patient_Prescription(UserId, currentAppointmentId);
                }
                else if (hasP)
                {
                    label15.Font = new Font(label1.Font, label1.Font.Style | FontStyle.Regular);
                    label16.Text = "Prescription";
                    label16.Visible = true;
                    label16.Font = new Font(label1.Font, label1.Font.Style | FontStyle.Bold);
                    dataGridView2.DataSource = controller.Show_Patient_Prescription(UserId, currentAppointmentId);
                }
                else if (hasV)
                {
                    label15.Font = new Font(label1.Font, label1.Font.Style | FontStyle.Regular);
                    label16.Text = "Vital signs";
                    label16.Visible = true;
                    label16.Font = new Font(label1.Font, label1.Font.Style | FontStyle.Bold);
                    dataGridView2.DataSource = controller.Show_Patient_Vital_signs(UserId, currentAppointmentId);
                }
            }
        }

        private void label14_Click(object sender, EventArgs e)
        {
            label14.Font = new Font(label1.Font, label1.Font.Style | FontStyle.Bold);
            label15.Text = " ";
            label16.Text = " ";
            label17.Text = " ";
            dataGridView2.DataSource = controller.Show_Patients_List(this.UserId);
        }

        private void label15_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedPatientName)) return;
            label15.Font = new Font(label1.Font, label1.Font.Style | FontStyle.Bold);
            label16.Font = new Font(label1.Font, label1.Font.Style | FontStyle.Regular);
            label17.Font = new Font(label1.Font, label1.Font.Style | FontStyle.Regular);

            dataGridView2.DataSource = controller.Show_Patient_Diagnosis(this.UserId, selectedPatientName);
        }

        private void label16_Click(object sender, EventArgs e)
        {
            if (currentAppointmentId == 0) return;

            label15.Font = new Font(label1.Font, label1.Font.Style | FontStyle.Regular);
            label16.Font = new Font(label1.Font, label1.Font.Style | FontStyle.Bold);
            if (label16.Text == "Prescription")
            {
                dataGridView2.DataSource = controller.Show_Patient_Prescription(UserId, currentAppointmentId);
            }
            else if (label16.Text == "Vital signs")
            {
                dataGridView2.DataSource = controller.Show_Patient_Vital_signs(UserId, currentAppointmentId);
            }
        }

        private void label17_Click(object sender, EventArgs e)
        {
            if (currentAppointmentId == 0) return;
            label17.Font = new Font(label1.Font, label1.Font.Style | FontStyle.Bold);
            label16.Font = new Font(label1.Font, label1.Font.Style | FontStyle.Regular);
            dataGridView2.DataSource = controller.Show_Patient_Vital_signs(UserId, currentAppointmentId);
        }
    }
}

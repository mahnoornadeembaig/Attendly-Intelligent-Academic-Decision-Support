using AMS.Classes;
using AMS.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AMS.Forms
{
    public partial class AttendenceRecordForm : Form
    {
        public AttendenceRecordForm()
        {
            InitializeComponent();
            UiTheme.Apply(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AdminForm f = new AdminForm();
            f.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AttendenceRecord ar = new AttendenceRecord();
            ar.read();
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            AttendenceRecord ar = new AttendenceRecord();
            DataTable dt = ar.readDate();
            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "Date";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!DateTime.TryParse(comboBox1.Text, out DateTime selectedDate))
            {
                MessageBox.Show("Select a valid attendance date.");
                return;
            }

            int? studentId = null;
            if (!string.IsNullOrWhiteSpace(textBox2.Text))
            {
                if (!int.TryParse(textBox2.Text.Trim(), out int parsedStudentId) || parsedStudentId <= 0)
                {
                    MessageBox.Show("Enter a valid Student ID.");
                    return;
                }

                studentId = parsedStudentId;
            }

            string? studentName = string.IsNullOrWhiteSpace(textBox3.Text)
                ? null
                : textBox3.Text.Trim();
            AttendenceRecord attendance = new();
            DataTable records = attendance.ReadReport(
                allowedStudentId: studentId,
                from: selectedDate.Date,
                to: selectedDate.Date,
                studentNameFilter: studentName);
            dataGridView1.DataSource = records;
            if (records.Rows.Count == 0)
            {
                MessageBox.Show("No attendance records match the selected date and student.");
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AttendenceRecord s = new AttendenceRecord();
            int id = Convert.ToInt32(textBox2.Text);
            s.delete(id);
        }
    }
}

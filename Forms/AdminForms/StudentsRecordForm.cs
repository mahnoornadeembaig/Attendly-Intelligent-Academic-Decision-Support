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
    public partial class StudentsRecordForm : Form
    {
        public StudentsRecordForm()
        {
            InitializeComponent();
            UiTheme.Apply(this);
        }
        void read()
        {
            Students s = new Students();
            DataTable dt = s.read();
            dataGridView1.DataSource = dt;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            read();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            AdminForm f = new AdminForm();
            f.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Students s = new Students();
            s.delete(Convert.ToInt32(textBox1.Text));
            read();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Students s = new Students();
            dataGridView1.DataSource = s.read(Convert.ToInt32(textBox1.Text));
            // read();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text, out int studentId) || studentId <= 0 ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                !InputValidation.IsValidEmail(textBox3.Text.Trim()) ||
                !string.IsNullOrWhiteSpace(textBox4.Text) && !InputValidation.IsValidPassword(textBox4.Text))
            {
                MessageBox.Show("Enter a valid student ID, name, and email. A new password must be at least 8 characters.");
                return;
            }

            Students s = new Students();
            s.Name = textBox2.Text.Trim();
            s.Email = textBox3.Text.Trim();
            s.Password = textBox4.Text;

            s.update(studentId);
            MessageBox.Show("Student updated successfully.");
            read();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}

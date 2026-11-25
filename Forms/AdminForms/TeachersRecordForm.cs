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
    public partial class TeachersRecordForm : Form
    {
        public TeachersRecordForm()
        {
            InitializeComponent();
            UiTheme.Apply(this);
        }
        void read()
        {
            Classes.Teachers t = new Classes.Teachers();
            DataTable dt = t.read();
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text, out int teacherId) || teacherId <= 0 ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                !InputValidation.IsValidEmail(textBox3.Text.Trim()) ||
                string.IsNullOrWhiteSpace(textBox6.Text) ||
                !int.TryParse(textBox7.Text, out int courseId) || courseId <= 0 ||
                !string.IsNullOrWhiteSpace(textBox4.Text) && !InputValidation.IsValidPassword(textBox4.Text))
            {
                MessageBox.Show("Enter a valid teacher ID, name, email, qualification, and course ID. A new password must be at least 8 characters.");
                return;
            }

            Classes.Teachers t = new Classes.Teachers();
            t.Name = textBox2.Text.Trim();
            t.Course = textBox5.Text.Trim();
            t.Email = textBox3.Text.Trim();
            t.Password = textBox4.Text;
            t.Qualification = textBox6.Text.Trim();
            t.CourseId = courseId;
            t.update(teacherId);
            MessageBox.Show("Teacher updated successfully.");
            read();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Teachers s = new Teachers();
            s.delete(Convert.ToInt32(textBox1.Text));
        }
    }
}

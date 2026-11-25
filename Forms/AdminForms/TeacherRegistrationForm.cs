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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AMS.Forms
{
    public partial class TeacherRegistrationForm : Form
    {
        public TeacherRegistrationForm()
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text, out int id) || id <= 0 ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                !InputValidation.IsValidEmail(textBox3.Text.Trim()) ||
                !InputValidation.IsValidPassword(textBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox6.Text) ||
                string.IsNullOrWhiteSpace(comboBox1.Text))
            {
                MessageBox.Show("Enter a positive ID, name, valid email, qualification, and a password of at least 8 characters.");
                return;
            }

            Classes.Teachers t = new Classes.Teachers();
            t.Id = id;
            t.Name = textBox2.Text.Trim();
            t.Email = textBox3.Text.Trim();
            t.Password = textBox4.Text;
            t.Qualification = textBox6.Text.Trim();
            t.Course = comboBox1.Text;
            if (!int.TryParse(textBox5.Text, out int courseId) || courseId <= 0)
            {
                MessageBox.Show("Please select a course or enter a valid course ID.");
                return;
            }

            t.CourseId = courseId;
            try
            {
                t.create();
                MessageBox.Show("Teacher registered successfully.");
            }
            catch (Microsoft.Data.Sqlite.SqliteException ex)
            {
                MessageBox.Show($"Registration failed: {ex.Message}");
            }
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            Courses c = new Courses();
            DataTable dt = c.readCourse();
            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "name";
            comboBox1.ValueMember = "Cid";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue != null &&
                int.TryParse(Convert.ToString(comboBox1.SelectedValue), out int courseId))
            {
                textBox5.Text = courseId.ToString();
            }
        }
    }
}

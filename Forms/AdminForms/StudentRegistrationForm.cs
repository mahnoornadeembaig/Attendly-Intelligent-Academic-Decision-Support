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
    public partial class StudentRegistrationForm : Form
    {
        public StudentRegistrationForm()
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
                !InputValidation.IsValidPassword(textBox4.Text))
            {
                MessageBox.Show("Enter a positive ID, name, valid email, and a password of at least 8 characters.");
                return;
            }

            Students s = new Students();
            s.Id = id;
            s.Name = textBox2.Text.Trim();
            s.Email = textBox3.Text.Trim();
            s.Password = textBox4.Text;
            try
            {
                s.create();
                MessageBox.Show("Student registered successfully.");
            }
            catch (Microsoft.Data.Sqlite.SqliteException ex)
            {
                MessageBox.Show($"Registration failed: {ex.Message}");
            }
        }
    }
}

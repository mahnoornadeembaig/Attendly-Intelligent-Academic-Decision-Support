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
namespace AMS.Forms.Admin
{
    public partial class AdminProfile : Form
    {
        public AdminProfile()
        {
            InitializeComponent();
            UiTheme.Apply(this);
            LoadAdmins();
        }

        private void LoadAdmins()
        {
            AdminC admin = new AdminC();
            DataTable admins = admin.read();
            dataGridView1.DataSource = admins;

            if (admins.Rows.Count > 0)
            {
                ShowAdmin(admins.Rows[0]);
            }
        }

        private void ShowAdmin(DataRow admin)
        {
            textBox1.Text = Convert.ToString(admin["Aid"]);
            textBox2.Text = Convert.ToString(admin["name"]);
            textBox3.Text = Convert.ToString(admin["email"]);
            textBox4.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            LoadAdmins();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text, out int adminId) || adminId <= 0 ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                !InputValidation.IsValidEmail(textBox3.Text.Trim()))
            {
                MessageBox.Show("Enter a valid administrator ID, name, and email.");
                return;
            }

            AdminC s = new AdminC();
            s.Name = textBox2.Text.Trim();
            s.Email = textBox3.Text.Trim();
            s.Password = textBox4.Text;
            if (!string.IsNullOrWhiteSpace(s.Password) && !InputValidation.IsValidPassword(s.Password))
            {
                MessageBox.Show("A new password must be at least 8 characters.");
                return;
            }

            s.update(adminId);
            LoadAdmins();
            MessageBox.Show("Update Sucessful!");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AdminC admin = new AdminC();
            DataTable admins = admin.read(Convert.ToInt32(textBox1.Text));
            dataGridView1.DataSource = admins;
            if (admins.Rows.Count > 0)
            {
                ShowAdmin(admins.Rows[0]);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AdminForm f = new AdminForm();
            f.Show();
            this.Hide();
        }
    }
}

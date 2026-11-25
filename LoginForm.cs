using AMS.Classes;
using AMS.Forms;
using AMS.Forms.Teacher;
using AMS.StudentForms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AMS
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            ApplyBrandLogo();
        }

        private void ApplyBrandLogo()
        {
            panelLogo.BackgroundImage = Properties.Resources.AttendlyLogo;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string email = textBox7.Text.Trim();
            string password = textBox8.Text;
            string user = comboBox1.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password))
            {
                labelStatus.Text = "Please enter your email and password.";
                return;
            }

            AuthenticatedUser? authenticatedUser = Authentication.Login(email, password, user);
            labelStatus.Text = string.Empty;
            if (authenticatedUser?.Role == "Admin")
            {
                AdminForm form = new AdminForm();
                form.Show();
                this.Hide();
            }
            else if (authenticatedUser?.Role == "Teacher")
            {
                TeacherForm form = new TeacherForm();
                form.Show();
                this.Hide();
            }
            else if (authenticatedUser?.Role == "Student")
            {
                StudentForm form = new StudentForm();
                form.Show();
                this.Hide();
            }
            else
            {
                labelStatus.Text = "Email or password is incorrect. Check your selected role.";
            }
        }
    }
}
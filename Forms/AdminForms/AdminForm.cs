using AMS.Classes;
using AMS.Forms.Admin;
using AMS.UI;
using AMS.Forms.Reports;
using AMS.Forms.Teacher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AMS.Forms
{
    public partial class AdminForm : Form
    {
        public AdminForm()
        {
            InitializeComponent();
            ApplyDashboardStyle();
        }

        private void ApplyDashboardStyle()
        {
            BackColor = Color.FromArgb(245, 248, 252);

            panelLogo.BackColor = Color.Transparent;
            panelLogo.BackgroundImageLayout = ImageLayout.Zoom;
            panelLogo.BackgroundImage = Properties.Resources.AttendlyLogo;

            buttonSignOut.FlatStyle = FlatStyle.Flat;
            buttonSignOut.FlatAppearance.BorderSize = 1;
            buttonSignOut.FlatAppearance.BorderColor = Color.FromArgb(91, 120, 145);
            buttonSignOut.BackColor = Color.FromArgb(40, 69, 95);
            buttonSignOut.ForeColor = Color.White;
            buttonSignOut.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            buttonSignOut.TextAlign = ContentAlignment.MiddleCenter;
            buttonSignOut.Cursor = Cursors.Hand;
            buttonSignOut.FlatAppearance.MouseOverBackColor = Color.FromArgb(198, 76, 86);
            buttonSignOut.Paint += (_, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var pen = new Pen(Color.White, 2);
                int y = buttonSignOut.Height / 2;
                e.Graphics.DrawLine(pen, 46, y, 62, y);
                e.Graphics.DrawLine(pen, 57, y - 5, 62, y);
                e.Graphics.DrawLine(pen, 57, y + 5, 62, y);
                e.Graphics.DrawArc(pen, 38, y - 8, 16, 16, 90, 180);
            };

            foreach (Control control in panelNav.Controls)
            {
                if (control is Button button)
                {
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    button.BackColor = Color.FromArgb(35, 67, 99);
                    button.ForeColor = Color.White;
                    button.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
                    button.TextAlign = ContentAlignment.MiddleLeft;
                    button.Cursor = Cursors.Hand;
                    button.FlatAppearance.MouseOverBackColor = Color.FromArgb(48, 88, 126);
                    button.Margin = new Padding(0, 4, 0, 4);
                }
            }

            var cardColors = new Dictionary<string, Color>
            {
                { "button3", Color.FromArgb(236, 248, 246) },
                { "button4", Color.FromArgb(237, 243, 251) },
                { "button5", Color.FromArgb(247, 242, 233) },
                { "button6", Color.FromArgb(244, 238, 249) }
            };
            var accentColors = new Dictionary<string, Color>
            {
                { "button3", Color.FromArgb(42, 174, 156) },
                { "button4", Color.FromArgb(70, 127, 192) },
                { "button5", Color.FromArgb(215, 156, 73) },
                { "button6", Color.FromArgb(142, 103, 184) }
            };

            foreach (Control control in panelContent.Controls)
            {
                if (control is not Button button) continue;

                Color baseColor = cardColors[button.Name];
                Color accent = accentColors[button.Name];
                button.Tag = accent;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.BackColor = baseColor;
                button.ForeColor = Color.FromArgb(36, 55, 74);
                button.Font = new Font("Segoe UI Semibold", 13F, FontStyle.Bold, GraphicsUnit.Point);
                button.TextAlign = ContentAlignment.BottomCenter;
                button.Text = "\n\n" + button.Text;
                button.Padding = new Padding(12, 12, 12, 24);
                button.Cursor = Cursors.Hand;

                button.MouseEnter += (_, _) => button.BackColor = ControlPaint.Light(baseColor, 0.08F);
                button.MouseLeave += (_, _) => button.BackColor = baseColor;
                button.Paint += (_, e) =>
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using var border = new Pen(ControlPaint.Light(accent, 0.45F), 1);
                    e.Graphics.DrawRectangle(border, 0, 0, button.Width - 1, button.Height - 1);
                    using var topBrush = new SolidBrush(accent);
                    e.Graphics.FillRectangle(topBrush, 0, 0, button.Width, 5);

                    int size = 58;
                    int x = (button.Width - size) / 2;
                    int y = Math.Max(18, (button.Height - 108) / 2);
                    using var iconBrush = new SolidBrush(Color.FromArgb(34, accent.R, accent.G, accent.B));
                    using var iconPen = new Pen(accent, 2.5F);
                    iconPen.StartCap = LineCap.Round;
                    iconPen.EndCap = LineCap.Round;

                    e.Graphics.FillEllipse(iconBrush, x, y, size, size);
                    using var whitePen = new Pen(Color.White, 2);
                    DrawCardIcon(e.Graphics, whitePen, x, y, size, button.Name);
                };

                button.Resize += (_, _) => UpdateCardShape(button);
                UpdateCardShape(button);
            }
        }

        private static void DrawCardIcon(Graphics graphics, Pen pen, int x, int y, int size, string cardName)
        {
            Rectangle inner = new Rectangle(x + 14, y + 14, size - 28, size - 28);

            if (cardName == "button3")
            {
                graphics.DrawEllipse(pen, inner.Left + 7, inner.Top, 10, 10);
                graphics.DrawArc(pen, inner.Left + 2, inner.Top + 12, 20, 16, 0, 180);
            }
            else if (cardName == "button4")
            {
                graphics.DrawRectangle(pen, inner);
                for (int line = inner.Top + 8; line < inner.Bottom - 2; line += 8)
                    graphics.DrawLine(pen, inner.Left + 6, line, inner.Right - 6, line);
            }
            else if (cardName == "button5")
            {
                graphics.DrawLine(pen, inner.Left, inner.Top + 8, inner.Right, inner.Top + 8);
                graphics.DrawLine(pen, inner.Left + 8, inner.Top + 8, inner.Left + 8, inner.Bottom);
                graphics.DrawLine(pen, inner.Right - 8, inner.Top + 8, inner.Right - 8, inner.Bottom);
                graphics.DrawEllipse(pen, inner.Left + 10, inner.Top + 15, 10, 10);
            }
            else
            {
                graphics.DrawRectangle(pen, inner);
                graphics.DrawLine(pen, inner.Left + 7, inner.Top + 9, inner.Right - 7, inner.Top + 9);
                graphics.DrawLine(pen, inner.Left + 7, inner.Top + 16, inner.Right - 7, inner.Top + 16);
                graphics.DrawEllipse(pen, inner.Right - 8, inner.Top - 3, 8, 8);
            }
        }

        private static void UpdateCardShape(Button button)
        {
            Rectangle bounds = button.ClientRectangle;
            if (bounds.Width < 2 || bounds.Height < 2) return;
            int diameter = 18;
            using var path = new GraphicsPath();
            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            Region? oldRegion = button.Region;
            button.Region = new Region(path);
            oldRegion?.Dispose();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        void read()
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            read();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            StudentRegistrationForm f = new StudentRegistrationForm();
            f.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            TeacherRegistrationForm f = new TeacherRegistrationForm();
            f.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            StudentsRecordForm f = new StudentsRecordForm();
            f.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            TeachersRecordForm f = new TeachersRecordForm();
            f.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            AttendenceRecordForm f = new AttendenceRecordForm();
            f.Show();
            this.Hide();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            AdminProfile f = new AdminProfile();
            f.Show();
            this.Hide();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            var report = new AttendanceReportsForm(this);
            report.Show();
            Hide();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            var attendance = new MarkAttendence();
            attendance.Show();
            Hide();
        }

        private void buttonSignOut_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to sign out?",
                "Sign Out",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            UserSession.SignOut();
            var login = new LoginForm();
            login.Show();
            Close();
        }
    }
}

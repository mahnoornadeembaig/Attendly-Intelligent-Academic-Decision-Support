using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AMS.UI;
using AMS.Forms.Reports;
using AMS.Classes;
using System.Drawing.Drawing2D;

namespace AMS.StudentForms
{
    public partial class StudentForm : Form
    {
        public StudentForm()
        {
            InitializeComponent();
            ApplyStudentStyle();
        }

        private void ApplyStudentStyle()
        {
            BackColor = Color.FromArgb(245, 248, 252);

            panelLogo.BackColor = Color.Transparent;
            panelLogo.BackgroundImageLayout = ImageLayout.Zoom;
            panelLogo.BackgroundImage = Properties.Resources.AttendlyLogo;

            foreach (Control control in panelNav.Controls)
            {
                if (control is not Button button) continue;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.BackColor = Color.FromArgb(35, 67, 99);
                button.ForeColor = Color.White;
                button.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
                button.TextAlign = ContentAlignment.MiddleLeft;
                button.Cursor = Cursors.Hand;
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(48, 88, 126);
            }

            buttonSignOut.FlatStyle = FlatStyle.Flat;
            buttonSignOut.FlatAppearance.BorderSize = 1;
            buttonSignOut.FlatAppearance.BorderColor = Color.FromArgb(91, 120, 145);
            buttonSignOut.BackColor = Color.FromArgb(40, 69, 95);
            buttonSignOut.ForeColor = Color.White;
            buttonSignOut.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            buttonSignOut.FlatAppearance.MouseOverBackColor = Color.FromArgb(198, 76, 86);

            panelContent.Controls.Clear();
            panelContent.ColumnStyles.Clear();
            panelContent.ColumnCount = 1;
            panelContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            panelContent.Controls.Add(CreateFeatureCard("Attendance History", "View attendance records and reports", true), 0, 0);
        }

        private Button CreateFeatureCard(string title, string caption, bool isAttendance)
        {
            var card = new Button
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(12),
                BackColor = isAttendance ? Color.FromArgb(237, 243, 251) : Color.FromArgb(236, 248, 246),
                ForeColor = Color.FromArgb(36, 55, 74),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point),
                Text = title + "\n\n" + caption,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };
            card.FlatAppearance.BorderSize = 0;
            card.Click += isAttendance ? button7_Click : ViewStudentProfile;

            card.Paint += (_, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Color accent = isAttendance ? Color.FromArgb(70, 127, 192) : Color.FromArgb(42, 174, 156);
                using var brush = new SolidBrush(accent);
                e.Graphics.FillRectangle(brush, 0, 0, card.Width, 6);
                e.Graphics.FillEllipse(brush, (card.Width - 64) / 2, 42, 64, 64);
                using var pen = new Pen(Color.White, 2.5F);
                int cx = card.Width / 2;
                if (isAttendance)
                {
                    e.Graphics.DrawRectangle(pen, cx - 18, 58, 36, 31);
                    e.Graphics.DrawLine(pen, cx - 18, 68, cx + 18, 68);
                }
                else
                {
                    e.Graphics.DrawEllipse(pen, cx - 10, 58, 20, 20);
                    e.Graphics.DrawArc(pen, cx - 17, 76, 34, 24, 0, 180);
                }
            };
            card.Resize += (_, _) => UpdateCardShape(card);
            UpdateCardShape(card);
            return card;
        }

        private void ViewStudentProfile(object? sender, EventArgs e)
        {
            AuthenticatedUser? user = UserSession.Current;
            if (user?.Role != "Student" || user.Id <= 0)
            {
                MessageBox.Show(
                    "Your student session could not be verified. Please sign in again.",
                    "Profile unavailable",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DataTable students = new Students().read(user.Id);
            if (students.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Your student profile could not be found.",
                    "Profile unavailable",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            DataRow student = students.Rows[0];
            using var profile = new Form
            {
                Text = "Attendly | My Profile",
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false,
                ClientSize = new Size(540, 390),
                BackColor = Color.FromArgb(245, 248, 252),
                Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point)
            };

            var header = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(24, 49, 74)
            };
            var avatar = new Label
            {
                Text = GetInitials(Convert.ToString(student["name"]) ?? string.Empty),
                Location = new Point(26, 27),
                Size = new Size(72, 72),
                BackColor = Color.FromArgb(42, 174, 156),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 21F, FontStyle.Bold, GraphicsUnit.Point),
                TextAlign = ContentAlignment.MiddleCenter
            };
            using (var avatarPath = new GraphicsPath())
            {
                avatarPath.AddEllipse(0, 0, avatar.Width, avatar.Height);
                avatar.Region = new Region(avatarPath);
            }
            header.Controls.Add(avatar);
            header.Controls.Add(new Label
            {
                AutoSize = true,
                Location = new Point(122, 29),
                ForeColor = Color.FromArgb(132, 224, 206),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold, GraphicsUnit.Point),
                Text = "STUDENT PROFILE"
            });
            header.Controls.Add(new Label
            {
                AutoEllipsis = true,
                Location = new Point(120, 52),
                Size = new Size(380, 36),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point),
                Text = Convert.ToString(student["name"]) ?? string.Empty
            });

            var details = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(26, 18, 26, 8),
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.White
            };
            details.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            details.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            for (int row = 0; row < 2; row++)
                details.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            AddProfileDetail(details, "Student ID", Convert.ToString(student["Sid"]) ?? string.Empty, 0);
            AddProfileDetail(details, "Email address", Convert.ToString(student["email"]) ?? string.Empty, 1);

            var footer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 248, 252),
                Padding = new Padding(22, 12, 22, 12)
            };
            var close = new Button
            {
                Text = "Close",
                Size = new Size(112, 40),
                Dock = DockStyle.Right,
                BackColor = Color.FromArgb(35, 67, 99),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point),
                DialogResult = DialogResult.OK,
                Cursor = Cursors.Hand
            };
            close.FlatAppearance.BorderSize = 0;
            close.FlatAppearance.MouseOverBackColor = Color.FromArgb(48, 88, 126);
            footer.Controls.Add(close);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 126F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 68F));
            layout.Controls.Add(header, 0, 0);
            layout.Controls.Add(details, 0, 1);
            layout.Controls.Add(footer, 0, 2);
            profile.Controls.Add(layout);

            profile.AcceptButton = close;
            profile.ShowDialog(this);
        }

        private static string GetInitials(string name)
        {
            string[] parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return "?";
            return parts.Length == 1
                ? parts[0].Substring(0, 1).ToUpperInvariant()
                : string.Concat(parts[0].Substring(0, 1), parts[parts.Length - 1].Substring(0, 1)).ToUpperInvariant();
        }

        private static void AddProfileDetail(TableLayoutPanel layout, string label, string value, int row)
        {
            layout.Controls.Add(new Label
            {
                Text = label,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(100, 116, 133),
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point)
            }, 0, row);
            layout.Controls.Add(new Label
            {
                Text = value,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(34, 52, 72),
                Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point),
                AutoEllipsis = true
            }, 1, row);
        }

        private static void UpdateCardShape(Button button)
        {
            Rectangle bounds = button.ClientRectangle;
            if (bounds.Width < 2 || bounds.Height < 2) return;
            int d = 18;
            using var path = new GraphicsPath();
            path.AddArc(0, 0, d, d, 180, 90);
            path.AddArc(bounds.Width - d, 0, d, d, 270, 90);
            path.AddArc(bounds.Width - d, bounds.Height - d, d, d, 0, 90);
            path.AddArc(0, bounds.Height - d, d, d, 90, 90);
            path.CloseFigure();
            Region? old = button.Region;
            button.Region = new Region(path);
            old?.Dispose();
        }

        private void buttonSignOut_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to sign out?", "Sign Out",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            UserSession.SignOut();
            new AMS.LoginForm().Show();
            Close();
        }

        private void StudentProfile_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object? sender, EventArgs e)
        {
            var report = new AttendanceReportsForm(this);
            report.Show();
            Hide();
        }
    }
}

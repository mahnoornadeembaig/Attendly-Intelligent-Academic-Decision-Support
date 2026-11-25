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
using System.Drawing.Drawing2D;

namespace AMS.Forms.Teacher
{
    public partial class TeacherForm : Form
    {
        public TeacherForm()
        {
            InitializeComponent();
            ApplyTeacherStyle();
        }

        private void ApplyTeacherStyle()
        {
            BackColor = Color.FromArgb(245, 248, 252);
            WindowState = FormWindowState.Normal;

            panelLogo.BackColor = Color.Transparent;
            panelLogo.BackgroundImageLayout = ImageLayout.Zoom;
            panelLogo.BackgroundImage = Properties.Resources.AttendlyLogo;

            StyleNavButton(button5, false);
            StyleNavButton(button8, false);
            StyleSignOut();

            panelContent.Controls.Clear();
            panelContent.Controls.Add(CreateFeatureCard("Mark Attendance", "Record and manage daily attendance", button5_Click), 0, 0);
            panelContent.Controls.Add(CreateFeatureCard("Attendance Analytics", "View reports and student attendance trends", button8_Click), 1, 0);
        }

        private void StyleNavButton(Button button, bool compact)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = Color.FromArgb(35, 67, 99);
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.Cursor = Cursors.Hand;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(48, 88, 126);
        }

        private void StyleSignOut()
        {
            buttonSignOut.FlatStyle = FlatStyle.Flat;
            buttonSignOut.FlatAppearance.BorderSize = 1;
            buttonSignOut.FlatAppearance.BorderColor = Color.FromArgb(91, 120, 145);
            buttonSignOut.BackColor = Color.FromArgb(40, 69, 95);
            buttonSignOut.ForeColor = Color.White;
            buttonSignOut.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            buttonSignOut.Cursor = Cursors.Hand;
            buttonSignOut.FlatAppearance.MouseOverBackColor = Color.FromArgb(198, 76, 86);
        }

        private Button CreateFeatureCard(string title, string caption, EventHandler action)
        {
            var card = new Button
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(12),
                BackColor = title.Contains("Mark") ? Color.FromArgb(236, 248, 246) : Color.FromArgb(237, 243, 251),
                ForeColor = Color.FromArgb(36, 55, 74),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point),
                Text = title + "\n\n" + caption,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };
            card.FlatAppearance.BorderSize = 0;
            card.Click += action;
            card.Paint += (_, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Color accent = title.Contains("Mark") ? Color.FromArgb(42, 174, 156) : Color.FromArgb(70, 127, 192);
                using var brush = new SolidBrush(accent);
                e.Graphics.FillRectangle(brush, 0, 0, card.Width, 6);
                e.Graphics.FillEllipse(brush, (card.Width - 64) / 2, 42, 64, 64);
                using var pen = new Pen(Color.White, 2.5F);
                int cx = card.Width / 2;
                if (title.Contains("Mark"))
                {
                    e.Graphics.DrawEllipse(pen, cx - 10, 58, 20, 20);
                    e.Graphics.DrawArc(pen, cx - 17, 76, 34, 24, 0, 180);
                }
                else
                {
                    e.Graphics.DrawLine(pen, cx - 16, 92, cx - 16, 69);
                    e.Graphics.DrawLine(pen, cx, 92, cx, 62);
                    e.Graphics.DrawLine(pen, cx + 16, 92, cx + 16, 74);
                }
            };
            card.Resize += (_, _) => UpdateCardShape(card);
            UpdateCardShape(card);
            return card;
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

        private void buttonSignOut_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to sign out?", "Sign Out",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            AMS.Classes.UserSession.SignOut();
            new AMS.LoginForm().Show();
            Close();
        }

        private void button5_Click(object? sender, EventArgs e)
        {
            MarkAttendence ma = new MarkAttendence();
            ma.Show();
            this.Hide();
        }

        private void button8_Click(object? sender, EventArgs e)
        {
            var report = new AttendanceReportsForm(this);
            report.Show();
            Hide();
        }
    }
}

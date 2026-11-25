using System.Drawing.Drawing2D;

namespace AMS.UI
{
    internal static class UiTheme
    {
        private static readonly Color Canvas = Color.FromArgb(245, 248, 252);
        private static readonly Color Ink = Color.FromArgb(34, 52, 72);
        private static readonly Color Muted = Color.FromArgb(100, 116, 133);
        private static readonly Color Accent = Color.FromArgb(27, 154, 138);
        private static readonly Color Navy = Color.FromArgb(35, 67, 99);
        private static readonly Color Danger = Color.FromArgb(198, 76, 86);
        private const int HeaderHeight = 92;

        public static void Apply(Form form)
        {
            Label? titleLabel = form.Controls
                .OfType<Label>()
                .FirstOrDefault(label => label.Font.Size >= 14F);
            string title = titleLabel?.Text ?? GetTitle(form.Text);

            if (titleLabel != null)
            {
                form.Controls.Remove(titleLabel);
                titleLabel.Dispose();
            }

            foreach (Control control in form.Controls)
            {
                control.Location = new Point(control.Left, control.Top + HeaderHeight);
            }

            form.ClientSize = new Size(form.ClientSize.Width, form.ClientSize.Height + HeaderHeight);
            form.BackColor = Canvas;
            form.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
            form.StartPosition = FormStartPosition.CenterScreen;
            form.FormBorderStyle = FormBorderStyle.Sizable;
            form.MaximizeBox = true;
            form.MinimumSize = new Size(Math.Min(800, form.ClientSize.Width), Math.Min(620, form.ClientSize.Height));
            form.Text = $"Attendly | {title}";

            StyleControls(form.Controls);

            Panel header = CreateHeader(form, title);
            form.Controls.Add(header);
            header.BringToFront();
        }

        private static Panel CreateHeader(Form form, string title)
        {
            var header = new Panel
            {
                Name = "themeHeader",
                BackColor = Navy,
                Location = Point.Empty,
                Size = new Size(form.ClientSize.Width, HeaderHeight),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            header.Paint += (_, e) =>
            {
                using var brush = new LinearGradientBrush(
                    header.ClientRectangle,
                    Color.FromArgb(28, 54, 82),
                    Color.FromArgb(30, 104, 112),
                    LinearGradientMode.Horizontal);
                e.Graphics.FillRectangle(brush, header.ClientRectangle);
            };

            header.Controls.Add(new Label
            {
                AutoSize = true,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = Color.FromArgb(132, 224, 206),
                Location = new Point(30, 11),
                Text = "ATTENDLY  /  INTELLIGENT ACADEMIC DECISION-SUPPORT"
            });
            header.Controls.Add(new Label
            {
                AutoSize = true,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 19F, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = Color.White,
                Location = new Point(27, 31),
                Text = title
            });
            if (!HasBackButton(form))
            {
                var backButton = new Button
                {
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(224, 232, 240),
                    ForeColor = Ink,
                    Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point),
                    Location = new Point(form.ClientSize.Width - 92, 48),
                    Size = new Size(72, 32),
                    Text = "Back",
                    Cursor = Cursors.Hand,
                    TabStop = false
                };
                backButton.FlatAppearance.BorderSize = 0;
                backButton.Click += (_, _) => NavigateBack(form);
                header.Controls.Add(backButton);
            }

            return header;
        }

        private static bool HasBackButton(Control control)
        {
            foreach (Control child in control.Controls)
            {
                if (child is Button button &&
                    button.Text.Trim().Equals("Back", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (child.HasChildren && HasBackButton(child))
                {
                    return true;
                }
            }

            return false;
        }

        private static void NavigateBack(Form form)
        {
            if (form is AMS.Forms.Reports.AttendanceReportsForm)
            {
                form.Close();
                return;
            }

            Form? dashboard = AMS.Classes.UserSession.Current?.Role switch
            {
                "Admin" => new AMS.Forms.AdminForm(),
                "Teacher" => new AMS.Forms.Teacher.TeacherForm(),
                "Student" => new AMS.StudentForms.StudentForm(),
                _ => null
            };

            if (dashboard != null)
            {
                dashboard.Show();
                form.Close();
            }
        }

        private static string GetTitle(string formName)
        {
            return formName switch
            {
                "AdminForm" => "Administrator Dashboard",
                "AdminProfile" => "Administrator Profile",
                "AttendenceRecordForm" => "Attendance Records",
                "CourseRegistrationForm" => "Course Registration",
                "StudentRegistrationForm" => "Student Registration",
                "StudentsRecordForm" => "Student Records",
                "TeacherRegistrationForm" => "Teacher Registration",
                "TeachersRecordForm" => "Teacher Records",
                "TeacherForm" => "Teacher Dashboard",
                "StudentForm" => "Student Dashboard",
                "MarkAttendence" => "Mark Attendance",
                _ => formName
            };
        }

        private static void StyleControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                switch (control)
                {
                    case Button button:
                        StyleButton(button);
                        break;
                    case Label label:
                        StyleLabel(label);
                        break;
                    case TextBox textBox:
                        textBox.BorderStyle = BorderStyle.FixedSingle;
                        textBox.BackColor = Color.White;
                        textBox.ForeColor = Ink;
                        textBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
                        break;
                    case ComboBox comboBox:
                        comboBox.FlatStyle = FlatStyle.Flat;
                        comboBox.BackColor = Color.White;
                        comboBox.ForeColor = Ink;
                        comboBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
                        break;
                    case DataGridView grid:
                        StyleGrid(grid);
                        break;
                    case DateTimePicker dateTimePicker:
                        dateTimePicker.CalendarMonthBackground = Color.White;
                        dateTimePicker.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
                        break;
                    case Panel panel when panel.Name != "themeHeader" && panel.Name != "metricAccent":
                        panel.BackColor = Color.White;
                        break;
                }

                if (control.HasChildren)
                {
                    StyleControls(control.Controls);
                }
            }
        }

        private static void StyleButton(Button button)
        {
            string text = button.Text.Trim().ToLowerInvariant();
            Color color = text.Contains("delete")
                ? Danger
                : text.Contains("back")
                    ? Color.FromArgb(224, 232, 240)
                    : text.Contains("register") || text.Contains("mark attendance") || text == "add" || text == "update"
                        ? Accent
                        : Navy;
            Color foreground = text.Contains("back") ? Ink : Color.White;

            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(color, 0.08F);
            button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(color, 0.16F);
            button.BackColor = color;
            button.ForeColor = foreground;
            button.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
            UpdateButtonShape(button);
            button.Resize += (_, _) => UpdateButtonShape(button);
        }

        private static void StyleLabel(Label label)
        {
            if (label.Font.Size >= 14F)
            {
                label.Font = new Font("Segoe UI", 17F, FontStyle.Bold, GraphicsUnit.Point);
                label.ForeColor = Ink;
            }
            else
            {
                label.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
                label.ForeColor = Ink;
            }
        }

        private static void StyleGrid(DataGridView grid)
        {
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Color.FromArgb(229, 235, 241);
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Navy;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
            grid.ColumnHeadersHeight = Math.Max(grid.ColumnHeadersHeight, 36);
            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.ForeColor = Ink;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(211, 240, 235);
            grid.DefaultCellStyle.SelectionForeColor = Ink;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(246, 249, 252);
            grid.RowHeadersVisible = false;
            grid.RowTemplate.Height = Math.Max(grid.RowTemplate.Height, 32);
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
        }

        private static void UpdateButtonShape(Button button)
        {
            Rectangle bounds = button.ClientRectangle;
            if (bounds.Width < 2 || bounds.Height < 2)
            {
                return;
            }

            int diameter = Math.Min(14, Math.Min(bounds.Width, bounds.Height));
            using var path = new GraphicsPath();
            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            var region = new Region(path);
            Region? oldRegion = button.Region;
            button.Region = region;
            oldRegion?.Dispose();
        }
    }
}

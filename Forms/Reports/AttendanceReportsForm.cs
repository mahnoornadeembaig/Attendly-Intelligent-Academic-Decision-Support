using AMS.Classes;
using AMS.UI;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;

namespace AMS.Forms.Reports
{
    public sealed class AttendanceReportsForm : Form
    {
        private const double LowAttendanceThreshold = 0.75;
        private readonly Form owner;
        private readonly AuthenticatedUser user;
        private readonly AttendenceRecord attendance = new();
        private readonly ComboBox courseFilter = new();
        private readonly ComboBox studentFilter = new();
        private readonly ComboBox statusFilter = new();
        private readonly TextBox searchText = new();
        private readonly DateTimePicker dateFrom = new();
        private readonly DateTimePicker dateTo = new();
        private readonly DataGridView reportGrid = new();
        private readonly DataGridView courseComparisonGrid = new();
        private readonly Panel monthlyTrendChart = new();
        private readonly Panel behaviorPanel = new();
        private readonly Label weekdayInsight = new();
        private readonly Label courseInsight = new();
        private readonly Label trendInsight = new();
        private readonly Label modelInsight = new();
        private readonly Label sessionsValue = new();
        private readonly Label rateValue = new();
        private readonly Label absentValue = new();
        private readonly Label alertsValue = new();
        private readonly Label attendanceDaysValue = new();
        private readonly Label alertMessage = new();
        private readonly PrintDocument reportCardDocument = new();
        private DataTable reportTable = new();
        private List<(string Month, double Rate)> monthlyTrend = new();
        private List<string> reportCardLines = new();
        private int printLineIndex;

        public AttendanceReportsForm(Form owner)
        {
            this.owner = owner;
            user = UserSession.Current ?? throw new InvalidOperationException("Sign in before opening attendance reports.");
            InitializeView();
            UiTheme.Apply(this);
            LoadCourseFilters();
            LoadStudentFilters();
            reportCardDocument.PrintPage += PrintStudentReportCardPage;
            RefreshReport();
            FormClosed += (_, _) => this.owner.Show();
        }

        private void InitializeView()
        {
            Text = "Attendance Analytics";
            ClientSize = new Size(1160, 920);
            MinimumSize = new Size(1000, 880);
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = true;

            var filters = new Panel
            {
                Location = new Point(18, 14),
                Size = new Size(1124, 132),
                BorderStyle = BorderStyle.FixedSingle
            };
            AddField(filters, "Course", courseFilter, 16, 10, 178);
            AddField(filters, "Search student / teacher", searchText, 208, 10, 200);
            AddField(filters, "From", dateFrom, 424, 10, 173);
            AddField(filters, "To", dateTo, 613, 10, 173);
            AddField(filters, "Status", statusFilter, 802, 10, 125);
            AddField(filters, "Individual student", studentFilter, 16, 63, 300);
            dateFrom.ShowCheckBox = true;
            dateTo.ShowCheckBox = true;
            dateFrom.Format = DateTimePickerFormat.Custom;
            dateTo.Format = DateTimePickerFormat.Custom;
            dateFrom.CustomFormat = "yyyy-MM-dd";
            dateTo.CustomFormat = "yyyy-MM-dd";
            dateFrom.Checked = false;
            dateTo.Checked = false;
            searchText.PlaceholderText = "Name or ID";
            statusFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            statusFilter.Items.AddRange(new object[] { "All", "Present", "Absent" });
            statusFilter.SelectedIndex = 0;
            studentFilter.DropDownStyle = ComboBoxStyle.DropDownList;

            var applyFilters = CreateButton("Apply filters", 940, 39, 150, 36, RefreshReport);
            var clearFilters = CreateButton("Reset", 940, 8, 150, 28, ResetFilters);
            filters.Controls.Add(applyFilters);
            filters.Controls.Add(clearFilters);
            Controls.Add(filters);

            var metrics = new TableLayoutPanel
            {
                Name = "metricRow",
                Location = new Point(18, 160),
                Size = new Size(1124, 82),
                BackColor = Color.Transparent,
                ColumnCount = 5,
                RowCount = 1,
                Margin = Padding.Empty
            };
            metrics.ColumnStyles.Clear();
            for (int i = 0; i < 5; i++)
            {
                metrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            }
            metrics.Controls.Add(CreateMetric("TOTAL SESSIONS", sessionsValue), 0, 0);
            metrics.Controls.Add(CreateMetric("ATTENDANCE DAYS", attendanceDaysValue), 1, 0);
            metrics.Controls.Add(CreateMetric("ATTENDANCE RATE", rateValue), 2, 0);
            metrics.Controls.Add(CreateMetric("ABSENT", absentValue), 3, 0);
            metrics.Controls.Add(CreateMetric("BELOW 75% ALERTS", alertsValue), 4, 0);
            Controls.Add(metrics);

            alertMessage.AutoSize = false;
            alertMessage.Location = new Point(22, 250);
            alertMessage.Size = new Size(1110, 24);
            alertMessage.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            Controls.Add(alertMessage);

            behaviorPanel.Location = new Point(18, 282);
            behaviorPanel.Size = new Size(1124, 96);
            behaviorPanel.BackColor = Color.FromArgb(241, 247, 250);
            behaviorPanel.BorderStyle = BorderStyle.FixedSingle;
            behaviorPanel.Controls.Add(new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 154, 138),
                Location = new Point(14, 8),
                Text = "ATTENDANCE BEHAVIOUR ANALYSIS  /  ML LOGISTIC MODEL"
            });
            ConfigureBehaviorLabel(weekdayInsight, new Point(14, 34), 355);
            ConfigureBehaviorLabel(courseInsight, new Point(378, 34), 355);
            ConfigureBehaviorLabel(trendInsight, new Point(742, 34), 366);
            ConfigureBehaviorLabel(modelInsight, new Point(14, 66), 1092);
            behaviorPanel.Controls.Add(weekdayInsight);
            behaviorPanel.Controls.Add(courseInsight);
            behaviorPanel.Controls.Add(trendInsight);
            behaviorPanel.Controls.Add(modelInsight);
            Controls.Add(behaviorPanel);

            monthlyTrendChart.Location = new Point(18, 390);
            monthlyTrendChart.Size = new Size(552, 180);
            monthlyTrendChart.BackColor = Color.White;
            monthlyTrendChart.BorderStyle = BorderStyle.FixedSingle;
            monthlyTrendChart.Paint += PaintMonthlyTrendChart;
            Controls.Add(monthlyTrendChart);

            var comparisonPanel = new Panel
            {
                Location = new Point(588, 390),
                Size = new Size(554, 180),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            comparisonPanel.Controls.Add(new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 52, 72),
                Location = new Point(10, 6),
                Text = "Course attendance comparison"
            });
            courseComparisonGrid.Location = new Point(8, 30);
            courseComparisonGrid.Size = new Size(536, 140);
            courseComparisonGrid.ReadOnly = true;
            courseComparisonGrid.AllowUserToAddRows = false;
            courseComparisonGrid.AllowUserToDeleteRows = false;
            courseComparisonGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            courseComparisonGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            courseComparisonGrid.CellFormatting += CourseComparisonGrid_CellFormatting;
            comparisonPanel.Controls.Add(courseComparisonGrid);
            comparisonPanel.Name = "courseComparisonPanel";
            Controls.Add(comparisonPanel);

            reportGrid.Location = new Point(18, 588);
            reportGrid.Size = new Size(1124, 240);
            reportGrid.ReadOnly = true;
            reportGrid.AllowUserToAddRows = false;
            reportGrid.AllowUserToDeleteRows = false;
            reportGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            reportGrid.CellFormatting += ReportGrid_CellFormatting;
            Controls.Add(reportGrid);

            Controls.Add(CreateButton("Export Excel", 18, 840, 150, 40, ExportExcel));
            Controls.Add(CreateButton("Export PDF", 184, 840, 150, 40, ExportPdf));
            Controls.Add(CreateButton("Print student report card", 350, 840, 220, 40, PrintStudentReportCard));
            Controls.Add(CreateButton("Back to dashboard", 974, 840, 168, 40, Close));
            ConfigureResponsiveLayout();
        }

        private void LoadCourseFilters()
        {
            int? studentId = user.Role == "Student" ? user.Id : null;
            int? teacherCourseId = user.Role == "Teacher"
                ? (user.CourseId > 0 ? user.CourseId : -1)
                : null;
            DataTable courses = attendance.ReadReportCourses(studentId, teacherCourseId);
            DataRow allCourses = courses.NewRow();
            allCourses["CourseID"] = 0;
            allCourses["Course"] = "All courses";
            courses.Rows.InsertAt(allCourses, 0);
            courseFilter.DataSource = courses;
            courseFilter.DisplayMember = "Course";
            courseFilter.ValueMember = "CourseID";
            courseFilter.SelectedIndex = 0;
        }

        private void LoadStudentFilters()
        {
            int? allowedStudentId = user.Role == "Student" ? user.Id : null;
            int? allowedCourseId = user.Role == "Teacher"
                ? (user.CourseId > 0 ? user.CourseId : -1)
                : null;
            DataTable students = attendance.ReadReportStudents(allowedStudentId, allowedCourseId);
            DataRow allStudents = students.NewRow();
            allStudents["StudentID"] = 0;
            allStudents["Student"] = "All students";
            students.Rows.InsertAt(allStudents, 0);

            studentFilter.DataSource = students;
            studentFilter.DisplayMember = "Student";
            studentFilter.ValueMember = "StudentID";
            studentFilter.SelectedIndex = 0;
            if (user.Role == "Student")
            {
                studentFilter.SelectedValue = user.Id;
                studentFilter.Enabled = false;
            }
            else
            {
                studentFilter.SelectedIndexChanged += (_, _) =>
                {
                    courseFilter.SelectedIndex = 0;
                    statusFilter.SelectedIndex = 0;
                    searchText.Clear();
                    dateFrom.Checked = false;
                    dateTo.Checked = false;
                    RefreshReport();
                };
            }
        }

        private void RefreshReport()
        {
            if (dateFrom.Checked && dateTo.Checked && dateFrom.Value.Date > dateTo.Value.Date)
            {
                MessageBox.Show("The start date must be on or before the end date.");
                return;
            }

            int? studentScope = user.Role == "Student" ? user.Id : null;
            if (user.Role != "Student" &&
                int.TryParse(Convert.ToString(studentFilter.SelectedValue), out int selectedStudent) &&
                selectedStudent > 0)
            {
                studentScope = selectedStudent;
            }
            int? courseScope = user.Role == "Teacher"
                ? (user.CourseId > 0 ? user.CourseId : -1)
                : null;
            int.TryParse(Convert.ToString(courseFilter.SelectedValue), out int selectedCourse);
            int? courseId = selectedCourse > 0 ? selectedCourse : null;
            DateTime? from = dateFrom.Checked ? dateFrom.Value.Date : null;
            DateTime? to = dateTo.Checked ? dateTo.Value.Date : null;
            string search = searchText.Text.Trim();

            DataTable metricRows = attendance.ReadReport(
                studentScope, courseScope, courseId, from, to, "All", search);
            reportTable = attendance.ReadReport(
                studentScope, courseScope, courseId, from, to, statusFilter.Text, search);

            Dictionary<(int StudentId, int CourseId), (int Total, int Present)> totals = BuildTotals(metricRows);
            int presentCount = totals.Values.Sum(value => value.Present);
            int totalCount = totals.Values.Sum(value => value.Total);
            int absentCount = totalCount - presentCount;
            int riskCount = totals.Values.Count(value =>
                value.Total > 0 && value.Present / (double)value.Total < LowAttendanceThreshold);

            sessionsValue.Text = totalCount.ToString();
            attendanceDaysValue.Text = BuildAttendanceDays(metricRows).ToString();
            rateValue.Text = totalCount == 0 ? "—" : $"{presentCount / (double)totalCount:P0}";
            absentValue.Text = absentCount.ToString();
            alertsValue.Text = riskCount.ToString();
            monthlyTrend = BuildMonthlyTrend(metricRows);
            courseComparisonGrid.DataSource = BuildCourseComparison(metricRows);
            ConfigureCourseComparisonGrid();
            ApplyBehaviourAnalysis(metricRows);
            alertMessage.ForeColor = riskCount > 0 ? Color.FromArgb(172, 87, 30) : Color.FromArgb(32, 126, 101);
            alertMessage.Text = riskCount > 0
                ? $"Warning: {riskCount} student/course combination(s) are below the 75% attendance threshold."
                : "No low-attendance alerts in the selected report period.";

            AddRateAndAlertColumns(reportTable, totals);
            reportGrid.DataSource = reportTable;
            if (reportGrid.Columns.Contains("AttendanceRate"))
            {
                reportGrid.Columns["AttendanceRate"].HeaderText = "Attendance %";
                reportGrid.Columns["AttendanceRate"].DefaultCellStyle.Format = "P0";
            }
            if (reportGrid.Columns.Contains("Alert"))
            {
                reportGrid.Columns["Alert"].HeaderText = "Risk";
            }
            monthlyTrendChart.Invalidate();
        }

        private void ConfigureBehaviorLabel(Label label, Point location, int width)
        {
            label.Location = location;
            label.Size = new Size(width, 24);
            label.AutoSize = false;
            label.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular);
            label.ForeColor = Color.FromArgb(67, 84, 102);
        }

        private void ApplyBehaviourAnalysis(DataTable rows)
        {
            AttendanceBehaviorAnalyzer.Analysis analysis =
                AttendanceBehaviorAnalyzer.Analyze(rows);

            weekdayInsight.Text = $"Highest-risk weekday: {analysis.WeekdayName}";
            weekdayInsight.ForeColor = analysis.WeekdayAbsenceRate >= 0.30
                ? Color.FromArgb(172, 87, 30)
                : Color.FromArgb(32, 126, 101);
            courseInsight.Text = $"Course pattern: {analysis.CourseName}";
            courseInsight.ForeColor = analysis.CourseAbsenceRate >= 0.30
                ? Color.FromArgb(172, 87, 30)
                : Color.FromArgb(32, 126, 101);
            trendInsight.Text = analysis.TrendText;
            trendInsight.ForeColor = analysis.TrendDeclining
                ? Color.FromArgb(172, 87, 30)
                : Color.FromArgb(32, 126, 101);
            modelInsight.Text = $"ML logistic-regression model trained on {analysis.TrainingRecords} records  •  " +
                $"Model confidence: {analysis.Confidence:P0}  •  " +
                "Patterns update automatically with the current filters";
        }

        private void ConfigureResponsiveLayout()
        {
            Resize += (_, _) =>
            {
                int contentWidth = Math.Max(964, ClientSize.Width - 36);
                var metricRow = Controls.OfType<TableLayoutPanel>()
                    .FirstOrDefault(panel => panel.Name == "metricRow");
                if (metricRow != null)
                {
                    metricRow.Location = new Point(18, 160);
                    metricRow.Width = contentWidth;
                }
                behaviorPanel.Width = contentWidth;
                reportGrid.Width = contentWidth;
                monthlyTrendChart.Width = Math.Max(420, (contentWidth - 18) / 2);

                var comparisonPanel = Controls.OfType<Panel>()
                    .FirstOrDefault(panel => panel.Name == "courseComparisonPanel");
                if (comparisonPanel != null)
                {
                    comparisonPanel.Location = new Point(monthlyTrendChart.Right + 18, comparisonPanel.Top);
                    comparisonPanel.Width = contentWidth - monthlyTrendChart.Width - 18;
                    if (comparisonPanel.Controls.Count > 1)
                    {
                        foreach (Control control in comparisonPanel.Controls)
                        {
                            if (control is DataGridView grid)
                                grid.Width = comparisonPanel.Width - 18;
                        }
                    }
                }

                reportGrid.Height = Math.Max(220, ClientSize.Height - 680);
                foreach (Control control in Controls)
                {
                    if (control is Button dashboardButton &&
                        dashboardButton.Text == "Back to dashboard")
                    {
                        dashboardButton.Left = ClientSize.Width - 186;
                    }
                    if (control is Button printButton &&
                        printButton.Text == "Print student report card")
                    {
                        printButton.Top = Math.Max(840, reportGrid.Bottom + 12);
                    }
                }

                int exportTop = Math.Max(840, reportGrid.Bottom + 12);
                foreach (Control control in Controls)
                {
                    if (control is Button button)
                    {
                        if (button.Text is "Export Excel" or "Export PDF" or "Print student report card" or "Back to dashboard")
                            button.Top = exportTop;
                    }
                }
            };
        }

        private static int BuildAttendanceDays(DataTable rows)
        {
            var attendanceDays = new HashSet<string>(StringComparer.Ordinal);
            foreach (DataRow row in rows.Rows)
            {
                if (!IsPresent(row))
                {
                    continue;
                }

                string dateText = Convert.ToString(row["Date"]) ?? string.Empty;
                if (DateTime.TryParse(dateText, out DateTime date))
                {
                    attendanceDays.Add(date.ToString("yyyy-MM-dd"));
                }
                else if (DateTime.TryParseExact(
                    dateText,
                    "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out date))
                {
                    attendanceDays.Add(dateText);
                }
            }

            return attendanceDays.Count;
        }

        private void ResetFilters()
        {
            searchText.Clear();
            statusFilter.SelectedIndex = 0;
            courseFilter.SelectedIndex = 0;
            if (user.Role != "Student")
            {
                studentFilter.SelectedIndex = 0;
            }
            dateFrom.Checked = false;
            dateTo.Checked = false;
            RefreshReport();
        }

        private void ReportGrid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex >= 0 &&
                reportGrid.Columns[e.ColumnIndex].Name == "Alert" &&
                e.Value is string alert)
            {
                e.CellStyle.ForeColor = alert == "On track"
                    ? Color.FromArgb(32, 126, 101)
                    : Color.FromArgb(172, 87, 30);
                e.CellStyle.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            }
        }

        private static List<(string Month, double Rate)> BuildMonthlyTrend(DataTable rows)
        {
            var totalsByMonth = new SortedDictionary<DateTime, (int Total, int Present)>();
            foreach (DataRow row in rows.Rows)
            {
                if (!DateTime.TryParse(Convert.ToString(row["Date"]), out DateTime date))
                {
                    continue;
                }

                DateTime month = new(date.Year, date.Month, 1);
                totalsByMonth.TryGetValue(month, out var current);
                totalsByMonth[month] = (current.Total + 1, current.Present + (IsPresent(row) ? 1 : 0));
            }

            return totalsByMonth
                .TakeLast(12)
                .Select(pair => (
                    pair.Key.ToString("MMM yy"),
                    pair.Value.Present / (double)pair.Value.Total))
                .ToList();
        }

        private static DataTable BuildCourseComparison(DataTable rows)
        {
            var totals = new Dictionary<(int CourseId, string Course), (int Total, int Present)>();
            foreach (DataRow row in rows.Rows)
            {
                int courseId = Convert.ToInt32(row["CourseID"]);
                string courseName = Convert.ToString(row["Course"]) ?? string.Empty;
                var key = (courseId, courseName);
                totals.TryGetValue(key, out var current);
                totals[key] = (current.Total + 1, current.Present + (IsPresent(row) ? 1 : 0));
            }

            var comparison = new DataTable();
            comparison.Columns.Add("CourseID", typeof(int));
            comparison.Columns.Add("Course", typeof(string));
            comparison.Columns.Add("Present", typeof(int));
            comparison.Columns.Add("Sessions", typeof(int));
            comparison.Columns.Add("AttendanceRate", typeof(double));
            comparison.Columns.Add("Alert", typeof(string));
            foreach (var item in totals.OrderBy(pair => pair.Key.Course, StringComparer.OrdinalIgnoreCase))
            {
                double rate = item.Value.Present / (double)item.Value.Total;
                comparison.Rows.Add(item.Key.CourseId, item.Key.Course, item.Value.Present,
                    item.Value.Total, rate, rate < LowAttendanceThreshold ? "Below 75%" : "On track");
            }

            return comparison;
        }

        private void ConfigureCourseComparisonGrid()
        {
            if (courseComparisonGrid.Columns.Contains("CourseID"))
            {
                courseComparisonGrid.Columns["CourseID"].Visible = false;
            }
            if (courseComparisonGrid.Columns.Contains("Course"))
            {
                courseComparisonGrid.Columns["Course"].HeaderText = "Course";
            }
            if (courseComparisonGrid.Columns.Contains("Sessions"))
            {
                courseComparisonGrid.Columns["Sessions"].HeaderText = "Sessions";
            }
            if (courseComparisonGrid.Columns.Contains("AttendanceRate"))
            {
                courseComparisonGrid.Columns["AttendanceRate"].HeaderText = "Attendance %";
                courseComparisonGrid.Columns["AttendanceRate"].DefaultCellStyle.Format = "P0";
            }
            if (courseComparisonGrid.Columns.Contains("Alert"))
            {
                courseComparisonGrid.Columns["Alert"].HeaderText = "Warning";
            }
        }

        private void CourseComparisonGrid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex >= 0 &&
                courseComparisonGrid.Columns[e.ColumnIndex].Name == "Alert" &&
                e.Value is string alert)
            {
                e.CellStyle.ForeColor = alert == "On track"
                    ? Color.FromArgb(32, 126, 101)
                    : Color.FromArgb(172, 87, 30);
                e.CellStyle.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            }
        }

        private void PaintMonthlyTrendChart(object? sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var titleFont = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            using var axisFont = new Font("Segoe UI", 7.5F, FontStyle.Regular);
            using var titleBrush = new SolidBrush(Color.FromArgb(34, 52, 72));
            using var mutedBrush = new SolidBrush(Color.FromArgb(100, 116, 133));
            using var gridPen = new Pen(Color.FromArgb(226, 233, 239));
            using var linePen = new Pen(Color.FromArgb(27, 154, 138), 2.5F);
            using var pointBrush = new SolidBrush(Color.FromArgb(27, 154, 138));
            graphics.DrawString("Monthly attendance trend", titleFont, titleBrush, 12, 7);

            if (monthlyTrend.Count == 0)
            {
                graphics.DrawString("No monthly attendance data for current filters",
                    axisFont, mutedBrush, 16, 78);
                return;
            }

            RectangleF plot = new(42, 34, monthlyTrendChart.Width - 58, monthlyTrendChart.Height - 66);
            foreach (int percentage in new[] { 0, 50, 100 })
            {
                float y = plot.Bottom - plot.Height * percentage / 100;
                graphics.DrawLine(gridPen, plot.Left, y, plot.Right, y);
                graphics.DrawString($"{percentage}%", axisFont, mutedBrush, 5, y - 7);
            }

            PointF[] points = monthlyTrend.Select((item, index) =>
            {
                float x = monthlyTrend.Count == 1
                    ? plot.Left + plot.Width / 2
                    : plot.Left + plot.Width * index / (monthlyTrend.Count - 1);
                float y = plot.Bottom - plot.Height * (float)item.Rate;
                return new PointF(x, y);
            }).ToArray();

            if (points.Length > 1)
            {
                graphics.DrawLines(linePen, points);
            }

            for (int i = 0; i < points.Length; i++)
            {
                graphics.FillEllipse(pointBrush, points[i].X - 3.5F, points[i].Y - 3.5F, 7, 7);
                if (i == 0 || i == points.Length - 1 || i % 2 == 0)
                {
                    graphics.DrawString(monthlyTrend[i].Month, axisFont, mutedBrush,
                        points[i].X - 20, plot.Bottom + 6);
                }
            }
        }

        private void PrintStudentReportCard()
        {
            int studentId;
            if (user.Role == "Student")
            {
                studentId = user.Id;
            }
            else if (!int.TryParse(Convert.ToString(studentFilter.SelectedValue), out studentId) ||
                studentId <= 0)
            {
                MessageBox.Show("Select an individual student before printing a report card.");
                return;
            }

            int? allowedCourseId = user.Role == "Teacher"
                ? (user.CourseId > 0 ? user.CourseId : -1)
                : null;
            DataTable rows = attendance.ReadReport(allowedStudentId: studentId, allowedCourseId: allowedCourseId);
            if (rows.Rows.Count == 0)
            {
                MessageBox.Show("There are no attendance records for this student to print.");
                return;
            }

            int present = rows.Rows.Cast<DataRow>().Count(IsPresent);
            int absent = rows.Rows.Count - present;
            double overallRate = present / (double)rows.Rows.Count;
            string studentName = Convert.ToString(rows.Rows[0]["Student"]) ?? "Student";
            DataTable courses = BuildCourseComparison(rows);
            reportCardLines = new List<string>
            {
                $"Student: {studentName}    Student ID: {studentId}",
                $"Report date: {DateTime.Today:dd MMM yyyy}",
                $"Overall attendance: {present}/{rows.Rows.Count} sessions ({overallRate:P0})",
                $"Present: {present}    Absent: {absent}",
                $"Overall warning: {(overallRate < LowAttendanceThreshold ? "Below 75% - attention required" : "On track")}",
                string.Empty,
                "COURSE DETAILS"
            };

            foreach (DataRow course in courses.Rows)
            {
                int sessions = Convert.ToInt32(course["Sessions"]);
                int coursePresent = Convert.ToInt32(course["Present"]);
                reportCardLines.Add(
                    $"{course["Course"]}: {coursePresent}/{sessions} present, {sessions - coursePresent} absent, " +
                    $"{Convert.ToDouble(course["AttendanceRate"]):P0} - {course["Alert"]}");
            }

            reportCardDocument.DocumentName = $"Attendance report card - {studentName}";
            printLineIndex = 0;
            using var preview = new PrintPreviewDialog
            {
                Document = reportCardDocument,
                Width = 1000,
                Height = 750
            };
            preview.ShowDialog(this);
        }

        private void PrintStudentReportCardPage(object? sender, PrintPageEventArgs e)
        {
            using var titleFont = new Font("Segoe UI", 16F, FontStyle.Bold);
            using var bodyFont = new Font("Segoe UI", 10F, FontStyle.Regular);
            using var headerBrush = new SolidBrush(Color.FromArgb(34, 52, 72));
            using var bodyBrush = new SolidBrush(Color.Black);
            using var dividerPen = new Pen(Color.FromArgb(27, 154, 138), 2F);

            float y = e.MarginBounds.Top;
            e.Graphics.DrawString("STUDENT ATTENDANCE REPORT CARD", titleFont, headerBrush,
                e.MarginBounds.Left, y);
            y += 42;
            e.Graphics.DrawLine(dividerPen, e.MarginBounds.Left, y, e.MarginBounds.Right, y);
            y += 18;

            while (printLineIndex < reportCardLines.Count && y + 24 <= e.MarginBounds.Bottom)
            {
                e.Graphics.DrawString(reportCardLines[printLineIndex], bodyFont, bodyBrush,
                    e.MarginBounds.Left, y);
                y += 24;
                printLineIndex++;
            }

            e.HasMorePages = printLineIndex < reportCardLines.Count;
            if (!e.HasMorePages)
            {
                printLineIndex = 0;
            }
        }

        private void ExportExcel()
        {
            if (reportTable.Rows.Count == 0)
            {
                MessageBox.Show("There are no attendance rows to export.");
                return;
            }

            using var dialog = new SaveFileDialog
            {
                Filter = "Excel workbook (*.xlsx)|*.xlsx",
                FileName = $"Attendance-{DateTime.Now:yyyyMMdd}.xlsx"
            };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    AttendanceReportExporter.ExportExcel(reportTable, dialog.FileName);
                    MessageBox.Show("Excel report exported successfully.");
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Could not save the Excel report: {ex.Message}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show($"Access denied while saving the Excel report: {ex.Message}");
                }
            }
        }

        private void ExportPdf()
        {
            if (reportTable.Rows.Count == 0)
            {
                MessageBox.Show("There are no attendance rows to export.");
                return;
            }

            using var dialog = new SaveFileDialog
            {
                Filter = "PDF document (*.pdf)|*.pdf",
                FileName = $"Attendance-{DateTime.Now:yyyyMMdd}.pdf"
            };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    AttendanceReportExporter.ExportPdf(reportTable, dialog.FileName);
                    MessageBox.Show("PDF report exported successfully.");
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Could not save the PDF report: {ex.Message}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show($"Access denied while saving the PDF report: {ex.Message}");
                }
            }
        }

        private static Dictionary<(int StudentId, int CourseId), (int Total, int Present)> BuildTotals(DataTable rows)
        {
            var totals = new Dictionary<(int StudentId, int CourseId), (int Total, int Present)>();
            foreach (DataRow row in rows.Rows)
            {
                var key = (Convert.ToInt32(row["StudentID"]), Convert.ToInt32(row["CourseID"]));
                totals.TryGetValue(key, out var current);
                bool isPresent = IsPresent(row);
                totals[key] = (current.Total + 1, current.Present + (isPresent ? 1 : 0));
            }

            return totals;
        }

        private static bool IsPresent(DataRow row)
        {
            string status = Convert.ToString(row["Status"]) ?? string.Empty;
            return status.Equals("Present", StringComparison.OrdinalIgnoreCase) ||
                status.Equals("P", StringComparison.OrdinalIgnoreCase);
        }

        private static void AddRateAndAlertColumns(
            DataTable rows,
            Dictionary<(int StudentId, int CourseId), (int Total, int Present)> totals)
        {
            if (!rows.Columns.Contains("AttendanceRate"))
            {
                rows.Columns.Add("AttendanceRate", typeof(double));
                rows.Columns.Add("Alert", typeof(string));
            }

            foreach (DataRow row in rows.Rows)
            {
                var key = (Convert.ToInt32(row["StudentID"]), Convert.ToInt32(row["CourseID"]));
                var value = totals[key];
                double rate = value.Present / (double)value.Total;
                row["AttendanceRate"] = rate;
                row["Alert"] = rate < LowAttendanceThreshold ? "Below 75%" : "On track";
            }
        }

        private static Panel CreateMetric(string title, Label value)
        {
            var card = new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 12, 0)
            };
            card.Controls.Add(new Panel
            {
                Name = "metricAccent",
                BackColor = Color.FromArgb(27, 154, 138),
                Location = new Point(0, 0),
                Size = new Size(5, 82)
            });
            card.Controls.Add(new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 133),
                Location = new Point(17, 10),
                Text = title
            });
            value.AutoSize = true;
            value.Font = new Font("Segoe UI", 21F, FontStyle.Bold);
            value.ForeColor = Color.FromArgb(34, 52, 72);
            value.Location = new Point(14, 30);
            card.Controls.Add(value);
            return card;
        }

        private static void AddField(Panel parent, string labelText, Control control, int x, int y, int width)
        {
            parent.Controls.Add(new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold),
                Location = new Point(x, y),
                Text = labelText
            });
            control.Location = new Point(x, y + 24);
            control.Size = new Size(width, 31);
            parent.Controls.Add(control);
        }

        private static Button CreateButton(string text, int x, int y, int width, int height, Action action)
        {
            var button = new Button
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                Text = text
            };
            button.Click += (_, _) => action();
            return button;
        }
    }
}

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
using System.Drawing.Drawing2D;

namespace AMS.Forms.Teacher
{
    public partial class MarkAttendence : Form
    {
        public MarkAttendence()
        {
            InitializeComponent();
            UiTheme.Apply(this);
            ConfigureResponsiveLayout();
            StatusRadio_CheckedChanged(this, EventArgs.Empty);
            AuthenticatedUser? user = UserSession.Current;
            if (user?.Role == "Teacher")
            {
                comboBox2.Text = user.Name;
                comboBox2.Enabled = false;
                button8.Text = "My Course";
                LoadAssignedCourse();
            }
            else if (user?.Role == "Admin")
            {
                button8.Text = "Course Catalog";
                DataTable teachers = new Teachers().readAssignedTeachers();
                comboBox2.DataSource = teachers;
                comboBox2.DisplayMember = "name";
                comboBox2.ValueMember = "Tid";
                comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
                comboBox1.Enabled = false;
            }

            read();
        }

        private void ConfigureResponsiveLayout()
        {
            Text = "Attendly | Mark Attendance";
            MinimumSize = new Size(900, 700);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            buttonSignOut.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            dataGridView3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button5.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button7.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button8.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            Resize += (_, _) =>
            {
                int available = Math.Max(300, ClientSize.Width - 24);
                dataGridView1.Width = available;
                int half = Math.Max(260, (available - 24) / 2);
                dataGridView3.Width = half;
                dataGridView2.Width = half;
                dataGridView2.Left = dataGridView3.Right + 12;

                int bottomY = Math.Max(dataGridView1.Bottom + 72, ClientSize.Height - 225);
                dataGridView3.Top = bottomY;
                dataGridView2.Top = bottomY;
            };
        }

        private void LoadAssignedCourse()
        {
            AuthenticatedUser? user = UserSession.Current;
            int courseId = 0;
            string courseName = string.Empty;
            if (user?.Role == "Teacher")
            {
                courseId = user.CourseId;
                courseName = user.CourseName;
            }
            else if (user?.Role == "Admin" && comboBox2.SelectedItem is DataRowView teacher)
            {
                courseId = Convert.ToInt32(teacher["CourseId"]);
                courseName = Convert.ToString(teacher["course"]) ?? string.Empty;
            }

            if (courseId > 0 && string.IsNullOrWhiteSpace(courseName))
            {
                DataTable course = new Courses().readCourse(courseId);
                if (course.Rows.Count > 0)
                {
                    courseName = Convert.ToString(course.Rows[0]["name"]) ?? string.Empty;
                }
            }

            var dt = new DataTable();
            dt.Columns.Add("Cid", typeof(int));
            dt.Columns.Add("name", typeof(string));
            if (courseId > 0 && !string.IsNullOrWhiteSpace(courseName))
            {
                dt.Rows.Add(courseId, courseName);
            }
            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "name";
            comboBox1.ValueMember = "Cid";
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Enabled = user?.Role == "Teacher";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_DropDown(object sender, EventArgs e)
        {
            comboBox2_SelectedIndexChanged(sender, e);
        }

        void read()
        {
            AttendenceRecord ar = new AttendenceRecord();
            AuthenticatedUser? user = UserSession.Current;
            DataTable dt = user?.Role == "Teacher"
                ? ar.ReadReport(allowedCourseId: user.CourseId)
                : ar.read();
            dataGridView1.DataSource = dt;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            read();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            textBox2.Text = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
            textBox3.Text = Convert.ToString(dataGridView1.Rows[e.RowIndex].Cells[1].Value);

        }

        private void checkBox1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            AttendenceRecord ar = new AttendenceRecord();
            DataTable dt = ar.readStudents();
            dataGridView3.DataSource = dt;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            AttendenceRecord ar = new AttendenceRecord();
            AuthenticatedUser? user = UserSession.Current;
            DataTable dt = user?.Role == "Teacher" && user.CourseId > 0
                ? ar.readCourse(user.CourseId)
                : ar.readCourse();
            dataGridView2.DataSource = dt;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            AuthenticatedUser? user = UserSession.Current;
            if (user?.Role != "Teacher" && user?.Role != "Admin")
            {
                MessageBox.Show("Only signed-in teachers or administrators can mark attendance.");
                return;
            }

            int assignedCourseId;
            string teacherName;
            if (user.Role == "Teacher")
            {
                assignedCourseId = user.CourseId;
                teacherName = user.Name;
            }
            else if (comboBox2.SelectedItem is DataRowView selectedTeacher)
            {
                assignedCourseId = Convert.ToInt32(selectedTeacher["CourseId"]);
                teacherName = Convert.ToString(selectedTeacher["name"]) ?? string.Empty;
            }
            else
            {
                MessageBox.Show("Select a teacher with an assigned course.");
                return;
            }

            if (assignedCourseId <= 0 ||
                !int.TryParse(Convert.ToString(comboBox1.SelectedValue), out int courseId) ||
                courseId != assignedCourseId ||
                !int.TryParse(textBox2.Text, out int studentId) ||
                studentId <= 0 ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                !presentRadio.Checked && !absentRadio.Checked)
            {
                MessageBox.Show("Select an assigned course, a valid student, and an attendance status.");
                return;
            }

            AttendenceRecord ar = new AttendenceRecord();
            ar.C_id = courseId;
            ar.S_id = studentId;
            ar.S_name = textBox3.Text;
            ar.C_name = comboBox1.Text;
            ar.T_name = teacherName;
            ar.Status = presentRadio.Checked ? "Present" : "Absent";
            ar.Date = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            bool created;
            try
            {
                created = ar.Create();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Invalid attendance", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!created)
            {
                MessageBox.Show("Duplicate prevented: this student already has attendance for this course and date.");
                return;
            }

            read();

            DataTable studentHistory = ar.ReadReport(studentId, courseId);
            int present = studentHistory.Rows.Cast<DataRow>()
                .Count(row => row["Status"].ToString() == "Present" || row["Status"].ToString() == "P");
            double attendanceRate = present / (double)studentHistory.Rows.Count;
            if (attendanceRate < 0.75)
            {
                MessageBox.Show(
                    $"Low attendance alert: {textBox3.Text} is at {attendanceRate:P0} for this course (below 75%).",
                    "Attendance warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            textBox2.Text = Convert.ToString(dataGridView3.Rows[e.RowIndex].Cells[0].Value);
            textBox3.Text = Convert.ToString(dataGridView3.Rows[e.RowIndex].Cells[1].Value);
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            int courseId = Convert.ToInt32(dataGridView2.Rows[e.RowIndex].Cells["Cid"].Value);
            if (UserSession.Current?.Role == "Admin")
            {
                DataTable teachers = new Teachers().readAssignedTeachers();
                DataRow? assignedTeacher = teachers.Rows
                    .Cast<DataRow>()
                    .FirstOrDefault(row => Convert.ToInt32(row["CourseId"]) == courseId);
                if (assignedTeacher == null)
                {
                    MessageBox.Show("No teacher is assigned to this course. Assign it in Teacher Records first.");
                    return;
                }

                comboBox2.SelectedValue = Convert.ToInt32(assignedTeacher["Tid"]);
            }
            else if (UserSession.Current?.Role == "Teacher" &&
                courseId == UserSession.Current.CourseId)
            {
                comboBox1.SelectedValue = courseId;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue != null &&
                int.TryParse(Convert.ToString(comboBox1.SelectedValue), out int courseId))
            {
                textBox1.Text = courseId.ToString();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UserSession.Current?.Role == "Admin")
            {
                LoadAssignedCourse();
            }
        }

        private void StatusRadio_CheckedChanged(object sender, EventArgs e)
        {
            presentRadio.BackColor = presentRadio.Checked
                ? Color.FromArgb(27, 154, 138)
                : Color.FromArgb(224, 232, 240);
            presentRadio.ForeColor = presentRadio.Checked ? Color.White : Color.FromArgb(70, 85, 100);
            absentRadio.BackColor = absentRadio.Checked
                ? Color.FromArgb(198, 76, 86)
                : Color.FromArgb(224, 232, 240);
            absentRadio.ForeColor = absentRadio.Checked ? Color.White : Color.FromArgb(70, 85, 100);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form dashboard = UserSession.Current?.Role == "Admin"
                ? new AdminForm()
                : new TeacherForm();
            dashboard.Show();
            this.Hide();
        }

        private void buttonSignOut_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "Are you sure you want to sign out?",
                "Sign Out",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            UserSession.SignOut();
            new AMS.LoginForm().Show();
            Close();
        }
    }
}

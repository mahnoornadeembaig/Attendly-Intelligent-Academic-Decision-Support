namespace AMS.Forms.Teacher
{
    partial class TeacherForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panelSidebar = new Panel();
            panelLogo = new Panel();
            labelBrand = new Label();
            labelCaption = new Label();
            panelNav = new Panel();
            panelContent = new TableLayoutPanel();
            button8 = new Button();
            button5 = new Button();
            buttonSignOut = new Button();
            panelSidebar.SuspendLayout();
            panelNav.SuspendLayout();
            panelContent.SuspendLayout();
            SuspendLayout();
            // 
            // panelSidebar
            // 
            panelSidebar.BackColor = Color.FromArgb(24, 49, 74);
            panelSidebar.Controls.Add(panelNav);
            panelSidebar.Controls.Add(labelCaption);
            panelSidebar.Controls.Add(labelBrand);
            panelSidebar.Controls.Add(panelLogo);
            panelSidebar.Controls.Add(buttonSignOut);
            panelSidebar.Dock = DockStyle.Left;
            panelSidebar.Name = "panelSidebar";
            panelSidebar.Size = new Size(230, 620);
            // 
            // panelLogo
            // 
            panelLogo.BackColor = Color.FromArgb(40, 180, 158);
            panelLogo.Location = new Point(24, 28);
            panelLogo.Name = "panelLogo";
            panelLogo.Size = new Size(82, 82);
            // 
            // labelBrand
            // 
            labelBrand.AutoSize = true;
            labelBrand.Font = new Font("Segoe UI", 19F, FontStyle.Bold, GraphicsUnit.Point);
            labelBrand.ForeColor = Color.White;
            labelBrand.Location = new Point(98, 31);
            labelBrand.Name = "labelBrand";
            labelBrand.Text = "Attendly";
            // 
            // labelCaption
            // 
            labelCaption.AutoSize = true;
            labelCaption.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point);
            labelCaption.ForeColor = Color.FromArgb(177, 199, 215);
            labelCaption.Location = new Point(98, 69);
            labelCaption.Name = "labelCaption";
            labelCaption.Text = "Teacher Workspace";
            // 
            // panelNav
            // 
            panelNav.BackColor = Color.Transparent;
            panelNav.Controls.Add(button8);
            panelNav.Controls.Add(button5);
            panelNav.Location = new Point(18, 122);
            panelNav.Name = "panelNav";
            panelNav.Size = new Size(194, 126);
            // 
            // button8
            // 
            button8.Location = new Point(0, 68);
            button8.Name = "button8";
            button8.Size = new Size(194, 52);
            button8.Text = "Attendance Analytics";
            button8.Click += button8_Click;
            // 
            // button5
            // 
            button5.Location = new Point(0, 0);
            button5.Name = "button5";
            button5.Size = new Size(194, 52);
            button5.Text = "Mark Attendance";
            button5.Click += button5_Click;
            // 
            // buttonSignOut
            // 
            buttonSignOut.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            buttonSignOut.Location = new Point(18, 554);
            buttonSignOut.Name = "buttonSignOut";
            buttonSignOut.Size = new Size(194, 48);
            buttonSignOut.Text = "Sign Out";
            buttonSignOut.Click += buttonSignOut_Click;
            // 
            // panelContent
            // 
            panelContent.BackColor = Color.FromArgb(245, 248, 252);
            panelContent.ColumnCount = 2;
            panelContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            panelContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            panelContent.RowCount = 1;
            panelContent.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            panelContent.Dock = DockStyle.Fill;
            panelContent.Location = new Point(230, 0);
            panelContent.Name = "panelContent";
            panelContent.Padding = new Padding(30);
            panelContent.Size = new Size(700, 620);
            // 
            // TeacherForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(930, 620);
            Controls.Add(panelContent);
            Controls.Add(panelSidebar);
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimumSize = new Size(800, 520);
            Name = "TeacherForm";
            Text = "Attendly | Teacher Dashboard";
            StartPosition = FormStartPosition.CenterScreen;
            panelSidebar.ResumeLayout(false);
            panelSidebar.PerformLayout();
            panelNav.ResumeLayout(false);
            panelContent.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panelSidebar;
        private Panel panelLogo;
        private Panel panelNav;
        private TableLayoutPanel panelContent;
        private Label labelBrand;
        private Label labelCaption;
        private Button button8;
        private Button button5;
        private Button buttonSignOut;
    }
}
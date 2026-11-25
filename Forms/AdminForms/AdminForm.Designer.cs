namespace AMS.Forms
{
    partial class AdminForm
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
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            button6 = new Button();
            button7 = new Button();
            button8 = new Button();
            button9 = new Button();
            button10 = new Button();
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
            panelSidebar.Location = new Point(0, 0);
            panelSidebar.Name = "panelSidebar";
            panelSidebar.Size = new Size(230, 620);
            panelSidebar.TabIndex = 0;
            // 
            // panelLogo
            // 
            panelLogo.BackColor = Color.FromArgb(40, 180, 158);
            panelLogo.Location = new Point(24, 28);
            panelLogo.Name = "panelLogo";
            panelLogo.Size = new Size(82, 82);
            panelLogo.TabIndex = 0;
            // 
            // labelBrand
            // 
            labelBrand.AutoSize = true;
            labelBrand.Font = new Font("Segoe UI", 19F, FontStyle.Bold, GraphicsUnit.Point);
            labelBrand.ForeColor = Color.White;
            labelBrand.Location = new Point(98, 31);
            labelBrand.Name = "labelBrand";
            labelBrand.Size = new Size(94, 45);
            labelBrand.TabIndex = 1;
            labelBrand.Text = "Attendly";
            // 
            // labelCaption
            // 
            labelCaption.AutoSize = true;
            labelCaption.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point);
            labelCaption.ForeColor = Color.FromArgb(177, 199, 215);
            labelCaption.Location = new Point(98, 69);
            labelCaption.Name = "labelCaption";
            labelCaption.Size = new Size(113, 20);
            labelCaption.TabIndex = 2;
            labelCaption.Text = "Attendance Portal";
            // 
            // buttonSignOut
            // 
            buttonSignOut.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            buttonSignOut.Location = new Point(18, 554);
            buttonSignOut.Name = "buttonSignOut";
            buttonSignOut.Size = new Size(194, 48);
            buttonSignOut.TabIndex = 4;
            buttonSignOut.Text = "  Sign Out";
            buttonSignOut.Click += buttonSignOut_Click;
            // 
            // panelNav
            // 
            panelNav.BackColor = Color.Transparent;
            panelNav.Controls.Add(button8);
            panelNav.Controls.Add(button7);
            panelNav.Controls.Add(button10);
            panelNav.Controls.Add(button9);
            panelNav.Location = new Point(18, 122);
            panelNav.Name = "panelNav";
            panelNav.Size = new Size(194, 310);
            panelNav.TabIndex = 3;
            // 
            // button8
            // 
            button8.Location = new Point(0, 0);
            button8.Name = "button8";
            button8.Size = new Size(194, 52);
            button8.TabIndex = 0;
            button8.Text = "  Administrator Profile";
            button8.Click += button8_Click;
            // 
            // button7
            // 
            button7.Location = new Point(0, 68);
            button7.Name = "button7";
            button7.Size = new Size(194, 52);
            button7.TabIndex = 1;
            button7.Text = "  Attendance Records";
            button7.Click += button7_Click;
            // 
            // button10
            // 
            button10.Location = new Point(0, 136);
            button10.Name = "button10";
            button10.Size = new Size(194, 52);
            button10.TabIndex = 2;
            button10.Text = "  Mark Attendance";
            button10.Click += button10_Click;
            // 
            // button9
            // 
            button9.Location = new Point(0, 204);
            button9.Name = "button9";
            button9.Size = new Size(194, 52);
            button9.TabIndex = 3;
            button9.Text = "  Attendance Analytics";
            button9.Click += button9_Click;
            // 
            // panelContent
            // 
            panelContent.BackColor = Color.FromArgb(245, 248, 252);
            panelContent.ColumnCount = 2;
            panelContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            panelContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            panelContent.RowCount = 2;
            panelContent.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            panelContent.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            panelContent.Controls.Add(button3, 0, 0);
            panelContent.Controls.Add(button4, 1, 0);
            panelContent.Controls.Add(button5, 0, 1);
            panelContent.Controls.Add(button6, 1, 1);
            panelContent.Dock = DockStyle.Fill;
            panelContent.Location = new Point(230, 0);
            panelContent.Name = "panelContent";
            panelContent.Padding = new Padding(30);
            panelContent.Size = new Size(700, 620);
            panelContent.TabIndex = 1;
            // 
            // Student card
            // 
            button3.Dock = DockStyle.Fill;
            button3.Margin = new Padding(10);
            button3.Name = "button3";
            button3.TabIndex = 0;
            button3.Text = "Student Registration";
            button3.Click += button3_Click;
            // 
            // Students record card
            // 
            button4.Dock = DockStyle.Fill;
            button4.Margin = new Padding(10);
            button4.Name = "button4";
            button4.TabIndex = 1;
            button4.Text = "Students Record";
            button4.Click += button4_Click;
            // 
            // Teacher card
            // 
            button5.Dock = DockStyle.Fill;
            button5.Margin = new Padding(10);
            button5.Name = "button5";
            button5.TabIndex = 2;
            button5.Text = "Teacher Registration";
            button5.Click += button5_Click;
            // 
            // Teachers record card
            // 
            button6.Dock = DockStyle.Fill;
            button6.Margin = new Padding(10);
            button6.Name = "button6";
            button6.TabIndex = 3;
            button6.Text = "Teachers Record";
            button6.Click += button6_Click;
            // 
            // AdminForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(930, 620);
            Controls.Add(panelContent);
            Controls.Add(panelSidebar);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MinimumSize = new Size(800, 520);
            MaximizeBox = false;
            Name = "AdminForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Attendly | Administrator Dashboard";
            panelSidebar.ResumeLayout(false);
            panelSidebar.PerformLayout();
            panelNav.ResumeLayout(false);
            panelContent.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        private Button button7;
        private Button button8;
        private Button button9;
        private Button button10;
        private Button buttonSignOut;
        private Panel panelSidebar;
        private Panel panelLogo;
        private Panel panelNav;
        private TableLayoutPanel panelContent;
        private Label labelBrand;
        private Label labelCaption;
    }
}
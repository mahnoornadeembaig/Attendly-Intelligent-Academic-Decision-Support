namespace AMS
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panelBrand = new Panel();
            panelAccent = new Panel();
            panelLogo = new Panel();
            labelBrand = new Label();
            labelBrandTitle = new Label();
            labelBrandCaption = new Label();
            labelWelcome = new Label();
            labelSubtitle = new Label();
            label1 = new Label();
            label2 = new Label();
            label7 = new Label();
            textBox7 = new TextBox();
            textBox8 = new TextBox();
            comboBox1 = new ComboBox();
            button4 = new Button();
            labelStatus = new Label();
            panelBrand.SuspendLayout();
            SuspendLayout();
            // 
            // panelBrand
            // 
            panelBrand.BackColor = Color.FromArgb(24, 45, 70);
            panelBrand.Controls.Add(labelBrandCaption);
            panelBrand.Controls.Add(labelBrandTitle);
            panelBrand.Controls.Add(labelBrand);
            panelBrand.Controls.Add(panelLogo);
            panelBrand.Controls.Add(panelAccent);
            panelBrand.Dock = DockStyle.Left;
            panelBrand.Location = new Point(0, 0);
            panelBrand.Name = "panelBrand";
            panelBrand.Size = new Size(320, 530);
            panelBrand.TabIndex = 0;
            // 
            // panelAccent
            // 
            panelAccent.BackColor = Color.FromArgb(36, 190, 166);
            panelAccent.Location = new Point(48, 178);
            panelAccent.Name = "panelAccent";
            panelAccent.Size = new Size(58, 5);
            panelAccent.TabIndex = 0;
            // 
            // panelLogo
            // 
            panelLogo.BackColor = Color.Transparent;
            panelLogo.Location = new Point(43, 34);
            panelLogo.Name = "panelLogo";
            panelLogo.Size = new Size(82, 82);
            panelLogo.BackgroundImageLayout = ImageLayout.Zoom;
            panelLogo.TabIndex = 4;
            // 
            // labelBrand
            // 
            labelBrand.AutoSize = false;
            labelBrand.Font = new Font("Segoe UI", 26F, FontStyle.Bold, GraphicsUnit.Point);
            labelBrand.ForeColor = Color.White;
            labelBrand.Location = new Point(44, 118);
            labelBrand.Name = "labelBrand";
            labelBrand.Size = new Size(260, 82);
            labelBrand.TextAlign = ContentAlignment.MiddleLeft;
            labelBrand.TabIndex = 1;
            labelBrand.Text = "Attendly";
            // 
            // labelBrandTitle
            // 
            labelBrandTitle.AutoSize = false;
            labelBrandTitle.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            labelBrandTitle.ForeColor = Color.White;
            labelBrandTitle.Location = new Point(44, 196);
            labelBrandTitle.Name = "labelBrandTitle";
            labelBrandTitle.Size = new Size(260, 118);
            labelBrandTitle.TextAlign = ContentAlignment.TopLeft;
            labelBrandTitle.TabIndex = 2;
            labelBrandTitle.Text = "Intelligent Academic\r\nDecision-Support\r\nSystem";
            // 
            // labelBrandCaption
            // 
            labelBrandCaption.AutoSize = false;
            labelBrandCaption.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            labelBrandCaption.ForeColor = Color.FromArgb(190, 205, 220);
            labelBrandCaption.Location = new Point(44, 326);
            labelBrandCaption.Name = "labelBrandCaption";
            labelBrandCaption.Size = new Size(260, 42);
            labelBrandCaption.TextAlign = ContentAlignment.TopLeft;
            labelBrandCaption.TabIndex = 3;
            labelBrandCaption.Text = "Technology-led attendance insights";
            // 
            // labelWelcome
            // 
            labelWelcome.AutoSize = true;
            labelWelcome.Font = new Font("Segoe UI", 23F, FontStyle.Bold, GraphicsUnit.Point);
            labelWelcome.ForeColor = Color.FromArgb(28, 43, 58);
            labelWelcome.Location = new Point(418, 53);
            labelWelcome.Name = "labelWelcome";
            labelWelcome.Size = new Size(274, 52);
            labelWelcome.TabIndex = 1;
            labelWelcome.Text = "Welcome back";
            // 
            // labelSubtitle
            // 
            labelSubtitle.AutoSize = true;
            labelSubtitle.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            labelSubtitle.ForeColor = Color.FromArgb(105, 118, 132);
            labelSubtitle.Location = new Point(423, 106);
            labelSubtitle.Name = "labelSubtitle";
            labelSubtitle.Size = new Size(223, 23);
            labelSubtitle.TabIndex = 2;
            labelSubtitle.Text = "Sign in to continue to your account";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = Color.FromArgb(50, 65, 80);
            label1.Location = new Point(423, 158);
            label1.Name = "label1";
            label1.Size = new Size(47, 21);
            label1.TabIndex = 3;
            label1.Text = "Email";
            // 
            // textBox7
            // 
            textBox7.BorderStyle = BorderStyle.FixedSingle;
            textBox7.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            textBox7.Location = new Point(423, 185);
            textBox7.Name = "textBox7";
            textBox7.PlaceholderText = "you@example.com";
            textBox7.Size = new Size(382, 32);
            textBox7.TabIndex = 0;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            label2.ForeColor = Color.FromArgb(50, 65, 80);
            label2.Location = new Point(423, 233);
            label2.Name = "label2";
            label2.Size = new Size(73, 21);
            label2.TabIndex = 4;
            label2.Text = "Password";
            // 
            // textBox8
            // 
            textBox8.BorderStyle = BorderStyle.FixedSingle;
            textBox8.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            textBox8.Location = new Point(423, 260);
            textBox8.Name = "textBox8";
            textBox8.PlaceholderText = "Enter your password";
            textBox8.Size = new Size(382, 32);
            textBox8.TabIndex = 1;
            textBox8.UseSystemPasswordChar = true;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            label7.ForeColor = Color.FromArgb(50, 65, 80);
            label7.Location = new Point(423, 308);
            label7.Name = "label7";
            label7.Size = new Size(39, 21);
            label7.TabIndex = 5;
            label7.Text = "Role";
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "Admin", "Teacher", "Student" });
            comboBox1.Location = new Point(423, 335);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(382, 31);
            comboBox1.TabIndex = 2;
            comboBox1.SelectedIndex = 0;
            // 
            // button4
            // 
            button4.BackColor = Color.FromArgb(26, 160, 143);
            button4.FlatAppearance.BorderSize = 0;
            button4.FlatStyle = FlatStyle.Flat;
            button4.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold, GraphicsUnit.Point);
            button4.ForeColor = Color.White;
            button4.Location = new Point(423, 393);
            button4.Name = "button4";
            button4.Size = new Size(382, 47);
            button4.TabIndex = 3;
            button4.Text = "Sign in";
            button4.UseVisualStyleBackColor = false;
            button4.Click += button4_Click;
            // 
            // labelStatus
            // 
            labelStatus.AutoSize = true;
            labelStatus.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            labelStatus.ForeColor = Color.FromArgb(190, 55, 55);
            labelStatus.Location = new Point(423, 449);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(0, 20);
            labelStatus.TabIndex = 6;
            // 
            // LoginForm
            // 
            AcceptButton = button4;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(860, 530);
            Controls.Add(labelStatus);
            Controls.Add(button4);
            Controls.Add(comboBox1);
            Controls.Add(label7);
            Controls.Add(textBox8);
            Controls.Add(label2);
            Controls.Add(textBox7);
            Controls.Add(label1);
            Controls.Add(labelSubtitle);
            Controls.Add(labelWelcome);
            Controls.Add(panelBrand);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Attendly | Sign in";
            panelBrand.ResumeLayout(false);
            panelBrand.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private Panel panelBrand;
        private Panel panelAccent;
        private Panel panelLogo;
        private Label labelBrand;
        private Label labelBrandTitle;
        private Label labelBrandCaption;
        private Label labelWelcome;
        private Label labelSubtitle;
        private Label label1;
        private Label label2;
        private Label label7;
        private TextBox textBox7;
        private TextBox textBox8;
        private ComboBox comboBox1;
        private Button button4;
        private Label labelStatus;
    }
}

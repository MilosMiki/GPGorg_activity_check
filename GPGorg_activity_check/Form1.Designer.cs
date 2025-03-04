namespace GPGorg_activity_check
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            label1 = new Label();
            textBox1 = new TextBox();
            label2 = new Label();
            listBox1 = new ListBox();
            label3 = new Label();
            label4 = new Label();
            listBox2 = new ListBox();
            button1 = new Button();
            button2 = new Button();
            listBox3 = new ListBox();
            label5 = new Label();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            listBox4 = new ListBox();
            checkBox1 = new CheckBox();
            dateTimePicker1 = new DateTimePicker();
            dateTimePicker2 = new DateTimePicker();
            label6 = new Label();
            label7 = new Label();
            numericUpDown1 = new NumericUpDown();
            label8 = new Label();
            numericUpDown2 = new NumericUpDown();
            label9 = new Label();
            label10 = new Label();
            label11 = new Label();
            checkBox2 = new CheckBox();
            button6 = new Button();
            button7 = new Button();
            label12 = new Label();
            button8 = new Button();
            textBox2 = new TextBox();
            label13 = new Label();
            label14 = new Label();
            textBox3 = new TextBox();
            button9 = new Button();
            checkBox3 = new CheckBox();
            numericUpDown3 = new NumericUpDown();
            label15 = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            label16 = new Label();
            label17 = new Label();
            button10 = new Button();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(121, 15);
            label1.TabIndex = 0;
            label1.Text = "Paste raw HTML here:";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(17, 34);
            textBox1.MaxLength = 2147483646;
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(223, 252);
            textBox1.TabIndex = 1;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(258, 9);
            label2.Name = "label2";
            label2.Size = new Size(38, 15);
            label2.TabIndex = 2;
            label2.Text = "Posts:";
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(258, 34);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(285, 544);
            listBox1.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(557, 9);
            label3.Name = "label3";
            label3.Size = new Size(77, 15);
            label3.TabIndex = 4;
            label3.Text = "Users Posted:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(720, 9);
            label4.Name = "label4";
            label4.Size = new Size(164, 15);
            label4.TabIndex = 6;
            label4.Text = "Users Not Posted + Warnings:";
            // 
            // listBox2
            // 
            listBox2.FormattingEnabled = true;
            listBox2.ItemHeight = 15;
            listBox2.Location = new Point(720, 34);
            listBox2.Name = "listBox2";
            listBox2.Size = new Size(215, 529);
            listBox2.TabIndex = 7;
            // 
            // button1
            // 
            button1.Location = new Point(972, 553);
            button1.Name = "button1";
            button1.Size = new Size(145, 29);
            button1.TabIndex = 8;
            button1.Text = "Reload";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(972, 588);
            button2.Name = "button2";
            button2.Size = new Size(145, 28);
            button2.TabIndex = 9;
            button2.Text = "Save";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // listBox3
            // 
            listBox3.Font = new Font("Segoe UI", 6F, FontStyle.Regular, GraphicsUnit.Point);
            listBox3.FormattingEnabled = true;
            listBox3.ItemHeight = 11;
            listBox3.Location = new Point(952, 34);
            listBox3.Name = "listBox3";
            listBox3.Size = new Size(182, 279);
            listBox3.TabIndex = 10;
            listBox3.DrawItem += listBox3_DrawItem;
            listBox3.MeasureItem += listBox3_MeasureItem;
            listBox3.KeyDown += listBox3_KeyDown;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(952, 9);
            label5.Name = "label5";
            label5.Size = new Size(55, 15);
            label5.TabIndex = 11;
            label5.Text = "Progress:";
            // 
            // button3
            // 
            button3.Location = new Point(972, 399);
            button3.Name = "button3";
            button3.Size = new Size(145, 49);
            button3.TabIndex = 12;
            button3.Text = "Load new posts";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(972, 330);
            button4.Name = "button4";
            button4.Size = new Size(145, 49);
            button4.TabIndex = 13;
            button4.Text = "How to use this app?";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.Location = new Point(972, 467);
            button5.Name = "button5";
            button5.Size = new Size(145, 49);
            button5.TabIndex = 14;
            button5.Text = "Refresh users posted";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // listBox4
            // 
            listBox4.FormattingEnabled = true;
            listBox4.ItemHeight = 15;
            listBox4.Location = new Point(557, 34);
            listBox4.Name = "listBox4";
            listBox4.Size = new Size(148, 529);
            listBox4.TabIndex = 15;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(326, 639);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(195, 19);
            checkBox1.TabIndex = 16;
            checkBox1.Text = "First race? (ignore startDeadline)";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.Visible = false;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.Location = new Point(68, 581);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(164, 23);
            dateTimePicker1.TabIndex = 17;
            dateTimePicker1.ValueChanged += dateTimePicker1_ValueChanged;
            // 
            // dateTimePicker2
            // 
            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.Location = new Point(68, 617);
            dateTimePicker2.Name = "dateTimePicker2";
            dateTimePicker2.Size = new Size(164, 23);
            dateTimePicker2.TabIndex = 18;
            dateTimePicker2.ValueChanged += dateTimePicker2_ValueChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(28, 587);
            label6.Name = "label6";
            label6.Size = new Size(34, 15);
            label6.TabIndex = 19;
            label6.Text = "Start:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(28, 623);
            label7.Name = "label7";
            label7.Size = new Size(30, 15);
            label7.TabIndex = 20;
            label7.Text = "End:";
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(68, 552);
            numericUpDown1.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(48, 23);
            numericUpDown1.TabIndex = 21;
            numericUpDown1.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown1.ValueChanged += numericUpDown1_ValueChanged;
            // 
            // label8
            // 
            label8.Location = new Point(17, 546);
            label8.Name = "label8";
            label8.Size = new Size(58, 31);
            label8.TabIndex = 22;
            label8.Text = "Current Page:";
            // 
            // numericUpDown2
            // 
            numericUpDown2.Location = new Point(485, 598);
            numericUpDown2.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDown2.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.Size = new Size(48, 23);
            numericUpDown2.TabIndex = 23;
            numericUpDown2.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown2.Visible = false;
            numericUpDown2.ValueChanged += numericUpDown2_ValueChanged;
            // 
            // label9
            // 
            label9.Location = new Point(437, 598);
            label9.Name = "label9";
            label9.Size = new Size(43, 31);
            label9.TabIndex = 24;
            label9.Text = "Boost Page:";
            label9.Visible = false;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.BackColor = Color.Transparent;
            label10.Location = new Point(557, 614);
            label10.Name = "label10";
            label10.Size = new Size(248, 30);
            label10.TabIndex = 25;
            label10.Text = "Loaded posts are automatically saved to disk, \r\nuntil users+warnings are manually saved.";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.ForeColor = Color.Red;
            label11.Location = new Point(676, 572);
            label11.Name = "label11";
            label11.Size = new Size(45, 15);
            label11.TabIndex = 26;
            label11.Text = "sample";
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Checked = true;
            checkBox2.CheckState = CheckState.Checked;
            checkBox2.Location = new Point(122, 553);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(119, 19);
            checkBox2.TabIndex = 27;
            checkBox2.Text = "Save posts to disk";
            checkBox2.UseVisualStyleBackColor = true;
            checkBox2.CheckedChanged += checkBox2_CheckedChanged;
            // 
            // button6
            // 
            button6.Location = new Point(258, 581);
            button6.Name = "button6";
            button6.Size = new Size(130, 23);
            button6.TabIndex = 28;
            button6.Text = "Select post manually";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // button7
            // 
            button7.Location = new Point(258, 617);
            button7.Name = "button7";
            button7.Size = new Size(130, 23);
            button7.TabIndex = 29;
            button7.Text = "Select post manually";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.ForeColor = Color.Red;
            label12.Location = new Point(676, 587);
            label12.Name = "label12";
            label12.Size = new Size(45, 15);
            label12.TabIndex = 30;
            label12.Text = "sample";
            // 
            // button8
            // 
            button8.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            button8.Location = new Point(17, 467);
            button8.Margin = new Padding(3, 2, 3, 2);
            button8.Name = "button8";
            button8.Size = new Size(222, 34);
            button8.TabIndex = 31;
            button8.Text = "LOAD NEXT PAGE";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(17, 314);
            textBox2.MaxLength = 2147483646;
            textBox2.Multiline = true;
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(223, 39);
            textBox2.TabIndex = 32;
            textBox2.TextChanged += textBox2_TextChanged;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(17, 296);
            label13.Name = "label13";
            label13.Size = new Size(125, 15);
            label13.TabIndex = 33;
            label13.Text = "Paste thread URL here:";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(17, 356);
            label14.Name = "label14";
            label14.Size = new Size(127, 15);
            label14.TabIndex = 34;
            label14.Text = "Paste image path here:";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(17, 376);
            textBox3.MaxLength = 2147483646;
            textBox3.Multiline = true;
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(223, 39);
            textBox3.TabIndex = 35;
            textBox3.TextChanged += textBox3_TextChanged;
            // 
            // button9
            // 
            button9.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            button9.Location = new Point(17, 506);
            button9.Margin = new Padding(3, 2, 3, 2);
            button9.Name = "button9";
            button9.Size = new Size(222, 34);
            button9.TabIndex = 36;
            button9.Text = "SAVE SCREENSHOT";
            button9.UseVisualStyleBackColor = true;
            button9.Click += button9_Click;
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Checked = true;
            checkBox3.CheckState = CheckState.Checked;
            checkBox3.Location = new Point(17, 420);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(92, 19);
            checkBox3.TabIndex = 37;
            checkBox3.Text = "Enable timer";
            checkBox3.UseVisualStyleBackColor = true;
            checkBox3.CheckedChanged += checkBox3_CheckedChanged;
            // 
            // numericUpDown3
            // 
            numericUpDown3.Location = new Point(184, 442);
            numericUpDown3.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDown3.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown3.Name = "numericUpDown3";
            numericUpDown3.Size = new Size(48, 23);
            numericUpDown3.TabIndex = 38;
            numericUpDown3.Value = new decimal(new int[] { 60, 0, 0, 0 });
            numericUpDown3.ValueChanged += numericUpDown3_ValueChanged;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(17, 443);
            label15.Name = "label15";
            label15.Size = new Size(138, 15);
            label15.TabIndex = 39;
            label15.Text = "Timer delay (in minutes):";
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 5000;
            timer1.Tick += timer1_Tick;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.BackColor = Color.Transparent;
            label16.Location = new Point(557, 572);
            label16.Name = "label16";
            label16.Size = new Size(103, 30);
            label16.TabIndex = 40;
            label16.Text = "Last update:\r\nUpdates every day";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.BackColor = Color.Transparent;
            label17.Location = new Point(956, 525);
            label17.Name = "label17";
            label17.Size = new Size(167, 15);
            label17.TabIndex = 41;
            label17.Text = "Users+warnings (from txt file):";
            // 
            // button10
            // 
            button10.Location = new Point(972, 623);
            button10.Name = "button10";
            button10.Size = new Size(145, 28);
            button10.TabIndex = 42;
            button10.Text = "Reset (all to 0)";
            button10.UseVisualStyleBackColor = true;
            button10.Click += button10_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(1152, 656);
            Controls.Add(button10);
            Controls.Add(label17);
            Controls.Add(label16);
            Controls.Add(label12);
            Controls.Add(label11);
            Controls.Add(label15);
            Controls.Add(numericUpDown3);
            Controls.Add(checkBox3);
            Controls.Add(button9);
            Controls.Add(textBox3);
            Controls.Add(label14);
            Controls.Add(numericUpDown1);
            Controls.Add(label13);
            Controls.Add(textBox2);
            Controls.Add(button8);
            Controls.Add(button7);
            Controls.Add(button6);
            Controls.Add(checkBox2);
            Controls.Add(label9);
            Controls.Add(numericUpDown2);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(dateTimePicker2);
            Controls.Add(dateTimePicker1);
            Controls.Add(checkBox1);
            Controls.Add(listBox4);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(label5);
            Controls.Add(listBox3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(listBox2);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(listBox1);
            Controls.Add(label2);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Controls.Add(label10);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox textBox1;
        private Label label2;
        private ListBox listBox1;
        private Label label3;
        private Label label4;
        private ListBox listBox2;
        private Button button1;
        private Button button2;
        private ListBox listBox3;
        private Label label5;
        private Button button3;
        private Button button4;
        private Button button5;
        private ListBox listBox4;
        private CheckBox checkBox1;
        private DateTimePicker dateTimePicker1;
        private DateTimePicker dateTimePicker2;
        private Label label6;
        private Label label7;
        private NumericUpDown numericUpDown1;
        private Label label8;
        private NumericUpDown numericUpDown2;
        private Label label9;
        private Label label10;
        private Label label11;
        private CheckBox checkBox2;
        private Button button6;
        private Button button7;
        private Label label12;
        private Button button8;
        private TextBox textBox2;
        private Label label13;
        private Label label14;
        private TextBox textBox3;
        private Button button9;
        private CheckBox checkBox3;
        private NumericUpDown numericUpDown3;
        private Label label15;
        private System.Windows.Forms.Timer timer1;
        private Label label16;
        private Label label17;
        private Button button10;
    }
}
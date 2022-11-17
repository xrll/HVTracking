namespace HVTracking
{
    partial class HTrackingForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pnlCanvas1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Simu = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.DCount = new System.Windows.Forms.NumericUpDown();
            this.ORD = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Num = new System.Windows.Forms.NumericUpDown();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.pnlCanvas2 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlCanvas1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ORD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Num)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlCanvas2)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 320F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.pnlCanvas1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.richTextBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.pnlCanvas2, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1463, 938);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // pnlCanvas1
            // 
            this.pnlCanvas1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCanvas1.Location = new System.Drawing.Point(323, 3);
            this.pnlCanvas1.Name = "pnlCanvas1";
            this.tableLayoutPanel1.SetRowSpan(this.pnlCanvas1, 2);
            this.pnlCanvas1.Size = new System.Drawing.Size(565, 932);
            this.pnlCanvas1.TabIndex = 0;
            this.pnlCanvas1.TabStop = false;
            this.pnlCanvas1.SizeChanged += new System.EventHandler(this.pnlCanvas1_SizeChanged);
            this.pnlCanvas1.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlCanvas1_Paint);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Simu);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(314, 463);
            this.panel1.TabIndex = 3;
            // 
            // Simu
            // 
            this.Simu.Location = new System.Drawing.Point(63, 308);
            this.Simu.Margin = new System.Windows.Forms.Padding(4);
            this.Simu.Name = "Simu";
            this.Simu.Size = new System.Drawing.Size(178, 118);
            this.Simu.TabIndex = 8;
            this.Simu.Text = "Start";
            this.Simu.UseVisualStyleBackColor = true;
            this.Simu.Click += new System.EventHandler(this.Simu_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.DCount);
            this.panel2.Controls.Add(this.ORD);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.Num);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(314, 275);
            this.panel2.TabIndex = 7;
            // 
            // DCount
            // 
            this.DCount.Location = new System.Drawing.Point(18, 22);
            this.DCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.DCount.Name = "DCount";
            this.DCount.Size = new System.Drawing.Size(240, 38);
            this.DCount.TabIndex = 2;
            this.DCount.ValueChanged += new System.EventHandler(this.DCount_ValueChanged);
            // 
            // ORD
            // 
            this.ORD.Location = new System.Drawing.Point(155, 182);
            this.ORD.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.ORD.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ORD.Name = "ORD";
            this.ORD.Size = new System.Drawing.Size(103, 38);
            this.ORD.TabIndex = 6;
            this.ORD.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.ORD.ValueChanged += new System.EventHandler(this.ORD_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 31);
            this.label1.TabIndex = 3;
            this.label1.Text = "N:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 184);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 31);
            this.label2.TabIndex = 5;
            this.label2.Text = "Order:";
            // 
            // Num
            // 
            this.Num.Location = new System.Drawing.Point(95, 91);
            this.Num.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.Num.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.Num.Name = "Num";
            this.Num.Size = new System.Drawing.Size(163, 38);
            this.Num.TabIndex = 4;
            this.Num.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.Num.ValueChanged += new System.EventHandler(this.ORD_ValueChanged);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(3, 472);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(314, 463);
            this.richTextBox1.TabIndex = 4;
            this.richTextBox1.Text = "";
            // 
            // pnlCanvas2
            // 
            this.pnlCanvas2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCanvas2.Location = new System.Drawing.Point(894, 3);
            this.pnlCanvas2.Name = "pnlCanvas2";
            this.tableLayoutPanel1.SetRowSpan(this.pnlCanvas2, 2);
            this.pnlCanvas2.Size = new System.Drawing.Size(566, 932);
            this.pnlCanvas2.TabIndex = 1;
            this.pnlCanvas2.TabStop = false;
            this.pnlCanvas2.SizeChanged += new System.EventHandler(this.pnlCanvas2_SizeChanged);
            this.pnlCanvas2.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlCanvas2_Paint);
            // 
            // HTrackingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1463, 938);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "HTrackingForm";
            this.Text = "水平跟踪多项式MPC";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HTrackingForm_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlCanvas1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ORD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Num)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlCanvas2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private PictureBox pnlCanvas1;
        private PictureBox pnlCanvas2;
        private NumericUpDown DCount;
        private Panel panel1;
        private RichTextBox richTextBox1;
        private NumericUpDown Num;
        private Label label1;
        private NumericUpDown ORD;
        private Label label2;
        private Button Simu;
        private Panel panel2;
    }
}
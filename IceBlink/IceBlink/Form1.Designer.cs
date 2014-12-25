namespace IceBlink
{
    partial class Form1
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
        	this.components = new System.ComponentModel.Container();
        	System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
        	this.panelBottom = new System.Windows.Forms.Panel();
        	this.btnAdvLogFontDecrease = new System.Windows.Forms.Button();
        	this.btnAdvLogFontIncrease = new System.Windows.Forms.Button();
        	this.rtxtLog = new System.Windows.Forms.RichTextBox();
        	this.panelRight = new System.Windows.Forms.Panel();
        	this.pc_button_5 = new System.Windows.Forms.Button();
        	this.pc_button_4 = new System.Windows.Forms.Button();
        	this.pc_button_0 = new System.Windows.Forms.Button();
        	this.pc_button_3 = new System.Windows.Forms.Button();
        	this.pc_button_2 = new System.Windows.Forms.Button();
        	this.pc_button_1 = new System.Windows.Forms.Button();
        	this.panelLeft = new System.Windows.Forms.Panel();
        	this.btnQuickSave = new System.Windows.Forms.Button();
        	this.pnlWorldTime = new System.Windows.Forms.Panel();
        	this.btnRest = new System.Windows.Forms.Button();
        	this.label1 = new System.Windows.Forms.Label();
        	this.txtFPS = new System.Windows.Forms.TextBox();
        	this.btnInventory = new System.Windows.Forms.Button();
        	this.btnJournal = new System.Windows.Forms.Button();
        	this.chkGrid = new System.Windows.Forms.CheckBox();
        	this.btnSettings = new System.Windows.Forms.Button();
        	this.timer = new System.Windows.Forms.Timer(this.components);
        	this.FPStimer = new System.Windows.Forms.Timer(this.components);
        	this.AnimationTimer = new System.Windows.Forms.Timer(this.components);
        	this.areaMusicTimer = new System.Windows.Forms.Timer(this.components);
        	this.areaSoundsTimer = new System.Windows.Forms.Timer(this.components);
        	this.floatyTextTimer = new System.Windows.Forms.Timer(this.components);
        	this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
        	this.realTimer = new System.Windows.Forms.Timer(this.components);
        	this.renderPanel = new System.Windows.Forms.Panel();
        	this.scrollTimer = new System.Windows.Forms.Timer(this.components);
        	this.panelBottom.SuspendLayout();
        	this.panelRight.SuspendLayout();
        	this.panelLeft.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// panelBottom
        	// 
        	this.panelBottom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.panelBottom.BackColor = System.Drawing.Color.Transparent;
        	this.panelBottom.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
        	this.panelBottom.Controls.Add(this.btnAdvLogFontDecrease);
        	this.panelBottom.Controls.Add(this.btnAdvLogFontIncrease);
        	this.panelBottom.Controls.Add(this.rtxtLog);
        	this.panelBottom.Location = new System.Drawing.Point(93, 531);
        	this.panelBottom.Name = "panelBottom";
        	this.panelBottom.Size = new System.Drawing.Size(691, 90);
        	this.panelBottom.TabIndex = 22;
        	// 
        	// btnAdvLogFontDecrease
        	// 
        	this.btnAdvLogFontDecrease.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        	this.btnAdvLogFontDecrease.BackColor = System.Drawing.Color.Gray;
        	this.btnAdvLogFontDecrease.FlatAppearance.BorderSize = 2;
        	this.btnAdvLogFontDecrease.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.btnAdvLogFontDecrease.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.btnAdvLogFontDecrease.Location = new System.Drawing.Point(530, 46);
        	this.btnAdvLogFontDecrease.Margin = new System.Windows.Forms.Padding(0);
        	this.btnAdvLogFontDecrease.Name = "btnAdvLogFontDecrease";
        	this.btnAdvLogFontDecrease.Size = new System.Drawing.Size(32, 32);
        	this.btnAdvLogFontDecrease.TabIndex = 27;
        	this.btnAdvLogFontDecrease.Text = "-A";
        	this.btnAdvLogFontDecrease.UseVisualStyleBackColor = false;
        	this.btnAdvLogFontDecrease.Visible = false;
        	this.btnAdvLogFontDecrease.Click += new System.EventHandler(this.btnAdvLogFontDecrease_Click);
        	// 
        	// btnAdvLogFontIncrease
        	// 
        	this.btnAdvLogFontIncrease.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        	this.btnAdvLogFontIncrease.BackColor = System.Drawing.Color.Gray;
        	this.btnAdvLogFontIncrease.FlatAppearance.BorderSize = 2;
        	this.btnAdvLogFontIncrease.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.btnAdvLogFontIncrease.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.btnAdvLogFontIncrease.Location = new System.Drawing.Point(530, 11);
        	this.btnAdvLogFontIncrease.Margin = new System.Windows.Forms.Padding(0);
        	this.btnAdvLogFontIncrease.Name = "btnAdvLogFontIncrease";
        	this.btnAdvLogFontIncrease.Size = new System.Drawing.Size(32, 32);
        	this.btnAdvLogFontIncrease.TabIndex = 26;
        	this.btnAdvLogFontIncrease.Text = "+A";
        	this.btnAdvLogFontIncrease.UseVisualStyleBackColor = false;
        	this.btnAdvLogFontIncrease.Visible = false;
        	this.btnAdvLogFontIncrease.Click += new System.EventHandler(this.btnAdvLogFontIncrease_Click);
        	// 
        	// rtxtLog
        	// 
        	this.rtxtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Left) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.rtxtLog.BackColor = System.Drawing.Color.Black;
        	this.rtxtLog.Cursor = System.Windows.Forms.Cursors.Default;
        	this.rtxtLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.rtxtLog.ForeColor = System.Drawing.Color.Silver;
        	this.rtxtLog.Location = new System.Drawing.Point(178, 8);
        	this.rtxtLog.Name = "rtxtLog";
        	this.rtxtLog.ReadOnly = true;
        	this.rtxtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
        	this.rtxtLog.Size = new System.Drawing.Size(349, 74);
        	this.rtxtLog.TabIndex = 25;
        	this.rtxtLog.Text = "";
        	// 
        	// panelRight
        	// 
        	this.panelRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.panelRight.BackColor = System.Drawing.Color.Transparent;
        	this.panelRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
        	this.panelRight.Controls.Add(this.pc_button_5);
        	this.panelRight.Controls.Add(this.pc_button_4);
        	this.panelRight.Controls.Add(this.pc_button_0);
        	this.panelRight.Controls.Add(this.pc_button_3);
        	this.panelRight.Controls.Add(this.pc_button_2);
        	this.panelRight.Controls.Add(this.pc_button_1);
        	this.panelRight.Location = new System.Drawing.Point(784, 31);
        	this.panelRight.Name = "panelRight";
        	this.panelRight.Size = new System.Drawing.Size(90, 590);
        	this.panelRight.TabIndex = 21;
        	// 
        	// pc_button_5
        	// 
        	this.pc_button_5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        	this.pc_button_5.BackColor = System.Drawing.Color.Gray;
        	this.pc_button_5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
        	this.pc_button_5.Enabled = false;
        	this.pc_button_5.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
        	this.pc_button_5.FlatAppearance.BorderSize = 0;
        	this.pc_button_5.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
        	this.pc_button_5.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
        	this.pc_button_5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.pc_button_5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.pc_button_5.ForeColor = System.Drawing.Color.White;
        	this.pc_button_5.Location = new System.Drawing.Point(8, 572);
        	this.pc_button_5.Name = "pc_button_5";
        	this.pc_button_5.Size = new System.Drawing.Size(72, 106);
        	this.pc_button_5.TabIndex = 17;
        	this.pc_button_5.TextAlign = System.Drawing.ContentAlignment.BottomRight;
        	this.toolTip1.SetToolTip(this.pc_button_5, "Right Click to \r\nMake Party Leader\r\n");
        	this.pc_button_5.UseVisualStyleBackColor = false;
        	this.pc_button_5.Visible = false;
        	this.pc_button_5.Click += new System.EventHandler(this.pc_button_5_Click);
        	this.pc_button_5.Paint += new System.Windows.Forms.PaintEventHandler(this.pc_button_5_Paint);
        	this.pc_button_5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pc_button_5_MouseDown);
        	// 
        	// pc_button_4
        	// 
        	this.pc_button_4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        	this.pc_button_4.BackColor = System.Drawing.Color.Gray;
        	this.pc_button_4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
        	this.pc_button_4.Enabled = false;
        	this.pc_button_4.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
        	this.pc_button_4.FlatAppearance.BorderSize = 0;
        	this.pc_button_4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
        	this.pc_button_4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
        	this.pc_button_4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.pc_button_4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.pc_button_4.ForeColor = System.Drawing.Color.White;
        	this.pc_button_4.Location = new System.Drawing.Point(8, 459);
        	this.pc_button_4.Name = "pc_button_4";
        	this.pc_button_4.Size = new System.Drawing.Size(72, 106);
        	this.pc_button_4.TabIndex = 16;
        	this.pc_button_4.TextAlign = System.Drawing.ContentAlignment.BottomRight;
        	this.toolTip1.SetToolTip(this.pc_button_4, "Right Click to \r\nMake Party Leader\r\n");
        	this.pc_button_4.UseVisualStyleBackColor = false;
        	this.pc_button_4.Visible = false;
        	this.pc_button_4.Click += new System.EventHandler(this.pc_button_4_Click);
        	this.pc_button_4.Paint += new System.Windows.Forms.PaintEventHandler(this.pc_button_4_Paint);
        	this.pc_button_4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pc_button_4_MouseDown);
        	// 
        	// pc_button_0
        	// 
        	this.pc_button_0.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        	this.pc_button_0.BackColor = System.Drawing.Color.Gray;
        	this.pc_button_0.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
        	this.pc_button_0.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
        	this.pc_button_0.FlatAppearance.BorderSize = 0;
        	this.pc_button_0.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
        	this.pc_button_0.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
        	this.pc_button_0.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.pc_button_0.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.pc_button_0.ForeColor = System.Drawing.Color.White;
        	this.pc_button_0.Location = new System.Drawing.Point(8, 7);
        	this.pc_button_0.Name = "pc_button_0";
        	this.pc_button_0.Size = new System.Drawing.Size(72, 106);
        	this.pc_button_0.TabIndex = 13;
        	this.pc_button_0.TextAlign = System.Drawing.ContentAlignment.BottomRight;
        	this.toolTip1.SetToolTip(this.pc_button_0, "Right Click to \r\nMake Party Leader");
        	this.pc_button_0.UseVisualStyleBackColor = false;
        	this.pc_button_0.Visible = false;
        	this.pc_button_0.Click += new System.EventHandler(this.pc_button_0_Click);
        	this.pc_button_0.Paint += new System.Windows.Forms.PaintEventHandler(this.pc_button_0_Paint);
        	this.pc_button_0.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pc_button_0_MouseDown);
        	// 
        	// pc_button_3
        	// 
        	this.pc_button_3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        	this.pc_button_3.BackColor = System.Drawing.Color.Gray;
        	this.pc_button_3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
        	this.pc_button_3.Enabled = false;
        	this.pc_button_3.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
        	this.pc_button_3.FlatAppearance.BorderSize = 0;
        	this.pc_button_3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
        	this.pc_button_3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
        	this.pc_button_3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.pc_button_3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.pc_button_3.ForeColor = System.Drawing.Color.White;
        	this.pc_button_3.Location = new System.Drawing.Point(8, 346);
        	this.pc_button_3.Name = "pc_button_3";
        	this.pc_button_3.Size = new System.Drawing.Size(72, 106);
        	this.pc_button_3.TabIndex = 14;
        	this.pc_button_3.TextAlign = System.Drawing.ContentAlignment.BottomRight;
        	this.toolTip1.SetToolTip(this.pc_button_3, "Right Click to \r\nMake Party Leader\r\n");
        	this.pc_button_3.UseVisualStyleBackColor = false;
        	this.pc_button_3.Visible = false;
        	this.pc_button_3.Click += new System.EventHandler(this.pc_button_3_Click);
        	this.pc_button_3.Paint += new System.Windows.Forms.PaintEventHandler(this.pc_button_3_Paint);
        	this.pc_button_3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pc_button_3_MouseDown);
        	// 
        	// pc_button_2
        	// 
        	this.pc_button_2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        	this.pc_button_2.BackColor = System.Drawing.Color.Gray;
        	this.pc_button_2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
        	this.pc_button_2.Enabled = false;
        	this.pc_button_2.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
        	this.pc_button_2.FlatAppearance.BorderSize = 0;
        	this.pc_button_2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
        	this.pc_button_2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
        	this.pc_button_2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.pc_button_2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.pc_button_2.ForeColor = System.Drawing.Color.White;
        	this.pc_button_2.Location = new System.Drawing.Point(8, 233);
        	this.pc_button_2.Name = "pc_button_2";
        	this.pc_button_2.Size = new System.Drawing.Size(72, 106);
        	this.pc_button_2.TabIndex = 15;
        	this.pc_button_2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
        	this.toolTip1.SetToolTip(this.pc_button_2, "Right Click to \r\nMake Party Leader\r\n");
        	this.pc_button_2.UseVisualStyleBackColor = false;
        	this.pc_button_2.Visible = false;
        	this.pc_button_2.Click += new System.EventHandler(this.pc_button_2_Click);
        	this.pc_button_2.Paint += new System.Windows.Forms.PaintEventHandler(this.pc_button_2_Paint);
        	this.pc_button_2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pc_button_2_MouseDown);
        	// 
        	// pc_button_1
        	// 
        	this.pc_button_1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        	this.pc_button_1.BackColor = System.Drawing.Color.Gray;
        	this.pc_button_1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
        	this.pc_button_1.Enabled = false;
        	this.pc_button_1.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
        	this.pc_button_1.FlatAppearance.BorderSize = 0;
        	this.pc_button_1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
        	this.pc_button_1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
        	this.pc_button_1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.pc_button_1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.pc_button_1.ForeColor = System.Drawing.Color.White;
        	this.pc_button_1.Location = new System.Drawing.Point(8, 120);
        	this.pc_button_1.Name = "pc_button_1";
        	this.pc_button_1.Size = new System.Drawing.Size(72, 106);
        	this.pc_button_1.TabIndex = 4;
        	this.pc_button_1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
        	this.toolTip1.SetToolTip(this.pc_button_1, "Right Click to \r\nMake Party Leader\r\n");
        	this.pc_button_1.UseVisualStyleBackColor = false;
        	this.pc_button_1.Visible = false;
        	this.pc_button_1.Click += new System.EventHandler(this.pc_button_1_Click);
        	this.pc_button_1.Paint += new System.Windows.Forms.PaintEventHandler(this.pc_button_1_Paint);
        	this.pc_button_1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pc_button_1_MouseDown);
        	// 
        	// panelLeft
        	// 
        	this.panelLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Left)));
        	this.panelLeft.BackColor = System.Drawing.Color.Transparent;
        	this.panelLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
        	this.panelLeft.Controls.Add(this.btnQuickSave);
        	this.panelLeft.Controls.Add(this.pnlWorldTime);
        	this.panelLeft.Controls.Add(this.btnRest);
        	this.panelLeft.Controls.Add(this.label1);
        	this.panelLeft.Controls.Add(this.txtFPS);
        	this.panelLeft.Controls.Add(this.btnInventory);
        	this.panelLeft.Controls.Add(this.btnJournal);
        	this.panelLeft.Controls.Add(this.chkGrid);
        	this.panelLeft.Controls.Add(this.btnSettings);
        	this.panelLeft.Location = new System.Drawing.Point(3, 31);
        	this.panelLeft.Name = "panelLeft";
        	this.panelLeft.Size = new System.Drawing.Size(90, 590);
        	this.panelLeft.TabIndex = 20;
        	// 
        	// btnQuickSave
        	// 
        	this.btnQuickSave.BackColor = System.Drawing.Color.Gray;
        	this.btnQuickSave.FlatAppearance.BorderSize = 2;
        	this.btnQuickSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.btnQuickSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.btnQuickSave.Location = new System.Drawing.Point(21, 362);
        	this.btnQuickSave.Margin = new System.Windows.Forms.Padding(0);
        	this.btnQuickSave.Name = "btnQuickSave";
        	this.btnQuickSave.Size = new System.Drawing.Size(48, 48);
        	this.btnQuickSave.TabIndex = 27;
        	this.btnQuickSave.Text = "Quick\r\nSave";
        	this.btnQuickSave.UseVisualStyleBackColor = false;
        	this.btnQuickSave.Visible = false;
        	this.btnQuickSave.Click += new System.EventHandler(this.btnQuickSave_Click);
        	// 
        	// pnlWorldTime
        	// 
        	this.pnlWorldTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.pnlWorldTime.Location = new System.Drawing.Point(20, 522);
        	this.pnlWorldTime.Name = "pnlWorldTime";
        	this.pnlWorldTime.Size = new System.Drawing.Size(50, 50);
        	this.pnlWorldTime.TabIndex = 23;
        	this.pnlWorldTime.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlWorldTime_Paint);
        	// 
        	// btnRest
        	// 
        	this.btnRest.Location = new System.Drawing.Point(21, 235);
        	this.btnRest.Name = "btnRest";
        	this.btnRest.Size = new System.Drawing.Size(48, 48);
        	this.btnRest.TabIndex = 22;
        	this.btnRest.UseVisualStyleBackColor = true;
        	this.btnRest.Click += new System.EventHandler(this.btnRest_Click);
        	// 
        	// label1
        	// 
        	this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.label1.AutoSize = true;
        	this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.label1.Location = new System.Drawing.Point(22, 496);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(24, 13);
        	this.label1.TabIndex = 21;
        	this.label1.Text = "fps:";
        	this.label1.Visible = false;
        	// 
        	// txtFPS
        	// 
        	this.txtFPS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.txtFPS.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.txtFPS.Location = new System.Drawing.Point(48, 492);
        	this.txtFPS.Name = "txtFPS";
        	this.txtFPS.Size = new System.Drawing.Size(20, 20);
        	this.txtFPS.TabIndex = 20;
        	this.txtFPS.Visible = false;
        	// 
        	// btnInventory
        	// 
        	this.btnInventory.Location = new System.Drawing.Point(21, 11);
        	this.btnInventory.Name = "btnInventory";
        	this.btnInventory.Size = new System.Drawing.Size(48, 48);
        	this.btnInventory.TabIndex = 18;
        	this.btnInventory.UseVisualStyleBackColor = true;
        	this.btnInventory.Click += new System.EventHandler(this.btnInventory_Click);
        	// 
        	// btnJournal
        	// 
        	this.btnJournal.Location = new System.Drawing.Point(21, 67);
        	this.btnJournal.Name = "btnJournal";
        	this.btnJournal.Size = new System.Drawing.Size(48, 48);
        	this.btnJournal.TabIndex = 17;
        	this.btnJournal.UseVisualStyleBackColor = true;
        	this.btnJournal.Click += new System.EventHandler(this.btnJournal_Click);
        	// 
        	// chkGrid
        	// 
        	this.chkGrid.Appearance = System.Windows.Forms.Appearance.Button;
        	this.chkGrid.Checked = true;
        	this.chkGrid.CheckState = System.Windows.Forms.CheckState.Checked;
        	this.chkGrid.FlatAppearance.BorderColor = System.Drawing.Color.Black;
        	this.chkGrid.FlatAppearance.CheckedBackColor = System.Drawing.Color.Red;
        	this.chkGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.chkGrid.Location = new System.Drawing.Point(21, 179);
        	this.chkGrid.Margin = new System.Windows.Forms.Padding(0);
        	this.chkGrid.Name = "chkGrid";
        	this.chkGrid.Size = new System.Drawing.Size(48, 48);
        	this.chkGrid.TabIndex = 19;
        	this.chkGrid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        	this.chkGrid.UseVisualStyleBackColor = true;
        	this.chkGrid.CheckedChanged += new System.EventHandler(this.chkGrid_CheckedChanged);
        	// 
        	// btnSettings
        	// 
        	this.btnSettings.Location = new System.Drawing.Point(21, 123);
        	this.btnSettings.Name = "btnSettings";
        	this.btnSettings.Size = new System.Drawing.Size(48, 48);
        	this.btnSettings.TabIndex = 16;
        	this.btnSettings.UseVisualStyleBackColor = true;
        	this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
        	// 
        	// timer
        	// 
        	this.timer.Enabled = true;
        	this.timer.Interval = 1;
        	this.timer.Tick += new System.EventHandler(this.timer_Tick);
        	// 
        	// FPStimer
        	// 
        	this.FPStimer.Enabled = true;
        	this.FPStimer.Interval = 500;
        	this.FPStimer.Tick += new System.EventHandler(this.FPStimer_Tick);
        	// 
        	// AnimationTimer
        	// 
        	this.AnimationTimer.Enabled = true;
        	this.AnimationTimer.Interval = 16;
        	this.AnimationTimer.Tick += new System.EventHandler(this.AnimationTimer_Tick);
        	// 
        	// areaMusicTimer
        	// 
        	this.areaMusicTimer.Tick += new System.EventHandler(this.areaMusicTimer_Tick);
        	// 
        	// areaSoundsTimer
        	// 
        	this.areaSoundsTimer.Tick += new System.EventHandler(this.areaSoundsTimer_Tick);
        	// 
        	// realTimer
        	// 
        	this.realTimer.Enabled = true;
        	this.realTimer.Interval = 10000;
        	this.realTimer.Tick += new System.EventHandler(this.realTimer_Tick);
        	// 
        	// renderPanel
        	// 
        	this.renderPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Left) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.renderPanel.BackColor = System.Drawing.Color.Black;
        	this.renderPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        	this.renderPanel.Location = new System.Drawing.Point(93, 31);
        	this.renderPanel.Name = "renderPanel";
        	this.renderPanel.Size = new System.Drawing.Size(691, 500);
        	this.renderPanel.TabIndex = 0;
        	this.renderPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.renderPanel_MouseDown);
        	this.renderPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.renderPanel_MouseMove);
        	// 
        	// scrollTimer
        	// 
        	this.scrollTimer.Enabled = true;
        	this.scrollTimer.Interval = 25;
        	// 
        	// Form1
        	// 
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
        	this.BackgroundImage = global::IceBlink.Properties.Resources.standard;
        	this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        	this.ClientSize = new System.Drawing.Size(877, 624);
        	this.Controls.Add(this.renderPanel);
        	this.Controls.Add(this.panelBottom);
        	this.Controls.Add(this.panelRight);
        	this.Controls.Add(this.panelLeft);
        	this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        	this.MinimizeBox = false;
        	this.MinimumSize = new System.Drawing.Size(666, 494);
        	this.Name = "Form1";
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        	this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
        	this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
        	this.Load += new System.EventHandler(this.Form1_Load);
        	this.Resize += new System.EventHandler(this.Form1_Resize);
        	this.Controls.SetChildIndex(this.panelLeft, 0);
        	this.Controls.SetChildIndex(this.panelRight, 0);
        	this.Controls.SetChildIndex(this.panelBottom, 0);
        	this.Controls.SetChildIndex(this.renderPanel, 0);
        	this.panelBottom.ResumeLayout(false);
        	this.panelRight.ResumeLayout(false);
        	this.panelLeft.ResumeLayout(false);
        	this.panelLeft.PerformLayout();
        	this.ResumeLayout(false);
        }
        private System.Windows.Forms.Timer scrollTimer;

        #endregion

        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnJournal;
        private System.Windows.Forms.Button btnInventory;
        private System.Windows.Forms.CheckBox chkGrid;
        private System.Windows.Forms.Timer FPStimer;
        private System.Windows.Forms.TextBox txtFPS;
        public System.Windows.Forms.Timer timer;
        public System.Windows.Forms.Timer AnimationTimer;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button pc_button_0;
        public System.Windows.Forms.Button pc_button_1;
        public System.Windows.Forms.Button pc_button_3;
        public System.Windows.Forms.Button pc_button_2;
        public System.Windows.Forms.Timer areaMusicTimer;
        public System.Windows.Forms.Timer areaSoundsTimer;
        public System.Windows.Forms.Button pc_button_5;
        public System.Windows.Forms.Button pc_button_4;
        public System.Windows.Forms.Timer floatyTextTimer;
        public System.Windows.Forms.Button btnRest;
        public System.Windows.Forms.Panel renderPanel;
        public System.Windows.Forms.Panel pnlWorldTime;
        public System.Windows.Forms.Timer realTimer;
        public System.Windows.Forms.ToolTip toolTip1;
        public System.Windows.Forms.Panel panelLeft;
        public System.Windows.Forms.Panel panelRight;
        public System.Windows.Forms.Panel panelBottom;
        public System.Windows.Forms.RichTextBox rtxtLog;
        public System.Windows.Forms.Button btnAdvLogFontDecrease;
        public System.Windows.Forms.Button btnAdvLogFontIncrease;
        public System.Windows.Forms.Button btnQuickSave;
    }
}


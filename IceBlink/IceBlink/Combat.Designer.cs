namespace IceBlink
{
    partial class Combat
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
        	System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Combat));
        	this.pictureBox1 = new System.Windows.Forms.PictureBox();
        	this.rtxtLog = new System.Windows.Forms.RichTextBox();
        	this.gbPartyAction = new IceBlink.IceBlinkGroupBoxMedium();
        	this.btnRunAway = new IceBlink.IceBlinkButtonMedium();
        	this.btnStayFight = new IceBlink.IceBlinkButtonMedium();
        	this.btnContinue = new IceBlink.IceBlinkButtonLarge();
        	this.lblMouseInfo = new System.Windows.Forms.Label();
        	this.txtInfo = new System.Windows.Forms.Label();
        	this.gbCreatures = new IceBlink.IceBlinkGroupBoxMedium();
        	this.gbCharacters = new IceBlink.IceBlinkGroupBoxMedium();
        	this.gbPlayerTurn = new IceBlink.IceBlinkGroupBoxMedium();
        	this.btnUseTrait = new IceBlink.IceBlinkButtonMedium();
        	this.btnUseSkill = new IceBlink.IceBlinkButtonMedium();
        	this.btnRangedAttack = new IceBlink.IceBlinkButtonMedium();
        	this.btnUseSpell = new IceBlink.IceBlinkButtonMedium();
        	this.btnUseItem = new IceBlink.IceBlinkButtonMedium();
        	this.btnDelayTurn = new IceBlink.IceBlinkButtonMedium();
        	this.rtxtInfo = new System.Windows.Forms.RichTextBox();
        	this.panel1 = new System.Windows.Forms.Panel();
        	this.combatRenderPanel = new System.Windows.Forms.Panel();
        	this.chkGrid = new System.Windows.Forms.CheckBox();
        	this.numAnimationDelay = new System.Windows.Forms.NumericUpDown();
        	this.label6 = new System.Windows.Forms.Label();
        	this.chkFacing = new System.Windows.Forms.CheckBox();
        	this.chkShowRange = new System.Windows.Forms.CheckBox();
        	this.combatTimer = new System.Windows.Forms.Timer(this.components);
        	this.gbMovesLeft = new IceBlink.IceBlinkGroupBoxMedium();
        	this.lblMovesLeft = new System.Windows.Forms.Label();
        	this.chkHoverOnly = new System.Windows.Forms.CheckBox();
        	this.scrollTimer = new System.Windows.Forms.Timer(this.components);
        	((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
        	this.gbPartyAction.SuspendLayout();
        	this.gbPlayerTurn.SuspendLayout();
        	this.panel1.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.numAnimationDelay)).BeginInit();
        	this.gbMovesLeft.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// pictureBox1
        	// 
        	this.pictureBox1.BackColor = System.Drawing.Color.Black;
        	this.pictureBox1.Location = new System.Drawing.Point(27, 629);
        	this.pictureBox1.Name = "pictureBox1";
        	this.pictureBox1.Size = new System.Drawing.Size(21, 21);
        	this.pictureBox1.TabIndex = 1;
        	this.pictureBox1.TabStop = false;
        	this.pictureBox1.Visible = false;
        	// 
        	// rtxtLog
        	// 
        	this.rtxtLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.rtxtLog.BackColor = System.Drawing.SystemColors.ControlDarkDark;
        	this.rtxtLog.Cursor = System.Windows.Forms.Cursors.Default;
        	this.rtxtLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.rtxtLog.Location = new System.Drawing.Point(1017, 34);
        	this.rtxtLog.Name = "rtxtLog";
        	this.rtxtLog.ReadOnly = true;
        	this.rtxtLog.Size = new System.Drawing.Size(265, 341);
        	this.rtxtLog.TabIndex = 24;
        	this.rtxtLog.Text = "";
        	// 
        	// gbPartyAction
        	// 
        	this.gbPartyAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        	this.gbPartyAction.BackColor = System.Drawing.Color.Transparent;
        	this.gbPartyAction.BackgroundColor = System.Drawing.Color.LightSlateGray;
        	this.gbPartyAction.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        	this.gbPartyAction.BorderThickness = 2F;
        	this.gbPartyAction.Controls.Add(this.btnRunAway);
        	this.gbPartyAction.Controls.Add(this.btnStayFight);
        	this.gbPartyAction.HeaderForeColor = System.Drawing.Color.White;
        	this.gbPartyAction.HeaderImage = ((System.Drawing.Image)(resources.GetObject("gbPartyAction.HeaderImage")));
        	this.gbPartyAction.HeaderShadowColor = System.Drawing.Color.Black;
        	this.gbPartyAction.Location = new System.Drawing.Point(1017, 593);
        	this.gbPartyAction.Name = "gbPartyAction";
        	this.gbPartyAction.Size = new System.Drawing.Size(265, 80);
        	this.gbPartyAction.TabIndex = 25;
        	this.gbPartyAction.TabStop = false;
        	this.gbPartyAction.Text = "PARTY ACTION";
        	this.gbPartyAction.TextIB = "iceBlinkGBMedium1";
        	// 
        	// btnRunAway
        	// 
        	this.btnRunAway.BackColor = System.Drawing.Color.Transparent;
        	this.btnRunAway.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnRunAway.BackgroundImage")));
        	this.btnRunAway.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        	this.btnRunAway.DisabledImage = null;
        	this.btnRunAway.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnRunAway.FlatAppearance.BorderSize = 0;
        	this.btnRunAway.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnRunAway.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnRunAway.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.btnRunAway.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.btnRunAway.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnRunAway.HoverImage")));
        	this.btnRunAway.Location = new System.Drawing.Point(138, 31);
        	this.btnRunAway.Name = "btnRunAway";
        	this.btnRunAway.NormalImage = ((System.Drawing.Image)(resources.GetObject("btnRunAway.NormalImage")));
        	this.btnRunAway.PressedImage = ((System.Drawing.Image)(resources.GetObject("btnRunAway.PressedImage")));
        	this.btnRunAway.Size = new System.Drawing.Size(118, 43);
        	this.btnRunAway.TabIndex = 22;
        	this.btnRunAway.TextIB = "Run Away";
        	this.btnRunAway.UseVisualStyleBackColor = true;
        	this.btnRunAway.Click += new System.EventHandler(this.btnRunAway_Click);
        	// 
        	// btnStayFight
        	// 
        	this.btnStayFight.BackColor = System.Drawing.Color.Transparent;
        	this.btnStayFight.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnStayFight.BackgroundImage")));
        	this.btnStayFight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        	this.btnStayFight.DisabledImage = null;
        	this.btnStayFight.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnStayFight.FlatAppearance.BorderSize = 0;
        	this.btnStayFight.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnStayFight.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnStayFight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.btnStayFight.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.btnStayFight.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnStayFight.HoverImage")));
        	this.btnStayFight.Location = new System.Drawing.Point(6, 31);
        	this.btnStayFight.Name = "btnStayFight";
        	this.btnStayFight.NormalImage = ((System.Drawing.Image)(resources.GetObject("btnStayFight.NormalImage")));
        	this.btnStayFight.PressedImage = ((System.Drawing.Image)(resources.GetObject("btnStayFight.PressedImage")));
        	this.btnStayFight.Size = new System.Drawing.Size(126, 43);
        	this.btnStayFight.TabIndex = 21;
        	this.btnStayFight.TextIB = "Begin Combat";
        	this.btnStayFight.UseVisualStyleBackColor = true;
        	this.btnStayFight.Click += new System.EventHandler(this.btnStayFight_Click);
        	// 
        	// btnContinue
        	// 
        	this.btnContinue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        	this.btnContinue.BackColor = System.Drawing.Color.Transparent;
        	this.btnContinue.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnContinue.BackgroundImage")));
        	this.btnContinue.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        	this.btnContinue.DisabledImage = null;
        	this.btnContinue.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnContinue.FlatAppearance.BorderSize = 0;
        	this.btnContinue.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnContinue.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnContinue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.btnContinue.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.btnContinue.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnContinue.HoverImage")));
        	this.btnContinue.Location = new System.Drawing.Point(1017, 677);
        	this.btnContinue.Name = "btnContinue";
        	this.btnContinue.NormalImage = ((System.Drawing.Image)(resources.GetObject("btnContinue.NormalImage")));
        	this.btnContinue.PressedImage = ((System.Drawing.Image)(resources.GetObject("btnContinue.PressedImage")));
        	this.btnContinue.Size = new System.Drawing.Size(265, 30);
        	this.btnContinue.TabIndex = 26;
        	this.btnContinue.TextIB = "CONTINUE";
        	this.btnContinue.UseVisualStyleBackColor = true;
        	this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
        	// 
        	// lblMouseInfo
        	// 
        	this.lblMouseInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.lblMouseInfo.AutoSize = true;
        	this.lblMouseInfo.BackColor = System.Drawing.SystemColors.ControlDarkDark;
        	this.lblMouseInfo.Location = new System.Drawing.Point(10, 652);
        	this.lblMouseInfo.Name = "lblMouseInfo";
        	this.lblMouseInfo.Size = new System.Drawing.Size(15, 13);
        	this.lblMouseInfo.TabIndex = 30;
        	this.lblMouseInfo.Text = "m";
        	this.lblMouseInfo.Visible = false;
        	// 
        	// txtInfo
        	// 
        	this.txtInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.txtInfo.AutoSize = true;
        	this.txtInfo.BackColor = System.Drawing.SystemColors.ControlDarkDark;
        	this.txtInfo.Location = new System.Drawing.Point(12, 629);
        	this.txtInfo.Name = "txtInfo";
        	this.txtInfo.Size = new System.Drawing.Size(9, 13);
        	this.txtInfo.TabIndex = 29;
        	this.txtInfo.Text = "i";
        	this.txtInfo.Visible = false;
        	// 
        	// gbCreatures
        	// 
        	this.gbCreatures.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Left)));
        	this.gbCreatures.BackColor = System.Drawing.Color.Transparent;
        	this.gbCreatures.BackgroundColor = System.Drawing.Color.LightSlateGray;
        	this.gbCreatures.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        	this.gbCreatures.BorderThickness = 2F;
        	this.gbCreatures.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.gbCreatures.HeaderForeColor = System.Drawing.Color.White;
        	this.gbCreatures.HeaderImage = ((System.Drawing.Image)(resources.GetObject("gbCreatures.HeaderImage")));
        	this.gbCreatures.HeaderShadowColor = System.Drawing.Color.Black;
        	this.gbCreatures.Location = new System.Drawing.Point(8, 286);
        	this.gbCreatures.Name = "gbCreatures";
        	this.gbCreatures.Size = new System.Drawing.Size(235, 330);
        	this.gbCreatures.TabIndex = 31;
        	this.gbCreatures.TabStop = false;
        	this.gbCreatures.Text = "MOVE ORDER";
        	this.gbCreatures.TextIB = "iceBlinkGBMedium1";
        	// 
        	// gbCharacters
        	// 
        	this.gbCharacters.BackColor = System.Drawing.Color.Transparent;
        	this.gbCharacters.BackgroundColor = System.Drawing.Color.LightSlateGray;
        	this.gbCharacters.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        	this.gbCharacters.BorderThickness = 2F;
        	this.gbCharacters.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.gbCharacters.HeaderForeColor = System.Drawing.Color.White;
        	this.gbCharacters.HeaderImage = ((System.Drawing.Image)(resources.GetObject("gbCharacters.HeaderImage")));
        	this.gbCharacters.HeaderShadowColor = System.Drawing.Color.Black;
        	this.gbCharacters.Location = new System.Drawing.Point(7, 34);
        	this.gbCharacters.Name = "gbCharacters";
        	this.gbCharacters.Size = new System.Drawing.Size(236, 245);
        	this.gbCharacters.TabIndex = 32;
        	this.gbCharacters.TabStop = false;
        	this.gbCharacters.Text = "CHARACTERS";
        	this.gbCharacters.TextIB = "iceBlinkGBMedium1";
        	// 
        	// gbPlayerTurn
        	// 
        	this.gbPlayerTurn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        	this.gbPlayerTurn.BackColor = System.Drawing.Color.Transparent;
        	this.gbPlayerTurn.BackgroundColor = System.Drawing.Color.LightSlateGray;
        	this.gbPlayerTurn.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        	this.gbPlayerTurn.BorderThickness = 2F;
        	this.gbPlayerTurn.Controls.Add(this.btnUseTrait);
        	this.gbPlayerTurn.Controls.Add(this.btnUseSkill);
        	this.gbPlayerTurn.Controls.Add(this.btnRangedAttack);
        	this.gbPlayerTurn.Controls.Add(this.btnUseSpell);
        	this.gbPlayerTurn.Controls.Add(this.btnUseItem);
        	this.gbPlayerTurn.Controls.Add(this.btnDelayTurn);
        	this.gbPlayerTurn.Enabled = false;
        	this.gbPlayerTurn.HeaderForeColor = System.Drawing.Color.White;
        	this.gbPlayerTurn.HeaderImage = ((System.Drawing.Image)(resources.GetObject("gbPlayerTurn.HeaderImage")));
        	this.gbPlayerTurn.HeaderShadowColor = System.Drawing.Color.Black;
        	this.gbPlayerTurn.Location = new System.Drawing.Point(1017, 492);
        	this.gbPlayerTurn.Name = "gbPlayerTurn";
        	this.gbPlayerTurn.Size = new System.Drawing.Size(265, 96);
        	this.gbPlayerTurn.TabIndex = 33;
        	this.gbPlayerTurn.TabStop = false;
        	this.gbPlayerTurn.Text = "PLAYER\'S TURN";
        	this.gbPlayerTurn.TextIB = "iceBlinkGBMedium1";
        	// 
        	// btnUseTrait
        	// 
        	this.btnUseTrait.BackColor = System.Drawing.Color.Transparent;
        	this.btnUseTrait.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnUseTrait.BackgroundImage")));
        	this.btnUseTrait.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        	this.btnUseTrait.DisabledImage = null;
        	this.btnUseTrait.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnUseTrait.FlatAppearance.BorderSize = 0;
        	this.btnUseTrait.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnUseTrait.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnUseTrait.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.btnUseTrait.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.btnUseTrait.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnUseTrait.HoverImage")));
        	this.btnUseTrait.Location = new System.Drawing.Point(177, 31);
        	this.btnUseTrait.Name = "btnUseTrait";
        	this.btnUseTrait.NormalImage = ((System.Drawing.Image)(resources.GetObject("btnUseTrait.NormalImage")));
        	this.btnUseTrait.PressedImage = ((System.Drawing.Image)(resources.GetObject("btnUseTrait.PressedImage")));
        	this.btnUseTrait.Size = new System.Drawing.Size(80, 28);
        	this.btnUseTrait.TabIndex = 28;
        	this.btnUseTrait.TextIB = "Use [T]rait";
        	this.btnUseTrait.UseVisualStyleBackColor = true;
        	this.btnUseTrait.Click += new System.EventHandler(this.btnUseTrait_Click);
        	// 
        	// btnUseSkill
        	// 
        	this.btnUseSkill.BackColor = System.Drawing.Color.Transparent;
        	this.btnUseSkill.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnUseSkill.BackgroundImage")));
        	this.btnUseSkill.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        	this.btnUseSkill.DisabledImage = null;
        	this.btnUseSkill.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnUseSkill.FlatAppearance.BorderSize = 0;
        	this.btnUseSkill.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnUseSkill.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnUseSkill.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.btnUseSkill.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.btnUseSkill.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnUseSkill.HoverImage")));
        	this.btnUseSkill.Location = new System.Drawing.Point(176, 61);
        	this.btnUseSkill.Name = "btnUseSkill";
        	this.btnUseSkill.NormalImage = ((System.Drawing.Image)(resources.GetObject("btnUseSkill.NormalImage")));
        	this.btnUseSkill.PressedImage = ((System.Drawing.Image)(resources.GetObject("btnUseSkill.PressedImage")));
        	this.btnUseSkill.Size = new System.Drawing.Size(80, 28);
        	this.btnUseSkill.TabIndex = 27;
        	this.btnUseSkill.TextIB = "Use S[k]ill";
        	this.btnUseSkill.UseVisualStyleBackColor = true;
        	this.btnUseSkill.Click += new System.EventHandler(this.btnUseSkill_Click);
        	// 
        	// btnRangedAttack
        	// 
        	this.btnRangedAttack.BackColor = System.Drawing.Color.Transparent;
        	this.btnRangedAttack.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnRangedAttack.BackgroundImage")));
        	this.btnRangedAttack.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        	this.btnRangedAttack.DisabledImage = null;
        	this.btnRangedAttack.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnRangedAttack.FlatAppearance.BorderSize = 0;
        	this.btnRangedAttack.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnRangedAttack.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnRangedAttack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.btnRangedAttack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.btnRangedAttack.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnRangedAttack.HoverImage")));
        	this.btnRangedAttack.Location = new System.Drawing.Point(6, 61);
        	this.btnRangedAttack.Name = "btnRangedAttack";
        	this.btnRangedAttack.NormalImage = ((System.Drawing.Image)(resources.GetObject("btnRangedAttack.NormalImage")));
        	this.btnRangedAttack.PressedImage = ((System.Drawing.Image)(resources.GetObject("btnRangedAttack.PressedImage")));
        	this.btnRangedAttack.Size = new System.Drawing.Size(80, 28);
        	this.btnRangedAttack.TabIndex = 26;
        	this.btnRangedAttack.TextIB = "[R]anged Att";
        	this.btnRangedAttack.UseVisualStyleBackColor = true;
        	this.btnRangedAttack.Click += new System.EventHandler(this.btnRangedAttack_Click);
        	// 
        	// btnUseSpell
        	// 
        	this.btnUseSpell.BackColor = System.Drawing.Color.Transparent;
        	this.btnUseSpell.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnUseSpell.BackgroundImage")));
        	this.btnUseSpell.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        	this.btnUseSpell.DisabledImage = null;
        	this.btnUseSpell.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnUseSpell.FlatAppearance.BorderSize = 0;
        	this.btnUseSpell.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnUseSpell.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnUseSpell.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.btnUseSpell.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.btnUseSpell.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnUseSpell.HoverImage")));
        	this.btnUseSpell.Location = new System.Drawing.Point(91, 61);
        	this.btnUseSpell.Name = "btnUseSpell";
        	this.btnUseSpell.NormalImage = ((System.Drawing.Image)(resources.GetObject("btnUseSpell.NormalImage")));
        	this.btnUseSpell.PressedImage = ((System.Drawing.Image)(resources.GetObject("btnUseSpell.PressedImage")));
        	this.btnUseSpell.Size = new System.Drawing.Size(80, 28);
        	this.btnUseSpell.TabIndex = 25;
        	this.btnUseSpell.TextIB = "Use [S]pell";
        	this.btnUseSpell.UseVisualStyleBackColor = true;
        	this.btnUseSpell.Click += new System.EventHandler(this.btnUseSpell_Click);
        	// 
        	// btnUseItem
        	// 
        	this.btnUseItem.BackColor = System.Drawing.Color.Transparent;
        	this.btnUseItem.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnUseItem.BackgroundImage")));
        	this.btnUseItem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        	this.btnUseItem.DisabledImage = null;
        	this.btnUseItem.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnUseItem.FlatAppearance.BorderSize = 0;
        	this.btnUseItem.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnUseItem.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnUseItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.btnUseItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.btnUseItem.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnUseItem.HoverImage")));
        	this.btnUseItem.Location = new System.Drawing.Point(91, 31);
        	this.btnUseItem.Name = "btnUseItem";
        	this.btnUseItem.NormalImage = ((System.Drawing.Image)(resources.GetObject("btnUseItem.NormalImage")));
        	this.btnUseItem.PressedImage = ((System.Drawing.Image)(resources.GetObject("btnUseItem.PressedImage")));
        	this.btnUseItem.Size = new System.Drawing.Size(80, 28);
        	this.btnUseItem.TabIndex = 24;
        	this.btnUseItem.TextIB = "[U]se Item";
        	this.btnUseItem.UseVisualStyleBackColor = true;
        	this.btnUseItem.Click += new System.EventHandler(this.btnUseItem_Click);
        	// 
        	// btnDelayTurn
        	// 
        	this.btnDelayTurn.BackColor = System.Drawing.Color.Transparent;
        	this.btnDelayTurn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnDelayTurn.BackgroundImage")));
        	this.btnDelayTurn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        	this.btnDelayTurn.DisabledImage = null;
        	this.btnDelayTurn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnDelayTurn.FlatAppearance.BorderSize = 0;
        	this.btnDelayTurn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnDelayTurn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        	this.btnDelayTurn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.btnDelayTurn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.btnDelayTurn.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnDelayTurn.HoverImage")));
        	this.btnDelayTurn.Location = new System.Drawing.Point(6, 31);
        	this.btnDelayTurn.Name = "btnDelayTurn";
        	this.btnDelayTurn.NormalImage = ((System.Drawing.Image)(resources.GetObject("btnDelayTurn.NormalImage")));
        	this.btnDelayTurn.PressedImage = ((System.Drawing.Image)(resources.GetObject("btnDelayTurn.PressedImage")));
        	this.btnDelayTurn.Size = new System.Drawing.Size(80, 28);
        	this.btnDelayTurn.TabIndex = 23;
        	this.btnDelayTurn.TextIB = "[E]nd Turn";
        	this.btnDelayTurn.UseVisualStyleBackColor = true;
        	this.btnDelayTurn.Click += new System.EventHandler(this.btnDelayTurn_Click);
        	// 
        	// rtxtInfo
        	// 
        	this.rtxtInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        	this.rtxtInfo.BackColor = System.Drawing.SystemColors.ControlDarkDark;
        	this.rtxtInfo.Cursor = System.Windows.Forms.Cursors.Default;
        	this.rtxtInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.rtxtInfo.Location = new System.Drawing.Point(1017, 381);
        	this.rtxtInfo.Name = "rtxtInfo";
        	this.rtxtInfo.ReadOnly = true;
        	this.rtxtInfo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
        	this.rtxtInfo.Size = new System.Drawing.Size(265, 105);
        	this.rtxtInfo.TabIndex = 34;
        	this.rtxtInfo.Text = "";
        	// 
        	// panel1
        	// 
        	this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Left) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.panel1.AutoScroll = true;
        	this.panel1.BackColor = System.Drawing.Color.Black;
        	this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        	this.panel1.Controls.Add(this.combatRenderPanel);
        	this.panel1.Location = new System.Drawing.Point(250, 34);
        	this.panel1.Name = "panel1";
        	this.panel1.Size = new System.Drawing.Size(761, 672);
        	this.panel1.TabIndex = 35;
        	// 
        	// combatRenderPanel
        	// 
        	this.combatRenderPanel.Location = new System.Drawing.Point(0, 0);
        	this.combatRenderPanel.Name = "combatRenderPanel";
        	this.combatRenderPanel.Size = new System.Drawing.Size(830, 800);
        	this.combatRenderPanel.TabIndex = 2;
        	this.combatRenderPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.combatRenderPanel_MouseDown);
        	this.combatRenderPanel.MouseLeave += new System.EventHandler(this.CombatRenderPanelMouseLeave);
        	this.combatRenderPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.combatRenderPanel_MouseMove);
        	// 
        	// chkGrid
        	// 
        	this.chkGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.chkGrid.Appearance = System.Windows.Forms.Appearance.Button;
        	this.chkGrid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        	this.chkGrid.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
        	this.chkGrid.Checked = true;
        	this.chkGrid.CheckState = System.Windows.Forms.CheckState.Checked;
        	this.chkGrid.FlatAppearance.BorderColor = System.Drawing.Color.Black;
        	this.chkGrid.FlatAppearance.CheckedBackColor = System.Drawing.Color.Silver;
        	this.chkGrid.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
        	this.chkGrid.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
        	this.chkGrid.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.chkGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.chkGrid.Location = new System.Drawing.Point(152, 677);
        	this.chkGrid.Margin = new System.Windows.Forms.Padding(0);
        	this.chkGrid.Name = "chkGrid";
        	this.chkGrid.Size = new System.Drawing.Size(90, 25);
        	this.chkGrid.TabIndex = 36;
        	this.chkGrid.Text = "[G]rid";
        	this.chkGrid.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        	this.chkGrid.UseVisualStyleBackColor = false;
        	this.chkGrid.CheckedChanged += new System.EventHandler(this.chkGrid_CheckedChanged);
        	// 
        	// numAnimationDelay
        	// 
        	this.numAnimationDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.numAnimationDelay.Location = new System.Drawing.Point(27, 691);
        	this.numAnimationDelay.Maximum = new decimal(new int[] {
        	        	        	30,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numAnimationDelay.Minimum = new decimal(new int[] {
        	        	        	1,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numAnimationDelay.Name = "numAnimationDelay";
        	this.numAnimationDelay.Size = new System.Drawing.Size(36, 20);
        	this.numAnimationDelay.TabIndex = 37;
        	this.numAnimationDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
        	this.numAnimationDelay.Value = new decimal(new int[] {
        	        	        	1,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numAnimationDelay.Visible = false;
        	this.numAnimationDelay.ValueChanged += new System.EventHandler(this.numAnimationDelay_ValueChanged);
        	// 
        	// label6
        	// 
        	this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.label6.AutoSize = true;
        	this.label6.BackColor = System.Drawing.SystemColors.ControlDarkDark;
        	this.label6.Location = new System.Drawing.Point(11, 679);
        	this.label6.Name = "label6";
        	this.label6.Size = new System.Drawing.Size(59, 26);
        	this.label6.TabIndex = 38;
        	this.label6.Text = "Animation\r\nDelay (ms):";
        	this.label6.Visible = false;
        	// 
        	// chkFacing
        	// 
        	this.chkFacing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.chkFacing.Appearance = System.Windows.Forms.Appearance.Button;
        	this.chkFacing.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        	this.chkFacing.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
        	this.chkFacing.FlatAppearance.BorderColor = System.Drawing.Color.Black;
        	this.chkFacing.FlatAppearance.CheckedBackColor = System.Drawing.Color.Silver;
        	this.chkFacing.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
        	this.chkFacing.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
        	this.chkFacing.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.chkFacing.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.chkFacing.Location = new System.Drawing.Point(152, 649);
        	this.chkFacing.Margin = new System.Windows.Forms.Padding(0);
        	this.chkFacing.Name = "chkFacing";
        	this.chkFacing.Size = new System.Drawing.Size(90, 25);
        	this.chkFacing.TabIndex = 39;
        	this.chkFacing.Text = "[F]acing";
        	this.chkFacing.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        	this.chkFacing.UseVisualStyleBackColor = false;
        	this.chkFacing.CheckedChanged += new System.EventHandler(this.chkFacing_CheckedChanged);
        	// 
        	// chkShowRange
        	// 
        	this.chkShowRange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.chkShowRange.Appearance = System.Windows.Forms.Appearance.Button;
        	this.chkShowRange.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        	this.chkShowRange.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
        	this.chkShowRange.FlatAppearance.BorderColor = System.Drawing.Color.Black;
        	this.chkShowRange.FlatAppearance.CheckedBackColor = System.Drawing.Color.Silver;
        	this.chkShowRange.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
        	this.chkShowRange.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
        	this.chkShowRange.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.chkShowRange.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.chkShowRange.Location = new System.Drawing.Point(152, 621);
        	this.chkShowRange.Margin = new System.Windows.Forms.Padding(0);
        	this.chkShowRange.Name = "chkShowRange";
        	this.chkShowRange.Size = new System.Drawing.Size(90, 25);
        	this.chkShowRange.TabIndex = 40;
        	this.chkShowRange.Text = "[V]iew Range";
        	this.chkShowRange.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        	this.chkShowRange.UseVisualStyleBackColor = false;
        	this.chkShowRange.CheckedChanged += new System.EventHandler(this.chkShowRange_CheckedChanged);
        	// 
        	// combatTimer
        	// 
        	this.combatTimer.Interval = 16;
        	this.combatTimer.Tick += new System.EventHandler(this.combatTimer_Tick);
        	// 
        	// gbMovesLeft
        	// 
        	this.gbMovesLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.gbMovesLeft.BackColor = System.Drawing.Color.Transparent;
        	this.gbMovesLeft.BackgroundColor = System.Drawing.Color.LightSlateGray;
        	this.gbMovesLeft.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        	this.gbMovesLeft.BorderThickness = 2F;
        	this.gbMovesLeft.Controls.Add(this.lblMovesLeft);
        	this.gbMovesLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.gbMovesLeft.HeaderForeColor = System.Drawing.Color.White;
        	this.gbMovesLeft.HeaderImage = ((System.Drawing.Image)(resources.GetObject("gbMovesLeft.HeaderImage")));
        	this.gbMovesLeft.HeaderShadowColor = System.Drawing.Color.Black;
        	this.gbMovesLeft.Location = new System.Drawing.Point(59, 621);
        	this.gbMovesLeft.Name = "gbMovesLeft";
        	this.gbMovesLeft.Size = new System.Drawing.Size(85, 82);
        	this.gbMovesLeft.TabIndex = 41;
        	this.gbMovesLeft.TabStop = false;
        	this.gbMovesLeft.Text = "MOVES LEFT";
        	this.gbMovesLeft.TextIB = "iceBlinkGBMedium1";
        	// 
        	// lblMovesLeft
        	// 
        	this.lblMovesLeft.BackColor = System.Drawing.Color.Transparent;
        	this.lblMovesLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.lblMovesLeft.Location = new System.Drawing.Point(6, 28);
        	this.lblMovesLeft.Name = "lblMovesLeft";
        	this.lblMovesLeft.Size = new System.Drawing.Size(73, 49);
        	this.lblMovesLeft.TabIndex = 30;
        	this.lblMovesLeft.Text = "5";
        	this.lblMovesLeft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        	// 
        	// chkHoverOnly
        	// 
        	this.chkHoverOnly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.chkHoverOnly.Appearance = System.Windows.Forms.Appearance.Button;
        	this.chkHoverOnly.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        	this.chkHoverOnly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
        	this.chkHoverOnly.FlatAppearance.BorderColor = System.Drawing.Color.Black;
        	this.chkHoverOnly.FlatAppearance.CheckedBackColor = System.Drawing.Color.Silver;
        	this.chkHoverOnly.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
        	this.chkHoverOnly.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
        	this.chkHoverOnly.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.chkHoverOnly.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.chkHoverOnly.Location = new System.Drawing.Point(8, 620);
        	this.chkHoverOnly.Margin = new System.Windows.Forms.Padding(0);
        	this.chkHoverOnly.Name = "chkHoverOnly";
        	this.chkHoverOnly.Size = new System.Drawing.Size(44, 86);
        	this.chkHoverOnly.TabIndex = 42;
        	this.chkHoverOnly.Text = "Hov-\r\ner\r\n\r\nOnly";
        	this.chkHoverOnly.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        	this.chkHoverOnly.UseVisualStyleBackColor = false;
        	// 
        	// scrollTimer
        	// 
        	this.scrollTimer.Interval = 25;
        	this.scrollTimer.Tick += new System.EventHandler(this.ScrollTimerTick);
        	// 
        	// Combat
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.BackColor = System.Drawing.Color.White;
        	this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
        	this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        	this.ClientSize = new System.Drawing.Size(1292, 718);
        	this.Controls.Add(this.chkHoverOnly);
        	this.Controls.Add(this.gbMovesLeft);
        	this.Controls.Add(this.chkShowRange);
        	this.Controls.Add(this.pictureBox1);
        	this.Controls.Add(this.chkFacing);
        	this.Controls.Add(this.label6);
        	this.Controls.Add(this.numAnimationDelay);
        	this.Controls.Add(this.chkGrid);
        	this.Controls.Add(this.panel1);
        	this.Controls.Add(this.rtxtInfo);
        	this.Controls.Add(this.gbPlayerTurn);
        	this.Controls.Add(this.gbCharacters);
        	this.Controls.Add(this.gbCreatures);
        	this.Controls.Add(this.lblMouseInfo);
        	this.Controls.Add(this.txtInfo);
        	this.Controls.Add(this.btnContinue);
        	this.Controls.Add(this.gbPartyAction);
        	this.Controls.Add(this.rtxtLog);
        	this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        	this.MaximizeBox = false;
        	this.MinimizeBox = false;
        	this.MinimumSize = new System.Drawing.Size(1278, 610);
        	this.Name = "Combat";
        	this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        	this.Text = "Combat";
        	this.TopMost = true;
        	this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Combat_FormClosed);
        	this.Controls.SetChildIndex(this.rtxtLog, 0);
        	this.Controls.SetChildIndex(this.gbPartyAction, 0);
        	this.Controls.SetChildIndex(this.btnContinue, 0);
        	this.Controls.SetChildIndex(this.txtInfo, 0);
        	this.Controls.SetChildIndex(this.lblMouseInfo, 0);
        	this.Controls.SetChildIndex(this.gbCreatures, 0);
        	this.Controls.SetChildIndex(this.gbCharacters, 0);
        	this.Controls.SetChildIndex(this.gbPlayerTurn, 0);
        	this.Controls.SetChildIndex(this.rtxtInfo, 0);
        	this.Controls.SetChildIndex(this.panel1, 0);
        	this.Controls.SetChildIndex(this.chkGrid, 0);
        	this.Controls.SetChildIndex(this.numAnimationDelay, 0);
        	this.Controls.SetChildIndex(this.label6, 0);
        	this.Controls.SetChildIndex(this.chkFacing, 0);
        	this.Controls.SetChildIndex(this.pictureBox1, 0);
        	this.Controls.SetChildIndex(this.chkShowRange, 0);
        	this.Controls.SetChildIndex(this.gbMovesLeft, 0);
        	this.Controls.SetChildIndex(this.chkHoverOnly, 0);
        	((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
        	this.gbPartyAction.ResumeLayout(false);
        	this.gbPlayerTurn.ResumeLayout(false);
        	this.panel1.ResumeLayout(false);
        	((System.ComponentModel.ISupportInitialize)(this.numAnimationDelay)).EndInit();
        	this.gbMovesLeft.ResumeLayout(false);
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }
        private System.Windows.Forms.Timer scrollTimer;

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RichTextBox rtxtLog;
        private IceBlinkGroupBoxMedium gbPartyAction;
        private IceBlinkButtonMedium btnRunAway;
        private IceBlinkButtonMedium btnStayFight;
        private IceBlinkButtonLarge btnContinue;
        private System.Windows.Forms.Label lblMouseInfo;
        private System.Windows.Forms.Label txtInfo;
        private IceBlinkGroupBoxMedium gbCreatures;
        private IceBlinkGroupBoxMedium gbCharacters;
        private IceBlinkGroupBoxMedium gbPlayerTurn;
        private IceBlinkButtonMedium btnRangedAttack;
        private IceBlinkButtonMedium btnUseSpell;
        private IceBlinkButtonMedium btnUseItem;
        private IceBlinkButtonMedium btnDelayTurn;
        private IceBlinkButtonMedium btnUseTrait;
        private IceBlinkButtonMedium btnUseSkill;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chkGrid;
        private System.Windows.Forms.NumericUpDown numAnimationDelay;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkFacing;
        private System.Windows.Forms.CheckBox chkShowRange;
        public System.Windows.Forms.Timer combatTimer;
        private System.Windows.Forms.Panel combatRenderPanel;
        public System.Windows.Forms.RichTextBox rtxtInfo;
        private IceBlinkGroupBoxMedium gbMovesLeft;
        private System.Windows.Forms.Label lblMovesLeft;
        private System.Windows.Forms.CheckBox chkHoverOnly;
    }
    
}
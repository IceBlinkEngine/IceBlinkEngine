namespace IceBlink
{
    partial class SpellSelect
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
        	System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpellSelect));
        	this.gbSpellList = new IceBlink.IceBlinkGroupBoxMedium();
        	this.gbSpellDesc = new IceBlink.IceBlinkGroupBoxMedium();
        	this.rtxtSpellDescription = new System.Windows.Forms.RichTextBox();
        	this.gbSpellDesc.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// gbSpellList
        	// 
        	this.gbSpellList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Left)));
        	this.gbSpellList.BackColor = System.Drawing.Color.Transparent;
        	this.gbSpellList.BackgroundColor = System.Drawing.Color.LightSlateGray;
        	this.gbSpellList.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        	this.gbSpellList.BorderThickness = 2F;
        	this.gbSpellList.HeaderForeColor = System.Drawing.Color.White;
        	this.gbSpellList.HeaderImage = ((System.Drawing.Image)(resources.GetObject("gbSpellList.HeaderImage")));
        	this.gbSpellList.HeaderShadowColor = System.Drawing.Color.Black;
        	this.gbSpellList.Location = new System.Drawing.Point(12, 36);
        	this.gbSpellList.Name = "gbSpellList";
        	this.gbSpellList.Size = new System.Drawing.Size(213, 270);
        	this.gbSpellList.TabIndex = 7;
        	this.gbSpellList.TabStop = false;
        	this.gbSpellList.Text = "Known Spells";
        	this.gbSpellList.TextIB = "iceBlinkGBMedium1";
        	// 
        	// gbSpellDesc
        	// 
        	this.gbSpellDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.gbSpellDesc.BackColor = System.Drawing.Color.Transparent;
        	this.gbSpellDesc.BackgroundColor = System.Drawing.Color.LightSlateGray;
        	this.gbSpellDesc.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        	this.gbSpellDesc.BorderThickness = 2F;
        	this.gbSpellDesc.Controls.Add(this.rtxtSpellDescription);
        	this.gbSpellDesc.HeaderForeColor = System.Drawing.Color.White;
        	this.gbSpellDesc.HeaderImage = ((System.Drawing.Image)(resources.GetObject("gbSpellDesc.HeaderImage")));
        	this.gbSpellDesc.HeaderShadowColor = System.Drawing.Color.Black;
        	this.gbSpellDesc.Location = new System.Drawing.Point(231, 36);
        	this.gbSpellDesc.Name = "gbSpellDesc";
        	this.gbSpellDesc.Size = new System.Drawing.Size(215, 270);
        	this.gbSpellDesc.TabIndex = 6;
        	this.gbSpellDesc.TabStop = false;
        	this.gbSpellDesc.Text = "Spell Description";
        	this.gbSpellDesc.TextIB = "iceBlinkGBMedium1";
        	// 
        	// rtxtSpellDescription
        	// 
        	this.rtxtSpellDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Left) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.rtxtSpellDescription.Location = new System.Drawing.Point(6, 30);
        	this.rtxtSpellDescription.Name = "rtxtSpellDescription";
        	this.rtxtSpellDescription.Size = new System.Drawing.Size(203, 234);
        	this.rtxtSpellDescription.TabIndex = 1;
        	this.rtxtSpellDescription.Text = "";
        	// 
        	// SpellSelect
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.BackColor = System.Drawing.SystemColors.AppWorkspace;
        	this.ClientSize = new System.Drawing.Size(458, 319);
        	this.Controls.Add(this.gbSpellList);
        	this.Controls.Add(this.gbSpellDesc);
        	this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        	this.MaximizeBox = false;
        	this.MaximumSize = new System.Drawing.Size(458, 600);
        	this.MinimizeBox = false;
        	this.MinimumSize = new System.Drawing.Size(458, 319);
        	this.Name = "SpellSelect";
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        	this.Text = "Spell Select";
        	this.TopMost = true;
        	this.Controls.SetChildIndex(this.gbSpellDesc, 0);
        	this.Controls.SetChildIndex(this.gbSpellList, 0);
        	this.gbSpellDesc.ResumeLayout(false);
        	this.ResumeLayout(false);
        }

        #endregion

        private IceBlinkGroupBoxMedium gbSpellList;
        private IceBlinkGroupBoxMedium gbSpellDesc;
        private System.Windows.Forms.RichTextBox rtxtSpellDescription;
    }
}
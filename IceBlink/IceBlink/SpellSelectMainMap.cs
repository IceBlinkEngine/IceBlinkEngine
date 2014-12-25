using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IceBlinkCore;

namespace IceBlink
{
    public partial class SpellSelectMainMap : IBForm
    {
        private Form1 frm;
        private Game game;
        private PC pc;

        public SpellSelectMainMap(Form1 f, Game g, PC p)
        {
            InitializeComponent();
            frm = f;
            game = g;
            pc = p;
            IceBlinkButtonResize.setupAll(game);
            IceBlinkButtonResize.Enabled = false;
            IceBlinkButtonResize.Visible = false;
            IceBlinkButtonClose.setupAll(game);
            gbSpellDesc.setupAll(game);
            gbSpellList.setupAll(game);
            gbPcSelect.setupAll(game);
            gbPcSelect.Visible = false;
            btnSelectMapPoint.setupAll(game);
            IceBlinkButtonResize.Enabled = false;
            this.setupAll(game);
            setupLabels();
            refreshFonts();
            setFormSize();
            createButtons();
            refreshPcPortraits();
        }
        public void refreshFonts()
        {
            gbSpellList.Font = game.module.ModuleTheme.ModuleFont;
            gbSpellDesc.Font = game.module.ModuleTheme.ModuleFont;
            rtxtSpellDescription.Font = game.module.ModuleTheme.ModuleFont;
            this.Invalidate();
        }
        private void setupLabels()
        {
            gbSpellList.Text = "Known " + game.module.LabelSpells;
            gbSpellDesc.Text = game.module.LabelSpells + " Description";
        }
        private void setFormSize()
        {
            if (pc.KnownSpellsList.spellList.Count < 8)
            {
                this.Height = 320;
            }
            else if (pc.KnownSpellsList.spellList.Count < 12)
            {
                this.Height = 400;
            }
            else if (pc.KnownSpellsList.spellList.Count < 16)
            {
                this.Height = 480;
            }
            else
            {
                this.Height = 560;
            }
        }
        private void createButtons()
        {
            int index = 0;
            int row = 0;
            // * sinopip, 20.12.14
            this.Width += 50;
            Panel p = new Panel();
			p.Size = this.gbSpellList.Size;
			p.Location = new System.Drawing.Point(0, 30);
            p.Width += 10;
			p.Height -= 30;
			this.gbSpellList.Width += 10;
			this.gbSpellDesc.Width += 10;
			this.gbSpellList.Left -= 10;
			//this.gbSpellDesc.Left += 10;
            p.BackColor = this.gbSpellList.BackColor;
            //            
            foreach (Spell sp in pc.KnownSpellsList.spellList)
            {
                if ((sp.UseableInSituation == UsableInSituation.Always) || (sp.UseableInSituation == UsableInSituation.OutOfCombat))
                {
                    IceBlinkButtonMedium btnNew = new IceBlink.IceBlinkButtonMedium();
                    if (index % 2 == 0) //even...place on left
                    {
                    	btnNew.Location = new System.Drawing.Point(10, row * 45);// + 30);
                    }
                    else //odd...place on right
                    {
                    	btnNew.Location = new System.Drawing.Point(110, row * 45);// + 30);
                        row++;
                    }
                    btnNew.Size = new System.Drawing.Size(93, 40);
                    btnNew.TextIB = sp.SpellName.ToUpper();
	                // * sinopip, 20.12.14
	                if (sp.SpellEffectType.ToString() == "Damage")
	                	btnNew.Image = Image.FromFile("data\\ui\\rest.png");
	                //
                    btnNew.Name = sp.SpellTag;
                    btnNew.Click += new System.EventHandler(this.btnSelectedSpell_Click);
                    btnNew.MouseEnter += new EventHandler(this.btnSelectedSpell_Enter);
                    btnNew.setupAll(game);
	                // * sinopip, 20.12.14
	                //this.gbSpellList.Controls.Add(btnNew);
	                p.Controls.Add(btnNew);
	                //
                    index++;
                }
            }
            // * sinopip, 20.12.14
            p.AutoScroll = true;
            p.HorizontalScroll.Visible = false;
            this.gbSpellList.Controls.Add(p);
            //
        }
        private void btnSelectedSpell_Click(object sender, EventArgs e)
        {
            IceBlinkButtonMedium selectBtn = (IceBlinkButtonMedium)sender;
            Spell selectedSpell = pc.KnownSpellsList.getSpellByTag(selectBtn.Name);
            int cost = selectedSpell.CostSP;
            // check to see if enough SP
            if (pc.SP >= cost) // if enough SP, then currentTS equals selected and close
            {
                frm.currentSpell = selectedSpell;
                gbSpellDesc.Visible = false;
                gbSpellList.Visible = false;
                gbPcSelect.Visible = true;
                if (selectedSpell.TargetIsSelf)
                {
                    frm.sf.MainMapSource = pc;
                    frm.sf.MainMapTarget = pc;
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                //this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            else // else message with not enough
            {
                IBMessageBox.Show(game, "You do not have enough SP at this time");
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            }
        }
        private void btnSelectedSpell_Enter(object sender, EventArgs e)
        {
            IceBlinkButtonMedium selectBtn = (IceBlinkButtonMedium)sender;
            Spell selectedSpell = pc.KnownSpellsList.getSpellByTag(selectBtn.Name);
            rtxtSpellDescription.Text = game.module.LabelSpells + " Name: " + selectedSpell.SpellName + Environment.NewLine +
                                        "SP Cost: " + selectedSpell.CostSP + Environment.NewLine +
                                        "Target Range: " + selectedSpell.Range + Environment.NewLine +
                                        "Area of Effect (square radius): " + selectedSpell.AoeRadiusOrLength + Environment.NewLine +
                                        Environment.NewLine +
                                        "Description: " + Environment.NewLine +
                                        selectedSpell.Description;
        }
        public void refreshPcPortraits()
        {
            try
            {
                if (game.playerList.PCList.Count >= 1)
                {
                    this.rbtnPc0.Visible = true;
                }
                if (game.playerList.PCList.Count >= 2)
                {
                    this.rbtnPc1.Visible = true;
                }
                if (game.playerList.PCList.Count >= 3)
                {
                    this.rbtnPc2.Visible = true;
                }
                if (game.playerList.PCList.Count >= 4)
                {
                    this.rbtnPc3.Visible = true;
                }
                if (game.playerList.PCList.Count >= 5)
                {
                    this.rbtnPc4.Visible = true;
                }
                if (game.playerList.PCList.Count >= 6)
                {
                    this.rbtnPc5.Visible = true;
                }

                if (game.playerList.PCList.Count > 0)
                {
                    rbtnPc0.BackgroundImage = (Image)game.playerList.PCList[0].portraitBitmapS;
                    this.rbtnPc0.Enabled = true;
                }
                if (game.playerList.PCList.Count > 1)
                {
                    rbtnPc1.BackgroundImage = (Image)game.playerList.PCList[1].portraitBitmapS;
                    this.rbtnPc1.Enabled = true;
                }
                if (game.playerList.PCList.Count > 2)
                {
                    rbtnPc2.BackgroundImage = (Image)game.playerList.PCList[2].portraitBitmapS;
                    this.rbtnPc2.Enabled = true;
                }
                if (game.playerList.PCList.Count > 3)
                {
                    rbtnPc3.BackgroundImage = (Image)game.playerList.PCList[3].portraitBitmapS;
                    this.rbtnPc3.Enabled = true;
                }
                if (game.playerList.PCList.Count > 4)
                {
                    rbtnPc4.BackgroundImage = (Image)game.playerList.PCList[4].portraitBitmapS;
                    this.rbtnPc4.Enabled = true;
                }
                if (game.playerList.PCList.Count > 5)
                {
                    rbtnPc5.BackgroundImage = (Image)game.playerList.PCList[5].portraitBitmapS;
                    this.rbtnPc5.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                game.errorLog(ex.ToString());
            }
        }
        private void rbtnPc0_CheckedChanged(object sender, EventArgs e)
        {
            frm.sf.MainMapSource = pc;
            frm.sf.MainMapTarget = game.playerList.PCList[0];
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
        private void rbtnPc1_CheckedChanged(object sender, EventArgs e)
        {
            frm.sf.MainMapSource = pc;
            frm.sf.MainMapTarget = game.playerList.PCList[1];
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
        private void rbtnPc2_CheckedChanged(object sender, EventArgs e)
        {
            frm.sf.MainMapSource = pc;
            frm.sf.MainMapTarget = game.playerList.PCList[2];
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
        private void rbtnPc3_CheckedChanged(object sender, EventArgs e)
        {
            frm.sf.MainMapSource = pc;
            frm.sf.MainMapTarget = game.playerList.PCList[3];
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
        private void rbtnPc4_CheckedChanged(object sender, EventArgs e)
        {
            frm.sf.MainMapSource = pc;
            frm.sf.MainMapTarget = game.playerList.PCList[4];
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
        private void rbtnPc5_CheckedChanged(object sender, EventArgs e)
        {
            frm.sf.MainMapSource = pc;
            frm.sf.MainMapTarget = game.playerList.PCList[5];
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
        private void btnSelectMapPoint_Click(object sender, EventArgs e)
        {

        }
    }
}

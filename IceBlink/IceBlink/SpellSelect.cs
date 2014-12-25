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
    public partial class SpellSelect : IBForm
    {
        private Combat msc_combat;
        private Game msc_game;
        private PC msc_pc;

        public SpellSelect(Combat c, Game g, PC pc)
        {
            InitializeComponent();
            msc_combat = c;
            msc_game = g;
            msc_pc = pc;
            IceBlinkButtonResize.setupAll(msc_game);
            IceBlinkButtonResize.Enabled = false;
            IceBlinkButtonResize.Visible = false;
            IceBlinkButtonClose.setupAll(msc_game);
            gbSpellDesc.setupAll(msc_game);
            gbSpellList.setupAll(msc_game);
            this.setupAll(msc_game);
            setupLabels();
            refreshFonts();
            setFormSize();
            createButtons();
        }
        public void refreshFonts()
        {
            gbSpellList.Font = msc_game.module.ModuleTheme.ModuleFont;
            gbSpellDesc.Font = msc_game.module.ModuleTheme.ModuleFont;
            rtxtSpellDescription.Font = msc_game.module.ModuleTheme.ModuleFont;
            this.Invalidate();
        }
        private void setupLabels()
        {
            gbSpellList.Text = "Known " + msc_game.module.LabelSpells;
            gbSpellDesc.Text = msc_game.module.LabelSpells + " Description";
        }
        private void setFormSize()
        {
            if (msc_pc.KnownSpellsList.spellList.Count < 8)
            {
                this.Height = 320;
            }
            else if (msc_pc.KnownSpellsList.spellList.Count < 12)
            {
                this.Height = 400;
            }
            else if (msc_pc.KnownSpellsList.spellList.Count < 16)
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
            // * sinopip, 19.12.14
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
            foreach (Spell sp in msc_pc.KnownSpellsList.spellList)
            {
                IceBlinkButtonMedium btnNew = new IceBlink.IceBlinkButtonMedium();
                if (index % 2 == 0) //even...place on left
                {
                	btnNew.Location = new System.Drawing.Point(10, row * 45);// + 30);
                }
                else //odd...place on right
                {
                	btnNew.Location = new System.Drawing.Point(103, row * 45);// + 30);
                    row++;
                }
                btnNew.Size = new System.Drawing.Size(93, 40);
                btnNew.TextIB = sp.SpellName.ToUpper();
                // * sinopip, 19.12.14
                if (sp.SpellIcon != "none" && sp.SpellIcon != "")
                try
                {
                	// load icon from directory ; could be "\\icons" in "\\graphics" ?
                	btnNew.Image = Image.FromFile(msc_game.mainDirectory + "\\modules\\" + msc_game.module.ModuleFolderName + "\\graphics\\"+ sp.SpellIcon);
                } catch { }
                else
                	try {
                		btnNew.Image = Image.FromFile("data\\ui\\"+ sp.SpellEffectType.ToString()+".png");
                	} catch { }
                //
                btnNew.Name = sp.SpellTag;
                btnNew.Click += new System.EventHandler(this.btnSelectedSpell_Click);
                btnNew.MouseEnter += new EventHandler(this.btnSelectedSpell_Enter);
                btnNew.setupAll(msc_game);
                // * sinopip, 19.12.14
                //this.gbSpellList.Controls.Add(btnNew);
                p.Controls.Add(btnNew);
                //
                index++;
            }
            // * sinopip, 19.12.14
            p.AutoScroll = true;
            p.HorizontalScroll.Visible = false;
            this.gbSpellList.Controls.Add(p);
            //
        }
        private void btnSelectedSpell_Click(object sender, EventArgs e)
        {
            IceBlinkButtonMedium selectBtn = (IceBlinkButtonMedium)sender;
            Spell selectedSpell = msc_pc.KnownSpellsList.getSpellByTag(selectBtn.Name);
            int cost = selectedSpell.CostSP;
            string strBonus = selectBtn.Name; //JamesManhattan 
            //MyString.Substring(MyString.Length-6);
            // check to see if enough Actions left - and if is a Bonus or not //JamesManhattan
            if (msc_combat.usedAction >= 1)
            {
                //combat.logText(" chose Trait it is " + strBonus, Color.Black); //JamesManhattan
                IBMessageBox.Show(msc_game, "You already used your Action");
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            }
            else
            {
                // check to see if enough SP
                if (msc_pc.SP >= cost) // if enough SP, then currentTS equals selected and close
                {
                    msc_combat.currentSpell = selectedSpell;
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else // else message with not enough
                {
                    IBMessageBox.Show(msc_game, "You do not have enough SP at this time");
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                }
            }
        }
        private void btnSelectedSpell_Enter(object sender, EventArgs e)
        {
            IceBlinkButtonMedium selectBtn = (IceBlinkButtonMedium)sender;
            Spell selectedSpell = msc_pc.KnownSpellsList.getSpellByTag(selectBtn.Name);
            rtxtSpellDescription.Text = msc_game.module.LabelSpells + " Name: " + selectedSpell.SpellName + Environment.NewLine +
                                        "SP Cost: " + selectedSpell.CostSP + Environment.NewLine +
                                        "Target Range: " + selectedSpell.Range + Environment.NewLine +
                                        "Area of Effect (square radius): " + selectedSpell.AoeRadiusOrLength + Environment.NewLine +
                                        Environment.NewLine +
                                        "Description: " + Environment.NewLine +
                                        selectedSpell.Description;
        }
    }
}

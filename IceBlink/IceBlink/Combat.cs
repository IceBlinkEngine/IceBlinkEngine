using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IceBlinkCore;
using System.Threading;
using System.IO;

namespace IceBlink
{
    public partial class Combat : IBForm
    {        
        public Game com_game;
        public Form1 com_frm;
        public Point txtOneLocation = new Point(0, 0);
        public Point txtTwoLocation = new Point(1, 1);
        public int turn = 1;
        public int currentMoves = 0; 
        public int usedAction = 0;  //JamesManhattan added 9/25/14
        public int midAttack = 0;  //JamesManhattan added 9/26/14
        public int startedMoving = 0; //JamesManhattan added 9/28/14
        public int usedSwiftBonusAction = 0;  //JamesManhattan added 9/25/14
        public int numAttacks = 1;  //JamesManhattan added 9/25/14 the default is 1
        public int numBonusAttacks = 0;  //JamesManhattan added 9/26/14 the default is 0
        public int numSpellAttacks = 1; //JamesManhattan for multi target spells, such as each bolt of magic missile 
        public int distance = 77;
        public int currentIndex = 0;       
        public int mousex, mousey;
        public int gridx, gridy;
        public Encounter com_encounter = new Encounter();
        public List<MoveOrder> com_moveOrderList = new List<MoveOrder>();
        public List<string> comLogList = new List<string>();
        public bool canMove = false;
        public int bumpedIntoCreatureIndex;
        public int currentMoveOrderIndex = 0;
        public int encounterXP = 0;
        public bool endEncounter = false;
        public Pathfinder _pathfinder;
        public bool rangedItem = false;
        public bool rangedSpell = false;
        public bool rangedTrait = false;
        public bool targetIsPC = false;
        public bool spellTargetSelected = false;
        public int currentSpellRadius = 0;
        public int currentSpellRange = 0;
        public int currentSpellCost = 0;
        public Item.projectileImage currentSpellProjImg = Item.projectileImage.Arrow;
        public bool AoEtype = false;
        public Spell currentSpell = new Spell();
        public Trait currentTrait = new Trait();
        public Skill currentSkill = new Skill();
        public int animationDelay = 10;
        public int combatRoundNumber = 0;
        public bool okToDrawHighlights = true;
        public bool showStepNumbers = false;
        // * sinopip, 20.12.14
		public bool is_upscrolling = false;
		public bool is_downscrolling = false;
		public bool is_leftscrolling = false;
		public bool is_rightscrolling = false;
		//
        

        public Combat(Game game, Form1 frm, Encounter encounter)
        {
            InitializeComponent();
            combatTimer.Enabled = true;
            combatTimer.Stop();
            // * sinopip, 20.12.14
            scrollTimer.Enabled = true;
            scrollTimer.Stop();
            //
            com_game = game;
            com_frm = frm;
            com_encounter = encounter;
            com_encounter.passRefs(com_game, null);
            gbCharacters.setupAll(com_game);
            gbCreatures.setupAll(com_game);
            gbMovesLeft.setupAll(com_game);
            gbPartyAction.setupAll(com_game);
            gbPlayerTurn.setupAll(com_game);
            btnContinue.setupAll(com_game);
            btnDelayTurn.setupAll(com_game);
            btnRangedAttack.setupAll(com_game);
            btnRunAway.setupAll(com_game);
            btnStayFight.setupAll(com_game);
            btnUseItem.setupAll(com_game);
            btnUseSkill.setupAll(com_game);
            btnUseSpell.setupAll(com_game);
            btnUseTrait.setupAll(com_game);            
            IceBlinkButtonResize.setupAll(com_game);
            IceBlinkButtonResize.Enabled = false;
            IceBlinkButtonResize.Visible = false;
            IceBlinkButtonClose.setupAll(com_game);
            IceBlinkButtonClose.Enabled = false;
            IceBlinkButtonClose.Visible = false;
            this.setupAll(com_game);

            //this.WindowState = FormWindowState.Maximized;
            SetupScreenSize();

            com_game.currentEncounter = com_encounter;
            bumpedIntoCreatureIndex = -1;

            Sprite turnSel = new Sprite();
            turnSel.Image = com_game.LoadBitmap(com_game.mainDirectory + "\\data\\graphics\\turnMarker.png");
            com_game.combatTurnSelectionIcon = turnSel;

            // loop through PC start locations and set
            int cnt = 0;
            foreach (Point PCloc in com_encounter.EncounterPcStartLocations)
            {
                int PcIndex = com_game.PartyCombatOrder[cnt];
                if (com_game.playerList.PCList.Count > PcIndex)
                {
                    com_game.playerList.PCList[PcIndex].CombatLocation = PCloc;
                }
                /*if (com_game.playerList.PCList.Count > cnt)
                {
                    com_game.playerList.PCList[cnt].CombatLocation = PCloc;
                }*/
                cnt++;
            }
            
            com_game.module.loadCombatArea(com_game, com_encounter.EncounterLevelFilename);
            com_game.currentCombatMapBitmap = new Bitmap(com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\areas\\" + com_encounter.EncounterMapFilename);
            // initialize graphics
//            this.pictureBox1.Width = com_game.currentCombatMapBitmap.Width;
//            this.pictureBox1.Height = com_game.currentCombatMapBitmap.Height;
            this.combatRenderPanel.Width = com_game.currentCombatMapBitmap.Width;
            this.combatRenderPanel.Height = com_game.currentCombatMapBitmap.Height;
            com_game.currentCombatMapBitmap.Dispose();
//            com_game.initializeCombatGraphics(this.pictureBox1);
            com_game.InitializeCombatRenderPanel(this.combatRenderPanel);
//            com_game.DeviceCombat.DrawImage((Image)com_game.currentCombatMapBitmap, 0, 0);
//            com_game.areaCombatUpdate();
            txtInfo.Text = "";
            lblMovesLeft.Text = "0";
            lblMouseInfo.Text = "";
            currentMoveOrderIndex = 0;
            btnContinue.Enabled = false;
            //chkGrid.Checked = false;
            foreach (Creature crtr in com_encounter.EncounterCreatureList.creatures)
            {
                encounterXP = encounterXP + crtr.RewardXP;
            }
            frm.sf.pathfinder = new Pathfinder(com_game);
            //_pathfinder = new Pathfinder(com_game);
//            drawSprites();
            setupMoveOrder();
            doCreateLabels();
            refreshCharacterPanel();
            refreshFonts();
            numAnimationDelay.Value = animationDelay;
            frm.playCombatAreaMusicSounds();
            combatTimer.Start();
            // * sinopip, 20.12.14
            scrollTimer.Start();
            foreach (Creature crtr in com_encounter.EncounterCreatureList.creatures)
            	com_frm.sf.SetLocalInt(crtr.Tag, "HasDied", 0);
            foreach (PC pc in game.playerList.PCList)
            	if (pc.HP > 0)
            		com_frm.sf.SetLocalInt(pc.Tag, "HasDied", 0);
            	else
            		com_frm.sf.SetLocalInt(pc.Tag, "HasDied", 1);
            //
        }
        private void SetupScreenSize()
        {
            if (com_frm.windowSize == 1)
            {
                this.Width = 1000;
                this.Height = 600;
                this.WindowState = FormWindowState.Normal;
            }
            else if (com_frm.windowSize == 2)
            {
                this.Width = 1215;
                this.Height = 700;
                this.WindowState = FormWindowState.Normal;
            }
            else if (com_frm.windowSize == 3)
            {
                this.Width = 1400;
                this.Height = 800;
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }
        public void refreshFonts()
        {
            rtxtInfo.BackColor = com_game.module.ModuleTheme.StandardBackColor;
            rtxtLog.BackColor = com_game.module.ModuleTheme.StandardBackColor;
            gbCharacters.Font = com_game.module.ModuleTheme.ModuleFont;
            gbCreatures.Font = com_game.module.ModuleTheme.ModuleFont;
            gbMovesLeft.Font = com_game.module.ModuleTheme.ModuleFont;
            gbPartyAction.Font = com_game.module.ModuleTheme.ModuleFont;
            gbPlayerTurn.Font = com_game.module.ModuleTheme.ModuleFont;
            btnContinue.Font = com_game.module.ModuleTheme.ModuleFont;
            btnDelayTurn.Font = com_frm.ChangeFontSize(com_game.module.ModuleTheme.ModuleFont, 0.95f);
            btnRangedAttack.Font = com_frm.ChangeFontSize(com_game.module.ModuleTheme.ModuleFont, 0.95f);
            btnRunAway.Font = com_game.module.ModuleTheme.ModuleFont;
            btnStayFight.Font = com_game.module.ModuleTheme.ModuleFont;
            btnUseItem.Font = com_frm.ChangeFontSize(com_game.module.ModuleTheme.ModuleFont, 0.95f);
            btnUseSkill.Font = com_frm.ChangeFontSize(com_game.module.ModuleTheme.ModuleFont, 0.95f);
            btnUseSpell.Font = com_frm.ChangeFontSize(com_game.module.ModuleTheme.ModuleFont, 0.95f);
            btnUseTrait.Font = com_frm.ChangeFontSize(com_game.module.ModuleTheme.ModuleFont, 0.95f);
            rtxtLog.Font = com_game.module.ModuleTheme.ModuleFont;
            rtxtInfo.Font = com_game.module.ModuleTheme.ModuleFont;
            chkGrid.Font = com_frm.ChangeFontSize(com_game.module.ModuleTheme.ModuleFont, 0.75f);
            chkFacing.Font = com_frm.ChangeFontSize(com_game.module.ModuleTheme.ModuleFont, 0.75f);
            chkShowRange.Font = com_frm.ChangeFontSize(com_game.module.ModuleTheme.ModuleFont, 0.75f);
            chkHoverOnly.Font = com_frm.ChangeFontSize(com_game.module.ModuleTheme.ModuleFont, 0.75f);
            lblMouseInfo.Font = com_frm.ChangeFontSize(com_game.module.ModuleTheme.ModuleFont, 0.75f);
            txtInfo.Font = com_frm.ChangeFontSize(com_game.module.ModuleTheme.ModuleFont, 0.75f);
            lblMovesLeft.Font = com_frm.ChangeFontSize(com_game.module.ModuleTheme.ModuleFont, 3.0f);
            this.Invalidate();
        }
        private void walkingPcAnimation(PC pc, Point lastPlayerLocation)
        {
            /*if (pc.CharSprite.WalkingNumberOfFrames > 1)
            {
                int sleep = 500 / pc.CharSprite.WalkingFPS;
                int lx = lastPlayerLocation.X;
                int ly = lastPlayerLocation.Y;
                int cx = pc.CombatLocation.X;
                int cy = pc.CombatLocation.Y;
                float increment = com_game._squareSize / pc.CharSprite.WalkingNumberOfFrames;
                float incX = com_game._squareSize / pc.CharSprite.WalkingNumberOfFrames;
                float incY = com_game._squareSize / pc.CharSprite.WalkingNumberOfFrames;
                if      ((cx == lx) && (cy < ly)) { incX *= 0; incY *= -1; } //Up
                else if ((cx > lx) && (cy < ly)) { incX *= 1; incY *= -1; } //UpRight
                else if ((cx < lx) && (cy < ly)) { incX *= -1; incY *= -1; } //UpLeft
                else if ((cx == lx) && (cy > ly)) { incX *= 0; incY *= 1; } //Down
                else if ((cx > lx) && (cy > ly)) { incX *= 1; incY *= 1; } //DownRight
                else if ((cx < lx) && (cy > ly)) { incX *= -1; incY *= 1; } //DownLeft
                else if ((cx > lx) && (cy == ly)) { incX *= 1; incY *= 0; } //Right
                else if ((cx < lx) && (cy == ly)) { incX *= -1; incY *= 0; } //Left
                //start a for loop based on the number of frames in the walking row
                for (int i = 0; i < pc.CharSprite.WalkingNumberOfFrames; i++)
                {                    
                    com_game.spriteErasePcCombatDraw((int)(lx * com_game._squareSize + (i * incX)),
                                                     (int)(ly * com_game._squareSize + (i * incY)), 
                                                      0);
                    com_game.spritePcCombatDraw((int)(lx * com_game._squareSize + (i * incX) + incX),
                                                (int)(ly * com_game._squareSize + (i * incY) + incY), 
                                                 pc, i, 2, sleep);
                    #region old
                    /*
                    if (lx != cx) //moved left or right
                    {
                        if (lx > cx) //moved left x--
                        {
                            //do erase
                            com_game.spriteErasePcCombatDraw((int)(lx * com_game._squareSize - (i * 12.5)), ly * com_game._squareSize, 0);
                            //do draw
                            com_game.spritePcCombatDraw((int)(lx * com_game._squareSize - (i * 12.5) - 12.5), ly * com_game._squareSize, 0, i, 2, sleep);
                        }
                        else //moved right x++
                        {
                            //do erase
                            com_game.spriteErasePcCombatDraw((int)(lx * com_game._squareSize + (i * 12.5)), ly * com_game._squareSize, 0);
                            //do draw
                            com_game.spritePcCombatDraw((int)(lx * com_game._squareSize + (i * 12.5) + 12.5), ly * com_game._squareSize, 0, i, 2, sleep);
                        }
                    }
                    else //moved up or down
                    {
                        if (ly > cy) //moved up y--
                        {
                            //do erase
                            com_game.spriteErasePcCombatDraw(lx * com_game._squareSize, (int)(ly * com_game._squareSize - (i * 12.5)), 0);
                            //do draw
                            com_game.spritePcCombatDraw(lx * com_game._squareSize, (int)(ly * com_game._squareSize - (i * 12.5) - 12.5), 0, i, 2, sleep);
                        }
                        else //moved down y++
                        {
                            //do erase
                            com_game.spriteErasePcCombatDraw(lx * com_game._squareSize, (int)(ly * com_game._squareSize + (i * 12.5)), 0);
                            //do draw
                            com_game.spritePcCombatDraw(lx * com_game._squareSize, (int)(ly * com_game._squareSize + (i * 12.5) + 12.5), 0, i, 2, sleep);
                        }
                    }
                    */
                    /*#endregion
                }
            }*/
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.D)
            {
                if (com_frm.debugMode)
                {
                    com_frm.debugMode = false;
                    logText("DebugMode Turned Off", Color.Lime);
                    logText(Environment.NewLine, Color.Black);
                }
                else
                {
                    com_frm.debugMode = true;
                    logText("DebugMode Turned On", Color.Lime);
                    logText(Environment.NewLine, Color.Black);
                }
            }
            if (keyData == Keys.P)
            {
                if (showStepNumbers) { showStepNumbers = false; }
                else { showStepNumbers = true; }
                return true;
            }
            if (keyData == Keys.V)
            {
                if (chkShowRange.Checked) { chkShowRange.Checked = false; }
                else { chkShowRange.Checked = true; }
                return true;
            }
            if (keyData == Keys.F)
            {
                if (chkFacing.Checked) { chkFacing.Checked = false; }
                else { chkFacing.Checked = true; }
                return true;
            }
            if (keyData == Keys.G)
            {
                if (chkGrid.Checked) { chkGrid.Checked = false; }
                else { chkGrid.Checked = true; }
                return true;
            }
            if (keyData == Keys.Delete)
            {
                logText("GOD MODE KILL ALL!! hotkey 'delete' was pressed", Color.Black);
                logText(Environment.NewLine, Color.Black);
                logText(Environment.NewLine, Color.Black);
                foreach (Creature crt in com_encounter.EncounterCreatureList.creatures)
                {
                    crt.HP = 0;
                }
                return true;
            }
            
            if (canMove)
            {
                if (!rangedItem)
                {
                    PC char_pt = new PC();
                    char_pt.passRefs(com_game, null);
                    char_pt = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];
                    
                    #region FaceDown
                    if (keyData == (Keys.Control | Keys.NumPad2))
                    {
                        char_pt.CombatFacing = CharBase.facing.Down;
                        doUpdate(char_pt);
                        return true; //for the active control to see the keypress, return false 
                    }
                    #endregion
                    #region FaceUp
                    else if (keyData == (Keys.Control | Keys.NumPad8))
                    {
                        char_pt.CombatFacing = CharBase.facing.Up;
                        doUpdate(char_pt);
                        return true; //for the active control to see the keypress, return false 
                    }
                    #endregion
                    #region FaceLeft
                    else if (keyData == (Keys.Control | Keys.NumPad4))
                    {
                        char_pt.CombatFacing = CharBase.facing.Left;
                        doUpdate(char_pt);
                        return true; //for the active control to see the keypress, return false 
                    }
                    #endregion
                    #region FaceRight
                    else if (keyData == (Keys.Control | Keys.NumPad6))
                    {
                        char_pt.CombatFacing = CharBase.facing.Right;
                        doUpdate(char_pt);
                        return true; //for the active control to see the keypress, return false 
                    }
                    #endregion
                    #region FaceDownRight
                    else if (keyData == (Keys.Control | Keys.NumPad3))
                    {
                        char_pt.CombatFacing = CharBase.facing.DownRight;
                        doUpdate(char_pt);
                        return true; //for the active control to see the keypress, return false 
                    }
                    #endregion
                    #region FaceDownLeft
                    else if (keyData == (Keys.Control | Keys.NumPad1))
                    {
                        char_pt.CombatFacing = CharBase.facing.DownLeft;
                        doUpdate(char_pt);
                        return true; //for the active control to see the keypress, return false 
                    }
                    #endregion
                    #region FaceUpRight
                    else if (keyData == (Keys.Control | Keys.NumPad9))
                    {
                        char_pt.CombatFacing = CharBase.facing.UpRight;
                        doUpdate(char_pt);
                        return true; //for the active control to see the keypress, return false 
                    }
                    #endregion
                    #region FaceUpLeft
                    else if (keyData == (Keys.Control | Keys.NumPad7))
                    {
                        char_pt.CombatFacing = CharBase.facing.UpLeft;
                        doUpdate(char_pt);
                        return true; //for the active control to see the keypress, return false 
                    }
                    #endregion
                    #region UpLeft
                    else if ((keyData == Keys.NumPad7) || (keyData == Keys.D7))
                    {
                        moveUpLeft(char_pt);
                        /*
                        if ((char_pt.CombatLocation.X > 0) && (char_pt.CombatLocation.Y > 0))
                        {
                            char_pt.CombatFacing = CharBase.facing.UpLeft;
                            Point lastLocation = new Point(char_pt.CombatLocation.X, char_pt.CombatLocation.Y);
                            char_pt.CombatLocation.X--;
                            char_pt.CombatLocation.Y--;                            
                            if (checkCollision() == true)
                            {
                                char_pt.CombatLocation.X++;
                                char_pt.CombatLocation.Y++;
                                if ((bumpedIntoCreatureIndex >= 0) && (com_encounter.EncounterCreatureList.creatures[bumpedIntoCreatureIndex].HP > 0))
                                {
                                    //IBMessageBox.Show(game, "you bumped into a creature...attack?");
                                    playPcAttackSound(char_pt, char_pt.MainHand.ItemOnUseSound);
                                    doPcTurn();
                                }
                            }
                            else
                            {
                                com_frm.sf.LeaveThreatenedCheck(char_pt, lastLocation);
                                walkingPcAnimation(char_pt, lastLocation);
                                //doTrigger
                                doTrigger(char_pt);
                                doPropOnEnter(char_pt, lastLocation);
                            }
                        }

                        doUpdate(char_pt);
                        */
                        return true; //for the active control to see the keypress, return false 
                    }
                    #endregion
                    #region Left
                    else if ((keyData == Keys.Left) || (keyData == Keys.NumPad4) || (keyData == Keys.D4))
                    {
                        moveLeft(char_pt);
                        /*
                        if (char_pt.CombatLocation.X > 0)
                        {
                            Point lastLocation = new Point(char_pt.CombatLocation.X, char_pt.CombatLocation.Y);
                            char_pt.CombatLocation.X--;
                            char_pt.CombatFacing = CharBase.facing.Left;
                            if (checkCollision() == true)
                            {
                                char_pt.CombatLocation.X++;
                                if ((bumpedIntoCreatureIndex >= 0) && (com_encounter.EncounterCreatureList.creatures[bumpedIntoCreatureIndex].HP > 0))
                                {
                                    //IBMessageBox.Show(game, "you bumped into a creature...attack?");
                                    playPcAttackSound(char_pt, char_pt.MainHand.ItemOnUseSound);
                                    doPcTurn();
                                }
                            }
                            else
                            {
                                com_frm.sf.LeaveThreatenedCheck(char_pt, lastLocation);
                                walkingPcAnimation(char_pt, lastLocation);
                                //doTrigger
                                doTrigger(char_pt);
                                doPropOnEnter(char_pt, lastLocation);
                            }
                        }

                        doUpdate(char_pt);
                        */
                        return true; //for the active control to see the keypress, return false 
                    }
                    #endregion
                    #region DownLeft
                    else if ((keyData == Keys.NumPad1) || (keyData == Keys.D1))
                    {
                        moveDownLeft(char_pt);
                        /*
                        if ((char_pt.CombatLocation.X > 0) && (char_pt.CombatLocation.Y < com_game.currentCombatArea.MapSizeInSquares.Height - 1))
                        {
                            char_pt.CombatFacing = CharBase.facing.DownLeft;
                            Point lastLocation = new Point(char_pt.CombatLocation.X, char_pt.CombatLocation.Y);
                            char_pt.CombatLocation.X--;
                            char_pt.CombatLocation.Y++;
                            if (checkCollision() == true)
                            {
                                char_pt.CombatLocation.X++;
                                char_pt.CombatLocation.Y--;
                                if ((bumpedIntoCreatureIndex >= 0) && (com_encounter.EncounterCreatureList.creatures[bumpedIntoCreatureIndex].HP > 0))
                                {
                                    //IBMessageBox.Show(game, "you bumped into a creature...attack?");
                                    playPcAttackSound(char_pt, char_pt.MainHand.ItemOnUseSound);
                                    doPcTurn();
                                }
                            }
                            else
                            {
                                com_frm.sf.LeaveThreatenedCheck(char_pt, lastLocation);
                                walkingPcAnimation(char_pt, lastLocation);
                                //doTrigger
                                doTrigger(char_pt);
                                doPropOnEnter(char_pt, lastLocation);
                            }
                        }

                        doUpdate(char_pt);
                        */
                        return true; //for the active control to see the keypress, return false 
                    }
                    #endregion
                    #region UpRight
                    else if ((keyData == Keys.NumPad9) || (keyData == Keys.D9))
                    {
                        moveUpRight(char_pt);
                        /*
                        if ((char_pt.CombatLocation.Y > 0) && (char_pt.CombatLocation.X < com_game.currentCombatArea.MapSizeInSquares.Width - 1))
                        {
                            char_pt.CombatFacing = CharBase.facing.UpRight;
                            Point lastLocation = new Point(char_pt.CombatLocation.X, char_pt.CombatLocation.Y);
                            char_pt.CombatLocation.X++;
                            char_pt.CombatLocation.Y--;
                            if (checkCollision() == true)
                            {
                                char_pt.CombatLocation.X--;
                                char_pt.CombatLocation.Y++;
                                if ((bumpedIntoCreatureIndex >= 0) && (com_encounter.EncounterCreatureList.creatures[bumpedIntoCreatureIndex].HP > 0))
                                {
                                    //IBMessageBox.Show(game, "you bumped into a creature...attack?");
                                    playPcAttackSound(char_pt, char_pt.MainHand.ItemOnUseSound);
                                    doPcTurn();
                                }
                            }
                            else
                            {
                                com_frm.sf.LeaveThreatenedCheck(char_pt, lastLocation);
                                walkingPcAnimation(char_pt, lastLocation);
                                //doTrigger
                                doTrigger(char_pt);
                                doPropOnEnter(char_pt, lastLocation);
                            }
                        }

                        doUpdate(char_pt);
                        */
                        return true; //for the active control to see the keypress, return false 
                    }
                    #endregion
                    #region Right
                    else if ((keyData == Keys.Right) || (keyData == Keys.NumPad6) || (keyData == Keys.D6))
                    {
                        moveRight(char_pt);
                        /*
                        if (char_pt.CombatLocation.X < com_game.currentCombatArea.MapSizeInSquares.Width - 1)
                        {
                            Point lastLocation = new Point(char_pt.CombatLocation.X, char_pt.CombatLocation.Y);
                            char_pt.CombatLocation.X++;
                            char_pt.CombatFacing = CharBase.facing.Right;
                            if (checkCollision() == true)
                            {
                                char_pt.CombatLocation.X--;
                                if ((bumpedIntoCreatureIndex >= 0) && (com_encounter.EncounterCreatureList.creatures[bumpedIntoCreatureIndex].HP > 0))
                                {
                                    //IBMessageBox.Show(game, "you bumped into a creature...attack?");
                                    playPcAttackSound(char_pt, char_pt.MainHand.ItemOnUseSound);
                                    doPcTurn();
                                }
                            }
                            else
                            {
                                com_frm.sf.LeaveThreatenedCheck(char_pt, lastLocation);
                                walkingPcAnimation(char_pt, lastLocation);
                                //doTrigger
                                doTrigger(char_pt);
                                doPropOnEnter(char_pt, lastLocation);
                            }
                        }

                        doUpdate(char_pt);
                        */
                        return true; //for the active control to see the keypress, return false 
                    }
                    #endregion
                    #region DownRight
                    else if ((keyData == Keys.NumPad3) || (keyData == Keys.D3))
                    {
                        moveDownRight(char_pt);
                        /*
                        if ((char_pt.CombatLocation.Y < com_game.currentCombatArea.MapSizeInSquares.Height - 1) && (char_pt.CombatLocation.X < com_game.currentCombatArea.MapSizeInSquares.Width - 1))
                        {
                            char_pt.CombatFacing = CharBase.facing.DownRight;
                            Point lastLocation = new Point(char_pt.CombatLocation.X, char_pt.CombatLocation.Y);
                            char_pt.CombatLocation.X++;
                            char_pt.CombatLocation.Y++;
                            if (checkCollision() == true)
                            {
                                char_pt.CombatLocation.X--;
                                char_pt.CombatLocation.Y--;
                                if ((bumpedIntoCreatureIndex >= 0) && (com_encounter.EncounterCreatureList.creatures[bumpedIntoCreatureIndex].HP > 0))
                                {
                                    //IBMessageBox.Show(game, "you bumped into a creature...attack?");
                                    playPcAttackSound(char_pt, char_pt.MainHand.ItemOnUseSound);
                                    doPcTurn();
                                }
                            }
                            else
                            {
                                com_frm.sf.LeaveThreatenedCheck(char_pt, lastLocation);
                                walkingPcAnimation(char_pt, lastLocation);
                                //doTrigger
                                doTrigger(char_pt);
                                doPropOnEnter(char_pt, lastLocation);
                            }
                        }

                        doUpdate(char_pt);
                        */
                        return true; //for the active control to see the keypress, return false 
                    }
                    #endregion
                    #region Up
                    else if ((keyData == Keys.Up) || (keyData == Keys.NumPad8) || (keyData == Keys.D8))
                    {
                        moveUp(char_pt);
                        /*
                        if (char_pt.CombatLocation.Y > 0)
                        {
                            char_pt.CombatFacing = CharBase.facing.Up;
                            Point lastLocation = new Point(char_pt.CombatLocation.X, char_pt.CombatLocation.Y);
                            char_pt.CombatLocation.Y--;
                            if (checkCollision() == true)
                            {
                                char_pt.CombatLocation.Y++;
                                if ((bumpedIntoCreatureIndex >= 0) && (com_encounter.EncounterCreatureList.creatures[bumpedIntoCreatureIndex].HP > 0))
                                {
                                    //IBMessageBox.Show(game, "you bumped into a creature...attack?");
                                    playPcAttackSound(char_pt, char_pt.MainHand.ItemOnUseSound);
                                    doPcTurn();
                                }
                            }
                            else
                            {
                                com_frm.sf.LeaveThreatenedCheck(char_pt, lastLocation);
                                walkingPcAnimation(char_pt, lastLocation);
                                //doTrigger
                                doTrigger(char_pt);
                                doPropOnEnter(char_pt, lastLocation);
                            }
                        }

                        doUpdate(char_pt);
                        */
                        return true; //for the active control to see the keypress, return false 
                    }
                    #endregion
                    #region Down
                    else if ((keyData == Keys.Down) || (keyData == Keys.NumPad2) || (keyData == Keys.D2))
                    {
                        moveDown(char_pt);
                        /*
                        if (char_pt.CombatLocation.Y < com_game.currentCombatArea.MapSizeInSquares.Height - 1)
                        {
                            char_pt.CombatFacing = CharBase.facing.Down;
                            Point lastLocation = new Point(char_pt.CombatLocation.X, char_pt.CombatLocation.Y);
                            char_pt.CombatLocation.Y++;
                            if (checkCollision() == true)
                            {
                                char_pt.CombatLocation.Y--;
                                if ((bumpedIntoCreatureIndex >= 0) && (com_encounter.EncounterCreatureList.creatures[bumpedIntoCreatureIndex].HP > 0))
                                {
                                    //IBMessageBox.Show(game, "you bumped into a creature...attack?");
                                    playPcAttackSound(char_pt, char_pt.MainHand.ItemOnUseSound);
                                    doPcTurn();
                                }
                            }
                            else
                            {
                                com_frm.sf.LeaveThreatenedCheck(char_pt, lastLocation);
                                walkingPcAnimation(char_pt, lastLocation);
                                //doTrigger
                                doTrigger(char_pt);
                                doPropOnEnter(char_pt, lastLocation);
                            }
                        }

                        doUpdate(char_pt);
                        */
                        return true; //for the active control to see the keypress, return false 
                    }
                    #endregion
                    #region HotKeys
                    else if (keyData == Keys.I)
                    {
                        com_frm.pcInventory.rbtnPc0.Checked = true;
                        com_frm.pcInventory.refreshlbxItems();
                        com_frm.pcInventory.btnUseItem.Enabled = false;
                        com_frm.pcInventory.Show();
                        return true;
                    }
                    else if ((keyData == Keys.R) && (btnRangedAttack.Enabled))
                    {
                        selectedRangedAttack();
                        return true;
                    }
                    else if ((keyData == Keys.E) && (btnDelayTurn.Enabled))
                    {
                        selectedEndTurn();
                        return true;
                    }
                    else if ((keyData == Keys.U) && (btnUseItem.Enabled))
                    {
                        selectedUseItem();
                        return true;
                    }
                    else if ((keyData == Keys.K) && (btnUseSkill.Enabled))
                    {
                        selectedUseSkill();
                        return true;
                    }
                    else if ((keyData == Keys.S) && (btnUseSpell.Enabled))
                    {
                        selectedUseSpell();
                        return true;
                    }
                    else if ((keyData == Keys.T) && (btnUseTrait.Enabled))
                    {
                        selectedUseTrait();
                        return true;
                    }
                    #endregion                    
                    else
                        return base.ProcessCmdKey(ref msg, keyData);
                }
                else
                    return base.ProcessCmdKey(ref msg, keyData);
            }            
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void moveUpLeft(PC char_pt)
        {
            if ((char_pt.CombatLocation.X > 0) && (char_pt.CombatLocation.Y > 0))
            {
                char_pt.CombatFacing = CharBase.facing.UpLeft;
                Point lastLocation = new Point(char_pt.CombatLocation.X, char_pt.CombatLocation.Y);
                char_pt.CombatLocation.X--;
                char_pt.CombatLocation.Y--;
                if (checkCollision() == true || (char_pt.MoveDistance - currentMoves) <= 0) //JamesManhattan check for out of moves. 9/26/14
                {
                    char_pt.CombatLocation.X++;
                    char_pt.CombatLocation.Y++;
                    if ((bumpedIntoCreatureIndex >= 0) && (com_encounter.EncounterCreatureList.creatures[bumpedIntoCreatureIndex].HP > 0))
                    {
                        //IBMessageBox.Show(game, "you bumped into a creature...attack?");
                        playPcAttackSound(char_pt, char_pt.MainHand.ItemOnUseSound);
                        doPcTurn();
                    }
                }
                else
                {
                    com_frm.sf.LeaveThreatenedCheck(char_pt, lastLocation);
                    walkingPcAnimation(char_pt, lastLocation);
                    //doTrigger
                    doTrigger(char_pt);
                    doPropOnEnter(char_pt, lastLocation);
                    currentMoves++; //JamesManhattan added here, removed from doUpdate
                }
            }

            doUpdate(char_pt);
        }
        private void moveLeft(PC char_pt)
        {
            if (char_pt.CombatLocation.X > 0)
            {
                Point lastLocation = new Point(char_pt.CombatLocation.X, char_pt.CombatLocation.Y);
                char_pt.CombatLocation.X--;
                char_pt.CombatFacing = CharBase.facing.Left;
                if (checkCollision() == true || (char_pt.MoveDistance - currentMoves) <= 0) //JamesManhattan check for out of moves. 9/26/14
                {
                    char_pt.CombatLocation.X++;
                    if ((bumpedIntoCreatureIndex >= 0) && (com_encounter.EncounterCreatureList.creatures[bumpedIntoCreatureIndex].HP > 0))
                    {
                        //IBMessageBox.Show(game, "you bumped into a creature...attack?");
                        playPcAttackSound(char_pt, char_pt.MainHand.ItemOnUseSound);
                        doPcTurn();
                    }
                }
                else
                {
                    com_frm.sf.LeaveThreatenedCheck(char_pt, lastLocation);
                    walkingPcAnimation(char_pt, lastLocation);
                    //doTrigger
                    doTrigger(char_pt);
                    doPropOnEnter(char_pt, lastLocation);
                    currentMoves++; //JamesManhattan added here, removed from doUpdate
                }
            }

            doUpdate(char_pt);
        }        
        private void moveDownLeft(PC char_pt)
        {
            if ((char_pt.CombatLocation.X > 0) && (char_pt.CombatLocation.Y < com_game.currentCombatArea.MapSizeInSquares.Height - 1))
            {
                char_pt.CombatFacing = CharBase.facing.DownLeft;
                Point lastLocation = new Point(char_pt.CombatLocation.X, char_pt.CombatLocation.Y);
                char_pt.CombatLocation.X--;
                char_pt.CombatLocation.Y++;
                if (checkCollision() == true || (char_pt.MoveDistance - currentMoves) <= 0) //JamesManhattan check for out of moves. 9/26/14
                {
                    char_pt.CombatLocation.X++;
                    char_pt.CombatLocation.Y--;
                    if ((bumpedIntoCreatureIndex >= 0) && (com_encounter.EncounterCreatureList.creatures[bumpedIntoCreatureIndex].HP > 0))
                    {
                        //IBMessageBox.Show(game, "you bumped into a creature...attack?");
                        playPcAttackSound(char_pt, char_pt.MainHand.ItemOnUseSound);
                        doPcTurn();
                    }
                }
                else
                {
                    com_frm.sf.LeaveThreatenedCheck(char_pt, lastLocation);
                    walkingPcAnimation(char_pt, lastLocation);
                    //doTrigger
                    doTrigger(char_pt);
                    doPropOnEnter(char_pt, lastLocation);
                    currentMoves++; //JamesManhattan added here, removed from doUpdate
                }
            }

            doUpdate(char_pt);
        }
        private void moveUpRight(PC char_pt)
        {
            if ((char_pt.CombatLocation.Y > 0) && (char_pt.CombatLocation.X < com_game.currentCombatArea.MapSizeInSquares.Width - 1))
            {
                char_pt.CombatFacing = CharBase.facing.UpRight;
                Point lastLocation = new Point(char_pt.CombatLocation.X, char_pt.CombatLocation.Y);
                char_pt.CombatLocation.X++;
                char_pt.CombatLocation.Y--;
                if (checkCollision() == true || (char_pt.MoveDistance - currentMoves) <= 0) //JamesManhattan check for out of moves. 9/26/14
                {
                    char_pt.CombatLocation.X--;
                    char_pt.CombatLocation.Y++;
                    if ((bumpedIntoCreatureIndex >= 0) && (com_encounter.EncounterCreatureList.creatures[bumpedIntoCreatureIndex].HP > 0))
                    {
                        //IBMessageBox.Show(game, "you bumped into a creature...attack?");
                        playPcAttackSound(char_pt, char_pt.MainHand.ItemOnUseSound);
                        doPcTurn();
                    }
                }
                else
                {
                    com_frm.sf.LeaveThreatenedCheck(char_pt, lastLocation);
                    walkingPcAnimation(char_pt, lastLocation);
                    //doTrigger
                    doTrigger(char_pt);
                    doPropOnEnter(char_pt, lastLocation);
                    currentMoves++; //JamesManhattan added here, removed from doUpdate
                }
            }

            doUpdate(char_pt);
        }
        private void moveRight(PC char_pt)
        {
            if (char_pt.CombatLocation.X < com_game.currentCombatArea.MapSizeInSquares.Width - 1)
            {
                Point lastLocation = new Point(char_pt.CombatLocation.X, char_pt.CombatLocation.Y);
                char_pt.CombatLocation.X++;
                char_pt.CombatFacing = CharBase.facing.Right;
                if (checkCollision() == true || (char_pt.MoveDistance - currentMoves) <= 0) //JamesManhattan check for out of moves. 9/26/14
                {
                    char_pt.CombatLocation.X--;
                    if ((bumpedIntoCreatureIndex >= 0) && (com_encounter.EncounterCreatureList.creatures[bumpedIntoCreatureIndex].HP > 0))
                    {
                        //IBMessageBox.Show(game, "you bumped into a creature...attack?");
                        playPcAttackSound(char_pt, char_pt.MainHand.ItemOnUseSound);
                        doPcTurn();
                    }
                }
                else
                {
                    com_frm.sf.LeaveThreatenedCheck(char_pt, lastLocation);
                    walkingPcAnimation(char_pt, lastLocation);
                    //doTrigger
                    doTrigger(char_pt);
                    doPropOnEnter(char_pt, lastLocation);
                    currentMoves++; //JamesManhattan added here, removed from doUpdate
                }
            }

            doUpdate(char_pt);
        }
        private void moveDownRight(PC char_pt)
        {
            if ((char_pt.CombatLocation.Y < com_game.currentCombatArea.MapSizeInSquares.Height - 1) && (char_pt.CombatLocation.X < com_game.currentCombatArea.MapSizeInSquares.Width - 1))
            {
                char_pt.CombatFacing = CharBase.facing.DownRight;
                Point lastLocation = new Point(char_pt.CombatLocation.X, char_pt.CombatLocation.Y);
                char_pt.CombatLocation.X++;
                char_pt.CombatLocation.Y++;
                if (checkCollision() == true || (char_pt.MoveDistance - currentMoves) <= 0) //JamesManhattan check for out of moves. 9/26/14
                {
                    char_pt.CombatLocation.X--;
                    char_pt.CombatLocation.Y--;
                    if ((bumpedIntoCreatureIndex >= 0) && (com_encounter.EncounterCreatureList.creatures[bumpedIntoCreatureIndex].HP > 0))
                    {
                        //IBMessageBox.Show(game, "you bumped into a creature...attack?");
                        playPcAttackSound(char_pt, char_pt.MainHand.ItemOnUseSound);
                        doPcTurn();
                    }
                }
                else
                {
                    com_frm.sf.LeaveThreatenedCheck(char_pt, lastLocation);
                    walkingPcAnimation(char_pt, lastLocation);
                    //doTrigger
                    doTrigger(char_pt);
                    doPropOnEnter(char_pt, lastLocation);
                    currentMoves++; //JamesManhattan added here, removed from doUpdate
                }
            }

            doUpdate(char_pt);
        }
        private void moveUp(PC char_pt)
        {
            if (char_pt.CombatLocation.Y > 0)
            {
                char_pt.CombatFacing = CharBase.facing.Up;
                Point lastLocation = new Point(char_pt.CombatLocation.X, char_pt.CombatLocation.Y);
                char_pt.CombatLocation.Y--;
                if (checkCollision() == true || (char_pt.MoveDistance - currentMoves) <= 0) //JamesManhattan check for out of moves. 9/26/14
                {
                    char_pt.CombatLocation.Y++;
                    if ((bumpedIntoCreatureIndex >= 0) && (com_encounter.EncounterCreatureList.creatures[bumpedIntoCreatureIndex].HP > 0))
                    {
                        //IBMessageBox.Show(game, "you bumped into a creature...attack?");
                        playPcAttackSound(char_pt, char_pt.MainHand.ItemOnUseSound);
                        doPcTurn();
                    }
                }
                else
                {
                    com_frm.sf.LeaveThreatenedCheck(char_pt, lastLocation);
                    walkingPcAnimation(char_pt, lastLocation);
                    //doTrigger
                    doTrigger(char_pt);
                    doPropOnEnter(char_pt, lastLocation);
                    currentMoves++; //JamesManhattan added here, removed from doUpdate 9/25/14
                }
            }

            doUpdate(char_pt);
        }
        private void moveDown(PC char_pt)
        {
            if (char_pt.CombatLocation.Y < com_game.currentCombatArea.MapSizeInSquares.Height - 1)
            {
                char_pt.CombatFacing = CharBase.facing.Down;
                Point lastLocation = new Point(char_pt.CombatLocation.X, char_pt.CombatLocation.Y);
                char_pt.CombatLocation.Y++;
                if (checkCollision() == true || (char_pt.MoveDistance - currentMoves) <= 0) //JamesManhattan check for out of moves. 9/26/14
                {
                    char_pt.CombatLocation.Y--;
                    if ((bumpedIntoCreatureIndex >= 0) && (com_encounter.EncounterCreatureList.creatures[bumpedIntoCreatureIndex].HP > 0))
                    {
                        //IBMessageBox.Show(game, "you bumped into a creature...attack?");
                        playPcAttackSound(char_pt, char_pt.MainHand.ItemOnUseSound);
                        doPcTurn();
                    }
                }
                else
                {
                    com_frm.sf.LeaveThreatenedCheck(char_pt, lastLocation);
                    walkingPcAnimation(char_pt, lastLocation);
                    //doTrigger
                    doTrigger(char_pt);
                    doPropOnEnter(char_pt, lastLocation);
                    currentMoves++; //JamesManhattan added here, removed from doUpdate
                }
            }

            doUpdate(char_pt);
        }

        /*
        private void leaveThreatenedCheck(PC pc, Point lastPlayerLocation)
        {
            //iterate through each creature
            foreach (Creature crt in com_encounter.EncounterCreatureList.creatures)
            {
                //if started in distance = 1 and now distance = 2 then do attackOfOpportunity
                if ((calcDistance(crt.CombatLocation, lastPlayerLocation) == 1) && (calcDistance(crt.CombatLocation, pc.CombatLocation) == 2))
                {
                    logText("Attack of Opportunity by: " + crt.Tag, Color.Blue);
                    logText(Environment.NewLine, Color.Black);
                    doCreatureAttackOfOpportunity(crt, pc, lastPlayerLocation);
                }
            }
        }
        private void doCreatureAttackOfOpportunity(Creature crt, PC pc, Point lastPlayerLocation)
        {
            if (crt.HP > 0)
            {
                com_frm.sf.CombatTarget = pc;
                com_frm.sf.CombatSource = crt;
                int attackRoll = com_game.Random(20);
                int attackMod = com_frm.sf.CalcCreatureAttackModifier();
                int defense = com_frm.sf.CalcPcDefense();
                int damage = com_frm.sf.CalcCreatureDamageToPc();
                int attack = attackRoll + attackMod;

                playCreatureAttackSound(crt);
                // do attack animation if sprite has animations
                if (crt.CharSprite.AttackingNumberOfFrames > 1)
                {
                    attackCreatureAnimation(crt);
                }
                if (attack >= defense)
                {
                    com_game.drawHitSymbol(lastPlayerLocation.X, lastPlayerLocation.Y);
                    com_game.UpdateCombat();
                    Application.DoEvents();
                    Thread.Sleep(100);
                    refreshMap();
                    logText(crt.Name, Color.LightGray);
                    logText(" attacks ", Color.Black);
                    logText(pc.Name, Color.Blue);
                    logText(" and HITS for ", Color.Black);
                    logText(damage.ToString(), Color.Red);
                    logText(" point(s) of damage", Color.Black);
                    logText(Environment.NewLine, Color.Black);
                    logText(attackRoll.ToString() + " + " + attackMod.ToString() + " >= " + defense.ToString(), Color.Black);
                    logText(Environment.NewLine, Color.Black);
                    logText(Environment.NewLine, Color.Black);

                    pc.HP = pc.HP - damage;
                    if (pc.HP <= 0)
                    {
                        logText(pc.Name + " has been killed!", Color.Red);
                        logText(Environment.NewLine, Color.Black);
                        logText(Environment.NewLine, Color.Black);
                        pc.Status = PC.charStatus.Dead;
                    }
                }
                else
                {
                    logText(crt.Name, Color.LightGray);
                    logText(" attacks ", Color.Black);
                    logText(pc.Name, Color.Blue);
                    logText(" and MISSES", Color.Black);
                    logText(Environment.NewLine, Color.Black);
                    logText(attackRoll.ToString() + " + " + attackMod.ToString() + " < " + defense.ToString(), Color.Black);
                    logText(Environment.NewLine, Color.Black);
                    logText(Environment.NewLine, Color.Black);
                }
            }
            else
            {
                logText(crt.Name, Color.LightGray);
                logText(" is unconscious...skips turn", Color.Black);
                logText(Environment.NewLine, Color.Black);
                logText(Environment.NewLine, Color.Black);
            }
        }
        */

        #region UI Stuff
        private void chkGrid_CheckedChanged(object sender, EventArgs e)
        {
            com_game.com_showGrid = chkGrid.Checked;
        }
        private void chkFacing_CheckedChanged(object sender, EventArgs e)
        {
            com_game.com_showFacing = chkFacing.Checked;
        }
        private void chkShowRange_CheckedChanged(object sender, EventArgs e)
        {
            com_game.com_showRange = chkShowRange.Checked;
        }
        private void btnContinue_Click(object sender, EventArgs e)
        {
            refreshMap();
            if (endEncounter)
            {
                #region OnEndCombat
                // run OnEndCombat script 
                var scriptEnc = com_encounter.OnEndCombat;
                com_frm.doScriptBasedOnFilename(scriptEnc.FilenameOrTag, scriptEnc.Parm1, scriptEnc.Parm2, scriptEnc.Parm3, scriptEnc.Parm4);
                #endregion
                this.Close();
            }
        }
        private void btnStayFight_Click(object sender, EventArgs e)
        {
            //Press this button to begin combat
            BeginRound();
            btnStayFight.Enabled = false;
        }
        private void btnRunAway_Click(object sender, EventArgs e)
        {
            try
            {
                #region OnFleeCombat
                com_frm.sf.returnScriptObject = null; //after using the generic "returnScriptObject", make sure to set it back to null
                //CombatTarget = pc;
                //CombatSource = crt;
                com_frm.sf.passParameterScriptObject = combatRoundNumber; //can be set to any object type passed into script, make sure to null out after use. used here to pass the number of rounds to the script without using up any of the Parms1-4
                var scriptEnc = com_encounter.OnFleeCombat;
                com_frm.doScriptBasedOnFilename(scriptEnc.FilenameOrTag, scriptEnc.Parm1, scriptEnc.Parm2, scriptEnc.Parm3, scriptEnc.Parm4);
                com_frm.sf.passParameterScriptObject = null; //can be set to any object type passed into script, make sure to null out after use
                #endregion

                if (com_frm.sf.returnScriptObject == null) { return; }
                if ((bool)com_frm.sf.returnScriptObject == true) //succeded in running away
                {
                    com_game.playerPosition.X = com_game.lastPlayerLocation.X;
                    com_game.playerPosition.Y = com_game.lastPlayerLocation.Y;
                    this.Close();
                }
            }
            finally
            {
                com_frm.sf.returnScriptObject = null; //after using the generic "returnScriptObject", make sure to set it back to null
            }
        }
        #endregion
        
        private void BeginRound()
        {           
            gbPlayerTurn.Enabled = false;
            refreshMap();
            if (endEncounter)
            {
                gbPartyAction.Enabled = false;
            }
            else
            {
                #region the beginning of the round
                if ((currentMoveOrderIndex == 0) || (currentMoveOrderIndex >= com_moveOrderList.Count))
                {
                    canMove = false;
                    btnContinue.Enabled = false;
                    gbPartyAction.Enabled = true;
                    // * sinopip, 10.08.14
                    //doOnDeathScripts();
                    cleanUpCreatures();
                    doCreateLabels();
                    refreshCharacterPanel();
                    combatRoundNumber++;                    
                    //IBMessageBox.Show(com_game, "Round " + combatRoundNumber.ToString() + " begins");
                    logText("Round " + combatRoundNumber.ToString() + " begins", Color.Black);
                    logText(Environment.NewLine, Color.Black);
                    logText(Environment.NewLine, Color.Black);

                    doScriptBasedOnFilename("dsCombatTime.cs", "none", "none", "none", "none");

                    #region OnStartCombatRound
                    // run OnStartCombatRound script 
                    var scriptEnc = com_encounter.OnStartCombatRound;
                    com_frm.doScriptBasedOnFilename(scriptEnc.FilenameOrTag, scriptEnc.Parm1, scriptEnc.Parm2, scriptEnc.Parm3, scriptEnc.Parm4);
                    #endregion

                    applyEffects();                    
                    StayAndFight();
                }
                #endregion

                #region middle of the round
                else
                {
                    if (com_moveOrderList[currentMoveOrderIndex].type == "pc") // if next on the list is a PC then doPcTurn
                    {
                        //if PC is dead or held, skip turn
                        PC char_pt = new PC();
                        char_pt.passRefs(com_game, null);
                        char_pt = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];
                        if ((char_pt.Status == PC.charStatus.Dead) || (char_pt.Status == PC.charStatus.Held))
                        {
                            checkEndEncounter();
                            doCreateLabels();
                            refreshCharacterPanel();
                            currentMoves = 99;
                            usedAction = 1; //JamesManhattan set dead players with no action
                            doUpdate(char_pt);
                            return;
                        }

                        gbPlayerTurn.Enabled = true;
                        PlayerTurnButtonsOnOff();
                        btnContinue.Enabled = false;
                        canMove = true;
                        lblMovesLeft.Text = char_pt.MoveDistance.ToString();
                        if (char_pt.OffHand.ItemCategory == Item.category.Melee) //JamesManhattan check for off hand equipped
                        {
                            logText(char_pt.Name + " has a off-hand weapon. ", Color.Black); //give notice of offhand weapon equipped
                            usedSwiftBonusAction = 0; //JamesManhattan 9/25/14 bonus action
                            numBonusAttacks = 1; //JamesManhattan 9/25/14 set that they can use bonus action to attack
                        }
                        if (HasTraitLookup(char_pt, "5E_ExtraAttack")) //JamesManhattan lookup whether player has 5E_ExtraAttack Trait
                        {                            
                            numAttacks = 2; //JamesManhattan 9/25/14 set the number of attacks at the start of the player turn.
                            logText(char_pt.Name, Color.LightGray);
                            logText(" can make 2 attacks. ", Color.Black); //give notice of 2 attacks
                            logText(Environment.NewLine, Color.Black);
                        }
                        else 
                        { numAttacks = 1;}
                        usedAction = 0; //JamesManhattan 9/26/14 set that the player has not used an Action yet. at the beginning
                        startedMoving = 0; //JamesManhattan 9/28/14 set that the player has not started moving

                        #region OnStartCombatTurn
                        // run OnStartCombatTurn script 
                        com_frm.sf.CombatSource = char_pt; 
                        var scriptPcTurn = char_pt.OnStartCombatTurn;
                        com_frm.doScriptBasedOnFilename(scriptPcTurn.FilenameOrTag, scriptPcTurn.Parm1, scriptPcTurn.Parm2, scriptPcTurn.Parm3, scriptPcTurn.Parm4);
                        #endregion
                    }
                    else // else if next on the list is a creature doCreatureTurn
                    {
                        btnContinue.Enabled = false;
                        canMove = false;
                        doCreatureTurn();
                        if (currentMoveOrderIndex > com_moveOrderList.Count - 1)
                        {
                            currentMoveOrderIndex = 0;
                        }
                        currentMoves = 0;
                        usedAction = 0; //JamesManhattan
                        startedMoving = 0; //JamesManhattan 9/28/14 set that the player has not started moving
                    }
                }
                #endregion
            }
        }                      
        private void StayAndFight()
        {
            gbPartyAction.Enabled = true;
            resetMoveOrder();
            currentMoveOrderIndex = 0;
            refreshMap();
            #region PC is first to go
            if (com_moveOrderList[currentMoveOrderIndex].type == "pc") // if next on the list is a PC then doPcTurn
            {
                //if PC is dead or held, skip turn
                PC char_pt = new PC();
                char_pt.passRefs(com_game, null);
                char_pt = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];
                if ((char_pt.Status == PC.charStatus.Dead) || (char_pt.Status == PC.charStatus.Held))
                {
                    checkEndEncounter();
                    doCreateLabels();
                    refreshCharacterPanel();
                    currentMoves = 99;
                    usedAction = 1; //JamesManhattan
                    doUpdate(char_pt);
                    return;
                }
                gbPlayerTurn.Enabled = true;
                PlayerTurnButtonsOnOff();
                canMove = true;
                lblMovesLeft.Text = char_pt.MoveDistance.ToString();
                if (char_pt.OffHand.ItemCategory == Item.category.Melee) //JamesManhattan check for off hand equipped
                {
                    logText(char_pt.Name + " has a off-hand weapon. ", Color.Black); //give notice of offhand weapon equipped
                    usedSwiftBonusAction = 0; //JamesManhattan 9/25/14 bonus action
                    numBonusAttacks = 1; //JamesManhattan 9/25/14 set that they can use bonus action to attack
                }
                if (HasTraitLookup(char_pt, "5E_ExtraAttack")) //JamesManhattan lookup whether player has 5E_ExtraAttack Trait
                {
                    numAttacks = 2; //JamesManhattan 9/25/14 set the number of attacks at the start of the player turn.
                    logText(char_pt.Name, Color.LightGray);
                    logText(" can make 2 attacks. ", Color.Black); //give notice of 2 attacks
                    logText(Environment.NewLine, Color.Black);
                }
                else
                { numAttacks = 1; }
                usedAction = 0; //JamesManhattan 9/26/14 set that the player has not used an Action yet. at the beginning
                startedMoving = 0; //JamesManhattan 9/28/14 set that the player has not started moving
            }
            #endregion

            #region Creature is first to go
            else // else if next on the list is a creature doCreatureTurn
            {
                btnContinue.Enabled = false;
                canMove = false;
                doCreatureTurn();
                if (currentMoveOrderIndex > com_moveOrderList.Count - 1)
                {
                    currentMoveOrderIndex = 0;
                }
                currentMoves = 0;
                usedAction = 0; //JamesManhattan reset used action at start of turn
                startedMoving = 0; //JamesManhattan 9/28/14 set that the player has not started moving
            }
            #endregion
        }
        private void doPcTurn()
        {
            if ((numAttacks <= 0 && numBonusAttacks <=0) || (usedAction > 0 && midAttack != 1)) //JamesManhattan added a check if already used action or out of #attacks.
            {
                IBMessageBox.Show(com_game, "used all Attacks this turn.");
            }
            else
            {
                try
                {
                    Creature crt_pt = new Creature();
                    crt_pt.passRefs(com_game, null);
                    PC char_pt = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];
                    crt_pt = com_encounter.EncounterCreatureList.creatures[bumpedIntoCreatureIndex];
                    char_pt.UpdateStats(com_frm.sf);

                    #region OnAttack
                    // run OnStartCombatTurn script 
                    com_frm.sf.CombatTarget = crt_pt;
                    com_frm.sf.CombatSource = char_pt;
                    var scriptCrt = char_pt.OnAttack;
                    com_frm.doScriptBasedOnFilename(scriptCrt.FilenameOrTag, scriptCrt.Parm1, scriptCrt.Parm2, scriptCrt.Parm3, scriptCrt.Parm4);
                    #endregion

                    #region PcAttack
                    // run OnStartCombatTurn script 
                    com_frm.sf.CombatTarget = crt_pt;
                    com_frm.sf.CombatSource = char_pt;
                    //int damage = com_frm.sf.CalcCreatureDamageToPc(); //JamesManhattan commented out, why needed here?
                    if (rangedItem == false && char_pt.MainHand.ItemCategory == Item.category.Ranged) //JamesManhattan check for using a ranged item in melee and give warning
                    {
                        IBMessageBox.Show(com_game, "You have a ranged weapon equipped and attempted a melee attack. You must press Ranged Attack or switch weapons.");
                    }
                    else
                    {
                        if (numAttacks > 0)
                        {

                            if (rangedItem == true) //JamesManhattan flagged from using a ranged attack
                            {
                                com_frm.doScriptBasedOnFilename("dsAttackPC.cs", "ranged", "none", "none", "none");
                            }
                            else
                            {
                                com_frm.doScriptBasedOnFilename("dsAttackPC.cs", "none", "none", "none", "none");
                            }
                        }

                        else
                        {
                            if (numBonusAttacks > 0)
                            {
                                com_frm.doScriptBasedOnFilename("dsAttackPC.cs", "none", "offhand", "none", "none"); //off-hand attack
                            }
                        }

                         #endregion
                        // * sinopip, 10.08.14
                        //doOnDeathScripts();   //JamesManhattan temp commented out for test 9/25/14 only orig one, this should go in different place than here
                        // * sinopip, 20.12.14 : needed to play sound and animation ; changed this class constructor to init "HasDied" LocalInts
                        doOnDeathScripts();
                        //cleanUpCreatures(); //JamesManhattan test if this works 9/25/14
                        //resetMoveOrder(); //JamesManhattan test if this works 9/25/14

                        //checkEndEncounter(); //JamesManhattan commented out for test 9/25/14  9/25/14 orig here
                        //doCreateLabels(); //JamesManhattan commented out for test 9/25/14  9/25/14 orig here      
                        usedAction = 1; //JamesManhattan added 9/26/14
                        midAttack = 1; //JamesManhattan added 9/26/14   to flag you started attacking and arent finished. for 5 ft step or other stuff                   
                        numAttacks = numAttacks - 1; //JamesManhattan added 9/25/14   
                        if (numAttacks < 0) { numBonusAttacks = numBonusAttacks - 1; } //first decrement attacks, once it goes negative, then decrement Bonus Attacks.
                        if (numAttacks <= 0 && numBonusAttacks <= 0)   //JamesManhattan added 9/26/14 once you used all attacks mark midAttack as 0    
                        {
                            midAttack = 0;
                        }
                    }
                    refreshCharacterPanel(); //JamesManhattan commented out for test 9/25/14  9/25/14 orig here
                    //currentMoves = 20; //JamesManhattan commented out 9/25/14                                     
                    //refreshMap(); //JamesManhattan commented out for test 9/25/14  9/25/14 orig here
                }
                catch (Exception ex)
                {
                    IBMessageBox.Show(com_game, "failed doPCTurn");
                    com_game.errorLog(ex.ToString());
                }
            }
        }
        public void attackPcAnimation(PC pc, int PcIndex)
        {
            int attackRowIndex = 1;
            int sleep = 1000 / pc.CharSprite.AttackingFPS;
            //start a for loop based on the number of frames in the attack row
            for (int x = 0; x < pc.CharSprite.AttackingNumberOfFrames; x++)
            {
                com_game.CombatAreaPcAnimateRenderAll(PcIndex, x, attackRowIndex);
                Thread.Sleep(sleep);
            }
        }
        // * sinopip, 25.12.14
        public void deathPCAnimation(PC pc, int PcIndex)
        {
            int deathRowIndex = 3;
            int sleep = 1000 / pc.CharSprite.DeathFPS;
            //start a for loop based on the number of frames in the attack row
            for (int x = 0; x < pc.CharSprite.DeathNumberOfFrames; x++)
            {
                com_game.CombatAreaPcAnimateRenderAll(PcIndex, x, deathRowIndex);
                Thread.Sleep(sleep);
            }
        }
        //
        public void playPcAttackSound(PC pc, string soundFileName)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            try
            {
                if (soundFileName == "none")
                {
                    player.SoundLocation = com_game.mainDirectory + "\\data\\sounds\\soundFX\\punch.wav";
                }
                //else
                //{
                //    player.SoundLocation = com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\sounds\\soundFX\\" + soundFileName;
                //}
                else if (File.Exists(com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\sounds\\soundFX\\" + soundFileName))
                {
                    player.SoundLocation = com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\sounds\\soundFX\\" + soundFileName;
                }
                else if (File.Exists(com_game.mainDirectory + "\\data\\sounds\\soundFX\\" + soundFileName))
                {
                    player.SoundLocation = com_game.mainDirectory + "\\data\\sounds\\soundFX\\" + soundFileName;
                }
                else
                {
                    player.SoundLocation = com_game.mainDirectory + "\\data\\sounds\\soundFX\\punch.wav";
                }
            }
            catch { }
            player.Play();
            Thread.Sleep(300);
            player.Dispose();
        }
        // * sinopip, 22.12.14
        public void playItemHitSound(Item it)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            try
            {
                if (File.Exists(com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\sounds\\soundFX\\" + it.ItemOnHitSound))
                    player.SoundLocation = com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\sounds\\soundFX\\" + it.ItemOnHitSound;
                else if (File.Exists(com_game.mainDirectory + "\\data\\sounds\\soundFX\\" + it.ItemOnHitSound))
                    player.SoundLocation = com_game.mainDirectory + "\\data\\sounds\\soundFX\\" + it.ItemOnHitSound;
                else
                    return;
            	player.Play();
            }
            catch { }
            Thread.Sleep(300);
            player.Dispose();
        }
		//        
        // * sinopip, 22.12.14
        public void playCreatureHitSound(Creature crt)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(); 			       
            try
            {
                if (crt.OnHitSound == "none")
                    player.SoundLocation = com_game.mainDirectory + "\\data\\sounds\\soundFX\\punch.wav";
                else if (File.Exists(com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\sounds\\soundFX\\" + crt.OnHitSound))
            			player.SoundLocation = com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\sounds\\soundFX\\" + crt.OnHitSound;
                else
                    player.SoundLocation = com_game.mainDirectory + "\\data\\sounds\\soundFX\\punch.wav";
            }
            catch { }
            player.Play();
            Thread.Sleep(100);
            player.Dispose();
        }
		//        
        private void doCreatureTurn()
        {
            try
            {                
                Creature crt_pt = new Creature();
                crt_pt.passRefs(com_game, null);                
                crt_pt = com_encounter.EncounterCreatureList.creatures[com_moveOrderList[currentMoveOrderIndex].index];
                
                com_frm.sf.DoCreatureTurn(crt_pt);

                #region moved to ScriptFunctions.dll
                /*
                if ((crt_pt.HP > 0) && (crt_pt.Status != PC.charStatus.Held))
                {
                    PC char_pt = new PC();
                    char_pt.passRefs(com_game, null);
                    com_frm.sf.CombatTarget = null;
                    com_frm.sf.CombatSource = crt_pt;

                    #region onStartCombatTurn
                    // run OnStartCombatTurn script                    
                    var scriptCrt = crt_pt.OnStartCombatTurn;
                    com_frm.doScriptBasedOnFilename(scriptCrt.FilenameOrTag, scriptCrt.Parm1, scriptCrt.Parm2, scriptCrt.Parm3, scriptCrt.Parm4);
                    #endregion

                    if (com_frm.sf.CombatTarget != null)
                    {
                        //use the target from sf.CombatTarget
                        if ((com_frm.sf.CombatTarget is int) && ((int)com_frm.sf.CombatTarget >= 0))
                        {
                            //the target is a PC using the PCindex
                            int PCindex = (int)com_frm.sf.CombatTarget;
                            char_pt = com_game.playerList.PCList[PCindex];
                            com_frm.sf.CombatTarget = char_pt;
                        }
                        else if (com_frm.sf.CombatTarget is Creature)
                        {
                            //target is a creature
                        }
                        else if (com_frm.sf.CombatTarget is Point)
                        {
                            //target is a location on the combat map
                        }
                        else
                        {
                            //IBMessageBox.Show(com_game, "failed to find a target from script, using nearest PC instead");
                            com_game.errorLog("creature failed to find a target from script, using nearest PC instead");
                            // use default closest PC target
                            int whichOne = findNearestPc();
                            char_pt = com_game.playerList.PCList[whichOne];
                            com_frm.sf.CombatTarget = char_pt;
                        }
                    }
                    else
                    {
                        // use default closest PC target
                        int whichOne = findNearestPc();
                        char_pt = com_game.playerList.PCList[whichOne];
                        com_frm.sf.CombatTarget = char_pt;
                    }
                    //make sure target is a PC
                    if (com_frm.sf.CombatTarget is PC)
                    {
                        char_pt = (PC)com_frm.sf.CombatTarget;
                        // determine if ranged or melee
                        if ((crt_pt.WeaponType == Creature.crCategory.Ranged) && (calcDistance(char_pt.CombatLocation, crt_pt.CombatLocation) <= crt_pt.AttackRange)  && (IsVisibleLineOfSight(crt_pt.CombatLocation, char_pt.CombatLocation)))
                        {
                            Point starting = new Point((crt_pt.CombatLocation.X * com_game._squareSize) + (com_game._squareSize / 2), (crt_pt.CombatLocation.Y * com_game._squareSize) + (com_game._squareSize / 2));
                            Point ending = new Point((char_pt.CombatLocation.X * com_game._squareSize) + (com_game._squareSize / 2), (char_pt.CombatLocation.Y * com_game._squareSize) + (com_game._squareSize / 2));
                            playCreatureAttackSound(crt_pt); 
                            drawProjectile(starting, ending, crt_pt.ProjectileSpriteFilename);

                            #region The actual attack portion
                            if (crt_pt.HP > 0)
                            {
                                #region OnAttack
                                // run OnStartCombatTurn script 
                                com_frm.sf.CombatTarget = char_pt;
                                com_frm.sf.CombatSource = crt_pt;
                                scriptCrt = crt_pt.OnAttack;
                                com_frm.doScriptBasedOnFilename(scriptCrt.FilenameOrTag, scriptCrt.Parm1, scriptCrt.Parm2, scriptCrt.Parm3, scriptCrt.Parm4);
                                #endregion
                                #region CreatureAttack
                                // run OnStartCombatTurn script 
                                com_frm.sf.CombatTarget = char_pt;
                                com_frm.sf.CombatSource = crt_pt;
                                com_frm.doScriptBasedOnFilename("attackCreature.cs", "none", "none", "none", "none");
                                #endregion
                            }
                            else
                            {
                                logText(crt_pt.Name, Color.LightGray);
                                logText(" is unconscious...skips turn", Color.Black);
                                logText(Environment.NewLine, Color.Black);
                                logText(Environment.NewLine, Color.Black);
                            }
                            #endregion
                        }
                        else
                        {
                            setupPathfindArray(crt_pt, char_pt);
                            _pathfinder.Squares[crt_pt.CombatLocation.X, crt_pt.CombatLocation.Y].ContentCode = SquareContent.Monster;
                            _pathfinder.Squares[char_pt.CombatLocation.X, char_pt.CombatLocation.Y].ContentCode = SquareContent.Hero;
                            Recalculate(crt_pt);
                            if (showStepNumbers)
                            {
                                showSteps();
                                MessageBox.Show("check it out");
                            }
                            setupHeroSquares(char_pt, crt_pt);
                            navigatePath();
                            
                            // if melee, try and move to attack or get close
                            // check to see if selected PC is one square away, if so attack else skip
                            //if (calcDistance(char_pt.CombatLocation, crt_pt.CombatLocation) == 1)
                            if (creatureWithinMeleeDistance(crt_pt,char_pt))
                            {
                                doCreatureCombatFacing(crt_pt, char_pt);
                                #region The actual attack portion
                                if (crt_pt.HP > 0)
                                {
                                    #region OnAttack
                                    // run OnStartCombatTurn script 
                                    com_frm.sf.CombatTarget = char_pt;
                                    com_frm.sf.CombatSource = crt_pt;
                                    scriptCrt = crt_pt.OnAttack;
                                    com_frm.doScriptBasedOnFilename(scriptCrt.FilenameOrTag, scriptCrt.Parm1, scriptCrt.Parm2, scriptCrt.Parm3, scriptCrt.Parm4);
                                    #endregion
                                    #region CreatureAttack
                                    // run OnStartCombatTurn script 
                                    com_frm.sf.CombatTarget = char_pt;
                                    com_frm.sf.CombatSource = crt_pt;
                                    com_frm.doScriptBasedOnFilename("attackCreature.cs", "none", "none", "none", "none");
                                    #endregion
                                }
                                else
                                {
                                    logText(crt_pt.Name, Color.LightGray);
                                    logText(" is unconscious...skips turn", Color.Black);
                                    logText(Environment.NewLine, Color.Black);
                                    logText(Environment.NewLine, Color.Black);
                                }
                                #endregion
                            }
                        }
                    }
                    
                    #region onEndCombatTurn
                    // run OnStartCombatTurn script
                    var scriptEndCrt = crt_pt.OnEndCombatTurn;
                    com_frm.doScriptBasedOnFilename(scriptEndCrt.FilenameOrTag, scriptCrt.Parm1, scriptCrt.Parm2, scriptCrt.Parm3, scriptCrt.Parm4);
                    #endregion
                }
                else
                {
                    //logText(crt_pt.Name, Color.LightGray);
                    //logText(" is unconscious or held...skips turn", Color.Black);
                    //logText(Environment.NewLine, Color.Black);
                    //logText(Environment.NewLine, Color.Black);
                }
                */
                #endregion
				// * sinopip, 14.08.14
                doOnDeathScripts();
                
                checkEndEncounter();
                btnContinue.Enabled = true;
                currentMoveOrderIndex++;
                doCreateLabels();
                refreshCharacterPanel();
                refreshMap();
                BeginRound();
            }
            catch (Exception ex)
            {
                //IBMessageBox.Show(com_game, "failed doCreatureTurn: " + ex.ToString());
                com_game.errorLog(ex.ToString());
            }
        }
        
        public void attackCreatureAnimation(Creature crt)
        {
            int attackRowIndex = 1;
            int sleep = 1000 / crt.CharSprite.AttackingFPS;
            //start a for loop based on the number of frames in the attack row
            for (int x = 0; x < crt.CharSprite.AttackingNumberOfFrames; x++)
            {
                com_game.CombatAreaCreatureAnimateRenderAll(crt, x, attackRowIndex);
                //com_game.CombatAreaPcAnimateRenderAll(crt, x, attackRowIndex);
                Thread.Sleep(sleep);
            }
            /*
            int attackRowIndex = 1;
            int sleep = (int)((1000.0d * ((double)crt.CharSprite.AttackingNumberOfFrames / (double)crt.CharSprite.AttackingFPS)) / (double)crt.CharSprite.AttackingNumberOfFrames);
            //start a for loop based on the number of frames in the attack row
            for (int x = 0; x < crt.CharSprite.AttackingNumberOfFrames; x++)
            {
                //draw each frame with erase first
                com_game.spriteEraseCreatureCombatDraw(crt.CombatLocation.X * com_game._squareSize, crt.CombatLocation.Y * com_game._squareSize, crt);
                com_game.spriteCreatureCombatDraw(crt.CombatLocation.X * com_game._squareSize, crt.CombatLocation.Y * com_game._squareSize, crt, x, attackRowIndex, sleep);
            }
            com_game.spriteEraseCreatureCombatDraw(crt.CombatLocation.X * com_game._squareSize, crt.CombatLocation.Y * com_game._squareSize, crt);
            com_game.spriteCreatureCombatDraw(crt.CombatLocation.X * com_game._squareSize, crt.CombatLocation.Y * com_game._squareSize, crt, 0, attackRowIndex, 100);
            com_game.UpdateCombat();
            */
        }        
        public void playCreatureAttackSound(Creature cr)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            try
            {
                if (cr.AttackSound == "none")
                {
                    player.SoundLocation = com_game.mainDirectory + "\\data\\sounds\\soundFX\\punch.wav";
                }
                else if (File.Exists(com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\sounds\\soundFX\\" + cr.AttackSound))
                {
                    player.SoundLocation = com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\sounds\\soundFX\\" + cr.AttackSound;
                }
                else if (File.Exists(com_game.mainDirectory + "\\data\\sounds\\soundFX\\" + cr.AttackSound))
                {
                    player.SoundLocation = com_game.mainDirectory + "\\data\\sounds\\soundFX\\" + cr.AttackSound;
                }
                else
                {
                    player.SoundLocation = com_game.mainDirectory + "\\data\\sounds\\soundFX\\punch.wav";
                }
            }
            catch { }
            player.Play();
            Thread.Sleep(300);
            player.Dispose();
        }
        public void playCreatureAttackSound(Creature cr, string soundFileName)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            try
            {
                if (soundFileName == "none")
                {
                    player.SoundLocation = com_game.mainDirectory + "\\data\\sounds\\soundFX\\punch.wav";
                }
                else if (File.Exists(com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\sounds\\soundFX\\" + soundFileName))
                {
                    player.SoundLocation = com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\sounds\\soundFX\\" + soundFileName;
                }
                else if (File.Exists(com_game.mainDirectory + "\\data\\sounds\\soundFX\\" + soundFileName))
                {
                    player.SoundLocation = com_game.mainDirectory + "\\data\\sounds\\soundFX\\" + soundFileName;
                }
                else
                {
                    player.SoundLocation = com_game.mainDirectory + "\\data\\sounds\\soundFX\\punch.wav";
                }
            }
            catch { }
            player.Play();
            Thread.Sleep(300);
            player.Dispose();
        }
        // * sinopip, 25.12.15
        public void deathCreatureAnimation(Creature crt)
        {
            int deathRowIndex = 3;
            int sleep = 1000 / crt.CharSprite.DeathFPS;
            //start a for loop based on the number of frames in the attack row
            for (int x = 0; x < crt.CharSprite.DeathNumberOfFrames; x++)
            {
                com_game.CombatAreaCreatureAnimateRenderAll(crt, x, deathRowIndex);
                Thread.Sleep(sleep);
            }
        }
        //
        
        #region Helper Functions
        // * sinopip, 14.08.14
        // * do scriptCrtDth of a creature only once per combat,
		// * unless a special healing which must reset the "HasDied" LocalInt to 0
		// * 20.12.14 private to public to allow to call it from ScriptFunctions (as spells need it)
        public void doOnDeathScripts()
        {
        	// * sinopip, 20.12.14
        	System.Media.SoundPlayer player = new System.Media.SoundPlayer();
        	//
            foreach (Creature crtr in com_encounter.EncounterCreatureList.creatures)
            {
            	//if (crtr.HP <= 0) //&& com_frm.sf.GetLocalInt(crtr.Tag, "HasDied")!=1)  //had to remove the getlocalint for the tag, because if its empty it causes error JamesManhattan 9/25/14
            	// * sinopip, 20.12.14
            	// * changed this class constructor to init "HasDied" LocalInts
            	if (crtr.HP <= 0 && com_frm.sf.GetLocalInt(crtr.Tag, "HasDied")!=1)
                {
            		Thread.Sleep(100);
                    com_frm.sf.CombatSource = crtr;
                    var scriptCrtDth = crtr.OnDeath;
                    com_frm.doScriptBasedOnFilename(scriptCrtDth.FilenameOrTag, scriptCrtDth.Parm1, scriptCrtDth.Parm2, scriptCrtDth.Parm3, scriptCrtDth.Parm4);
                    // * sinopip, 20.12.14
        			try {
            			player.SoundLocation = com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\sounds\\soundFX\\" + crtr.OnDeathSound;
        				player.Play();
		            	Thread.Sleep(100);
		            } catch { }
                    // * sinopip, 25.12.14
		            // * default death animation, or from spritsheet
		            if (crtr.CharSprite.DeathNumberOfFrames <= 1)
		            	com_frm.currentCombat.drawEndEffect(crtr.CombatLocation, 0, "generic_death.spt"); // if file doesn't exists, this does nothing
		            else
		            	deathCreatureAnimation(crtr);
		            Thread.Sleep(100);
                    //
                    com_frm.sf.SetLocalInt(crtr.Tag, "HasDied", 1);
                    Thread.Sleep(100);
                }
            }
            foreach (PC chr in com_game.playerList.PCList)
            {
                //if (chr.HP <= 0) // && com_frm.sf.GetLocalInt(chr.Tag, "HasDied") != 1) //had to remove the getlocalint for the tag, because if its empty it causes error JamesManhattan 9/25/14
                // * sinopip, 20.12.14
            	// * changed this class constructor to init "HasDied" LocalInts
            	if (chr.HP <= 0 && com_frm.sf.GetLocalInt(chr.Tag, "HasDied")!=1)
                {
                	Thread.Sleep(100);
                    com_frm.sf.CombatSource = chr;                 
                    var scriptCrtDth = chr.OnDeath;
                    com_frm.doScriptBasedOnFilename(scriptCrtDth.FilenameOrTag, scriptCrtDth.Parm1, scriptCrtDth.Parm2, scriptCrtDth.Parm3, scriptCrtDth.Parm4);
                    // * sinopip, 25.12.14
                    if (chr.CharSprite.DeathNumberOfFrames > 1)
                    	for (int index = 0; index < com_game.playerList.PCList.Count; index++)
                    		if (com_game.playerList.PCList[index].NameWithNotes == chr.NameWithNotes)
                    			deathPCAnimation(chr, index);
                    //
		            com_frm.sf.SetLocalInt(chr.Tag, "HasDied", 1);
                    Thread.Sleep(100);
                }
            }
        }
        private void checkEndEncounter()
        {            
            int foundOneCrtr = 0;
            foreach (Creature crtr in com_encounter.EncounterCreatureList.creatures)
            {
                if (crtr.HP > 0)
                {
                    foundOneCrtr = 1;
                }
            }
            if (foundOneCrtr == 0)
            {
                //IBMessageBox.Show(game, "You have defeated all the creatures");
                int giveEachXP = encounterXP / com_game.playerList.PCList.Count;
                logText(Environment.NewLine, Color.Black);
                logText("Each character receives " + giveEachXP.ToString() + " XP", Color.Purple);
                logText(Environment.NewLine, Color.Black);
                logText(Environment.NewLine, Color.Black);
                foreach (PC givePcXp in com_game.playerList.PCList)
                {
                    givePcXp.XP = givePcXp.XP + giveEachXP;
                }
                // give InventoryList
                if (com_encounter.EncounterInventoryList.Count > 0)
                {
                    logText("The party has found:", Color.Blue);
                    logText(Environment.NewLine, Color.Black);
                    foreach (Item it in com_encounter.EncounterInventoryList)
                    {
                        logText(it.ItemName, Color.Blue);
                        logText(Environment.NewLine, Color.Black);
                        com_game.partyInventoryList.Add(it);
                        com_game.partyInventoryTagList.Add(it.ItemTag);
                    }
                }
                // exit encounter
                endEncounter = true;
                com_encounter.EncounterActive = false;
                cleanUpCreatures();
            }

            int foundOnePc = 0;
            foreach (PC chr in com_game.playerList.PCList)
            {
                if (chr.HP > 0)
                {
                    foundOnePc = 1;
                }
            }
            if (foundOnePc == 0)
            {
                IBMessageBox.Show(com_game, "Your party has been defeated");
                this.Close();
                // your party is wiped out
                // show load/quit screen
            }
        }
        private int findNearestPc()
        {
            int index = -1;
            int farDist = 99;
            int cnt = 0;
            foreach (PC pc in com_game.playerList.PCList)
            {
                if (pc.Status != PC.charStatus.Dead)
                {
                    int dist = calcDistance(com_encounter.EncounterCreatureList.creatures[com_moveOrderList[currentMoveOrderIndex].index].CombatLocation, pc.CombatLocation);
                    if (dist < farDist)
                    {
                        farDist = dist;
                        index = cnt;
                    }
                }
                cnt++;
            }
            return index;
        }
        private void setupMoveOrder()
        {
            com_moveOrderList.Clear();
            int cnt = 0;
            MoveOrder newMoveOrderItem = new MoveOrder();
            foreach (PC chr in com_game.playerList.PCList)
            {
                newMoveOrderItem = new MoveOrder();
                newMoveOrderItem.type = "pc";
                newMoveOrderItem.tag = chr.Tag;
                newMoveOrderItem.index = cnt;
                int dexMod = (chr.Dexterity - 10) / 2;
                newMoveOrderItem.rank = com_game.Random(20) + (dexMod) + Stats.CalcInitiativeBonuses(chr); //JamesManhattan changed to d20
                com_moveOrderList.Add(newMoveOrderItem);
                cnt++;
            }
            cnt = 0;
            foreach (Creature crt in com_encounter.EncounterCreatureList.creatures)
            {
                newMoveOrderItem = new MoveOrder();
                newMoveOrderItem.type = "creature";
                newMoveOrderItem.tag = crt.Tag;
                newMoveOrderItem.index = cnt;
                int dexMod = (crt.Dexterity - 10) / 2;
                newMoveOrderItem.rank = com_game.Random(20) + (dexMod) + crt.InitiativeModifier; //JamesManhattan changed to d20
                com_moveOrderList.Add(newMoveOrderItem);
                cnt++;
            }
            com_moveOrderList = com_moveOrderList.OrderByDescending(x => x.rank).ToList();
        }
        private void resetMoveOrder()
        {
            com_moveOrderList = com_moveOrderList.OrderByDescending(x => x.rank).ToList();
            /*com_moveOrderList.Clear();            
            foreach (MoveOrder mo in originalMoveOrderList)
            {
                MoveOrder newMoveOrderItem = new MoveOrder();
                newMoveOrderItem.type = mo.type;
                newMoveOrderItem.index = mo.index;
                newMoveOrderItem.rank = mo.rank;
                com_moveOrderList.Add(newMoveOrderItem);
            }*/
        }
        private void cleanUpCreatures()
        {
            int MoCnt = com_moveOrderList.Count;
            for (int i = MoCnt; i > 0; i--)
            {
                if (com_moveOrderList[i-1].type == "creature")
                {
                    Creature crt_pt = com_encounter.EncounterCreatureList.creatures[com_moveOrderList[i-1].index];
                    crt_pt.passRefs(com_game, null);
                    if (crt_pt.HP <= 0)
                    {
                        //logText("remove: " + crt_pt.Name + " at moveOrder index " + com_moveOrderList[i - 1].rank + " and crt index " + com_moveOrderList[i - 1].index, Color.Black);
                        //logText(Environment.NewLine, Color.Black);
                        com_moveOrderList.RemoveAt(i-1);
                    }
                }
            }
            for (int i = com_encounter.EncounterCreatureList.creatures.Count; i > 0; i--)
            {
                if (com_encounter.EncounterCreatureList.creatures[i - 1].HP <= 0)
                {
                    //logText("remove: " + com_encounter.EncounterCreatureList.creatures[i - 1].Name, Color.Black);
                    //logText(Environment.NewLine, Color.Black);
                    com_encounter.EncounterCreatureList.creatures.RemoveAt(i - 1);
                }
            }
            foreach (MoveOrder mo in com_moveOrderList)
            {
                if (mo.type == "creature")
                {
                    for (int x = 0; x < com_encounter.EncounterCreatureList.creatures.Count; x++)
                    {
                        if (mo.tag == com_encounter.EncounterCreatureList.creatures[x].Tag)
                        {
                            mo.index = x;
                        }
                    }
                }
            }
        }
        public void logText(string text, Color color)
        {
            rtxtLog.SelectionColor = color;
            rtxtLog.AppendText(text);
            rtxtLog.ScrollToCaret();
        }        
        private bool checkCollision()
        {
            bumpedIntoCreatureIndex = -1;
            PC char_pt = new PC();
            char_pt.passRefs(com_game, null);
            //Creature crt_pt = new Creature();
            //crt_pt.passRefs(com_game, null);
            //bool foundOne = false;

            char_pt = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];
            foreach (PC pcChk in com_game.playerList.PCList)
            {
                if ((char_pt.Name != pcChk.Name) && (char_pt.CombatLocation == pcChk.CombatLocation) && (pcChk.HP > 0))
                {
                    //foundOne = true;
                    return true;
                }
            }
            foreach (Prop prp in com_encounter.EncounterPropList.propsList)
            {
                if ((char_pt.CombatLocation == prp.Location) && (prp.HasCollision))
                {
                    //foundOne = true;
                    return true;
                }
            }
            int crIndex = 0;
            foreach (Creature crtChk in com_encounter.EncounterCreatureList.creatures)
            {
                if (crtChk.Size == 1)
                {
                    if ((char_pt.CombatLocation == crtChk.CombatLocation) && (crtChk.HP > 0))
                    {
                        //foundOne = true;
                        bumpedIntoCreatureIndex = crIndex;
                        return true;
                    }
                }
                if (crtChk.Size == 2)
                {
                    int cx = crtChk.CombatLocation.X;
                    int cy = crtChk.CombatLocation.Y;
                    if (crtChk.HP > 0)
                    {
                        if    ((char_pt.CombatLocation == new Point(cx + 0, cy + 0))
                            || (char_pt.CombatLocation == new Point(cx + 1, cy + 0))
                            || (char_pt.CombatLocation == new Point(cx + 0, cy + 1))
                            || (char_pt.CombatLocation == new Point(cx + 1, cy + 1)))
                        {
                            //foundOne = true;
                            bumpedIntoCreatureIndex = crIndex;
                            return true;
                        }
                    }
                }
                if (crtChk.Size == 3)
                {
                    int cx = crtChk.CombatLocation.X;
                    int cy = crtChk.CombatLocation.Y;
                    if (crtChk.HP > 0)
                    {
                        if    ((char_pt.CombatLocation == new Point(cx + 0, cy + 0))
                            || (char_pt.CombatLocation == new Point(cx + 1, cy + 0))
                            || (char_pt.CombatLocation == new Point(cx + 2, cy + 0))
                            || (char_pt.CombatLocation == new Point(cx + 0, cy + 1))
                            || (char_pt.CombatLocation == new Point(cx + 2, cy + 1))
                            || (char_pt.CombatLocation == new Point(cx + 0, cy + 2))
                            || (char_pt.CombatLocation == new Point(cx + 1, cy + 2))
                            || (char_pt.CombatLocation == new Point(cx + 2, cy + 2)))
                        {
                            //foundOne = true;
                            bumpedIntoCreatureIndex = crIndex;
                            return true;
                        }
                    }
                }
                crIndex++;
            }
            if (com_game.currentCombatArea.getCombatTile(char_pt.CombatLocation.X, char_pt.CombatLocation.Y).collidable == true)
            {
                //foundOne = true;
                return true;
            }
            return false;
            //if (foundOne) { return true;  }
            //else          { return false; }
        }
        public bool IsVisibleLineOfSight(Point s, Point e)
        {
            // Bresenham Line algorithm
            // Creates a line from Begin to End starting at (x0,y0) and ending at (x1,y1)
            // where x0 less than x1 and y0 less than y1
            // AND line is less steep than it is wide (dx less than dy)    
            Point start = new Point((s.X * com_game._squareSize) + (com_game._squareSize / 2), (s.Y * com_game._squareSize) + (com_game._squareSize / 2));
            Point end = new Point((e.X * com_game._squareSize) + (com_game._squareSize / 2), (e.Y * com_game._squareSize) + (com_game._squareSize / 2));
            int deltax = Math.Abs(end.X - start.X);
            int deltay = Math.Abs(end.Y - start.Y);
            int ystep = 10;
            int xstep = 10;

            #region low angle version
            if (deltax > deltay) //Low Angle line
            {
                Point nextPoint = start;
                int error = deltax / 2;

                if (end.Y < start.Y) { ystep = -1 * ystep; } //down and right or left

                if (end.X > start.X) //down and right
                {
                    for (int x = start.X; x <= end.X; x += xstep)
                    {
                        nextPoint.X = x;
                        error -= deltay;
                        if (error < 0)
                        {
                            nextPoint.Y += ystep;
                            error += deltax;
                        }
                        //do your checks here for LoS blocking
                        gridx = nextPoint.X / com_game._squareSize;
                        gridy = nextPoint.Y / com_game._squareSize;
                        if (com_game.currentCombatArea.TileMapList[gridy * com_game.currentCombatArea.MapSizeInSquares.Width + gridx].LoSBlocked)
                        {
//                            Pen pen = new Pen(Color.Blue, 1);
//                            com_game.DeviceCombat.DrawLine(pen, start, nextPoint);
//                            pen.Dispose();
//                            com_game.UpdateCombat();
                            return false;
                        }                        
                    }
                }
                else //down and left
                {
                    for (int x = start.X; x >= end.X; x -= xstep)
                    {
                        nextPoint.X = x;
                        error -= deltay;
                        if (error < 0)
                        {
                            nextPoint.Y += ystep;
                            error += deltax;
                        }
                        //do your checks here for LoS blocking
                        gridx = nextPoint.X / com_game._squareSize;
                        gridy = nextPoint.Y / com_game._squareSize;
                        if (com_game.currentCombatArea.TileMapList[gridy * com_game.currentCombatArea.MapSizeInSquares.Width + gridx].LoSBlocked)
                        {
//                            Pen pen = new Pen(Color.Blue, 1);
//                            com_game.DeviceCombat.DrawLine(pen, start, nextPoint);
//                            pen.Dispose();
//                            com_game.UpdateCombat();
                            return false;
                        }                        
                    }
                }
//                Pen pen2 = new Pen(Color.Blue, 1);
//                com_game.DeviceCombat.DrawLine(pen2, start, nextPoint);
//                pen2.Dispose();
//                com_game.UpdateCombat();
            }
            #endregion

            #region steep version
            else //Low Angle line
            {
                Point nextPoint = start;
                int error = deltay / 2;

                if (end.X < start.X) { xstep = -1 * xstep; } //up and right or left

                if (end.Y > start.Y) //up and right
                {
                    for (int y = start.Y; y <= end.Y; y += ystep)
                    {
                        nextPoint.Y = y;
                        error -= deltax;
                        if (error < 0)
                        {
                            nextPoint.X += xstep;
                            error += deltay;
                        }
                        //do your checks here for LoS blocking
                        gridx = nextPoint.X / com_game._squareSize;
                        gridy = nextPoint.Y / com_game._squareSize;
                        if (com_game.currentCombatArea.TileMapList[gridy * com_game.currentCombatArea.MapSizeInSquares.Width + gridx].LoSBlocked)
                        {
//                            Pen pen = new Pen(Color.Blue, 1);
//                            com_game.DeviceCombat.DrawLine(pen, start, nextPoint);
//                            pen.Dispose();
//                            com_game.UpdateCombat();
                            return false;
                        }                        
                    }
                }
                else //up and right
                {
                    for (int y = start.Y; y >= end.Y; y -= ystep)
                    {
                        nextPoint.Y = y;
                        error -= deltax;
                        if (error < 0)
                        {
                            nextPoint.X += xstep;
                            error += deltay;
                        }
                        //do your checks here for LoS blocking
                        gridx = nextPoint.X / com_game._squareSize;
                        gridy = nextPoint.Y / com_game._squareSize;
                        if (com_game.currentCombatArea.TileMapList[gridy * com_game.currentCombatArea.MapSizeInSquares.Width + gridx].LoSBlocked)
                        {
//                            Pen pen = new Pen(Color.Blue, 1);
//                            com_game.DeviceCombat.DrawLine(pen, start, nextPoint);
//                            pen.Dispose();
//                            com_game.UpdateCombat();
                            return false;
                        }                        
                    }
                }
//                Pen pen2 = new Pen(Color.Blue, 1);
//                com_game.DeviceCombat.DrawLine(pen2, start, nextPoint);
//                pen2.Dispose();
//                com_game.UpdateCombat();
            }
            #endregion

            return true;
        }
        public int calcDistance(Point locCr, Point locPc)
        {
            int dist = 0;
            int deltaX = (int)Math.Abs((locCr.X - locPc.X));
            int deltaY = (int)Math.Abs((locCr.Y - locPc.Y));
            if (deltaX > deltaY)
                dist = deltaX;
            else
                dist = deltaY;
            return dist;
        }
        #endregion

        public void doTrigger(PC currentPC)
        {
            try
            {
                com_frm.sf.CombatSource = currentPC;
                Trigger trig = com_game.currentCombatArea.getCombatTriggerByLocation(currentPC.CombatLocation.X, currentPC.CombatLocation.Y);
                if ((trig != null) && (trig.Enabled))
                {
                    if ((trig.EnabledEvent1) && (trig.Parameters1.FilenameOrTag != "none") && (trig.EventType1 == Trigger.TriggerType.Script))
                    {
                       doScriptBasedOnFilename(trig.Parameters1.FilenameOrTag, trig.Parameters1.Parm1, trig.Parameters1.Parm2, trig.Parameters1.Parm3, trig.Parameters1.Parm4);
                    }
                    if ((trig.EnabledEvent2) && (trig.Parameters2.FilenameOrTag != "none") && (trig.EventType2 == Trigger.TriggerType.Script))
                    {
                       doScriptBasedOnFilename(trig.Parameters2.FilenameOrTag, trig.Parameters2.Parm1, trig.Parameters2.Parm2, trig.Parameters2.Parm3, trig.Parameters2.Parm4);
                    }
                    if ((trig.EnabledEvent3) && (trig.Parameters3.FilenameOrTag != "none") && (trig.EventType3 == Trigger.TriggerType.Script))
                    {
                       doScriptBasedOnFilename(trig.Parameters3.FilenameOrTag, trig.Parameters3.Parm1, trig.Parameters3.Parm2, trig.Parameters3.Parm3, trig.Parameters3.Parm4);
                    }
                    if ((trig.EnabledEvent4) && (trig.Parameters4.FilenameOrTag != "none") && (trig.EventType4 == Trigger.TriggerType.Script))
                    {
                       doScriptBasedOnFilename(trig.Parameters4.FilenameOrTag, trig.Parameters4.Parm1, trig.Parameters4.Parm2, trig.Parameters4.Parm3, trig.Parameters4.Parm4);
                    }
                    if ((trig.EnabledEvent5) && (trig.Parameters5.FilenameOrTag != "none") && (trig.EventType5 == Trigger.TriggerType.Script))
                    {
                       doScriptBasedOnFilename(trig.Parameters5.FilenameOrTag, trig.Parameters5.Parm1, trig.Parameters5.Parm2, trig.Parameters5.Parm3, trig.Parameters5.Parm4);
                    }
                    if ((trig.EnabledEvent6) && (trig.Parameters6.FilenameOrTag != "none") && (trig.EventType6 == Trigger.TriggerType.Script))
                    {
                       doScriptBasedOnFilename(trig.Parameters6.FilenameOrTag, trig.Parameters6.Parm1, trig.Parameters6.Parm2, trig.Parameters6.Parm3, trig.Parameters6.Parm4);
                    }
                }
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(com_game, "failed to do trigger: " + ex.ToString());
                com_game.errorLog(ex.ToString());
            }
        }
        public void doPropOnEnter(PC currentPC, Point lastPCLocation)
        {
            try
            {
                foreach (Prop prp in com_game.currentEncounter.EncounterPropList.propsList)
                {
                    if (currentPC.CombatLocation == prp.Location)
                    {
                        com_frm.sf.CombatSource = currentPC;
                        com_frm.sf.lastPcCombatLocation = lastPCLocation;
                        com_frm.sf.passParameterScriptObject = prp.PropTag;
                        var scriptPropOnEnter = prp.OnEnter;
                        doScriptBasedOnFilename(scriptPropOnEnter.FilenameOrTag, scriptPropOnEnter.Parm1, scriptPropOnEnter.Parm2, scriptPropOnEnter.Parm3, scriptPropOnEnter.Parm4);
                        com_frm.sf.passParameterScriptObject = null;
                    }
                }
            }
            catch (Exception ex)
            {
                com_game.errorLog("failed on Prop OnEnter: " + ex.ToString());
            }
        }
        public void doScriptBasedOnFilename(string filename, string p1, string p2, string p3, string p4)
        {
            if (filename != "none")
            {
                try
                {
                    com_game.parm1 = p1;
                    com_game.parm2 = p2;
                    com_game.parm3 = p3;
                    com_game.parm4 = p4;
                    if (File.Exists(com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\scripts\\" + filename))
                    {
                        com_game.executeScript(com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\scripts\\" + filename);
                    }
                    else if (File.Exists(com_game.mainDirectory + "\\data\\scripts\\" + filename))
                    {
                        com_game.executeScript(com_game.mainDirectory + "\\data\\scripts\\" + filename);
                    }
                    else
                    {
                        IBMessageBox.Show(com_game, "couldn't find the script: " + filename);
                    }
                    com_game.parm1 = "";
                    com_game.parm2 = "";
                    com_game.parm3 = "";
                    com_game.parm4 = "";
                }
                catch (Exception ex)
                {
                    IBMessageBox.Show(com_game, "failed to run script");
                    com_game.errorLog(ex.ToString());
                }
            }
        }
        public void applyEffects()
        {
            try
            {
                //maybe reorder all based on their order property            
                foreach (PC pc in com_game.playerList.PCList)
                {
                    foreach (Effect ef in pc.EffectsList.effectsList)
                    {
                        //increment duration of all
                        ef.CurrentDurationInUnits = com_game.module.WorldTime - ef.StartingTimeInUnits;
                        if (!ef.UsedForUpdateStats) //not used for stat updates
                        {                            
                            //do script for each effect
                            com_frm.sf.CombatSource = pc;
                            var scriptCrt = ef.EffectScript;
                            scriptCrt.Parm1 = ef.CurrentDurationInUnits.ToString();
                            scriptCrt.Parm2 = ef.DurationInUnits.ToString();
                            com_frm.doScriptBasedOnFilename(scriptCrt.FilenameOrTag, scriptCrt.Parm1, scriptCrt.Parm2, scriptCrt.Parm3, scriptCrt.Parm4);
                        }
                    }
                }
                foreach (Creature crtr in com_encounter.EncounterCreatureList.creatures)
                {
                    foreach (Effect ef in crtr.EffectsList.effectsList)
                    {
                        //increment duration of all
                        ef.CurrentDurationInUnits = com_game.module.WorldTime - ef.StartingTimeInUnits;
                        if (!ef.UsedForUpdateStats) //not used for stat updates
                        {                            
                            //do script for each effect
                            com_frm.sf.CombatSource = crtr;
                            var scriptCrt = ef.EffectScript;
                            scriptCrt.Parm1 = ef.CurrentDurationInUnits.ToString();
                            scriptCrt.Parm2 = ef.DurationInUnits.ToString();
                            com_frm.doScriptBasedOnFilename(scriptCrt.FilenameOrTag, scriptCrt.Parm1, scriptCrt.Parm2, scriptCrt.Parm3, scriptCrt.Parm4);
                        }
                    }
                }
                //if duration equals ending or greater, remove from list
                foreach (PC pc in com_game.playerList.PCList)
                {
                    for (int i = pc.EffectsList.effectsList.Count; i > 0; i--)
                    {
                        if (pc.EffectsList.effectsList[i - 1].CurrentDurationInUnits >= pc.EffectsList.effectsList[i - 1].DurationInUnits)
                        {
                            pc.EffectsList.effectsList.RemoveAt(i - 1);
                        }
                    }
                }
                foreach (Creature crtr in com_encounter.EncounterCreatureList.creatures)
                {
                    for (int i = crtr.EffectsList.effectsList.Count; i > 0; i--)
                    {
                        if (crtr.EffectsList.effectsList[i - 1].CurrentDurationInUnits >= crtr.EffectsList.effectsList[i - 1].DurationInUnits)
                        {
                            crtr.EffectsList.effectsList.RemoveAt(i - 1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(com_game, ex.ToString());
            }
        }

        public void refreshMap()
        {
            
            //com_game.areaCombatUpdate();
            if ((com_moveOrderList.Count > 0) && (currentMoveOrderIndex <= com_moveOrderList.Count - 1))
            {
                drawTurnMarker();
            }
            //drawSprites();
            //com_game.UpdateCombat();
            
        }
        private void doUpdate(PC pc)
        {            
            //currentMoves++; //JamesManhattan 9/25/14 moved this increment to each individual moveUp moveDown etc. so attacking wont mess up moves
            refreshMap();   
            txtInfo.Text = "currentMoves = " + currentMoves.ToString();
            int moveleft = pc.MoveDistance - currentMoves;
            if (moveleft < 0) { moveleft = 0; }
            lblMovesLeft.Text = moveleft.ToString();
            //doOnDeathScripts(); //JamesManhattan Added this here, since its better to be called here rather than in doPCTurn() 9/25/14 - might slow it down to be doing this after every move!
            //checkEndEncounter(); //JamesManhattan added this 9/25/14 - this also cleans up creatures - might slow it down to be doing this after every move!
            //doCreateLabels(); //JamesManhattan moved these up out of the if statement below 9/25/14
            //refreshCharacterPanel(); //JamesManhattan moved these up out of the if statement below 9/25/14
            //PC pc = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];
            //if (currentMoves >= 5)
            //if (currentMoves >= pc.MoveDistance) //Added this line and below JamesManhattan 9/25/14
            //{ 
                   //canMove = false;
            //       btnContinue.Enabled = true;
           //        doCreateLabels(); //JamesManhattan moved these up out of the if statement below 9/25/14
            //       refreshCharacterPanel(); //JamesManhattan moved these up out of the if statement below 9/25/14
            //}  
            if (usedAction < 1 && currentMoves > 0) //they started moving before taking an action, mark it
            {
                startedMoving = 1;
            }
            if (com_frm.sf.GetGlobalInt("rsMoveAttackMove") != 1) //JamesManhattan check for globalint rules options such as can players move, attack, then finish moving.
            {
                if ((usedAction >= 1 && midAttack != 1) && (startedMoving > 0)) //they already moved some, and used their action and aren't mid attack, so mark all their movement as used.
                {
                    //currentMoves = pc.MoveDistance; 
                    currentMoves = 99;
                }

            }
            if ((currentMoves >= pc.MoveDistance) && (numAttacks <= 0 || (usedAction >= 1 && midAttack != 1))) //checks to see if both used action/attacks and all movement JamesManhattan 9/25/14 
            {
                //canMove = false; //Commented out, see above JamesManhattan
                btnContinue.Enabled = true;
                currentMoveOrderIndex++;
                if (currentMoveOrderIndex > com_moveOrderList.Count - 1) 
                {
                    currentMoveOrderIndex = 0;
                }
                doOnDeathScripts(); //JamesManhattan Added this here,   since its better to be called here rather than in doPCTurn() 9/25/14
                doCreateLabels(); //JamesManhattan moved these out of the if statement above 9/25/14
                refreshCharacterPanel();  //JamesManhattan moved these out of the if statement above 9/25/14
                currentMoves = 0;
                numSpellAttacks = 1; //JamesManhattan
                usedAction = 0; //JamesManhattan
                BeginRound();
            }
        }
        private void drawTurnMarker()
        {
            if (com_moveOrderList[currentMoveOrderIndex].type == "pc")
            {
                PC pc = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];
                if (pc.MoveDistance == 0) { pc.MoveDistance = 5; }
                com_game.currentPcTurn = pc;
                com_game.currentPcTurnMovesMade = currentMoves;
//                com_game.drawMoveToSquares(pc, pc.MoveDistance - currentMoves);
//                com_game.drawAttackRangeSquares(pc, pc.MainHand.ItemAttackRange);
//                com_game.spriteCombatTurnSelectorDraw(pc.CombatLocation.X * com_game._squareSize, com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index].CombatLocation.Y * com_game._squareSize);
            }
        }
        private void drawSprites()
        {
            /*
            int cnt = 0;
            foreach (PC pc in com_game.playerList.PCList)
            {
                com_game.spritePcCombatDraw(pc.CombatLocation.X * com_game._squareSize, pc.CombatLocation.Y * com_game._squareSize, cnt);
                cnt++;
            }
            */
        }
        private void refreshCharacterPanel()
        {
            gbCharacters.Controls.Clear();

            int PCcount = 0;
            foreach (PC pc in com_game.playerList.PCList)
            {
                Panel newPanel = new Panel();
                newPanel.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
                newPanel.BackgroundImage = (Image)pc.portraitBitmapM;
                newPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
                newPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
                newPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.PcPanel_Paint);
                int x = 8 + (PCcount * 75);
                if (PCcount > 2)
                {
                    x = 8 + ((PCcount - 3) * 75);
                }
                int y = 28 + ((PCcount / 3) * 108);
                newPanel.Location = new System.Drawing.Point(x, y);
                newPanel.Name = "PCpanel" + PCcount.ToString();
                newPanel.Size = new System.Drawing.Size(68, 104); //used to be 36x58 -> 40x62
                gbCharacters.Controls.Add(newPanel);
                newPanel.Invalidate();
                PCcount++;
            }
        }        
        private void doCreateLabels()
        {
            int row = 50;
            int cnt = 0;
            gbCreatures.Controls.Clear();

            #region Set up column headings
            Label label2 = new Label();
            Label label1 = new Label();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = com_game.module.ModuleTheme.ModuleFont;
            label1.Font = new Font(label1.Font, FontStyle.Underline);
            label1.Location = new System.Drawing.Point(6, 30);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(104, 17);
            label1.TabIndex = 0;
            label1.Text = "Creature/PC Name";
            gbCreatures.Controls.Add(label1);
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = com_game.module.ModuleTheme.ModuleFont;
            label2.Font = new Font(label2.Font, FontStyle.Underline);
            label2.Location = new System.Drawing.Point(200, 30);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(27, 17);
            label2.TabIndex = 1;
            label2.Text = "init";
            gbCreatures.Controls.Add(label2);            
            #endregion

            #region Fill in MoveOrder List Box
            int count = 0;
            foreach (MoveOrder mo in com_moveOrderList)
            {                
                try
                {
                    if (com_moveOrderList[count].type == "pc")
                    {
                        PC char_pt = com_game.playerList.PCList[com_moveOrderList[count].index];
                        char_pt.passRefs(com_game, null);
                        Label newLinkLabel = new Label();
                        newLinkLabel.AutoSize = true;
                        if (currentMoveOrderIndex == count)
                        {
                            newLinkLabel.ForeColor = System.Drawing.Color.Lime;
                        }
                        else
                        {
                            newLinkLabel.ForeColor = System.Drawing.Color.Black;
                        }
                        newLinkLabel.Location = new System.Drawing.Point(6, row);
                        newLinkLabel.Text = char_pt.Name;
                        gbCreatures.Controls.Add(newLinkLabel);
                        Label newLabel = new Label();
                        newLabel.AutoSize = true;
                        if (currentMoveOrderIndex == count)
                        {
                            newLabel.ForeColor = System.Drawing.Color.Lime;
                        }
                        else
                        {
                            newLabel.ForeColor = System.Drawing.Color.Black;
                        }
                        newLabel.Location = new System.Drawing.Point(200, row);
                        newLabel.Text = mo.rank.ToString();
                        gbCreatures.Controls.Add(newLabel);
                        row = row + 16;
                        cnt++;
                    }
                    else
                    {
                        Creature crt_pt = com_encounter.EncounterCreatureList.creatures[com_moveOrderList[count].index];
                        crt_pt.passRefs(com_game, null);
                        Label newLinkLabel = new Label();
                        newLinkLabel.AutoSize = true;
                        if (currentMoveOrderIndex == count)
                        {
                            newLinkLabel.ForeColor = System.Drawing.Color.Lime;
                        }
                        else
                        {
                            newLinkLabel.ForeColor = System.Drawing.Color.Black;
                        }
                        newLinkLabel.Location = new System.Drawing.Point(6, row);
                        newLinkLabel.Text = crt_pt.Name;
                        gbCreatures.Controls.Add(newLinkLabel);
                        Label newLabel = new Label();
                        newLabel.AutoSize = true;
                        if (currentMoveOrderIndex == count)
                        {
                            newLabel.ForeColor = System.Drawing.Color.Lime;
                        }
                        else
                        {
                            newLabel.ForeColor = System.Drawing.Color.Black;
                        }
                        newLabel.Location = new System.Drawing.Point(200, row);
                        newLabel.Text = mo.rank.ToString();
                        gbCreatures.Controls.Add(newLabel);
                        row = row + 16;
                        cnt++;
                    }
                }
                catch (Exception ex)
                {
                    //IBMessageBox.Show(game, "failed to send info: " + ex.ToString());
                    com_game.errorLog(ex.ToString());
                }
                count++;
            }            
            #endregion
        }
        private void PcPanel_Paint(object sender, PaintEventArgs e)
        {
            var pnl = (Panel)sender;
            if (pnl.Name == "PCpanel0")
            {
                PcPortraitStats(e, 0);
            }
            else if (pnl.Name == "PCpanel1")
            {
                PcPortraitStats(e, 1);
            }
            else if (pnl.Name == "PCpanel2")
            {
                PcPortraitStats(e, 2);
            }
            else if (pnl.Name == "PCpanel3")
            {
                PcPortraitStats(e, 3);
            }
            else if (pnl.Name == "PCpanel4")
            {
                PcPortraitStats(e, 4);
            }
            else if (pnl.Name == "PCpanel5")
            {
                PcPortraitStats(e, 5);
            }
            else
            {
                //do nothing, didn't find one
            }
        }
        private void PcPortraitStats(PaintEventArgs e, int index)
        {
            Theme theme = com_game.module.ModuleTheme;
            int cnt = 0;
            foreach (Effect ef in com_game.playerList.PCList[index].EffectsList.effectsList)
            {
                string letter = ef.EffectLetter;
                com_game.DrawTextShadowOutline(e, 15 * cnt + 3, 20, 0, letter, 100, 255,
                                               theme.ModuleFont.FontFamily,
                                               theme.ModuleFont.SizeInPoints * theme.ModuleFontScale,
                                               ef.EffectLetterColor, Color.Black);
                cnt++;
            }
            string text = com_game.playerList.PCList[index].HP + "/" + com_game.playerList.PCList[index].HPMax;
            com_game.DrawTextShadowOutline(e, 3, 83, 0, text, 100, 255, com_game.module.ModuleTheme.ModuleFont.FontFamily,
                                           com_game.module.ModuleTheme.ModuleFont.SizeInPoints * com_game.module.ModuleTheme.ModuleFontScale,
                                           Color.Lime, Color.Black);
            text = com_game.playerList.PCList[index].SP + "/" + com_game.playerList.PCList[index].SPMax;
            com_game.DrawTextShadowOutline(e, 3, 100, 0, text, 100, 255, com_game.module.ModuleTheme.ModuleFont.FontFamily,
                                           com_game.module.ModuleTheme.ModuleFont.SizeInPoints * com_game.module.ModuleTheme.ModuleFontScale,
                                           Color.Yellow, Color.Black);
            // do color borders
            try
            {
                if (currentMoveOrderIndex < com_moveOrderList.Count)
                {
                    if (com_moveOrderList[currentMoveOrderIndex].type == "pc")
                    {

                        if (com_moveOrderList[currentMoveOrderIndex].index == index)
                        {
                            Pen pen = new Pen(Color.Lime, 4);
                            e.Graphics.DrawRectangle(pen, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1);
                            pen.Dispose();
                        }
                        else
                        {
                            Pen pen = new Pen(Color.Black, 4);
                            e.Graphics.DrawRectangle(pen, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1);
                            pen.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                com_game.errorLog(ex.ToString());
            }
        }

        #region Pathfinding
        private void setupPathfindArray(Creature c, PC p)
        {
            for (int x = 0; x <= (com_game.currentCombatArea.MapSizeInSquares.Width - 1); x++)
            {
                for (int y = 0; y <= (com_game.currentCombatArea.MapSizeInSquares.Height - 1); y++)
                {
                    //logText("x=" + x.ToString() + " y=" + y.ToString(), Color.Black);
                    //logText(Environment.NewLine, Color.Black);
                    if (com_game.currentCombatArea.getCombatTile(x, y).collidable == true)
                        _pathfinder.Squares[x, y].ContentCode = SquareContent.Wall;
                    else
                        _pathfinder.Squares[x, y].ContentCode = SquareContent.Empty;
                }
            }
            foreach (Creature crt in com_encounter.EncounterCreatureList.creatures)
            {
                if (crt == c) { continue; }
                if (crt.HP > 0)
                {
                    //TODO if creature size is greater than 1, make all squares walls
                    if (crt.Size == 1)
                    {
                        _pathfinder.Squares[crt.CombatLocation.X, crt.CombatLocation.Y].ContentCode = SquareContent.Wall;
                    }
                    if (crt.Size == 2)
                    {
                        _pathfinder.Squares[crt.CombatLocation.X + 0, crt.CombatLocation.Y + 0].ContentCode = SquareContent.Wall;
                        _pathfinder.Squares[crt.CombatLocation.X + 1, crt.CombatLocation.Y + 0].ContentCode = SquareContent.Wall;
                        _pathfinder.Squares[crt.CombatLocation.X + 0, crt.CombatLocation.Y + 1].ContentCode = SquareContent.Wall;
                        _pathfinder.Squares[crt.CombatLocation.X + 1, crt.CombatLocation.Y + 1].ContentCode = SquareContent.Wall;
                    }
                    if (crt.Size == 3)
                    {
                        _pathfinder.Squares[crt.CombatLocation.X + 0, crt.CombatLocation.Y + 0].ContentCode = SquareContent.Wall;
                        _pathfinder.Squares[crt.CombatLocation.X + 1, crt.CombatLocation.Y + 0].ContentCode = SquareContent.Wall;
                        _pathfinder.Squares[crt.CombatLocation.X + 2, crt.CombatLocation.Y + 0].ContentCode = SquareContent.Wall;
                        _pathfinder.Squares[crt.CombatLocation.X + 0, crt.CombatLocation.Y + 1].ContentCode = SquareContent.Wall;
                        _pathfinder.Squares[crt.CombatLocation.X + 2, crt.CombatLocation.Y + 1].ContentCode = SquareContent.Wall;
                        _pathfinder.Squares[crt.CombatLocation.X + 0, crt.CombatLocation.Y + 2].ContentCode = SquareContent.Wall;
                        _pathfinder.Squares[crt.CombatLocation.X + 1, crt.CombatLocation.Y + 2].ContentCode = SquareContent.Wall;
                        _pathfinder.Squares[crt.CombatLocation.X + 2, crt.CombatLocation.Y + 2].ContentCode = SquareContent.Wall;
                    }
                }
            }
            foreach (Prop prp in com_encounter.EncounterPropList.propsList)
            {
                if (prp.HasCollision)
                {
                    _pathfinder.Squares[prp.Location.X, prp.Location.Y].ContentCode = SquareContent.Wall;
                }
            }
            foreach (PC pc in com_game.playerList.PCList)
            {
                if (pc == p) { continue; }
                if (pc.HP > 0)
                {
                    _pathfinder.Squares[pc.CombatLocation.X, pc.CombatLocation.Y].ContentCode = SquareContent.Wall;
                }
            }
        }
        private void Recalculate(Creature crt)
        {
            _pathfinder.ClearLogic();
            _pathfinder.Pathfind(crt);
            //navigatePath();
            //_pathfinder.DrawBoard(boardControl1);
        }
        private void showSteps()
        {
            Font font = new Font("Arial",20.0f);
            Brush brush = new SolidBrush(Color.White);
            foreach (Point point in Pathfinder.AllSquares(com_game))
            {
                int steps = _pathfinder.Squares[point.X,point.Y].DistanceSteps;
                string stepsString = steps.ToString();
                if (stepsString == "10000") { stepsString = "X"; }
//                com_game.DeviceCombat.DrawString(stepsString, font, brush, new PointF(point.X * com_game._squareSize, point.Y * com_game._squareSize));
            }
//            com_game.UpdateCombat();
        }
        public bool creatureWithinMeleeDistance(Creature crt, PC pc)
        {
            if (crt.Size == 1)
            {
                if (calcDistance(pc.CombatLocation, crt.CombatLocation) == 1)
                {
                    return true;
                }
            }
            else if (crt.Size == 2)
            {
                for (int x = pc.CombatLocation.X - 2; x <= pc.CombatLocation.X + 1; x++)
                {
                    for (int y = pc.CombatLocation.Y - 2; y <= pc.CombatLocation.Y + 1; y++)
                    {
                        if (new Point(x, y) == crt.CombatLocation)
                        {
                            return true;
                        }
                    }
                }
            }
            else if (crt.Size == 3)
            {
                for (int x = pc.CombatLocation.X - 3; x <= pc.CombatLocation.X + 1; x++)
                {
                    for (int y = pc.CombatLocation.Y - 3; y <= pc.CombatLocation.Y + 1; y++)
                    {
                        if (new Point(x, y) == crt.CombatLocation)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public void doCreatureCombatFacing(Creature crt_pt, PC char_pt)
        {
            if ((char_pt.CombatLocation.X == crt_pt.CombatLocation.X) && (char_pt.CombatLocation.Y > crt_pt.CombatLocation.Y))
            { crt_pt.CombatFacing = CharBase.facing.Down; }
            if ((char_pt.CombatLocation.X > crt_pt.CombatLocation.X) && (char_pt.CombatLocation.Y > crt_pt.CombatLocation.Y))
            { crt_pt.CombatFacing = CharBase.facing.DownRight; }
            if ((char_pt.CombatLocation.X < crt_pt.CombatLocation.X) && (char_pt.CombatLocation.Y > crt_pt.CombatLocation.Y))
            { crt_pt.CombatFacing = CharBase.facing.DownLeft; }
            if ((char_pt.CombatLocation.X == crt_pt.CombatLocation.X) && (char_pt.CombatLocation.Y < crt_pt.CombatLocation.Y))
            { crt_pt.CombatFacing = CharBase.facing.Up; }
            if ((char_pt.CombatLocation.X > crt_pt.CombatLocation.X) && (char_pt.CombatLocation.Y < crt_pt.CombatLocation.Y))
            { crt_pt.CombatFacing = CharBase.facing.UpRight; }
            if ((char_pt.CombatLocation.X < crt_pt.CombatLocation.X) && (char_pt.CombatLocation.Y < crt_pt.CombatLocation.Y))
            { crt_pt.CombatFacing = CharBase.facing.UpLeft; }
            if ((char_pt.CombatLocation.X > crt_pt.CombatLocation.X) && (char_pt.CombatLocation.Y == crt_pt.CombatLocation.Y))
            { crt_pt.CombatFacing = CharBase.facing.Right; }
            if ((char_pt.CombatLocation.X < crt_pt.CombatLocation.X) && (char_pt.CombatLocation.Y == crt_pt.CombatLocation.Y))
            { crt_pt.CombatFacing = CharBase.facing.Left; }
        }
        private void setupHeroSquares(PC pc, Creature crt)
        {
            if (crt.Size == 1)
            {
                return;
            }
            if (crt.Size == 2)
            {
                for (int x = pc.CombatLocation.X - 1; x < pc.CombatLocation.X + 1; x++)
                {
                    for (int y = pc.CombatLocation.Y - 1; y < pc.CombatLocation.Y + 1; y++)
                    {
                        if (Pathfinder.ValidCoordinates(x, y, com_game))
                        {
                            _pathfinder.Squares[x, y].ContentCode = SquareContent.Hero;
                        }
                    }
                }
            }
            if (crt.Size == 3)
            {
                for (int x = pc.CombatLocation.X - 2; x < pc.CombatLocation.X + 1; x++)
                {
                    for (int y = pc.CombatLocation.Y - 2; y < pc.CombatLocation.Y + 1; y++)
                    {
                        if (Pathfinder.ValidCoordinates(x, y, com_game))
                        {
                            _pathfinder.Squares[x, y].ContentCode = SquareContent.Hero;
                        }
                    }
                }
            }
        }
        public int navigatePath()
        {
            Creature crt_pt = new Creature();
            crt_pt.passRefs(com_game, null);
            crt_pt = com_encounter.EncounterCreatureList.creatures[com_moveOrderList[currentMoveOrderIndex].index];
            //
            // Mark the path from monster to hero.
            //
            Point startingPoint = _pathfinder.FindCode(SquareContent.Monster);
            int pointX = startingPoint.X;
            int pointY = startingPoint.Y;
            if (pointX == -1 && pointY == -1)
            {
                return 10000;
            }
            int lowest = 10000;
            int creatureMoves = 0;
            while (true)
            {
                /*
                 * 
                 * Look through each direction and find the square
                 * with the lowest number of steps marked.
                 * 
                 * */
                Point lowestPoint = Point.Empty;
                lowest = 10000;

                //iterate through all squares around current location and find the square to move to next
                //lowestPoint is set to the location to move to
                foreach (Point movePoint in _pathfinder.ValidMoves(pointX, pointY, crt_pt))
                {
                    int count = _pathfinder.Squares[movePoint.X, movePoint.Y].DistanceSteps;
                    if (count < lowest)
                    {
                        lowest = count;
                        lowestPoint.X = movePoint.X;
                        lowestPoint.Y = movePoint.Y;
                    }
                }
                if (lowest != 10000) //check to see if next location was found
                {
                    /*
                     * 
                     * Mark the square as part of the path if it is the lowest
                     * number. Set the current position as the square with
                     * that number of steps.
                     * 
                     * */
                    if (_pathfinder.Squares[lowestPoint.X, lowestPoint.Y].ContentCode != SquareContent.Hero)
                    {
                        //logText("lwstPnt.X=" + lowestPoint.X.ToString() + " lwstPnt.Y=" + lowestPoint.Y.ToString(), Color.Black);
                        //logText(Environment.NewLine, Color.Black);
                        if ((lowestPoint.X == crt_pt.CombatLocation.X) && (lowestPoint.Y > crt_pt.CombatLocation.Y))
                            { crt_pt.CombatFacing = CharBase.facing.Down; }
                        if ((lowestPoint.X > crt_pt.CombatLocation.X) && (lowestPoint.Y > crt_pt.CombatLocation.Y))
                            { crt_pt.CombatFacing = CharBase.facing.DownRight; }
                        if ((lowestPoint.X < crt_pt.CombatLocation.X) && (lowestPoint.Y > crt_pt.CombatLocation.Y))
                            { crt_pt.CombatFacing = CharBase.facing.DownLeft; }
                        if ((lowestPoint.X == crt_pt.CombatLocation.X) && (lowestPoint.Y < crt_pt.CombatLocation.Y))
                            { crt_pt.CombatFacing = CharBase.facing.Up; }
                        if ((lowestPoint.X > crt_pt.CombatLocation.X) && (lowestPoint.Y < crt_pt.CombatLocation.Y))
                            { crt_pt.CombatFacing = CharBase.facing.UpRight; }
                        if ((lowestPoint.X < crt_pt.CombatLocation.X) && (lowestPoint.Y < crt_pt.CombatLocation.Y))
                            { crt_pt.CombatFacing = CharBase.facing.UpLeft; }
                        if ((lowestPoint.X > crt_pt.CombatLocation.X) && (lowestPoint.Y == crt_pt.CombatLocation.Y))
                            { crt_pt.CombatFacing = CharBase.facing.Right; }
                        if ((lowestPoint.X < crt_pt.CombatLocation.X) && (lowestPoint.Y == crt_pt.CombatLocation.Y))
                            { crt_pt.CombatFacing = CharBase.facing.Left; }
                        crt_pt.CombatLocation = lowestPoint;
                        //crt_pt.CombatLocation.Y = lowestPoint.Y;
                        com_game.CombatAreaRenderAll();
                        Thread.Sleep(100);
                        creatureMoves++;
                        if (creatureMoves >= crt_pt.MoveDistance)
                            { break; }
                    }
                    //logText(crt_pt.Tag + " lowest: " + lowest.ToString(), Color.Black);
                    //logText(Environment.NewLine, Color.Black);
                    //_pathfinder.Squares[lowestPoint.X, lowestPoint.Y].IsPath = true;
                    pointX = lowestPoint.X;
                    pointY = lowestPoint.Y;
                }
                else
                {
                    //logText(crt_pt.Tag + " lowest: " + lowest.ToString(), Color.Black);
                    //logText(Environment.NewLine, Color.Black);
                    break;
                }

                if (_pathfinder.Squares[pointX, pointY].ContentCode == SquareContent.Hero)
                {
                    /*
                     * 
                     * We went from monster to hero, so we're finished.
                     * 
                     * */
                    break;
                }
            }
            return lowest;
        }
        #endregion

        public void Item1_Click(object sender, EventArgs e)
        {
            var tsItem = (MenuItem)sender;
            var cms = (ContextMenu)tsItem.Parent;

            IBMessageBox.Show(com_game, "you choose Item 1 from " + cms.SourceControl.Name);
        }
        public void Item2_Click(object sender, EventArgs e)
        {
            IBMessageBox.Show(com_game, "you choose Item 2");
        }

        #region PC Turn Stuff
        private void PlayerTurnButtonsOnOff()
        {
            PC char_pt = new PC();
            char_pt.passRefs(com_game, null);
            char_pt = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];
            //RANGED WEAPON
            if (char_pt.MainHand.ItemCategory == Item.category.Ranged || char_pt.MainHand.ItemAttackRange > 1) //some melee items such as daggers should be able to be thrown also. until their is seperate flag, check range
                { btnRangedAttack.Enabled = true; }
            else
                { btnRangedAttack.Enabled = false; }
            //SPELLS
            if (char_pt.KnownSpellsList.spellList.Count > 0)
                { btnUseSpell.Enabled = true; }
            else
                { btnUseSpell.Enabled = false; }
            //TRAITS
            if (char_pt.KnownTraitsList.traitList.Count > 0)
                { btnUseTrait.Enabled = true; }
            else
                { btnUseTrait.Enabled = false; }
        }        
        private void btnDelayTurn_Click(object sender, EventArgs e)
        {
            selectedEndTurn();
        }
        private void selectedEndTurn()
        {
            PC char_pt = new PC();
            char_pt.passRefs(com_game, null);
            char_pt = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];
            // copy the current MoveOrder to a temp, delete the current one then add the temp to the end of the list
            //MoveOrder tempMO = new MoveOrder();
            //tempMO.index = com_moveOrderList[currentMoveOrderIndex].index;
            //tempMO.rank = com_moveOrderList[currentMoveOrderIndex].rank;
            //tempMO.type = com_moveOrderList[currentMoveOrderIndex].type;
            //com_moveOrderList.RemoveAt(currentMoveOrderIndex);
            //com_moveOrderList.Add(tempMO);
            // do end turn stuff
            gbPlayerTurn.Enabled = false;
            spellTargetSelected = false;
            AoEtype = false;
            currentSpellRadius = 0;
            currentSpellRange = 0;
            //currentSpell = MageSpells.mageSpellList.None;
            doOnDeathScripts(); //JamesManhattan added 9/25/14
            checkEndEncounter();
            doCreateLabels();
            refreshCharacterPanel();
            currentMoves = 99;
            numAttacks = 0; //Added this JamesManhattan 9/25/14
            usedAction = 1; //Added this JamesManhattan 9/25/14
            refreshMap();
            doUpdate(char_pt);
        }
        private void btnRangedAttack_Click(object sender, EventArgs e)
        {
            selectedRangedAttack();
            /*  
             * at the beginning of turn, you have the options to move, ranged, feat/spell, item, delay
             * if you start moving, turn off all buttons
             * if ranged is selected, do not allow to move, moving the mouse around will place a selection box on hovered cell
             * if you exceed the range, the selection box goes away
             * the selection box should be the size of the area-of-effect
             * if you click on a box, check to see if there is a creature and it is in range
             * if so then doPCTurn and fire off an animation
             * 
             */ 
        }
        private void selectedRangedAttack()
        {
            rangedItem = true;
            //gbPlayerTurn.Enabled = false; //JamesManhattan I want to be able to do stuff even after making ranged attack. commented out
        }
        private void btnUseItem_Click(object sender, EventArgs e)
        {
            selectedUseItem();
        }
        private void selectedUseItem()
        {
            PC char_pt = new PC();
            char_pt.passRefs(com_game, null);
            char_pt = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];
            int PCindex = com_moveOrderList[currentMoveOrderIndex].index;
            //gbPlayerTurn.Enabled = false; //JamesManhattan
            UseItemCombat uic = new UseItemCombat(com_game, com_frm, PCindex);
            DialogResult result = uic.ShowDialog();
            if (result == DialogResult.Yes)
            {
                //IBMessageBox.Show(game, "You used an item");
                // do end turn stuff
                gbPlayerTurn.Enabled = false;
                spellTargetSelected = false;
                AoEtype = false;
                currentSpellRadius = 0;
                currentSpellRange = 0;
                //currentSpell = MageSpells.mageSpellList.None;
                checkEndEncounter();
                doCreateLabels();
                refreshCharacterPanel();
                currentMoves = 20;
                refreshMap();
                doUpdate(char_pt);
            }
            else if ((result == DialogResult.No) || (result == DialogResult.Cancel))
            {
                //IBMessageBox.Show(game, "You chose to not use an item");
                gbPlayerTurn.Enabled = true;
                return;
            }
            else
            {
                IBMessageBox.Show(com_game, "Error in use item dialog");
            }
        }
        private void btnUseTrait_Click(object sender, EventArgs e)
        {
            selectedUseTrait();
        }
        private void selectedUseTrait()
        {
            PC char_pt = new PC();
            char_pt.passRefs(com_game, null);
            char_pt = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];
            //gbPlayerTurn.Enabled = false; //JamesManhattan want to still do more
            //currentSpell = MageSpells.mageSpellList.None;
            //MageSpellsCombat msc = new MageSpellsCombat(this, com_game, char_pt);
            //msc.ShowDialog();
            TraitSelect tss = new TraitSelect(this, com_game, char_pt);
            DialogResult result = tss.ShowDialog();
            if (result == DialogResult.OK)
            {
                TraitTargeting();
            }
            else if (result == DialogResult.Cancel)
            {
                gbPlayerTurn.Enabled = true;
                logText("cancelled trait use", Color.DarkRed);
                logText(Environment.NewLine, Color.Black);
                return;
            }
            else
            {
                IBMessageBox.Show(com_game, "didn't register your choice...cancelled");
            }
        }
        private void btnUseSkill_Click(object sender, EventArgs e)
        {
            selectedUseSkill();
        }
        private void selectedUseSkill()
        {
            //IBMessageBox.Show(game, "not implemented yet");
            PC char_pt = new PC();
            char_pt.passRefs(com_game, null);
            char_pt = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];
            //gbPlayerTurn.Enabled = false; JamesManhattan want to still do more
            SkillSelect sks = new SkillSelect(this, com_game, char_pt);
            DialogResult result = sks.ShowDialog();
            if (result == DialogResult.OK)
            {
                com_frm.sf.CombatSource = char_pt;
                var script = currentSkill.SkillScript;
                com_frm.doScriptBasedOnFilename(script.FilenameOrTag, script.Parm1, script.Parm2, script.Parm3, script.Parm4);
                checkEndEncounter();
                doCreateLabels();
                refreshCharacterPanel();
                currentMoves = 20;
                refreshMap();
                doUpdate(char_pt);
            }
            else if (result == DialogResult.Cancel)
            {
                gbPlayerTurn.Enabled = true;
                logText("cancelled skill use", Color.DarkRed);
                logText(Environment.NewLine, Color.Black);
                return;
            }
            else
            {
                IBMessageBox.Show(com_game, "didn't register your choice...cancelled");
            }

            
        }
        private void btnUseSpell_Click(object sender, EventArgs e)
        {
            selectedUseSpell();
        }
        private void selectedUseSpell()
        {
            PC char_pt = new PC();
            char_pt.passRefs(com_game, null);
            char_pt = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];
            //gbPlayerTurn.Enabled = false; JamesManhattan want to still do more
            //currentSpell = MageSpells.mageSpellList.None;
            //MageSpellsCombat msc = new MageSpellsCombat(this, com_game, char_pt);
            //msc.ShowDialog();
            SpellSelect tss = new SpellSelect(this, com_game, char_pt);
            DialogResult result = tss.ShowDialog();
            if (result == DialogResult.OK)
            {
                //if (currentSpell.SpellTag == "5E_MagicMissile_X3") { numSpellAttacks = 3; } //JamesManhattan testing spell 9/26/14
                if (currentSpell.SpellTag.Substring(currentSpell.SpellTag.Length - 3, 2) == "_X") //JamesManhattan checks the tag of the spell which I put _X as a multiplier
                {
                    //numSpellAttacks = 3;
                    numSpellAttacks = Int32.Parse(currentSpell.SpellTag.Substring(currentSpell.SpellTag.Length - 1, 1));
                }
                else { numSpellAttacks = 1; } 
                SpellTargeting();      
            }
            else if (result == DialogResult.Cancel)
            {
                gbPlayerTurn.Enabled = true;
                logText("cancelled spell use", Color.DarkRed);
                logText(Environment.NewLine, Color.Black);
                return;
            }
            else
            {
                IBMessageBox.Show(com_game, "didn't register your choice...cancelled");
            }
        }
        /*private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            gridx = e.X / com_game._squareSize;
            gridy = e.Y / com_game._squareSize;
            mousex = e.X;
            mousey = e.Y;
            lblMouseInfo.Text = "CURSOR " + e.X.ToString() + "," + e.Y.ToString() +
                " - GRID " + gridx.ToString() + "," + gridy.ToString();
            if (gridx < 0) { gridx = 0; }
            if (gridy < 0) { gridy = 0; }
            if (gridx > (com_game.currentCombatArea.MapSizeInSquares.Width - 1)) { gridx = (com_game.currentCombatArea.MapSizeInSquares.Width - 1); }
            if (gridy > (com_game.currentCombatArea.MapSizeInSquares.Height - 1)) { gridy = (com_game.currentCombatArea.MapSizeInSquares.Height - 1); }

            #region Info Box Stuff
            bool foundSomething = false;
            foreach (Creature crt in com_encounter.EncounterCreatureList.creatures)
            {
                if ((gridx == crt.CombatLocation.X) && (gridy == crt.CombatLocation.Y))
                {
                    rtxtInfo.Text = crt.Name + Environment.NewLine + "HPs: " + crt.HP;
                    if (okToDrawHighlights)
                    {
                        drawCrtHighlightBoxes(crt);
                        okToDrawHighlights = false;
                    }
                    foundSomething = true;
                }
            }
            foreach (PC pc in com_game.playerList.PCList)
            {
                if ((gridx == pc.CombatLocation.X) && (gridy == pc.CombatLocation.Y))
                {
                    rtxtInfo.Text = pc.Name + Environment.NewLine +
                                    "HP: " + pc.HP +
                                    "   SP: " + pc.SP + Environment.NewLine +
                                    "Status: " + pc.Status.ToString() + Environment.NewLine +
                                    "Weapon: " + pc.MainHand.ItemName;
                    foundSomething = true;
                }
            }
            if (!foundSomething)
            {
                rtxtInfo.Text = "";
                if (!okToDrawHighlights)
                { 
                    refreshMap();
                    okToDrawHighlights = true;
                }

            }
            #endregion

            if (rangedItem)
            {
                PC char_pt = new PC();
                char_pt.passRefs(com_game, null);
                char_pt = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];
                Point selectedPoint = new Point(gridx, gridy);
                int dist = calcDistance(selectedPoint, char_pt.CombatLocation);
                if (char_pt.MainHand.ItemAttackRange >= dist) // check to see if selected is in range
                {                    
                    refreshMap();
                    IsVisibleLineOfSight(char_pt.CombatLocation, selectedPoint);
                    com_game.drawSelectionBox(gridx, gridy, 0);
                }
            }

            if (rangedSpell)
            {
                PC char_pt = new PC();
                char_pt.passRefs(com_game, null);
                char_pt = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];
                Point selectedPoint = new Point(gridx, gridy);
                int dist = calcDistance(selectedPoint, char_pt.CombatLocation);
                if (currentSpell.Range >= dist) // check to see if selected is in range
                {
                    refreshMap();
                    IsVisibleLineOfSight(char_pt.CombatLocation, selectedPoint);
                    com_game.drawSelectionBox(gridx, gridy, currentSpell.AoeRadiusOrLength);
                }
            }

            if (rangedTrait)
            {
                PC char_pt = new PC();
                char_pt.passRefs(com_game, null);
                char_pt = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];
                Point selectedPoint = new Point(gridx, gridy);
                int dist = calcDistance(selectedPoint, char_pt.CombatLocation);
                if (currentTrait.Range >= dist) // check to see if selected is in range
                {
                    refreshMap();
                    IsVisibleLineOfSight(char_pt.CombatLocation, selectedPoint);
                    com_game.drawSelectionBox(gridx, gridy, currentSpell.AoeRadiusOrLength);
                }
            }
        }
        */
        private void drawCrtHighlightBoxes(Creature crt)
        {            
            //com_game.drawCrtMoveToSquares(crt, crt.MoveDistance);
            //com_game.drawCrtAttackRangeSquares(crt, crt.AttackRange);
            //drawSprites();
            //com_game.UpdateCombat();
        }
        /*private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            gridx = e.X / com_game._squareSize;
            gridy = e.Y / com_game._squareSize;
            Point selectedPoint = new Point(gridx, gridy);

            #region For Ranged Weapons
            if (rangedItem)
            {
                PC char_pt = new PC();
                char_pt.passRefs(com_game, null);
                char_pt = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];
                //setupPcAttackSound(char_pt, char_pt.MainHand.ItemOnUseSound);
                if (e.Button == MouseButtons.Left)
                {
                    //gridx = e.X / com_game._squareSize;
                    //gridy = e.Y / com_game._squareSize;
                    //Point selectedPoint = new Point(gridx, gridy);
                    int dist = calcDistance(selectedPoint, char_pt.CombatLocation);
                    if ((char_pt.MainHand.ItemAttackRange >= dist) && (IsVisibleLineOfSight(char_pt.CombatLocation, selectedPoint))) // check to see if selected is in range
                    {
                        int crIndex = 0;
                        foreach (Creature crt in com_encounter.EncounterCreatureList.creatures)
                        {
                            if (!AoEtype) // if needs a target then check the following (not AoE)
                            {
                                if ((gridx == crt.CombatLocation.X) && (gridy == crt.CombatLocation.Y))
                                {                                    
                                    // check distance first to see if in range
                                    // doPCTurn using ranged weapon
                                    // do attack animation if sprite has animations
                                    if (char_pt.CharSprite.AttackingNumberOfFrames > 1)
                                    {
                                        attackPcAnimation(char_pt, com_moveOrderList[currentMoveOrderIndex].index);
                                    }
                                    //IBMessageBox.Show(game, "you found one");
                                    Point starting = new Point((char_pt.CombatLocation.X * com_game._squareSize) + (com_game._squareSize / 2), (char_pt.CombatLocation.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                    Point ending = new Point((selectedPoint.X * com_game._squareSize) + (com_game._squareSize / 2), (selectedPoint.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                    // add a third parm for projectile type
                                    playPcAttackSound(char_pt, char_pt.MainHand.ItemOnUseSound);
                                    drawProjectile(starting, ending, char_pt.MainHand.ProjectileSpriteFilename);
                                    drawEndEffect(selectedPoint, char_pt.MainHand.ItemAreaOfEffect, char_pt.MainHand.SpriteEndingFilename);
                                    bumpedIntoCreatureIndex = crIndex;
                                    doPcTurn();
                                    rangedItem = false;
                                    gbPlayerTurn.Enabled = false;
                                    refreshMap();
                                    doUpdate();
                                    break;
                                }
                            }
                            else // is an AoE ranged attack
                            {
                                // after picking a location check to see if it is in range
                                // apply effect to all creatures within AoE
                            }
                            crIndex++;
                        }
                    }
                }
                else // right click was selected so exit ranged attack
                {
                    rangedItem = false;
                    gbPlayerTurn.Enabled = true;
                    refreshMap();
                }
            }
            #endregion

            #region For Ranged Spells
            if (rangedSpell)
            {
                PC char_pt = new PC();
                char_pt.passRefs(com_game, null);
                char_pt = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];
                
                if (e.Button == MouseButtons.Left)
                {
                    //gridx = e.X / com_game._squareSize;
                    //gridy = e.Y / com_game._squareSize;
                    //Point selectedPoint = new Point(gridx, gridy);
                    int dist = calcDistance(selectedPoint, char_pt.CombatLocation);
                    if ((currentSpell.Range >= dist) && (IsVisibleLineOfSight(char_pt.CombatLocation, selectedPoint)))// check to see if selected is in range
                    {
                        playPcAttackSound(char_pt, currentSpell.SpellStartSound); 
                        if (currentSpell.AoeRadiusOrLength <= 0) // if needs a target then check the following (not AoE)
                        {
                            if (currentSpell.TargetIsPC) // target is a PC
                            {
                                int pcIndex = 0;
                                foreach (PC pc in com_game.playerList.PCList)
                                {
                                    if ((gridx == pc.CombatLocation.X) && (gridy == pc.CombatLocation.Y))
                                    {
                                        rangedSpell = false;
                                        if (char_pt.CharSprite.AttackingNumberOfFrames > 1)
                                        {
                                            attackPcAnimation(char_pt, com_moveOrderList[currentMoveOrderIndex].index);
                                        }
                                        Point starting = new Point((char_pt.CombatLocation.X * com_game._squareSize) + (com_game._squareSize / 2), (char_pt.CombatLocation.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                        Point ending = new Point((selectedPoint.X * com_game._squareSize) + (com_game._squareSize / 2), (selectedPoint.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                        //playPcAttackSound(char_pt, currentSpell.SpellStartSound);  
                                        drawProjectile(starting, ending, currentSpell.SpriteFilename);
                                        drawEndEffect(selectedPoint, currentSpell.AoeRadiusOrLength, currentSpell.SpriteEndingFilename);
                                        bumpedIntoCreatureIndex = pcIndex;
                                        com_frm.sf.CombatSource = char_pt;
                                        com_frm.sf.CombatTarget = pc;
                                        SpellExecute(char_pt, null, pc);
                                        gbPlayerTurn.Enabled = false;
                                        spellTargetSelected = false;
                                        DecreaseSpellPoints(currentSpell.CostSP, char_pt);
                                        checkEndEncounter();
                                        doCreateLabels();
                                        refreshCharacterPanel();
                                        currentMoves = 10;
                                        refreshMap();
                                        doUpdate();
                                        break;
                                    }
                                    pcIndex++;
                                }
                            }
                            else // target is a creature
                            {
                                int crIndex = 0;
                                foreach (Creature crt in com_encounter.EncounterCreatureList.creatures)
                                {
                                    if ((gridx == crt.CombatLocation.X) && (gridy == crt.CombatLocation.Y))
                                    {
                                        rangedSpell = false;
                                        if (char_pt.CharSprite.AttackingNumberOfFrames > 1)
                                        {
                                            attackPcAnimation(char_pt, com_moveOrderList[currentMoveOrderIndex].index);
                                        }
                                        Point starting = new Point((char_pt.CombatLocation.X * com_game._squareSize) + (com_game._squareSize / 2), (char_pt.CombatLocation.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                        Point ending = new Point((selectedPoint.X * com_game._squareSize) + (com_game._squareSize / 2), (selectedPoint.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                        //playPcAttackSound(char_pt, currentSpell.SpellStartSound);  
                                        drawProjectile(starting, ending, currentSpell.SpriteFilename);
                                        playPcAttackSound(char_pt, currentSpell.SpellEndSound);
                                        drawEndEffect(selectedPoint, currentSpell.AoeRadiusOrLength, currentSpell.SpriteEndingFilename);
                                        bumpedIntoCreatureIndex = crIndex;
                                        com_frm.sf.CombatSource = char_pt;
                                        com_frm.sf.CombatTarget = crt;
                                        SpellExecute(char_pt, crt, null);
                                        gbPlayerTurn.Enabled = false;
                                        spellTargetSelected = false;
                                        DecreaseSpellPoints(currentSpell.CostSP, char_pt);
                                        checkEndEncounter();
                                        doCreateLabels();
                                        refreshCharacterPanel();
                                        currentMoves = 10;
                                        refreshMap();
                                        doUpdate();
                                        break;
                                    }
                                    crIndex++;
                                }
                            }
                        }
                        else // is an AoE ranged attack
                        {
                            rangedSpell = false;
                            if (char_pt.CharSprite.AttackingNumberOfFrames > 1)
                            {
                                attackPcAnimation(char_pt, com_moveOrderList[currentMoveOrderIndex].index);
                            }
                            Point starting = new Point((char_pt.CombatLocation.X * com_game._squareSize) + (com_game._squareSize / 2), (char_pt.CombatLocation.Y * com_game._squareSize) + (com_game._squareSize / 2));
                            Point ending = new Point((selectedPoint.X * com_game._squareSize) + (com_game._squareSize / 2), (selectedPoint.Y * com_game._squareSize) + (com_game._squareSize / 2));
                            //playPcAttackSound(char_pt, currentSpell.SpellStartSound); 
                            drawProjectile(starting, ending, currentSpell.SpriteFilename);
                            playPcAttackSound(char_pt, currentSpell.SpellEndSound);
                            drawEndEffect(selectedPoint, currentSpell.AoeRadiusOrLength, currentSpell.SpriteEndingFilename);
                            Point pnt = new Point(selectedPoint.X, selectedPoint.Y);
                            com_frm.sf.CombatSource = char_pt;
                            com_frm.sf.CombatTarget = pnt;
                            SpellExecute(char_pt, null, null);

                            gbPlayerTurn.Enabled = false;
                            spellTargetSelected = false;
                            DecreaseSpellPoints(currentSpell.CostSP, char_pt);
                            checkEndEncounter();
                            doCreateLabels();
                            refreshCharacterPanel();
                            currentMoves = 10;
                            refreshMap();
                            doUpdate();
                        }
                    }
                }
                else // right click was selected so exit ranged spell
                {
                    rangedSpell = false;
                    gbPlayerTurn.Enabled = true;
                    spellTargetSelected = false;
                    //AoEtype = false;
                    //currentSpellRadius = 0;
                    //currentSpellRange = 0;
                    //currentSpell = MageSpells.mageSpellList.None;
                    logText("Cancelled Spell Use", Color.DarkRed);
                    logText(Environment.NewLine, Color.Black);
                    refreshMap();
                }
            }
            #endregion

            #region For Ranged Traits
            if (rangedTrait)
            {
                PC char_pt = new PC();
                char_pt.passRefs(com_game, null);
                char_pt = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];
                
                if (e.Button == MouseButtons.Left)
                {
                    //gridx = e.X / com_game._squareSize;
                    //gridy = e.Y / com_game._squareSize;
                    //Point selectedPoint = new Point(gridx, gridy);
                    int dist = calcDistance(selectedPoint, char_pt.CombatLocation);
                    if ((currentTrait.Range >= dist)  && (IsVisibleLineOfSight(char_pt.CombatLocation, selectedPoint)))// check to see if selected is in range
                    {
                        playPcAttackSound(char_pt, currentTrait.TraitStartSound); 

                        if (currentTrait.AoeRadiusOrLength <= 0) // if needs a target then check the following (not AoE)
                        {
                            if (currentTrait.TargetIsPC) // target is a PC
                            {
                                int pcIndex = 0;
                                foreach (PC pc in com_game.playerList.PCList)
                                {
                                    if ((gridx == pc.CombatLocation.X) && (gridy == pc.CombatLocation.Y))
                                    {
                                        rangedTrait = false;
                                        if (char_pt.CharSprite.AttackingNumberOfFrames > 1)
                                        {
                                            attackPcAnimation(char_pt, com_moveOrderList[currentMoveOrderIndex].index);
                                        }
                                        Point starting = new Point((char_pt.CombatLocation.X * com_game._squareSize) + (com_game._squareSize / 2), (char_pt.CombatLocation.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                        Point ending = new Point((selectedPoint.X * com_game._squareSize) + (com_game._squareSize / 2), (selectedPoint.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                        //playPcAttackSound(char_pt, currentTrait.TraitStartSound); 
                                        drawProjectile(starting, ending, currentTrait.SpriteFilename);
                                        drawEndEffect(selectedPoint, currentTrait.AoeRadiusOrLength, currentTrait.SpriteEndingFilename);
                                        bumpedIntoCreatureIndex = pcIndex;
                                        com_frm.sf.CombatSource = char_pt;
                                        com_frm.sf.CombatTarget = pc;
                                        TraitExecute(char_pt, null, pc);
                                        gbPlayerTurn.Enabled = false;
                                        DecreaseSpellPoints(currentTrait.CostSP, char_pt);
                                        checkEndEncounter();
                                        doCreateLabels();
                                        refreshCharacterPanel();
                                        currentMoves = 10;
                                        refreshMap();
                                        doUpdate();
                                        break;
                                    }
                                    pcIndex++;
                                }
                            }
                            else // target is a creature
                            {
                                int crIndex = 0;
                                foreach (Creature crt in com_encounter.EncounterCreatureList.creatures)
                                {
                                    if ((gridx == crt.CombatLocation.X) && (gridy == crt.CombatLocation.Y))
                                    {
                                        rangedTrait = false;
                                        if (char_pt.CharSprite.AttackingNumberOfFrames > 1)
                                        {
                                            attackPcAnimation(char_pt, com_moveOrderList[currentMoveOrderIndex].index);
                                        }
                                        Point starting = new Point((char_pt.CombatLocation.X * com_game._squareSize) + (com_game._squareSize / 2), (char_pt.CombatLocation.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                        Point ending = new Point((selectedPoint.X * com_game._squareSize) + (com_game._squareSize / 2), (selectedPoint.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                        //playPcAttackSound(char_pt, currentTrait.TraitStartSound); 
                                        drawProjectile(starting, ending, currentTrait.SpriteFilename);
                                        drawEndEffect(selectedPoint, currentTrait.AoeRadiusOrLength, currentTrait.SpriteEndingFilename);
                                        bumpedIntoCreatureIndex = crIndex;
                                        com_frm.sf.CombatSource = char_pt;
                                        com_frm.sf.CombatTarget = crt;
                                        TraitExecute(char_pt, crt, null);
                                        gbPlayerTurn.Enabled = false;
                                        DecreaseSpellPoints(currentTrait.CostSP, char_pt);
                                        checkEndEncounter();
                                        doCreateLabels();
                                        refreshCharacterPanel();
                                        currentMoves = 10;
                                        refreshMap();
                                        doUpdate();
                                        break;
                                    }
                                    crIndex++;
                                }
                            }
                        }
                        else // is an AoE ranged attack
                        {
                            rangedTrait = false;
                            if (char_pt.CharSprite.AttackingNumberOfFrames > 1)
                            {
                                attackPcAnimation(char_pt, com_moveOrderList[currentMoveOrderIndex].index);
                            }
                            Point starting = new Point((char_pt.CombatLocation.X * com_game._squareSize) + (com_game._squareSize / 2), (char_pt.CombatLocation.Y * com_game._squareSize) + (com_game._squareSize / 2));
                            Point ending = new Point((selectedPoint.X * com_game._squareSize) + (com_game._squareSize / 2), (selectedPoint.Y * com_game._squareSize) + (com_game._squareSize / 2));
                            //playPcAttackSound(char_pt, currentTrait.TraitStartSound); 
                            drawProjectile(starting, ending, currentTrait.SpriteFilename);
                            drawEndEffect(selectedPoint, currentTrait.AoeRadiusOrLength, currentTrait.SpriteEndingFilename);
                            int crIndex = 0;
                            Point pnt = new Point(gridx, gridy);
                            com_frm.sf.CombatSource = char_pt;
                            com_frm.sf.CombatTarget = pnt;
                            foreach (Creature crt in com_encounter.EncounterCreatureList.creatures)
                            {
                                // if in range of radius of x and radius of y
                                if ((crt.CombatLocation.X >= gridx - currentTrait.AoeRadiusOrLength) && (crt.CombatLocation.X <= gridx + currentTrait.AoeRadiusOrLength))
                                {
                                    if ((crt.CombatLocation.Y >= gridy - currentTrait.AoeRadiusOrLength) && (crt.CombatLocation.Y <= gridy + currentTrait.AoeRadiusOrLength))
                                    {
                                        bumpedIntoCreatureIndex = crIndex;
                                        TraitExecute(char_pt, crt, null);
                                    }
                                }
                                crIndex++;
                            }
                            gbPlayerTurn.Enabled = false;
                            DecreaseSpellPoints(currentTrait.CostSP, char_pt);
                            checkEndEncounter();
                            doCreateLabels();
                            refreshCharacterPanel();
                            currentMoves = 10;
                            refreshMap();
                            doUpdate();
                        }
                    }
                }
                else // right click was selected so exit ranged trait
                {
                    rangedTrait = false;
                    gbPlayerTurn.Enabled = true;
                    logText("Cancelled Trait Use", Color.DarkRed);
                    logText(Environment.NewLine, Color.Black);
                    refreshMap();
                }
            }
            #endregion
        }*/
        private void combatRenderPanel_MouseDown(object sender, MouseEventArgs e)
        {
            
            gridx = e.X / com_game._squareSize;
            gridy = e.Y / com_game._squareSize;
            Point selectedPoint = new Point(gridx, gridy);

            if (!chkHoverOnly.Checked) //only do if the "Hover Only" button is not checked
            {
                #region For Ranged Weapons
                if (rangedItem)
                {
                    PC char_pt = new PC();
                    char_pt.passRefs(com_game, null);
                    char_pt = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];
                    //setupPcAttackSound(char_pt, char_pt.MainHand.ItemOnUseSound);
                    if (e.Button == MouseButtons.Left)
                    {
                        //gridx = e.X / com_game._squareSize;
                        //gridy = e.Y / com_game._squareSize;
                        //Point selectedPoint = new Point(gridx, gridy);
                        int dist = calcDistance(selectedPoint, char_pt.CombatLocation);
                        if ((char_pt.MainHand.ItemAttackRange >= dist) && (IsVisibleLineOfSight(char_pt.CombatLocation, selectedPoint))) // check to see if selected is in range
                        {
                            int crIndex = 0;
                            foreach (Creature crt in com_encounter.EncounterCreatureList.creatures)
                            {
                                if (!AoEtype) // if needs a target then check the following (not AoE)
                                {
                                    if ((gridx == crt.CombatLocation.X) && (gridy == crt.CombatLocation.Y))
                                    {
                                        // check distance first to see if in range
                                        // doPCTurn using ranged weapon
                                        // do attack animation if sprite has animations

                                        //IBMessageBox.Show(game, "you found one");
                                        Point starting = new Point((char_pt.CombatLocation.X * com_game._squareSize) + (com_game._squareSize / 2), (char_pt.CombatLocation.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                        Point ending = new Point((selectedPoint.X * com_game._squareSize) + (com_game._squareSize / 2), (selectedPoint.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                        if (starting.X > ending.X) { char_pt.CombatFacing = CharBase.facing.Left; }
                                        else { char_pt.CombatFacing = CharBase.facing.Right; }
                                        if (char_pt.CharSprite.AttackingNumberOfFrames > 1)
                                        {
                                            attackPcAnimation(char_pt, com_moveOrderList[currentMoveOrderIndex].index);
                                        }
                                        // add a third parm for projectile type
                                        playPcAttackSound(char_pt, char_pt.MainHand.ItemOnUseSound);
                                        drawProjectile(starting, ending, char_pt.MainHand.ProjectileSpriteFilename);
                                        drawEndEffect(selectedPoint, char_pt.MainHand.ItemAreaOfEffect, char_pt.MainHand.SpriteEndingFilename);
                                        bumpedIntoCreatureIndex = crIndex;
                                        doPcTurn();
                                        rangedItem = false;
                                        //gbPlayerTurn.Enabled = false; //JamesManhattan
                                        gbPlayerTurn.Enabled = true; //JamesManhattan had to turn the buttons back on to allow more attacks
                                        refreshMap();
                                        doUpdate(char_pt);
                                        break;
                                    }
                                }
                                else // is an AoE ranged attack
                                {
                                    // after picking a location check to see if it is in range
                                    // apply effect to all creatures within AoE
                                }
                                crIndex++;
                            }
                        }
                    }
                    else // right click was selected so exit ranged attack
                    {
                        rangedItem = false;
                        gbPlayerTurn.Enabled = true;
                        refreshMap();
                    }
                }
                #endregion

                #region For Ranged Spells
                else if (rangedSpell)
                {
                    PC char_pt = new PC();
                    char_pt.passRefs(com_game, null);
                    char_pt = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];

                    if (e.Button == MouseButtons.Left)
                    {
                        //gridx = e.X / com_game._squareSize;
                        //gridy = e.Y / com_game._squareSize;
                        //Point selectedPoint = new Point(gridx, gridy);
                        int dist = calcDistance(selectedPoint, char_pt.CombatLocation);
                        if ((currentSpell.Range >= dist) && (IsVisibleLineOfSight(char_pt.CombatLocation, selectedPoint)))// check to see if selected is in range
                        {
                            playPcAttackSound(char_pt, currentSpell.SpellStartSound);
                            // check to see if is AoE or Point Target else needs a target PC or Creature
                            //if ((currentSpell.AoeRadiusOrLength > 0) || (currentSpell.TargetIsPointLocation))
                            if ((currentSpell.AoeRadiusOrLength > 0) || (currentSpell.SpellTargetType == TargetType.PointLocation))
                            {
                                if (numSpellAttacks <= 1) //JamesManhattan allow multiple target spells such as each magic missile bolt
                                {
                                    rangedSpell = false;
                                }
                                else
                                {
                                    logText(" Choose another target.", Color.DarkRed);
                                    numSpellAttacks = numSpellAttacks - 1;
                                }

                                Point starting = new Point((char_pt.CombatLocation.X * com_game._squareSize) + (com_game._squareSize / 2), (char_pt.CombatLocation.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                Point ending = new Point((selectedPoint.X * com_game._squareSize) + (com_game._squareSize / 2), (selectedPoint.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                if (starting.X > ending.X) { char_pt.CombatFacing = CharBase.facing.Left; }
                                else { char_pt.CombatFacing = CharBase.facing.Right; }
                                if (char_pt.CharSprite.AttackingNumberOfFrames > 1)
                                {
                                    attackPcAnimation(char_pt, com_moveOrderList[currentMoveOrderIndex].index);
                                }
                                //playPcAttackSound(char_pt, currentSpell.SpellStartSound); 
                                drawProjectile(starting, ending, currentSpell.SpriteFilename);
                                playPcAttackSound(char_pt, currentSpell.SpellEndSound);
                                drawEndEffect(selectedPoint, currentSpell.AoeRadiusOrLength, currentSpell.SpriteEndingFilename);
                                Point pnt = new Point(selectedPoint.X, selectedPoint.Y);
                                com_frm.sf.CombatSource = char_pt;
                                com_frm.sf.CombatTarget = pnt;
                                SpellExecute(char_pt, null, null);

                                //gbPlayerTurn.Enabled = false; //JamesManhattan
                                spellTargetSelected = false;
                                DecreaseSpellPoints(currentSpell.CostSP, char_pt);
                                checkEndEncounter();
                                doCreateLabels();
                                refreshCharacterPanel();
                                //currentMoves = 99; //JamesManhattan
                                usedAction = usedAction + 1; //JamesManhattan
                                refreshMap();
                                doUpdate(char_pt);
                            }
                            else // is not an AoE ranged attack, is a PC or Creature
                            {
                                //if (currentSpell.TargetIsPC) // target is a PC
                                if ((currentSpell.SpellTargetType == TargetType.Friend) || (currentSpell.SpellTargetType == TargetType.Self)) // target is a PC
                                {
                                    int pcIndex = 0;
                                    foreach (PC pc in com_game.playerList.PCList)
                                    {
                                        if ((gridx == pc.CombatLocation.X) && (gridy == pc.CombatLocation.Y))
                                        {
                                            if (numSpellAttacks <= 1) //JamesManhattan allow multiple target spells such as each magic missile bolt
                                            {
                                                rangedSpell = false;
                                            }
                                            else
                                            {
                                                logText(" Choose another target.", Color.DarkRed);
                                                numSpellAttacks = numSpellAttacks - 1;
                                            }

                                            Point starting = new Point((char_pt.CombatLocation.X * com_game._squareSize) + (com_game._squareSize / 2), (char_pt.CombatLocation.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                            Point ending = new Point((selectedPoint.X * com_game._squareSize) + (com_game._squareSize / 2), (selectedPoint.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                            if (starting.X > ending.X) { char_pt.CombatFacing = CharBase.facing.Left; }
                                            else { char_pt.CombatFacing = CharBase.facing.Right; }
                                            if (char_pt.CharSprite.AttackingNumberOfFrames > 1)
                                            {
                                                attackPcAnimation(char_pt, com_moveOrderList[currentMoveOrderIndex].index);
                                            }
                                            //playPcAttackSound(char_pt, currentSpell.SpellStartSound);  
                                            drawProjectile(starting, ending, currentSpell.SpriteFilename);
                                            drawEndEffect(selectedPoint, currentSpell.AoeRadiusOrLength, currentSpell.SpriteEndingFilename);
                                            bumpedIntoCreatureIndex = pcIndex;
                                            com_frm.sf.CombatSource = char_pt;
                                            com_frm.sf.CombatTarget = pc;
                                            SpellExecute(char_pt, null, pc);
                                            //gbPlayerTurn.Enabled = false; //JamesManhattan
                                            spellTargetSelected = false;
                                            DecreaseSpellPoints(currentSpell.CostSP, char_pt);
                                            checkEndEncounter();
                                            doCreateLabels();
                                            refreshCharacterPanel();
                                            //currentMoves = 99; //JamesManhattan
                                            usedAction = usedAction + 1; //JamesManhattan
                                            refreshMap(); 
                                            doUpdate(char_pt);
                                            break;
                                        }
                                        pcIndex++;
                                    }
                                }
                                else // target is a creature
                                {
                                    int crIndex = 0;
                                    foreach (Creature crt in com_encounter.EncounterCreatureList.creatures)
                                    {
                                        if ((gridx == crt.CombatLocation.X) && (gridy == crt.CombatLocation.Y))
                                        {
                                            if (numSpellAttacks <= 1) //JamesManhattan allow multiple target spells such as each magic missile bolt
                                            {
                                                rangedSpell = false;
                                            }
                                            else
                                            {
                                                logText(" Choose another target.", Color.DarkRed);
                                                numSpellAttacks = numSpellAttacks - 1;
                                            }

                                            Point starting = new Point((char_pt.CombatLocation.X * com_game._squareSize) + (com_game._squareSize / 2), (char_pt.CombatLocation.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                            Point ending = new Point((selectedPoint.X * com_game._squareSize) + (com_game._squareSize / 2), (selectedPoint.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                            if (starting.X > ending.X) { char_pt.CombatFacing = CharBase.facing.Left; }
                                            else { char_pt.CombatFacing = CharBase.facing.Right; }
                                            if (char_pt.CharSprite.AttackingNumberOfFrames > 1)
                                            {
                                                attackPcAnimation(char_pt, com_moveOrderList[currentMoveOrderIndex].index);
                                            }
                                            //playPcAttackSound(char_pt, currentSpell.SpellStartSound);  
                                            drawProjectile(starting, ending, currentSpell.SpriteFilename);
                                            playPcAttackSound(char_pt, currentSpell.SpellEndSound);
                                            drawEndEffect(selectedPoint, currentSpell.AoeRadiusOrLength, currentSpell.SpriteEndingFilename);
                                            bumpedIntoCreatureIndex = crIndex;
                                            com_frm.sf.CombatSource = char_pt;
                                            com_frm.sf.CombatTarget = crt;
                                            SpellExecute(char_pt, crt, null);
                                            //gbPlayerTurn.Enabled = false; //JamesManhattan
                                            spellTargetSelected = false;
                                            DecreaseSpellPoints(currentSpell.CostSP, char_pt);
                                            checkEndEncounter();
                                            doCreateLabels();
                                            refreshCharacterPanel();
                                            //currentMoves = 99; //JamesManhattan
                                            usedAction = usedAction + 1; //JamesManhattan
                                            refreshMap();
                                            doUpdate(char_pt);
                                            break;
                                        }
                                        crIndex++;
                                    }
                                }
                            }
                        }
                    }
                    else // right click was selected so exit ranged spell
                    {
                        rangedSpell = false;
                        gbPlayerTurn.Enabled = true;
                        spellTargetSelected = false;
                        //AoEtype = false;
                        //currentSpellRadius = 0;
                        //currentSpellRange = 0;
                        //currentSpell = MageSpells.mageSpellList.None;
                        logText("Cancelled Spell Use", Color.DarkRed);
                        logText(Environment.NewLine, Color.Black);
                        refreshMap();
                    }
                }
                #endregion

                #region For Ranged Traits
                else if (rangedTrait)
                {
                    PC char_pt = new PC();
                    char_pt.passRefs(com_game, null);
                    char_pt = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];

                    if (e.Button == MouseButtons.Left)
                    {
                        //gridx = e.X / com_game._squareSize;
                        //gridy = e.Y / com_game._squareSize;
                        //Point selectedPoint = new Point(gridx, gridy);
                        int dist = calcDistance(selectedPoint, char_pt.CombatLocation);
                        if ((currentTrait.Range >= dist) && (IsVisibleLineOfSight(char_pt.CombatLocation, selectedPoint)))// check to see if selected is in range
                        {
                            playPcAttackSound(char_pt, currentTrait.TraitStartSound);
                            // check to see if is AoE or Point Target else needs a target PC or Creature
                            //if ((currentTrait.AoeRadiusOrLength > 0) || (currentTrait.TargetIsPointLocation))
                            if ((currentTrait.AoeRadiusOrLength > 0) || (currentTrait.TraitTargetType == TargetType.PointLocation))
                            {
                                rangedTrait = false;

                                Point starting = new Point((char_pt.CombatLocation.X * com_game._squareSize) + (com_game._squareSize / 2), (char_pt.CombatLocation.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                Point ending = new Point((selectedPoint.X * com_game._squareSize) + (com_game._squareSize / 2), (selectedPoint.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                if (starting.X > ending.X) { char_pt.CombatFacing = CharBase.facing.Left; }
                                else { char_pt.CombatFacing = CharBase.facing.Right; }
                                if (char_pt.CharSprite.AttackingNumberOfFrames > 1)
                                {
                                    attackPcAnimation(char_pt, com_moveOrderList[currentMoveOrderIndex].index);
                                }
                                //playPcAttackSound(char_pt, currentTrait.TraitStartSound); 
                                drawProjectile(starting, ending, currentTrait.SpriteFilename);
                                drawEndEffect(selectedPoint, currentTrait.AoeRadiusOrLength, currentTrait.SpriteEndingFilename);
                                int crIndex = 0;
                                Point pnt = new Point(gridx, gridy);
                                com_frm.sf.CombatSource = char_pt;
                                com_frm.sf.CombatTarget = pnt;
                                foreach (Creature crt in com_encounter.EncounterCreatureList.creatures)
                                {
                                    // if in range of radius of x and radius of y
                                    if ((crt.CombatLocation.X >= gridx - currentTrait.AoeRadiusOrLength) && (crt.CombatLocation.X <= gridx + currentTrait.AoeRadiusOrLength))
                                    {
                                        if ((crt.CombatLocation.Y >= gridy - currentTrait.AoeRadiusOrLength) && (crt.CombatLocation.Y <= gridy + currentTrait.AoeRadiusOrLength))
                                        {
                                            bumpedIntoCreatureIndex = crIndex;
                                            TraitExecute(char_pt, crt, null);
                                        }
                                    }
                                    crIndex++;
                                }
                                //gbPlayerTurn.Enabled = false; //JamesManhattan
                                DecreaseSpellPoints(currentTrait.CostSP, char_pt);
                                checkEndEncounter();
                                doCreateLabels();
                                refreshCharacterPanel();
                                //currentMoves = 99; //JamesManhattan
                                usedAction = usedAction + 1; //JamesManhattan
                                refreshMap();
                                doUpdate(char_pt);
                            }
                            else // is not an AoE ranged attack, is a PC or Creature
                            {
                                //if (currentTrait.TargetIsPC) // target is a PC
                                if ((currentTrait.TraitTargetType == TargetType.Friend) || (currentTrait.TraitTargetType == TargetType.Self)) // target is a PC
                                {
                                    int pcIndex = 0;
                                    foreach (PC pc in com_game.playerList.PCList)
                                    {
                                        if ((gridx == pc.CombatLocation.X) && (gridy == pc.CombatLocation.Y))
                                        {
                                            rangedTrait = false;

                                            Point starting = new Point((char_pt.CombatLocation.X * com_game._squareSize) + (com_game._squareSize / 2), (char_pt.CombatLocation.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                            Point ending = new Point((selectedPoint.X * com_game._squareSize) + (com_game._squareSize / 2), (selectedPoint.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                            if (starting.X > ending.X) { char_pt.CombatFacing = CharBase.facing.Left; }
                                            else { char_pt.CombatFacing = CharBase.facing.Right; }
                                            if (char_pt.CharSprite.AttackingNumberOfFrames > 1)
                                            {
                                                attackPcAnimation(char_pt, com_moveOrderList[currentMoveOrderIndex].index);
                                            }
                                            //playPcAttackSound(char_pt, currentTrait.TraitStartSound); 
                                            drawProjectile(starting, ending, currentTrait.SpriteFilename);
                                            drawEndEffect(selectedPoint, currentTrait.AoeRadiusOrLength, currentTrait.SpriteEndingFilename);
                                            bumpedIntoCreatureIndex = pcIndex;
                                            com_frm.sf.CombatSource = char_pt;
                                            com_frm.sf.CombatTarget = pc;
                                            TraitExecute(char_pt, null, pc);
                                            //gbPlayerTurn.Enabled = false; //JamesManhattan
                                            DecreaseSpellPoints(currentTrait.CostSP, char_pt);
                                            checkEndEncounter();
                                            doCreateLabels();
                                            refreshCharacterPanel();
                                            //currentMoves = 99; //JamesManhattan
                                            usedAction = usedAction + 1; //JamesManhattan
                                            refreshMap();
                                            doUpdate(char_pt);
                                            break;
                                        }
                                        pcIndex++;
                                    }
                                }
                                else // target is a creature
                                {
                                    int crIndex = 0;
                                    foreach (Creature crt in com_encounter.EncounterCreatureList.creatures)
                                    {
                                        if ((gridx == crt.CombatLocation.X) && (gridy == crt.CombatLocation.Y))
                                        {
                                            rangedTrait = false;

                                            Point starting = new Point((char_pt.CombatLocation.X * com_game._squareSize) + (com_game._squareSize / 2), (char_pt.CombatLocation.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                            Point ending = new Point((selectedPoint.X * com_game._squareSize) + (com_game._squareSize / 2), (selectedPoint.Y * com_game._squareSize) + (com_game._squareSize / 2));
                                            if (starting.X > ending.X) { char_pt.CombatFacing = CharBase.facing.Left; }
                                            else { char_pt.CombatFacing = CharBase.facing.Right; }
                                            if (char_pt.CharSprite.AttackingNumberOfFrames > 1)
                                            {
                                                attackPcAnimation(char_pt, com_moveOrderList[currentMoveOrderIndex].index);
                                            }
                                            //playPcAttackSound(char_pt, currentTrait.TraitStartSound); 
                                            drawProjectile(starting, ending, currentTrait.SpriteFilename);
                                            drawEndEffect(selectedPoint, currentTrait.AoeRadiusOrLength, currentTrait.SpriteEndingFilename);
                                            bumpedIntoCreatureIndex = crIndex;
                                            com_frm.sf.CombatSource = char_pt;
                                            com_frm.sf.CombatTarget = crt;
                                            TraitExecute(char_pt, crt, null);
                                            //gbPlayerTurn.Enabled = false; //JamesManhattan
                                            DecreaseSpellPoints(currentTrait.CostSP, char_pt);
                                            checkEndEncounter();
                                            doCreateLabels();
                                            refreshCharacterPanel();
                                            //currentMoves = 99; //JamesManhattan
                                            usedAction = 1; //JamesManhattan
                                            refreshMap();
                                            doUpdate(char_pt);
                                            break;
                                        }
                                        crIndex++;
                                    }
                                }
                            }
                        }
                    }
                    else // right click was selected so exit ranged trait
                    {
                        rangedTrait = false;
                        gbPlayerTurn.Enabled = true;
                        logText("Cancelled Trait Use", Color.DarkRed);
                        logText(Environment.NewLine, Color.Black);
                        refreshMap();
                    }
                }
                #endregion

                #region MoveByMouseClickOrTouch
                else
                {
                    if (canMove)
                    {
                        PC char_pt = new PC();
                        char_pt.passRefs(com_game, null);
                        char_pt = com_game.playerList.PCList[com_moveOrderList[currentMoveOrderIndex].index];

                        int pcX = char_pt.CombatLocation.X;
                        int pcY = char_pt.CombatLocation.Y;

                        if (e.Button == MouseButtons.Left)
                        {
                            if ((gridx == pcX - 1) && (gridy == pcY - 1))
                            {
                                moveUpLeft(char_pt);
                            }
                            else if ((gridx == pcX - 1) && (gridy == pcY))
                            {
                                moveLeft(char_pt);
                            }
                            else if ((gridx == pcX - 1) && (gridy == pcY + 1))
                            {
                                moveDownLeft(char_pt);
                            }
                            else if ((gridx == pcX + 1) && (gridy == pcY - 1))
                            {
                                moveUpRight(char_pt);
                            }
                            else if ((gridx == pcX + 1) && (gridy == pcY))
                            {
                                moveRight(char_pt);
                            }
                            else if ((gridx == pcX + 1) && (gridy == pcY + 1))
                            {
                                moveDownRight(char_pt);
                            }
                            else if ((gridx == pcX) && (gridy == pcY - 1))
                            {
                                moveUp(char_pt);
                            }
                            else if ((gridx == pcX) && (gridy == pcY + 1))
                            {
                                moveDown(char_pt);
                            }
                            else
                            {
                                //do nothing, didn't click on an adjacent square
                            }
                        }
                    }
                }
                #endregion
            }
        }
        private void combatRenderPanel_MouseMove(object sender, MouseEventArgs e)
        {
            gridx = e.X / com_game._squareSize;
            gridy = e.Y / com_game._squareSize;
            mousex = e.X;
            mousey = e.Y;
            com_game.mouseCombatLocation = new Point(gridx, gridy);
            lblMouseInfo.Text = "CURSOR " + e.X.ToString() + "," + e.Y.ToString() +
                " - GRID " + gridx.ToString() + "," + gridy.ToString();
            if (gridx < 0) { gridx = 0; }
            if (gridy < 0) { gridy = 0; }
            if (gridx > (com_game.currentCombatArea.MapSizeInSquares.Width - 1)) { gridx = (com_game.currentCombatArea.MapSizeInSquares.Width - 1); }
            if (gridy > (com_game.currentCombatArea.MapSizeInSquares.Height - 1)) { gridy = (com_game.currentCombatArea.MapSizeInSquares.Height - 1); }

            #region Info Box Stuff
            bool foundSomething = false;
            foreach (Creature crt in com_encounter.EncounterCreatureList.creatures)
            {
                if (crt.HP > 0)
                {
                    if ((gridx == crt.CombatLocation.X) && (gridy == crt.CombatLocation.Y))
                    {
                        com_frm.sf.passParameterScriptObject = crt;
                        com_frm.doScriptBasedOnFilename("dsCombatInfoPanelData.cs", "", "", "", "");
                        com_frm.sf.passParameterScriptObject = null;
                        /*rtxtInfo.Text = crt.Name + Environment.NewLine +
                                        "HP: " + crt.HP +
                                        "   SP: " + crt.SP + Environment.NewLine +
                                        "Status: " + crt.Status.ToString();*/
                        if (okToDrawHighlights)
                        {
                            drawCrtHighlightBoxes(crt);
                            okToDrawHighlights = false;
                        }
                        foundSomething = true;
                    }
                }
            }
            foreach (PC pc in com_game.playerList.PCList)
            {
                if ((gridx == pc.CombatLocation.X) && (gridy == pc.CombatLocation.Y))
                {
                    com_frm.sf.passParameterScriptObject = pc;
                    com_frm.doScriptBasedOnFilename("dsCombatInfoPanelData.cs", "", "", "", "");
                    com_frm.sf.passParameterScriptObject = null;
                    /*rtxtInfo.Text = pc.Name + Environment.NewLine +
                                    "HP: " + pc.HP +
                                    "   SP: " + pc.SP + Environment.NewLine +
                                    "Status: " + pc.Status.ToString() + Environment.NewLine +
                                    "Weapon: " + pc.MainHand.ItemName;*/
                    foundSomething = true;
                }
            }
            if (!foundSomething)
            {
                rtxtInfo.Text = "";
                if (!okToDrawHighlights)
                {
                    refreshMap();
                    okToDrawHighlights = true;
                }

            }
            #endregion            
            
            // * sinopip, 20.12.14
            // * enable scroll area when mouse is on borders (scrollbar position values are negative)
            is_leftscrolling = false;
        	is_rightscrolling = false;
        	is_upscrolling = false;
        	is_downscrolling = false;
        	if (mousex < 100 + -panel1.AutoScrollPosition.X) 
        		is_leftscrolling = true;
        	if (mousey < 100 + -panel1.AutoScrollPosition.Y) 
        		is_upscrolling = true;
        	if (mousex > (panel1.Width-100) + -panel1.AutoScrollPosition.X)
        		is_rightscrolling = true;
        	if (mousey > (panel1.Height-100) + -panel1.AutoScrollPosition.Y)
        		is_downscrolling = true;
			//        	
        }
        public void drawEndEffect(Point target, int radius, string spriteFilename)
        {
            Point corner = new Point((target.X - radius) * com_game._squareSize, (target.Y - radius) * com_game._squareSize);
            Sprite newSprite = new Sprite();
            newSprite.passRefs(com_game);
            if (File.Exists(com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\graphics\\sprites\\FXs\\" + spriteFilename))
            {
                newSprite = newSprite.loadSpriteFile(com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\graphics\\sprites\\FXs\\" + spriteFilename);
                newSprite.passRefs(com_game);
            }
            else if (File.Exists(com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\graphics\\sprites\\" + spriteFilename))
            {
                newSprite = newSprite.loadSpriteFile(com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\graphics\\sprites\\" + spriteFilename);
                newSprite.passRefs(com_game);
            }
            else if (File.Exists(com_game.mainDirectory + "\\data\\graphics\\sprites\\FXs\\" + spriteFilename))
            {
                newSprite = newSprite.loadSpriteFile(com_game.mainDirectory + "\\data\\graphics\\sprites\\FXs\\" + spriteFilename);
                newSprite.passRefs(com_game);
            }
            else if (File.Exists(com_game.mainDirectory + "\\data\\graphics\\sprites\\" + spriteFilename))
            {
                newSprite = newSprite.loadSpriteFile(com_game.mainDirectory + "\\data\\graphics\\sprites\\" + spriteFilename);
                newSprite.passRefs(com_game);
            }
            else { return; }
//            com_game.currentCombatEndEffectSprite = newSprite;
            com_game.LoadCombatProjectileTextures(com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName, newSprite);

            int sleep = (1000 / newSprite.FPS);
            int maxFrames = newSprite.NumberOfFrames;
            int frameIndx = 0;
            for (int i = 0; i < maxFrames; i++)
            {
                com_game.CombatAreaEndEffectRenderAll(newSprite, corner, frameIndx);
//                com_game.eraseEndEffectBitmap(corner.X, corner.Y, newSprite.SpriteSize.Width, newSprite.SpriteSize.Height);
//                com_game.drawEndEffectBitmap(corner.X, corner.Y, newSprite.SpriteSize.Width, newSprite.SpriteSize.Height, frameIndx, sleep);
//                com_game.UpdateCombat();
                frameIndx++;
                Thread.Sleep(sleep);
            }
        }
        public void drawProjectile(Point start, Point end, string spriteFilename)
        {
            // Bresenham Line algorithm
            // Creates a line from Begin to End starting at (x0,y0) and ending at (x1,y1)
            // where x0 less than x1 and y0 less than y1
            // AND line is less steep than it is wide (dx less than dy)
//            com_game.currentCombatMapSnapshotBitmap = (Bitmap)com_game.gc_surface.Clone();
            Sprite newSprite = new Sprite();
            newSprite.passRefs(com_game);

            if (File.Exists(com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\graphics\\sprites\\FXs\\" + spriteFilename))
            {
                newSprite = newSprite.loadSpriteFile(com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\graphics\\sprites\\FXs\\" + spriteFilename);
                newSprite.passRefs(com_game);
            }
            else if (File.Exists(com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\graphics\\sprites\\" + spriteFilename))
            {
                newSprite = newSprite.loadSpriteFile(com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName + "\\graphics\\sprites\\" + spriteFilename);
                newSprite.passRefs(com_game);
            }
            else if (File.Exists(com_game.mainDirectory + "\\data\\graphics\\sprites\\FXs\\" + spriteFilename))
            {
                newSprite = newSprite.loadSpriteFile(com_game.mainDirectory + "\\data\\graphics\\sprites\\FXs\\" + spriteFilename);
                newSprite.passRefs(com_game);
            }
            else if (File.Exists(com_game.mainDirectory + "\\data\\graphics\\sprites\\" + spriteFilename))
            {
                newSprite = newSprite.loadSpriteFile(com_game.mainDirectory + "\\data\\graphics\\sprites\\" + spriteFilename);
                newSprite.passRefs(com_game);
            }
            else { return; }            
//            com_game.currentCombatProjectileSprite = newSprite;
            com_game.LoadCombatProjectileTextures(com_game.mainDirectory + "\\modules\\" + com_game.module.ModuleFolderName, newSprite);
            float angle = com_game.AngleRad(start, end);
            //logText("angle = " + angle.ToString(), Color.Black);
            //logText(Environment.NewLine, Color.Black);
            int deltax = Math.Abs(end.X - start.X);
            int deltay = Math.Abs(end.Y - start.Y);
            int ystep = 10;
            int xstep = 10;
            int sleep = animationDelay;
            int maxFrames = newSprite.NumberOfFrames;

            #region low angle version
            if (deltax > deltay)
            {
                Point nextPoint = start;
                int error = deltax / 2;

                if (end.Y < start.Y) { ystep = -1 * ystep; }

                if (end.X > start.X)
                {
                    int frameIndx = 0;
                    for (int x = start.X; x <= end.X; x += xstep)
                    {
//                        com_game.eraseProjectileBitmap(nextPoint.X, nextPoint.Y);
                        nextPoint.X = x;
                        error -= deltay;
                        if (error < 0)
                        {
                            nextPoint.Y += ystep;
                            error += deltax;
                        }                        
                        //refreshMap();
                        com_game.CombatAreaProjectileRenderAll(newSprite, nextPoint, angle, frameIndx);
//                        com_game.drawProjectileBitmap(nextPoint.X, nextPoint.Y, angle, frameIndx, sleep);
//                        com_game.UpdateCombat();
                        frameIndx++;
                        if (frameIndx == maxFrames) { frameIndx = 0; }
                        Thread.Sleep(sleep);
                    }
                }
                else
                {
                    int frameIndx = 0;
                    for (int x = start.X; x >= end.X; x -= xstep)
                    {
//                        com_game.eraseProjectileBitmap(nextPoint.X, nextPoint.Y);
                        nextPoint.X = x;
                        error -= deltay;
                        if (error < 0)
                        {
                            nextPoint.Y += ystep;
                            error += deltax;
                        }
                        //refreshMap();
                        com_game.CombatAreaProjectileRenderAll(newSprite, nextPoint, angle, frameIndx);
//                        com_game.drawProjectileBitmap(nextPoint.X, nextPoint.Y, angle, frameIndx, sleep);
//                        com_game.UpdateCombat();
                        frameIndx++;
                        if (frameIndx == maxFrames) { frameIndx = 0; }
                        Thread.Sleep(sleep);
                    }
                }
            }
            #endregion

            #region steep version
            else
            {
                Point nextPoint = start;
                int error = deltay / 2;

                if (end.X < start.X) { xstep = -1 * xstep; }

                if (end.Y > start.Y)
                {
                    int frameIndx = 0;
                    for (int y = start.Y; y <= end.Y; y += ystep)
                    {
//                        com_game.eraseProjectileBitmap(nextPoint.X, nextPoint.Y);
                        nextPoint.Y = y;
                        error -= deltax;
                        if (error < 0)
                        {
                            nextPoint.X += xstep;
                            error += deltay;
                        }
                        //refreshMap();
                        com_game.CombatAreaProjectileRenderAll(newSprite, nextPoint, angle, frameIndx);
//                        com_game.drawProjectileBitmap(nextPoint.X, nextPoint.Y, angle, frameIndx, sleep);
//                        com_game.UpdateCombat();
                        frameIndx++;
                        if (frameIndx == maxFrames) { frameIndx = 0; }
                        Thread.Sleep(sleep);
                    }
                }
                else
                {
                    int frameIndx = 0;
                    for (int y = start.Y; y >= end.Y; y -= ystep)
                    {
//                        com_game.eraseProjectileBitmap(nextPoint.X, nextPoint.Y);
                        nextPoint.Y = y;
                        error -= deltax;
                        if (error < 0)
                        {
                            nextPoint.X += xstep;
                            error += deltay;
                        }
                        //refreshMap();
                        com_game.CombatAreaProjectileRenderAll(newSprite, nextPoint, angle, frameIndx);
//                        com_game.drawProjectileBitmap(nextPoint.X, nextPoint.Y, angle, frameIndx, sleep);
//                        com_game.UpdateCombat();
                        frameIndx++;
                        if (frameIndx == maxFrames) { frameIndx = 0; }
                        Thread.Sleep(sleep);
                    }
                }
            }
            #endregion
            //newSprite.Image.Dispose();
        }

        #region Spell/Trait Handlers
        private void SpellTargeting()
        {
            rangedSpell = true;
            logText("Select a Target (right-click to cancel)", Color.DarkRed);
            logText(Environment.NewLine, Color.Black);
        }
        private void SpellExecute(PC pc, Creature c, PC targetPC)
        {
            var script = currentSpell.SpellScript;
            com_frm.doScriptBasedOnFilename(script.FilenameOrTag, script.Parm1, script.Parm2, script.Parm3, script.Parm4);
            /*if (currentSpell.SpellEffectType == Spell.EffectType.Damage)
            {
                int damage = currentSpell.DamageNumDice * (com_game.Random(currentSpell.DamageDie) + currentSpell.DamageDieAdder);
                logText(pc.Name, Color.Blue);
                logText(" attacks ", Color.Black);
                logText(c.Name, Color.LightGray);
                logText(" with a " + currentSpell.SpellName + " and HITS for ", Color.Black);
                logText(damage.ToString(), Color.Lime);
                logText(" point(s) of damage", Color.Black);
                logText(Environment.NewLine, Color.Black);
                logText(Environment.NewLine, Color.Black);

                c.HP = c.HP - damage;
                if (c.HP <= 0)
                {
                    logText("You killed the " + c.Name, Color.Lime);
                    logText(Environment.NewLine, Color.Black);
                    logText(Environment.NewLine, Color.Black);
                    com_encounter.EncounterCreatureList.creatures[bumpedIntoCreatureIndex].CharSprite.Image = new Bitmap(com_game.mainDirectory + "\\data\\rip.png");
                    //com_game.spriteCombatList[bumpedIntoCreatureIndex].Image = new Bitmap(com_game.mainDirectory + "\\data\\rip.png");
                    refreshMap();
                }
            }
            else // Heal type effect
            {
                int heal = currentSpell.HealNumDice * (com_game.Random(currentSpell.HealDie) + currentSpell.HealDieAdder);
                logText(pc.Name, Color.Blue);
                logText(" heals ", Color.Black);
                logText(targetPC.Name, Color.LightGray);
                logText(" for ", Color.Black);
                logText(heal.ToString(), Color.Lime);
                logText("hit points", Color.Black);
                logText(Environment.NewLine, Color.Black);
                logText(Environment.NewLine, Color.Black);
                targetPC.HP += heal;
                if (targetPC.HP > targetPC.HPMax)
                {
                    targetPC.HP = targetPC.HPMax;
                }
            }*/
        }
        private void TraitTargeting()
        {
            rangedTrait = true;
            logText("Select a Target (right-click to cancel)", Color.DarkRed);
            logText(Environment.NewLine, Color.Black);
        }
        private void TraitExecute(PC pc, Creature c, PC targetPC)
        {
            var script = currentTrait.TraitScript;
            com_frm.doScriptBasedOnFilename(script.FilenameOrTag, script.Parm1, script.Parm2, script.Parm3, script.Parm4);
            /*if (currentTrait.TraitEffectType == Spell.EffectType.Damage)
            {
                int damage = currentTrait.DamageNumDice * (com_game.Random(currentTrait.DamageDie) + currentTrait.DamageDieAdder);
                logText(pc.Name, Color.Blue);
                logText(" attacks ", Color.Black);
                logText(c.Name, Color.LightGray);
                logText(" with a " + currentTrait.TraitName + " and HITS for ", Color.Black);
                logText(damage.ToString(), Color.Lime);
                logText(" point(s) of damage", Color.Black);
                logText(Environment.NewLine, Color.Black);
                logText(Environment.NewLine, Color.Black);

                c.HP = c.HP - damage;
                if (c.HP <= 0)
                {
                    logText("You killed the " + c.Name, Color.Lime);
                    logText(Environment.NewLine, Color.Black);
                    logText(Environment.NewLine, Color.Black);
                    com_encounter.EncounterCreatureList.creatures[bumpedIntoCreatureIndex].CharSprite.Image = new Bitmap(com_game.mainDirectory + "\\data\\rip.png");
                    //com_game.spriteCombatList[bumpedIntoCreatureIndex].Image = new Bitmap(com_game.mainDirectory + "\\data\\rip.png");
                    refreshMap();
                }
            }
            else // Heal type effect
            {
                int heal = currentTrait.HealNumDice * (com_game.Random(currentTrait.HealDie) + currentTrait.HealDieAdder);
                logText(pc.Name, Color.Blue);
                logText(" heals ", Color.Black);
                logText(targetPC.Name, Color.LightGray);
                logText(" for ", Color.Black);
                logText(heal.ToString(), Color.Lime);
                logText("hit points", Color.Black);
                logText(Environment.NewLine, Color.Black);
                logText(Environment.NewLine, Color.Black);
                targetPC.HP += heal;
                if (targetPC.HP > targetPC.HPMax)
                {
                    targetPC.HP = targetPC.HPMax;
                }
            }*/
        }
        private void DecreaseSpellPoints(int decrease, PC pc)
        {
            pc.SP -= decrease;
            if (pc.SP < 0)
                pc.SP = 0;
        }
        #endregion

        private void numAnimationDelay_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                animationDelay = (int)numAnimationDelay.Value;
            }
            catch (Exception ex)
            {
                com_game.errorLog(ex.ToString());
            }
        }
        #endregion      

        private void combatTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                com_game.CombatAreaRenderAll();
            }
            catch (Exception ex)
            {
                com_game.errorLog(ex.ToString());
            }
        }
        private void Combat_FormClosed(object sender, FormClosedEventArgs e)
        {
            combatTimer.Stop();
            // * sinopip, 20.12.14
            scrollTimer.Stop();
            //
            com_game.DisposeCombatSpritesTextures();
        }
        //JamesManhattan added this function to look up whether a player possesses a certain trait.
        public bool HasTraitLookup(PC pc, string strTraitTag)
        {
            foreach (Trait tr in pc.KnownTraitsList.traitList)
            {
                if (tr.TraitTag == strTraitTag) { return true; }
            }
            return false;
        }
       
        // * sinopip, 20.12.14
		// * scroll the map (timer component of 25ms tick)
        void ScrollTimerTick(object sender, EventArgs e)
        {
        	if (is_leftscrolling) 
        		panel1.AutoScrollPosition = new Point(
        			-panel1.AutoScrollPosition.X - 16,
        			-panel1.AutoScrollPosition.Y);
        	if (is_rightscrolling) panel1.AutoScrollPosition = new Point(
        			-panel1.AutoScrollPosition.X + 16,
        			-panel1.AutoScrollPosition.Y);
        	if (is_upscrolling) panel1.AutoScrollPosition = new Point(
        			-panel1.AutoScrollPosition.X,
        			-panel1.AutoScrollPosition.Y - 16);
        	if (is_downscrolling) panel1.AutoScrollPosition = new Point(
        			-panel1.AutoScrollPosition.X,
        			-panel1.AutoScrollPosition.Y + 16);    	
        }
        void CombatRenderPanelMouseLeave(object sender, EventArgs e)
        {
            is_leftscrolling = false;
        	is_rightscrolling = false;
        	is_upscrolling = false;
        	is_downscrolling = false;        	
        }
        //
    }

    public class MoveOrder
    {
        public MoveOrder() { }

        public string type { get; set; }
        public string tag { get; set; }
        public int index { get; set; }
        public int rank { get; set; }
    }
 
}

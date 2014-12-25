/* IceBlink Engine by Jeremy Smith of BreeArts, copyright 2013 */

using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using IceBlinkCore;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Threading;
using System.Drawing.Imaging;
using SharpDX.Direct3D9;
using Sprite = IceBlinkCore.Sprite;
using Font = System.Drawing.Font;

namespace IceBlink
{
    public partial class Form1 : IBForm
    {
        public int squareSize = 64;
        public int numberOfSquares = 16;
        private string _mainDir;
        public Game game;
        public Game savedGame;
        public ScriptFunctions sf;
        public Theme currentTheme;
        public LoadScreen loadScrn;
        public CharacterSheet pcSheet0 = new CharacterSheet();
        public CharacterSheet pcSheet1 = new CharacterSheet();
        public CharacterSheet pcSheet2 = new CharacterSheet();
        public CharacterSheet pcSheet3 = new CharacterSheet();
        public CharacterSheet pcSheet4 = new CharacterSheet();
        public CharacterSheet pcSheet5 = new CharacterSheet();
        public PartyInventory pcInventory = new PartyInventory();
        public JournalScreen pcJournalScreen = new JournalScreen();
        public SettingsForm settings = new SettingsForm();
        public Debug debugForm = new Debug();
        public bool debugOpen = false;
        public bool inventoryOpen = false;
        public bool journalOpen = false;
        public Encounter currentEncounter;
        public Combat currentCombat;
        public Container currentContainer;
        public Shop currentShop;
        public Creature currentCreatureWithConversation;
        public Prop currentPropWithConversation;
        public Sprite newSprite;
        public bool autosave = false;
        public bool debugMode = false;
        public int movementDelayInMiliseconds = 100;
        private long timeStamp = 0;
        private bool finishedMove = true;
        public WMPLib.WindowsMediaPlayer areaMusic;
        public WMPLib.WindowsMediaPlayer areaSounds;
        public WMPLib.WindowsMediaPlayer convoSounds;
        public bool startScreenCompleted = false;
        public Spell currentSpell = new Spell();
        public Trait currentTrait = new Trait();
        public int windowSize = 0;
        public float AdvLogScale = 1.2f;
        public bool touchScreenFeatures = false;
        // * sinopip, 20.12.14
        public int mouseX;
        public int mouseY;
		public bool is_upscrolling = false;
		public bool is_downscrolling = false;
		public bool is_leftscrolling = false;
		public bool is_rightscrolling = false;
		//
		
        public Form1()
        {
            InitializeComponent();
            //SharpDX.Configuration.EnableObjectTracking = true;
            areaMusicTimer.Enabled = false;
            areaSoundsTimer.Enabled = false;
            //main loop timer
            timer.Stop();
            //animation loop timer
            AnimationTimer.Stop();
            //start updating FPS monitor
            FPStimer.Stop();
            realTimer.Enabled = true;
            realTimer.Stop();
            // * sinopip, 20.12.14
            scrollTimer.Stop();
            //
        }      
        private void Form1_Load(object sender, EventArgs e)
        {            
            CreateGameObject();
            CreateScriptFunctionsObject();

            //LoadConfigurationObject();
            //refreshTheme(); //Load the default Theme
            LoadTheme();
            //LoadXPTable();
            
            //bool newgame = doStartScreen(); /********** new way below ************/
            bool newgame = false;
            while (!startScreenCompleted)
            {
                newgame = doStartScreen2();
            }

            //IBMessageBox.Show(game, "testing");
            
            //game.initializeGraphics(this.pictureBox1); //can delete as this is the old gdi+ method
            SetupScreenSize();
            
            game.initializeRenderPanel(this.renderPanel);
            
            if (newgame) { CreateNewGame(); }
            //else         { LoadSavedGame(); } /********** new way below ************/
            else         { LoadSaveGame(); }

            //game.assemblyObjList.Clear();
            
            //this.WindowState = FormWindowState.Maximized;

            SetupAllBackImagesAndPassRefs();

            game.createDevice();
            game.newAreaInitializeGraphics();
            
            refreshPartyButtons();
            setupMusicPlayers();
            
            IceBlinkButtonResize.setupAll(game);
            IceBlinkButtonResize.Enabled = false;
            IceBlinkButtonResize.Visible = false;
            IceBlinkButtonClose.setupAll(game);
                        
            doScriptBasedOnFilename("dsOnModuleLoad.cs", "none", "none", "none", "none");

            sf.pathfinderMainArea = new PathfinderMainArea(game);

            doUpdate();
            chkGrid.Checked = false;
            setPcButtonColors();

            //main loop timer
            timer.Start();
            //animation loop timer
            AnimationTimer.Start();
            //start updating FPS monitor
            //FPStimer.Start();
            realTimer.Start();
            // * sinopip, 20.12.14
            scrollTimer.Start();
        }
        private void SetupScreenSize()
        {
            if (windowSize == 1)
            {
                this.Width = 1000;
                this.Height = 600;
                this.WindowState = FormWindowState.Normal;
            }
            else if (windowSize == 2)
            {
                this.Width = 1215;
                this.Height = 700;
                this.WindowState = FormWindowState.Normal;
            }
            else if (windowSize == 3)
            {
                this.Width = 1400;
                this.Height = 800;
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
            }
            // check the screen size and adjust the form as needed
            /*Screen scrn = Screen.FromControl(this);
            if (scrn == null)
            {
                scrn = Screen.PrimaryScreen;
            }
            int deskHeight = scrn.Bounds.Height;
            int deskWidth = scrn.Bounds.Width;
            //MessageBox.Show("Your screen resolution is " + deskWidth + "x" + deskHeight);
            if (deskHeight > 900)
            {
                this.Size = new System.Drawing.Size(983, 800);
            }*/            
        }
        private void CreateGameObject()
        {
            //create game object
            game = new Game();
            game.passRefs(this);

            game.mainDirectory = Directory.GetCurrentDirectory();
            _mainDir = Directory.GetCurrentDirectory();  //only used if Load Game is chosen

            game.errorLog("Starting IceBlink Engine");
        }
        private void CreateScriptFunctionsObject()
        {
            sf = new ScriptFunctions();
            sf.passRefs(this, game);            
        }
        private void LoadDefaultTheme()
        {
            string path = game.mainDirectory + "\\data\\ui\\";
            this.btnInventory.Image = currentTheme.LoadBtnInventoryBitmap(path + "inventory.png");
            this.btnJournal.Image = currentTheme.LoadBtnJournalBitmap(path + "journal.png");
            this.btnSettings.Image = currentTheme.LoadBtnSettingsBitmap(path + "settings.png");
            this.chkGrid.Image = currentTheme.LoadChkGridBitmap(path + "grid.png");
            this.btnRest.Image = currentTheme.LoadBtnRestBitmap(path + "rest.png");
            currentTheme.LoadStandardThemeBitmap(path + "standard.jpg");
            currentTheme.LoadStandardLoadScreen(path + "loadScreen.jpg");
            this.BackgroundImage = currentTheme.StandardThemeBitmap;
            rtxtLog.Font = ChangeFontSize(currentTheme.ModuleFont, 1.2f);
            //logText("Welcome to " + game.module.ModuleName + ", The Adventure Begins", Color.Silver);
            //logText(Environment.NewLine, Color.Silver);
        }
        public void LoadTheme()
        {
            currentTheme = new Theme();
            currentTheme.passRefs(game);
            try
            {
                if (game.module.ModuleFolderName != "")
                {
                    currentTheme = game.module.ModuleTheme;
                    currentTheme.ModuleFont = new Font(currentTheme.ModuleFontName, currentTheme.ModuleFontPointSize, FontStyle.Regular);
                    string path = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\ui\\"; 
                    this.btnInventory.Image = currentTheme.LoadBtnInventoryBitmap(path + "inventory.png");
                    this.btnJournal.Image = currentTheme.LoadBtnJournalBitmap(path + "journal.png");
                    this.btnSettings.Image = currentTheme.LoadBtnSettingsBitmap(path + "settings.png");
                    this.chkGrid.Image = currentTheme.LoadChkGridBitmap(path + "grid.png");
                    this.btnRest.Image = currentTheme.LoadBtnRestBitmap(path + "rest.png");
                    currentTheme.LoadStandardThemeBitmap(path + "standard.jpg");
                    currentTheme.LoadStandardLoadScreen(path + "loadScreen.jpg");
                    this.BackgroundImage = currentTheme.StandardThemeBitmap;
                    rtxtLog.Font = ChangeFontSize(currentTheme.ModuleFont, 1.2f);
                    logText("Welcome to " + game.module.ModuleName + ", The Adventure Begins", Color.Silver);
                    logText(Environment.NewLine, Color.Silver);
                    logText(Environment.NewLine, Color.Silver);
        //            this.setupAll(game);
        //            this.IBTitle = currentTheme.TitleText;
        //            this.IBTitleForeColor = currentTheme.TitleForeColor;
        //            this.IBTitleShadowColor = currentTheme.TitleShadowColor;
                }
                else
                {
                    LoadDefaultTheme();
                }
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "Failed to open Module UI, will use the default UI:" + Environment.NewLine + ex.ToString());
                LoadDefaultTheme();
            }
        }
        private void CreateNewGame()
        {
            game.module = new IceBlinkCore.Module();
            doOpenModule();
            LoadTheme();
            try
            {
                loadScrn = new LoadScreen(game, this);
                loadScrn.BackgroundImage = currentTheme.StandardLoadScreen;
                loadScrn.SetupScreenSize();
                loadScrn.Show();
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, ex.ToString());
            }
            #region Load Items
            try
            {
                string _filenameItems = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.ItemsFileName;
                game.module.ModuleItemsList = game.module.ModuleItemsList.loadItemsFile(_filenameItems);
                game.module.ModuleItemsList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open items file");
                game.errorLog(ex.ToString());
            }
            #endregion            
            #region Load Props
            try
            {
                string _filenameProps = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.PropsFileName;
                game.module.ModulePropsList = game.module.ModulePropsList.loadPropsFile(_filenameProps);
                game.module.ModulePropsList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open props file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load Journal
            try
            {
                string _filenameJournal = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.JournalFileName;
                game.module.ModuleJournal = game.module.ModuleJournal.loadJournalFile(_filenameJournal);
                game.module.ModuleJournal.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open module's journal file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load Containers
            try
            {
                string _filenameContainers = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.ContainersFileName;
                game.module.ModuleContainersList = game.module.ModuleContainersList.loadContainersFile(_filenameContainers);
                game.module.ModuleContainersList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open containers file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load Shops
            try
            {
                string _filenameShops = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.ShopsFileName;
                game.module.ModuleShopsList = game.module.ModuleShopsList.loadShopsFile(_filenameShops);
                game.module.ModuleShopsList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open shops file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load Creatures
            try
            {
                string _filenameCreatures = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.CreaturesFileName;
                game.module.ModuleCreaturesList = game.module.ModuleCreaturesList.loadCreaturesFile(_filenameCreatures);
                game.module.ModuleCreaturesList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open creatures file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load Encounters
            try
            {
                string _filenameEncounters = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.EncountersFileName;
                game.module.ModuleEncountersList = game.module.ModuleEncountersList.loadEncountersFile(_filenameEncounters);
                game.module.ModuleEncountersList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open encounters file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load PlayerClasses
            try
            {
                string _filenamePlayerClasses = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.PlayerClassesFileName;
                game.module.ModulePlayerClassList = game.module.ModulePlayerClassList.loadPlayerClassesFile(_filenamePlayerClasses);
                game.module.ModulePlayerClassList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open playerClasses file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load Races
            try
            {
                string _filenameRaces = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.RacesFileName;
                game.module.ModuleRacesList = game.module.ModuleRacesList.loadRacesFile(_filenameRaces);
                game.module.ModuleRacesList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open Races file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load Skills
            try
            {
                string _filenameSkills = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.SkillsFileName;
                game.module.ModuleSkillsList = game.module.ModuleSkillsList.loadSkillsFile(_filenameSkills);
                game.module.ModuleSkillsList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open Skills file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load Effects
            try
            {
                string _filenameEffects = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.EffectsFileName;
                game.module.ModuleEffectsList = game.module.ModuleEffectsList.loadEffectsFile(_filenameEffects);
                game.module.ModuleEffectsList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open Effects file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load Traits
            try
            {
                string _filenameTraits = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.TraitsFileName;
                game.module.ModuleTraitsList = game.module.ModuleTraitsList.loadTraitsFile(_filenameTraits);
                game.module.ModuleTraitsList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open Traits file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load Spells
            try
            {
                string _filenameSpells = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.SpellsFileName;
                game.module.ModuleSpellsList = game.module.ModuleSpellsList.loadSpellsFile(_filenameSpells);
                game.module.ModuleSpellsList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open Spells file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region PassRefs again just in case some files failed to load
            game.module.ModuleJournal.passRefs(game);
            game.module.ModuleContainersList.passRefs(game);
            game.module.ModuleShopsList.passRefs(game);
            game.module.ModuleCreaturesList.passRefs(game);
            game.module.ModuleEncountersList.passRefs(game);
            game.module.ModulePlayerClassList.passRefs(game);
            game.module.ModuleRacesList.passRefs(game);
            game.module.ModuleSkillsList.passRefs(game);
            game.module.ModuleEffectsList.passRefs(game);
            game.module.ModuleTraitsList.passRefs(game);
            game.module.ModuleSpellsList.passRefs(game);
            #endregion

            #region doLoadEncounters
            try { doLoadEncounters(); }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to doLoadEncounters file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region doLoadContainers
            try { doLoadContainers(); }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to doLoadContainers file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region doLoadShops
            try { doLoadShops(); }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to doLoadShops file");
                game.errorLog(ex.ToString());
            }
            #endregion
            
            //LoadTheme();

            this.btnRest.Visible = game.module.UseRestSystem;

            try
            {
                loadScrn.Hide();
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, ex.ToString());
            }

            launchCharacterPartySelection();

            try
            {
                loadScrn.BackgroundImage = currentTheme.StandardLoadScreen;
                loadScrn.SetupScreenSize();
                loadScrn.Show();
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, ex.ToString());
            }
            #region Inventory Stuff
            game.partyInventoryList = new List<Item>();
            pcInventory.passRefs(game, this);
            //game.partyInventoryList.Add(game.module.ModuleItemsList.itemsList[6]);
            //game.partyGold = 100;
            #endregion
            #region loadPCItems
            try { loadPCItems(); }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to load PC inventory items");
                game.errorLog(ex.ToString());
            }
            #endregion

            #region Journal Stuff
            pcJournalScreen.passRefs(game, this);

            game.partyJournalQuests = new Journal();
            game.partyJournalQuests.passRefs(game);
            game.partyJournalCompleted = new Journal();
            game.partyJournalCompleted.passRefs(game);
            //add a couple of quests for testing
            /*
            sf.AddJournalEntry("catTag3", 0);
            sf.AddJournalEntry("catTag3", "entryTag5");
            sf.AddJournalEntry("catTag3", 2);
            sf.AddJournalEntry("catTag7", "entryTag9");
            sf.AddJournalEntry("catTag8", 0);
            game.partyJournalQuests.categories.Add(game.module.ModuleJournal.categories[0]);
            game.partyJournalQuests.categories.Add(game.module.ModuleJournal.categories[1]);
            game.partyJournalCompleted.categories.Add(game.module.ModuleJournal.categories[0]);
            */
            #endregion

            #region Character Sheet PassRefs
            if (game.playerList.PCList.Count > 0)
                pcSheet0.passRefs(this, game, 0);
            if (game.playerList.PCList.Count > 1)
                pcSheet1.passRefs(this, game, 1);
            if (game.playerList.PCList.Count > 2)
                pcSheet2.passRefs(this, game, 2);
            if (game.playerList.PCList.Count > 3)
                pcSheet3.passRefs(this, game, 3);
            if (game.playerList.PCList.Count > 4)
                pcSheet4.passRefs(this, game, 4);
            if (game.playerList.PCList.Count > 5)
                pcSheet5.passRefs(this, game, 5);
            #endregion
            
            //Load All Areas
            game.module.loadAreas(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\areas\\");

            //Set Current Area from module.startingArea
            game.module.loadStartingArea(game);

            //loadCurrentAreaBitmap();
            //loadCurrentAreaTexture();
            //game.areaRenderAll();
            //game.Device.DrawImage((Image)game.currentMapBitmap, 0, 0);

            btnRest.Enabled = game.currentArea.RestingAllowed;

            #region doLoadAreaObjects
            try { doLoadAreaObjects(); }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to doLoadAreaObjects file");
                game.errorLog(ex.ToString());
            }
            #endregion

            #region Setup Initial Lists (containers, shops, areas)
            foreach (Container c in game.module.ModuleContainersList.containers)
            {
                c.InitialContainerItemTags.Clear();
                foreach (string i in c.containerItemTags)
                {
                    c.InitialContainerItemTags.Add(i);
                }
            }
            foreach (Shop s in game.module.ModuleShopsList.shopsList)
            {
                s.InitialShopItemTags.Clear();
                foreach (string i in s.shopItemTags)
                {
                    s.InitialShopItemTags.Add(i);
                }
            }
            foreach (Area a in game.module.ModuleAreasObjects)
            {
                a.InitialAreaPropTagsList.Clear();
                foreach (PropRefs p in a.AreaPropRefsList)
                {
                    a.InitialAreaPropTagsList.Add(p.PropTag);
                }
                a.InitialAreaCreatureTagsList.Clear();
                foreach (CreatureRefs c in a.AreaCreatureRefsList)
                {
                    a.InitialAreaCreatureTagsList.Add(c.CreatureTag);
                }
            }
            #endregion

            game.playerPosition.X = game.module.StartingPlayerPosition.X;
            game.playerPosition.Y = game.module.StartingPlayerPosition.Y;

            try
            {
                loadScrn.Hide();
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, ex.ToString());
            }
        }
        private void CreateNewGameWithKnownModule()
        {
            game.module = new IceBlinkCore.Module();
            game.module = game.module.loadModuleFile(game.mainDirectory + "\\modules\\" + savedGame.module.ModuleFolderName + "\\" + savedGame.module.ModuleName + ".module");
            if (game.module == null)
            {
                IBMessageBox.Show(game, "returned a null module");
            }
            game.module.passRefs(game, null);

            LoadTheme();
            try
            {
                loadScrn = new LoadScreen(game, this);
                loadScrn.BackgroundImage = currentTheme.StandardLoadScreen;
                loadScrn.SetupScreenSize();
                loadScrn.Show();
                // * sinopip, 15.08.14
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, ex.ToString());
            }

            #region Load Items
            try
            {
                string _filenameItems = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.ItemsFileName;
                game.module.ModuleItemsList = game.module.ModuleItemsList.loadItemsFile(_filenameItems);
                game.module.ModuleItemsList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open items file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load Props
            try
            {
                string _filenameProps = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.PropsFileName;
                game.module.ModulePropsList = game.module.ModulePropsList.loadPropsFile(_filenameProps);
                game.module.ModulePropsList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open props file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load Journal
            try
            {
                string _filenameJournal = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.JournalFileName;
                game.module.ModuleJournal = game.module.ModuleJournal.loadJournalFile(_filenameJournal);
                game.module.ModuleJournal.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open module's journal file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load Containers
            try
            {
                string _filenameContainers = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.ContainersFileName;
                game.module.ModuleContainersList = game.module.ModuleContainersList.loadContainersFile(_filenameContainers);
                game.module.ModuleContainersList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open containers file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load Shops
            try
            {
                string _filenameShops = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.ShopsFileName;
                game.module.ModuleShopsList = game.module.ModuleShopsList.loadShopsFile(_filenameShops);
                game.module.ModuleShopsList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open shops file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load Creatures
            try
            {
                string _filenameCreatures = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.CreaturesFileName;
                game.module.ModuleCreaturesList = game.module.ModuleCreaturesList.loadCreaturesFile(_filenameCreatures);
                game.module.ModuleCreaturesList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open creatures file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load Encounters
            try
            {
                string _filenameEncounters = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.EncountersFileName;
                game.module.ModuleEncountersList = game.module.ModuleEncountersList.loadEncountersFile(_filenameEncounters);
                game.module.ModuleEncountersList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open encounters file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load PlayerClasses
            try
            {
                string _filenamePlayerClasses = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.PlayerClassesFileName;
                game.module.ModulePlayerClassList = game.module.ModulePlayerClassList.loadPlayerClassesFile(_filenamePlayerClasses);
                game.module.ModulePlayerClassList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open playerClasses file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load Races
            try
            {
                string _filenameRaces = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.RacesFileName;
                game.module.ModuleRacesList = game.module.ModuleRacesList.loadRacesFile(_filenameRaces);
                game.module.ModuleRacesList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open Races file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load Skills
            try
            {
                string _filenameSkills = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.SkillsFileName;
                game.module.ModuleSkillsList = game.module.ModuleSkillsList.loadSkillsFile(_filenameSkills);
                game.module.ModuleSkillsList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open Skills file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load Effects
            try
            {
                string _filenameEffects = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.EffectsFileName;
                game.module.ModuleEffectsList = game.module.ModuleEffectsList.loadEffectsFile(_filenameEffects);
                game.module.ModuleEffectsList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open Effects file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load Traits
            try
            {
                string _filenameTraits = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.TraitsFileName;
                game.module.ModuleTraitsList = game.module.ModuleTraitsList.loadTraitsFile(_filenameTraits);
                game.module.ModuleTraitsList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open Traits file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region Load Spells
            try
            {
                string _filenameSpells = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.SpellsFileName;
                game.module.ModuleSpellsList = game.module.ModuleSpellsList.loadSpellsFile(_filenameSpells);
                game.module.ModuleSpellsList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open Spells file");
                game.errorLog(ex.ToString());
            }
            #endregion

            #region PassRefs again just in case some files failed to load
            game.module.ModuleJournal.passRefs(game);
            game.module.ModuleContainersList.passRefs(game);
            game.module.ModuleShopsList.passRefs(game);
            game.module.ModuleCreaturesList.passRefs(game);
            game.module.ModuleEncountersList.passRefs(game);
            game.module.ModulePlayerClassList.passRefs(game);
            game.module.ModuleRacesList.passRefs(game);
            game.module.ModuleSkillsList.passRefs(game);
            game.module.ModuleEffectsList.passRefs(game);
            game.module.ModuleTraitsList.passRefs(game);
            game.module.ModuleSpellsList.passRefs(game);
            #endregion

            #region doLoadEncounters
            try { doLoadEncounters(); }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to doLoadEncounters file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region doLoadContainers
            try { doLoadContainers(); }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to doLoadContainers file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region doLoadShops
            try { doLoadShops(); }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to doLoadShops file");
                game.errorLog(ex.ToString());
            }
            #endregion

            //LoadTheme();

            this.btnRest.Visible = game.module.UseRestSystem;

            //launchCharacterPartySelection();
            game.playerList.PCList.Clear();
            foreach (PC pc in savedGame.playerList.PCList)
            {
                game.playerList.PCList.Add(pc);
            }
            foreach (PC pc in game.playerList.PCList)
            {
                pc.passRefs(game, null);
                pc.LoadAllPcStuff(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName);
            }
            #region Inventory Stuff
            game.partyInventoryList = new List<Item>();
            pcInventory.passRefs(game, this);
            //game.partyInventoryList.Add(game.module.ModuleItemsList.itemsList[6]);
            //game.partyGold = 100;
            #endregion
            #region loadPCObjects
            try { loadPCObjects(); }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to load PC Objects (Race, Class, Skills, etc.)");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region loadPCItems
            try { loadPCItems(); }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to load PC inventory items");
                game.errorLog(ex.ToString());
            }
            #endregion
            foreach (PC pc in game.playerList.PCList)
            {
                pc.LoadSpriteStuff(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName);
            }

            #region Journal Stuff
            pcJournalScreen.passRefs(game, this);

            game.partyJournalQuests = new Journal();
            game.partyJournalQuests.passRefs(game);
            game.partyJournalCompleted = new Journal();
            game.partyJournalCompleted.passRefs(game);
            #endregion
            
            #region Character Sheet PassRefs
            if (game.playerList.PCList.Count > 0)
                pcSheet0.passRefs(this, game, 0);
            if (game.playerList.PCList.Count > 1)
                pcSheet1.passRefs(this, game, 1);
            if (game.playerList.PCList.Count > 2)
                pcSheet2.passRefs(this, game, 2);
            if (game.playerList.PCList.Count > 3)
                pcSheet3.passRefs(this, game, 3);
            if (game.playerList.PCList.Count > 4)
                pcSheet4.passRefs(this, game, 4);
            if (game.playerList.PCList.Count > 5)
                pcSheet5.passRefs(this, game, 5);
            #endregion

            //Load All Areas
            game.module.loadAreas(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\areas\\");

            //Set Current Area from module.startingArea
            game.module.loadStartingArea(game);

            //loadCurrentAreaBitmap();
            //loadCurrentAreaTexture();
            //game.Device.DrawImage((Image)game.currentMapBitmap, 0, 0);

            btnRest.Enabled = game.currentArea.RestingAllowed;

            #region doLoadAreaObjects
            try { doLoadAreaObjects(); }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to doLoadAreaObjects file");
                game.errorLog(ex.ToString());
            }
            #endregion

            #region Setup Initial Lists (containers, shops, areas)
            foreach (Container c in game.module.ModuleContainersList.containers)
            {
                c.InitialContainerItemTags.Clear();
                foreach (string i in c.containerItemTags)
                {
                    c.InitialContainerItemTags.Add(i);
                }
            }
            foreach (Shop s in game.module.ModuleShopsList.shopsList)
            {
                s.InitialShopItemTags.Clear();
                foreach (string i in s.shopItemTags)
                {
                    s.InitialShopItemTags.Add(i);
                }
            }
            foreach (Area a in game.module.ModuleAreasObjects)
            {
                a.InitialAreaPropTagsList.Clear();
                foreach (PropRefs p in a.AreaPropRefsList)
                {
                    a.InitialAreaPropTagsList.Add(p.PropTag);
                }
                a.InitialAreaCreatureTagsList.Clear();
                foreach (CreatureRefs c in a.AreaCreatureRefsList)
                {
                    a.InitialAreaCreatureTagsList.Add(c.CreatureTag);
                }
            }
            #endregion

            game.playerPosition.X = game.module.StartingPlayerPosition.X;
            game.playerPosition.Y = game.module.StartingPlayerPosition.Y;
        }
        private void LoadSaveGame()
        {
            
            //load saved game and store in another Game object (like Game savedGame = load from file)
            //create new game object (same as done with a new game)
            CreateNewGameWithKnownModule();
            //iterate through savedGame and update new game object as needed
            #region UPDATE MODULE STUFF
            #region Encounters
            foreach (Encounter enc in savedGame.module.ModuleEncountersList.encounters)
            {
                //if (enc.EncounterCreatureList.creatures.Count < 1)
                if (enc.EncounterActive == false)
                {
                    Encounter e = game.module.ModuleEncountersList.getEncounter(enc.EncounterName);
                    e.EncounterCreatureList.creatures.Clear();
                }
            }
            #endregion
            #region GlobalInts
            game.module.ModuleGlobalInts.Clear();
            foreach (GlobalInt gInt in savedGame.module.ModuleGlobalInts)
            {
                game.module.ModuleGlobalInts.Add(gInt);
            }
            #endregion
            #region GlobalStrings
            game.module.ModuleGlobalStrings.Clear();
            foreach (GlobalString gStr in savedGame.module.ModuleGlobalStrings)
            {
                game.module.ModuleGlobalStrings.Add(gStr);
            }
            #endregion
            #region GlobalObjects
            game.module.ModuleGlobalObjects.Clear();
            foreach (GlobalObject gObj in savedGame.module.ModuleGlobalObjects)
            {
                game.module.ModuleGlobalObjects.Add(gObj);
            }
            #endregion
            #region Containers
            //TODO maybe have an original list and compare the list from the saved game to the new game
            foreach (Container cnt in savedGame.module.ModuleContainersList.containers)
            {
                //fill container with items that are still in the saved game 
                Container updatedCont = game.module.ModuleContainersList.getContainer(cnt.ContainerTag);
                if (updatedCont != null)
                {
                    updatedCont.containerInventoryList.Clear();
                    updatedCont.containerItemTags.Clear();
                    updatedCont.items.Clear();
                    foreach (string it in cnt.containerItemTags)
                    {                        
                        Item newItem = game.module.ModuleItemsList.getItemByTag(it);
                        if (newItem != null)
                        {
                            Item i = newItem.DeepCopy();
                            updatedCont.containerInventoryList.Add(i);
                            updatedCont.containerItemTags.Add(i.ItemTag);
                            updatedCont.items.Add(i.ItemName);
                        }
                    }
                    //compare lists and add items that are new
                    foreach (string itemTag in updatedCont.InitialContainerItemTags)
                    {
                        if (cnt.InitialContainerItemTags.Contains(itemTag))
                        {
                            //item tag found in both, do nothing
                        }
                        else
                        {
                            //item is not in the saved game initial container item list so add it to the container
                            Item newItem = game.module.ModuleItemsList.getItemByTag(itemTag);
                            if (newItem != null)
                            {
                                Item i = newItem.DeepCopy();
                                updatedCont.containerInventoryList.Add(i);
                                updatedCont.containerItemTags.Add(i.ItemTag);
                                updatedCont.items.Add(i.ItemName);
                            }
                        }
                    }
                }
            }
            #endregion
            #region Shops
            //TODO maybe have an original list and compare the list from the saved game to the new game
            foreach (Shop shp in savedGame.module.ModuleShopsList.shopsList)
            {
                Shop updatedShop = game.module.ModuleShopsList.getShopByTag(shp.ShopTag);
                if (updatedShop != null)
                {
                    updatedShop.shopItemObjectsList.Clear();
                    foreach (string it in shp.shopItemTags)
                    {
                        Item newItem = game.module.ModuleItemsList.getItemByTag(it);
                        if (newItem != null)
                        {
                            Item i = newItem.DeepCopy();
                            updatedShop.shopItemObjectsList.Add(newItem);
                        }
                    }
                    //compare lists and add items that are new
                    foreach (string itemTag in updatedShop.InitialShopItemTags)
                    {
                        if (shp.InitialShopItemTags.Contains(itemTag))
                        {
                            //item tag found in both, do nothing
                        }
                        else
                        {
                            //item is not in the saved game initial container item list so add it to the container
                            Item newItem = game.module.ModuleItemsList.getItemByTag(itemTag);
                            if (newItem != null)
                            {
                                Item i = newItem.DeepCopy();
                                updatedShop.shopItemObjectsList.Add(i);
                            }
                        }
                    }
                }
            }
            #endregion
            #region Areas
            //TODO maybe have an original list and compare the list from the saved game to the new game
            foreach (Area ar in game.module.ModuleAreasObjects)
            {
                foreach (Area sar in savedGame.module.ModuleAreasObjects)
                {
                    if (sar.AreaFileName == ar.AreaFileName) //sar is saved game, ar is new game from toolset version
                    {
                        //tiles
                        for (int index = 0; index < (ar.MapSizeInSquares.Width * ar.MapSizeInSquares.Height); index++)
                        {
                            ar.TileMapList[index].visible = sar.TileMapList[index].visible;
                        }
                        //props
                        //start at the end of the newMod prop list and work up
                        //if the prop tag is found in the save game, update it
                        //else if not found in saved game, but exists in the 
                        //saved game initial list (the toolset version of the prop list), remove prop
                        //else leave it alone
                        for (int index = ar.AreaPropList.propsList.Count - 1; index >= 0; index--)
                        //foreach (Prop prp in ar.AreaPropList.propsList)
                        {
                            Prop prp = ar.AreaPropList.propsList[index];
                            bool foundOne = false;
                            foreach (Prop sprp in sar.AreaPropList.propsList) //sar is the saved game area
                            //foreach (PropRefs sprp in sar.AreaPropRefsList) //sar is the saved game area
                            {
                                if (prp.PropTag == sprp.PropTag)
                                {
                                    foundOne = true; //the prop tag exists in the saved game
                                    prp.Show = sprp.Show;
                                    prp.Visible = sprp.Visible;
                                    prp.Location = sprp.Location;
                                    prp.PropLocked = sprp.PropLocked;
                                    prp.PropTrapped = sprp.PropTrapped;
                                    prp.MouseOverText = sprp.MouseOverText;
                                }
                            }
                            if (!foundOne) //didn't find the prop tag in the saved game
                            {
                                if (sar.InitialAreaPropTagsList.Contains(prp.PropTag))
                                {
                                    //was once on the map, but was deleted so remove from the newMod prop list
                                    ar.AreaPropList.propsList.RemoveAt(index);
                                }
                                else
                                {
                                    //is new to the mod so leave it alone, don't remove from the prop list
                                }
                            }
                        }
                        //creatures
                        for (int index = ar.AreaCreatureList.creatures.Count - 1; index >= 0; index--)
                        //foreach (Creature crt in ar.AreaCreatureList.creatures)
                        {
                            Creature crt = ar.AreaCreatureList.creatures[index];
                            bool foundOne = false;
                            foreach (Creature scrt in sar.AreaCreatureList.creatures)
                            {
                                if (crt.Tag == scrt.Tag)
                                {
                                    foundOne = true;
                                    crt.Show = scrt.Show;
                                    crt.Visible = scrt.Visible;
                                    crt.MapLocation = scrt.MapLocation;
                                    crt.MouseOverText = scrt.MouseOverText;
                                    //will replace all LocalInts from the toolset with those from Saved game
                                    //if a Local in the saved game does not exist in the newer toolset list, the saved game Local will be added to the loaded game list
                                    foreach (LocalInt ls in scrt.CharLocalInts)
                                    {
                                        bool foundInt = false;
                                        foreach (LocalInt lt in crt.CharLocalInts)
                                        {
                                            if (ls.Key == lt.Key)
                                            {
                                                lt.Value = ls.Value;
                                                foundInt = true;
                                            }
                                        }
                                        if (!foundInt)
                                        {
                                            LocalInt Lint = new LocalInt();
                                            Lint.Key = ls.Key;
                                            Lint.Value = ls.Value;
                                            crt.CharLocalInts.Add(Lint);
                                        }
                                    }
                                    //will replace all LocalStrings from the toolset with those from Saved game
                                    foreach (LocalString ls in scrt.CharLocalStrings)
                                    {
                                        bool foundInt = false;
                                        foreach (LocalString lt in crt.CharLocalStrings)
                                        {
                                            if (ls.Key == lt.Key)
                                            {
                                                lt.Value = ls.Value;
                                                foundInt = true;
                                            }
                                        }
                                        if (!foundInt)
                                        {
                                            LocalString Lint = new LocalString();
                                            Lint.Key = ls.Key;
                                            Lint.Value = ls.Value;
                                            crt.CharLocalStrings.Add(Lint);
                                        }
                                    }
                                }
                            }
                            if (!foundOne) //didn't find the creature tag in the saved game
                            {
                                if (sar.InitialAreaCreatureTagsList.Contains(crt.Tag))
                                {
                                    //was once on the map, but was deleted so remove from the newMod creature list
                                    ar.AreaCreatureList.creatures.RemoveAt(index);
                                }
                                else
                                {
                                    //is new to the mod so leave it alone, don't remove from the prop list
                                }
                            }
                        }
                        //triggers
                        foreach (Trigger tr in ar.AreaTriggerList.triggersList)
                        {
                            foreach (Trigger str in sar.AreaTriggerList.triggersList)
                            {
                                if (tr.TriggerTag == str.TriggerTag)
                                {
                                    tr.Enabled = str.Enabled;
                                    tr.EnabledEvent1 = str.EnabledEvent1;
                                    tr.EnabledEvent2 = str.EnabledEvent2;
                                    tr.EnabledEvent3 = str.EnabledEvent3;
                                    tr.EnabledEvent4 = str.EnabledEvent4;
                                    tr.EnabledEvent5 = str.EnabledEvent5;
                                    tr.EnabledEvent6 = str.EnabledEvent6;
                                }
                            }
                        }
                    }
                }                
            }
            #endregion
            #region ModuleConvoSavedValuesList
            game.module.ModuleConvoSavedValuesList.Clear();
            foreach (ConvoSavedValues csv in savedGame.module.ModuleConvoSavedValuesList)
            {
                game.module.ModuleConvoSavedValuesList.Add(csv);
            }
            #endregion
            game.module.WorldTime = savedGame.module.WorldTime;
            #endregion
            #region UPDATE GAME STUFF            
            //player position
            game.playerPosition = savedGame.playerPosition;
            //lastPlayerLocation (use the saved game location)
            game.lastPlayerLocation = savedGame.lastPlayerLocation;
            //set currentArea
            try
            {
                //change currentArea to new area
                foreach (Area area in game.module.ModuleAreasObjects)
                {
                    if (savedGame.currentArea.AreaFileName == area.AreaFileName)
                    {
                        game.currentArea = area;
                        //loadCurrentAreaBitmap();
                        //loadCurrentAreaTexture();
                        btnRest.Enabled = game.currentArea.RestingAllowed;
                    }
                }
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to setup current area");
                game.errorLog(ex.ToString());
            }
            //PartyCombatOrder (use saved game order)
            for(int i = 0; i < 6; i++)
            {
                game.PartyCombatOrder[i] = savedGame.PartyCombatOrder[i];
            }
            //partyGold (use saved game amount)
            game.partyGold = savedGame.partyGold;
            //playerList (use saved game list and update, see PC)
            //partyInventoryList (update from master Item List)
            //partyJournalQuests (use saved game, but update text as needed)
            game.partyJournalQuests = savedGame.partyJournalQuests.DeepCopy();
            //partyJournalCompleted (use saved game, but update text as needed)
            game.partyJournalCompleted = savedGame.partyJournalCompleted.DeepCopy();
            //partyJournalNotes (use saved game)
            game.partyJournalNotes = savedGame.partyJournalNotes;
            //com_showGrid (use from saved game)
            game.com_showGrid = savedGame.com_showGrid;
            //com_showFacing (use from saved game)
            game.com_showFacing = savedGame.com_showFacing;
            //com_showRange (use from saved game)
            game.com_showRange = savedGame.com_showRange;
            //frm_showGrid (use from saved game)
            game.frm_showGrid = savedGame.frm_showGrid;
            //make sure inventory sheet is refreshed to show equipped items
            pcInventory.refreshEquippedToPc0();
            #endregion
            try
            {
                loadScrn.Hide();
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, ex.ToString());
            }
        }
        /*private void LoadSavedGame()
        {
            game.mainDirectory = _mainDir;
            game.module.passRefs(game, null);

            LoadTheme();

            this.btnRest.Visible = game.module.UseRestSystem;

            #region Load Items
            try
            {
                game.module.ModuleItemsList.itemsList.Clear();
                string _filenameItems = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\data\\" + game.module.ItemsFileName;
                game.module.ModuleItemsList = game.module.ModuleItemsList.loadItemsFile(_filenameItems);
                game.module.ModuleItemsList.passRefs(game);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open items file");
                game.errorLog(ex.ToString());
            }
            #endregion

            //pass refs that are in Module construct method
            game.module.ModuleItemsList.passRefs(game);
            game.module.ModulePropsList.passRefs(game);
            game.module.ModuleCreaturesList.passRefs(game);
            game.module.ModuleContainersList.passRefs(game);
            game.module.ModuleShopsList.passRefs(game);
            game.module.ModulePlayerClassList.passRefs(game);
            game.module.ModuleRacesList.passRefs(game);
            game.module.ModuleSkillsList.passRefs(game);
            game.module.ModuleTraitsList.passRefs(game);
            game.module.ModuleEffectsList.passRefs(game);
            game.module.ModuleSpellsList.passRefs(game);
            game.module.ModuleJournal.passRefs(game);
            game.partyJournalQuests.passRefs(game);
            game.partyJournalCompleted.passRefs(game);
            //load PC portraits
            int cnt = 0;
            foreach (PC pc in game.playerList.PCList)
            {
                pc.passRefs(game, null);
                pc.LoadAllPcStuff(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName);
                cnt++;
            }

            #region load up PC buttons, inventory sheets
            if (game.playerList.PCList.Count >= 1)
            {
                pc_button_0.Image = (Image)game.playerList.PCList[0].portraitBitmapM;
                pc_button_0.Enabled = true;
            }
            if (game.playerList.PCList.Count >= 2)
            {
                pc_button_1.Image = (Image)game.playerList.PCList[1].portraitBitmapM;
                pc_button_1.Enabled = true;
            }
            if (game.playerList.PCList.Count >= 3)
            {
                pc_button_2.Image = (Image)game.playerList.PCList[2].portraitBitmapM;
                pc_button_2.Enabled = true;
            }
            if (game.playerList.PCList.Count >= 4)
            {
                pc_button_3.Image = (Image)game.playerList.PCList[3].portraitBitmapM;
                pc_button_3.Enabled = true;
            }
            if (game.playerList.PCList.Count >= 5)
            {
                pc_button_4.Image = (Image)game.playerList.PCList[4].portraitBitmapM;
                pc_button_4.Enabled = true;
            }
            if (game.playerList.PCList.Count >= 6)
            {
                pc_button_5.Image = (Image)game.playerList.PCList[5].portraitBitmapM;
                pc_button_5.Enabled = true;
            }
            //PC inventory pass refs                
            pcInventory.passRefs(game, this);
            //PC sheets, pass refs
            if (game.playerList.PCList.Count > 0)
            {
                pcSheet0.passRefs(this, game, 0);
                pcInventory.rbtnPc0.Enabled = true;
            }
            if (game.playerList.PCList.Count > 1)
            {
                pcSheet1.passRefs(this, game, 1);
                pcInventory.rbtnPc1.Enabled = true;
            }
            if (game.playerList.PCList.Count > 2)
            {
                pcSheet2.passRefs(this, game, 2);
                pcInventory.rbtnPc2.Enabled = true;
            }
            if (game.playerList.PCList.Count > 3)
            {
                pcSheet3.passRefs(this, game, 3);
                pcInventory.rbtnPc3.Enabled = true;
            }
            if (game.playerList.PCList.Count > 4)
            {
                pcSheet4.passRefs(this, game, 4);
                pcInventory.rbtnPc4.Enabled = true;
            }
            if (game.playerList.PCList.Count > 5)
            {
                pcSheet5.passRefs(this, game, 5);
                pcInventory.rbtnPc5.Enabled = true;
            }
            #endregion

            pcJournalScreen.passRefs(game, this);

            foreach (Area a in game.module.ModuleAreasObjects)
            {
                a.passRefs(game, a.MapSizeInSquares.Width, a.MapSizeInSquares.Height);
                game.module.loadAreaStuff(a);
            }

            game.currentArea.passRefs(game, game.currentArea.MapSizeInSquares.Width, game.currentArea.MapSizeInSquares.Height);
            game.module.loadAreaStuff(game.currentArea);

            #region doLoadEncountersStuff
            try { doLoadEncountersStuff(); } 
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to doLoadEncountersStuff file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region doLoadContainers
            try { doLoadContainers(); }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to doLoadContainers file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region doLoadShops
            try { doLoadShops(); }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to doLoadShops file");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region loadPCObjects
            try { loadPCObjects(); }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to load PC Objects (Race, Class, Skills, etc.)");
                game.errorLog(ex.ToString());
            }
            #endregion
            #region loadPCItems
            try { loadPCItems(); }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to load PC inventory items");
                game.errorLog(ex.ToString());
            }
            #endregion

            // load current level and map
            //loadCurrentAreaBitmap();
            //loadCurrentAreaTexture();
            btnRest.Enabled = game.currentArea.RestingAllowed;
            //game.Device.DrawImage((Image)game.currentMapBitmap, 0, 0);
            //game.Update();

            game.playerPosition.X = game.lastPlayerLocation.X;
            game.playerPosition.Y = game.lastPlayerLocation.Y;

            foreach (PC pc in game.playerList.PCList)
            {
                pc.LoadSpriteStuff(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName);
            }
        }*/
        private void SetupAllBackImagesAndPassRefs()
        {
            pcSheet0.BackgroundImage = currentTheme.StandardThemeBitmap;
            pcSheet1.BackgroundImage = currentTheme.StandardThemeBitmap;
            pcSheet2.BackgroundImage = currentTheme.StandardThemeBitmap;
            pcSheet3.BackgroundImage = currentTheme.StandardThemeBitmap;
            pcSheet4.BackgroundImage = currentTheme.StandardThemeBitmap;
            pcSheet5.BackgroundImage = currentTheme.StandardThemeBitmap;
            debugForm.BackgroundImage = currentTheme.StandardThemeBitmap;
            pcInventory.BackgroundImage = currentTheme.StandardThemeBitmap;
            pcJournalScreen.BackgroundImage = currentTheme.StandardThemeBitmap;
            settings.BackgroundImage = currentTheme.StandardThemeBitmap;

            debugForm.passRefs(this, game);
            settings.passRefs(this, game);
        }
        

        #region Area Music/Sounds
        public void setupMusicPlayers()
        {
            try
            {
                areaMusic = new WMPLib.WindowsMediaPlayer();
                areaMusic.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(AreaMusic_PlayStateChange);
                areaMusic.MediaError += new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);
                areaMusic.settings.volume = 25;

                areaSounds = new WMPLib.WindowsMediaPlayer();
                areaSounds.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(AreaSounds_PlayStateChange);
                areaSounds.MediaError += new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);

                convoSounds = new WMPLib.WindowsMediaPlayer();

                playAreaMusicSounds();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to setup Music Player...Audio will be disabled (see debug.txt for more details). Most likely due to not having Windows Media Player installed or having an incompatible version.");
                game.errorLog("Failed on setupMusicPlayers()" + ex.ToString());
            }
        }
        public void playAreaMusicSounds()
        {
            try
            {
                areaMusic.controls.stop();
                areaSounds.controls.stop();

                if (game.currentArea.AreaMusic != "none")
                {
                    if (File.Exists(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\sounds\\areaMusic\\" + game.currentArea.AreaMusic))
                    {
                        areaMusic.URL = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\sounds\\areaMusic\\" + game.currentArea.AreaMusic;
                    }
                    else if (File.Exists(game.mainDirectory + "\\data\\sounds\\areaMusic\\" + game.currentArea.AreaMusic))
                    {
                        areaMusic.URL = game.mainDirectory + "\\data\\sounds\\areaMusic\\" + game.currentArea.AreaMusic;
                    }
                    else
                    {
                        areaMusic.URL = "";
                    }
                    if (areaMusic.URL != "")
                    {
                        areaMusic.controls.stop();
                        areaMusic.controls.play();
                    }
                }
                else
                {
                    areaMusic.URL = "";
                }
                if (game.currentArea.AreaSounds != "none")
                {
                    if (File.Exists(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\sounds\\areaSounds\\" + game.currentArea.AreaSounds))
                    {
                        areaSounds.URL = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\sounds\\areaSounds\\" + game.currentArea.AreaSounds;
                    }
                    else if (File.Exists(game.mainDirectory + "\\data\\sounds\\areaSounds\\" + game.currentArea.AreaSounds))
                    {
                        areaSounds.URL = game.mainDirectory + "\\data\\sounds\\areaSounds\\" + game.currentArea.AreaSounds;
                    }
                    else
                    {
                        areaSounds.URL = "";
                    }
                    if (areaSounds.URL != "")
                    {
                        areaSounds.controls.stop();
                        areaSounds.controls.play();
                    }
                }
                else
                {
                    areaSounds.URL = "";
                }
            }
            catch (Exception ex)
            {
                game.errorLog("Failed on playAreaMusicSounds()" + ex.ToString());
            }
        }
        public void playCombatAreaMusicSounds()
        {
            try
            {
                areaMusic.controls.stop();
                areaSounds.controls.stop();

                if (game.currentCombatArea.AreaMusic != "none")
                {
                    if (File.Exists(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\sounds\\areaMusic\\" + game.currentCombatArea.AreaMusic))
                    {
                        areaMusic.URL = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\sounds\\areaMusic\\" + game.currentCombatArea.AreaMusic;
                    }
                    else if (File.Exists(game.mainDirectory + "\\data\\sounds\\areaMusic\\" + game.currentCombatArea.AreaMusic))
                    {
                        areaMusic.URL = game.mainDirectory + "\\data\\sounds\\areaMusic\\" + game.currentCombatArea.AreaMusic;
                    }
                    else
                    {
                        areaMusic.URL = "";
                    }
                    if (areaMusic.URL != "")
                    {
                        areaMusic.controls.stop();
                        areaMusic.controls.play();
                    }
                }
                else
                {
                    areaMusic.URL = "";
                }
                if (game.currentCombatArea.AreaSounds != "none")
                {
                    if (File.Exists(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\sounds\\areaSounds\\" + game.currentCombatArea.AreaSounds))
                    {
                        areaSounds.URL = game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\sounds\\areaSounds\\" + game.currentCombatArea.AreaSounds;
                    }
                    else if (File.Exists(game.mainDirectory + "\\data\\sounds\\areaSounds\\" + game.currentCombatArea.AreaSounds))
                    {
                        areaSounds.URL = game.mainDirectory + "\\data\\sounds\\areaSounds\\" + game.currentCombatArea.AreaSounds;
                    }
                    else
                    {
                        areaSounds.URL = "";
                    }
                    if (areaSounds.URL != "")
                    {
                        areaSounds.controls.stop();
                        areaSounds.controls.play();
                    }
                }
                else
                {
                    areaSounds.URL = "";
                }
            }
            catch (Exception ex)
            {
                game.errorLog("Failed on playCombatAreaMusicSounds()" + ex.ToString());
            }
        }
        private void AreaMusic_PlayStateChange(int NewState)
        {
            try
            {
                if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsStopped)
                {
                    delayMusic();
                }
            }
            catch (Exception ex)
            {
                game.errorLog("Failed on AreaMusic_PlayStateChange()" + ex.ToString());
            }
        }
        private void AreaSounds_PlayStateChange(int NewState)
        {
            try
            {
                if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsStopped)
                {
                    delaySounds();
                }
            }
            catch (Exception ex)
            {
                game.errorLog("Failed on AreaSounds_PlayStateChange()" + ex.ToString());
            }
        }
        private void Player_MediaError(object pMediaObject)
        {
            logMainText("Cannot play media file.", Color.Black);
            logMainText(Environment.NewLine, Color.Black);
        }
        private void delayMusic()
        {
            try
            {
                int rand = game.Random(game.currentArea.AreaMusicDelayRandomAdder);
                areaMusicTimer.Enabled = false;
                areaMusic.controls.stop();
                areaMusicTimer.Interval = game.currentArea.AreaMusicDelay + rand;
                areaMusicTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                game.errorLog("Failed on delayMusic()" + ex.ToString());
            }
        }
        private void areaMusicTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (areaMusic.URL != "")
                {
                    areaMusic.controls.play();
                }
                areaMusicTimer.Enabled = false;
            }
            catch (Exception ex)
            {
                game.errorLog("Failed on areaMusicTimer_Tick()" + ex.ToString());
            }
        }        
        private void delaySounds()
        {
            try
            {
                int rand = game.Random(game.currentArea.AreaSoundsDelayRandomAdder);
                areaSoundsTimer.Enabled = false;
                areaSounds.controls.stop();
                areaSoundsTimer.Interval = game.currentArea.AreaSoundsDelay + rand;
                areaSoundsTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                game.errorLog("Failed on delaySounds()" + ex.ToString());
            }
        }
        private void areaSoundsTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (areaSounds.URL != "")
                {
                    areaSounds.controls.play();
                }
                areaSoundsTimer.Enabled = false;
            }
            catch (Exception ex)
            {
                game.errorLog("Failed on areaSoundsTimer_Tick()" + ex.ToString());
            }
        }
        #endregion

        private void loadPCObjects()
        {
            try
            {
                foreach (PC pc in game.playerList.PCList)
                {
                    
                    // when loading characters that have a blank "raceTag" or lists are empty, create lists and tags based on current objects
                    // this "if" statement may not be necessary once everyone has converted their PCs to the new format
                    if (pc.RaceTag == "")
                    {
                        pc.RaceTag = pc.Race.RaceTag;
                        pc.ClassTag = pc.Class.PlayerClassTag;
                        pc.KnownSpellsTags.Clear();
                        foreach (Spell sp in pc.KnownSpellsList.spellList)
                        {
                            pc.KnownSpellsTags.Add(sp.SpellTag);
                        }
                        pc.KnownTraitsTags.Clear();
                        foreach (Trait tr in pc.KnownTraitsList.traitList)
                        {
                            pc.KnownTraitsTags.Add(tr.TraitTag);
                        }
                        pc.KnownSkillRefsTags.Clear();
                        foreach (Skill sk in pc.KnownSkillsList.skillsList)
                        {
                            SkillRefs sr = new SkillRefs();
                            sr.SkillName = sk.SkillName;
                            sr.SkillTag = sk.SkillTag;
                            sr.SkillRanks = sk.Ranks;
                            pc.KnownSkillRefsTags.Add(sr);
                        }
                    }
                    pc.Race = game.module.ModuleRacesList.getRaceByTag(pc.RaceTag).DeepCopy();
                    pc.Class = game.module.ModulePlayerClassList.getPlayerClassByTag(pc.ClassTag).DeepCopy();
                    pc.KnownSpellsList.spellList.Clear();
                    foreach (string spTag in pc.KnownSpellsTags)
                    {
                        Spell sp = game.module.ModuleSpellsList.getSpellByTag(spTag).DeepCopy();
                        pc.KnownSpellsList.spellList.Add(sp);
                    }
                    pc.KnownTraitsList.traitList.Clear();
                    foreach (string trTag in pc.KnownTraitsTags)
                    {
                        Trait tr = game.module.ModuleTraitsList.getTraitByTag(trTag).DeepCopy();
                        pc.KnownTraitsList.traitList.Add(tr);
                    }
                    pc.KnownSkillsList.skillsList.Clear();
                    foreach (Skill sk in game.module.ModuleSkillsList.skillsList)
                    {
                        pc.KnownSkillsList.skillsList.Add(sk.DeepCopy());
                    }
                    foreach (SkillRefs sr in pc.KnownSkillRefsTags)
                    {
                        foreach (Skill sk in pc.KnownSkillsList.skillsList)
                        {
                            if (sk.SkillTag == sr.SkillTag)
                            {
                                sk.Ranks = sr.SkillRanks;
                            }
                        }
                    }
                    pc.KnownSkillRefsTags.Clear();
                    foreach (Skill sk in pc.KnownSkillsList.skillsList)
                    {
                        SkillRefs sr = new SkillRefs();
                        sr.SkillName = sk.SkillName;
                        sr.SkillTag = sk.SkillTag;
                        sr.SkillRanks = sk.Ranks;
                        pc.KnownSkillRefsTags.Add(sr);
                    }
                    foreach (Skill sk in pc.KnownSkillsList.skillsList)
                    {
                        sk.reCalculate(pc);
                    }
                }
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to load character from save game file");
                game.errorLog(ex.ToString());
            }
        }
        private void loadPCItems()
        {
            foreach (PC pc in game.playerList.PCList)
            {
                try { pc.Head = game.module.ModuleItemsList.getItemByTag(pc.HeadTag).DeepCopy(); }
                catch { }
                try { pc.Neck = game.module.ModuleItemsList.getItemByTag(pc.NeckTag).DeepCopy(); }
                catch { }
                try { pc.Body = game.module.ModuleItemsList.getItemByTag(pc.BodyTag).DeepCopy(); }
                catch { }
                try { pc.MainHand = game.module.ModuleItemsList.getItemByTag(pc.MainHandTag).DeepCopy(); }
                catch { }
                try { pc.OffHand = game.module.ModuleItemsList.getItemByTag(pc.OffHandTag).DeepCopy(); }
                catch { }
                try { pc.Ring1 = game.module.ModuleItemsList.getItemByTag(pc.Ring1Tag).DeepCopy(); }
                catch { }
                try { pc.Ring2 = game.module.ModuleItemsList.getItemByTag(pc.Ring2Tag).DeepCopy(); }
                catch { }
                try { pc.Feet = game.module.ModuleItemsList.getItemByTag(pc.FeetTag).DeepCopy(); }
                catch { }
            }
            if (savedGame != null)
            {
                game.partyInventoryList.Clear();
                foreach (string iTag in savedGame.partyInventoryTagList)
                {
                    try
                    {
                        Item newItem = game.module.ModuleItemsList.getItemByTag(iTag).DeepCopy();
                        game.partyInventoryList.Add(newItem);
                        game.partyInventoryTagList.Add(newItem.ItemTag);
                    }
                    catch { }
                }
            }
        }
        /*private void loadCurrentAreaBitmap()
        {
            using (Bitmap bitmap = new Bitmap(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\areas\\" + game.currentArea.MapFileName))
            {
                game.currentMapBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format32bppPArgb);
            }
        }*/
        private void timer_Tick(object sender, EventArgs e)
        {
            //check if we need some rendering
            game.RenderingLoop();
        }
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                //we animate sprites here
                //game.UpdateAnimated();
                game.areaUpdate();
                game.areaRenderAll();
            }
            catch (Exception ex)
            {
                //game.userResized = true;
                game.errorLog(ex.ToString());
            }
        }
        private void FPStimer_Tick(object sender, EventArgs e)
        {
            //show new FPS
            //txtFPS.Text = Convert.ToString(game.GetFPS());
            //AnimationTimer.Start();
        }
        private void realTimer_Tick(object sender, EventArgs e)
        {
            doScriptBasedOnFilename("dsRealTime.cs", "none", "none", "none", "none");
        }
        /*private void floatyTextTimer_Tick(object sender, EventArgs e)
        {
            //doFloatyTextTimer();
            //doFloatyTextFades();
            //game.Update();
        }*/
        /*private void doFloatyTextTimer()
        {
            //used to determine the fade in and out start times and the speed to float up
            foreach (ShadowTextObject to in game.shadowTextPool)
            {
                to.Timer++;
                to.Z = -(to.Timer / 7);
                if (to.Timer <= 1)
                {
                    to.FadeIn = true;
                    to.FadeOut = false;
                }
                if (to.Timer > to.TimeLength)
                {
                    to.FadeOut = true;
                    to.FadeIn = false;
                }
            }
            for (int i = game.shadowTextPool.Count - 1; i >= 0; i--)
            {
                if (game.shadowTextPool[i].Timer > game.shadowTextPool[i].TimeLength + 30)
                {
                    game.shadowTextPool.RemoveAt(i);
                }
            }
            //this.pictureBox1.Invalidate();
            foreach (ShadowTextObject to in game.shadowTextPool)
            {
                game.DrawTextShadowOutlineMainMap(to.X, to.Y, to.Z, to.Text, to.AlphaShadow, to.AlphaText, to.Font, to.FontPointSize, to.TextColor, to.ShadowColor);
            }
        }
        private void doFloatyTextFades()
        {
            //controls the fade in and out
            foreach (ShadowTextObject to in game.shadowTextPool)
            {
                if (to.FadeIn)
                {
                    to.AlphaShadow += 10;
                    if (to.AlphaShadow > 100)
                        to.AlphaShadow = 100;
                    to.AlphaText += 25;
                    if (to.AlphaText > 255)
                        to.AlphaText = 255;
                }
                if (to.FadeOut)
                {
                    to.AlphaShadow -= 4;
                    if (to.AlphaShadow < 0)
                        to.AlphaShadow = 0;
                    to.AlphaText -= 10;
                    if (to.AlphaText < 0)
                        to.AlphaText = 0;
                }
            }
            //this.pictureBox1.Invalidate();
            foreach (ShadowTextObject to in game.shadowTextPool)
            {
                game.DrawTextShadowOutlineMainMap(to.X, to.Y, to.Z, to.Text, to.AlphaShadow, to.AlphaText, to.Font, to.FontPointSize, to.TextColor, to.ShadowColor);
            }
        }*/
        /*public void addPCtoParty()
        {
            //TODO look in module folder first
            if (game.playerList.PCList.Count == 1)
            {                
                game.playerList.PCList[0].portraitBitmapG = game.playerList.PCList[0].LoadCharacterPortraitBitmapG(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[0].PortraitFileG);
                game.playerList.PCList[0].portraitBitmapL = game.playerList.PCList[0].LoadCharacterPortraitBitmapL(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[0].PortraitFileL);
                game.playerList.PCList[0].portraitBitmapM = game.playerList.PCList[0].LoadCharacterPortraitBitmapM(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[0].PortraitFileM);
                game.playerList.PCList[0].portraitBitmapS = game.playerList.PCList[0].LoadCharacterPortraitBitmapS(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[0].PortraitFileS);
                pc_button_0.Enabled = true;
                pcInventory.rbtnPc0.Enabled = true;
                pc_button_0.Image = (Image)game.playerList.PCList[0].portraitBitmapM;
                pcSheet0.passRefs(this, game, 0);
                pcSheet0.refreshSheet();
            }
            if (game.playerList.PCList.Count == 2)
            {
                game.playerList.PCList[1].portraitBitmapG = game.playerList.PCList[1].LoadCharacterPortraitBitmapG(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[1].PortraitFileG);
                game.playerList.PCList[1].portraitBitmapL = game.playerList.PCList[1].LoadCharacterPortraitBitmapL(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[1].PortraitFileL);
                game.playerList.PCList[1].portraitBitmapM = game.playerList.PCList[1].LoadCharacterPortraitBitmapM(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[1].PortraitFileM);
                game.playerList.PCList[1].portraitBitmapS = game.playerList.PCList[1].LoadCharacterPortraitBitmapS(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[1].PortraitFileS);
                pc_button_1.Enabled = true;
                pcInventory.rbtnPc1.Enabled = true;
                pc_button_1.Image = (Image)game.playerList.PCList[1].portraitBitmapM;
                pcSheet1.passRefs(this, game, 1);
                pcSheet1.refreshSheet();
            }
            if (game.playerList.PCList.Count == 3)
            {
                game.playerList.PCList[2].portraitBitmapG = game.playerList.PCList[2].LoadCharacterPortraitBitmapG(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[2].PortraitFileG);
                game.playerList.PCList[2].portraitBitmapL = game.playerList.PCList[2].LoadCharacterPortraitBitmapL(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[2].PortraitFileL);
                game.playerList.PCList[2].portraitBitmapM = game.playerList.PCList[2].LoadCharacterPortraitBitmapM(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[2].PortraitFileM);
                game.playerList.PCList[2].portraitBitmapS = game.playerList.PCList[2].LoadCharacterPortraitBitmapS(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[2].PortraitFileS);
                pc_button_2.Enabled = true;
                pcInventory.rbtnPc2.Enabled = true;
                pc_button_2.Image = (Image)game.playerList.PCList[2].portraitBitmapM;
                pcSheet2.passRefs(this, game, 2);
                pcSheet2.refreshSheet();
            }
            if (game.playerList.PCList.Count == 4)
            {
                game.playerList.PCList[3].portraitBitmapG = game.playerList.PCList[3].LoadCharacterPortraitBitmapG(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[3].PortraitFileG);
                game.playerList.PCList[3].portraitBitmapL = game.playerList.PCList[3].LoadCharacterPortraitBitmapL(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[3].PortraitFileL);
                game.playerList.PCList[3].portraitBitmapM = game.playerList.PCList[3].LoadCharacterPortraitBitmapM(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[3].PortraitFileM);
                game.playerList.PCList[3].portraitBitmapS = game.playerList.PCList[3].LoadCharacterPortraitBitmapS(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[3].PortraitFileS);
                pc_button_3.Enabled = true;
                pcInventory.rbtnPc3.Enabled = true;
                pc_button_3.Image = (Image)game.playerList.PCList[3].portraitBitmapM;
                pcSheet3.passRefs(this, game, 3);
                pcSheet3.refreshSheet();
            }
            pcInventory.refreshPanelInfo();
            doPortraitStats();
        }*/
        public void refreshPartyButtons()
        {
            try
            {
                if (game.playerList.PCList.Count >= 1)
                {
                    pc_button_0.Visible = true;
                    pcInventory.rbtnPc0.Visible = true;
                }
                if (game.playerList.PCList.Count >= 2)
                {
                    pc_button_1.Visible = true;
                    pcInventory.rbtnPc1.Visible = true;
                }
                if (game.playerList.PCList.Count >= 3)
                {
                    pc_button_2.Visible = true;
                    pcInventory.rbtnPc2.Visible = true;
                }
                if (game.playerList.PCList.Count >= 4)
                {                    
                    pc_button_3.Visible = true;
                    pcInventory.rbtnPc3.Visible = true;
                }
                if (game.playerList.PCList.Count >= 5)
                {
                    pc_button_4.Visible = true;
                    pcInventory.rbtnPc4.Visible = true;
                }
                if (game.playerList.PCList.Count >= 6)
                {                    
                    pc_button_5.Visible = true;
                    pcInventory.rbtnPc5.Visible = true;
                }
                //TODO check module folder first
                if (game.playerList.PCList.Count >= 1)
                {
                    game.playerList.PCList[0].LoadAllPcStuff(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName);
                    //game.playerList.PCList[0].portraitBitmapG = game.playerList.PCList[0].LoadCharacterPortraitBitmapG(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[0].PortraitFileG);
                    //game.playerList.PCList[0].portraitBitmapL = game.playerList.PCList[0].LoadCharacterPortraitBitmapL(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[0].PortraitFileL);
                    //game.playerList.PCList[0].portraitBitmapM = game.playerList.PCList[0].LoadCharacterPortraitBitmapM(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[0].PortraitFileM);
                    //game.playerList.PCList[0].portraitBitmapS = game.playerList.PCList[0].LoadCharacterPortraitBitmapS(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[0].PortraitFileS);
                    pc_button_0.Enabled = true;
                    pcInventory.rbtnPc0.Enabled = true;
                    //pc_button_0.Image = (Image)game.playerList.PCList[0].portraitBitmapM;
                    pc_button_0.BackgroundImage = (Image)game.playerList.PCList[0].portraitBitmapM;
                    pcSheet0.passRefs(this, game, 0);
                    pcSheet0.refreshSheet();
                }
                if (game.playerList.PCList.Count >= 2)
                {
                    game.playerList.PCList[1].LoadAllPcStuff(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName);
                    //game.playerList.PCList[1].portraitBitmapG = game.playerList.PCList[1].LoadCharacterPortraitBitmapG(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[1].PortraitFileG);
                    //game.playerList.PCList[1].portraitBitmapL = game.playerList.PCList[1].LoadCharacterPortraitBitmapL(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[1].PortraitFileL);
                    //game.playerList.PCList[1].portraitBitmapM = game.playerList.PCList[1].LoadCharacterPortraitBitmapM(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[1].PortraitFileM);
                    //game.playerList.PCList[1].portraitBitmapS = game.playerList.PCList[1].LoadCharacterPortraitBitmapS(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[1].PortraitFileS);
                    pc_button_1.Enabled = true;
                    pcInventory.rbtnPc1.Enabled = true;
                    pc_button_1.BackgroundImage = (Image)game.playerList.PCList[1].portraitBitmapM;
                    pcSheet1.passRefs(this, game, 1);
                    pcSheet1.refreshSheet();
                }
                if (game.playerList.PCList.Count >= 3)
                {
                    game.playerList.PCList[2].LoadAllPcStuff(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName);
                    //game.playerList.PCList[2].portraitBitmapG = game.playerList.PCList[2].LoadCharacterPortraitBitmapG(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[2].PortraitFileG);
                    //game.playerList.PCList[2].portraitBitmapL = game.playerList.PCList[2].LoadCharacterPortraitBitmapL(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[2].PortraitFileL);
                    //game.playerList.PCList[2].portraitBitmapM = game.playerList.PCList[2].LoadCharacterPortraitBitmapM(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[2].PortraitFileM);
                    //game.playerList.PCList[2].portraitBitmapS = game.playerList.PCList[2].LoadCharacterPortraitBitmapS(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[2].PortraitFileS);
                    pc_button_2.Enabled = true;
                    pcInventory.rbtnPc2.Enabled = true;
                    pc_button_2.BackgroundImage = (Image)game.playerList.PCList[2].portraitBitmapM;
                    pcSheet2.passRefs(this, game, 2);
                    pcSheet2.refreshSheet();
                }
                if (game.playerList.PCList.Count >= 4)
                {
                    game.playerList.PCList[3].LoadAllPcStuff(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName);
                    //game.playerList.PCList[3].portraitBitmapG = game.playerList.PCList[3].LoadCharacterPortraitBitmapG(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[3].PortraitFileG);
                    //game.playerList.PCList[3].portraitBitmapL = game.playerList.PCList[3].LoadCharacterPortraitBitmapL(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[3].PortraitFileL);
                    //game.playerList.PCList[3].portraitBitmapM = game.playerList.PCList[3].LoadCharacterPortraitBitmapM(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[3].PortraitFileM);
                    //game.playerList.PCList[3].portraitBitmapS = game.playerList.PCList[3].LoadCharacterPortraitBitmapS(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[3].PortraitFileS);
                    pc_button_3.Enabled = true;
                    pcInventory.rbtnPc3.Enabled = true;
                    pc_button_3.BackgroundImage = (Image)game.playerList.PCList[3].portraitBitmapM;
                    pcSheet3.passRefs(this, game, 3);
                    pcSheet3.refreshSheet();
                }
                if (game.playerList.PCList.Count >= 5)
                {
                    game.playerList.PCList[4].LoadAllPcStuff(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName);
                    //game.playerList.PCList[4].portraitBitmapG = game.playerList.PCList[4].LoadCharacterPortraitBitmapG(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[4].PortraitFileG);
                    //game.playerList.PCList[4].portraitBitmapL = game.playerList.PCList[4].LoadCharacterPortraitBitmapL(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[4].PortraitFileL);
                    //game.playerList.PCList[4].portraitBitmapM = game.playerList.PCList[4].LoadCharacterPortraitBitmapM(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[4].PortraitFileM);
                    //game.playerList.PCList[4].portraitBitmapS = game.playerList.PCList[4].LoadCharacterPortraitBitmapS(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[4].PortraitFileS);
                    pc_button_4.Enabled = true;
                    pcInventory.rbtnPc4.Enabled = true;
                    pc_button_4.BackgroundImage = (Image)game.playerList.PCList[4].portraitBitmapM;
                    pcSheet4.passRefs(this, game, 4);
                    pcSheet4.refreshSheet();
                }
                if (game.playerList.PCList.Count >= 6)
                {
                    game.playerList.PCList[5].LoadAllPcStuff(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName);
                    //game.playerList.PCList[5].portraitBitmapG = game.playerList.PCList[5].LoadCharacterPortraitBitmapG(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[5].PortraitFileG);
                    //game.playerList.PCList[5].portraitBitmapL = game.playerList.PCList[5].LoadCharacterPortraitBitmapL(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[5].PortraitFileL);
                    //game.playerList.PCList[5].portraitBitmapM = game.playerList.PCList[5].LoadCharacterPortraitBitmapM(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[5].PortraitFileM);
                    //game.playerList.PCList[5].portraitBitmapS = game.playerList.PCList[5].LoadCharacterPortraitBitmapS(game.mainDirectory + "\\portraits\\" + game.playerList.PCList[5].PortraitFileS);
                    pc_button_5.Enabled = true;
                    pcInventory.rbtnPc5.Enabled = true;
                    pc_button_5.BackgroundImage = (Image)game.playerList.PCList[5].portraitBitmapM;
                    pcSheet5.passRefs(this, game, 5);
                    pcSheet5.refreshSheet();
                }
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, ex.ToString());
            }
            pcInventory.refreshPanelInfo();
            doPortraitStats();
        }
        private void doLoadEncountersStuff()
        {
            foreach (Encounter encntr in game.module.ModuleEncountersList.encounters)
            {
                foreach (Creature crt in encntr.EncounterCreatureList.creatures)
                {
                    crt.passRefs(game, null);
                    crt.LoadSpriteStuff(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName);
                    
                    /*if (File.Exists(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\graphics\\sprites\\tokens\\module\\" + crt.CharSprite.SpriteSheetFilename))
                    {
                        crt.CharSprite.Image = new Bitmap(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\graphics\\sprites\\tokens\\module\\" + crt.CharSprite.SpriteSheetFilename);
                    }
                    else if (File.Exists(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\graphics\\sprites\\tokens\\" + crt.CharSprite.SpriteSheetFilename))
                    {
                        crt.CharSprite.Image = new Bitmap(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\graphics\\sprites\\tokens\\" + crt.CharSprite.SpriteSheetFilename);
                    }
                    else
                    {
                        crt.CharSprite.Image = new Bitmap(game.mainDirectory + "\\data\\graphics\\sprites\\tokens\\" + crt.CharSprite.SpriteSheetFilename);
                    }*/
                    //crt.passRefs(game, null);
                    //crt.CharSprite.passRefs(game);
                }
                foreach (Prop prp in encntr.EncounterPropList.propsList)
                {
                    prp.PropSprite.Image = new Bitmap(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\graphics\\sprites\\props\\" + prp.PropSprite.SpriteSheetFilename);
                    prp.passRefs(game, null);
                    prp.PropSprite.passRefs(game);
                }                
                // foreach through Inventory Tag list and add items to the Inventory List
                foreach (Item encItem in encntr.EncounterInventoryList)
                {
                    encItem.passRefs(game, null);
                }
            }
        }
        private void doLoadEncounters()
        {
            foreach (Encounter encntr in game.module.ModuleEncountersList.encounters)
            {
                foreach (CreatureRefs crtRef in encntr.EncounterCreatureRefsList)
                {
                    Creature newCrt = new Creature();
                    newCrt.passRefs(game, null);
                    newCrt = game.module.ModuleCreaturesList.getCreatureByResRef(crtRef.CreatureResRef).DeepCopy();
                    newCrt.passRefs(game, null);
                    newCrt.CombatLocation = crtRef.CreatureStartLocation;
                    newCrt.Tag = crtRef.CreatureTag;
                    newCrt.LoadSpriteStuff(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName);
                    //newCrt.CharSprite.passRefs(game);
                    newCrt.UpdateSimpleStats();
                    encntr.EncounterCreatureList.creatures.Add(newCrt);
                }
                foreach (PropRefs prpRef in encntr.EncounterPropRefsList)
                {
                    Prop newProp = new Prop();
                    newProp.passRefs(game, null);
                    newProp = game.module.ModulePropsList.getPropByResRef(prpRef.PropResRef).DeepCopy();
                    newProp.passRefs(game, null);
                    newProp.Location = prpRef.PropStartLocation;
                    newProp.PropContainerChk = prpRef.PropContainerChk;
                    newProp.PropContainerTag = prpRef.PropContainerTag;
                    newProp.PropTrapped = prpRef.PropTrappedChk;
                    newProp.PropLocked = prpRef.PropLockedChk;
                    newProp.PropKeyTag = prpRef.PropKeyTag;
                    newProp.PropTag = prpRef.PropTag;
                    //load spritesheet and pass refs 
                    newProp.LoadPropSpriteStuff(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName);
                    /*if (File.Exists(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\graphics\\sprites\\props\\" + newProp.PropSpriteFilename))
                    {
                        newProp.LoadSpriteStuff(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\graphics\\sprites\\props");
                        //newProp.PropSprite.Image = new Bitmap(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\graphics\\sprites\\props\\" + newProp.PropSprite.SpriteSheetFilename);
                    }
                    else if (File.Exists(game.mainDirectory + "\\data\\graphics\\sprites\\props\\" + newProp.PropSpriteFilename))
                    {
                        newProp.LoadSpriteStuff(game.mainDirectory + "\\data\\graphics\\sprites\\props");
                        //newProp.PropSprite.Image = new Bitmap(game.mainDirectory + "\\data\\graphics\\sprites\\props\\" + newProp.PropSprite.SpriteSheetFilename);
                    }
                    else
                    {
                        IBMessageBox.Show(game, "Failed to find prop sprite (" + newProp.PropSpriteFilename + ") in data or module folders");
                    }*/
                    //newProp.PropSprite.passRefs(game);
                    encntr.EncounterPropList.propsList.Add(newProp);
                }
                // foreach through Inventory Tag list and add items to the Inventory List
                foreach (string encItem in encntr.EncounterInventoryTagList)
                {
                    Item newItem = new Item();
                    newItem.passRefs(game, null);
                    newItem = game.module.ModuleItemsList.getItemByTag(encItem).DeepCopy();
                    encntr.EncounterInventoryList.Add(newItem);
                }
            }
        }
        private void doLoadAreaObjects()
        {
            foreach (Area a in game.module.ModuleAreasObjects)
            {
                foreach (CreatureRefs crtRef in a.AreaCreatureRefsList)
                {
                    Creature newCrt = new Creature();
                    newCrt.passRefs(game, null);
                    newCrt = game.module.ModuleCreaturesList.getCreatureByResRef(crtRef.CreatureResRef).DeepCopy();
                    newCrt.passRefs(game, null);
                    newCrt.MapLocation = crtRef.CreatureStartLocation;
                    newCrt.Tag = crtRef.CreatureTag;
                    if (crtRef.MouseOverText != "")
                    {
                        newCrt.MouseOverText = crtRef.MouseOverText;
                    }
                    //load spritesheet and pass refs
                    newCrt.LoadSpriteStuff(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName);
                    /*if (File.Exists(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\graphics\\sprites\\tokens\\module\\" + newCrt.CharSprite.SpriteSheetFilename))
                    {
                        newCrt.CharSprite.Image = new Bitmap(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\graphics\\sprites\\tokens\\module\\" + newCrt.CharSprite.SpriteSheetFilename);
                    }
                    else if (File.Exists(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\graphics\\sprites\\tokens\\" + newCrt.CharSprite.SpriteSheetFilename))
                    {
                        newCrt.CharSprite.Image = new Bitmap(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\graphics\\sprites\\tokens\\" + newCrt.CharSprite.SpriteSheetFilename);
                    }
                    else if (File.Exists(game.mainDirectory + "\\data\\graphics\\sprites\\tokens\\" + newCrt.CharSprite.SpriteSheetFilename))
                    {
                        newCrt.CharSprite.Image = new Bitmap(game.mainDirectory + "\\data\\graphics\\sprites\\tokens\\" + newCrt.CharSprite.SpriteSheetFilename);
                    }
                    else
                    {
                        IBMessageBox.Show(game, "Failed to find creature sprite (" + newCrt.CharSprite.SpriteSheetFilename + ") in data or module folders");
                    }
                    newCrt.CharSprite.passRefs(game);*/
                    a.AreaCreatureList.creatures.Add(newCrt);
                }
                foreach (PropRefs prpRef in a.AreaPropRefsList)
                {
                    Prop newProp = new Prop();
                    newProp.passRefs(game, null);
                    newProp = game.module.ModulePropsList.getPropByResRef(prpRef.PropResRef).DeepCopy();
                    newProp.passRefs(game, null);
                    newProp.Location = prpRef.PropStartLocation;
                    newProp.PropContainerChk = prpRef.PropContainerChk;
                    newProp.PropContainerTag = prpRef.PropContainerTag;
                    newProp.PropTrapped = prpRef.PropTrappedChk;
                    newProp.PropLocked = prpRef.PropLockedChk;
                    newProp.PropKeyTag = prpRef.PropKeyTag;
                    newProp.PropTag = prpRef.PropTag;
                    if (prpRef.MouseOverText != "")
                    {
                        newProp.MouseOverText = prpRef.MouseOverText;
                    }
                    //load spritesheet and pass refs    
                    newProp.LoadPropSpriteStuff(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName);
                    /*if (File.Exists(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\graphics\\sprites\\props\\" + newProp.PropSpriteFilename))
                    {
                        newProp.LoadSpriteStuff(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\graphics\\sprites\\props");
                        //newProp.PropSprite.Image = new Bitmap(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\graphics\\sprites\\props\\" + newProp.PropSprite.SpriteSheetFilename);
                    }
                    else if (File.Exists(game.mainDirectory + "\\data\\graphics\\sprites\\props\\" + newProp.PropSpriteFilename))
                    {
                        newProp.LoadSpriteStuff(game.mainDirectory + "\\data\\graphics\\sprites\\props");
                        //newProp.PropSprite.Image = new Bitmap(game.mainDirectory + "\\data\\graphics\\sprites\\props\\" + newProp.PropSprite.SpriteSheetFilename);
                    }
                    else
                    {
                        IBMessageBox.Show(game, "Failed to find prop sprite (" + newProp.PropSpriteFilename + ") in data or module folders");
                    }*/
                    //newProp.PropSprite.passRefs(game);
                    a.AreaPropList.propsList.Add(newProp);
                }
            }
        }
        private void doLoadContainers()
        {
            foreach (Container cntnr in game.module.ModuleContainersList.containers)
            {
                if (cntnr.containerItemTags.Count == 0)
                {
                    foreach (string s in cntnr.items)
                    {
                        Item newItem = game.module.ModuleItemsList.getItem(s);
                        cntnr.containerItemTags.Add(newItem.ItemTag);                    
                    }
                }
                cntnr.containerInventoryList.Clear();
                for (int i = 0; i < cntnr.containerItemTags.Count; i++)
                {
                    Item newItem = game.module.ModuleItemsList.getItemByTag(cntnr.containerItemTags[i]).DeepCopy();
                    cntnr.containerInventoryList.Add(newItem);
                }
            }
        }
        private void doLoadShops()
        {
            foreach (Shop shp in game.module.ModuleShopsList.shopsList)
            {
                shp.shopItemObjectsList.Clear();
                for (int i = 0; i < shp.shopItemTags.Count; i++)
                {
                    Item newItem = game.module.ModuleItemsList.getItemByTag(shp.shopItemTags[i]).DeepCopy();
                    shp.shopItemObjectsList.Add(newItem);
                }
            }
        }
        public void logMainText(string text, Color color)
        {
            debugForm.rtxtMainLog.SelectionColor = color;
            debugForm.rtxtMainLog.AppendText(text);
            debugForm.rtxtMainLog.ScrollToCaret();
        }
        public void logText(string text, Color color)
        {
            this.rtxtLog.SelectionColor = color;
            this.rtxtLog.AppendText(text);
            this.rtxtLog.ScrollToCaret();
            this.rtxtLog.Invalidate();
        }        
        public Font ChangeFontSize(Font font, float scale)
        {
            if (font != null)
            {
                float currentSize = font.Size;
                if (currentSize != (font.Size * scale))
                {
                    font = new Font(font.Name, font.Size * scale, font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
                }
            }
            return font;
        }
        public static Font ChangeFontSizeStatic(Font font, float scale)
        {
            if (font != null)
            {
                float currentSize = font.Size;
                if (currentSize != (font.Size * scale))
                {
                    font = new Font(font.Name, font.Size * scale, font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
                }
            }
            return font;
        }
        
        #region Movement Stuff
        private bool moveDelay()
        {
            long elapsed = DateTime.Now.Ticks - timeStamp;
            if (elapsed > 10000 * movementDelayInMiliseconds) //10,000 ticks in 1 ms
            {                
                timeStamp = DateTime.Now.Ticks;
                return true;
            }
            return false;
        }
        private void walkingPcAnimation()
        {
            if (game.playerList.PCList[game.selectedPartyLeader].CharSprite.WalkingNumberOfFrames > 1)
            {
                //AnimationTimer.Stop();
                int sleep = 1000 / game.playerList.PCList[game.selectedPartyLeader].CharSprite.WalkingFPS;
                int lx = game.lastPlayerLocation.X;
                int ly = game.lastPlayerLocation.Y;
                int cx = game.playerPosition.X;
                int cy = game.playerPosition.Y;
                double increment = (double)game._squareSize / (double)game.playerList.PCList[game.selectedPartyLeader].CharSprite.WalkingNumberOfFrames;
                //start a for loop based on the number of frames in the walking row
                for (int i = 0; i < game.playerList.PCList[game.selectedPartyLeader].CharSprite.WalkingNumberOfFrames; i++)
                {
                    if (lx != cx) //moved left or right
                    {
                        if (lx > cx) //moved left x--
                        {
                            game.areaPcAnimateRenderAll((int)(lx * game._squareSize - (i * increment) - increment), ly * game._squareSize, game.selectedPartyLeader, i);
                            //game.spritePcDrawFrame((int)(lx * game._squareSize - (i * increment) - increment), ly * game._squareSize, 0, i);
                        }
                        else //moved right x++
                        {
                            game.areaPcAnimateRenderAll((int)(lx * game._squareSize + (i * increment) + increment), ly * game._squareSize, game.selectedPartyLeader, i);
                            //game.spritePcDrawFrame((int)(lx * game._squareSize + (i * increment) + increment), ly * game._squareSize, 0, i);
                        }
                    }
                    else //moved up or down
                    {
                        if (ly > cy) //moved up y--
                        {
                            game.areaPcAnimateRenderAll(lx * game._squareSize, (int)(ly * game._squareSize - (i * increment) - increment), game.selectedPartyLeader, i);
                            //game.spritePcDrawFrame(lx * game._squareSize, (int)(ly * game._squareSize - (i * increment) - increment), 0, i);
                        }
                        else //moved down y++
                        {
                            game.areaPcAnimateRenderAll(lx * game._squareSize, (int)(ly * game._squareSize + (i * increment) + increment), game.selectedPartyLeader, i);
                            //game.spritePcDrawFrame(lx * game._squareSize, (int)(ly * game._squareSize + (i * increment) + increment), 0, i);
                        }
                    }
                    Thread.Sleep(sleep);
                }
                //AnimationTimer.Start();
            }
        }
        private void moveLeft()
        {
            finishedMove = false;
            if (game.playerPosition.X > 0)
            {
                game.playerList.PCList[game.selectedPartyLeader].Facing = CharBase.facing.Left;
                if (game.currentArea.GetBlocked(game.playerPosition.X - 1, game.playerPosition.Y) == false)
                {
                    game.playerPosition.X--;
                    walkingPcAnimation();
                }
                else
                {
                    checkForConversation(game.playerPosition.X - 1, game.playerPosition.Y);
                    //blocked and could be a secret door, see if can find
                    //MessageBox.Show("you hit a wall");
                }
            }
            doUpdate();
            autoSave();
        }
        private void moveRight()
        {
            int mapwidth = game.currentArea.MapSizeInSquares.Width;
            finishedMove = false;
            if (game.playerPosition.X < (mapwidth - 1))
            {
                game.playerList.PCList[game.selectedPartyLeader].Facing = CharBase.facing.Right;
                if (game.currentArea.GetBlocked(game.playerPosition.X + 1, game.playerPosition.Y) == false)
                {
                    game.playerPosition.X++;
                    walkingPcAnimation();
                }
                else
                {
                    checkForConversation(game.playerPosition.X + 1, game.playerPosition.Y);
                    //MessageBox.Show("you hit a wall");
                }
            }
            doUpdate();
            autoSave();
        }
        private void moveUp()
        {
            finishedMove = false;
            if (game.playerPosition.Y > 0)
            {
                if (game.playerList.PCList[game.selectedPartyLeader].CharSprite.TopDown)
                {
                    game.playerList.PCList[game.selectedPartyLeader].Facing = CharBase.facing.Up;
                }
                if (game.currentArea.GetBlocked(game.playerPosition.X, game.playerPosition.Y - 1) == false)
                {
                    game.playerPosition.Y--;
                    walkingPcAnimation();
                }
                else
                {
                    checkForConversation(game.playerPosition.X, game.playerPosition.Y - 1);
                    //MessageBox.Show("you hit a wall");
                }
            }
            doUpdate();
            autoSave();
        }
        private void moveDown()
        {
            int mapheight = game.currentArea.MapSizeInSquares.Height;
            finishedMove = false;
            if (game.playerPosition.Y < (mapheight - 1))
            {
                if (game.playerList.PCList[game.selectedPartyLeader].CharSprite.TopDown)
                {
                    game.playerList.PCList[game.selectedPartyLeader].Facing = CharBase.facing.Down;
                }
                if (game.currentArea.GetBlocked(game.playerPosition.X, game.playerPosition.Y + 1) == false)
                {
                    game.playerPosition.Y++;
                    walkingPcAnimation();
                }
                else
                {
                    checkForConversation(game.playerPosition.X, game.playerPosition.Y + 1);
                    //MessageBox.Show("you hit a wall");
                }
            }
            doUpdate();
            autoSave();
        }
        private void renderPanel_MouseDown(object sender, MouseEventArgs e)
        {
            int gridx = e.X / game._squareSize;
            int gridy = e.Y / game._squareSize;
            Point selectedPoint = new Point(gridx, gridy);
            Point upperLeftPixel = game.UpperLeftPixels();
            int delX = selectedPoint.X + (upperLeftPixel.X / game._squareSize) - game.playerPosition.X;
            int absX = Math.Abs(delX);
            int delY = selectedPoint.Y + (upperLeftPixel.Y / game._squareSize) - game.playerPosition.Y;
            int absY = Math.Abs(delY);
            //if delX>delY then left or right, left if delX < 0
            //else up or down, up if delY < 0
            if (e.Button == MouseButtons.Left)
            {
                if ((moveDelay()) && (finishedMove))
                {
                    game.lastPlayerLocation = new Point(game.playerPosition.X, game.playerPosition.Y);                    
                    if (absX > absY) //delX > delY then left or right, left if delX < 0
                    {
                        if (delX < 0) //move Left
                        {
                            moveLeft();
                        }
                        else //move Right
                        {
                            moveRight();
                        }
                    }
                    else //else up or down, up if delY < 0
                    {
                        if (delY < 0) //move Up
                        {
                            moveUp();
                        }
                        else //move Down
                        {
                            moveDown();
                        }
                    }
                }
            }
        }
        private void renderPanel_MouseMove(object sender, MouseEventArgs e)
        {
        	// * sinopip, 20.12.14
        	mouseX = e.X;
        	mouseY = e.Y;
        	//
            int gridx = (e.X + game.upperLeftPixel.X) / game._squareSize;
            int gridy = (e.Y + game.upperLeftPixel.Y) / game._squareSize;
            if (gridx < 0) { gridx = 0; }
            if (gridy < 0) { gridy = 0; }
            if (gridx > (game.currentArea.MapSizeInSquares.Width - 1)) { gridx = (game.currentArea.MapSizeInSquares.Width - 1); }
            if (gridy > (game.currentArea.MapSizeInSquares.Height - 1)) { gridy = (game.currentArea.MapSizeInSquares.Height - 1); }
            game.mouseMainMapLocation = new Point(gridx, gridy);
            
            // * sinopip, 20.12.14
            // enable scroll area when mouse is on borders (scrollbar position values are negative)
        	if (mouseX < 100 + -renderPanel.AutoScrollPosition.X) 
        		is_leftscrolling = true;
        	else is_leftscrolling = false;
        	if (mouseY < 100 + -renderPanel.AutoScrollPosition.Y) 
        		is_upscrolling = true;
        	else is_upscrolling = false;
        	if (mouseX > (renderPanel.Width-100) + -renderPanel.AutoScrollPosition.X)
        		is_rightscrolling = true;
        	else is_rightscrolling = false;
        	if (mouseY > (renderPanel.Height-100) + -renderPanel.AutoScrollPosition.Y)
        		is_downscrolling = true;
        	else is_downscrolling = false;
			//        	            
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.U)
            {
                game.DrawFloatyText("This is the new font size ABC0123456789, This is the new font size ABC0123456789, This is the new font size ABC0123456789", game.playerPosition.X * game._squareSize - 150 + 32, game.playerPosition.Y * game._squareSize - 20, 200, Color.White, Color.Black);
            }
            if (keyData == Keys.R)
            {
                game.ResetMainMapAll(this.renderPanel);
                logText("Reset DX9 Renderer", Color.Red);
                logText(Environment.NewLine, Color.Black);
            }
            if (keyData == Keys.S)
            {
                if (btnAdvLogFontIncrease.Visible)
                {
                    btnAdvLogFontIncrease.Visible = false;
                    btnAdvLogFontDecrease.Visible = false;
                    logText("Font Scaling Buttons Hidden", Color.Lime);
                    logText(Environment.NewLine, Color.Black);
                }
                else
                {
                    btnAdvLogFontIncrease.Visible = true;
                    btnAdvLogFontDecrease.Visible = true;
                    logText("Font Scaling Buttons Visible", Color.Lime);
                    logText(Environment.NewLine, Color.Black);
                }
            }
            if (keyData == Keys.T)
            {
                if (touchScreenFeatures)
                {
                    btnQuickSave.Visible = false;
                    touchScreenFeatures = false;
                    logText("Touch Screen Features Turned Off", Color.Lime);
                    logText(Environment.NewLine, Color.Black);
                }
                else
                {
                    btnQuickSave.Visible = true;
                    touchScreenFeatures = true;
                    logText("Touch Screen Features Turned On", Color.Lime);
                    logText(Environment.NewLine, Color.Black);
                }
            }
            if ((moveDelay()) && (finishedMove))
            {                
                game.lastPlayerLocation = new Point(game.playerPosition.X, game.playerPosition.Y);
                int mapwidth = game.currentArea.MapSizeInSquares.Width;
                int mapheight = game.currentArea.MapSizeInSquares.Height;
                #region Left
                if (keyData == Keys.Left | keyData == Keys.D4 | keyData == Keys.NumPad4)
                {
                    moveLeft();
                    /*
                    finishedMove = false;
                    if (game.playerPosition.X > 0)
                    {
                        game.playerList.PCList[game.selectedPartyLeader].Facing = CharBase.facing.Left;
                        if (game.currentArea.GetBlocked(game.playerPosition.X - 1, game.playerPosition.Y) == false)
                        {                            
                            game.playerPosition.X--;
                            walkingPcAnimation();
                        }
                        else
                        {
                            checkForConversation(game.playerPosition.X - 1, game.playerPosition.Y);
                            //blocked and could be a secret door, see if can find
                            //MessageBox.Show("you hit a wall");
                        }
                    }
                    doUpdate();
                    autoSave();
                    */
                    return true; //for the active control to see the keypress, return false 
                }
                #endregion
                #region Right
                else if (keyData == Keys.Right | keyData == Keys.D6 | keyData == Keys.NumPad6)
                {
                    moveRight();
                    /*
                    finishedMove = false;
                    if (game.playerPosition.X < (mapwidth - 1))
                    {
                        game.playerList.PCList[game.selectedPartyLeader].Facing = CharBase.facing.Right;
                        if (game.currentArea.GetBlocked(game.playerPosition.X + 1, game.playerPosition.Y) == false)
                        {                            
                            game.playerPosition.X++;
                            walkingPcAnimation();
                        }
                        else
                        {
                            checkForConversation(game.playerPosition.X + 1, game.playerPosition.Y);
                            //MessageBox.Show("you hit a wall");
                        }
                    }
                    doUpdate();
                    autoSave();
                    */
                    return true; //for the active control to see the keypress, return false 
                }
                #endregion
                #region Up
                else if (keyData == Keys.Up | keyData == Keys.D8 | keyData == Keys.NumPad8)
                {
                    moveUp();
                    /*
                    finishedMove = false;
                    if (game.playerPosition.Y > 0)
                    {
                        if (game.playerList.PCList[game.selectedPartyLeader].CharSprite.TopDown)
                        {
                            game.playerList.PCList[game.selectedPartyLeader].Facing = CharBase.facing.Up;
                        }
                        if (game.currentArea.GetBlocked(game.playerPosition.X, game.playerPosition.Y - 1) == false)
                        {                            
                            game.playerPosition.Y--;
                            walkingPcAnimation();
                        }
                        else
                        {
                            checkForConversation(game.playerPosition.X, game.playerPosition.Y - 1);
                            //MessageBox.Show("you hit a wall");
                        }
                    }
                    doUpdate();
                    autoSave();
                    */
                    return true; //for the active control to see the keypress, return false 
                }
                #endregion
                #region Down
                else if (keyData == Keys.Down | keyData == Keys.D2 | keyData == Keys.NumPad2)
                {
                    moveDown();
                    /*
                    finishedMove = false;
                    if (game.playerPosition.Y < (mapheight - 1))
                    {
                        if (game.playerList.PCList[game.selectedPartyLeader].CharSprite.TopDown)
                        {
                            game.playerList.PCList[game.selectedPartyLeader].Facing = CharBase.facing.Down;
                        }
                        if (game.currentArea.GetBlocked(game.playerPosition.X, game.playerPosition.Y + 1) == false)
                        {                            
                            game.playerPosition.Y++;
                            walkingPcAnimation();
                        }
                        else
                        {
                            checkForConversation(game.playerPosition.X, game.playerPosition.Y + 1);
                            //MessageBox.Show("you hit a wall");
                        }
                    }
                    doUpdate();
                    autoSave();
                    */
                    return true; //for the active control to see the keypress, return false 
                }
                #endregion      
                else { }
            }
            if (keyData == Keys.Q)
            {
                quickSave();
                game.DrawFloatyText("QUICKSAVED", game.playerPosition.X * game._squareSize - 150 + 32, game.playerPosition.Y * game._squareSize, 50, Color.Green, Color.Black);
                logText("QUICKSAVED", Color.Lime);
                logText(Environment.NewLine, Color.Black);
                //IBMessageBox.Show(game, "QuickSave Completed");
            }
            if (keyData == Keys.A)
            {
                if (autosave)
                { 
                    autosave = false;
                    logText("AutoSave Turned Off", Color.Lime);
                    logText(Environment.NewLine, Color.Black);
                    //IBMessageBox.Show(game, "AutoSave Turned Off");
                }
                else 
                { 
                    autosave = true;
                    logText("AutoSave Turned On", Color.Lime);
                    logText(Environment.NewLine, Color.Black);
                    //IBMessageBox.Show(game, "AutoSave Turned On");
                }
            }
            if (keyData == Keys.D)
            {
                if (debugMode)
                {
                    debugMode = false;
                    logText("DebugMode Turned Off", Color.Lime);
                    logText(Environment.NewLine, Color.Black);
                }
                else
                {
                    debugMode = true;
                    logText("DebugMode Turned On", Color.Lime);
                    logText(Environment.NewLine, Color.Black);
                }
            }
            if (keyData == Keys.I)
            {
                if (inventoryOpen)
                {
                    inventoryOpen = false;
                    pcInventory.Hide();
                    return true;
                }
                else
                {
                    inventoryOpen = true;
                    openInventory();
                    return true;
                }
            }
            if (keyData == Keys.J)
            {
                if (journalOpen)
                {
                    journalOpen = false;
                    pcJournalScreen.Hide();
                    return true;
                }
                else
                {
                    journalOpen = true;
                    openJournal();
                    return true;
                }
            }
            else if (keyData == Keys.Z)
            {
                if (debugOpen)
                {
                    debugForm.Hide();
                    debugOpen = false;
                }
                else
                {
                    debugForm.Show();
                    debugOpen = true;
                }
                return true;
            }
            else
                return base.ProcessCmdKey(ref msg, keyData);
        }
        private void checkForConversation(int futureX, int futureY)
        {
            //blocked and could be a NPC or object with convo, see if they want to talk
            if (game.currentArea.GetPropConversable(futureX, futureY) == true)
            {
                //find the Prop
                try
                {
                    Prop convoProp = game.currentArea.getPropByLocation(futureX, futureY);
                    doConversationBasedOnObject(convoProp.PropTag);
                    //startPropConversation(convoProp);
                }
                catch (Exception ex)
                {
                    IBMessageBox.Show(game, "failed to start conversation with Prop");
                    game.errorLog(ex.ToString());
                }
            }
            if (game.currentArea.GetCreatureConversable(futureX, futureY) == true)
            {
                try
                {
                    Creature convoCrt = game.currentArea.getCreatureByLocation(futureX, futureY);
                    doConversationBasedOnObject(convoCrt.Tag);
                    //startCreatureConversation(convoCrt);
                }
                catch (Exception ex)
                {
                    IBMessageBox.Show(game, "failed to start conversation with Creature");
                    game.errorLog(ex.ToString());
                }
            }
        }
        private void startPropConversation(Prop prp) //not used anymore, can delete
        {
            try
            {
                ConversationDialogBox t = new ConversationDialogBox(game, this, prp.PropConversationTag);
                t.BackgroundImage = currentTheme.StandardThemeBitmap;
                t.ShowDialog();
                /*if (game.addPCScriptFired)
                {
                    addPCtoParty();
                    game.currentArea.AreaPropList.propsList.Remove(prp);
                    game.addPCScriptFired = false;
                    doUpdate();
                }*/
                if (game.uncheckConvo)
                {
                    prp.PropConversationChk = false;
                    game.uncheckConvo = false;
                    doUpdate();
                }
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to start conversation");
                game.errorLog(ex.ToString());
            }
        }
        private void startCreatureConversation(Creature crt) //not used anymore, can delete
        {
            try
            {
                ConversationDialogBox t = new ConversationDialogBox(game, this, crt.ConversationTag);
                t.BackgroundImage = currentTheme.StandardThemeBitmap;
                t.ShowDialog();
                if (game.uncheckConvo)
                {
                    crt.ConversationChk = false;
                    game.uncheckConvo = false;
                    doUpdate();
                }
                if (game.addPCScriptFired)
                {
                    //addPCtoParty();
                    game.addPCScriptFired = false;
                    game.currentArea.AreaCreatureList.creatures.Remove(crt);
                    doUpdate();
                    return;
                }
                /*if (game.addPCScriptFired)
                {
                    addPCtoParty();
                    game.currentArea.AreaCreatureList.creatures.Remove(crt);
                    game.addPCScriptFired = false;
                    doUpdate();
                    return;
                }*/
                if (game.removeCreature)
                {
                    game.removeCreature = false;
                    game.currentArea.AreaCreatureList.creatures.Remove(crt);
                    doUpdate();
                    return;
                }
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to start conversation");
                game.errorLog(ex.ToString());
            }
        }
        #endregion

        

        #region Form Controls 
        public void refreshFormControls()
        {
            pc_button_0.Invalidate();
            pc_button_1.Invalidate();
            pc_button_2.Invalidate();
            pc_button_3.Invalidate();
            pc_button_4.Invalidate();
            pc_button_5.Invalidate();
        }        
        private void pc_button_0_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (game.playerList.PCList[0].Status != CharBase.charStatus.Dead)
                {
                    game.ChangePartySprite();
                    game.selectedPartyLeader = 0;
                    setPcButtonColors();
                    doScriptBasedOnFilename("dsChangedPartyLeader.cs", game.selectedPartyLeader.ToString(), "", "", "");
                }
            }
        }
        private void pc_button_1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (game.playerList.PCList[1].Status != CharBase.charStatus.Dead)
                {
                    game.ChangePartySprite();
                    game.selectedPartyLeader = 1;
                    setPcButtonColors();
                    doScriptBasedOnFilename("dsChangedPartyLeader.cs", game.selectedPartyLeader.ToString(), "", "", "");
                }
            }
        }
        private void pc_button_2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (game.playerList.PCList[2].Status != CharBase.charStatus.Dead)
                {
                    game.ChangePartySprite();
                    game.selectedPartyLeader = 2;
                    setPcButtonColors();
                    doScriptBasedOnFilename("dsChangedPartyLeader.cs", game.selectedPartyLeader.ToString(), "", "", "");
                }
            }
        }
        private void pc_button_3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (game.playerList.PCList[3].Status != CharBase.charStatus.Dead)
                {
                    game.ChangePartySprite();
                    game.selectedPartyLeader = 3;
                    setPcButtonColors();
                    doScriptBasedOnFilename("dsChangedPartyLeader.cs", game.selectedPartyLeader.ToString(), "", "", "");
                }
            }
        }
        private void pc_button_4_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (game.playerList.PCList[4].Status != CharBase.charStatus.Dead)
                {
                    game.ChangePartySprite();
                    game.selectedPartyLeader = 4;
                    setPcButtonColors();
                    doScriptBasedOnFilename("dsChangedPartyLeader.cs", game.selectedPartyLeader.ToString(), "", "", "");
                }
            }
        }
        private void pc_button_5_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (game.playerList.PCList[5].Status != CharBase.charStatus.Dead)
                {
                    game.ChangePartySprite();
                    game.selectedPartyLeader = 5;
                    setPcButtonColors();
                    doScriptBasedOnFilename("dsChangedPartyLeader.cs", game.selectedPartyLeader.ToString(), "", "", "");
                }
            }
        }
        private void pc_button_0_Click(object sender, EventArgs e)
        {
            pcSheet0.refreshSheet();
            pcSheet0.Show();
        }
        private void pc_button_1_Click(object sender, EventArgs e)
        {
            pcSheet1.refreshSheet();
            pcSheet1.Show();
        }
        private void pc_button_2_Click(object sender, EventArgs e)
        {
            pcSheet2.refreshSheet();
            pcSheet2.Show();
        }
        private void pc_button_3_Click(object sender, EventArgs e)
        {
            pcSheet3.refreshSheet();
            pcSheet3.Show();
        }
        private void pc_button_4_Click(object sender, EventArgs e)
        {
            pcSheet4.refreshSheet();
            pcSheet4.Show();
        }
        private void pc_button_5_Click(object sender, EventArgs e)
        {
            pcSheet5.refreshSheet();
            pcSheet5.Show();
        }
        private void btnSettings_Click(object sender, EventArgs e)
        {
            settings.setupLabels();
            settings.Show();
        }
        private void chkGrid_CheckedChanged(object sender, EventArgs e)
        {
            game.frm_showGrid = chkGrid.Checked;
            //game.areaUpdateAll();
            //game.spritePcDraw(game.playerPosition.X * squareSize, game.playerPosition.Y * squareSize, 0);
            //game.Update();
        }
        private void btnInventory_Click(object sender, EventArgs e)
        {
            openInventory();
        }
        private void openInventory()
        {
            pcInventory.rbtnPc0.Checked = true;
            pcInventory.refreshlbxItems();
            pcInventory.refreshFonts();
            pcInventory.refreshDescriptionBox();
            pcInventory.btnUseItem.Enabled = true;
            pcInventory.Show();
        }
        private void btnJournal_Click(object sender, EventArgs e)
        {
            openJournal();
        }
        private void openJournal()
        {
            pcJournalScreen.refreshAll();
            pcJournalScreen.Show();
        }
        private void btnRest_Click(object sender, EventArgs e)
        {
            doScriptBasedOnFilename("dsRestSystem.cs", "none", "none", "none", "none");
            doPortraitStats();
            //MessageBox.Show("clicked rest");
        }
        private void btnAdvLogFontIncrease_Click(object sender, EventArgs e)
        {
            AdvLogScale += 0.1f;
            rtxtLog.Font = ChangeFontSize(game.module.ModuleTheme.ModuleFont, AdvLogScale);
        }
        private void btnAdvLogFontDecrease_Click(object sender, EventArgs e)
        {
            AdvLogScale -= 0.1f;
            rtxtLog.Font = ChangeFontSize(game.module.ModuleTheme.ModuleFont, AdvLogScale);
        }
        private void btnQuickSave_Click(object sender, EventArgs e)
        {
            quickSave();
            game.DrawFloatyText("QUICKSAVED", game.playerPosition.X * game._squareSize - 150 + 32, game.playerPosition.Y * game._squareSize, 50, Color.Green, Color.Black);
            logText("QUICKSAVED", Color.Lime);
            logText(Environment.NewLine, Color.Black);
        }
        #endregion

        public void autoSave()
        {
            //SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            string filename = game.mainDirectory + "\\saves\\autosave.ofs";
            if (autosave)
            {
                game.saveGameFile(filename);
            }
        }
        public void quickSave()
        {
            string filename = game.mainDirectory + "\\saves\\quicksave.ofs";
            game.saveGameFile(filename);
        }
        public void SwitchToNextAvailablePartyLeader()
        {
            int idx = 0;
            foreach (PC pc in game.playerList.PCList)
            {
                if (pc.Status != CharBase.charStatus.Dead)
                {
                    game.selectedPartyLeader = idx;
                    setPcButtonColors();
                    return;
                }
                idx++;
            }
        }
        public void doUpdate()
        {
            //switch selectedPartyLeader if current one is DEAD
            if (game.playerList.PCList[game.selectedPartyLeader].Status == CharBase.charStatus.Dead)
            {
                SwitchToNextAvailablePartyLeader();
            }
            //logMainText("LocX = " + game.playerPosition.X.ToString() + "  LocY = " + game.playerPosition.Y.ToString(), Color.Black);
            //logMainText(Environment.NewLine, Color.Black);
            doPortraitStats();
            game.areaUpdate();
            //game.areaRenderAll();            
            //game.spritePcDraw(game.playerPosition.X * squareSize, game.playerPosition.Y * squareSize, 0);
            //game.Update();
            doScriptBasedOnFilename("dsWorldTime.cs", "none", "none", "none", "none");
            doHeartBeats();
            doApplyEffects();
            doPropOnEnter();
            doTrap();
            checkForContainer();
            doTrigger();
            finishedMove = true;            
        }
        public void doApplyEffects()
        {
            try
            {
                sf.MainMapScriptCall = true; //can be used as a flag in your scripts that the call is coming from the main map
                //maybe reorder all based on their order property            
                foreach (PC pc in game.playerList.PCList)
                {
                    foreach (IceBlinkCore.Effect ef in pc.EffectsList.effectsList)
                    {
                        //increment duration of all
                        ef.CurrentDurationInUnits = game.module.WorldTime - ef.StartingTimeInUnits;
                        if (!ef.UsedForUpdateStats) //not used for stat updates
                        {                            
                            //do script for each effect
                            sf.MainMapSource = pc;
                            var scriptCrt = ef.EffectScript;
                            scriptCrt.Parm1 = ef.CurrentDurationInUnits.ToString();
                            scriptCrt.Parm2 = ef.DurationInUnits.ToString();
                            doScriptBasedOnFilename(scriptCrt.FilenameOrTag, scriptCrt.Parm1, scriptCrt.Parm2, scriptCrt.Parm3, scriptCrt.Parm4);
                        }
                    }
                }
                //if duration equals ending or greater, remove from list
                foreach (PC pc in game.playerList.PCList)
                {
                    for (int i = pc.EffectsList.effectsList.Count; i > 0; i--)
                    {
                        if (pc.EffectsList.effectsList[i - 1].CurrentDurationInUnits >= pc.EffectsList.effectsList[i - 1].DurationInUnits)
                        {
                            if (debugMode)
                            {
                                logText("Removed " + pc.EffectsList.effectsList[i - 1].EffectName + " from " + pc.Name, Color.Lime);
                                logText(Environment.NewLine, Color.Lime);
                            }
                            pc.EffectsList.effectsList.RemoveAt(i - 1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, ex.ToString());
            }
            finally
            {
                sf.MainMapScriptCall = false; //set back to false after use
            }
        }
        public void doHeartBeats()
        {
            //Module HeartBeat
            var scriptMod = game.module.OnHeartBeat;
            doScriptBasedOnFilename(scriptMod.FilenameOrTag, scriptMod.Parm1, scriptMod.Parm2, scriptMod.Parm3, scriptMod.Parm4);
            //Area HeartBeat (current area only)
            var scriptArea = game.currentArea.OnHeartBeat;
            doScriptBasedOnFilename(scriptArea.FilenameOrTag, scriptArea.Parm1, scriptArea.Parm2, scriptArea.Parm3, scriptArea.Parm4);
            //Creatures in current area HeartBeat
            foreach (Creature crt in game.currentArea.AreaCreatureList.creatures)
            {
                game.scriptOwnerCreature = crt;
                if (crt.OnHeartBeat.FilenameOrTag != "none")
                {
                    var scriptCrt = crt.OnHeartBeat;
                    doScriptBasedOnFilename(scriptCrt.FilenameOrTag, scriptCrt.Parm1, scriptCrt.Parm2, scriptCrt.Parm3, scriptCrt.Parm4);
                }
            }
        }
        public void doPropOnEnter()
        {
            try
            {
                foreach (Prop prp in game.currentArea.AreaPropList.propsList)
                {
                    if (game.playerPosition == prp.Location)
                    {
                        sf.passParameterScriptObject = prp.PropTag;
                        var scriptPropOnEnter = prp.OnEnter;
                        doScriptBasedOnFilename(scriptPropOnEnter.FilenameOrTag, scriptPropOnEnter.Parm1, scriptPropOnEnter.Parm2, scriptPropOnEnter.Parm3, scriptPropOnEnter.Parm4);
                        sf.passParameterScriptObject = null;
                    }
                }
            }
            catch (Exception ex)
            {
                game.errorLog("failed on Prop OnEnter: " + ex.ToString());
            }
        }
        public void doTrigger()
        {
            try
            {
                //[TODO]need to cycle through Z order some how
                Trigger trig = game.currentArea.getTriggerByLocation(game.playerPosition.X, game.playerPosition.Y);
                if ((trig != null) && (trig.Enabled))
                {
                    //iterate through each event                  
                    #region Event1 stuff
                    //check to see if enabled and parm not "none"
                    if ((trig.EnabledEvent1) && (trig.Parameters1.FilenameOrTag != "none"))
                    {
                        //check to see what type of event
                        switch (trig.EventType1)
                        {
                            case Trigger.TriggerType.Container:
                                //do stuff
                                doContainerBasedOnTag(trig.Parameters1.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Shop:
                                //do stuff
                                doShopBasedOnTag(trig.Parameters1.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Conversation:
                                //do stuff
                                doConversationBasedOnObject(trig.Parameters1.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Narration:
                                //do stuff
                                doNarrationBasedOnTag(trig.Parameters1.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Encounter:
                                //do stuff
                                doEncounterBasedOnTag(trig.Parameters1.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Script:
                                doScriptBasedOnFilename(trig.Parameters1.FilenameOrTag, trig.Parameters1.Parm1, trig.Parameters1.Parm2, trig.Parameters1.Parm3, trig.Parameters1.Parm4);
                                break;
                            case Trigger.TriggerType.Transition:
                                //do stuff
                                doTransitionBasedOnAreaLocation(trig.Parameters1.FilenameOrTag, trig.Parameters1.TransPoint.X, trig.Parameters1.TransPoint.Y);
                                break;
                            default:
                                //must have been "None" so do nothing
                                break;
                        }
                        //do that event
                        if (trig.DoOnceOnlyEvent1)
                        {
                            trig.EnabledEvent1 = false;
                        }
                    }
                    #endregion
                    #region Event2 stuff
                    //check to see if enabled and parm not "none"
                    if ((trig.EnabledEvent2) && (trig.Parameters2.FilenameOrTag != "none"))
                    {
                        //check to see what type of event
                        switch (trig.EventType2)
                        {
                            case Trigger.TriggerType.Container:
                                //do stuff
                                doContainerBasedOnTag(trig.Parameters2.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Shop:
                                //do stuff
                                doShopBasedOnTag(trig.Parameters2.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Conversation:
                                //do stuff
                                doConversationBasedOnObject(trig.Parameters2.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Narration:
                                //do stuff
                                doNarrationBasedOnTag(trig.Parameters2.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Encounter:
                                //do stuff
                                doEncounterBasedOnTag(trig.Parameters2.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Script:
                                doScriptBasedOnFilename(trig.Parameters2.FilenameOrTag, trig.Parameters2.Parm1, trig.Parameters2.Parm2, trig.Parameters2.Parm3, trig.Parameters2.Parm4);
                                break;
                            case Trigger.TriggerType.Transition:
                                //do stuff
                                doTransitionBasedOnAreaLocation(trig.Parameters2.FilenameOrTag, trig.Parameters2.TransPoint.X, trig.Parameters2.TransPoint.Y);
                                break;
                            default:
                                //must have been "None" so do nothing
                                break;
                        }
                        //do that event
                        if (trig.DoOnceOnlyEvent2)
                        {
                            trig.EnabledEvent2 = false;
                        }
                    }
                    #endregion
                    #region Event3 stuff
                    //check to see if enabled and parm not "none"
                    if ((trig.EnabledEvent3) && (trig.Parameters3.FilenameOrTag != "none"))
                    {
                        //check to see what type of event
                        switch (trig.EventType3)
                        {
                            case Trigger.TriggerType.Container:
                                //do stuff
                                doContainerBasedOnTag(trig.Parameters3.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Shop:
                                //do stuff
                                doShopBasedOnTag(trig.Parameters3.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Conversation:
                                //do stuff
                                doConversationBasedOnObject(trig.Parameters3.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Narration:
                                //do stuff
                                doNarrationBasedOnTag(trig.Parameters3.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Encounter:
                                //do stuff
                                doEncounterBasedOnTag(trig.Parameters3.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Script:
                                doScriptBasedOnFilename(trig.Parameters3.FilenameOrTag, trig.Parameters3.Parm1, trig.Parameters3.Parm2, trig.Parameters3.Parm3, trig.Parameters3.Parm4);
                                break;
                            case Trigger.TriggerType.Transition:
                                //do stuff
                                doTransitionBasedOnAreaLocation(trig.Parameters3.FilenameOrTag, trig.Parameters3.TransPoint.X, trig.Parameters3.TransPoint.Y);
                                break;
                            default:
                                //must have been "None" so do nothing
                                break;
                        }
                        //do that event
                        if (trig.DoOnceOnlyEvent2)
                        {
                            trig.EnabledEvent2 = false;
                        }
                    }
                    #endregion
                    #region Event4 stuff
                    //check to see if enabled and parm not "none"
                    if ((trig.EnabledEvent4) && (trig.Parameters4.FilenameOrTag != "none"))
                    {
                        //check to see what type of event
                        switch (trig.EventType4)
                        {
                            case Trigger.TriggerType.Container:
                                //do stuff
                                doContainerBasedOnTag(trig.Parameters4.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Shop:
                                //do stuff
                                doShopBasedOnTag(trig.Parameters4.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Conversation:
                                //do stuff
                                doConversationBasedOnObject(trig.Parameters4.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Narration:
                                //do stuff
                                doNarrationBasedOnTag(trig.Parameters4.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Encounter:
                                //do stuff
                                doEncounterBasedOnTag(trig.Parameters4.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Script:
                                doScriptBasedOnFilename(trig.Parameters4.FilenameOrTag, trig.Parameters4.Parm1, trig.Parameters4.Parm2, trig.Parameters4.Parm3, trig.Parameters4.Parm4);
                                break;
                            case Trigger.TriggerType.Transition:
                                //do stuff
                                doTransitionBasedOnAreaLocation(trig.Parameters4.FilenameOrTag, trig.Parameters4.TransPoint.X, trig.Parameters4.TransPoint.Y);
                                break;
                            default:
                                //must have been "None" so do nothing
                                break;
                        }
                        //do that event
                        if (trig.DoOnceOnlyEvent4)
                        {
                            trig.EnabledEvent4 = false;
                        }
                    }
                    #endregion
                    #region Event5 stuff
                    //check to see if enabled and parm not "none"
                    if ((trig.EnabledEvent5) && (trig.Parameters5.FilenameOrTag != "none"))
                    {
                        //check to see what type of event
                        switch (trig.EventType5)
                        {
                            case Trigger.TriggerType.Container:
                                //do stuff
                                doContainerBasedOnTag(trig.Parameters5.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Shop:
                                //do stuff
                                doShopBasedOnTag(trig.Parameters5.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Conversation:
                                //do stuff
                                doConversationBasedOnObject(trig.Parameters5.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Narration:
                                //do stuff
                                doNarrationBasedOnTag(trig.Parameters5.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Encounter:
                                //do stuff
                                doEncounterBasedOnTag(trig.Parameters5.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Script:
                                doScriptBasedOnFilename(trig.Parameters5.FilenameOrTag, trig.Parameters5.Parm1, trig.Parameters5.Parm2, trig.Parameters5.Parm3, trig.Parameters5.Parm4);
                                break;
                            case Trigger.TriggerType.Transition:
                                //do stuff
                                doTransitionBasedOnAreaLocation(trig.Parameters5.FilenameOrTag, trig.Parameters5.TransPoint.X, trig.Parameters5.TransPoint.Y);
                                break;
                            default:
                                //must have been "None" so do nothing
                                break;
                        }
                        //do that event
                        if (trig.DoOnceOnlyEvent5)
                        {
                            trig.EnabledEvent5 = false;
                        }
                    }
                    #endregion
                    #region Event6 stuff
                    //check to see if enabled and parm not "none"
                    if ((trig.EnabledEvent6) && (trig.Parameters6.FilenameOrTag != "none"))
                    {
                        //check to see what type of event
                        switch (trig.EventType6)
                        {
                            case Trigger.TriggerType.Container:
                                //do stuff
                                doContainerBasedOnTag(trig.Parameters6.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Shop:
                                //do stuff
                                doShopBasedOnTag(trig.Parameters6.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Conversation:
                                //do stuff
                                doConversationBasedOnObject(trig.Parameters6.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Narration:
                                //do stuff
                                doNarrationBasedOnTag(trig.Parameters6.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Encounter:
                                //do stuff
                                doEncounterBasedOnTag(trig.Parameters6.FilenameOrTag);
                                break;
                            case Trigger.TriggerType.Script:
                                doScriptBasedOnFilename(trig.Parameters6.FilenameOrTag, trig.Parameters6.Parm1, trig.Parameters6.Parm2, trig.Parameters6.Parm3, trig.Parameters6.Parm4);
                                break;
                            case Trigger.TriggerType.Transition:
                                //do stuff
                                doTransitionBasedOnAreaLocation(trig.Parameters6.FilenameOrTag, trig.Parameters6.TransPoint.X, trig.Parameters6.TransPoint.Y);
                                break;
                            default:
                                //must have been "None" so do nothing
                                break;
                        }
                        //do that event
                        if (trig.DoOnceOnlyEvent6)
                        {
                            trig.EnabledEvent6 = false;
                        }
                    }
                    #endregion

                    if (trig.DoOnceOnly)
                    {
                        trig.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to do trigger: " + ex.ToString());
                game.errorLog(ex.ToString());
            }
        }
        public void checkForContainer()
        {
            //find the Prop
            try
            {
                Prop containerProp = game.currentArea.getPropByLocation(game.playerPosition.X, game.playerPosition.Y);
                if (containerProp != null)
                {
                    if (containerProp.PropContainerChk)
                    {
                        currentContainer = game.module.ModuleContainersList.getContainer(containerProp.PropContainerTag);
                        if (currentContainer.containerInventoryList.Count > 0)
                        {
                            ContainerDialogBox contnr = new ContainerDialogBox(game, currentContainer);
                            contnr.BackgroundImage = currentTheme.StandardThemeBitmap;
                            contnr.ShowDialog();
                        }
                        else
                        {
                            IBMessageBox.Show(game, "no items left here");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open container: " + ex.ToString());
                game.errorLog(ex.ToString());
            }
        }
        public void doContainerBasedOnTag(string tag)
        {
            currentContainer = game.module.ModuleContainersList.getContainer(tag);
            if (currentContainer != null)
            {
                if (currentContainer.containerInventoryList.Count > 0)
                {
                    ContainerDialogBox contnr = new ContainerDialogBox(game, currentContainer);
                    contnr.BackgroundImage = currentTheme.StandardThemeBitmap;
                    contnr.ShowDialog();
                }
                else
                {
                    IBMessageBox.Show(game, "no items left here");
                }
            }
        }
        public void doShopBasedOnTag(string tag)
        {            
            currentShop = game.module.ModuleShopsList.getShopByTag(tag);
            if (currentShop != null)
            {                
                Store str = new Store();
                str.passRefs(game, this, currentShop);
                str.BackgroundImage = currentTheme.StandardThemeBitmap;
                str.ShowDialog();
            }
        }
        public void doNarrationBasedOnTag(string tag)
        {
            try
            {
                //AnimationTimer.Stop();
                ConversationDialogBox t = new ConversationDialogBox(game, this, tag);
                t.BackgroundImage = currentTheme.StandardThemeBitmap;
                t.ShowDialog();
                //AnimationTimer.Start();
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open conversation with tag: " + tag);
                game.errorLog(ex.ToString());
            }
        }
        public void doConversationBasedOnObject(string tag)
        {
            //find the object that has the tag and then get the object's conversation tag
            foreach (Creature crt in game.currentArea.AreaCreatureList.creatures)
            {
                if (crt.Tag == tag)
                {
                    try
                    {
                        //AnimationTimer.Stop();
                        sf.ConvoOwnerTag = crt.Tag; //set ConvoOwnerTag to this creature (for use in CheckLocal and SetLocal convo scripts)
                        ConversationDialogBox t = new ConversationDialogBox(game, this, crt.ConversationTag);
                        t.BackgroundImage = currentTheme.StandardThemeBitmap;
                        t.ShowDialog();
                        sf.ConvoOwnerTag = ""; //set ConvoOwnerTag back to empty after use (for use in CheckLocal and SetLocal convo scripts)
                        if (game.uncheckConvo)
                        {
                            crt.ConversationChk = false;
                            game.uncheckConvo = false;
                            //doUpdate();
                        }
                        if (game.addPCScriptFired)
                        {
                            //addPCtoParty();
                            game.addPCScriptFired = false;
                            game.currentArea.AreaCreatureList.creatures.Remove(crt);                           
                            doUpdate();
                            return;
                        }
                        if (game.removeCreature)
                        {
                            game.removeCreature = false;
                            game.currentArea.AreaCreatureList.creatures.Remove(crt);
                            doUpdate();
                            return;
                        }
                        //AnimationTimer.Start();
                    }
                    catch (Exception ex)
                    {
                        IBMessageBox.Show(game, "failed to open conversation");
                        game.errorLog(ex.ToString());
                    }
                }
            }
            foreach (Prop prp in game.currentArea.AreaPropList.propsList)
            {
                if (prp.PropTag == tag)
                {
                    try
                    {
                        sf.ConvoOwnerTag = prp.PropTag; //set ConvoOwnerTag to this Prop (for use in CheckLocal and SetLocal convo scripts)
                        ConversationDialogBox t = new ConversationDialogBox(game, this, prp.PropConversationTag);
                        t.BackgroundImage = currentTheme.StandardThemeBitmap;
                        t.ShowDialog();
                        sf.ConvoOwnerTag = ""; //set ConvoOwnerTag back to empty after use (for use in CheckLocal and SetLocal convo scripts)
                    }
                    catch (Exception ex)
                    {
                        IBMessageBox.Show(game, "failed to open conversation");
                        game.errorLog(ex.ToString());
                    }
                }
            }
        }
        public void doEncounterBasedOnTag(string tag)
        {
            try
            {
                currentEncounter = game.module.ModuleEncountersList.getEncounter(tag);
                if (currentEncounter.EncounterCreatureList.creatures.Count > 0)
                {
                    AnimationTimer.Stop();
                    realTimer.Stop();
                    //game.disposeSpritesTextures();
                    currentCombat = new Combat(game, this, currentEncounter);
                    currentCombat.BackgroundImage = currentTheme.StandardThemeBitmap;
                    currentCombat.ShowDialog();
                    //game.newAreaInitializeGraphics();
                    AnimationTimer.Start();
                    realTimer.Start();
                    int foundOnePc = 0;
                    foreach (PC chr in game.playerList.PCList)
                    {
                        if (chr.HP > 0)
                        {
                            foundOnePc = 1;
                        }
                    }
                    if (foundOnePc == 0)
                    {
                        IBMessageBox.Show(game, "Party is wiped out...game over");
                        this.Close();
                    }
                    playAreaMusicSounds();
                    //game.areaUpdateAll();
                    //game.spritePcDraw(game.playerPosition.X * squareSize, game.playerPosition.Y * squareSize, 0);
                    //game.Update();
                    doPortraitStats();
                }
                else
                {
                    //IBMessageBox.Show(game, "no creatures left here"); 
                }
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to open encounter");
                game.errorLog(ex.ToString());
                //game.disposeSpritesTextures();
                //game.newAreaInitializeGraphics();
                AnimationTimer.Start();
                realTimer.Start();
            }
        }
        public void doScriptBasedOnFilename(string filename, string p1, string p2, string p3, string p4)
        {
            if (filename != "none")
            {
                try
                {
                    game.parm1 = p1;
                    game.parm2 = p2;
                    game.parm3 = p3;
                    game.parm4 = p4;
                    if (File.Exists(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\scripts\\" + filename))
                    {
                        game.executeScript(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\scripts\\" + filename);
                    }
                    else if (File.Exists(game.mainDirectory + "\\data\\scripts\\" + filename))
                    {
                        game.executeScript(game.mainDirectory + "\\data\\scripts\\" + filename);
                    }
                    else
                    {
                        game.errorLog("failed doScriptBasedOnFilename: couldn't find the script file: " + filename);
                        //IBMessageBox.Show(game, "couldn't find the script file: " + filename);
                    }
                    game.parm1 = "";
                    game.parm2 = "";
                    game.parm3 = "";
                    game.parm4 = "";
                }
                catch (Exception ex)
                {
                    IBMessageBox.Show(game, "failed to run script");
                    game.errorLog(ex.ToString());
                }
            }
        }
        public void doTransitionBasedOnAreaLocation(string areaFilename, int x, int y)
        {
            try
            {
                //change currentArea to new area
                foreach (Area area in game.module.ModuleAreasObjects)
                {
                    if (areaFilename == area.AreaFileName)
                    {
                        AnimationTimer.Stop();
                        //change playerPosition.X and Y to transition points
                        game.playerPosition.X = x;
                        game.playerPosition.Y = y;
                        game.shadowTextPool.Clear();

                        game.disposeSpritesTextures();
                        //IBMessageBox.Show(game, "game.playerPosition.X = " + game.playerPosition.X.ToString());
                        //IBMessageBox.Show(game, "game.playerPosition.Y = " + game.playerPosition.Y.ToString());
                        game.currentArea = area;
                        //game.currentMapBitmap = new Bitmap(game.mainDirectory + "\\modules\\" + game.module.ModuleFolderName + "\\" + game.currentArea.MapFileName);
                        //loadCurrentAreaBitmap();
                        //loadCurrentAreaTexture();
                        //game.newAreaInitGraphics(this.pictureBox1);  
                        game.resetDevice();
                        game.newAreaInitializeGraphics();
                        playAreaMusicSounds();
                        //game.areaUpdateAll();
                        btnRest.Enabled = game.currentArea.RestingAllowed;
                        sf.pathfinderMainArea = new PathfinderMainArea(game);
                        AnimationTimer.Start();
                        //IBMessageBox.Show(game, "game.currentArea = " + game.currentArea.a_fileName);
                    }
                }
                doUpdate();
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, "failed to transition to area");
                game.errorLog(ex.ToString());
            }
        }
        public void doPortraitStats()
        {
            refreshFormControls();
        }
        public void doTrap()
        {
        }
        public bool doStartScreen()
        {
            StartScreen startScreen = new StartScreen(game, this);
            startScreen.BackgroundImage = currentTheme.StandardThemeBitmap;
            DialogResult result = startScreen.ShowDialog();
            if (result == DialogResult.Yes)
            {
                //IBMessageBox.Show(game, "You chose to load new game");
                return true;
            }
            else if (result == DialogResult.No)
            {
                string currentDir = Directory.GetCurrentDirectory();
                //IBMessageBox.Show(game, "You chose to load a saved game");
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = game.mainDirectory + "\\saves";
                //Empty the FileName text box of the dialog
                openFileDialog1.FileName = String.Empty;
                openFileDialog1.Filter = "Saved Game (*.ofs)|*.ofs|All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 1;
                openFileDialog1.RestoreDirectory = true;
                DialogResult openresult = openFileDialog1.ShowDialog(); // Show the dialog.
                if (openresult == DialogResult.OK) // Test result.
                {
                    string openfilename = Path.GetFullPath(openFileDialog1.FileName);
                    game = game.loadGameFile(openfilename);
                    if (game == null)
                    {
                        IBMessageBox.Show(game, "returned a null saved game file");
                    }
                    sf = new ScriptFunctions();
                    sf.passRefs(this, game);
                    game.passRefs(this);
                }
                Directory.SetCurrentDirectory(currentDir);
                return false;
            }
            else
                IBMessageBox.Show(game, "Error in splash screen result");
            return false;
        }
        public bool doStartScreen2()
        {
            StartScreen startScreen = new StartScreen(game, this);
            startScreen.BackgroundImage = currentTheme.StandardThemeBitmap;
            DialogResult result = startScreen.ShowDialog();
            if (result == DialogResult.Yes)
            {
                //IBMessageBox.Show(game, "You chose to load new game");
                startScreenCompleted = true;
                return true;
            }
            else if (result == DialogResult.No)
            {
                string currentDir = Directory.GetCurrentDirectory();
                //IBMessageBox.Show(game, "You chose to load a saved game");
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = game.mainDirectory + "\\saves";
                //Empty the FileName text box of the dialog
                openFileDialog1.FileName = String.Empty;
                openFileDialog1.Filter = "Saved Game (*.ofs)|*.ofs|All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 1;
                openFileDialog1.RestoreDirectory = true;
                DialogResult openresult = openFileDialog1.ShowDialog(); // Show the dialog.
                if (openresult == DialogResult.OK) // Test result.
                {
                    //create game object
                    savedGame = new Game();
                    savedGame.passRefs(this);
                    string openfilename = Path.GetFullPath(openFileDialog1.FileName);
                    bool isGzip = FileChecker.CheckSignature(openfilename, 3, "1F-8B-08");
                    if (isGzip)
                    {
                        //IBMessageBox.Show(game, "uncompressing saved game");
                        savedGame = savedGame.loadGameFile(openfilename);
                        if (savedGame == null)
                        {
                            IBMessageBox.Show(game, "returned a null saved game file");
                        }
                    }
                    else //old style saved game, use old load method
                    {
                        IBMessageBox.Show(game, "old type of saved game...opening with old method");
                        savedGame = savedGame.loadGameFileOld(openfilename);
                        if (savedGame == null)
                        {
                            IBMessageBox.Show(game, "returned a null saved game file");
                        }
                    }
                    
                    sf = new ScriptFunctions();
                    sf.passRefs(this, game);
                    game.passRefs(this);
                    startScreenCompleted = true;
                }
                Directory.SetCurrentDirectory(currentDir);
                return false;
            }
            else if (result == DialogResult.Cancel)
            {
                startScreenCompleted = true;
            }
            else
                IBMessageBox.Show(game, "Error in splash screen result");
            return false;
        }
        public void doOpenModule()
        {
            try
            {
                OpenModule openMod = new OpenModule(game, this);
                openMod.BackgroundImage = currentTheme.StandardThemeBitmap;
                openMod.ShowDialog();
                game.module.passRefs(game, null);
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(game, ex.ToString());
            }
        }
        public void launchCharacterPartySelection()
        {
            CharacterPartySelection charPartySelectionScreen = new CharacterPartySelection(game, this);
            charPartySelectionScreen.BackgroundImage = currentTheme.StandardThemeBitmap;
            charPartySelectionScreen.ShowDialog();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //DialogResult dlg = MessageBox.Show(this, "Are you sure you wish to exit?","Closing", MessageBoxButtons.YesNo);
            DialogResult dlg = IBMessageBox.Show(game, "Are you sure you wish to exit?", enumMessageButton.YesNo);
            if (dlg == DialogResult.Yes)
            {
                e.Cancel = false;
            }
            if (dlg == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            game.disposeSpritesTextures();
            Application.Exit();
        }

        private void floatyTextPanel_Paint(object sender, PaintEventArgs e)
        {
            //foreach (ShadowTextObject to in game.shadowTextPool)
            //{
            //    game.DrawTextShadowOutline(e, to.X, to.Y, to.Z, to.Text, to.AlphaShadow, to.AlphaText, to.Font, to.FontPointSize, to.TextColor, to.ShadowColor);
            //}
        } 
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //foreach (ShadowTextObject to in game.shadowTextPool)
            //{
            //    game.DrawTextShadowOutline(e, to.X, to.Y, to.Z, to.Text, to.AlphaShadow, to.AlphaText, to.Font, to.FontPointSize, to.TextColor, to.ShadowColor);
            //}
        }

        private void PcPortraitStats(PaintEventArgs e, int index)
        {
            Theme theme = game.module.ModuleTheme;
            int cnt = 0;
            foreach (IceBlinkCore.Effect ef in game.playerList.PCList[index].EffectsList.effectsList)
            {
                string letter = ef.EffectLetter;
                game.DrawTextShadowOutline(e, 15 * cnt + 3, 20, 0, letter, 100, 255,
                                               theme.ModuleFont.FontFamily,
                                               theme.ModuleFont.SizeInPoints * theme.ModuleFontScale,
                                               ef.EffectLetterColor, Color.Black);
                cnt++;
            }
            if (game.playerList.PCList[index].IsReadyToAdvanceLevel())
            {
                string text = "LEVEL UP" + Environment.NewLine
                               + game.playerList.PCList[index].HP + " / "
                               + game.playerList.PCList[index].HPMax + Environment.NewLine
                               + game.playerList.PCList[index].SP + " / "
                               + game.playerList.PCList[index].SPMax;
                game.DrawTextShadowOutline(e, 5, 100, 0, text, 100, 255, game.module.ModuleTheme.ModuleFont.FontFamily, game.module.ModuleTheme.ModuleFont.SizeInPoints * game.module.ModuleTheme.ModuleFontScale, Color.White, Color.Black);
            }
            else
            {
                string text = game.playerList.PCList[index].HP + " / "
                                   + game.playerList.PCList[index].HPMax + Environment.NewLine
                                   + game.playerList.PCList[index].SP + " / "
                                   + game.playerList.PCList[index].SPMax;
                game.DrawTextShadowOutline(e, 5, 100, 0, text, 100, 255, game.module.ModuleTheme.ModuleFont.FontFamily, game.module.ModuleTheme.ModuleFont.SizeInPoints * game.module.ModuleTheme.ModuleFontScale, Color.White, Color.Black);
            }
        }
        private void setPcButtonColors()
        {
            if (pc_button_0.Enabled)
            {
                if (game.selectedPartyLeader == 0)
                {
                    pc_button_0.BackColor = Color.Lime;
                }
                else
                {
                    pc_button_0.BackColor = Color.Gray;
                }
            }
            if (pc_button_1.Enabled)
            {
                if (game.selectedPartyLeader == 1)
                {
                    pc_button_1.BackColor = Color.Lime;
                }
                else
                {
                    pc_button_1.BackColor = Color.Gray;
                }
            }
            if (pc_button_2.Enabled)
            {
                if (game.selectedPartyLeader == 2)
                {
                    pc_button_2.BackColor = Color.Lime;
                }
                else
                {
                    pc_button_2.BackColor = Color.Gray;
                }
            }
            if (pc_button_3.Enabled)
            {
                if (game.selectedPartyLeader == 3)
                {
                    pc_button_3.BackColor = Color.Lime;
                }
                else
                {
                    pc_button_3.BackColor = Color.Gray;
                }
            }
            if (pc_button_4.Enabled)
            {
                if (game.selectedPartyLeader == 4)
                {
                    pc_button_4.BackColor = Color.Lime;
                }
                else
                {
                    pc_button_4.BackColor = Color.Gray;
                }
            }
            if (pc_button_5.Enabled)
            {
                if (game.selectedPartyLeader == 5)
                {
                    pc_button_5.BackColor = Color.Lime;
                }
                else
                {
                    pc_button_5.BackColor = Color.Gray;
                }
            }
        }
        private void pc_button_0_Paint(object sender, PaintEventArgs e)
        {
            if (pc_button_0.Enabled)
            {
                PcPortraitStats(e, 0);
            }
            //this.OnPaint(e);
        }        
        private void pc_button_1_Paint(object sender, PaintEventArgs e)
        {
            if (pc_button_1.Enabled)
            {
                PcPortraitStats(e, 1);
            }
            //this.OnPaint(e);
        }
        private void pc_button_2_Paint(object sender, PaintEventArgs e)
        {
            if (pc_button_2.Enabled)
            {
                PcPortraitStats(e, 2);
            }
            //this.OnPaint(e);
        }
        private void pc_button_3_Paint(object sender, PaintEventArgs e)
        {
            if (pc_button_3.Enabled)
            {
                PcPortraitStats(e, 3);
            }
            //this.OnPaint(e);
        }
        private void pc_button_4_Paint(object sender, PaintEventArgs e)
        {
            if (pc_button_4.Enabled)
            {
                PcPortraitStats(e, 4);
            }
            //this.OnPaint(e);
        }
        private void pc_button_5_Paint(object sender, PaintEventArgs e)
        {
            if (pc_button_5.Enabled)
            {
                PcPortraitStats(e, 5);
            }
            //this.OnPaint(e);
        }

        private void pnlWorldTime_Paint(object sender, PaintEventArgs e)
        {
            if (game.module.ModuleName != "")
            {
                string text = "";
                sf.returnScriptObject = null;
                doScriptBasedOnFilename("dsWorldTimeUIText.cs", "", "", "", "");
                if (sf.returnScriptObject != null)
                {
                    try
                    {
                        text = (string)sf.returnScriptObject;
                    }
                    catch { }
                }
                game.DrawTextShadowOutline(e, 2, 48, 0, text, 100, 255, game.module.ModuleTheme.ModuleFont.FontFamily, game.module.ModuleTheme.ModuleFont.SizeInPoints / 1.2f * game.module.ModuleTheme.ModuleFontScale, Color.White, Color.Black);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (game != null)
            {
                game.userResized = true;
            }
            /*
            try
            {
                if (game != null)
                {
                    game.areaUpdate();
                    game.spritePcDraw(game.playerPosition.X * squareSize, game.playerPosition.Y * squareSize, 0);
                    game.Update();
                }
            }
            catch (Exception ex)
            {
                //IBMessageBox.Show(game, "failed to resize: " + ex.ToString());
                game.errorLog(ex.ToString());
            }
            */
        }

        public void buttonBottomPanel_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            doScriptBasedOnFilename("dsBottomPanelButtons.cs", (string)clickedButton.Tag, "", "", "");
            doPortraitStats();
        }
        public void buttonLeftPanel_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            doScriptBasedOnFilename("dsLeftPanelButtons.cs", (string)clickedButton.Tag, "", "", "");
            doPortraitStats();
        }
       
        // * sinopip, 20.12.14
		// added a component timer (name is "scrollTimer") of 200ms tick        
        void ScrollTimerTick(object sender, EventArgs e)
        {
        	if (is_leftscrolling) 
        		renderPanel.AutoScrollPosition = new Point(
        			-renderPanel.AutoScrollPosition.X - 10,
        			-renderPanel.AutoScrollPosition.Y);
        	if (is_rightscrolling) 
        		renderPanel.AutoScrollPosition = new Point(
        			-renderPanel.AutoScrollPosition.X + 10,
        			-renderPanel.AutoScrollPosition.Y);
        	if (is_upscrolling) 
        		renderPanel.AutoScrollPosition = new Point(
        			-renderPanel.AutoScrollPosition.X,
        			-renderPanel.AutoScrollPosition.Y - 10);
        	if (is_downscrolling) 
        		renderPanel.AutoScrollPosition = new Point(
        			-renderPanel.AutoScrollPosition.X,
        			-renderPanel.AutoScrollPosition.Y + 10);   
        }
    }
}

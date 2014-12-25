using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using IceBlinkCore;
using WeifenLuo.WinFormsUI.Docking;

namespace IceBlinkToolset
{
    //public enum pass
    //{ passable, blocked, locked }
    /*public struct paletteTileStruct
    {
        public int tilenum;
        public bool collidable;
        public bool visible;
        public bool transition;
        public int transitionX;
        public int transitionY;
        public string transitionFile;
    }*/
    public struct selectionStruct
    {
        public int index;
        public int oldIndex;
        public int x, y;
    }

    public partial class LevelEditor : DockContent
    {
        public ParentForm prntForm;
        public const int tileSize = 64;
        public Module le_mod;
        public Game le_game;
        public Bitmap drawArea;
        public Bitmap gameMapBitmap;
        public Bitmap selectedBitmap;
        public int selectedBitmapSize = 1;
        public int mousex, mousey;
        public Graphics gfx;
        public int gridx, gridy;
        public Font fontArial;
        public string g_filename = "";
        public string g_directory = "";
        public Area area;
        public selectionStruct selectedTile;
        public List<string> spriteIconList = new List<string>();
        public List<string> scriptList = new List<string>();
        public bool showGrid = true;
        public Bitmap g_walkPass;
        public Bitmap g_walkBlock;
        public Bitmap g_LoSBlock;
        public Point lastSelectedCreaturePropIcon;
        public string lastSelectedObjectTag;
        public string lastSelectedObjectResRef;
        public Creature le_selectedCreature = new Creature();
        public Prop le_selectedProp = new Prop();
        public List<Bitmap> crtBitmapList = new List<Bitmap>(); //index will match AreaCreatureList index
        public List<Bitmap> propBitmapList = new List<Bitmap>(); //index will match AreaPropList index
        //public bool paintTriggerSelected = false;
        //public bool editTriggerSelected = false;
        //public bool toggleWalkable = false;
        // * sinopip, 22.12.14        
		public bool is_upscrolling = false;
		public bool is_downscrolling = false;
		public bool is_leftscrolling = false;
		public bool is_rightscrolling = false;
		//
		
        public LevelEditor(Module mod, Game g, ParentForm p)
        {
            InitializeComponent();
            le_mod = mod;
            le_game = g;
            prntForm = p;
        }

        #region Methods
        private void loadAreaTriggerPassRefs()
        {
            foreach (Trigger t in area.AreaTriggerList.triggersList)
            {
                t.passRefs(le_game, prntForm, area);
            }
        }
        private void convertObjectsToRefs()
        {
            //need to find resref of crt and get the tag
            foreach (Creature crt in area.AreaCreatureList.creatures)
            {
                Creature test = area.AreaCreatureList.getCreatureByResRef(crt.ResRef);
                if (test != null)
                {
                    CreatureRefs newCrtRef = new CreatureRefs();
                    newCrtRef.CreatureName = crt.Name;
                    newCrtRef.CreatureTag = crt.Tag;
                    newCrtRef.CreatureResRef = crt.ResRef;
                    newCrtRef.CreatureSize = crt.Size;
                    newCrtRef.MouseOverText = crt.MouseOverText;
                    newCrtRef.CreatureStartLocation = new Point(crt.MapLocation.X, crt.MapLocation.Y);
                    area.AreaCreatureRefsList.Add(newCrtRef);
                }
                else
                {
                    MessageBox.Show("Didn't find ResRef: " + crt.ResRef + " ...Failed to automatically convert object to a reference");
                }
            }
            area.AreaCreatureList.creatures.Clear();

            foreach (Prop prp in area.AreaPropList.propsList)
            {
                Prop test = area.AreaPropList.getPropByResRef(prp.PropResRef);
                if (test != null)
                {
                    PropRefs newPrpRef = new PropRefs();
                    newPrpRef.PropName = prp.PropName;
                    newPrpRef.PropTag = prp.PropTag;
                    newPrpRef.PropResRef = prp.PropResRef;
                    newPrpRef.PropContainerChk = prp.PropContainerChk;
                    newPrpRef.PropContainerTag = prp.PropContainerTag;
                    newPrpRef.PropTrappedChk = prp.PropTrapped;
                    newPrpRef.PropLockedChk = prp.PropLocked;
                    newPrpRef.PropKeyTag = prp.PropKeyTag;
                    newPrpRef.MouseOverText = prp.MouseOverText;
                    newPrpRef.PropStartLocation = new Point(prp.Location.X, prp.Location.Y);
                    area.AreaPropRefsList.Add(newPrpRef);
                }
                else
                {
                    MessageBox.Show("Didn't find ResRef: " + prp.PropResRef + " ...Failed to automatically convert object to a reference");
                }
            }
            area.AreaPropList.propsList.Clear();
        }
        private void loadAreaObjectBitmapLists()
        {
            convertObjectsToRefs();
            /*Bitmap newBitmap;
            foreach (Creature crt in area.AreaCreatureList.creatures)
            {
                newBitmap = new Bitmap(prntForm._mainDirectory + "\\modules\\" + le_mod.ModuleFolderName + "\\graphics\\sprites\\tokens\\" + crt.CharSprite.SpriteSheetFilename);
                crtBitmapList.Add(newBitmap);
            }*/
            foreach (CreatureRefs crtRef in area.AreaCreatureRefsList)
            {
                // get Creature by Tag and then get Icon filename, add Bitmap to list
                Creature newCrt = prntForm.creaturesList.getCreatureByResRef(crtRef.CreatureResRef);
                if (newCrt != null)
                {
                    if (File.Exists(prntForm._mainDirectory + "\\modules\\" + le_mod.ModuleFolderName + "\\graphics\\sprites\\tokens\\module\\" + newCrt.CharSprite.SpriteSheetFilename))
                    {
                        Bitmap newBitmap = new Bitmap(prntForm._mainDirectory + "\\modules\\" + le_mod.ModuleFolderName + "\\graphics\\sprites\\tokens\\module\\" + newCrt.CharSprite.SpriteSheetFilename);
                        crtBitmapList.Add(newBitmap);
                    }
                    else if (File.Exists(prntForm._mainDirectory + "\\data\\graphics\\sprites\\tokens\\" + newCrt.CharSprite.SpriteSheetFilename))
                    {
                        Bitmap newBitmap = new Bitmap(prntForm._mainDirectory + "\\data\\graphics\\sprites\\tokens\\" + newCrt.CharSprite.SpriteSheetFilename);
                        crtBitmapList.Add(newBitmap);
                    }
                    else
                    {
                        MessageBox.Show("Failed to find creature sprite (" + newCrt.CharSprite.SpriteSheetFilename + ") in data or module folders");
                        Bitmap newBitmap = new Bitmap(prntForm._mainDirectory + "\\data\\graphics\\blank.png");
                        crtBitmapList.Add(newBitmap);
                    }
                }
                else
                {
                    MessageBox.Show("Failed to find creature based on ResRef: " + crtRef.CreatureResRef);
                    Bitmap newBitmap = new Bitmap(prntForm._mainDirectory + "\\data\\graphics\\blank.png");
                    crtBitmapList.Add(newBitmap);
                }
                
            }
            /*foreach (Prop prp in area.AreaPropList.propsList)
            {
                newBitmap = new Bitmap(prntForm._mainDirectory + "\\modules\\" + le_mod.ModuleFolderName + "\\graphics\\sprites\\props\\" + prp.PropSprite.SpriteSheetFilename);
                propBitmapList.Add(newBitmap);
            }*/
            foreach (PropRefs prpRef in area.AreaPropRefsList)
            {
                // get Creature by Tag and then get Icon filename, add Bitmap to list
                Prop newPrp = prntForm.propsList.getPropByResRef(prpRef.PropResRef);
                if (File.Exists(prntForm._mainDirectory + "\\modules\\" + le_mod.ModuleFolderName + "\\graphics\\sprites\\props\\" + newPrp.PropSprite.SpriteSheetFilename))
                {
                    Bitmap newBitmap = new Bitmap(prntForm._mainDirectory + "\\modules\\" + le_mod.ModuleFolderName + "\\graphics\\sprites\\props\\" + newPrp.PropSprite.SpriteSheetFilename);
                    propBitmapList.Add(newBitmap);
                }
                else if (File.Exists(prntForm._mainDirectory + "\\data\\graphics\\sprites\\props\\" + newPrp.PropSprite.SpriteSheetFilename))
                {
                    Bitmap newBitmap = new Bitmap(prntForm._mainDirectory + "\\data\\graphics\\sprites\\props\\" + newPrp.PropSprite.SpriteSheetFilename);
                    propBitmapList.Add(newBitmap);
                }
                else
                {
                    MessageBox.Show("Failed to find prop sprite (" + newPrp.PropSprite.SpriteSheetFilename + ") in data or module folders");
                }
            }          
        }
        /*private void loadAreaObjectPassRefs()
        {
            foreach (Creature crt in area.AreaCreatureList.creatures)
            {
                crt.passRefs(le_game, prntForm);
            }
            foreach (Prop prp in area.AreaPropList.propsList)
            {
                prp.passRefs(le_game, prntForm);
            }
        }*/
        private void openLevel(string g_dir, string g_fil, string g_filNoEx)
        {
            this.Cursor = Cursors.WaitCursor;
            
            try
            {
                area = area.loadAreaFile(g_dir + "\\" + g_fil + ".level");
                if (area == null)
                {
                    MessageBox.Show("returned a null area");
                    le_game.errorLog("returned a null area");
                }
                //MessageBox.Show("open file success");
                loadAreaObjectBitmapLists();
                loadAreaTriggerPassRefs();
                area.passEventRefs(prntForm);
                //loadAreaObjectPassRefs();
                //loadAreaCreaturePropLists();                
                //redrawTilemap();
            }
            catch (Exception ex)
            {
                MessageBox.Show("failed to open file: " + ex.ToString());
                le_game.errorLog("failed to open file: " + ex.ToString());
            }

            // load JPG Map first
            try
            {
                gameMapBitmap = new Bitmap(g_dir + "\\" + area.MapFileName);
                drawArea = new Bitmap(g_dir + "\\" + area.MapFileName);
            }
            catch
            {
                gameMapBitmap = new Bitmap(area.MapSizeInPixels.Width, area.MapSizeInPixels.Height);
                drawArea = new Bitmap(area.MapSizeInPixels.Width, area.MapSizeInPixels.Height);
            }
            pictureBox1.Width = gameMapBitmap.Size.Width;
            pictureBox1.Height = gameMapBitmap.Size.Height;
            pictureBox1.Image = (Image)drawArea;
            gfx = Graphics.FromImage(drawArea);
            if (drawArea == null)
            {
                MessageBox.Show("returned a null Map bitmap");
                le_game.errorLog("returned a null Map bitmap");
                return;
            }
            refreshMap();
            this.Cursor = Cursors.Arrow;
        }
        /*private void loadTilemapFile()
        {
            //display the open file dialog
            openFileDialog1.DefaultExt = ".level";
            openFileDialog1.Filter = "Tilemap Files|*.level";
            openFileDialog1.Multiselect = false;
            openFileDialog1.Title = "Load Level File";
            openFileDialog1.InitialDirectory = prntForm._mainDirectory + "\\modules\\" + le_mod.ModuleFolderName;
            DialogResult result = openFileDialog1.ShowDialog();
            if (result != DialogResult.OK) return;
            //g_directory = Path.GetDirectoryName(openFileDialog1.FileName);
            g_directory = prntForm._mainDirectory + "\\modules\\" + le_mod.ModuleFolderName;
            g_filename = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
            string filenameNoExtension = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);

            openLevel(g_directory, g_filename, filenameNoExtension);
            
            this.Cursor = Cursors.WaitCursor;

            // load PNG Map first
            //ee_encounter.encounterPngMapName = filenameNoExtension + ".png";
            gameMapBitmap = new Bitmap(g_directory + "\\" + filenameNoExtension + ".png");
            drawArea = new Bitmap(g_directory + "\\" + filenameNoExtension + ".png");
            pictureBox1.Image = (Image)drawArea;
            gfx = Graphics.FromImage(drawArea);
            if (drawArea == null)
            {
                MessageBox.Show("returned a null Map bitmap");
            }                      

            try
            {
                area = area.loadAreaFile(g_directory + "\\" + g_filename);
                if (area == null)
                {
                    MessageBox.Show("returned a null area");
                }
                //MessageBox.Show("open file success");
                refreshMap();
                //redrawTilemap();
            }
            catch (Exception es)
            {
                MessageBox.Show(es.Message);
                MessageBox.Show("failed to open file");
            }

            this.Cursor = Cursors.Arrow;
            
        }*/
        private void saveTilemapFileAs()
        {
            //display the open file dialog
            saveFileDialog1.DefaultExt = ".level";
            saveFileDialog1.Filter = "Tilemap Files|*.level";
            saveFileDialog1.Title = "Save Level File";
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.OverwritePrompt = true;
            saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result != DialogResult.OK) return;
            if (saveFileDialog1.FileName.Length == 0) return;
            g_filename = Path.GetFileNameWithoutExtension(saveFileDialog1.FileName);
            g_directory = Path.GetDirectoryName(saveFileDialog1.FileName);
            area.AreaFileName = g_filename;
            area.MapFileName = Path.GetFileNameWithoutExtension(saveFileDialog1.FileName) + ".jpg";
            saveTilemapFile();
        }
        public void saveTilemapFile()
        {
            if (g_filename.Length == 0)
            {
                saveTilemapFileAs();
                return;
            }
            area.AreaFileName = g_filename;
            //area.MapFileName = Path.GetFileNameWithoutExtension(g_filename) + ".jpg";
            area.saveAreaFile(g_directory + "\\" + g_filename + ".level");
        }
        private void createNewArea(int width, int height)
        {
            //create tilemap
            area = null;
            area = new Area();
            area.MapSizeInSquares.Width = width;
            area.MapSizeInSquares.Height = height;
            area.MapSizeInPixels.Width = width * tileSize;
            area.MapSizeInPixels.Height = height * tileSize;
            for (int index = 0; index < (width * height); index++)
            {
                Tile newTile = new Tile();
                newTile.collidable = false;
                newTile.LoSBlocked = false;
                newTile.visible = false;
                area.TileMapList.Add(newTile);
            }
            try
            {
                gameMapBitmap = new Bitmap(area.MapSizeInPixels.Width, area.MapSizeInPixels.Height);
                drawArea = new Bitmap(area.MapSizeInPixels.Width, area.MapSizeInPixels.Height);
            }
            catch
            {
                gameMapBitmap = new Bitmap(800, 800);
                drawArea = new Bitmap(800, 800);
            }
            pictureBox1.Width = gameMapBitmap.Size.Width;
            pictureBox1.Height = gameMapBitmap.Size.Height;
            pictureBox1.Image = (Image)drawArea;
            gfx = Graphics.FromImage(drawArea);
            if (drawArea == null)
            {
                MessageBox.Show("returned a null Map bitmap");
                le_game.errorLog("returned a null Map bitmap");
                return;
            }
            area.passEventRefs(prntForm);
            refreshMap();
        }
        private void resetAreaTileValues(int width, int height)
        {
            //create tilemap
            //area = null;
            //area = new Area();
            area.MapSizeInSquares.Width = width;
            area.MapSizeInSquares.Height = height;
            area.MapSizeInPixels.Width = width * tileSize;
            area.MapSizeInPixels.Height = height * tileSize;
            for (int index = 0; index < (width * height); index++)
            {
                Tile newTile = new Tile();
                newTile.collidable = false;
                newTile.LoSBlocked = false;
                newTile.visible = false;
                area.TileMapList.Add(newTile);
            }
        }
        public void refreshMap()
        {
            try
            {
                if (gameMapBitmap != null)
                {
                    gfx.DrawImage((Image)gameMapBitmap, 0, 0);
                }
                int cnt = 0;
                foreach (CreatureRefs crtRef in area.AreaCreatureRefsList)
                {
                    int cspx = crtRef.CreatureStartLocation.X;
                    int cspy = crtRef.CreatureStartLocation.Y;
                    spriteCreatureDraw(cspx, cspy, cnt, crtRef.CreatureSize);
                    cnt++;
                }
                cnt = 0;
                foreach (PropRefs prpRef in area.AreaPropRefsList)
                {
                    int cspx = prpRef.PropStartLocation.X;
                    int cspy = prpRef.PropStartLocation.Y;
                    spritePropDraw(cspx, cspy, cnt);
                    cnt++;
                }

                /*cnt = 0;
                foreach (Creature crt in area.AreaCreatureList.creatures)
                {
                    int cspx = crt.MapLocation.X;
                    int cspy = crt.MapLocation.Y;
                    spriteCreatureDraw(cspx, cspy, cnt);
                    cnt++;
                }
                cnt = 0;
                foreach (Prop prp in area.AreaPropList.propsList)
                {
                    int cspx = prp.Location.X;
                    int cspy = prp.Location.Y;
                    spritePropDraw(cspx, cspy, cnt);
                    cnt++;
                }*/
                if (area != null)
                {
                    drawTileSettings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("failed on refresh map: " + ex.ToString());
                le_game.errorLog("failed on refresh map: " + ex.ToString());
            }
            drawTileSettings();
        }
        private void spriteCreatureDraw(int cspx, int cspy, int spriteListIndex, int size)
        {
            //source image
            Rectangle source = new Rectangle(0, 0, tileSize * size, tileSize * size);
            //target location
            Rectangle target = new Rectangle(cspx * tileSize, cspy * tileSize, tileSize * size, tileSize * size);
            //draw sprite
            gfx.DrawImage((Image)crtBitmapList[spriteListIndex], target, source, GraphicsUnit.Pixel);
        }
        private void spritePropDraw(int cspx, int cspy, int spriteListIndex)
        {
            //source image
            Rectangle source = new Rectangle(0, 0, le_selectedProp.PropSprite.SpriteSize.Width, le_selectedProp.PropSprite.SpriteSize.Height);
            //target location
            Rectangle target = new Rectangle(cspx * tileSize, cspy * tileSize, le_selectedProp.PropSprite.SpriteSize.Width, le_selectedProp.PropSprite.SpriteSize.Height);
            //draw sprite
            gfx.DrawImage((Image)propBitmapList[spriteListIndex], target, source, GraphicsUnit.Pixel);
        }
        private void drawTileSettings()
        {
            //for (int index = 0; index < area.MapSizeInSquares.Width * area.MapSizeInSquares.Height; index++)
            for (int x = 0; x < area.MapSizeInSquares.Width; x++)
            {
                for (int y = 0; y < area.MapSizeInSquares.Height; y++)
                {
                    if (showGrid) //if show grid is turned on, draw grid squares
                    {
                        if (area.TileMapList[y * this.area.MapSizeInSquares.Width + x].LoSBlocked)
                        {
                            Rectangle src = new Rectangle(0, 0, tileSize, tileSize);
                            int dx = x * tileSize;
                            int dy = y * tileSize;
                            Rectangle target = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
                            gfx.DrawImage(g_LoSBlock, target, src, GraphicsUnit.Pixel);
                        }
                        if (area.TileMapList[y * this.area.MapSizeInSquares.Width + x].collidable)
                        {
                            Rectangle src = new Rectangle(0, 0, tileSize, tileSize);
                            int dx = x * tileSize;
                            int dy = y * tileSize;
                            Rectangle target = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
                            gfx.DrawImage(g_walkBlock, target, src, GraphicsUnit.Pixel);
                        }
                        else
                        {
                            Rectangle src = new Rectangle(0, 0, tileSize, tileSize);
                            int dx = x * tileSize;
                            int dy = y * tileSize;
                            Rectangle target = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
                            gfx.DrawImage(g_walkPass, target, src, GraphicsUnit.Pixel);
                        }
                    }    
                }
            }
            foreach (Trigger t in area.AreaTriggerList.triggersList)
            {
                foreach (Point p in t.TriggerSquaresList)
                {
                    int dx = p.X * tileSize;
                    int dy = p.Y * tileSize;
                    Pen pen = new Pen(t.TriggerColor, 2);
                    Rectangle rect = new Rectangle(dx + 3, dy + 3, tileSize - 6, tileSize - 6);
                    gfx.DrawRectangle(pen, rect);
                }
            }
            pictureBox1.Image = drawArea;
        }
        public void drawSelectionBox(int gridx, int gridy)
        {
            //hideSelectionBox();
            refreshMap();
            //remember current tile
            //selectedTile.oldIndex = selectedTile.index;

            //draw selection box around tile
            int dx = gridx * tileSize;
            int dy = gridy * tileSize;
            Pen pen = new Pen(Color.DarkMagenta, 2);
            Rectangle rect = new Rectangle(dx + 1, dy + 1, tileSize - 2, tileSize - 2);
            gfx.DrawRectangle(pen, rect);

            //save changes
            pictureBox1.Image = drawArea;
        }
        /*public void refreshCmbBoxes()
        {
            cmbEncounter.BeginUpdate();
            cmbEncounter.DataSource = null;
            cmbEncounter.DataSource = prntForm.encountersList.encounters;
            cmbEncounter.DisplayMember = "EncounterName";
            cmbEncounter.EndUpdate();

            cmbContainer.BeginUpdate();
            cmbContainer.DataSource = null;
            cmbContainer.DataSource = prntForm.containersList.containers;
            cmbContainer.DisplayMember = "ContainerTag";
            cmbContainer.EndUpdate();

            cmbConversation.BeginUpdate();
            cmbConversation.DataSource = null;
            cmbConversation.DataSource = prntForm.mod.ModuleConvosList;
            cmbConversation.EndUpdate();

            cmbArea.BeginUpdate();
            cmbArea.DataSource = null;
            cmbArea.DataSource = prntForm.mod.ModuleAreasList;
            cmbArea.EndUpdate();

            fillSpriteIconList();

            cmbSpriteIcon.BeginUpdate();
            cmbSpriteIcon.DataSource = null;
            cmbSpriteIcon.DataSource = spriteIconList;
            cmbSpriteIcon.EndUpdate();

            fillScriptList();

            cmbScript.BeginUpdate();
            cmbScript.DataSource = null;
            cmbScript.DataSource = scriptList;
            cmbScript.EndUpdate();
        }*/
        public void refreshLeftPanelInfo()
        {
            //refreshCmbBoxes();
            //cleanUpUnchecked();
            //show selected tile # for editing
            selectedTile.x = gridx;
            selectedTile.y = gridy;
            selectedTile.index = gridy * area.MapSizeInSquares.Width + gridx;
            /*
            chkCollidable.Checked = area.TileMapList[selectedTile.index].collidable;
            chkVisible.Checked = area.TileMapList[selectedTile.index].visible;
            chkTransition.Checked = area.TileMapList[selectedTile.index].transition;
            transitionX.Value = area.TileMapList[selectedTile.index].transitionX;
            transitionY.Value = area.TileMapList[selectedTile.index].transitionY;
            chkScript.Checked = area.TileMapList[selectedTile.index].scriptChk;
            chkConversation.Checked = area.TileMapList[selectedTile.index].conversationChk;
            chkEncounter.Checked = area.TileMapList[selectedTile.index].encounterChk;
            chkContainer.Checked = area.TileMapList[selectedTile.index].containerChk;
            chkSpriteIcon.Checked = area.TileMapList[selectedTile.index].iconChk;
            cmbEncounter.SelectedIndex = cmbEncounter.FindStringExact(area.TileMapList[selectedTile.index].encounterTag);
            cmbConversation.SelectedIndex = cmbConversation.FindStringExact(area.TileMapList[selectedTile.index].conversationTag);
            cmbContainer.SelectedIndex = cmbContainer.FindStringExact(area.TileMapList[selectedTile.index].containerTag);
            cmbArea.SelectedIndex = cmbArea.FindStringExact(area.TileMapList[selectedTile.index].transitionFile);
            cmbSpriteIcon.SelectedIndex = cmbSpriteIcon.FindStringExact(area.TileMapList[selectedTile.index].iconFilename);
            cmbScript.SelectedIndex = cmbScript.FindStringExact(area.TileMapList[selectedTile.index].scriptTag);
            */
            //draw selection box
            drawSelectionBox(gridx, gridy);
        }
        /*private void fillSpriteIconList()
        {
            spriteIconList.Clear();
            string jobDir = prntForm._mainDirectory + "\\modules\\" + le_mod.ModuleFolderName + "\\graphics\\sprites\\tokens";
            foreach (string f in Directory.GetFiles(jobDir, "*.png"))
            {
                string filename = Path.GetFileName(f);
                spriteIconList.Add(filename);
            }
        }*/
        /*private void fillScriptList()
        {
            scriptList.Clear();
            string jobDir = prntForm._mainDirectory + "\\modules\\" + le_mod.ModuleFolderName + "\\scripts";
            foreach (string f in Directory.GetFiles(jobDir, "*.cs"))
            {
                string filename = Path.GetFileName(f);
                scriptList.Add(filename);
            }
            string defaultDir = prntForm._mainDirectory + "\\data\\scripts";
            foreach (string f in Directory.GetFiles(defaultDir, "*.cs"))
            {
                string filename = Path.GetFileName(f);
                if (!scriptList.Contains(filename))
                {
                    scriptList.Add(filename);
                }
            }
        }*/
        private void clickDrawArea(MouseEventArgs e)
        {
            switch (e.Button)
            {
                #region Left Button
                case MouseButtons.Left:
                    refreshLeftPanelInfo();
                    #region Creature Selected
                    if (prntForm.CreatureSelected)
                    {
                        string selectedCrt = prntForm.selectedLevelMapCreatureTag;
                        prntForm.logText(selectedCrt);
                        prntForm.logText(Environment.NewLine);

                        gridx = e.X / tileSize;
                        gridy = e.Y / tileSize;

                        prntForm.logText("gridx = " + gridx.ToString() + "gridy = " + gridy.ToString());
                        prntForm.logText(Environment.NewLine);
                        // verify that there is no creature, blocked, or PC already on this location
                        // add to a List<> a new item with the x,y coordinates
                        if (le_selectedCreature.SpriteFilename == "blank.spt")
                        {
                            return;
                        }
                        //check to see if a creature already exists on this square
                        foreach (CreatureRefs cr in area.AreaCreatureRefsList)
                        {
                            if (cr.CreatureStartLocation == new Point(gridx, gridy))
                            {
                                MessageBox.Show("Can only place one creature per square...aborting placement");
                                return;
                            }
                        }
                        CreatureRefs newCrtRef = new CreatureRefs();
                        newCrtRef.CreatureName = le_selectedCreature.Name;
                        newCrtRef.CreatureTag = le_selectedCreature.Tag + "_" + prntForm.mod.NextIdNumber;
                        newCrtRef.CreatureResRef = le_selectedCreature.ResRef;
                        newCrtRef.CreatureSize = le_selectedCreature.Size;
                        newCrtRef.MouseOverText = le_selectedCreature.MouseOverText;
                        newCrtRef.CreatureStartLocation = new Point(gridx, gridy);
                        area.AreaCreatureRefsList.Add(newCrtRef);                        
                        /*Creature newCrt = new Creature();
                        newCrt = le_selectedCreature.DeepCopy();
                        newCrt.Tag = le_selectedCreature.Tag + "_" + prntForm.mod.NextIdNumber;
                        newCrt.MapLocation = new Point(gridx, gridy);
                        area.AreaCreatureList.creatures.Add(newCrt);*/
                        // show the item on the map
                        if (le_mod.ModuleName != "NewModule")
                        {
                            Creature crt = le_selectedCreature.DeepCopy();
                            crt.LoadSpriteStuff(prntForm._mainDirectory + "\\modules\\" + le_mod.ModuleFolderName);
                            Bitmap newBitmap = (Bitmap)crt.CharSprite.Image.Clone();
                            //Bitmap newBitmap = new Bitmap(prntForm._mainDirectory + "\\modules\\" + le_mod.ModuleFolderName + "\\graphics\\sprites\\tokens\\module\\" + le_selectedCreature.CharSprite.SpriteSheetFilename);
                            crtBitmapList.Add(newBitmap);                            
                        }
                        else
                        {
                            Creature crt = le_selectedCreature.DeepCopy();
                            crt.LoadSpriteStuff(prntForm._mainDirectory + "\\data\\NewModule");
                            Bitmap newBitmap = (Bitmap)crt.CharSprite.Image.Clone();
                            //Bitmap newBitmap = new Bitmap(prntForm._mainDirectory + "\\data\\NewModule\\graphics\\sprites\\tokens\\" + le_selectedCreature.CharSprite.SpriteSheetFilename);
                            crtBitmapList.Add(newBitmap);
                        }
                    }
                    #endregion
                    #region Prop Selected
                    else if (prntForm.PropSelected)
                    {
                        string selectedProp = prntForm.selectedLevelMapPropTag;
                        prntForm.logText(selectedProp);
                        prntForm.logText(Environment.NewLine);

                        gridx = e.X / tileSize;
                        gridy = e.Y / tileSize;

                        prntForm.logText("gridx = " + gridx.ToString() + "gridy = " + gridy.ToString());
                        prntForm.logText(Environment.NewLine);
                        // verify that there is no creature, blocked, or PC already on this location
                        // add to a List<> a new item with the x,y coordinates
                        if (le_selectedProp.PropSpriteFilename == "blank.spt")
                        {
                            return;
                        }
                        PropRefs newPropRef = new PropRefs();
                        newPropRef.PropName = le_selectedProp.PropName;
                        newPropRef.PropTag = le_selectedProp.PropTag + "_" + prntForm.mod.NextIdNumber;
                        newPropRef.PropResRef = le_selectedProp.PropResRef;
                        newPropRef.PropContainerChk = le_selectedProp.PropContainerChk;
                        newPropRef.PropContainerTag = le_selectedProp.PropContainerTag;
                        newPropRef.PropTrappedChk = le_selectedProp.PropTrapped;
                        newPropRef.PropLockedChk = le_selectedProp.PropLocked;
                        newPropRef.PropKeyTag = le_selectedProp.PropKeyTag;
                        newPropRef.MouseOverText = le_selectedProp.MouseOverText;
                        newPropRef.PropStartLocation = new Point(gridx, gridy);
                        area.AreaPropRefsList.Add(newPropRef);                        
                        /*Prop newProp = new Prop();
                        newProp = le_selectedProp.DeepCopy();
                        newProp.PropTag = le_selectedProp.PropTag + "_" + prntForm.mod.NextIdNumber;
                        newProp.Location = new Point(gridx, gridy);
                        area.AreaPropList.propsList.Add(newProp);*/
                        // show the item on the map
                        if (le_mod.ModuleName != "NewModule")
                        {
                            Prop prp = le_selectedProp.DeepCopy();
                            prp.LoadPropSpriteStuffForTS(prntForm._mainDirectory + "\\modules\\" + le_mod.ModuleFolderName);
                            Bitmap newBitmap = (Bitmap)prp.PropSprite.Image.Clone();
                            //Bitmap newBitmap = new Bitmap(prntForm._mainDirectory + "\\modules\\" + le_mod.ModuleFolderName + "\\graphics\\sprites\\props\\" + le_selectedProp.PropSprite.SpriteSheetFilename);
                            propBitmapList.Add(newBitmap);
                        }
                        else
                        {
                            Prop prp = le_selectedProp.DeepCopy();
                            prp.LoadPropSpriteStuffForTS(prntForm._mainDirectory + "\\data\\NewModule");
                            Bitmap newBitmap = (Bitmap)prp.PropSprite.Image.Clone();
                            //Bitmap newBitmap = new Bitmap(prntForm._mainDirectory + "\\data\\NewModule\\graphics\\sprites\\props\\" + le_selectedProp.PropSprite.SpriteSheetFilename);
                            propBitmapList.Add(newBitmap);
                        }
                    }
                    #endregion
                    #region Paint New Trigger Selected
                    else if (rbtnPaintTrigger.Checked)
                    {                                                                  
                        string selectedTrigger = prntForm.selectedLevelMapTriggerTag;
                        prntForm.logText(selectedTrigger);
                        prntForm.logText(Environment.NewLine);

                        gridx = e.X / tileSize;
                        gridy = e.Y / tileSize;

                        prntForm.logText("gridx = " + gridx.ToString() + "gridy = " + gridy.ToString());
                        prntForm.logText(Environment.NewLine);
                        Point newPoint = new Point(gridx, gridy);
                        //add the selected square to the squareList if doesn't already exist
                        try
                        {
                            //check: if click square already exists, then erase from list                            
                            Trigger newTrigger = area.AreaTriggerList.getTriggerByTag(selectedTrigger);
                            bool exists = false;
                            foreach (Point p in newTrigger.TriggerSquaresList)
                            {
                                if (p == newPoint)
                                {
                                    //already exists, erase
                                    newTrigger.TriggerSquaresList.Remove(p);
                                    exists = true;
                                    break;
                                }
                            }
                            if (!exists) //doesn't exist so is a new point, add to list
                            {
                                newTrigger.TriggerSquaresList.Add(newPoint);
                            }
                        }
                        catch
                        {
                            MessageBox.Show("The tag of the selected Trigger was not found in the area's trigger list"); 
                        }                        
                        //update the map to show colored squares    
                        refreshMap();
                    }
                    #endregion
                    #region Edit Trigger Selected
                    else if (rbtnEditTrigger.Checked)
                    {
                        if (prntForm.selectedLevelMapTriggerTag != null)
                        {
                            string selectedTrigger = prntForm.selectedLevelMapTriggerTag;
                            prntForm.logText(selectedTrigger);
                            prntForm.logText(Environment.NewLine);

                            gridx = e.X / tileSize;
                            gridy = e.Y / tileSize;

                            prntForm.logText("gridx = " + gridx.ToString() + "gridy = " + gridy.ToString());
                            prntForm.logText(Environment.NewLine);
                            Point newPoint = new Point(gridx, gridy);
                            try
                            {
                                //check: if click square already exists, then erase from list                            
                                Trigger newTrigger = area.AreaTriggerList.getTriggerByTag(selectedTrigger);
                                if (newTrigger == null)
                                {
                                    MessageBox.Show("error: make sure to select a trigger to edit first (click info button then click on trigger)");
                                }
                                bool exists = false;
                                foreach (Point p in newTrigger.TriggerSquaresList)
                                {
                                    if (p == newPoint)
                                    {
                                        //already exists, erase
                                        newTrigger.TriggerSquaresList.Remove(p);
                                        exists = true;
                                        break;
                                    }
                                }
                                if (!exists) //doesn't exist so is a new point, add to list
                                {
                                    newTrigger.TriggerSquaresList.Add(newPoint);
                                }
                            }
                            catch
                            {
                                MessageBox.Show("The tag of the selected Trigger was not found in the area's trigger list");
                            }
                            //update the map to show colored squares    
                            refreshMap();
                        }
                    }
                    #endregion
                    #region Walkmesh Toggle Selected
                    else if (rbtnWalkable.Checked)
                    {
                        gridx = e.X / tileSize;
                        gridy = e.Y / tileSize;
                        selectedTile.index = gridy * area.MapSizeInSquares.Width + gridx;
                        prntForm.logText("gridx = " + gridx.ToString() + "gridy = " + gridy.ToString());
                        prntForm.logText(Environment.NewLine);
                        if (area.TileMapList[selectedTile.index].collidable == true)
                            area.TileMapList[selectedTile.index].collidable = false;
                        else
                            area.TileMapList[selectedTile.index].collidable = true;
                        refreshMap();
                    }
                    #endregion
                    #region LoS mesh Toggle Selected
                    else if (rbtnLoS.Checked)
                    {
                        gridx = e.X / tileSize;
                        gridy = e.Y / tileSize;
                        selectedTile.index = gridy * area.MapSizeInSquares.Width + gridx;
                        prntForm.logText("gridx = " + gridx.ToString() + "gridy = " + gridy.ToString());
                        prntForm.logText(Environment.NewLine);
                        if (area.TileMapList[selectedTile.index].LoSBlocked == true)
                            area.TileMapList[selectedTile.index].LoSBlocked = false;
                        else
                            area.TileMapList[selectedTile.index].LoSBlocked = true;
                        refreshMap();
                    }
                    #endregion
                    #region None Selected
                    else // not placing, just getting info and possibly deleteing icon
                    {
                        contextMenuStrip1.Items.Clear();
                        //when left click, get location
                        gridx = e.X / tileSize;
                        gridy = e.Y / tileSize;
                        Point newPoint = new Point(gridx, gridy);                        
                        EventHandler handler = new EventHandler(HandleContextMenuClick);
                        //loop through all the objects
                        //if has that location, add the tag to the list                    
                        //draw selection box
                        drawSelectionBox(gridx, gridy);
                        txtSelectedIconInfo.Text = "";

                        foreach (CreatureRefs crt in area.AreaCreatureRefsList)
                        {
                            if (crt.CreatureStartLocation == newPoint)
                            {
                                // if so then give details about that icon (name, tag, etc.)
                                txtSelectedIconInfo.Text = "name: " + crt.CreatureName + Environment.NewLine
                                                            + "tag: " + crt.CreatureTag + Environment.NewLine
                                                            + "resref: " + crt.CreatureResRef;
                                //lastSelectedObjectResRef = crt.CreatureResRef;
                                lastSelectedObjectTag = crt.CreatureTag;
                                pictureBox1.ContextMenuStrip.Items.Add(crt.CreatureTag, null, handler); //string, image, handler
                                prntForm.frmIceBlinkProperties.propertyGrid1.SelectedObject = crt;
                            }
                        }
                        foreach (PropRefs prp in area.AreaPropRefsList)
                        {
                            if (prp.PropStartLocation == newPoint)
                            {
                                // if so then give details about that icon (name, tag, etc.)
                                txtSelectedIconInfo.Text = "name: " + prp.PropName + Environment.NewLine
                                                            + "tag: " + prp.PropTag + Environment.NewLine
                                                            + "resref: " + prp.PropResRef;
                                lastSelectedObjectResRef = prp.PropResRef;
                                lastSelectedObjectTag = prp.PropTag;
                                pictureBox1.ContextMenuStrip.Items.Add(prp.PropTag, null, handler); //string, image, handler
                                prntForm.frmIceBlinkProperties.propertyGrid1.SelectedObject = prp;
                            }
                        }
                        /*foreach (Creature crt in area.AreaCreatureList.creatures)
                        {
                            if (crt.MapLocation == newPoint)
                            {
                                // if so then give details about that icon (name, tag, etc.)
                                txtSelectedIconInfo.Text = "name: " + crt.Name + Environment.NewLine + "tag: " + crt.Tag;
                                lastSelectedObjectTag = crt.Tag;
                                //prntForm.selectedLevelMapCreatureTag = crt.Tag;
                                pictureBox1.ContextMenuStrip.Items.Add(crt.Tag, null, handler); //string, image, handler
                                prntForm.frmIceBlinkProperties.propertyGrid1.SelectedObject = crt;
                            }
                        }
                        foreach (Prop prp in area.AreaPropList.propsList)
                        {
                            if (prp.Location == newPoint)
                            {
                                // if so then give details about that icon (name, tag, etc.)
                                txtSelectedIconInfo.Text = "name: " + prp.PropName + Environment.NewLine + "tag: " + prp.PropTag;
                                lastSelectedObjectTag = prp.PropTag;
                                //prntForm.selectedLevelMapPropTag = prp.PropTag;
                                pictureBox1.ContextMenuStrip.Items.Add(prp.PropTag, null, handler); //string, image, handler
                                prntForm.frmIceBlinkProperties.propertyGrid1.SelectedObject = prp;
                            }
                        }*/
                        foreach (Trigger t in area.AreaTriggerList.triggersList)
                        {
                            foreach (Point p in t.TriggerSquaresList)
                            {
                                if (p == newPoint)
                                {
                                    txtSelectedIconInfo.Text = "Trigger Tag: " + Environment.NewLine + t.TriggerTag;
                                    lastSelectedObjectTag = t.TriggerTag;
                                    pictureBox1.ContextMenuStrip.Items.Add(t.TriggerTag, null, handler); //string, image, handler
                                    prntForm.frmIceBlinkProperties.propertyGrid1.SelectedObject = t;
                                }
                            }
                        }
                        //if the list is less than 2, do nothing
                        if (pictureBox1.ContextMenuStrip.Items.Count > 1)
                        {
                            contextMenuStrip1.Show(pictureBox1, e.Location);
                        }
                    }
                    #endregion
                    break;
                #endregion
                #region Right Button
                case MouseButtons.Right:
                    // exit by right click or ESC
                    prntForm.logText("entered right-click");
                    prntForm.logText(Environment.NewLine);
                    //prntForm.selectedEncounterCreatureTag = "";
                    prntForm.selectedLevelMapCreatureTag = "";
                    prntForm.selectedLevelMapPropTag = "";
                    prntForm.CreatureSelected = false;
                    prntForm.PropSelected = false;
                    refreshMap();
                    pictureBox1.Image = drawArea;
                    rbtnInfo.Checked = true;
                    break;
                #endregion
            }
        }
        
        #endregion

        #region Event Handlers
        private void btnLoadMap_Click(object sender, EventArgs e)
        {
            if (le_mod.ModuleName != "NewModule")
                openFileDialog2.InitialDirectory = prntForm._mainDirectory + "\\modules\\" + le_mod.ModuleFolderName + "\\areas";
            else
                openFileDialog2.InitialDirectory = prntForm._mainDirectory + "\\data\\NewModule";
            //openFileDialog2.InitialDirectory = Environment.CurrentDirectory;
            //Empty the FileName text box of the dialog
            openFileDialog2.FileName = String.Empty;
            openFileDialog2.Filter = "Map (*.jpg)|*.jpg";
            openFileDialog2.FilterIndex = 1;

            DialogResult result = openFileDialog2.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string filename = Path.GetFullPath(openFileDialog2.FileName);
                area.MapFileName = Path.GetFileName(openFileDialog2.FileName);
                //string directory = Path.GetDirectoryName(openFileDialog2.FileName);
                //MessageBox.Show("filename selected = " + filename);
                gameMapBitmap = new Bitmap(filename);
                drawArea = new Bitmap(filename);
                pictureBox1.Width = gameMapBitmap.Size.Width;
                pictureBox1.Height = gameMapBitmap.Size.Height;
                pictureBox1.Image = (Image)drawArea;
                gfx = Graphics.FromImage(drawArea);

                if (drawArea == null)
                {
                    MessageBox.Show("returned a null bitmap");
                    le_game.errorLog("returned a null Map bitmap");
                }

                resetAreaTileValues(gameMapBitmap.Size.Width / tileSize, gameMapBitmap.Size.Height / tileSize);
                refreshMap();
            }
        }
        private void btnSaveArea_Click(object sender, EventArgs e)
        {
            saveTilemapFile();
            MessageBox.Show("Area Saved");
        }
        private void LevelEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            //MessageBox.Show("closing editor and removing from openAreaList");
            prntForm.openAreasList.Remove(area);
        }
        private void LevelEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            gfx.Dispose();
            drawArea.Dispose();          
            //gfxSelected.Dispose();
            //selectedBitmap.Dispose();
            //this.Close();
        }
        private void LevelEditor_Load(object sender, EventArgs e)
        {
            //prntForm = (ParentForm)this.ParentForm;
            try
            {
                g_walkPass = new Bitmap(prntForm._mainDirectory + "\\data\\graphics\\walkPass.png");
                g_walkBlock = new Bitmap(prntForm._mainDirectory + "\\data\\graphics\\walkBlock.png");
                g_LoSBlock = new Bitmap(prntForm._mainDirectory + "\\data\\graphics\\LoSBlock.png");
            }
            catch (Exception ex)
            {
                MessageBox.Show("failed to load walkPass and walkBlock bitmaps: " + ex.ToString());
                le_game.errorLog("failed to load walkPass and walkBlock bitmaps: " + ex.ToString());
            }
            area = new Area();
            area.MapSizeInSquares.Width = 16;
            area.MapSizeInSquares.Height = 16;
            //createNewArea(area.MapSizeInSquares.Width, area.MapSizeInSquares.Height);
            //set up level drawing surface
            drawArea = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            pictureBox1.Image = drawArea;
            gfx = Graphics.FromImage(drawArea);

            //selected image
            //selectedBitmap = new Bitmap(picSelected.Size.Width, picSelected.Size.Height);
            //picSelected.Image = selectedBitmap;
            //gfxSelected = Graphics.FromImage(selectedBitmap);

            //create font
            //fontArial = new Font("Arial Narrow", 40);

            // try and load the file selected if it exists
            string g_filename = le_mod.ModuleAreasList[prntForm._selectedLbxAreaIndex];
            string g_directory = prntForm._mainDirectory + "\\modules\\" + le_mod.ModuleFolderName + "\\areas";
            string filenameNoExtension = Path.GetFileNameWithoutExtension(le_mod.ModuleAreasList[prntForm._selectedLbxAreaIndex]);
            if (File.Exists(g_directory + "\\" + g_filename + ".level"))
            {
                openLevel(g_directory, g_filename, filenameNoExtension);
                if (area == null)
                {
                    createNewArea(area.MapSizeInSquares.Width, area.MapSizeInSquares.Height);
                    area.AreaFileName = g_filename;
                }
            }
            else
            {
                createNewArea(area.MapSizeInSquares.Width, area.MapSizeInSquares.Height);
                area.AreaFileName = g_filename;
            }
            //refreshCmbBoxes();
            prntForm.openAreasList.Add(area);
            rbtnInfo.Checked = true;            
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            clickDrawArea(e);
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {            
            gridx = e.X / tileSize;
            gridy = e.Y / tileSize;
            mousex = e.X;
            mousey = e.Y;
            lblMouseInfo.Text = "CURSOR " + e.X.ToString() + "," + e.Y.ToString() + Environment.NewLine + "GRID " + gridx.ToString() + "," + gridy.ToString();
            if ((prntForm.CreatureSelected) || (prntForm.PropSelected))
            {
                refreshMap();
                try
                {
                    if (selectedBitmap != null)
                    {
                        //source image size
                        Rectangle frame = new Rectangle(0, 0, tileSize * selectedBitmapSize, tileSize * selectedBitmapSize);
                        //target location
                        Rectangle target = new Rectangle(gridx * tileSize, gridy * tileSize, tileSize * selectedBitmapSize, tileSize * selectedBitmapSize);
                        //draw sprite
                        gfx.DrawImage((Image)selectedBitmap, target, frame, GraphicsUnit.Pixel);
                    }
                }
                catch (Exception ex) { MessageBox.Show("failed mouse move: " + ex.ToString()); }
                //save changes
                pictureBox1.Image = drawArea;
            }
            // * sinopip, 22.12.14
            // * enable scroll area when mouse is on borders (scrollbar position values are negative)
            is_leftscrolling = false;
        	is_rightscrolling = false;
        	is_upscrolling = false;
        	is_downscrolling = false;
        	if (mousex < 100 + -panel3.AutoScrollPosition.X) 
        		is_leftscrolling = true;
        	if (mousey < 100 + -panel3.AutoScrollPosition.Y) 
        		is_upscrolling = true;
        	if (mousex > (panel3.Width-100) + -panel3.AutoScrollPosition.X)
        		is_rightscrolling = true;
        	if (mousey > (panel3.Height-100) + -panel3.AutoScrollPosition.Y)
        		is_downscrolling = true;
			//                          
        }
        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
        	// * sinopip, 22.12.14
            is_leftscrolling = false;
        	is_rightscrolling = false;
        	is_upscrolling = false;
        	is_downscrolling = false;   
			//        	
            try
            {
                refreshMap();
                pictureBox1.Image = drawArea;
            }
            catch (Exception ex)
            {
                MessageBox.Show("failed on mouse leave map: " + ex.ToString());
            }
        }
        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.Select();
            try
            {
                if (prntForm.selectedLevelMapCreatureTag != "")
                {
                    prntForm.CreatureSelected = true;
                }
                if (prntForm.selectedLevelMapPropTag != "")
                {
                    prntForm.PropSelected = true;
                }
                if (prntForm.CreatureSelected)
                {
                    string selectedCrt = prntForm.selectedLevelMapCreatureTag;
                    le_selectedCreature = prntForm.creaturesList.getCreatureByTag(selectedCrt);
                    if (le_selectedCreature != null)
                    {
                        selectedBitmap = le_selectedCreature.CharSprite.Image;
                        selectedBitmapSize = le_selectedCreature.Size;
                    }
                }
                else if (prntForm.PropSelected)
                {
                    string selectedProp = prntForm.selectedLevelMapPropTag;
                    le_selectedProp = prntForm.propsList.getPropByTag(selectedProp);
                    if (le_selectedProp != null)
                    {
                        selectedBitmap = le_selectedProp.PropSprite.Image;
                        selectedBitmapSize = le_selectedProp.PropSprite.SpriteSize.Width / tileSize;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("failed on mouse enter map: " + ex.ToString());
            }
        }
        /*private void LevelEditor_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    chkCollidable.Checked = !chkCollidable.Checked;
                    refreshMap();
                    break;
            }
        }
        private void LevelEditor_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    //Close();
                    break;
            }
        }*/
        private void btnRemoveSelectedObject_Click(object sender, EventArgs e)
        {
            int cnt = 0;
            foreach (CreatureRefs crt in area.AreaCreatureRefsList)
            {
                //if (crt.CreatureResRef == lastSelectedObjectResRef)
                if (crt.CreatureTag == lastSelectedObjectTag)
                {
                    // remove at index of matched location
                    area.AreaCreatureRefsList.RemoveAt(cnt);
                    crtBitmapList.RemoveAt(cnt);
                    refreshMap();
                    return;
                }
                cnt++;
            }
            /*cnt = 0;
            foreach (Creature crt in area.AreaCreatureList.creatures)
            {
                if (crt.Tag == lastSelectedObjectTag)
                {
                    // remove at index of matched tag
                    area.AreaCreatureList.creatures.RemoveAt(cnt);
                    crtBitmapList.RemoveAt(cnt);
                    refreshMap();
                    return;
                }
                cnt++;
            }*/
            cnt = 0;
            foreach (PropRefs prp in area.AreaPropRefsList)
            {
                //if (prp.PropResRef == lastSelectedObjectResRef)
                if (prp.PropTag == lastSelectedObjectTag)
                {
                    // remove at index of matched tag
                    area.AreaPropRefsList.RemoveAt(cnt);
                    propBitmapList.RemoveAt(cnt);
                    refreshMap();
                    return;
                }
                cnt++;
            }
            /*cnt = 0;
            foreach (Prop prp in area.AreaPropList.propsList)
            {
                if (prp.PropTag == lastSelectedObjectTag)
                {
                    // remove at index of matched tag
                    area.AreaPropList.propsList.RemoveAt(cnt);
                    propBitmapList.RemoveAt(cnt);
                    refreshMap();
                    return;
                }
                cnt++;
            }*/
            foreach (Trigger t in area.AreaTriggerList.triggersList)
            {
                if (t.TriggerTag == lastSelectedObjectTag)
                {
                    // remove at index of matched tag
                    area.AreaTriggerList.triggersList.Remove(t);
                    refreshMap();
                    return;
                }
            }
        }
        private void rbtnInfo_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnInfo.Checked)
            {
                prntForm.logText("info on selecting map objects");
                prntForm.logText(Environment.NewLine);
                prntForm.selectedLevelMapCreatureTag = "";
                prntForm.selectedLevelMapPropTag = "";
                prntForm.CreatureSelected = false;
                prntForm.PropSelected = false;
                refreshMap();
                pictureBox1.Image = drawArea;
            }
        }
        private void rbtnPaintTrigger_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnPaintTrigger.Checked)
            {
                //create a new trigger object
                Trigger newTrigger = new Trigger();
                newTrigger.passRefs(le_game, prntForm, area);
                //increment the tag to something unique
                newTrigger.TriggerTag = "newTrigger_" + prntForm.mod.NextIdNumber;
                prntForm.selectedLevelMapTriggerTag = newTrigger.TriggerTag;
                area.AreaTriggerList.triggersList.Add(newTrigger);
                //set propertygrid to the new object
                prntForm.frmIceBlinkProperties.propertyGrid1.SelectedObject = newTrigger;
                prntForm.logText("painting a new trigger");
                prntForm.logText(Environment.NewLine);
                prntForm.selectedLevelMapCreatureTag = "";
                prntForm.selectedLevelMapPropTag = "";
                prntForm.CreatureSelected = false;
                prntForm.PropSelected = false;
                refreshMap();
                pictureBox1.Image = drawArea;
            }
        }
        private void rbtnEditTrigger_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnEditTrigger.Checked)
            {
                prntForm.logText("edit trigger: ");
                prntForm.logText(Environment.NewLine);
                prntForm.selectedLevelMapCreatureTag = "";
                prntForm.selectedLevelMapPropTag = "";
                prntForm.CreatureSelected = false;
                prntForm.PropSelected = false;
                prntForm.selectedLevelMapTriggerTag = lastSelectedObjectTag;
                refreshMap();
                pictureBox1.Image = drawArea;
            }
        }
        private void rbtnWalkable_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnWalkable.Checked)
            {
                prntForm.logText("editing walkmesh");
                prntForm.logText(Environment.NewLine);
                prntForm.selectedLevelMapCreatureTag = "";
                prntForm.selectedLevelMapPropTag = "";
                prntForm.selectedLevelMapTriggerTag = "";
                prntForm.CreatureSelected = false;
                prntForm.PropSelected = false;
                refreshMap();
                pictureBox1.Image = drawArea;
            }
        }
        private void rbtnLoS_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnLoS.Checked)
            {
                prntForm.logText("editing line-of-sight mesh");
                prntForm.logText(Environment.NewLine);
                prntForm.selectedLevelMapCreatureTag = "";
                prntForm.selectedLevelMapPropTag = "";
                prntForm.selectedLevelMapTriggerTag = "";
                prntForm.CreatureSelected = false;
                prntForm.PropSelected = false;
                refreshMap();
                pictureBox1.Image = drawArea;
            }
        }
        private void chkGrid_CheckedChanged(object sender, EventArgs e)
        {
            showGrid = chkGrid.Checked;
            refreshMap();
        }
        private void LevelEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                // exit by right click or ESC
                prntForm.logText("pressed escape");
                prntForm.logText(Environment.NewLine);
                //prntForm.selectedEncounterCreatureTag = "";
                prntForm.selectedLevelMapCreatureTag = "";
                prntForm.selectedLevelMapPropTag = "";
                prntForm.CreatureSelected = false;
                prntForm.PropSelected = false;
                refreshMap();
                pictureBox1.Image = drawArea;
                rbtnInfo.Checked = true;
            }
        }
        public void HandleContextMenuClick(object sender, EventArgs e)
        {
            //else, handler returns the selected tag
            ToolStripMenuItem menuItm = (ToolStripMenuItem)sender;
            //get object by tag
            //Set propertygrid to that object
            //set last selected object...used for remove
            foreach (CreatureRefs crt in area.AreaCreatureRefsList)
            {
                if (crt.CreatureTag == menuItm.Text)
                {
                    // if so then give details about that icon (name, tag, etc.)
                    txtSelectedIconInfo.Text = "name: " + crt.CreatureName + Environment.NewLine + "tag: " + crt.CreatureTag;
                    lastSelectedObjectResRef = crt.CreatureResRef;
                    lastSelectedObjectTag = crt.CreatureTag;
                    prntForm.frmIceBlinkProperties.propertyGrid1.SelectedObject = crt;
                    return;
                }
            }
            /*foreach (Creature crt in area.AreaCreatureList.creatures)
            {
                if (crt.Tag == menuItm.Text)
                {
                    // if so then give details about that icon (name, tag, etc.)
                    txtSelectedIconInfo.Text = "name: " + crt.Name + Environment.NewLine + "tag: " + crt.Tag;
                    lastSelectedObjectTag = crt.Tag;
                    //prntForm.selectedLevelMapCreatureTag = crt.Tag;
                    prntForm.frmIceBlinkProperties.propertyGrid1.SelectedObject = crt;
                    return;
                }
            }*/
            foreach (PropRefs prp in area.AreaPropRefsList)
            {
                if (prp.PropTag == menuItm.Text)
                {
                    // if so then give details about that icon (name, tag, etc.)
                    txtSelectedIconInfo.Text = "name: " + prp.PropName + Environment.NewLine + "tag: " + prp.PropTag;
                    lastSelectedObjectResRef = prp.PropResRef;
                    lastSelectedObjectTag = prp.PropTag;
                    prntForm.frmIceBlinkProperties.propertyGrid1.SelectedObject = prp;
                    return;
                }
            }
            /*foreach (Prop prp in area.AreaPropList.propsList)
            {
                if (prp.PropTag == menuItm.Text)
                {
                    // if so then give details about that icon (name, tag, etc.)
                    txtSelectedIconInfo.Text = "name: " + prp.PropName + Environment.NewLine + "tag: " + prp.PropTag;
                    lastSelectedObjectTag = prp.PropTag;
                    //prntForm.selectedLevelMapPropTag = prp.PropTag;
                    prntForm.frmIceBlinkProperties.propertyGrid1.SelectedObject = prp;
                    return;
                }
            }*/
            foreach (Trigger t in area.AreaTriggerList.triggersList)
            {
                if (t.TriggerTag == menuItm.Text)
                {
                    txtSelectedIconInfo.Text = "Trigger Tag: " + Environment.NewLine + t.TriggerTag;
                    lastSelectedObjectTag = t.TriggerTag;
                    prntForm.frmIceBlinkProperties.propertyGrid1.SelectedObject = t;
                    return;
                }
            }
        }
        #endregion  

        private void btnProperties_Click(object sender, EventArgs e)
        {
            prntForm.frmIceBlinkProperties.propertyGrid1.SelectedObject = area;
        }

        private void pictureBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                // exit by right click or ESC
                prntForm.logText("pressed escape");
                prntForm.logText(Environment.NewLine);
                //prntForm.selectedEncounterCreatureTag = "";
                prntForm.selectedLevelMapCreatureTag = "";
                prntForm.selectedLevelMapPropTag = "";
                prntForm.CreatureSelected = false;
                prntForm.PropSelected = false;
                refreshMap();
                pictureBox1.Image = drawArea;
                rbtnInfo.Checked = true;
            }
        }

        // * sinopip, 22.12.14
		// * scroll the map (timer component of 25ms tick)
        void ScrollTimerTick(object sender, EventArgs e)
        {
        	if (is_leftscrolling)
        		panel3.AutoScrollPosition = new Point(
        			-panel3.AutoScrollPosition.X - 16,
        			-panel3.AutoScrollPosition.Y);
        	if (is_rightscrolling) panel3.AutoScrollPosition = new Point(
        			-panel3.AutoScrollPosition.X + 16,
        			-panel3.AutoScrollPosition.Y);
        	if (is_upscrolling) panel3.AutoScrollPosition = new Point(
        			-panel3.AutoScrollPosition.X,
        			-panel3.AutoScrollPosition.Y - 16);
        	if (is_downscrolling) panel3.AutoScrollPosition = new Point(
        			-panel3.AutoScrollPosition.X,
        			-panel3.AutoScrollPosition.Y + 16);    	
        }
		//  
    }
}

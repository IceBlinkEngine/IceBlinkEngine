using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using IceBlinkToolset;
using IceBlinkCore;
using IceBlink;
using System.Threading;
using WMPLib;

///Instructions and Version History
///
/// VERSION HISTORY
/// 
/// DATE        TAG             COMMENTS
/// ----------  -----------     ----------------------------------------------------------------------
/// 2013-10-27  SD_20131027     Initial kick-off of history section. Use the TAGs at the end of edited
///                             code lines so that everyone can easily see the lines of code that were
///                             updated. For example use "//SD_20131027" at the end of an edited line.
///                             If you have multiple lines of code in a row that are part of your edit,
///                             just add a TAG at the top like "start SD_20131027" and at the end of
///                             those edits like "end SD_20131027".
///                             Make sure to follow the "Comment Out" process stated below.
/// 2013-10-27  YN1_20131027    Made changes in SF.cs and Pf.cs
///                             Added two new overloads (under the pathfinding region) and made bug fixes
///                             to Pf.cs 
/// 2013-10-29  SD_20131029     Added some addtional code to Stats.cs for updating saving throw bases
/// 2013-11-02  SD_20131102     Added a new parameter "ConvoOwnerTag" to allow for easy CheckLocalInt and
///                             SetLocalInt in conversations.
/// 2013-11-02  SD_20131102     Added a bunch of DebugMode output to MainMap log (use "D" hotkey to toggle)
///                             Added a CheckAttribute() function
/// 2013-11-08  SD_20131108     Added a RemoveCreatureByTag() function
/// 2013-11-10  SD_20131110     Added a new Enum set called "TargetType" and updated all spell and trait scripts
/// 2013-11-15  SD_20131115     Created Tables for BAB, SkillPointsToSpendAtLevelUp, SpellPointsToSpendAtLevelUp,
///                             TraitPointsToSpendAtLevelUp
/// 2013-11-16  SD_20131116     Added a CheckIsAlignment() function
///                             Added movement calcs based on race and armor weight
/// 2013-11-20  SD_20131120     Fixed CheckForItem() and TakeItem() whcih were missing 4 of the 8 equipped slots
/// 2013-11-24  SD_20131124     Created a new Enum for DamageType for use with Items and Creatures
///                             Created a bunch of calculations for resistances in UpdateStats
/// 2013-11-26  SD_20131126     Added resistances to the damage calculations on PCs and Creatures
/// 2013-11-27  SD_20131127     Changed the way Items can modify saving throws...reflex, will and fortitude each
///                             have their own property on the item so the item can modify multiple saving throws.
///                             Also, did the same for Item attribute modifiers, made individual properties for
///                             each attribute.
/// 2013-12-01  SD_20131201     Fixed a bug with the "toughness" trait (raises maxHP) by moving the HP > maxHP
///                             check to the very end of the UpdateStats() function.
/// 2013-12-03  SD_20131203     Reveamped DoCreatureAttackOfOpportunity() to be similar to free attack against shooter
///                             code found in dsAttackPC.cs
/// 2013-12-15  SD_20131215     Fixed a bug with LeaveThreatenedCheck where sleeping and held creatures were still attacking
/// 
/// 
/// INSTRUCTIONS:
/// 
/// I think that we will try using the following standards for editing the ScripFunctions.dll (sf.dll)
/// as a way of trying to keep things organized. Any suggestions on ways to improve this process are
/// welcome. Here are the options:
/// 
/// 1) Comment Out -  If you would like to submit a change to any of the standard functions and your
///                   changes do not adversly impact other projects, then just comment out (use "//"
///                   in front of each line of code to comment out) the old code lines that you wish
///                   to replace and put your new code under that. I will review the new code and if
///                   it is okay, I will then delete the commented out part and your code will be the
///                   new standard (will keep the TAG on your new code line forever ;-).
/// 
/// 2) Overload -     If you want to change an entire function just for your projects without impacting
///                   others and you do not want to suggest replacing the original function as the new
///                   standard, then use an overload. I suggest copying the original function and just
///                   adding a new parameter input to the end of the inputs like say "string Karl". Here
///                   is an example:
///                   ORIGINAL
///                   public void GiveItem(string tag, int quantity)
///                   {
///                      Item newItem = gm.module.ModuleItemsList.getItemByTag(tag).DeepCopy();
///                      ....
///                   }
///                   YOUR OVERLOAD
///                   public void GiveItem(string tag, int quantity, string Karl)
///                   {
///                      Item newItem = gm.module.ModuleItemsList.getItemByTag(tag).DeepCopy();
///                      ....
///                   }
///                   Now when you want to use your function instead of the original one, just add a string
///                   to the end of the function call in your scripts like so:
///                   sf.GiveItem("coolSword", 1, "Karl Overload of Super Coolness");
///                   if you want to use the original one somewhere else in your script you would do this:
///                   sf.GiveItem("coolSword", 1);
///        
/// 3) New Function - You can create completely new functions as well. Just give them a different name than
///                   exisiting functions.
///                   
/// If you are adding overloads or new functions, it may be a good idea to place them at the bottom of the 
/// sf.dll code in the "#region Author Functions" section. This way you can easily copy them over to newer
/// versions of sf.dll
/// 
/// 

namespace IceBlink
{
	
	public class SpecialActionResult
	{
		public int Roll = 0;
		public int RollMod = 0;
		public int DC = 0;
		public bool SavedAgainstSuccessfully = false;
		public int Score = 0; // ie damage
		public int ScoreFinal = 0; // ie damage after Save roll
		
		public SpecialActionResult()
		{

		}
	}
	
	public class SpellParameters
	{
		public string Name = "";
		public string Type = ""; // "Heal", "Damage", "Buff" or "Debuff" ; new "Summon"
		public string TargetType = "";
		public List<string> ExtraParams;
		public Effect Effect;
		public string CountersEffects = "";
		public int NbDice = 0;
		public int Die = 1;
		public int DiceAdd = 0; // <NbDice>d<Die> +<DiceAdd>
		public int BaseDC = 0;
		public double SuccessSaveResistance = 0.0;
		public string EnergySource = "";
		public string StatMod = ""; // "INT", "WIS", "CHA"
		public string Save; // "Fortitude", "Reflex" or "Will"
		public Color SpellColor = Color.Black; // ie "Color.Blue" for a cold spell
		public string Description = "";
		public string EffectDescription = "";
		public string SpriteFileName = "";
		public string SoundFX = "";

		
		public SpellParameters()
		{
			ExtraParams = new List<string>();
		}
	}
	
    public class ScriptFunctions
    {
		#region CORE FUNCTIONS    	
        #region Properties
        public Form1 frm;
        public Game gm;
        public object CombatTarget = new object(); //can be an (int) for PCindex, (PC) for a PC, (Point) for map location, or (Creature) for creatures
        public object CombatSource = new object(); //can be an (int) for PCindex, (PC) for a PC, or (Creature) for creatures
        public object MainMapTarget = new object(); //can be an (int) for PCindex, (PC) for a PC, (Point) for map location, or (Creature) for creatures
        public object MainMapSource = new object(); //can be an (int) for PCindex, (PC) for a PC, or (Creature) for creatures
        public bool MainMapScriptCall = false;      //set to true when a script is called from the main map, make sure to set to false after use
        public object returnScriptObject = null;   //can be set to any object type by script and cast back to the original type here, make sure to null out after use 
        public object ActionToTake = null;         //can be set to any object type by script and cast back to the original type here, make sure to null out after use 
        public Spell SpellToCast = null;           //Spell that the creature is casting, make sure to null out after use 
        public Trait TraitToUse = null;            //Trait that the creature is using, make sure to null out after use 
        public object passParameterScriptObject = null;   //can be set to any object type passed into script, make sure to null out after use 
        public string ConvoOwnerTag = "";          //The engine will use this to pass the Tag of the Creature or Prop that owns the current conversation ( SD_20131102 )
        public Point lastPcCombatLocation = new Point(0, 0);
        public CharacterSheet currentCharacterSheet = new CharacterSheet();
        public Pathfinder pathfinder;
        public PathfinderMainArea pathfinderMainArea;
        #endregion

        public ScriptFunctions()
        {
        }
        public void passRefs(Form1 f, Game g)
        {
            frm = f;
            gm = g;
            //pathfinder = new Pathfinder(gm);
        }
        public void SetSquareCollidable(string areaFilename, int x, int y, bool collidable)
        {
            foreach (Area ar in gm.module.ModuleAreasObjects)
            {
                if (ar.AreaFileName == areaFilename)
                {
                    //will change the square to be walkable or non-walkable based on the input of "collidable"
                    ar.TileMapList[y * ar.MapSizeInSquares.Width + x].collidable = collidable;
                }
            }
        }
        public void AddCharacterToParty(string filename)
        {
            try
            {
                string jobDir = gm.mainDirectory + "\\modules\\" + gm.module.ModuleFolderName + "\\characters\\companions";
                string moduleFolder = gm.mainDirectory + "\\modules\\" + gm.module.ModuleFolderName;
                //load the character file
                PC newPC = new PC();
                newPC.passRefs(gm, null);
                newPC = newPC.loadPCFile(jobDir + "\\" + filename);
                newPC.passRefs(gm, null);
                newPC.LoadAllPcStuff(moduleFolder);
                //check to see if already in party before adding
                bool foundOne = false;
                foreach (PC pc in gm.playerList.PCList)
                {
                    if (newPC.Tag == pc.Tag)
                    {
                        foundOne = true;
                    }
                }
                if (!foundOne)
                {
                    // when loading characters that have a blank "raceTag" or lists are empty, create lists and tags based on current objects
                    // this "if" statement may not be necessary once everyone has converted their PCs to the new format
                    if ((newPC.RaceTag == "") || (newPC.KnownSkillRefsTags.Count < 1))
                    {
                        newPC.RaceTag = newPC.Race.RaceTag;
                        newPC.ClassTag = newPC.Class.PlayerClassTag;
                        newPC.KnownSpellsTags.Clear();
                        foreach (Spell sp in newPC.KnownSpellsList.spellList)
                        {
                            newPC.KnownSpellsTags.Add(sp.SpellTag);
                        }
                        newPC.KnownTraitsTags.Clear();
                        foreach (Trait tr in newPC.KnownTraitsList.traitList)
                        {
                            newPC.KnownTraitsTags.Add(tr.TraitTag);
                        }
                        newPC.KnownSkillRefsTags.Clear();
                        foreach (Skill sk in newPC.KnownSkillsList.skillsList)
                        {
                            SkillRefs sr = new SkillRefs();
                            sr.SkillName = sk.SkillName;
                            sr.SkillTag = sk.SkillTag;
                            sr.SkillRanks = sk.Ranks;
                            newPC.KnownSkillRefsTags.Add(sr);
                        }
                    }
                    newPC.Race = gm.module.ModuleRacesList.getRaceByTag(newPC.RaceTag).DeepCopy();
                    newPC.Class = gm.module.ModulePlayerClassList.getPlayerClassByTag(newPC.ClassTag).DeepCopy();
                    newPC.KnownSpellsList.spellList.Clear();
                    foreach (string spTag in newPC.KnownSpellsTags)
                    {
                        Spell sp = gm.module.ModuleSpellsList.getSpellByTag(spTag).DeepCopy();
                        newPC.KnownSpellsList.spellList.Add(sp);
                    }
                    newPC.KnownTraitsList.traitList.Clear();
                    foreach (string trTag in newPC.KnownTraitsTags)
                    {
                        Trait tr = gm.module.ModuleTraitsList.getTraitByTag(trTag).DeepCopy();
                        newPC.KnownTraitsList.traitList.Add(tr);
                    }
                    newPC.KnownSkillsList.skillsList.Clear();
                    foreach (Skill sk in gm.module.ModuleSkillsList.skillsList)
                    {
                        newPC.KnownSkillsList.skillsList.Add(sk.DeepCopy());
                    }
                    foreach (SkillRefs sr in newPC.KnownSkillRefsTags)
                    {
                        foreach (Skill sk in newPC.KnownSkillsList.skillsList)
                        {
                            if (sk.SkillTag == sr.SkillTag)
                            {
                                sk.Ranks = sr.SkillRanks;
                            }
                        }
                    }
                    newPC.KnownSkillRefsTags.Clear();
                    foreach (Skill sk in newPC.KnownSkillsList.skillsList)
                    {
                        SkillRefs sr = new SkillRefs();
                        sr.SkillName = sk.SkillName;
                        sr.SkillTag = sk.SkillTag;
                        sr.SkillRanks = sk.Ranks;
                        newPC.KnownSkillRefsTags.Add(sr);
                    }
                    foreach (Skill sk in newPC.KnownSkillsList.skillsList)
                    {
                        sk.reCalculate(newPC);
                    }
                    try { newPC.Head = gm.module.ModuleItemsList.getItemByTag(newPC.HeadTag).DeepCopy(); }
                    catch { }
                    try { newPC.Neck = gm.module.ModuleItemsList.getItemByTag(newPC.NeckTag).DeepCopy(); }
                    catch { }
                    try { newPC.Body = gm.module.ModuleItemsList.getItemByTag(newPC.BodyTag).DeepCopy(); }
                    catch { }
                    try { newPC.MainHand = gm.module.ModuleItemsList.getItemByTag(newPC.MainHandTag).DeepCopy(); }
                    catch { }
                    try { newPC.OffHand = gm.module.ModuleItemsList.getItemByTag(newPC.OffHandTag).DeepCopy(); }
                    catch { }
                    try { newPC.Ring1 = gm.module.ModuleItemsList.getItemByTag(newPC.Ring1Tag).DeepCopy(); }
                    catch { }
                    try { newPC.Ring2 = gm.module.ModuleItemsList.getItemByTag(newPC.Ring2Tag).DeepCopy(); }
                    catch { }
                    try { newPC.Feet = gm.module.ModuleItemsList.getItemByTag(newPC.FeetTag).DeepCopy(); }
                    catch { }
                    gm.playerList.PCList.Add(newPC);


                    //gm.playerList.PCList.Add(newPC);
                    gm.addPCScriptFired = true;
                    //gm.uncheckConvo = true;

                    if (gm.playerList.PCList.Count > 5)
                    {
                        frm.pc_button_5.Enabled = true;
                        frm.pcInventory.rbtnPc5.Enabled = true;
                        frm.pc_button_5.BackgroundImage = (Image)gm.playerList.PCList[5].portraitBitmapM;
                        frm.pcSheet5.passRefs(frm, gm, 5);
                        frm.pcSheet5.refreshSheet();
                    }
                    if (gm.playerList.PCList.Count > 4)
                    {
                        frm.pc_button_4.Enabled = true;
                        frm.pcInventory.rbtnPc4.Enabled = true;
                        frm.pc_button_4.BackgroundImage = (Image)gm.playerList.PCList[4].portraitBitmapM;
                        frm.pcSheet4.passRefs(frm, gm, 4);
                        frm.pcSheet4.refreshSheet();
                    }
                    if (gm.playerList.PCList.Count > 3)
                    {
                        frm.pc_button_3.Enabled = true;
                        frm.pcInventory.rbtnPc3.Enabled = true;
                        frm.pc_button_3.BackgroundImage = (Image)gm.playerList.PCList[3].portraitBitmapM;
                        frm.pcSheet3.passRefs(frm, gm, 3);
                        frm.pcSheet3.refreshSheet();
                    }
                    if (gm.playerList.PCList.Count > 2)
                    {
                        frm.pc_button_2.Enabled = true;
                        frm.pcInventory.rbtnPc2.Enabled = true;
                        frm.pc_button_2.BackgroundImage = (Image)gm.playerList.PCList[2].portraitBitmapM;
                        frm.pcSheet2.passRefs(frm, gm, 2);
                        frm.pcSheet2.refreshSheet();
                    }
                    if (gm.playerList.PCList.Count > 1)
                    {
                        frm.pc_button_1.Enabled = true;
                        frm.pcInventory.rbtnPc1.Enabled = true;
                        frm.pc_button_1.BackgroundImage = (Image)gm.playerList.PCList[1].portraitBitmapM;
                        frm.pcSheet1.passRefs(frm, gm, 1);
                        frm.pcSheet1.refreshSheet();
                    }
                    if (gm.playerList.PCList.Count > 0)
                    {
                        frm.pc_button_0.Enabled = true;
                        frm.pcInventory.rbtnPc0.Enabled = true;
                        frm.pc_button_0.BackgroundImage = (Image)gm.playerList.PCList[0].portraitBitmapM;
                        frm.pcSheet0.passRefs(frm, gm, 0);
                        frm.pcSheet0.refreshSheet();
                    }
                    frm.refreshPartyButtons();
                    frm.doPortraitStats();
                }
                else
                {
                    IBMessageBox.Show(gm, "This PC is already in the party");
                }
            }
            catch
            {
                IBMessageBox.Show(gm, "failed to load character from character folder");
            }            
        }
        public Creature getScriptOwnerCreature()
        {
            return gm.scriptOwnerCreature;
        }
        public void EnableDisableTrigger(string tag, bool enable)
        {
            try
            {
                foreach (Area ar in gm.module.ModuleAreasObjects)
                {
                    Trigger trig = ar.AreaTriggerList.getTriggerByTag(tag);
                    if (trig != null)
                    {
                        trig.Enabled = enable;
                        return;
                    }
                }
                IBMessageBox.Show(gm, "can't find designated trigger tag in any area");
            }
            catch
            {
                IBMessageBox.Show(gm, "Error...failed trying to find trigger in any area");
            }
        }
        public void RemoveCreatureByTag(string tag) //SD_20131108
        {
            //removes a creature from any area, looking for placed creature tag
            try
            {
                foreach (Area ar in gm.module.ModuleAreasObjects)
                {
                    Creature crt = ar.AreaCreatureList.getCreatureByTag(tag);
                    if (crt != null)
                    {
                        ar.AreaCreatureList.creatures.Remove(crt);
                        return;
                    }
                }
                IBMessageBox.Show(gm, "can't find designated creature tag in any area");
            }
            catch
            {
                IBMessageBox.Show(gm, "Error...failed trying to find creature in any area");
            }
        }
        public void EnableDisableTriggerEvent(string tag, int eventNumber, bool enable)
        {
            try
            {
                foreach (Area ar in gm.module.ModuleAreasObjects)
                {
                    Trigger trig = gm.currentArea.AreaTriggerList.getTriggerByTag(tag);
                    if (trig != null)
                    {
                        if (eventNumber == 1)
                        {
                            trig.EnabledEvent1 = enable;
                        }
                        else if (eventNumber == 2)
                        {
                            trig.EnabledEvent2 = enable;
                        }
                        else if (eventNumber == 3)
                        {
                            trig.EnabledEvent3 = enable;
                        }
                        else if (eventNumber == 4)
                        {
                            trig.EnabledEvent4 = enable;
                        }
                        else if (eventNumber == 5)
                        {
                            trig.EnabledEvent5 = enable;
                        }
                        else if (eventNumber == 6)
                        {
                            trig.EnabledEvent6 = enable;
                        }
                        return;
                    }
                }
                IBMessageBox.Show(gm, "can't find designated trigger tag for event in this area");
            }
            catch
            {
                IBMessageBox.Show(gm, "Error...failed trying to find trigger tag for event in any area");
            }
        }
        public void GiveFunds(int amount)
        {
            gm.partyGold += amount;
        }
        public void GiveItem(string tag, int quantity)
        {
            Item newItem = gm.module.ModuleItemsList.getItemByTag(tag).DeepCopy();
            for (int i = 0; i < quantity; i++)
            {
                gm.partyInventoryList.Add(newItem);
                gm.partyInventoryTagList.Add(newItem.ItemTag);
            }
        }
        public void GiveXP(int amount)
        {
            if (gm.playerList.PCList.Count > 0)
            {
                int xpToGive = amount / gm.playerList.PCList.Count;
                //give xp to each PC member...split the value given
                foreach (PC givePcXp in gm.playerList.PCList)
                {
                    givePcXp.XP += xpToGive;
                }
            }
        }
        public Prop GetPropByTag(string tag)
        {
            Prop prp = gm.currentArea.AreaPropList.getPropByTag(tag);
            if (prp != null)
            {
                return prp;
            }
            else
            {
                prp = gm.currentEncounter.EncounterPropList.getPropByTag(tag);
                return prp;
            }
        }
        public PropRefs GetPropRefsByTag(string tag)
        {
            foreach (PropRefs prp in gm.currentArea.AreaPropRefsList)
            {
                if (prp.PropTag == tag) return prp;
            }
            foreach (PropRefs prp in gm.currentEncounter.EncounterPropRefsList)
            {
                if (prp.PropTag == tag) return prp;
            }
            return null;
        }
        public void DrawFloatyTextOverThisScriptOwnerCreature(string text, int lengthOfTime)
        {
            int centerX = (gm.scriptOwnerCreature.MapLocation.X * gm._squareSize) - 150 + (gm._squareSize / 2);
            int centerY = (gm.scriptOwnerCreature.MapLocation.Y * gm._squareSize) - 20;
            gm.DrawFloatyText(text, centerX, centerY, lengthOfTime, Color.White, Color.Black);
        }
        public void DrawFloatyTextOverThisScriptOwnerCreature(string text, int lengthOfTime, Color textColor, Color shadowColor)
        {
            int centerX = (gm.scriptOwnerCreature.MapLocation.X * gm._squareSize) - 150 + (gm._squareSize / 2);
            int centerY = (gm.scriptOwnerCreature.MapLocation.Y * gm._squareSize) - 20;
            gm.DrawFloatyText(text, centerX, centerY, lengthOfTime, textColor, shadowColor);
        }
        public void DrawFloatyTextOverSquare(string text, int squareX, int squareY, int lengthOfTime, Color textColor, Color shadowColor)
        {
            int centerX = (squareX * gm._squareSize) - 150 + (gm._squareSize / 2);
            int centerY = (squareY * gm._squareSize) - 20;
            gm.DrawFloatyText(text, centerX, centerY, lengthOfTime, textColor, shadowColor);
        }
        public void DrawFloatyTextOverSquare(string text, int squareX, int squareY, int lengthOfTime, int floatSpeed, int startHeight, Color textColor, Color shadowColor)
        {
            int centerX = (squareX * gm._squareSize) - 150 + (gm._squareSize / 2);
            int centerY = (squareY * gm._squareSize) - startHeight;
            gm.DrawFloatyText(text, centerX, centerY, lengthOfTime, floatSpeed, textColor, shadowColor);
        }
        public void DrawCombatFloatyTextOverSquare(string text, int squareX, int squareY, int lengthOfTime, Color textColor, Color shadowColor)
        {
            int centerX = (squareX * gm._squareSize) - 150 + (gm._squareSize / 2);
            int centerY = (squareY * gm._squareSize);
            gm.DrawCombatFloatyText(text, centerX, centerY, lengthOfTime, textColor, shadowColor);
        }
        public void DrawCombatFloatyTextOverSquare(string text, int squareX, int squareY, int lengthOfTime, int floatSpeed, int startHeight, Color textColor, Color shadowColor)
        {
            int centerX = (squareX * gm._squareSize) - 150 + (gm._squareSize / 2);
            int centerY = (squareY * gm._squareSize) - startHeight;
            gm.DrawCombatFloatyText(text, centerX, centerY, lengthOfTime, floatSpeed, textColor, shadowColor);
        }
        public void DrawCombatFloatyTextOverThisScriptOwnerCreature(string text, int lengthOfTime, Color textColor, Color shadowColor)
        {
            int centerX = (gm.scriptOwnerCreature.CombatLocation.X * gm._squareSize) - 150 + (gm._squareSize / 2);
            int centerY = (gm.scriptOwnerCreature.CombatLocation.Y * gm._squareSize);
            gm.DrawCombatFloatyText(text, centerX, centerY, lengthOfTime, textColor, shadowColor);
        }
        public void DrawCombatFloatyTextOverCreatureCombatTarget(string text, int lengthOfTime, Color textColor, Color shadowColor)
        {
            Creature crt = (Creature)CombatTarget;
            int centerX = (crt.CombatLocation.X * gm._squareSize) - 150 + (gm._squareSize / 2);
            int centerY = (crt.CombatLocation.Y * gm._squareSize);
            gm.DrawCombatFloatyText(text, centerX, centerY, lengthOfTime, textColor, shadowColor);
        }
        public void DrawCombatFloatyTextOverPcCombatTarget(string text, int lengthOfTime, Color textColor, Color shadowColor)
        {
            PC pc = (PC)CombatTarget;
            int centerX = (pc.CombatLocation.X * gm._squareSize) - 150 + (gm._squareSize / 2);
            int centerY = (pc.CombatLocation.Y * gm._squareSize);
            gm.DrawCombatFloatyText(text, centerX, centerY, lengthOfTime, textColor, shadowColor);
        }
        public void DrawCombatFloatyTextOverCreatureCombatSource(string text, int lengthOfTime, Color textColor, Color shadowColor)
        {
            Creature crt = (Creature)CombatSource;
            int centerX = (crt.CombatLocation.X * gm._squareSize) - 150 + (gm._squareSize / 2);
            int centerY = (crt.CombatLocation.Y * gm._squareSize);
            gm.DrawCombatFloatyText(text, centerX, centerY, lengthOfTime, textColor, shadowColor);
        }
        public void DrawCombatFloatyTextOverPcCombatSource(string text, int lengthOfTime, Color textColor, Color shadowColor)
        {
            PC pc = (PC)CombatSource;
            int centerX = (pc.CombatLocation.X * gm._squareSize) - 150 + (gm._squareSize / 2);
            int centerY = (pc.CombatLocation.Y * gm._squareSize);
            gm.DrawCombatFloatyText(text, centerX, centerY, lengthOfTime, textColor, shadowColor);
        }
        public void AddButtonToBottomPanel(string buttonTag, int LocationInPanelX, int LocationInPanelY, int Width, int Height)
        {
            System.Windows.Forms.Button button;
            button = new System.Windows.Forms.Button();
            button.Location = new System.Drawing.Point(LocationInPanelX, LocationInPanelY);
            button.Name = buttonTag;
            button.Tag = buttonTag;
            button.Size = new System.Drawing.Size(Width, Height);
            button.Visible = true;
            button.UseVisualStyleBackColor = true;
            button.Click += new System.EventHandler(frm.buttonBottomPanel_Click);
            frm.panelBottom.Controls.Add(button);            
        }
        public System.Windows.Forms.Button GetBottomPanelButtonByTag(string buttonTag)
        {
            System.Windows.Forms.Button returnBtn = null;
            foreach (Control btn in frm.panelBottom.Controls)
            {
                if (btn is System.Windows.Forms.Button)
                {
                    if ((string)btn.Tag == buttonTag)
                    {
                        return (System.Windows.Forms.Button)btn;
                    }
                }
            }
            return returnBtn;
        }
        public void AddButtonToLeftPanel(string buttonTag, int LocationInPanelX, int LocationInPanelY, int Width, int Height)
        {
            System.Windows.Forms.Button button;
            button = new System.Windows.Forms.Button();
            button.Location = new System.Drawing.Point(LocationInPanelX, LocationInPanelY);
            button.Name = buttonTag;
            button.Tag = buttonTag;
            button.Size = new System.Drawing.Size(Width, Height);
            button.Visible = true;
            button.UseVisualStyleBackColor = true;
            button.Click += new System.EventHandler(frm.buttonLeftPanel_Click);
            frm.panelLeft.Controls.Add(button);
        }
        public System.Windows.Forms.Button GetLeftPanelButtonByTag(string buttonTag)
        {
            System.Windows.Forms.Button returnBtn = null;
            foreach (Control btn in frm.panelLeft.Controls)
            {
                if (btn is System.Windows.Forms.Button)
                {
                    if ((string)btn.Tag == buttonTag)
                    {
                        return (System.Windows.Forms.Button)btn;
                    }
                }
            }
            return returnBtn;
        }

        #region Global and Local Variables
        //ints
        public int GetGlobalInt(string variableName)
        {
            foreach (GlobalInt variable in gm.module.ModuleGlobalInts)
            {
                if (variable.Key == variableName)
                {
                   return variable.Value;
                }
            }
            if (frm.debugMode) //SD_20131102
            {
                IBMessageBox.Show(gm, "Couldn't find the tag specified...returning a value of -1");
            }
            return -1;
        }
        public int GetLocalInt(string objectTag, string variableName)
        {
            //check creatures, PCs, Props, areas, items
            #region Search PCs
            foreach (PC pc in gm.playerList.PCList)
            {
                if (pc.Tag == objectTag)
                {
                    foreach (LocalInt variable in pc.CharLocalInts)
                    {
                        if (variable.Key == variableName)
                        {
                            return variable.Value;
                        }
                    }
                    // * sinopip, 16.08.14
                    if (frm.debugMode)
                    IBMessageBox.Show(gm, "Found the object, but couldn't find the tag specified...returning a value of -1");
                    return -1;
                }
            }
            #endregion
            foreach (Area a in gm.module.ModuleAreasObjects)
            {
                #region Area itself
                if (a.AreaFileName == objectTag)
                {
                    foreach (LocalInt variable in a.AreaLocalInts)
                    {
                        if (variable.Key == variableName)
                        {
                            return variable.Value;
                        }
                    }
                    // * sinopip, 16.08.14
                    if (frm.debugMode)
                    IBMessageBox.Show(gm, "Found the object, but couldn't find the tag specified...returning a value of -1");
                    return -1;
                }
                #endregion
                #region Creatures
                foreach (Creature cr in a.AreaCreatureList.creatures)
                {
                    if (cr.Tag == objectTag)
                    {
                        foreach (LocalInt variable in cr.CharLocalInts)
                        {
                            if (variable.Key == variableName)
                            {
                                return variable.Value;
                            }
                        }
                        // * sinopip, 16.08.14
                    	if (frm.debugMode)
                    	IBMessageBox.Show(gm, "Found the object, but couldn't find the tag specified...returning a value of -1");
                        return -1;
                    }
                }
                #endregion
                #region Props
                foreach (Prop pr in a.AreaPropList.propsList)
                {
                    if (pr.PropTag == objectTag)
                    {
                        foreach (LocalInt variable in pr.PropLocalInts)
                        {
                            if (variable.Key == variableName)
                            {
                                return variable.Value;
                            }
                        }
                        // * sinopip, 16.08.14
                   		if (frm.debugMode)
                    	IBMessageBox.Show(gm, "Found the object, but couldn't find the tag specified...returning a value of -1");
                        return -1;
                    }
                }
                #endregion
            }
            foreach (Encounter e in gm.module.ModuleEncountersList.encounters)
            {
                #region Creatures
                foreach (Creature cr in e.EncounterCreatureList.creatures)
                {
                    if (cr.Tag == objectTag)
                    {
                        foreach (LocalInt variable in cr.CharLocalInts)
                        {
                            if (variable.Key == variableName)
                            {
                                return variable.Value;
                            }
                        }
                        // * sinopip, 16.08.14
                    	if (frm.debugMode)
                    	IBMessageBox.Show(gm, "Found the object, but couldn't find the tag specified...returning a value of -1");
                        return -1;
                    }
                }
                #endregion
                #region Props
                foreach (Prop pr in e.EncounterPropList.propsList)
                {
                    if (pr.PropTag == objectTag)
                    {
                        foreach (LocalInt variable in pr.PropLocalInts)
                        {
                            if (variable.Key == variableName)
                            {
                                return variable.Value;
                            }
                        }
                        // * sinopip, 16.08.14
                    	if (frm.debugMode)
                    	IBMessageBox.Show(gm, "Found the object, but couldn't find the tag specified...returning a value of -1");
                        return -1;
                    }
                }
                #endregion
            }
            if (frm.debugMode) //SD_20131102
            {
                IBMessageBox.Show(gm, "couldn't find the object with the tag specified (only PCs, Creatures, Areas and Props), returning a value of -1");
            }
            return -1;
        }
        public void SetGlobalInt(string variableName, int value)
        {
            int exists = 0;
            foreach (GlobalInt variable in gm.module.ModuleGlobalInts)
            {
                if (variable.Key == variableName)
                {
                    variable.Value = value;
                    exists = 1;
                    if (frm.debugMode) //SD_20131102
                    {
                        frm.logText("setGlobal: " + variableName + " = " + value.ToString(), Color.GreenYellow);
                        frm.logText(Environment.NewLine, Color.GreenYellow);
                    }
                }
            }
            if (exists == 0)
            {
                GlobalInt newGlobal = new GlobalInt();
                newGlobal.Key = variableName;
                newGlobal.Value = value;
                gm.module.ModuleGlobalInts.Add(newGlobal);
                if (frm.debugMode) //SD_20131102
                {
                    frm.logText("setGlobal: " + variableName + " = " + value.ToString(), Color.GreenYellow);
                    frm.logText(Environment.NewLine, Color.GreenYellow);
                }
            }
        }
        public void SetLocalInt(string objectTag, string variableName, int value)
        {
            //check creatures, PCs, Props, areas, items
            #region Search PCs
            foreach (PC pc in gm.playerList.PCList)
            {
                if (pc.Tag == objectTag)
                {
                    int exists = 0;
                    foreach (LocalInt variable in pc.CharLocalInts)
                    {
                        if (variable.Key == variableName)
                        {
                            variable.Value = value;
                            exists = 1;
                        }
                    }
                    if (exists == 0)
                    {
                        LocalInt newLocalInt = new LocalInt();
                        newLocalInt.Key = variableName;
                        newLocalInt.Value = value;
                        pc.CharLocalInts.Add(newLocalInt);
                    }
                    return;
                }
            }
            #endregion
            foreach (Area a in gm.module.ModuleAreasObjects)
            {
                #region Area itself
                if (a.AreaFileName == objectTag)
                {
                    int exists = 0;
                    foreach (LocalInt variable in a.AreaLocalInts)
                    {
                        if (variable.Key == variableName)
                        {
                            variable.Value = value;
                            exists = 1;
                        }
                    }
                    if (exists == 0)
                    {
                        LocalInt newLocalInt = new LocalInt();
                        newLocalInt.Key = variableName;
                        newLocalInt.Value = value;
                        a.AreaLocalInts.Add(newLocalInt);
                    }
                    return;
                }
                #endregion
                #region Creatures
                foreach (Creature cr in a.AreaCreatureList.creatures)
                {
                    if (cr.Tag == objectTag)
                    {
                        int exists = 0;
                        foreach (LocalInt variable in cr.CharLocalInts)
                        {
                            if (variable.Key == variableName)
                            {
                                variable.Value = value;
                                exists = 1;
                            }
                        }
                        if (exists == 0)
                        {
                            LocalInt newLocalInt = new LocalInt();
                            newLocalInt.Key = variableName;
                            newLocalInt.Value = value;
                            cr.CharLocalInts.Add(newLocalInt);
                        }
                        return;
                    }
                }
                #endregion
                #region Props
                foreach (Prop pr in a.AreaPropList.propsList)
                {
                    if (pr.PropTag == objectTag)
                    {
                        int exists = 0;
                        foreach (LocalInt variable in pr.PropLocalInts)
                        {
                            if (variable.Key == variableName)
                            {
                                variable.Value = value;
                                exists = 1;
                            }
                        }
                        if (exists == 0)
                        {
                            LocalInt newLocalInt = new LocalInt();
                            newLocalInt.Key = variableName;
                            newLocalInt.Value = value;
                            pr.PropLocalInts.Add(newLocalInt);
                        }
                        return;
                    }
                }
                #endregion
            }
            foreach (Encounter e in gm.module.ModuleEncountersList.encounters)
            {
                #region Creatures
                foreach (Creature cr in e.EncounterCreatureList.creatures)
                {
                    if (cr.Tag == objectTag)
                    {
                        int exists = 0;
                        foreach (LocalInt variable in cr.CharLocalInts)
                        {
                            if (variable.Key == variableName)
                            {
                                variable.Value = value;
                                exists = 1;
                            }
                        }
                        if (exists == 0)
                        {
                            LocalInt newLocalInt = new LocalInt();
                            newLocalInt.Key = variableName;
                            newLocalInt.Value = value;
                            cr.CharLocalInts.Add(newLocalInt);
                        }
                        return;
                    }
                }
                #endregion
                #region Props
                foreach (Prop pr in e.EncounterPropList.propsList)
                {
                    if (pr.PropTag == objectTag)
                    {
                        int exists = 0;
                        foreach (LocalInt variable in pr.PropLocalInts)
                        {
                            if (variable.Key == variableName)
                            {
                                variable.Value = value;
                                exists = 1;
                            }
                        }
                        if (exists == 0)
                        {
                            LocalInt newLocalInt = new LocalInt();
                            newLocalInt.Key = variableName;
                            newLocalInt.Value = value;
                            pr.PropLocalInts.Add(newLocalInt);
                        }
                        return;
                    }
                }
                #endregion
            }
            if (frm.debugMode) //SD_20131102
            {
                IBMessageBox.Show(gm, "couldn't find the object with the tag (tag: " + objectTag + ") specified (only PCs, Creatures, Areas and Props)");
            }
        }
        public bool CheckGlobalInt(string variableName, string compare, int value)
        {
            if (frm.debugMode) //SD_20131102
            {
                frm.logText("checkGlobal: " + variableName + " " + compare + " " + value.ToString(), Color.Yellow);
                frm.logText(Environment.NewLine, Color.GreenYellow);
            }
            foreach (GlobalInt variable in gm.module.ModuleGlobalInts)
            {                
                if (variable.Key == variableName)
                {
                    if (compare == "=")
                    {
                        if (variable.Value == value)
                        {
                            if (frm.debugMode) //SD_20131102
                            {
                                frm.logText("foundGlobal: " + variable.Key + " == " + variable.Value.ToString(), Color.Yellow);
                                frm.logText(Environment.NewLine, Color.GreenYellow);
                            }
                            return true;
                        }
                    }
                    else if (compare == ">")
                    {
                        if (variable.Value > value)
                        {
                            if (frm.debugMode) //SD_20131102
                            {
                                frm.logText("foundGlobal: " + variable.Key + " > " + variable.Value.ToString(), Color.Yellow);
                                frm.logText(Environment.NewLine, Color.GreenYellow);
                            }
                            return true;
                        }
                    }
                    else if (compare == "<")
                    {
                        if (variable.Value < value)
                        {
                            if (frm.debugMode) //SD_20131102
                            {
                                frm.logText("foundGlobal: " + variable.Key + " < " + variable.Value.ToString(), Color.Yellow);
                                frm.logText(Environment.NewLine, Color.GreenYellow);
                            }
                            return true;
                        }
                    }
                    else if (compare == "!")
                    {
                        if (variable.Value != value)
                        {
                            if (frm.debugMode) //SD_20131102
                            {
                                frm.logText("foundGlobal: " + variable.Key + " != " + variable.Value.ToString(), Color.Yellow);
                                frm.logText(Environment.NewLine, Color.GreenYellow);
                            }
                            return true;
                        }
                    }
                }
            }
            if (frm.debugMode) //SD_20131102
            {
                frm.logText("found nothing", Color.Yellow);
                frm.logText(Environment.NewLine, Color.GreenYellow);
            }
            return false;
        }
        public bool CheckLocalInt(string objectTag, string variableName, string compare, int value)
        {
            //check creatures, PCs, Props, areas, items
            #region Search PCs
            foreach (PC pc in gm.playerList.PCList)
            {
                if (pc.Tag == objectTag)
                {
                    foreach (LocalInt variable in pc.CharLocalInts)
                    {
                        if (variable.Key == variableName)
                        {
                            if (compare == "=")
                            {
                                if (variable.Value == value)
                                {
                                    return true;
                                }
                            }
                            else if (compare == ">")
                            {
                                if (variable.Value > value)
                                {
                                    return true;
                                }
                            }
                            else if (compare == "<")
                            {
                                if (variable.Value < value)
                                {
                                    return true;
                                }
                            }
                            else if (compare == "!")
                            {
                                if (variable.Value != value)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    return false;
                }
            }
            #endregion
            foreach (Area a in gm.module.ModuleAreasObjects)
            {
                #region Area itself
                if (a.AreaFileName == objectTag)
                {
                    foreach (LocalInt variable in a.AreaLocalInts)
                    {
                        if (variable.Key == variableName)
                        {
                            if (compare == "=")
                            {
                                if (variable.Value == value)
                                {
                                    return true;
                                }
                            }
                            else if (compare == ">")
                            {
                                if (variable.Value > value)
                                {
                                    return true;
                                }
                            }
                            else if (compare == "<")
                            {
                                if (variable.Value < value)
                                {
                                    return true;
                                }
                            }
                            else if (compare == "!")
                            {
                                if (variable.Value != value)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    return false;
                }
                #endregion
                #region Creatures
                foreach (Creature cr in a.AreaCreatureList.creatures)
                {
                    if (cr.Tag == objectTag)
                    {
                        foreach (LocalInt variable in cr.CharLocalInts)
                        {
                            if (variable.Key == variableName)
                            {
                                if (compare == "=")
                                {
                                    if (variable.Value == value)
                                    {
                                        return true;
                                    }
                                }
                                else if (compare == ">")
                                {
                                    if (variable.Value > value)
                                    {
                                        return true;
                                    }
                                }
                                else if (compare == "<")
                                {
                                    if (variable.Value < value)
                                    {
                                        return true;
                                    }
                                }
                                else if (compare == "!")
                                {
                                    if (variable.Value != value)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        return false;
                    }
                }
                #endregion
                #region Props
                foreach (Prop pr in a.AreaPropList.propsList)
                {
                    if (pr.PropTag == objectTag)
                    {
                        foreach (LocalInt variable in pr.PropLocalInts)
                        {
                            if (variable.Key == variableName)
                            {
                                if (compare == "=")
                                {
                                    if (variable.Value == value)
                                    {
                                        return true;
                                    }
                                }
                                else if (compare == ">")
                                {
                                    if (variable.Value > value)
                                    {
                                        return true;
                                    }
                                }
                                else if (compare == "<")
                                {
                                    if (variable.Value < value)
                                    {
                                        return true;
                                    }
                                }
                                else if (compare == "!")
                                {
                                    if (variable.Value != value)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        return false;
                    }
                }
                #endregion
            }
            foreach (Encounter e in gm.module.ModuleEncountersList.encounters)
            {
                #region Creatures
                foreach (Creature cr in e.EncounterCreatureList.creatures)
                {
                    if (cr.Tag == objectTag)
                    {
                        foreach (LocalInt variable in cr.CharLocalInts)
                        {
                            if (variable.Key == variableName)
                            {
                                if (compare == "=")
                                {
                                    if (variable.Value == value)
                                    {
                                        return true;
                                    }
                                }
                                else if (compare == ">")
                                {
                                    if (variable.Value > value)
                                    {
                                        return true;
                                    }
                                }
                                else if (compare == "<")
                                {
                                    if (variable.Value < value)
                                    {
                                        return true;
                                    }
                                }
                                else if (compare == "!")
                                {
                                    if (variable.Value != value)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        return false;
                    }
                }
                #endregion
                #region Props
                foreach (Prop pr in e.EncounterPropList.propsList)
                {
                    if (pr.PropTag == objectTag)
                    {
                        foreach (LocalInt variable in pr.PropLocalInts)
                        {
                            if (variable.Key == variableName)
                            {
                                if (compare == "=")
                                {
                                    if (variable.Value == value)
                                    {
                                        return true;
                                    }
                                }
                                else if (compare == ">")
                                {
                                    if (variable.Value > value)
                                    {
                                        return true;
                                    }
                                }
                                else if (compare == "<")
                                {
                                    if (variable.Value < value)
                                    {
                                        return true;
                                    }
                                }
                                else if (compare == "!")
                                {
                                    if (variable.Value != value)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        return false;
                    }
                }
                #endregion
            }
            if (frm.debugMode) //SD_20131102
            {
                IBMessageBox.Show(gm, "couldn't find the object with the tag (tag: " + objectTag + ") specified (only PCs, Creatures, Areas and Props)");
            }
            return false;
        }
        //objects
        public object GetGlobalObject(string variableName)
        {
            foreach (GlobalObject variable in gm.module.ModuleGlobalObjects)
            {
                if (variable.Key == variableName)
                {
                    return variable.Object;
                }
            }
            if (frm.debugMode) //SD_20131102
            {
                IBMessageBox.Show(gm, "Couldn't find the tag specified...returning a value of -1");
            }
            return -1;
        }
        public object GetLocalObject(string objectTag, string variableName)
        {
            //check creatures, PCs, Props, areas, items
            #region Search PCs
            foreach (PC pc in gm.playerList.PCList)
            {
                if (pc.Tag == objectTag)
                {
                    foreach (LocalObject variable in pc.CharLocalObjects)
                    {
                        if (variable.Key == variableName)
                        {
                            return variable.Object;
                        }
                    }
                    IBMessageBox.Show(gm, "Found the object, but couldn't find the tag specified...returning a value of -1");
                    return -1;
                }
            }
            #endregion
            foreach (Area a in gm.module.ModuleAreasObjects)
            {
                #region Area itself
                if (a.AreaFileName == objectTag)
                {
                    foreach (LocalObject variable in a.AreaLocalObjects)
                    {
                        if (variable.Key == variableName)
                        {
                            return variable.Object;
                        }
                    }
                    IBMessageBox.Show(gm, "Found the object, but couldn't find the tag specified...returning a value of -1");
                    return -1;
                }
                #endregion
                #region Creatures
                foreach (Creature cr in a.AreaCreatureList.creatures)
                {
                    if (cr.Tag == objectTag)
                    {
                        foreach (LocalObject variable in cr.CharLocalObjects)
                        {
                            if (variable.Key == variableName)
                            {
                                return variable.Object;
                            }
                        }
                        IBMessageBox.Show(gm, "Found the object, but couldn't find the tag specified...returning a value of -1");
                        return -1;
                    }
                }
                #endregion
                #region Props
                foreach (Prop pr in a.AreaPropList.propsList)
                {
                    if (pr.PropTag == objectTag)
                    {
                        foreach (LocalObject variable in pr.PropLocalObjects)
                        {
                            if (variable.Key == variableName)
                            {
                                return variable.Object;
                            }
                        }
                        IBMessageBox.Show(gm, "Found the object, but couldn't find the tag specified...returning a value of -1");
                        return -1;
                    }
                }
                #endregion
            }
            foreach (Encounter e in gm.module.ModuleEncountersList.encounters)
            {
                #region Creatures
                foreach (Creature cr in e.EncounterCreatureList.creatures)
                {
                    if (cr.Tag == objectTag)
                    {
                        foreach (LocalObject variable in cr.CharLocalObjects)
                        {
                            if (variable.Key == variableName)
                            {
                                return variable.Object;
                            }
                        }
                        IBMessageBox.Show(gm, "Found the object, but couldn't find the tag specified...returning a value of -1");
                        return -1;
                    }
                }
                #endregion
                #region Props
                foreach (Prop pr in e.EncounterPropList.propsList)
                {
                    if (pr.PropTag == objectTag)
                    {
                        foreach (LocalObject variable in pr.PropLocalObjects)
                        {
                            if (variable.Key == variableName)
                            {
                                return variable.Object;
                            }
                        }
                        IBMessageBox.Show(gm, "Found the object, but couldn't find the tag specified...returning a value of -1");
                        return -1;
                    }
                }
                #endregion
            }
            if (frm.debugMode) //SD_20131102
            {
                IBMessageBox.Show(gm, "couldn't find the object with the tag specified (only PCs, Creatures, Areas and Props), returning a value of -1");
            }
            return -1;
        }
        public void SetGlobalObject(string variableName, object value)
        {
            int exists = 0;
            foreach (GlobalObject variable in gm.module.ModuleGlobalObjects)
            {
                if (variable.Key == variableName)
                {
                    variable.Object = value;
                    exists = 1;
                }
            }
            if (exists == 0)
            {
                GlobalObject newGlobal = new GlobalObject();
                newGlobal.Key = variableName;
                newGlobal.Object = value;
                gm.module.ModuleGlobalObjects.Add(newGlobal);
            }
        }
        public void SetLocalObject(string objectTag, string variableName, object value)
        {
            //check creatures, PCs, Props, areas, items
            #region Search PCs
            foreach (PC pc in gm.playerList.PCList)
            {
                if (pc.Tag == objectTag)
                {
                    int exists = 0;
                    foreach (LocalObject variable in pc.CharLocalObjects)
                    {
                        if (variable.Key == variableName)
                        {
                            variable.Object = value;
                            exists = 1;
                        }
                    }
                    if (exists == 0)
                    {
                        LocalObject newLocalObject = new LocalObject();
                        newLocalObject.Key = variableName;
                        newLocalObject.Object = value;
                        pc.CharLocalObjects.Add(newLocalObject);
                    }
                    return;
                }
            }
            #endregion
            foreach (Area a in gm.module.ModuleAreasObjects)
            {
                #region Area itself
                if (a.AreaFileName == objectTag)
                {
                    int exists = 0;
                    foreach (LocalObject variable in a.AreaLocalObjects)
                    {
                        if (variable.Key == variableName)
                        {
                            variable.Object = value;
                            exists = 1;
                        }
                    }
                    if (exists == 0)
                    {
                        LocalObject newLocalObject = new LocalObject();
                        newLocalObject.Key = variableName;
                        newLocalObject.Object = value;
                        a.AreaLocalObjects.Add(newLocalObject);
                    }
                    return;
                }
                #endregion
                #region Creatures
                foreach (Creature cr in a.AreaCreatureList.creatures)
                {
                    if (cr.Tag == objectTag)
                    {
                        int exists = 0;
                        foreach (LocalObject variable in cr.CharLocalObjects)
                        {
                            if (variable.Key == variableName)
                            {
                                variable.Object = value;
                                exists = 1;
                            }
                        }
                        if (exists == 0)
                        {
                            LocalObject newLocalObject = new LocalObject();
                            newLocalObject.Key = variableName;
                            newLocalObject.Object = value;
                            cr.CharLocalObjects.Add(newLocalObject);
                        }
                        return;
                    }
                }
                #endregion
                #region Props
                foreach (Prop pr in a.AreaPropList.propsList)
                {
                    if (pr.PropTag == objectTag)
                    {
                        int exists = 0;
                        foreach (LocalObject variable in pr.PropLocalObjects)
                        {
                            if (variable.Key == variableName)
                            {
                                variable.Object = value;
                                exists = 1;
                            }
                        }
                        if (exists == 0)
                        {
                            LocalObject newLocalObject = new LocalObject();
                            newLocalObject.Key = variableName;
                            newLocalObject.Object = value;
                            pr.PropLocalObjects.Add(newLocalObject);
                        }
                        return;
                    }
                }
                #endregion
            }
            foreach (Encounter e in gm.module.ModuleEncountersList.encounters)
            {
                #region Creatures
                foreach (Creature cr in e.EncounterCreatureList.creatures)
                {
                    if (cr.Tag == objectTag)
                    {
                        int exists = 0;
                        foreach (LocalObject variable in cr.CharLocalObjects)
                        {
                            if (variable.Key == variableName)
                            {
                                variable.Object = value;
                                exists = 1;
                            }
                        }
                        if (exists == 0)
                        {
                            LocalObject newLocalObject = new LocalObject();
                            newLocalObject.Key = variableName;
                            newLocalObject.Object = value;
                            cr.CharLocalObjects.Add(newLocalObject);
                        }
                        return;
                    }
                }
                #endregion
                #region Props
                foreach (Prop pr in e.EncounterPropList.propsList)
                {
                    if (pr.PropTag == objectTag)
                    {
                        int exists = 0;
                        foreach (LocalObject variable in pr.PropLocalObjects)
                        {
                            if (variable.Key == variableName)
                            {
                                variable.Object = value;
                                exists = 1;
                            }
                        }
                        if (exists == 0)
                        {
                            LocalObject newLocalObject = new LocalObject();
                            newLocalObject.Key = variableName;
                            newLocalObject.Object = value;
                            pr.PropLocalObjects.Add(newLocalObject);
                        }
                        return;
                    }
                }
                #endregion
            }
            if (frm.debugMode) //SD_20131102
            {
                IBMessageBox.Show(gm, "couldn't find the object with the tag specified (only PCs, Creatures, Areas and Props)");
            }
        }
        #endregion

        #region Combat Stuff
        //
     	// * sinopip, 16.08.14
     	//
        public void DoCreatureTurn(Creature crt)
        {
            //Creature crt_pt = new Creature();
            //crt_pt.passRefs(gm, null);
            //crt_pt = com_encounter.EncounterCreatureList.creatures[com_moveOrderList[currentMoveOrderIndex].index];

            if ((crt.HP > 0) && (crt.Status != PC.charStatus.Held))
            {
            	PC char_pt = new PC();
            	char_pt.passRefs(gm, null);
                // * sinopip, 16.08.14
            	Creature creature_pt = new Creature();
                creature_pt.passRefs(gm, null);
                //
                CombatTarget = null;
                CombatSource = crt;
                ActionToTake = null;
                SpellToCast = null;
                TraitToUse = null;

                //determine the action to take
                #region onStartCombatTurn                
                var scriptCrt = crt.OnStartCombatTurn;
                frm.doScriptBasedOnFilename(scriptCrt.FilenameOrTag, scriptCrt.Parm1, scriptCrt.Parm2, scriptCrt.Parm3, scriptCrt.Parm4);
                #endregion

                //do the action (melee/ranged, cast spell, use trait, etc.)
                if ((ActionToTake == null) || ((ChosenAction)ActionToTake == ChosenAction.MeleeRangedAttack))
                {
                    #region Do a Melee or Ranged Attack
                    if (CombatTarget != null)
                    {
                        //use the target from sf.CombatTarget
                        if ((CombatTarget is int) && ((int)CombatTarget >= 0))
                        {
                            //the target is a PC using the PCindex
                            int PCindex = (int)CombatTarget;
                            char_pt = gm.playerList.PCList[PCindex];
                            CombatTarget = char_pt;
                        }
                        else if (CombatTarget is Creature)
                        {
                        	// * sinopip, 16.08.14
                        	creature_pt = (Creature)CombatTarget;
                        	CombatTarget = creature_pt;
							//
                            //target is a creature
                        }
                        else if (CombatTarget is Point)
                        {
                            //target is a location on the combat map
                        }
                        else
                        {
                            //IBMessageBox.Show(com_game, "failed to find a target from script, using nearest PC instead");
                            gm.errorLog("creature failed to find a target from script, using nearest PC instead");
                            // use default closest PC target
                            int whichOne = FindNearestPc();
                            char_pt = gm.playerList.PCList[whichOne];
                            CombatTarget = char_pt;
                        }
                    }
                    else
                    {
                        // use default closest PC target
                        int whichOne = FindNearestPc();
                        char_pt = gm.playerList.PCList[whichOne];
                        CombatTarget = char_pt;
                    }
                    // * sinopip, 16.08.14
                    if (CombatTarget is PC)
                    	CreatureDoesAttack(crt, char_pt);
                    else if (CombatTarget is Creature)
                    	CreatureDoesAttack(crt, creature_pt);
                    //
                    #endregion
                }
                else if ((ChosenAction)ActionToTake == ChosenAction.CastSpell)
                {
                    if ((SpellToCast != null) && (CombatTarget != null))
                    {
                        //IBMessageBox.Show(gm, "cast spell: " + SpellToCast.SpellName + " target: " + CombatTarget.ToString());
                        CreatureCastsSpell(crt);
                    }
                }
                else if ((ChosenAction)ActionToTake == ChosenAction.UseTrait)
                {
                    if ((TraitToUse != null) && (CombatTarget != null))
                    {
                        //IBMessageBox.Show(gm, "use trait: " + TraitToUse.TraitName + " target: " + CombatTarget.ToString());
                        CreatureUsesTrait(crt);
                    }
                }
                ActionToTake = null;
                SpellToCast = null;
                TraitToUse = null;

                #region onEndCombatTurn
                // run OnStartCombatTurn script
                var scriptEndCrt = crt.OnEndCombatTurn;
                frm.doScriptBasedOnFilename(scriptEndCrt.FilenameOrTag, scriptCrt.Parm1, scriptCrt.Parm2, scriptCrt.Parm3, scriptCrt.Parm4);
                #endregion
            }
            else
            {
                //logText(crt_pt.Name, Color.LightGray);
                //logText(" is unconscious or held...skips turn", Color.Black);
                //logText(Environment.NewLine, Color.Black);
                //logText(Environment.NewLine, Color.Black);
            }
        }
        public void CreatureDoesAttack(Creature crt_pt, PC char_pt)
        {
            
            char_pt = (PC)CombatTarget;
            // determine if ranged or melee
            if ((crt_pt.WeaponType == Creature.crCategory.Ranged) && (CalcDistance(char_pt.CombatLocation, crt_pt.CombatLocation) <= crt_pt.AttackRange) && (frm.currentCombat.IsVisibleLineOfSight(crt_pt.CombatLocation, char_pt.CombatLocation)))
            {
                Point starting = new Point((crt_pt.CombatLocation.X * gm._squareSize) + (gm._squareSize / 2), (crt_pt.CombatLocation.Y * gm._squareSize) + (gm._squareSize / 2));
                Point ending = new Point((char_pt.CombatLocation.X * gm._squareSize) + (gm._squareSize / 2), (char_pt.CombatLocation.Y * gm._squareSize) + (gm._squareSize / 2));
                frm.currentCombat.playCreatureAttackSound(crt_pt);
                frm.currentCombat.drawProjectile(starting, ending, crt_pt.ProjectileSpriteFilename);

                #region The actual attack portion
                if (crt_pt.HP > 0)
                {
                    #region OnAttack
                    // run OnStartCombatTurn script 
                    CombatTarget = char_pt;
                    CombatSource = crt_pt;
                    var scriptCrt = crt_pt.OnAttack;
                    frm.doScriptBasedOnFilename(scriptCrt.FilenameOrTag, scriptCrt.Parm1, scriptCrt.Parm2, scriptCrt.Parm3, scriptCrt.Parm4);
                    #endregion
                    #region CreatureAttack
                    // run OnStartCombatTurn script 
                    CombatTarget = char_pt;
                    CombatSource = crt_pt;
                    frm.doScriptBasedOnFilename("dsAttackCreature.cs", "none", "none", "none", "none");
                    #endregion
                }
                else
                {
                    frm.currentCombat.logText(crt_pt.Name, Color.LightGray);
                    frm.currentCombat.logText(" is unconscious...skips turn", Color.Black);
                    frm.currentCombat.logText(Environment.NewLine, Color.Black);
                    frm.currentCombat.logText(Environment.NewLine, Color.Black);
                }
                #endregion
            }
            else
            {
                setupPathfindArray(crt_pt, char_pt);
                pathfinder.Squares[crt_pt.CombatLocation.X, crt_pt.CombatLocation.Y].ContentCode = SquareContent.Monster;
                pathfinder.Squares[char_pt.CombatLocation.X, char_pt.CombatLocation.Y].ContentCode = SquareContent.Hero;
                Recalculate(crt_pt);
                setupHeroSquares(char_pt, crt_pt);
                navigatePath(crt_pt);

                // if melee, try and move to attack or get close
                // check to see if selected PC is one square away, if so attack else skip
                //if (calcDistance(char_pt.CombatLocation, crt_pt.CombatLocation) == 1)
                if (creatureWithinMeleeDistance(crt_pt, char_pt))
                {
                    doCreatureCombatFacing(crt_pt, char_pt);
                    #region The actual attack portion
                    if (crt_pt.HP > 0)
                    {
                        #region OnAttack
                        // run OnStartCombatTurn script 
                        CombatTarget = char_pt;
                        CombatSource = crt_pt;
                        var scriptCrt = crt_pt.OnAttack;
                        frm.doScriptBasedOnFilename(scriptCrt.FilenameOrTag, scriptCrt.Parm1, scriptCrt.Parm2, scriptCrt.Parm3, scriptCrt.Parm4);
                        #endregion
                        #region CreatureAttack
                        // run OnStartCombatTurn script 
                        CombatTarget = char_pt;
                        CombatSource = crt_pt;
                        frm.doScriptBasedOnFilename("dsAttackCreature.cs", "none", "none", "none", "none");
                        #endregion
                    }
                    else
                    {
                        frm.currentCombat.logText(crt_pt.Name, Color.LightGray);
                        frm.currentCombat.logText(" is unconscious...skips turn", Color.Black);
                        frm.currentCombat.logText(Environment.NewLine, Color.Black);
                        frm.currentCombat.logText(Environment.NewLine, Color.Black);
                    }
                    #endregion
                }
            }
        }
        public void CreatureCastsSpell(Creature crt)
        {
            Point pnt = new Point();
            if ((CombatTarget is int) && ((int)CombatTarget >= 0))
            {
                //the target is a PC using the PCindex
                int PCindex = (int)CombatTarget;
                PC pc = gm.playerList.PCList[PCindex];
                //CombatTarget = pc;
                pnt = (Point)pc.CombatLocation;
            }         
            else if (CombatTarget is PC)
            {
                PC pc = (PC)CombatTarget;
                pnt = (Point)pc.CombatLocation;
            }
            else if (CombatTarget is Creature)
            {
                Creature crtTarget = (Creature)CombatTarget;
                pnt = (Point)crtTarget.CombatLocation;
            }
            else if (CombatTarget is Point)
            {
                pnt = (Point)CombatTarget;
            }
            else //do not understand, what is the target
            {
                MessageBox.Show("target is not a PC, Creature or Point...creature failed to cast spell");
                return;
            }
            if ((CalcDistance(pnt, crt.CombatLocation) <= SpellToCast.Range) && (frm.currentCombat.IsVisibleLineOfSight(crt.CombatLocation, pnt)))
            {
                Point starting = new Point((crt.CombatLocation.X * gm._squareSize) + (gm._squareSize / 2), (crt.CombatLocation.Y * gm._squareSize) + (gm._squareSize / 2));
                Point ending = new Point((pnt.X * gm._squareSize) + (gm._squareSize / 2), (pnt.Y * gm._squareSize) + (gm._squareSize / 2));
                if (starting.X > ending.X) { crt.CombatFacing = CharBase.facing.Left; }
                else { crt.CombatFacing = CharBase.facing.Right; }
                frm.currentCombat.playCreatureAttackSound(crt, SpellToCast.SpellStartSound);
                frm.currentCombat.drawProjectile(starting, ending, SpellToCast.SpriteFilename);

                if (crt.CharSprite.AttackingNumberOfFrames > 1)
                {
                    frm.currentCombat.attackCreatureAnimation(crt);
                }
                frm.currentCombat.playCreatureAttackSound(crt, SpellToCast.SpellEndSound);
                frm.currentCombat.drawEndEffect(pnt, SpellToCast.AoeRadiusOrLength, SpellToCast.SpriteEndingFilename);

                CombatSource = crt;
                if ((CombatTarget is int) && ((int)CombatTarget >= 0)) //SD_20131110
                {
                    int PCindex = (int)CombatTarget;
                    PC pc = gm.playerList.PCList[PCindex];
                    CombatTarget = pc;
                }
                var script = SpellToCast.SpellScript;
                frm.doScriptBasedOnFilename(script.FilenameOrTag, script.Parm1, script.Parm2, script.Parm3, script.Parm4);

                crt.SP -= SpellToCast.CostSP;
                if (crt.SP < 0) { crt.SP = 0; }
            }
            else
            {
                //MessageBox.Show("target out of range from spell...move instead");
                #region Do a Melee or Ranged Attack
                // use default closest PC target
                int whichOne = FindNearestPc();
                PC pc = gm.playerList.PCList[whichOne];
                CombatTarget = pc;
                CreatureDoesAttack(crt, pc);
                #endregion
            }
        }
        public void CreatureUsesTrait(Creature crt)
        {
            Point pnt = new Point();
            if ((CombatTarget is int) && ((int)CombatTarget >= 0))
            {
                //the target is a PC using the PCindex
                int PCindex = (int)CombatTarget;
                PC pc = gm.playerList.PCList[PCindex];
                //CombatTarget = pc;
                pnt = (Point)pc.CombatLocation;
            }
            else if (CombatTarget is PC)
            {
                PC pc = (PC)CombatTarget;
                pnt = (Point)pc.CombatLocation;
            }
            else if (CombatTarget is Creature)
            {
                Creature crtTarget = (Creature)CombatTarget;
                pnt = (Point)crtTarget.CombatLocation;
            }
            else if (CombatTarget is Point)
            {
                pnt = (Point)CombatTarget;
            }
            else //do not understand, what is the target
            {
                MessageBox.Show("target is not a PC, Creature or Point...creature failed to use trait");
                return;
            }
            if ((CalcDistance(pnt, crt.CombatLocation) <= TraitToUse.Range) && (frm.currentCombat.IsVisibleLineOfSight(crt.CombatLocation, pnt)))
            {
                Point starting = new Point((crt.CombatLocation.X * gm._squareSize) + (gm._squareSize / 2), (crt.CombatLocation.Y * gm._squareSize) + (gm._squareSize / 2));
                Point ending = new Point((pnt.X * gm._squareSize) + (gm._squareSize / 2), (pnt.Y * gm._squareSize) + (gm._squareSize / 2));
                if (starting.X > ending.X) { crt.CombatFacing = CharBase.facing.Left; }
                else { crt.CombatFacing = CharBase.facing.Right; } 
                frm.currentCombat.playCreatureAttackSound(crt, TraitToUse.TraitStartSound);
                frm.currentCombat.drawProjectile(starting, ending, TraitToUse.SpriteFilename);

                if (crt.CharSprite.AttackingNumberOfFrames > 1)
                {
                    frm.currentCombat.attackCreatureAnimation(crt);
                }
                frm.currentCombat.playCreatureAttackSound(crt, TraitToUse.TraitEndSound);
                frm.currentCombat.drawEndEffect(pnt, TraitToUse.AoeRadiusOrLength, TraitToUse.SpriteEndingFilename);

                CombatSource = crt;
                if ((CombatTarget is int) && ((int)CombatTarget >= 0)) //SD_20131110
                {
                    int PCindex = (int)CombatTarget;
                    PC pc = gm.playerList.PCList[PCindex];
                    CombatTarget = pc;
                }
                var script = TraitToUse.TraitScript;
                frm.doScriptBasedOnFilename(script.FilenameOrTag, script.Parm1, script.Parm2, script.Parm3, script.Parm4);

                crt.SP -= TraitToUse.CostSP;
                if (crt.SP < 0) { crt.SP = 0; }
            }
            else //SD_20131110
            {
                //MessageBox.Show("target out of range from spell...move instead");
                #region Do a Melee or Ranged Attack
                // use default closest PC target
                int whichOne = FindNearestPc();
                PC pc = gm.playerList.PCList[whichOne];
                CombatTarget = pc;
                CreatureDoesAttack(crt, pc);
                #endregion
            }
        }
        /*public bool successfulBackStab(PC pc, Creature crt)
        {
            return false;
        }*/
        /*public bool canMakeAdditionalAttack(PC pc, Creature crt)
        {
            return false;
        }*/
        public void doPcAttackAnimation()
        {
            PC pc = (PC)CombatSource;
            if ((!frm.currentCombat.rangedItem) && (!frm.currentCombat.rangedSpell) && (!frm.currentCombat.rangedTrait))
            {
                if (pc.CharSprite.AttackingNumberOfFrames > 1)
                {
                    frm.currentCombat.attackPcAnimation(pc, frm.currentCombat.com_moveOrderList[frm.currentCombat.currentMoveOrderIndex].index);
                }
            }
        }
        public void drawHitSymbolOnPC()
        {
            PC pc = (PC)CombatTarget;
            gm.CombatAreaHitSymbolOnPcRenderAll(pc);
            //gm.drawHitSymbol(pc.CombatLocation.X, pc.CombatLocation.Y);
            //gm.UpdateCombat();
            //Application.DoEvents();
            Thread.Sleep(200);
            //frm.currentCombat.refreshMap();
        }
        public void drawHitSymbolOnCreature()
        {
            Creature crt = (Creature)CombatTarget;
            gm.CombatAreaHitSymbolOnCreatureRenderAll(crt);
            //gm.drawHitSymbol(crt.CombatLocation.X, crt.CombatLocation.Y);
            //gm.UpdateCombat();
            //Application.DoEvents();
            Thread.Sleep(200);
            //frm.currentCombat.refreshMap();
        }
        /*public void doPcAttack(PC pc, Creature crt)
        {
            try
            {
                //check if backstab
                //check if multiple attack same target
                //check if multiple targets
                //check if multiple targets if first dies

                CombatTarget = crt;
                CombatSource = pc;
                pc.UpdateSimpleStats();
                int attackRoll = gm.Random(20);
                int attackMod = CalcPcAttackModifier();
                int attack = attackRoll + attackMod;
                int defense = CalcCreatureDefense();
                int damage = CalcPcDamageToCreature();
                // do attack animation if sprite has animations
                if ((!frm.currentCombat.rangedItem) && (!frm.currentCombat.rangedSpell) && (!frm.currentCombat.rangedTrait))
                {
                    if (pc.CharSprite.AttackingNumberOfFrames > 1)
                    {
                        //frm.currentCombat.attackPcAnimation(pc, frm.currentCombat.com_moveOrderList[frm.currentCombat.currentMoveOrderIndex].index);
                    }
                }
                if (attack >= defense) //HIT
                {                    
                    //gm.drawHitSymbol(crt.CombatLocation.X, crt.CombatLocation.Y);
                    //gm.UpdateCombat();
                    //Application.DoEvents();
                    //Thread.Sleep(100);
                    //frm.currentCombat.refreshMap();
                    frm.currentCombat.logText(pc.Name, Color.Blue);
                    frm.currentCombat.logText(" attacks ", Color.Black);
                    frm.currentCombat.logText(crt.Name, Color.LightGray);
                    frm.currentCombat.logText(" and HITS for ", Color.Black);
                    frm.currentCombat.logText(damage.ToString(), Color.Lime);
                    frm.currentCombat.logText(" point(s) of damage", Color.Black);
                    frm.currentCombat.logText(Environment.NewLine, Color.Black);
                    frm.currentCombat.logText(attackRoll.ToString() + " + " + attackMod.ToString() + " >= " + defense.ToString(), Color.Black);
                    frm.currentCombat.logText(Environment.NewLine, Color.Black);
                    frm.currentCombat.logText(Environment.NewLine, Color.Black);

                    crt.HP = crt.HP - damage;
                    if (crt.HP <= 0)
                    {
                        frm.currentCombat.logText("You killed the " + crt.Name, Color.Lime);
                        frm.currentCombat.logText(Environment.NewLine, Color.Black);
                        frm.currentCombat.logText(Environment.NewLine, Color.Black);
                        frm.currentCombat.com_encounter.EncounterCreatureList.creatures[frm.currentCombat.bumpedIntoCreatureIndex].CharSprite.Image = new Bitmap(gm.mainDirectory + "\\data\\graphics\\rip.png");
                        frm.currentCombat.refreshMap();
                    }
                }
                else //MISSED
                {
                    frm.currentCombat.logText(pc.Name, Color.Blue);
                    frm.currentCombat.logText(" attacks ", Color.Black);
                    frm.currentCombat.logText(crt.Name, Color.LightGray);
                    frm.currentCombat.logText(" and MISSES", Color.Black);
                    frm.currentCombat.logText(Environment.NewLine, Color.Black);
                    frm.currentCombat.logText(attackRoll.ToString() + " + " + attackMod.ToString() + " < " + defense.ToString(), Color.Black);
                    frm.currentCombat.logText(Environment.NewLine, Color.Black);
                    frm.currentCombat.logText(Environment.NewLine, Color.Black);
                }
            }
            catch (Exception ex)
            {
                IBMessageBox.Show(gm, "failed doPcAttack");
                gm.errorLog(ex.ToString());
            }
        }*/
        public int CalcPcAttackModifier()
        {
            PC pc = (PC)CombatSource;
            Creature crt = (Creature)CombatTarget;
            int modifier = 0;
            if ((pc.MainHand.ItemCategory == Item.category.Melee) || (pc.MainHand.ItemName == ""))
            {
                modifier = (pc.Strength - 10) / 2;
            }
            else //ranged weapon used
            {
                modifier = (pc.Dexterity - 10) / 2;
            }
            int attackMod = modifier + pc.BaseAttBonus + pc.MainHand.ItemAttackBonus;
            return attackMod;
        }
        public int CalcCreatureDefense()
        {
            PC pc = (PC)CombatSource;
            Creature crt = (Creature)CombatTarget;
            int defense = crt.ACBase;
            return defense;
        }
        public int CalcPcDamageToCreature() //SD_20131126 //SD_20131127
        {
            PC pc = (PC)CombatSource;
            Creature crt = (Creature)CombatTarget;
            int damModifier = 0;
            int adder = 0;
            if ((pc.MainHand.ItemCategory == Item.category.Melee) || (pc.MainHand.ItemName == ""))
            {
                damModifier = (pc.Strength - 10) / 2;
                foreach (Trait tr in pc.KnownTraitsList.traitList)
                {
                    adder += tr.DamageMeleeModifier;
                }
                foreach (Effect ef in pc.EffectsList.effectsList)
                {
                    adder += ef.DamageMeleeModifier;
                }
            }
            else //ranged weapon used
            {
                damModifier = 0;
                foreach (Trait tr in pc.KnownTraitsList.traitList)
                {
                    adder += tr.DamageRangedModifier;
                }
                foreach (Effect ef in pc.EffectsList.effectsList)
                {
                    adder += ef.DamageRangedModifier;
                }
            }            

            int dDam = pc.MainHand.ItemDamageDie;
            float damage = (pc.MainHand.ItemDamageNumberOfDice * gm.Random(dDam)) + damModifier + adder + pc.MainHand.ItemDamageAdder - crt.DamageReduction; //SD_20131127 changed to use ItemDamageAdder
            if (damage < 0)
            {
                damage = 0;
            }

            float resist = 0;

            if (pc.MainHand.TypeOfDamage == DamageType.Acid)
            {
                resist = (float)(1f - ((float)crt.DamageTypeResistanceValueAcid / 100f));
            }
            else if (pc.MainHand.TypeOfDamage == DamageType.Bludgeoning)
            {
                resist = (float)(1f - ((float)crt.DamageTypeResistanceValueBludgeoning / 100f));
            }
            else if (pc.MainHand.TypeOfDamage == DamageType.Cold)
            {
                resist = (float)(1f - ((float)crt.DamageTypeResistanceValueCold / 100f));
            }
            else if (pc.MainHand.TypeOfDamage == DamageType.Electricity)
            {
                resist = (float)(1f - ((float)crt.DamageTypeResistanceValueElectricity / 100f));
            }
            else if (pc.MainHand.TypeOfDamage == DamageType.Fire)
            {
                resist = (float)(1f - ((float)crt.DamageTypeResistanceValueFire / 100f));
            }
            else if (pc.MainHand.TypeOfDamage == DamageType.Light)
            {
                resist = (float)(1f - ((float)crt.DamageTypeResistanceValueLight / 100f));
            }
            else if (pc.MainHand.TypeOfDamage == DamageType.Magic)
            {
                resist = (float)(1f - ((float)crt.DamageTypeResistanceValueMagic / 100f));
            }
            else if (pc.MainHand.TypeOfDamage == DamageType.Piercing)
            {
                resist = (float)(1f - ((float)crt.DamageTypeResistanceValuePiercing / 100f));
            }
            else if (pc.MainHand.TypeOfDamage == DamageType.Poison)
            {
                resist = (float)(1f - ((float)crt.DamageTypeResistanceValuePoison / 100f));
            }
            else if (pc.MainHand.TypeOfDamage == DamageType.Slashing)
            {
                resist = (float)(1f - ((float)crt.DamageTypeResistanceValueSlashing / 100f));
            }
            else if (pc.MainHand.TypeOfDamage == DamageType.Sonic)
            {
                resist = (float)(1f - ((float)crt.DamageTypeResistanceValueSonic / 100f));
            }

            int totalDam = (int)(damage * resist);
            if (totalDam < 0)
            {
                totalDam = 0;
            }

            return totalDam;
        }
        /*public void doCreatureAttack(Creature crt, PC pc)
        {
            CombatTarget = pc;
            CombatSource = crt;
            int attackRoll = gm.Random(20);
            int attackMod = CalcCreatureAttackModifier();
            int defense = CalcPcDefense();
            int damage = CalcCreatureDamageToPc();
            int attack = attackRoll + attackMod;
            //frm.currentCombat.playCreatureAttackSound(crt);
            // do attack animation if sprite has animations
            if (crt.CharSprite.AttackingNumberOfFrames > 1)
            {
                frm.currentCombat.attackCreatureAnimation(crt);
            }
            if (crt.WeaponType != Creature.crCategory.Ranged)
            {
                frm.currentCombat.playCreatureAttackSound(crt);
            }
            if (attack >= defense)
            {
                //gm.drawHitSymbol(pc.CombatLocation.X, pc.CombatLocation.Y);
                //gm.UpdateCombat();
                //Application.DoEvents();
                //Thread.Sleep(100);
                //frm.currentCombat.refreshMap();
                frm.currentCombat.logText(crt.Name, Color.LightGray);
                frm.currentCombat.logText(" attacks ", Color.Black);
                frm.currentCombat.logText(pc.Name, Color.Blue);
                frm.currentCombat.logText(" and HITS for ", Color.Black);
                frm.currentCombat.logText(damage.ToString(), Color.Red);
                frm.currentCombat.logText(" point(s) of damage", Color.Black);
                frm.currentCombat.logText(Environment.NewLine, Color.Black);
                frm.currentCombat.logText(attackRoll.ToString() + " + " + attackMod.ToString() + " >= " + defense.ToString(), Color.Black);
                frm.currentCombat.logText(Environment.NewLine, Color.Black);
                frm.currentCombat.logText(Environment.NewLine, Color.Black);

                pc.HP = pc.HP - damage;
                if (pc.HP <= 0)
                {
                    frm.currentCombat.logText(pc.Name + " has been killed!", Color.Red);
                    frm.currentCombat.logText(Environment.NewLine, Color.Black);
                    frm.currentCombat.logText(Environment.NewLine, Color.Black);
                    pc.Status = PC.charStatus.Dead;
                }
            }
            else
            {
                frm.currentCombat.logText(crt.Name, Color.LightGray);
                frm.currentCombat.logText(" attacks ", Color.Black);
                frm.currentCombat.logText(pc.Name, Color.Blue);
                frm.currentCombat.logText(" and MISSES", Color.Black);
                frm.currentCombat.logText(Environment.NewLine, Color.Black);
                frm.currentCombat.logText(attackRoll.ToString() + " + " + attackMod.ToString() + " < " + defense.ToString(), Color.Black);
                frm.currentCombat.logText(Environment.NewLine, Color.Black);
                frm.currentCombat.logText(Environment.NewLine, Color.Black);
            }
        }*/
        public int CalcCreatureAttackModifier()
        {
            PC pc = (PC)CombatTarget;
            Creature crt = (Creature)CombatSource;
            return crt.Attack;
        }
        public int CalcCreatureAttackModifier(PC pc, Creature crt)
        {
            return crt.Attack;
        }
        public int CalcPcDefense()
        {
            PC pc = (PC)CombatTarget;
            Creature crt = (Creature)CombatSource;
            pc.UpdateStats(this);
            return pc.AC;
        }
        public int CalcPcDefense(PC pc, Creature crt)
        {
            pc.UpdateStats(this);
            return pc.AC;
        }
        public int CalcCreatureDamageToPc() //SD_20131126
        {
            PC pc = (PC)CombatTarget;
            Creature crt = (Creature)CombatSource;
            int totalDam = CalcCreatureDamageToPc(pc, crt);
            return totalDam;
        }
        public int CalcCreatureDamageToPc(PC pc, Creature crt) //SD_20131126
        {
            int armDamRed = 0;
            if (pc.Body != null)
            {
                armDamRed = pc.Body.ItemDamageReduction;
            }
            int dDam = crt.DamageDie;
            float damage = (crt.NumberOfDamageDice * gm.Random(dDam)) - armDamRed + crt.DamageAdder;
            if (damage < 0)
            {
                damage = 0;
            }

            float resist = 0;

            if (crt.TypeOfDamage == DamageType.Acid)
            {
                resist = (float)(1f - ((float)pc.DamageTypeResistanceTotalAcid / 100f));
            }
            else if (crt.TypeOfDamage == DamageType.Bludgeoning)
            {
                resist = (float)(1f - ((float)pc.DamageTypeResistanceTotalBludgeoning / 100f));
            }
            else if (crt.TypeOfDamage == DamageType.Cold)
            {
                resist = (float)(1f - ((float)pc.DamageTypeResistanceTotalCold / 100f));
            }
            else if (crt.TypeOfDamage == DamageType.Electricity)
            {
                resist = (float)(1f - ((float)pc.DamageTypeResistanceTotalElectricity / 100f));
            }
            else if (crt.TypeOfDamage == DamageType.Fire)
            {
                resist = (float)(1f - ((float)pc.DamageTypeResistanceTotalFire / 100f));
            }
            else if (crt.TypeOfDamage == DamageType.Light)
            {
                resist = (float)(1f - ((float)pc.DamageTypeResistanceTotalLight / 100f));
            }
            else if (crt.TypeOfDamage == DamageType.Magic)
            {
                resist = (float)(1f - ((float)pc.DamageTypeResistanceTotalMagic / 100f));
            }
            else if (crt.TypeOfDamage == DamageType.Piercing)
            {
                resist = (float)(1f - ((float)pc.DamageTypeResistanceTotalPiercing / 100f));
            }
            else if (crt.TypeOfDamage == DamageType.Poison)
            {
                resist = (float)(1f - ((float)pc.DamageTypeResistanceTotalPoison / 100f));
            }
            else if (crt.TypeOfDamage == DamageType.Slashing)
            {
                resist = (float)(1f - ((float)pc.DamageTypeResistanceTotalSlashing / 100f));
            }
            else if (crt.TypeOfDamage == DamageType.Sonic)
            {
                resist = (float)(1f - ((float)pc.DamageTypeResistanceTotalSonic / 100f));
            }

            int totalDam = (int)(damage * resist);
            if (totalDam < 0)
            {
                totalDam = 0;
            }

            return totalDam;
        }
        public void LeaveThreatenedCheck(PC pc, Point lastPlayerLocation)
        {
            //iterate through each creature
            foreach (Creature crt in gm.currentEncounter.EncounterCreatureList.creatures)
            {
            	// * sinopip, 25.12.14 : added a check for PC alive too
            	if ((crt.HP > 0) && (pc.HP > 0) && (crt.Status != CharBase.charStatus.Held) && (crt.Status != CharBase.charStatus.Sleeping)) //SD_20131215
                {
                    //if started in distance = 1 and now distance = 2 then do attackOfOpportunity
                    if ((CalcDistance(crt.CombatLocation, lastPlayerLocation) == 1) && (CalcDistance(crt.CombatLocation, pc.CombatLocation) == 2))
                    {
                        frm.currentCombat.logText("Attack of Opportunity by: " + crt.Name, Color.Blue);
                        frm.currentCombat.logText(Environment.NewLine, Color.Black);
                        DoCreatureAttackOfOpportunity(crt, pc, lastPlayerLocation);
                        if (pc.HP <= 0)
                        {
                            frm.currentCombat.currentMoves = 20;
                            // * sinopip, 25.12.14
                            frm.currentCombat.doOnDeathScripts();
                            //
                        }
                    }
                }
            }
        }
        public void DoCreatureAttackOfOpportunity(Creature crt, PC pc, Point lastPlayerLocation)
        {
            returnScriptObject = null; //after using the generic "returnScriptObject", make sure to set it back to null

            #region AvoidAttackOfOpportunity
            // run OnStartCombatTurn script 
            CombatTarget = pc;
            CombatSource = crt;
            frm.doScriptBasedOnFilename("dsAvoidAttackOfOpportunity.cs", "none", "none", "none", "none");
            #endregion

            if (returnScriptObject == null) { return; }
            if ((bool)returnScriptObject == false) //failed to avoid the attack of opportunity
            {
                if ((crt.HP > 0) && (crt.Status != CharBase.charStatus.Held) && (crt.Status != CharBase.charStatus.Sleeping))
                {
                    Combat c = frm.currentCombat;

                    //note: I had to to borrow lots of code directly from scriptfunctions. dll as the function calls themselves mixed up crt and pc
                    int attackRoll = gm.Random(20);
                    int attackMod = CalcCreatureAttackModifier(pc, crt);
                    int defense = CalcPcDefense(pc, crt);
                    int damage = CalcCreatureDamageToPc(pc, crt);
                    int attack = attackRoll + attackMod;
                    #region Critical Hit Stuff
                    int criticalHitScored = 0;
                    //critical hit (see detail in modified attackPlayer.cs)
                    int criticalHitRange = crt.CriticalHitRange;
                    //variable for storing the critical hit multiplier
                    int criticalHitDamageMultiplier = crt.CriticalHitDamageMultiplier;
                    //critical hit mechanism - triggered on a roll equal or higher than criticalHitRange  
                    if ((attackRoll >= criticalHitRange) || (attackRoll == 20))
                    {
                        int attackRoll2 = gm.Random(20);
                        int attackMod2 = CalcCreatureAttackModifier(pc, crt);
                        int attack2 = attackRoll2 + attackMod2;
                        int defense2 = CalcPcDefense(pc, crt);
                        if ((attack2 >= defense2) || (attackRoll2 == 20))
                        {
                            criticalHitScored = 1;
                            //normal damage is rolled aagin - backstab damage is not double rolled
                            int additionaldamage = damage;
                            for (int i = 0; i < criticalHitDamageMultiplier; i++)
                            {
                                damage += additionaldamage;
                            }
                        }
                    }
                    #endregion
                    // do attack animation if sprite has animations
                    if (crt.CharSprite.AttackingNumberOfFrames > 1)
                    {
                        c.attackCreatureAnimation(crt);
                    }
                    if (crt.WeaponType != Creature.crCategory.Ranged)
                    {
                        c.playCreatureAttackSound(crt);
                    }
                    if ((attack >= defense) || (attackRoll == 20))
                    {
                        //sf.drawHitSymbolOnPC();
                        
                        // * sinopip, 22.12.14
                        c.playCreatureHitSound(crt);
                        //

                        //Some addition to explain why the extra attack has happened
                        string attackResult = (damage.ToString() + " of " + pc.HP.ToString());
                        if (criticalHitScored == 1)
                        {
                            DrawCombatFloatyTextOverSquare(("CRITICAL free attack of opportunity: " + attackResult), pc.CombatLocation.X, pc.CombatLocation.Y, 60, 8, -35, Color.Red, Color.Black);
                        }
                        else
                        {
                            DrawCombatFloatyTextOverSquare(("Free attack of opportunity: " + attackResult), pc.CombatLocation.X, pc.CombatLocation.Y, 60, 8, -35, Color.Red, Color.Black);
                        }
                        c.logText(crt.Name, Color.LightGray);
                        if (criticalHitScored == 1)
                        {
                            c.logText(" CRITICALLY attacks ", Color.Black);
                        }
                        else
                        {
                            c.logText(" attacks ", Color.Black);
                        }
                        c.logText(pc.Name, Color.Blue);
                        //Some addition to explain why the extra attack has happened
                        c.logText(" due to attack of opportunity and HITS for ", Color.Black);
                        c.logText(damage.ToString(), Color.Red);
                        c.logText(" point(s) of damage", Color.Black);
                        c.logText(Environment.NewLine, Color.Black);
                        c.logText(attackRoll.ToString() + " + " + attackMod.ToString() + " >= " + defense.ToString(), Color.Black);
                        c.logText(Environment.NewLine, Color.Black);
                        c.logText(Environment.NewLine, Color.Black);
                                                
                        pc.HP = pc.HP - damage;
                        if (pc.HP <= 0)
                        {
                            c.logText(pc.Name + " drops down unconsciously!", Color.Red);
                            c.logText(Environment.NewLine, Color.Black);
                            c.logText(Environment.NewLine, Color.Black);
                            pc.Status = PC.charStatus.Dead;
                        }
                    }
                    else
                    {
                        DrawCombatFloatyTextOverSquare("Evades free attack of opportunity!", pc.CombatLocation.X, pc.CombatLocation.Y, 60, 16, -5, Color.Blue, Color.Black);
                        c.logText(crt.Name, Color.LightGray);
                        c.logText(" attacks ", Color.Black);
                        c.logText(pc.Name, Color.Blue);
                        c.logText(" due to attack of opportunity and MISSES", Color.Black);
                        c.logText(Environment.NewLine, Color.Black);
                        c.logText(attackRoll.ToString() + " + " + attackMod.ToString() + " < " + defense.ToString(), Color.Black);
                        c.logText(Environment.NewLine, Color.Black);
                        c.logText(Environment.NewLine, Color.Black);
                    }

                    /* OLD WAY OF CREATURE ATTACK
                    frm.sf.CombatTarget = pc;
                    frm.sf.CombatSource = crt;
                    int attackRoll = gm.Random(20);
                    int attackMod = frm.sf.CalcCreatureAttackModifier();
                    int defense = frm.sf.CalcPcDefense();
                    int damage = frm.sf.CalcCreatureDamageToPc();
                    int attack = attackRoll + attackMod;

                    frm.currentCombat.playCreatureAttackSound(crt);
                    // do attack animation if sprite has animations
                    if (crt.CharSprite.AttackingNumberOfFrames > 1)
                    {
                        frm.currentCombat.attackCreatureAnimation(crt);
                    }
                    if (attack >= defense)
                    {
                        //gm.drawHitSymbol(lastPlayerLocation.X, lastPlayerLocation.Y);
                        //gm.UpdateCombat();
                        //Application.DoEvents();
                        //Thread.Sleep(100);
                        //frm.currentCombat.refreshMap();
                        frm.currentCombat.logText(crt.Name, Color.LightGray);
                        frm.currentCombat.logText(" attacks ", Color.Black);
                        frm.currentCombat.logText(pc.Name, Color.Blue);
                        frm.currentCombat.logText(" and HITS for ", Color.Black);
                        frm.currentCombat.logText(damage.ToString(), Color.Red);
                        frm.currentCombat.logText(" point(s) of damage", Color.Black);
                        frm.currentCombat.logText(Environment.NewLine, Color.Black);
                        frm.currentCombat.logText(attackRoll.ToString() + " + " + attackMod.ToString() + " >= " + defense.ToString(), Color.Black);
                        frm.currentCombat.logText(Environment.NewLine, Color.Black);
                        frm.currentCombat.logText(Environment.NewLine, Color.Black);

                        pc.HP = pc.HP - damage;
                        if (pc.HP <= 0)
                        {
                            frm.currentCombat.logText(pc.Name + " has been killed!", Color.Red);
                            frm.currentCombat.logText(Environment.NewLine, Color.Black);
                            frm.currentCombat.logText(Environment.NewLine, Color.Black);
                            pc.Status = PC.charStatus.Dead;
                            frm.currentCombat.currentMoves = 99;
                        }
                    }
                    else
                    {
                        frm.currentCombat.logText(crt.Name, Color.LightGray);
                        frm.currentCombat.logText(" attacks ", Color.Black);
                        frm.currentCombat.logText(pc.Name, Color.Blue);
                        frm.currentCombat.logText(" and MISSES", Color.Black);
                        frm.currentCombat.logText(Environment.NewLine, Color.Black);
                        frm.currentCombat.logText(attackRoll.ToString() + " + " + attackMod.ToString() + " < " + defense.ToString(), Color.Black);
                        frm.currentCombat.logText(Environment.NewLine, Color.Black);
                        frm.currentCombat.logText(Environment.NewLine, Color.Black);
                    }
                    */
                }
                else
                {
                    frm.currentCombat.logText(crt.Name, Color.LightGray);
                    frm.currentCombat.logText(" is unconscious...skips turn", Color.Black);
                    frm.currentCombat.logText(Environment.NewLine, Color.Black);
                    frm.currentCombat.logText(Environment.NewLine, Color.Black);
                }
            }
            returnScriptObject = null; //after using the generic "returnScriptObject", make sure to set it back to null
        }
        public int ReturnPCWithLowestHP()
        {
            int PCindex = 0;
            int lowHP = 999;
            int cnt = 0;
            foreach (PC pc in gm.playerList.PCList)
            {
                if (pc.Status != PC.charStatus.Dead)
                {
                    if (pc.HP < lowHP)
                    {
                        lowHP = pc.HP;
                        PCindex = cnt;
                    }
                }
                cnt++;
            }
            return PCindex;
        }
        public int TargetPCWithLowestHP(bool ignoreIfInStealthMode)
        {
            int PCindex = 0;
            int lowHP = 999;
            int cnt = 0;
            foreach (PC pc in gm.playerList.PCList)
            {
                if (ignoreIfInStealthMode)
                {
                    if ((pc.Status != PC.charStatus.Dead) && (!CheckLocalInt(pc.Tag, "StealthModeOn", "=", 1)))
                    {
                        if (pc.HP < lowHP)
                        {
                            lowHP = pc.HP;
                            PCindex = cnt;
                        }
                    }
                    cnt++;
                }
                else
                {
                    if (pc.Status != PC.charStatus.Dead)
                    {
                        if (pc.HP < lowHP)
                        {
                            lowHP = pc.HP;
                            PCindex = cnt;
                        }
                    }
                    cnt++;
                }
            }
            return PCindex;
        }
        public int TargetClosestPC(bool ignoreIfInStealthMode)
        {
            Creature crt = (Creature)CombatSource; //this is the creature that is calling this script
            int PCindex = -1;
            int farDist = 99;
            int cnt = 0;
            foreach (PC pc in gm.playerList.PCList)
            {
                if (ignoreIfInStealthMode)
                {
                    if ((pc.Status != PC.charStatus.Dead) && (!CheckLocalInt(pc.Tag, "StealthModeOn", "=", 1)))
                    {
                        int dist = CalcDistance(crt.CombatLocation, pc.CombatLocation);
                        if (dist < farDist)
                        {
                            farDist = dist;
                            PCindex = cnt;
                        }
                    }
                    cnt++;
                }
                else
                {
                    if (pc.Status != PC.charStatus.Dead)
                    {
                        int dist = CalcDistance(crt.CombatLocation, pc.CombatLocation);
                        if (dist < farDist)
                        {
                            farDist = dist;
                            PCindex = cnt;
                        }
                    }
                    cnt++;
                }
            }
            return PCindex;
        }
        public int TargetClosestPcNotHeld(bool ignoreIfInStealthMode)
        {
            Creature crt = (Creature)CombatSource; //this is the creature that is calling this script
            int PCindex = -1;
            int farDist = 99;
            int cnt = 0;
            foreach (PC pc in gm.playerList.PCList)
            {
                if (ignoreIfInStealthMode)
                {
                    //ignore if held, dead, or in stealth mode
                    if ((pc.Status != PC.charStatus.Held) && (pc.Status != PC.charStatus.Dead) && (!CheckLocalInt(pc.Tag, "StealthModeOn", "=", 1)))
                    {
                        int dist = CalcDistance(crt.CombatLocation, pc.CombatLocation);
                        if (dist < farDist)
                        {
                            farDist = dist;
                            PCindex = cnt;
                        }
                    }
                    cnt++;
                }
                else
                {
                    //ignore if held or dead
                    if ((pc.Status != PC.charStatus.Held) && (pc.Status != PC.charStatus.Dead))
                    {
                        int dist = CalcDistance(crt.CombatLocation, pc.CombatLocation);
                        if (dist < farDist)
                        {
                            farDist = dist;
                            PCindex = cnt;
                        }
                    }
                    cnt++;
                }
            }
            return PCindex;
        }
        public int FindNearestPc()
        {
            int index = -1;
            int farDist = 99;
            int cnt = 0;
            foreach (PC pc in gm.playerList.PCList)
            {
                if (pc.Status != PC.charStatus.Dead)
                {
                    int dist = CalcDistance(frm.currentEncounter.EncounterCreatureList.creatures[frm.currentCombat.com_moveOrderList[frm.currentCombat.currentMoveOrderIndex].index].CombatLocation, pc.CombatLocation);
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
        public int CalcDistance(Point locCr, Point locPc)
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
        public Creature GetCreatureWithLowestHP()
        {
            int lowHP = 999;
            Creature returnCrt = null;
            foreach (Creature crt in gm.currentEncounter.EncounterCreatureList.creatures)
            {
                if (crt.HP > 0)
                {
                    if (crt.HP < lowHP)
                    {
                        lowHP = crt.HP;
                        returnCrt = crt;
                    }
                }
            }
            return returnCrt;
        }
        public Creature GetNextAdjacentCreature(PC pc)
        {
            foreach (Creature nextCrt in gm.currentEncounter.EncounterCreatureList.creatures)
            {
                if ((CalcDistance(pc.CombatLocation, nextCrt.CombatLocation) < 2) && (nextCrt.HP > 0))
                {
                    return nextCrt;
                }
            }
            return null;
        }
        #endregion

        #region Pathfinding
        public void setupPathfindArray(Creature c, PC p)
        {
            for (int x = 0; x <= (gm.currentCombatArea.MapSizeInSquares.Width - 1); x++)
            {
                for (int y = 0; y <= (gm.currentCombatArea.MapSizeInSquares.Height - 1); y++)
                {
                    //logText("x=" + x.ToString() + " y=" + y.ToString(), Color.Black);
                    //logText(Environment.NewLine, Color.Black);
                    if (gm.currentCombatArea.getCombatTile(x, y).collidable == true)
                        pathfinder.Squares[x, y].ContentCode = SquareContent.Wall;
                    else
                        pathfinder.Squares[x, y].ContentCode = SquareContent.Empty;
                }
            }
            foreach (Creature crt in frm.currentEncounter.EncounterCreatureList.creatures)
            {
                if (crt == c) { continue; }
                if (crt.HP > 0)
                {
                    //TODO if creature size is greater than 1, make all squares walls
                    if (crt.Size == 1)
                    {
                        pathfinder.Squares[crt.CombatLocation.X, crt.CombatLocation.Y].ContentCode = SquareContent.Wall;
                    }
                    if (crt.Size == 2)
                    {
                        pathfinder.Squares[crt.CombatLocation.X + 0, crt.CombatLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinder.Squares[crt.CombatLocation.X + 1, crt.CombatLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinder.Squares[crt.CombatLocation.X + 0, crt.CombatLocation.Y + 1].ContentCode = SquareContent.Wall;
                        pathfinder.Squares[crt.CombatLocation.X + 1, crt.CombatLocation.Y + 1].ContentCode = SquareContent.Wall;
                    }
                    if (crt.Size == 3)
                    {
                        pathfinder.Squares[crt.CombatLocation.X + 0, crt.CombatLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinder.Squares[crt.CombatLocation.X + 1, crt.CombatLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinder.Squares[crt.CombatLocation.X + 2, crt.CombatLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinder.Squares[crt.CombatLocation.X + 0, crt.CombatLocation.Y + 1].ContentCode = SquareContent.Wall;
                        pathfinder.Squares[crt.CombatLocation.X + 2, crt.CombatLocation.Y + 1].ContentCode = SquareContent.Wall;
                        pathfinder.Squares[crt.CombatLocation.X + 0, crt.CombatLocation.Y + 2].ContentCode = SquareContent.Wall;
                        pathfinder.Squares[crt.CombatLocation.X + 1, crt.CombatLocation.Y + 2].ContentCode = SquareContent.Wall;
                        pathfinder.Squares[crt.CombatLocation.X + 2, crt.CombatLocation.Y + 2].ContentCode = SquareContent.Wall;
                    }
                }
            }
            foreach (Prop prp in frm.currentEncounter.EncounterPropList.propsList)
            {
                if (prp.HasCollision)
                {
                    pathfinder.Squares[prp.Location.X, prp.Location.Y].ContentCode = SquareContent.Wall;
                }
            }
            foreach (PC pc in gm.playerList.PCList)
            {
                if (pc == p) { continue; }
                if (pc.HP > 0)
                {
                    pathfinder.Squares[pc.CombatLocation.X, pc.CombatLocation.Y].ContentCode = SquareContent.Wall;
                }
            }
        }
        public void setupPathfindArrayForCreatureMainArea(Creature c)
        {
            for (int x = 0; x <= (gm.currentArea.MapSizeInSquares.Width - 1); x++)
            {
                for (int y = 0; y <= (gm.currentArea.MapSizeInSquares.Height - 1); y++)
                {
                    //logText("x=" + x.ToString() + " y=" + y.ToString(), Color.Black);
                    //logText(Environment.NewLine, Color.Black);
                    if (gm.currentArea.getTile(x, y).collidable == true)
                        pathfinderMainArea.Squares[x, y].ContentCode = SquareContent.Wall;
                    else
                        pathfinderMainArea.Squares[x, y].ContentCode = SquareContent.Empty;
                }
            }
            foreach (Creature crt in gm.currentArea.AreaCreatureList.creatures)
            {
                if (crt == c) { continue; }
                if (crt.HP > 0)
                {
                    //TODO if creature size is greater than 1, make all squares walls
                    if (crt.Size == 1)
                    {
                        pathfinderMainArea.Squares[crt.MapLocation.X, crt.MapLocation.Y].ContentCode = SquareContent.Wall;
                    }
                    if (crt.Size == 2)
                    {
                        pathfinderMainArea.Squares[crt.MapLocation.X + 0, crt.MapLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crt.MapLocation.X + 1, crt.MapLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crt.MapLocation.X + 0, crt.MapLocation.Y + 1].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crt.MapLocation.X + 1, crt.MapLocation.Y + 1].ContentCode = SquareContent.Wall;
                    }
                    if (crt.Size == 3)
                    {
                        pathfinderMainArea.Squares[crt.MapLocation.X + 0, crt.MapLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crt.MapLocation.X + 1, crt.MapLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crt.MapLocation.X + 2, crt.MapLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crt.MapLocation.X + 0, crt.MapLocation.Y + 1].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crt.MapLocation.X + 2, crt.MapLocation.Y + 1].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crt.MapLocation.X + 0, crt.MapLocation.Y + 2].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crt.MapLocation.X + 1, crt.MapLocation.Y + 2].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crt.MapLocation.X + 2, crt.MapLocation.Y + 2].ContentCode = SquareContent.Wall;
                    }
                }
            }
            foreach (Prop prp in gm.currentArea.AreaPropList.propsList)
            {
                if (prp.HasCollision)
                {
                    pathfinderMainArea.Squares[prp.Location.X, prp.Location.Y].ContentCode = SquareContent.Wall;
                }
            }
            pathfinderMainArea.Squares[gm.playerPosition.X, gm.playerPosition.Y].ContentCode = SquareContent.Wall;
        }
        public void setupPathfindArrayForPlayerMainArea(PC p)
        {
            for (int x = 0; x <= (gm.currentArea.MapSizeInSquares.Width - 1); x++)
            {
                for (int y = 0; y <= (gm.currentArea.MapSizeInSquares.Height - 1); y++)
                {
                    //logText("x=" + x.ToString() + " y=" + y.ToString(), Color.Black);
                    //logText(Environment.NewLine, Color.Black);
                    if (gm.currentArea.getTile(x, y).collidable == true)
                        pathfinderMainArea.Squares[x, y].ContentCode = SquareContent.Wall;
                    else
                        pathfinderMainArea.Squares[x, y].ContentCode = SquareContent.Empty;
                }
            }
            foreach (Creature crt in gm.currentArea.AreaCreatureList.creatures)
            {
                if (crt.HP > 0)
                {
                    //TODO if creature size is greater than 1, make all squares walls
                    if (crt.Size == 1)
                    {
                        pathfinderMainArea.Squares[crt.MapLocation.X, crt.MapLocation.Y].ContentCode = SquareContent.Wall;
                    }
                    if (crt.Size == 2)
                    {
                        pathfinderMainArea.Squares[crt.MapLocation.X + 0, crt.MapLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crt.MapLocation.X + 1, crt.MapLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crt.MapLocation.X + 0, crt.MapLocation.Y + 1].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crt.MapLocation.X + 1, crt.MapLocation.Y + 1].ContentCode = SquareContent.Wall;
                    }
                    if (crt.Size == 3)
                    {
                        pathfinderMainArea.Squares[crt.MapLocation.X + 0, crt.MapLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crt.MapLocation.X + 1, crt.MapLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crt.MapLocation.X + 2, crt.MapLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crt.MapLocation.X + 0, crt.MapLocation.Y + 1].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crt.MapLocation.X + 2, crt.MapLocation.Y + 1].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crt.MapLocation.X + 0, crt.MapLocation.Y + 2].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crt.MapLocation.X + 1, crt.MapLocation.Y + 2].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crt.MapLocation.X + 2, crt.MapLocation.Y + 2].ContentCode = SquareContent.Wall;
                    }
                }
            }
            foreach (Prop prp in gm.currentArea.AreaPropList.propsList)
            {
                if (prp.HasCollision)
                {
                    pathfinderMainArea.Squares[prp.Location.X, prp.Location.Y].ContentCode = SquareContent.Wall;
                }
            }
            //pathfinderMainArea.Squares[gm.playerPosition.X, gm.playerPosition.Y].ContentCode = SquareContent.Wall;
        }
        public void Recalculate(Creature crt)
        {
            pathfinder.ClearLogic();
            pathfinder.Pathfind(crt);
            //navigatePath();
            //_pathfinder.DrawBoard(boardControl1);
        }
        public void RecalculateForCreatureMainMap(Creature crt)
        {
            pathfinderMainArea.ClearLogic();
            pathfinderMainArea.Pathfind(crt);
            //navigatePath();
            //_pathfinder.DrawBoard(boardControl1);
        }
        public void RecalculateForPlayerMainArea(PC pc)
        {
            pathfinderMainArea.ClearLogic();
            pathfinderMainArea.Pathfind(pc);
            //navigatePath();
            //_pathfinder.DrawBoard(boardControl1);
        }
        public void showSteps()
        {
            Font font = new Font("Arial", 20.0f);
            Brush brush = new SolidBrush(Color.White);
            foreach (Point point in Pathfinder.AllSquares(gm))
            {
                int steps = pathfinder.Squares[point.X, point.Y].DistanceSteps;
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
                if (CalcDistance(pc.CombatLocation, crt.CombatLocation) == 1)
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
        public void setupHeroSquares(PC pc, Creature crt)
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
                        if (Pathfinder.ValidCoordinates(x, y, gm))
                        {
                            pathfinder.Squares[x, y].ContentCode = SquareContent.Hero;
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
                        if (Pathfinder.ValidCoordinates(x, y, gm))
                        {
                            pathfinder.Squares[x, y].ContentCode = SquareContent.Hero;
                        }
                    }
                }
            }
        }
        public void setupHeroSquaresMainMap(Creature crt)
        {
            if (crt.Size == 1)
            {
                return;
            }
            if (crt.Size == 2)
            {
                for (int x = gm.playerPosition.X - 1; x < gm.playerPosition.X + 1; x++)
                {
                    for (int y = gm.playerPosition.Y - 1; y < gm.playerPosition.Y + 1; y++)
                    {
                        if (PathfinderMainArea.ValidCoordinates(x, y, gm))
                        {
                            pathfinderMainArea.Squares[x, y].ContentCode = SquareContent.Hero;
                        }
                    }
                }
            }
            if (crt.Size == 3)
            {
                for (int x = gm.playerPosition.X - 2; x < gm.playerPosition.X + 1; x++)
                {
                    for (int y = gm.playerPosition.Y - 2; y < gm.playerPosition.Y + 1; y++)
                    {
                        if (PathfinderMainArea.ValidCoordinates(x, y, gm))
                        {
                            pathfinderMainArea.Squares[x, y].ContentCode = SquareContent.Hero;
                        }
                    }
                }
            }
        }
        public int navigatePath(Creature crt_pt)
        {
            //Creature crt_pt = new Creature();
            //crt_pt.passRefs(gm, null);
            //crt_pt = com_encounter.EncounterCreatureList.creatures[com_moveOrderList[currentMoveOrderIndex].index];
            //
            // Mark the path from monster to hero.
            //
            Point startingPoint = pathfinder.FindCode(SquareContent.Monster);
            int pointX = startingPoint.X;
            int pointY = startingPoint.Y;
            if (pointX == -1 && pointY == -1) { return 10000; }
            int lowest = 10000;
            int creatureMoves = 0;
            if (crt_pt.CreatureMoveDistance < 1) { return 10000; }
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
                foreach (Point movePoint in pathfinder.ValidMoves(pointX, pointY, crt_pt))
                {
                    int count = pathfinder.Squares[movePoint.X, movePoint.Y].DistanceSteps;
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
                    if (pathfinder.Squares[lowestPoint.X, lowestPoint.Y].ContentCode != SquareContent.Hero)
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
                        gm.CombatAreaRenderAll();
                        Thread.Sleep(100);
                        creatureMoves++;
                        if (creatureMoves >= crt_pt.CreatureMoveDistance)
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

                if (pathfinder.Squares[pointX, pointY].ContentCode == SquareContent.Hero)
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
        public int navigatePathMainMap(Creature crt_pt)
        {
            //Creature crt_pt = new Creature();
            //crt_pt.passRefs(gm, null);
            //crt_pt = com_encounter.EncounterCreatureList.creatures[com_moveOrderList[currentMoveOrderIndex].index];
            //
            // Mark the path from monster to hero.
            //
            Point startingPoint = pathfinderMainArea.FindCode(SquareContent.Monster);
            int pointX = startingPoint.X;
            int pointY = startingPoint.Y;
            if (pointX == -1 && pointY == -1)
            {
                return 10000;
            }
            int lowest = 10000;
            //int creatureMoves = 0;
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
                foreach (Point movePoint in pathfinderMainArea.ValidMoves(pointX, pointY, crt_pt))
                {
                    int count = pathfinderMainArea.Squares[movePoint.X, movePoint.Y].DistanceSteps;
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
                    if (pathfinderMainArea.Squares[lowestPoint.X, lowestPoint.Y].ContentCode != SquareContent.Hero)
                    {
                        if ((lowestPoint.X == crt_pt.MapLocation.X) && (lowestPoint.Y > crt_pt.MapLocation.Y))
                        { crt_pt.Facing = CharBase.facing.Down; }
                        if ((lowestPoint.X == crt_pt.MapLocation.X) && (lowestPoint.Y < crt_pt.MapLocation.Y))
                        { crt_pt.Facing = CharBase.facing.Up; }
                        if ((lowestPoint.X > crt_pt.MapLocation.X) && (lowestPoint.Y == crt_pt.MapLocation.Y))
                        { crt_pt.Facing = CharBase.facing.Right; }
                        if ((lowestPoint.X < crt_pt.MapLocation.X) && (lowestPoint.Y == crt_pt.MapLocation.Y))
                        { crt_pt.Facing = CharBase.facing.Left; }
                        crt_pt.MapLocation = lowestPoint;
                        //crt_pt.CombatLocation.Y = lowestPoint.Y;
                        gm.areaRenderAll();
                        //Thread.Sleep(100);
                        //creatureMoves++;
                        //if (creatureMoves >= crt_pt.MoveDistance)
                        //{ break; }
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

                if (pathfinderMainArea.Squares[pointX, pointY].ContentCode == SquareContent.Hero)
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
        //start YN1_20131027
        public void setupHeroSquaresMainMap(int XCord, int YCord, Creature crt, string YN1Overload)
        {
            if (crt.Size == 1)
            {
                return;
            }
            if (crt.Size == 2)
            {
                for (int x = XCord - 1; x < XCord + 1; x++)
                {
                    for (int y = YCord - 1; y < YCord + 1; y++)
                    {
                        if (PathfinderMainArea.ValidCoordinates(x, y, gm))
                        {
                            pathfinderMainArea.Squares[x, y].ContentCode = SquareContent.Hero;
                        }
                    }
                }
            }
            if (crt.Size == 3)
            {
                for (int x = XCord - 2; x < XCord + 1; x++)
                {
                    for (int y = YCord - 2; y < YCord + 1; y++)
                    {
                        if (PathfinderMainArea.ValidCoordinates(x, y, gm))
                        {
                            pathfinderMainArea.Squares[x, y].ContentCode = SquareContent.Hero;
                        }
                    }
                }
            }
        }
        public void setupPathfindArrayForCreatureMainArea(Creature c, string YN1Overload)
        {
            for (int x = 0; x <= (gm.currentArea.MapSizeInSquares.Width - 1); x++)
            {
                for (int y = 0; y <= (gm.currentArea.MapSizeInSquares.Height - 1); y++)
                {
                    if (gm.currentArea.getTile(x, y).collidable == true)
                    {
                        pathfinderMainArea.Squares[x, y].ContentCode = SquareContent.Wall;
                    }
                    else
                    {
                        pathfinderMainArea.Squares[x, y].ContentCode = SquareContent.Empty;
                    }
                }
            }

            foreach (Creature crtr in gm.currentArea.AreaCreatureList.creatures)
            {
                if (crtr == c) { continue; }
                if (crtr.HP > 0)
                {
                    //TODO if creature size is greater than 1, make all squares walls
                    if (crtr.Size == 1)
                    {
                        pathfinderMainArea.Squares[crtr.MapLocation.X, crtr.MapLocation.Y].ContentCode = SquareContent.Wall;
                    }
                    if (crtr.Size == 2)
                    {
                        pathfinderMainArea.Squares[crtr.MapLocation.X + 0, crtr.MapLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crtr.MapLocation.X + 1, crtr.MapLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crtr.MapLocation.X + 0, crtr.MapLocation.Y + 1].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crtr.MapLocation.X + 1, crtr.MapLocation.Y + 1].ContentCode = SquareContent.Wall;
                    }
                    if (crtr.Size == 3)
                    {
                        pathfinderMainArea.Squares[crtr.MapLocation.X + 0, crtr.MapLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crtr.MapLocation.X + 1, crtr.MapLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crtr.MapLocation.X + 2, crtr.MapLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crtr.MapLocation.X + 0, crtr.MapLocation.Y + 1].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crtr.MapLocation.X + 2, crtr.MapLocation.Y + 1].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crtr.MapLocation.X + 0, crtr.MapLocation.Y + 2].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crtr.MapLocation.X + 1, crtr.MapLocation.Y + 2].ContentCode = SquareContent.Wall;
                        pathfinderMainArea.Squares[crtr.MapLocation.X + 2, crtr.MapLocation.Y + 2].ContentCode = SquareContent.Wall;
                    }
                }
            }

            foreach (Prop prp in gm.currentArea.AreaPropList.propsList)
            {
                if (prp.HasCollision)
                {
                    pathfinderMainArea.Squares[prp.Location.X, prp.Location.Y].ContentCode = SquareContent.Wall;
                }
            }

            //pathfinderMainArea.Squares[gm.playerPosition.X, gm.playerPosition.Y].ContentCode = SquareContent.Wall;

        }
        public int navigatePathMainMap(Creature crt_pt, string YN1Overload)
        {
            //Creature crt_pt = new Creature();
            //crt_pt.passRefs(gm, null);
            //crt_pt = com_encounter.EncounterCreatureList.creatures[com_moveOrderList[currentMoveOrderIndex].index];
            //
            // Mark the path from monster to hero.
            //
            Point startingPoint = pathfinderMainArea.FindCode(SquareContent.Monster);
            int pointX = startingPoint.X;
            int pointY = startingPoint.Y;
            if (pointX == -1 && pointY == -1)
            {
                return 10000;
            }
            int lowest = 10000;
            //int creatureMoves = 0;
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
                foreach (Point movePoint in pathfinderMainArea.ValidMoves(pointX, pointY, crt_pt))
                {
                    int count = pathfinderMainArea.Squares[movePoint.X, movePoint.Y].DistanceSteps;
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
                    //if (pathfinderMainArea.Squares[lowestPoint.X, lowestPoint.Y].ContentCode != SquareContent.Hero)
                    //{
                    if ((lowestPoint.X == crt_pt.MapLocation.X) && (lowestPoint.Y > crt_pt.MapLocation.Y))
                    { crt_pt.Facing = CharBase.facing.Down; }
                    if ((lowestPoint.X == crt_pt.MapLocation.X) && (lowestPoint.Y < crt_pt.MapLocation.Y))
                    { crt_pt.Facing = CharBase.facing.Up; }
                    if ((lowestPoint.X > crt_pt.MapLocation.X) && (lowestPoint.Y == crt_pt.MapLocation.Y))
                    { crt_pt.Facing = CharBase.facing.Right; }
                    if ((lowestPoint.X < crt_pt.MapLocation.X) && (lowestPoint.Y == crt_pt.MapLocation.Y))
                    { crt_pt.Facing = CharBase.facing.Left; }
                    crt_pt.MapLocation = lowestPoint;
                    //crt_pt.CombatLocation.Y = lowestPoint.Y;
                    gm.areaRenderAll();
                    break;
                    //Thread.Sleep(100);
                    //creatureMoves++;
                    //if (creatureMoves >= crt_pt.MoveDistance)
                    //{ break; }
                    //}
                    //logText(crt_pt.Tag + " lowest: " + lowest.ToString(), Color.Black);
                    //logText(Environment.NewLine, Color.Black);
                    //_pathfinder.Squares[lowestPoint.X, lowestPoint.Y].IsPath = true;

                    /*pointX = lowestPoint.X;
                    pointY = lowestPoint.Y;*/
                }
                else
                {
                    //logText(crt_pt.Tag + " lowest: " + lowest.ToString(), Color.Black);
                    //logText(Environment.NewLine, Color.Black);
                    break;
                }

                /*if (pathfinderMainArea.Squares[pointX, pointY].ContentCode == SquareContent.Hero)
                {
                    /*
                     * 
                     * We went from monster to hero, so we're finished.
                     * 
                     * */
                    /*break;
                }*/
            }
            return lowest;
        }
        //end YN1_20131027
        #endregion

        public void TakeItem(string tag, int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                bool FoundOne = false;
                int cnt = 0;
                foreach (Item item in gm.partyInventoryList)
                {
                    if (!FoundOne)
                    {
                        if (item.ItemTag == tag)
                        {
                            gm.partyInventoryList.RemoveAt(cnt);
                            gm.partyInventoryTagList.RemoveAt(cnt);
                            FoundOne = true;
                            break;
                        }
                    }
                    cnt++;
                }
                cnt = 0;
                foreach (PC pc in gm.playerList.PCList)
                {
                    if (!FoundOne)
                    {
                        if (pc.Head.ItemTag == tag) //SD_20131120
                        {
                            gm.playerList.PCList[cnt].Head = new Item();
                            gm.playerList.PCList[cnt].Head.ItemName = "";
                            FoundOne = true;
                        }
                        if (pc.Neck.ItemTag == tag) //SD_20131120
                        {
                            gm.playerList.PCList[cnt].Neck = new Item();
                            gm.playerList.PCList[cnt].Neck.ItemName = "";
                            FoundOne = true;
                        }
                        if (pc.Body.ItemTag == tag)
                        {
                            gm.playerList.PCList[cnt].Body = new Item();
                            gm.playerList.PCList[cnt].Body.ItemName = "";
                            FoundOne = true;
                        }
                        if (pc.OffHand.ItemTag == tag)
                        {
                            gm.playerList.PCList[cnt].OffHand = new Item();
                            gm.playerList.PCList[cnt].OffHand.ItemName = "";
                            FoundOne = true;
                        }
                        if (pc.MainHand.ItemTag == tag)
                        {
                            gm.playerList.PCList[cnt].MainHand = new Item();
                            gm.playerList.PCList[cnt].MainHand.ItemName = "";
                            FoundOne = true;
                        }
                        if (pc.Ring1.ItemTag == tag)
                        {
                            gm.playerList.PCList[cnt].Ring1 = new Item();
                            gm.playerList.PCList[cnt].Ring1.ItemName = "";
                            FoundOne = true;
                        }
                        if (pc.Ring2.ItemTag == tag) //SD_20131120
                        {
                            gm.playerList.PCList[cnt].Ring2 = new Item();
                            gm.playerList.PCList[cnt].Ring2.ItemName = "";
                            FoundOne = true;
                        }
                        if (pc.Feet.ItemTag == tag) //SD_20131120
                        {
                            gm.playerList.PCList[cnt].Feet = new Item();
                            gm.playerList.PCList[cnt].Feet.ItemName = "";
                            FoundOne = true;
                        }
                    }
                    cnt++;
                }
            }
        }
        public bool CheckForItem(string tag, int quantity)
        {
            //check if item is on any of the party members
            int numFound = 0;
            foreach (PC pc in gm.playerList.PCList)
            {
                if (pc.Body.ItemTag == tag) { numFound++; }
                if (pc.MainHand.ItemTag == tag) { numFound++; }
                if (pc.Ring1.ItemTag == tag) { numFound++; }
                if (pc.OffHand.ItemTag == tag) { numFound++; }
                if (pc.Head.ItemTag == tag) { numFound++; } //SD_20131120
                if (pc.Neck.ItemTag == tag) { numFound++; } //SD_20131120
                if (pc.Ring2.ItemTag == tag) { numFound++; } //SD_20131120
                if (pc.Feet.ItemTag == tag) { numFound++; } //SD_20131120
            }
            foreach (Item item in gm.partyInventoryList)
            {
                if (item.ItemTag == tag)
                    numFound++;
            }
            if (numFound >= quantity)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CheckIsRace(int PCIndex, string tag)
        {
            if (gm.playerList.PCList[PCIndex].Race.RaceTag == tag)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CheckIsGender(int PCIndex, string tag)
        {
            if (gm.playerList.PCList[PCIndex].Gender.ToString() == tag)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CheckIsMale(int PCIndex)
        {
            if (gm.playerList.PCList[PCIndex].Gender == CharBase.gender.Male)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CheckIsClassLevel(int PCIndex, string tag, int level)
        {
            if (gm.playerList.PCList[PCIndex].Class.PlayerClassTag == tag)
            {
                if (gm.playerList.PCList[PCIndex].ClassLevel >= level)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public bool CheckFunds(int amount)
        {
            if (gm.partyGold >= amount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CheckAttribute(int PCIndex, string attribute, string compare, int value)
        {
            gm.playerList.PCList[PCIndex].UpdateStats(this);
            int pcAttValue = 0;
            if (attribute == "str")
            {
                pcAttValue = gm.playerList.PCList[PCIndex].Strength;
            }
            else if (attribute == "dex")
            {
                pcAttValue = gm.playerList.PCList[PCIndex].Dexterity;
            }
            else if (attribute == "con")
            {
                pcAttValue = gm.playerList.PCList[PCIndex].Constitution;
            }
            else if (attribute == "int")
            {
                pcAttValue = gm.playerList.PCList[PCIndex].Intelligence;
            }
            else if (attribute == "wis")
            {
                pcAttValue = gm.playerList.PCList[PCIndex].Wisdom;
            }
            else if (attribute == "cha")
            {
                pcAttValue = gm.playerList.PCList[PCIndex].Charisma;
            }
            else
            {
                frm.logText("Failed to recognize attribute: " + attribute, Color.Yellow);
                frm.logText(Environment.NewLine, Color.GreenYellow);
                return false;
            }
            
            if (compare == "=")
            {
                if (pcAttValue == value)
                {
                    if (frm.debugMode) //SD_20131102
                    {
                        frm.logText("pcAttValue: " + pcAttValue + " == " + value.ToString(), Color.Yellow);
                        frm.logText(Environment.NewLine, Color.GreenYellow);
                    }
                    return true;
                }
            }
            else if (compare == ">")
            {
                if (pcAttValue > value)
                {
                    if (frm.debugMode) //SD_20131102
                    {
                        frm.logText("pcAttValue: " + pcAttValue + " > " + value.ToString(), Color.Yellow);
                        frm.logText(Environment.NewLine, Color.GreenYellow);
                    }
                    return true;
                }
            }
            else if (compare == "<")
            {
                if (pcAttValue < value)
                {
                    if (frm.debugMode) //SD_20131102
                    {
                        frm.logText("pcAttValue: " + pcAttValue + " < " + value.ToString(), Color.Yellow);
                        frm.logText(Environment.NewLine, Color.GreenYellow);
                    }
                    return true;
                }
            }
            else if (compare == "!")
            {
                if (pcAttValue != value)
                {
                    if (frm.debugMode) //SD_20131102
                    {
                        frm.logText("pcAttValue: " + pcAttValue + " != " + value.ToString(), Color.Yellow);
                        frm.logText(Environment.NewLine, Color.GreenYellow);
                    }
                    return true;
                }
            }
            return false;
        }
        public bool CheckIsAlignmnet(int PCIndex, int GoodEvil, int LawChaos) //SD_20131116
        {
            PC pc = gm.playerList.PCList[PCIndex];
            if ((GoodEvil == 0) && (LawChaos != 0))
            {
                if ((pc.AlignLawChaos == CharBase.AlignmentLawChaos.Lawful) && (LawChaos == 1)) { return true; }
                if ((pc.AlignLawChaos == CharBase.AlignmentLawChaos.Neutral) && (LawChaos == 2)) { return true; }
                if ((pc.AlignLawChaos == CharBase.AlignmentLawChaos.Chaotic) && (LawChaos == 3)) { return true; }
                return false;
            }
            else if ((GoodEvil != 0) && (LawChaos == 0))
            {
                if ((pc.AlignGoodEvil == CharBase.AlignmentGoodEvil.Good) && (GoodEvil == 1)) { return true; }
                if ((pc.AlignGoodEvil == CharBase.AlignmentGoodEvil.Neutral) && (GoodEvil == 2)) { return true; }
                if ((pc.AlignGoodEvil == CharBase.AlignmentGoodEvil.Evil) && (GoodEvil == 3)) { return true; }
                return false;
            }
            else if ((GoodEvil != 0) && (LawChaos != 0))
            {
                if ((pc.AlignLawChaos == CharBase.AlignmentLawChaos.Lawful) && (LawChaos == 1))
                {
                    if ((pc.AlignGoodEvil == CharBase.AlignmentGoodEvil.Good) && (GoodEvil == 1)) { return true; }
                    if ((pc.AlignGoodEvil == CharBase.AlignmentGoodEvil.Neutral) && (GoodEvil == 2)) { return true; }
                    if ((pc.AlignGoodEvil == CharBase.AlignmentGoodEvil.Evil) && (GoodEvil == 3)) { return true; }
                    return false;
                }
                if ((pc.AlignLawChaos == CharBase.AlignmentLawChaos.Neutral) && (LawChaos == 2)) 
                {
                    if ((pc.AlignGoodEvil == CharBase.AlignmentGoodEvil.Good) && (GoodEvil == 1)) { return true; }
                    if ((pc.AlignGoodEvil == CharBase.AlignmentGoodEvil.Neutral) && (GoodEvil == 2)) { return true; }
                    if ((pc.AlignGoodEvil == CharBase.AlignmentGoodEvil.Evil) && (GoodEvil == 3)) { return true; }
                    return false;
                }
                if ((pc.AlignLawChaos == CharBase.AlignmentLawChaos.Chaotic) && (LawChaos == 3))
                {
                    if ((pc.AlignGoodEvil == CharBase.AlignmentGoodEvil.Good) && (GoodEvil == 1)) { return true; }
                    if ((pc.AlignGoodEvil == CharBase.AlignmentGoodEvil.Neutral) && (GoodEvil == 2)) { return true; }
                    if ((pc.AlignGoodEvil == CharBase.AlignmentGoodEvil.Evil) && (GoodEvil == 3)) { return true; }
                    return false;
                }
            }
            else
            {
                return false;
            }
            return false;
        }
        public void AddJournalEntry(string categoryTag, int entryIndex)
        {
            JournalCategory jcm = gm.module.ModuleJournal.getJournalCategoryByTag(categoryTag);
            if (jcm != null)
            {
                JournalCategory jcp = gm.partyJournalQuests.getJournalCategoryByTag(categoryTag);
                if (jcp != null) //an existing category, just add entry
                {
                    if ((entryIndex >= 0) && (entryIndex < jcm.Entries.Count))
                    {
                        JournalEntry jem = jcm.Entries[entryIndex];
                        jcp.Entries.Add(jem);
                        IBMessageBox.Show(gm, "Your journal has been updated with: " + jem.EntryTitle);
                        if (jem.EndPoint)
                        {
                            gm.partyJournalCompleted.categories.Add(jcp);
                            gm.partyJournalQuests.categories.Remove(jcp);
                        }
                    }
                    else
                    {
                        IBMessageBox.Show(gm, "module's journal entry wasn't found based on index given");
                    }
                }
                else //a new category, add category and entry
                {
                    JournalCategory jcp2 = gm.module.ModuleJournal.getJournalCategoryByTag(categoryTag).DeepCopy();
                    //MessageBox.Show(gm, "player's journal category wasn't found based on tag given, creating category...");
                    if ((entryIndex >= 0) && (entryIndex < jcm.Entries.Count))
                    {
                        JournalEntry jem = jcm.Entries[entryIndex];
                        jcp2.Entries.Clear();
                        jcp2.Entries.Add(jem);
                        gm.partyJournalQuests.categories.Add(jcp2);
                        IBMessageBox.Show(gm, "Your journal has been updated with: " + jem.EntryTitle);
                        if (jem.EndPoint)
                        {
                            gm.partyJournalCompleted.categories.Add(jcp2);
                            gm.partyJournalQuests.categories.Remove(jcp2);
                        }
                    }
                    else
                    {
                        IBMessageBox.Show(gm, "module's journal entry wasn't found based on index given");
                    }
                }
            }
            else
            {
                IBMessageBox.Show(gm, "module's journal category wasn't found based on tag given");
            }
        }
        public void AddJournalEntry(string categoryTag, string entryTag)
        {
            JournalCategory jcm = gm.module.ModuleJournal.getJournalCategoryByTag(categoryTag);
            if (jcm != null)
            {
                JournalCategory jcp = gm.partyJournalQuests.getJournalCategoryByTag(categoryTag);
                if (jcp != null) //an existing category, just add entry
                {
                    JournalEntry jem = jcm.getJournalEntryByTag(entryTag);
                    if (jem != null)
                    {
                        jcp.Entries.Add(jem);
                        IBMessageBox.Show(gm, "Your journal has been updated with: " + jem.EntryTitle);
                        if (jem.EndPoint)
                        {
                            gm.partyJournalCompleted.categories.Add(jcp);
                            gm.partyJournalQuests.categories.Remove(jcp);
                        }
                    }
                    else
                    {
                        IBMessageBox.Show(gm, "module's journal entry wasn't found based on tag given");
                    }
                }
                else //a new category, add category and entry
                {
                    JournalCategory jcp2 = gm.module.ModuleJournal.getJournalCategoryByTag(categoryTag).DeepCopy();
                    //MessageBox.Show("player's journal category wasn't found based on tag given, creating category...");
                    JournalEntry jem = jcm.getJournalEntryByTag(entryTag);
                    if (jem != null)
                    {
                        jcp2.Entries.Clear();
                        jcp2.Entries.Add(jem);
                        gm.partyJournalQuests.categories.Add(jcp2);
                        IBMessageBox.Show(gm, "Your journal has been updated with: " + jem.EntryTitle);
                        if (jem.EndPoint)
                        {
                            gm.partyJournalCompleted.categories.Add(jcp2);
                            gm.partyJournalQuests.categories.Remove(jcp2);
                        }
                    }
                    else
                    {
                        IBMessageBox.Show(gm, "module's journal entry wasn't found based on tag given");
                    }
                }
            }
            else
            {
                IBMessageBox.Show(gm, "module's journal category wasn't found based on tag given");
            }
        }
        public void PlaySoundFX(string soundFileNameWithExtension)//, bool looping)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            try
            {
                player.SoundLocation = gm.mainDirectory + "\\modules\\" + gm.module.ModuleFolderName + "\\sounds\\soundFX\\" + soundFileNameWithExtension;
            }
            catch { }
            //if (looping)
            	//player.PlayLooping();
            player.Play();
            //Thread.Sleep(300); //uncomment this delay function if the sound doesn't play because the Dispose() kills it too soon.
            //player.Dispose(); //if you do not want to have the Thread.Sleep delay which freezes everything, maybe commnet out this player.Dispose() function and let the garbage collector clean it up later
        }
        public void PlaySoundFX(string soundFileNameWithExtension, int delay)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            try
            {
                player.SoundLocation = gm.mainDirectory + "\\modules\\" + gm.module.ModuleFolderName + "\\sounds\\soundFX\\" + soundFileNameWithExtension;
                player.Play();
            	Thread.Sleep(delay);
				//player.Dispose(); //if you do not want to have the Thread.Sleep delay which freezes everything, maybe commnet out this player.Dispose() function and let the garbage collector clean it up later            	
            }
            catch { }            
        }
        public void OpenShop(string shopTag)
        {
            frm.doShopBasedOnTag(shopTag);
        }
        #endregion
        
        #region AUTHOR FUNCTIONS
        //place any methods/functions here that are overloads or new functions.
        //
		// ______________________
		// might return null or ""
        public string GetLocalString(string objectTag, string variableName)
        {
            //check creatures, PCs, Props, areas, items
            #region Search PCs
            foreach (PC pc in gm.playerList.PCList)
            {
                if (pc.Tag == objectTag)
                {
                    foreach (LocalString variable in pc.CharLocalStrings)
                    {
                        if (variable.Key == variableName)
                        {
                            return variable.Value;
                        }
                    }
                    	IBMessageBox.Show(gm, "Found the object, but couldn't find the tag specified...returning a value of -1");
                    return null;
                }
            }
            #endregion
            foreach (Area a in gm.module.ModuleAreasObjects)
            {
                #region Area itself
                if (a.AreaFileName == objectTag)
                {
                    foreach (LocalString variable in a.AreaLocalStrings)
                    {
                        if (variable.Key == variableName)
                        {
                            return variable.Value;
                        }
                    }
                    if (frm.debugMode)
                    	IBMessageBox.Show(gm, "Found the object, but couldn't find the tag specified...returning a value of -1");
                    return null;
                }
                #endregion
                #region Creatures
                foreach (Creature cr in a.AreaCreatureList.creatures)
                {
                    if (cr.Tag == objectTag)
                    {
                        foreach (LocalString variable in cr.CharLocalStrings)
                        {
                            if (variable.Key == variableName)
                            {
                                return variable.Value;
                            }
                        }
                    	if (frm.debugMode)
                    		IBMessageBox.Show(gm, "Found the object, but couldn't find the tag specified...returning a value of -1");
                        return null;
                    }
                }
                #endregion
                #region Props
                foreach (Prop pr in a.AreaPropList.propsList)
                {
                    if (pr.PropTag == objectTag)
                    {
                        foreach (LocalString variable in pr.PropLocalStrings)
                        {
                            if (variable.Key == variableName)
                            {
                                return variable.Value;
                            }
                        }
                   		if (frm.debugMode)
                    		IBMessageBox.Show(gm, "Found the object, but couldn't find the tag specified...returning a value of -1");
                        return null;
                    }
                }
                #endregion
            }
            foreach (Encounter e in gm.module.ModuleEncountersList.encounters)
            {
                #region Creatures
                foreach (Creature cr in e.EncounterCreatureList.creatures)
                {
                    if (cr.Tag == objectTag)
                    {
                        foreach (LocalString variable in cr.CharLocalStrings)
                        {
                            if (variable.Key == variableName)
                            {
                                return variable.Value;
                            }
                        }
                    	if (frm.debugMode)
                    		IBMessageBox.Show(gm, "Found the object, but couldn't find the tag specified...returning a value of -1");
                        return null;
                    }
                }
                #endregion
                #region Props
                foreach (Prop pr in e.EncounterPropList.propsList)
                {
                    if (pr.PropTag == objectTag)
                    {
                        foreach (LocalString variable in pr.PropLocalStrings)
                        {
                            if (variable.Key == variableName)
                            {
                                return variable.Value;
                            }
                        }
                    	if (frm.debugMode)
                    		IBMessageBox.Show(gm, "Found the object, but couldn't find the tag specified...returning a value of -1");
                        return null;
                    }
                }
                #endregion
            }
            if (frm.debugMode) //SD_20131102
            {
                IBMessageBox.Show(gm, "couldn't find the object with the tag specified (only PCs, Creatures, Areas and Props), returning a value of -1");
            }
            return null;
        }
        //
        //
        public void SetLocalString(string objectTag, string variableName, string value)
        {
            //check creatures, PCs, Props, areas, items
            #region Search PCs
            foreach (PC pc in gm.playerList.PCList)
            {
                if (pc.Tag == objectTag)
                {
                    int exists = 0;
                    foreach (LocalString variable in pc.CharLocalStrings)
                    {
                        if (variable.Key == variableName)
                        {
                            variable.Value = value;
                            exists = 1;
                        }
                    }
                    if (exists == 0)
                    {
                        LocalString newLocalString = new LocalString();
                        newLocalString.Key = variableName;
                        newLocalString.Value = value;
                        pc.CharLocalStrings.Add(newLocalString);
                    }
                    return;
                }
            }
            #endregion
            foreach (Area a in gm.module.ModuleAreasObjects)
            {
                #region Area itself
                if (a.AreaFileName == objectTag)
                {
                    int exists = 0;
                    foreach (LocalString variable in a.AreaLocalStrings)
                    {
                        if (variable.Key == variableName)
                        {
                            variable.Value = value;
                            exists = 1;
                        }
                    }
                    if (exists == 0)
                    {
                        LocalString newLocalString = new LocalString();
                        newLocalString.Key = variableName;
                        newLocalString.Value = value;
                        a.AreaLocalStrings.Add(newLocalString);
                    }
                    return;
                }
                #endregion
                #region Creatures
                foreach (Creature cr in a.AreaCreatureList.creatures)
                {
                    if (cr.Tag == objectTag)
                    {
                        int exists = 0;
                        foreach (LocalString variable in cr.CharLocalStrings)
                        {
                            if (variable.Key == variableName)
                            {
                                variable.Value = value;
                                exists = 1;
                            }
                        }
                        if (exists == 0)
                        {
                            LocalString newLocalString = new LocalString();
                            newLocalString.Key = variableName;
                            newLocalString.Value = value;
                            cr.CharLocalStrings.Add(newLocalString);
                        }
                        return;
                    }
                }
                #endregion
                #region Props
                foreach (Prop pr in a.AreaPropList.propsList)
                {
                    if (pr.PropTag == objectTag)
                    {
                        int exists = 0;
                        foreach (LocalString variable in pr.PropLocalStrings)
                        {
                            if (variable.Key == variableName)
                            {
                                variable.Value = value;
                                exists = 1;
                            }
                        }
                        if (exists == 0)
                        {
                            LocalString newLocalString = new LocalString();
                            newLocalString.Key = variableName;
                            newLocalString.Value = value;
                            pr.PropLocalStrings.Add(newLocalString);
                        }
                        return;
                    }
                }
                #endregion
            }
            foreach (Encounter e in gm.module.ModuleEncountersList.encounters)
            {
                #region Creatures
                foreach (Creature cr in e.EncounterCreatureList.creatures)
                {
                    if (cr.Tag == objectTag)
                    {
                        int exists = 0;
                        foreach (LocalString variable in cr.CharLocalStrings)
                        {
                            if (variable.Key == variableName)
                            {
                                variable.Value = value;
                                exists = 1;
                            }
                        }
                        if (exists == 0)
                        {
                            LocalString newLocalString = new LocalString();
                            newLocalString.Key = variableName;
                            newLocalString.Value = value;
                            cr.CharLocalStrings.Add(newLocalString);
                        }
                        return;
                    }
                }
                #endregion
                #region Props
                foreach (Prop pr in e.EncounterPropList.propsList)
                {
                    if (pr.PropTag == objectTag)
                    {
                        int exists = 0;
                        foreach (LocalString variable in pr.PropLocalStrings)
                        {
                            if (variable.Key == variableName)
                            {
                                variable.Value = value;
                                exists = 1;
                            }
                        }
                        if (exists == 0)
                        {
                            LocalString newLocalString = new LocalString();
                            newLocalString.Key = variableName;
                            newLocalString.Value = value;
                            pr.PropLocalStrings.Add(newLocalString);
                        }
                        return;
                    }
                }
                #endregion
            }
            if (frm.debugMode) //SD_20131102
            {
                IBMessageBox.Show(gm, "couldn't find the object with the tag (tag: " + objectTag + ") specified (only PCs, Creatures, Areas and Props)");
            }
        }        
	//
	// ______________________
	public void WriteToLog(string text, Color color)
	{
		Form1 f = null;
		Combat c = null;
		
		if (MainMapScriptCall)
			f = frm;
		else
			c = frm.currentCombat;
		
		if (f != null)
			f.logText(text, color);
		else if (c != null)
			c.logText(text, color);
	}
		
	//
	// ______________________
        public Creature GetActionCreatureData()
        {
                Creature source = null;
                PC PCsource = null;
                if (MainMapScriptCall)
                {
                	PCsource = (PC)MainMapSource;
                }
                else
                {
                  if (CombatSource is PC)
                    PCsource = (PC)CombatSource;
                  else if (CombatSource is Creature)           
                    source = (Creature)CombatSource;
                }
                if (PCsource != null)
                {             
                   source = new Creature();
                   source.Name = PCsource.Name;
                   source.ClassLevel = PCsource.ClassLevel;
				   // * stats for spells DC                   
                   source.Intelligence = PCsource.Intelligence;
                   source.Wisdom = PCsource.Wisdom;
                   source.Charisma = PCsource.Charisma;
                   source.CombatLocation = PCsource.CombatLocation;
                   // * other vars might be required later
                }
                return source;
    	}
        //
        // ______________________
        // * get the owner of an effect (or for a spell with a single target)
        public object GetSourceCreatureObject()
        {
        	if (MainMapScriptCall)
        	{
        		return MainMapSource;
        	}
        	else
        		return CombatSource;
        }
        //
		// ______________________
		public void DoDeathScript(object crt)
		{
			object temp = CombatSource;
			// * sinopip, 20.12.14
        	System.Media.SoundPlayer player = new System.Media.SoundPlayer();
        	//
            if (crt is PC)
            {
				if (((PC)crt).HP <= 0 && GetLocalInt(((PC)crt).Tag, "HasDied") != 1)
                {
                    CombatSource = crt;
                    var scriptCrtDth = ((PC)crt).OnDeath;
                    frm.doScriptBasedOnFilename(scriptCrtDth.FilenameOrTag, scriptCrtDth.Parm1, scriptCrtDth.Parm2, scriptCrtDth.Parm3, scriptCrtDth.Parm4);
                    // * sinopip, 25.12.14
                    if (((PC)crt).CharSprite.DeathNumberOfFrames > 1)
                    	for (int index = 0; index < gm.playerList.PCList.Count; index++)
                    		if (gm.playerList.PCList[index].NameWithNotes == ((PC)crt).NameWithNotes)
                    			deathPCAnimation(((PC)crt), index);
                    //
                    SetLocalInt(((PC)crt).Tag, "HasDied", 1);
                }
            }
			else if (crt is Creature)
            {
                if (((Creature)crt).HP <= 0 && GetLocalInt(((Creature)crt).Tag, "HasDied") != 1)
                {
                    CombatSource = crt;
                    var scriptCrtDth = ((Creature)crt).OnDeath;
                    frm.doScriptBasedOnFilename(scriptCrtDth.FilenameOrTag, scriptCrtDth.Parm1, scriptCrtDth.Parm2, scriptCrtDth.Parm3, scriptCrtDth.Parm4);
                    // * sinopip, 20.12.14
        			try {
                    	player.SoundLocation = gm.mainDirectory + "\\modules\\" + gm.module.ModuleFolderName + "\\sounds\\soundFX\\" + ((Creature)crt).OnDeathSound;
        			} catch { }
		            player.Play();
		            frm.doScriptBasedOnFilename(scriptCrtDth.FilenameOrTag, scriptCrtDth.Parm1, scriptCrtDth.Parm2, scriptCrtDth.Parm3, scriptCrtDth.Parm4);
                    // * sinopip, 25.12.14
		            // * default death animation, or from spritsheet
		            if (((Creature)crt).CharSprite.DeathNumberOfFrames <= 1)
		            	frm.currentCombat.drawEndEffect(((Creature)crt).CombatLocation, 0, "generic_death.spt"); // if file doesn't exists, this does nothing
		            else
		            	deathCreatureAnimation(((Creature)crt));
		            Thread.Sleep(100);
                    //                
                    SetLocalInt(((Creature)crt).Tag, "HasDied", 1);
                }
            }
			CombatSource = temp;
		}
        // * sinopip, 25.12.14
        public void deathPCAnimation(PC pc, int PcIndex)
        {
            int deathRowIndex = 3;
            int sleep = 1000 / pc.CharSprite.DeathFPS;
            //start a for loop based on the number of frames in the attack row
            for (int x = 0; x < pc.CharSprite.DeathNumberOfFrames; x++)
            {
                gm.CombatAreaPcAnimateRenderAll(PcIndex, x, deathRowIndex);
                Thread.Sleep(sleep);
            }
        }
        //
        // * sinopip, 25.12.15
        public void deathCreatureAnimation(Creature crt)
        {
            int deathRowIndex = 3;
            int sleep = 1000 / crt.CharSprite.DeathFPS;
            //start a for loop based on the number of frames in the attack row
            for (int x = 0; x < crt.CharSprite.DeathNumberOfFrames; x++)
            {
                gm.CombatAreaCreatureAnimateRenderAll(crt, x, deathRowIndex);
                Thread.Sleep(sleep);
            }
        }
        //  
        
        //
		// ______________________
		// for a spell cast in combat with one or more targets
        // * produces list of objects (will need to be cast to use either as PC or as Creature)
        public List<object> GetAllCombatTargets(string targets_type)
        {
        	List<object> creatures_list = new List<object>();
        	Combat c = frm.currentCombat;
        	Point target;
        	if (c.currentSpell.AoeRadiusOrLength > 0)
        		target = (Point)CombatTarget;
        	else
        	{
        		creatures_list.Add(CombatTarget);
        		return creatures_list;
        	}
        	// Encounter Creatures
			foreach (Creature crt in c.com_encounter.EncounterCreatureList.creatures)
            {
                // if in range of radius of x and radius of y
                if ((crt.CombatLocation.X >= target.X - c.currentSpell.AoeRadiusOrLength) && (crt.CombatLocation.X <= target.X + c.currentSpell.AoeRadiusOrLength))
                    if ((crt.CombatLocation.Y >= target.Y - c.currentSpell.AoeRadiusOrLength) && (crt.CombatLocation.Y <= target.Y + c.currentSpell.AoeRadiusOrLength))
                		if (targets_type == "any" 
                		    || (targets_type == "enemies" && CombatSource is PC)
                		    || (targets_type == "allies" && CombatSource is Creature))
                		creatures_list.Add(crt);
			}
			// PCs
            foreach (PC pc in gm.playerList.PCList)
            {
                // if in range of radius of x and radius of y
                if ((pc.CombatLocation.X >= target.X - c.currentSpell.AoeRadiusOrLength) && (pc.CombatLocation.X <= target.X + c.currentSpell.AoeRadiusOrLength))
                    if ((pc.CombatLocation.Y >= target.Y - c.currentSpell.AoeRadiusOrLength) && (pc.CombatLocation.Y <= target.Y + c.currentSpell.AoeRadiusOrLength))
                		if (targets_type == "any"
                		    || (targets_type == "enemies" && CombatSource is Creature)
                		    || (targets_type == "allies" && CombatSource is PC))
                		creatures_list.Add(pc);
            }
            return creatures_list;
        }
        //
	// ______________________
        public int GetSpellDC(SpellParameters sp)
        {
        	int dc = sp.BaseDC;
        	if (sp.BaseDC > 0)
        	{
	        	Creature source = GetActionCreatureData();
	        	if (sp.StatMod.ToLower() == "int")
	        		dc += source.Intelligence/2 - 5;        		
	        	else if (sp.StatMod.ToLower() == "wis")
	        		dc += source.Wisdom/2 - 5;
	        	else if (sp.StatMod.ToLower() == "cha")
	        		dc += source.Charisma/2 - 5;        	
	        	// * might add extra dc with high levels...
        	}
        	return dc;
        }
        //
	// ______________________
        public SpecialActionResult RollVsDC(object target, SpellParameters sp)
        {
        	//if (target == null) return;
			// * check for a valid data object
        	PC pc = null;
        	Creature creature = null;        	
        	if (target is PC)
        		pc = (PC)target;
        	else if (target is Creature)
        		creature = (Creature)target;
        	else return null;
        	
        	SpecialActionResult car = new SpecialActionResult();
        	// * to get the DC, param1 of spell script is checked
        	car.DC = GetSpellDC(sp);
        	car.Roll = gm.Random(20);
        	if (sp.Save=="Fortitude")
        	{
        		if (creature != null)
        			car.RollMod = creature.Fortitude; 
        		else
        			car.RollMod = pc.Fortitude;
        	}
        	else if (sp.Save=="Reflex")
        	{
        		if (creature != null)
        			car.RollMod = creature.Reflex; 
        		else
        			car.RollMod = pc.Reflex;
        	}
        	if (sp.Save=="Will")
        	{
        		if (creature != null)
        			car.RollMod = creature.Will; 
        		else
        			car.RollMod = pc.Will;
        	}
        	if (car.Roll+car.RollMod >= car.DC && car.DC > 0)
        		car.SavedAgainstSuccessfully = true;
        	else
        		car.SavedAgainstSuccessfully = false;
        	// * compute score of dice, in case of damage or other effect with a value
	          int score = 0;	          
	          for (int i=0; i < sp.NbDice; i++)
	          	score += gm.Random(sp.Die);
	          score += sp.DiceAdd;
	          car.Score = score;
	          car.ScoreFinal = score;		          
          	if (sp.EnergySource != "")
		  	{
     			float resist = 1f;
				if (pc != null)        		
				switch(sp.EnergySource.ToLower())
		  		{
				  	case "acid" :
				  	  resist = (float)(1f - ((float)pc.DamageTypeResistanceTotalAcid / 100f));
				  	  break;
					case "bludgeoning" :        			  		
				  	  resist = (float)(1f - ((float)pc.DamageTypeResistanceTotalBludgeoning / 100f));
				  	  break;
					case "cold" :        			  		
				  	  resist = (float)(1f - ((float)pc.DamageTypeResistanceTotalCold / 100f));
				  	  break;
					case "electricity" :        			  		
				  	  resist = (float)(1f - ((float)pc.DamageTypeResistanceTotalElectricity / 100f));
				  	  break;
					case "fire" :        			  		
				  	  resist = (float)(1f - ((float)pc.DamageTypeResistanceTotalFire / 100f));
				  	  break;
					case "light" :        			  		
				  	  resist = (float)(1f - ((float)pc.DamageTypeResistanceTotalLight / 100f));
				  	  break;
					case "magic" :        			  		
				  	  resist = (float)(1f - ((float)pc.DamageTypeResistanceTotalMagic / 100f));
				  	  break;
					case "piercing" :        			  		
				  	  resist = (float)(1f - ((float)pc.DamageTypeResistanceTotalPiercing / 100f));
				  	  break;
					case "poison" :        			  		
				  	  resist = (float)(1f - ((float)pc.DamageTypeResistanceTotalPoison / 100f));
				  	  break;
					case "slashing" :        			  		
				  	  resist = (float)(1f - ((float)pc.DamageTypeResistanceTotalSlashing / 100f));
				  	  break;
				  	case "sonic" :
				  	  resist = (float)(1f - ((float)pc.DamageTypeResistanceTotalSonic / 100f));
				  	  break;
				  	default : break;
				}
				else if (creature != null)
				switch(sp.EnergySource.ToLower())
		  		{
				  	case "acid" :
				  	  resist = (float)(1f - ((float)creature.DamageTypeResistanceTotalAcid / 100f));
				  	  break;
					case "bludgeoning" :        			  		
				  	  resist = (float)(1f - ((float)creature.DamageTypeResistanceTotalBludgeoning / 100f));
				  	  break;
					case "cold" :        			  		
				  	  resist = (float)(1f - ((float)creature.DamageTypeResistanceTotalCold / 100f));
				  	  break;
					case "electricity" :        			  		
				  	  resist = (float)(1f - ((float)creature.DamageTypeResistanceTotalElectricity / 100f));
				  	  break;
					case "fire" :        			  		
				  	  resist = (float)(1f - ((float)creature.DamageTypeResistanceTotalFire / 100f));
				  	  break;
					case "light" :        			  		
				  	  resist = (float)(1f - ((float)creature.DamageTypeResistanceTotalLight / 100f));
				  	  break;
					case "magic" :        			  		
				  	  resist = (float)(1f - ((float)creature.DamageTypeResistanceTotalMagic / 100f));
				  	  break;
					case "piercing" :        			  		
				  	  resist = (float)(1f - ((float)creature.DamageTypeResistanceTotalPiercing / 100f));
				  	  break;
					case "poison" :        			  		
				  	  resist = (float)(1f - ((float)creature.DamageTypeResistanceTotalPoison / 100f));
				  	  break;
					case "slashing" :        			  		
				  	  resist = (float)(1f - ((float)creature.DamageTypeResistanceTotalSlashing / 100f));
				  	  break;
				  	case "sonic" :
				  	  resist = (float)(1f - ((float)creature.DamageTypeResistanceTotalSonic / 100f));
				  	  break;
				  	default : break;
				}					
				if (resist < 0.0f)
					resist = 0.0f;
				if (resist < 1.0f)
					WriteToLog("(with damage resistance : "+(1.0f-resist)*100+"%)", Color.Chartreuse);
				car.ScoreFinal = (int)(resist * score);
        	}
          return car;
        }
	//
	// ______________________
        public void DoHeal(SpellParameters sp, SpecialActionResult spell_result, object target)
        {
        	PC pc = null;
        	Creature creature = null;        	
        	if (target is PC)
        		pc = (PC)target;
        	else if (target is Creature)
        		creature = (Creature)target;
        	else return;

			if (pc != null)
			{
			  //if (pc.Status != CharBase.charStatus.Dead || sp.Effect.EffectName.Contains("Revive"))
			  if (pc.HP > -20 || sp.Effect.EffectName.Contains("Revive"))
			  {
			    // * special revive effect on the dead
			    if (sp.Effect != null)
			      if (sp.Effect.EffectName.Contains("Revive") && pc.Status == CharBase.charStatus.Dead)
			      {
					pc.Status = CharBase.charStatus.Alive;
					pc.HP = 1;
					WriteToLog(pc.Name+" is revived!",Color.Gold);
					SetLocalInt(pc.Tag, "HasDied", 0);
			      }
			    if (pc.HP > -20)
			    {
				    // * standard heal effect	
				    WriteToLog(pc.Name+" ",Color.Blue);        					
				    WriteToLog("heals ", Color.Gray);
				    WriteToLog(Math.Min(spell_result.ScoreFinal,pc.HPMax-pc.HP) +" ", Color.Lime);
				    WriteToLog("hit points",Color.Gray);
				    pc.HP = Math.Min(pc.HPMax , pc.HP + spell_result.ScoreFinal);
	               	DrawCombatFloatyTextOverSquare(
	                	"+"+spell_result.ScoreFinal,
	                	pc.CombatLocation.X, pc.CombatLocation.Y,
	                	4,
	                	Color.Green, Color.Lime);
					// * only for PCs
	                if ((pc.HP > 0) && (pc.Status == CharBase.charStatus.Dead))
	                {
	                  pc.Status = CharBase.charStatus.Alive;
	                }
				}
			    else
			    	WriteToLog("Cannot heal "+pc.Name+" with this spell.", Color.Gray);
			  }
			}
			else if (creature != null)
			  //if (creature.Status != CharBase.charStatus.Dead || sp.Effect.EffectName.Contains("Revive"))
				//if (creature.HP > 0 || sp.Effect.EffectName.Contains("Revive"))
			  {
			    // * special revive effect on the dead
			    if (sp.Effect != null)
			    if (sp.Effect.EffectName.Contains("Revive") && creature.Status == CharBase.charStatus.Dead)
			    {
					creature.Status = CharBase.charStatus.Alive;
					creature.HP = 1;
					WriteToLog(creature.Name+" is revived!",Color.DarkGreen);
					SetLocalInt(creature.Tag, "HasDied", 0);
			    }
			    // * standard heal effect	
			    WriteToLog(creature.Name+" ",Color.WhiteSmoke);        					
			    WriteToLog("heals ", Color.Gray);
			    WriteToLog(Math.Min(spell_result.ScoreFinal,creature.HPMax-creature.HP) +" ", Color.Green);
			    WriteToLog("hit points",Color.Gray);
			    creature.HP = Math.Min(creature.HPMax , creature.HP + spell_result.ScoreFinal);
               	DrawCombatFloatyTextOverSquare(
                	"+"+spell_result.ScoreFinal,
                	creature.CombatLocation.X, creature.CombatLocation.Y,
                	4,
                	Color.Green, Color.Lime);
				//c.Refresh();        				
			}        	
        }     
	//
	// ______________________
        public void DoDamage(SpellParameters sp, SpecialActionResult spell_result, object target)
        {
			// * check for a valid data object
        	PC pc = null;
        	Creature creature = null;        	
        	if (target is PC)
        		pc = (PC)target;
        	else if (target is Creature)
        		creature = (Creature)target;
        	else return;

        	Combat c = frm.currentCombat;
        	// * successful save
			if (spell_result.SavedAgainstSuccessfully)
			{
				//loglines.AddRange(sp.SuccessSave_Text);
    			// * resist damage
    			spell_result.ScoreFinal = (int)(spell_result.ScoreFinal*(1.0-sp.SuccessSaveResistance));
    			// * if Trait "Evasion", cancels damage of spell with Reflex Save...
    			// * ...
    		}
			// * failed save, also applies secondary effect
    		else 
    		{
    			if (sp.Effect != null)
    			{
    				Effect effect = sp.Effect.DeepCopy();
    				//effect.CurrentDurationInUnits = 0;
    				if (pc != null)
    				{
    					if (sp.Effect.EffectName.Contains("Death"))
    				    	pc.HP = -20;
    				  	else	
    						pc.AddEffectByObject (effect);
    				}
    				else if (creature != null)
    					if (sp.Effect.EffectName.Contains("Death"))
    				    	creature.HP = 0;
    				  	else	
	    					creature.AddEffectByObject(effect);
    			}
    		}
    		// * animation on hit (could it be only on failed save?)
   			/*if (sp.SpriteFileName != "") // anim only on failed save?
    			if (pc != null && pc.HP > -20)
    				frm.currentCombat.drawEndEffect(pc.CombatLocation, 0, sp.SpriteFileName);
   				else if (creature != null && creature.HP > 0)
    				frm.currentCombat.drawEndEffect(creature.CombatLocation, 0, sp.SpriteFileName);*/
    		// subtract HP and description in comlog
    		if (pc != null)
    		{
				if (pc.HP > -20)
				{        
			  		if (sp.BaseDC >= 0)
					{
				    	WriteToLog("("+pc.Name+" ",Color.Blue);        					
				        WriteToLog("rolls "+spell_result.Roll+" +"+spell_result.RollMod+" vs DC "+spell_result.DC+" : ",Color.Gray);
				    	if (spell_result.SavedAgainstSuccessfully)
					  		WriteToLog("success",Color.LightGray);
				    	else
					  	WriteToLog("failure",Color.Red);
				    	WriteToLog(") ", Color.Gray);
				    }
			  		WriteToLog(pc.Name+" "+sp.Description+" : ", Color.Gray);
			  		WriteToLog(spell_result.ScoreFinal+" ", Color.Crimson);
			  		WriteToLog("points of damage ",Color.Gray);
			  
		      		pc.HP -= spell_result.ScoreFinal;        				
              		DrawCombatFloatyTextOverSquare(
                		(-spell_result.ScoreFinal).ToString(),
	                	pc.CombatLocation.X,pc.CombatLocation.Y,
                		4,
                		Color.Red, Color.Crimson);
		      		// * effect description            			  
			  		if (!spell_result.SavedAgainstSuccessfully)
						if (sp.Effect != null)
			      			if (!sp.Effect.EffectName.Contains("Special Effect"))
			        			if (sp.EffectDescription != null)
				      				WriteToLog(pc.Name+" "+sp.EffectDescription, Color.Orange);
				    			else
				      				WriteToLog("Effect "+ sp.Effect.EffectName+" on "+ pc.Name, Color.Orange);
			  
              /*if (pc.HP <= 0 && pc.HP > -20)
              {
               	c.logText(Environment.NewLine, Color.Black);
                c.logText(GetActionCreatureData().Name + " knocked out " + pc.Name + " unconscious...", Color.Orange);
                c.logText(Environment.NewLine, Color.Black);
                //pc.Status = CharBase.charStatus.Dead;
                //pc.CharSprite.Image = new Bitmap(gm.mainDirectory + "\\data\\rip.png");
                c.refreshMap();
              }*/
              //else if (pc.HP <= -20)
              // ...
            	}
				if (pc.HP <= 0)
		        {
		            pc.Status = CharBase.charStatus.Dead;
		           	WriteToLog(Environment.NewLine, Color.Black);
		            WriteToLog(GetActionCreatureData().Name + " killed " + pc.Name, Color.LightGreen);
		            DoDeathScript(pc);
		            pc.CharSprite.Image = new Bitmap(gm.mainDirectory + "\\data\\rip.png");
		        }
    		}
			if (creature != null)
			{
				if (creature.HP > 0)
				{
				  if (sp.BaseDC >= 0)
				  {
					  WriteToLog("("+creature.Name+" ",Color.WhiteSmoke);        					
				      WriteToLog("rolls "+spell_result.Roll+" +"+spell_result.RollMod+" vs DC "+spell_result.DC+" : ",Color.Gray);
					  if (spell_result.SavedAgainstSuccessfully)
						WriteToLog("success",Color.LightGray);
					  else
						WriteToLog("failure",Color.Red);
					  WriteToLog(") ", Color.Gray);
				  }
				  WriteToLog(creature.Name+" "+sp.Description+" : ", Color.Gray);
				  WriteToLog(spell_result.ScoreFinal+" ", Color.Crimson);
				  WriteToLog("points of damage",Color.Gray);        					
				  creature.HP -= spell_result.ScoreFinal;
				  DrawCombatFloatyTextOverSquare(
	                		(-spell_result.ScoreFinal).ToString(),
	                		creature.CombatLocation.X,creature.CombatLocation.Y,
	                		4,
	                		Color.Red,Color.Crimson);
			      // * effect description            			  
				  if (!spell_result.SavedAgainstSuccessfully)
					if (sp.Effect != null)
				      if (!sp.Effect.EffectName.Contains("Special Effect"))
				        if (sp.EffectDescription != null)
					      WriteToLog(creature.Name+" "+sp.EffectDescription, Color.Orange);
					    else
					      WriteToLog("Effect "+ sp.Effect.EffectName+" on "+ creature.Name, Color.Orange);
              
				}
				if (creature != null && creature.HP <= 0)
            	{
	            	creature.Status = CharBase.charStatus.Dead;
	                WriteToLog(GetActionCreatureData().Name + " killed the " + creature.Name, Color.LightGreen);
	                DoDeathScript(creature);
	                //creature.CharSprite.Image = new Bitmap(gm.mainDirectory + "\\data\\rip.png");
	            }	
			}
			//WriteToLog(Environment.NewLine, Color.Black);
        }
	//
	// ______________________
        public void DoBuff(SpellParameters sp, SpecialActionResult spell_result, object target)
        {
			// * check for a valid data object
        	PC pc = null;
        	Creature creature = null;        	
        	if (target is PC)
        		pc = (PC)target;
        	else if (target is Creature)
        		creature = (Creature)target;
        	else return;
        	
    		if (sp.Effect != null)
    		{
   		     	Effect buff = sp.Effect.DeepCopy();
   		     	//buff.CurrentDurationInUnits = 0;
   		     	if (spell_result.ScoreFinal > 0)
    				buff.DurationInUnits = spell_result.ScoreFinal * 6;
    			if (sp.Effect.EffectName.Contains("Restore"))
        		{
        		  // * go through all debuff effects and remove them
        		  // * ...
        		}
    			if (pc != null)
    				pc.AddEffectByObject(buff);
    			else if (creature != null)
    				creature.AddEffectByObject(buff);
			    // * effect description  
			    if (pc != null)
				    if (sp.EffectDescription != null)
				    	WriteToLog(pc.Name+" "+sp.EffectDescription, Color.Orange);
					else
						WriteToLog("Effect "+ sp.Effect.EffectName+" on "+ pc.Name, Color.Orange);
			    else if (creature != null)
				    if (sp.EffectDescription != null)
				    	WriteToLog(creature.Name+" "+sp.EffectDescription, Color.Orange);
					else
						WriteToLog("Effect "+ sp.Effect.EffectName+" on "+ creature.Name, Color.Orange);
				if (sp.NbDice > 0 || sp.DiceAdd > 0)
				{
					
					WriteToLog(" for ", Color.Orange);
					WriteToLog(spell_result.ScoreFinal.ToString(), Color.Green);
					WriteToLog(" rounds", Color.Orange);
					WriteToLog(Environment.NewLine, Color.Black);
				}			    
			}
        }
	//
	// ______________________
        public void DoDebuff(SpellParameters sp, SpecialActionResult spell_result, object target)
        {
			// * check for a valid data object
        	PC pc = null;
        	Creature creature = null;        	
        	if (target is PC)
        		pc = (PC)target;
        	else if (target is Creature)
        		creature = (Creature)target;
        	else return;

			if (sp.BaseDC > 0)
				if (pc != null)
				{
				    WriteToLog("("+pc.Name+" ",Color.Blue);        					
			        WriteToLog("rolls "+spell_result.Roll+" +"+spell_result.RollMod+" vs DC "+spell_result.DC+" : ",Color.Gray);
				    if (spell_result.SavedAgainstSuccessfully)
					  WriteToLog("success",Color.LightGray);
				    else
					  WriteToLog("failure",Color.Red);
				    WriteToLog(") ", Color.Gray);
				}        	
				else if (creature != null)
				{
				    WriteToLog("("+creature.Name+" ",Color.Blue);        					
			        WriteToLog("rolls "+spell_result.Roll+" +"+spell_result.RollMod+" vs DC "+spell_result.DC+" : ",Color.Gray);
				    if (spell_result.SavedAgainstSuccessfully)
					  WriteToLog("success",Color.LightGray);
				    else
					  WriteToLog("failure",Color.Red);
				    WriteToLog(") ", Color.Gray);
				}    
    		if (!spell_result.SavedAgainstSuccessfully)
    		{
    			if (sp.Effect != null)
    			{
    				Effect debuff = sp.Effect.DeepCopy();
    				//debuff.CurrentDurationInUnits = 0;
    				if (spell_result.ScoreFinal > 0)
    					debuff.DurationInUnits = spell_result.ScoreFinal * 6;
    				if (pc != null)
    				{
    					pc.AddEffectByObject(debuff);
						if (!sp.Effect.EffectName.Contains("Special Effect")) // * !to implement!
			    		  if (sp.EffectDescription != null)
					    	WriteToLog(pc.Name+" "+sp.EffectDescription, Color.Orange);
					      else
						    WriteToLog("Effect "+ sp.Effect.EffectName+" on "+ pc.Name, Color.Orange);
						// * add status
						if (debuff.EffectCategory == "Hold")
							pc.Status = CharBase.charStatus.Held;
						if (debuff.EffectCategory == "Sleep") // ?
							pc.Status = CharBase.charStatus.Sleeping;
    				}
    				else if (creature != null)
    				{
    					creature.AddEffectByObject(debuff);
						if (!sp.Effect.EffectName.Contains("Special Effect")) // * !to implement!
			    		  if (sp.EffectDescription != null)
						    WriteToLog(creature.Name+" "+sp.EffectDescription, Color.Orange);
					      else
						    WriteToLog("Effect "+ sp.Effect.EffectName+" on "+ creature.Name, Color.Orange); 					
						// * add status
						if (debuff.EffectCategory == "Hold")
							creature.Status = CharBase.charStatus.Held;
						if (debuff.EffectCategory == "Sleep") // ?
							creature.Status = CharBase.charStatus.Sleeping;
    				}
    			}
        		//WriteToLog(Environment.NewLine, Color.Black);
    		}       	
        }
	//
	// ______________________
        public void DoCounter(SpellParameters sp, object target)
        {
			// * check for a valid data object
        	PC pc = null;
        	Creature creature = null;        	
        	if (target is PC)
        		pc = (PC)target;
        	else if (target is Creature)
        		creature = (Creature)target;
        	else return;

        	int i;
        	if (pc != null)
        	  for (i = pc.EffectsList.effectsList.Count; i > 0; i--)
        		if (sp.CountersEffects.Contains(pc.EffectsList.effectsList[i-1].EffectCategory))
        		{
        			WriteToLog(pc.Name, Color.Blue);
                	WriteToLog(" effect "+pc.EffectsList.effectsList[i-1].EffectName+" fades away.", Color.Black);
                	pc.EffectsList.effectsList.RemoveAt(i - 1);
        		}
        	else if (creature != null)
        	  for (i = creature.EffectsList.effectsList.Count; i > 0; i--)
        		if (sp.CountersEffects.Contains(creature.EffectsList.effectsList[i-1].EffectCategory))
        		{
        			WriteToLog(creature.Name, Color.WhiteSmoke);
                	WriteToLog(" effect "+creature.EffectsList.effectsList[i-1].EffectName+" fades away.", Color.Black);
                	creature.EffectsList.effectsList.RemoveAt(i - 1);
        		}   
        }
	//
	// ______________________
	// * target could be Point where summons will be in a radius of 1
    	public void DoSummon(SpellParameters sp, SpecialActionResult spell_result, object target)
        {        	
        	Creature summon = null;
        	int list_index = 0;
        	//CreatureRefs cr_ref = new CreatureRefs();
            	
        	// * select the summon from the list of sp.ExtraParams
        	if (sp.ExtraParams.Count > 0)
        	  list_index = gm.Random(sp.ExtraParams.Count/2)-1;
        	else
        	{
				WriteToLog("No creature to summoned", Color.Black);
        		WriteToLog(Environment.NewLine, Color.Black);
        		return;
        	}        	
        	if (list_index * 2 + 1 < sp.ExtraParams.Count)
        	{
        		summon = gm.module.ModuleCreaturesList.getCreatureByTag(sp.ExtraParams[list_index*2]);
        	}
        	if (summon == null)
        	{ 
        		WriteToLog("No creature is summoned", Color.Black);
        		WriteToLog(Environment.NewLine, Color.Black);
        		return;
        	}
        	int count;
        	try
        	{
           		count = int.Parse(sp.ExtraParams[list_index*2+1]);
        	}
        	catch 
        	{
        		if (frm.debugMode)
        		{
        			WriteToLog("Next parameter of summon is not a number", Color.Black);
        			WriteToLog(Environment.NewLine, Color.Black);
        		}
        		return;
        	}
           	// * add ResRefs to encounter and add creatures
        	int order = frm.currentCombat.currentMoveOrderIndex; 
        	Creature crt = new Creature();
        	CreatureRefs crt_ref = null;
        	for (int i=0; i < count; i++)
        	{
        		// * ResRef
           		crt_ref = new CreatureRefs();
            	crt_ref.CreatureResRef = summon.ResRef;
            	crt_ref.CreatureName = summon.Name + " Ally";
            	crt_ref.CreatureTag = summon.Tag+i;
 				//crt_ref.CreatureStartLocation = new Point(frm.currentEncounter.EncounterPcStartLocations[0].X + gm.Random(6)-3,
            	//                             frm.currentEncounter.EncounterPcStartLocations[0].Y + 5 + gm.Random(3)-2);
            	crt_ref.CreatureStartLocation = new Point(((Point)target).X, ((Point)target).Y+i);
            	gm.currentEncounter.EncounterCreatureRefsList.Add(crt_ref);
            	
            	// * creature
            	crt = summon.DeepCopy();
        		//crt.passRefs(gm, null);
        		crt.Tag = "Summoned "+summon.Name+" "+gm.currentEncounter.EncounterCreatureList.creatures.Count; // * need to check for further summonings!
        		crt.OnStartCombatTurn.FilenameOrTag = "crtPCAllyOnStartCombatTurn.cs";
            	crt.CombatLocation = crt_ref.CreatureStartLocation;
            	
            	gm.currentEncounter.EncounterCreatureList.creatures.Add(crt);
           		//gm.currentCombatArea.AreaCreatureList.creatures.Add(crt); // * adds duplicates of already present creatures??
            	MoveOrder mo = new MoveOrder();
            	mo.index = gm.currentEncounter.EncounterCreatureList.creatures.Count -1;
				mo.type = "";
                mo.tag = crt.Tag;
                mo.rank = gm.Random(10);// gm.Random(100) + (dexMod * 10) + Stats.CalcInitiativeBonuses(chr);
                frm.currentCombat.com_moveOrderList.Add(mo);
                //
                /*frm.currentCombat.drawEndEffect(crt.CombatLocation, 0, "on_effect.spt");
                frm.currentCombat.Refresh();
                Application.DoEvents();
            	Thread.Sleep(200);
                //*/
                //gm.RenderCreatureSpriteStatic(crt);
        	}
			WriteToLog(sp.ExtraParams[list_index*2+1]+" "+crt.Name+((count>1)?"s are":" is")+" summoned.", Color.YellowGreen);
        	//frm.currentCombat.com_moveOrderList = frm.currentCombat.com_moveOrderList.OrderByDescending(x => x.rank).ToList();
        	//frm.currentCombat.currentMoveOrderIndex = gm.currentEncounter.EncounterCreatureList.creatures.Count-count;
        }
        // ------------------------------------------
		//
		// ______________________
        public void DoSpellAction(SpellParameters sp, bool onMainMap)
        {
        	List<object> targets = new List<object>();
        	if (onMainMap)
        		targets.Add(MainMapTarget);
        	else
        		targets = GetAllCombatTargets(sp.TargetType);
        	SpecialActionResult spell_result;
			// ** do attack spell through all targets    	    
        	foreach (object target in targets)
        	{
				// * check for a valid data object
	        	PC pc = null;
	        	Creature creature = null;        	
	        	if (target is PC)
	        		pc = (PC)target;
	        	else if (target is Creature)
	        		creature = (Creature)target;
	        	else if (sp.Type == "Summon")
	        	{
    				DoSummon(sp, null, target);
    				return;	
	        	}
        		
	        	// ** spell result
        		spell_result = RollVsDC (target, sp);
        		if (spell_result == null) continue; // * ignore spell targetting errors        		
        		// * store the spell result on a target in a global varible, for external access
        		if (pc != null)
        			SetGlobalObject(pc.Tag+" last_spell_result", spell_result);
        		else if (creature != null)
        			SetGlobalObject(creature.Tag+" last_spell_result", spell_result);
        		
 				// * sounds and animations
 				if (sp.Type == "Damage" || !spell_result.SavedAgainstSuccessfully)
 				{
	    			if (sp.SoundFX != "")
	    				PlaySoundFX(sp.SoundFX);
	    			if (sp.SpriteFileName != "")
	    				if (pc != null && pc.HP > -20)
	    					frm.currentCombat.drawEndEffect(pc.CombatLocation, 0, sp.SpriteFileName);
	    			  	else if (creature != null && creature.HP > 0)
	    					frm.currentCombat.drawEndEffect(creature.CombatLocation, 0, sp.SpriteFileName);
 				}
 				
    			// "sp.Type" of spell
    			// * heal
    			// * possible extra : "Revive" in Effect's Tag variable -> make alive from dead
    			if (sp.Type == "Heal")
    				DoHeal(sp, spell_result, target);
    			// * damage
    			// * possible extra : "Death" in Effect's Tag variable -> instant death
    			else if (sp.Type == "Damage")
    				DoDamage(sp, spell_result, target);
    			// * buff
    			// * (todo) possible extra : "Restore" in Effect's Tag variable -> cancel debuffs 
    			else if (sp.Type == "Buff")
    				DoBuff(sp, spell_result, target);
    			// * debuff
    			else if (sp.Type == "Debuff")
    				DoDebuff(sp, spell_result, target);
    			if (sp.CountersEffects != "")
    				DoCounter(sp, target);
        	}
        }	
	//
	// ______________________
        public void DoSpell(SpellParameters sp)
        {        	
        	WriteToLog(GetActionCreatureData().Name, Color.LightBlue);
        	WriteToLog(" casts ",Color.Black);
        	WriteToLog(sp.Name,sp.SpellColor);
        	WriteToLog(Environment.NewLine, Color.Black);
   	
        	if (MainMapScriptCall) 
        	{
        		DoSpellAction(sp, true);
        		frm.Refresh();
        		frm.logText(Environment.NewLine, Color.Black);
        	}
        	else
        	{
        		DoSpellAction(sp, false);
        		frm.currentCombat.logText(Environment.NewLine, Color.Black);
        		frm.currentCombat.Refresh();
        	}
        	WriteToLog(Environment.NewLine, Color.Black); 
        }
        #endregion
        
        #region ALLIES
        //
        // * sinopip, 16.08.14
        // * all from here
        public Creature TargetCreatureWithLowestHP(bool ignoreIfInStealthMode)
        {
            Creature target = null;
            int lowHP = 999;
            List<Creature> creatures = gm.module.ModuleEncountersList.
            				getEncounter(frm.currentCombat.com_encounter.EncounterName).
            				EncounterCreatureList.creatures;
            foreach (Creature crt in creatures)
            {
				// * sinopip, 16.08.14
                if (crt.Tag.ToLower().Contains("summoned") || crt.Tag.ToLower().Contains("ally"))
                	continue;

                if (ignoreIfInStealthMode)
                {
                    if ((crt.Status != PC.charStatus.Dead) && (!CheckLocalInt(crt.Tag, "StealthModeOn", "=", 1)))
                    {
                        if (crt.HP < lowHP)
                        {
                            lowHP = crt.HP;
                            target = crt;
                        }
                    }
                }
                else
                {
                    if (crt.Status != PC.charStatus.Dead)
                    {
                        if (crt.HP < lowHP)
                        {
                            lowHP = crt.HP;
                            target = crt;
                        }
                    }
                }
            }
            return target;
        }
        public Creature TargetClosestCreature(bool ignoreIfInStealthMode)
        {
            Creature crt = (Creature)CombatSource; //this is the creature that is calling this script
            Creature target = null;
            int farDist = 99;
            List<Creature> creatures = gm.module.ModuleEncountersList.
            				getEncounter(frm.currentCombat.com_encounter.EncounterName).
            				EncounterCreatureList.creatures;
            foreach (Creature crttarget in creatures)
            {
            	if (crttarget.CombatLocation == crt.CombatLocation) continue; // exclude self
            	// * sinopip, 16.08.14
                if (crt.Tag.ToLower().Contains("summoned") || crt.Tag.ToLower().Contains("ally"))
                	continue;
                if (ignoreIfInStealthMode)
                {
                    if ((crttarget.Status != PC.charStatus.Dead) && (!CheckLocalInt(crt.Tag, "StealthModeOn", "=", 1)))
                    {
                        int dist = CalcDistance(crt.CombatLocation, crttarget.CombatLocation);
                        if (dist < farDist)
                        {
                            farDist = dist;
                            target = crttarget;
                        }
                    }
                }
                else
                {
                    if (crttarget.Status != PC.charStatus.Dead)
                    {
                        int dist = CalcDistance(crt.CombatLocation, crttarget.CombatLocation);
                        if (dist < farDist)
                        {
                            farDist = dist;
                            target = crttarget;
                        }
                    }
                }
            }
            return target;
        }
        public Creature TargetClosestCreatureNotHeld(bool ignoreIfInStealthMode)
        {
            Creature crt = (Creature)CombatSource; //this is the creature that is calling this script
            Creature target = null;
            int farDist = 99;
            List<Creature> creatures = gm.module.ModuleEncountersList.
            				getEncounter(frm.currentCombat.com_encounter.EncounterName).
            				EncounterCreatureList.creatures;
            foreach (Creature crttarget in creatures)    
            {
            	// * sinopip, 16.08.14
                if (crt.Tag.ToLower().Contains("summoned") || crt.Tag.ToLower().Contains("ally"))
                	continue;

                if (ignoreIfInStealthMode)
                {
                    //ignore if held, dead, or in stealth mode
                    if ((crttarget.Status != PC.charStatus.Held) && (crttarget.Status != PC.charStatus.Dead) && (!CheckLocalInt(crttarget.Tag, "StealthModeOn", "=", 1)))
                    {
                        int dist = CalcDistance(crt.CombatLocation, crttarget.CombatLocation);
                        if (dist < farDist)
                        {
                            farDist = dist;
                            target = crttarget;
                        }
                    }
                }
                else
                {
                    //ignore if held or dead
                    if ((crttarget.Status != PC.charStatus.Held) && (crttarget.Status != PC.charStatus.Dead))
                    {
                        int dist = CalcDistance(crt.CombatLocation, crttarget.CombatLocation);
                        if (dist < farDist)
                        {
                            farDist = dist;
                            target = crttarget;
                        }
                    }
                }
            }
            return target;
        }
        public PC GetPCWithLowestHP()
        {
            int lowHP = 999;
            PC returnCrt = null;
            foreach (PC pc in gm.playerList.PCList)
            {
                if (pc.HP > 0)
                {
                    if (pc.HP < lowHP)
                    {
                        lowHP = pc.HP;
                        returnCrt = pc;
                    }
                }
            }
            return returnCrt;
        }
        public Creature GetNextAdjacentCreature(Creature crt)
        {
            foreach (Creature nextCrt in gm.currentEncounter.EncounterCreatureList.creatures)
            {
                if ((CalcDistance(crt.CombatLocation, nextCrt.CombatLocation) < 2) && (nextCrt.HP > 0))
                {
                    return nextCrt;
                }
            }
            return null;
        }    
        //
		// * sinopip, 16.08.14
		// * all from here
		// * many functions todos : take in account creature of size greater than 1x1
        public void setupPathfindArray(Creature c, Creature target)
        {
            for (int x = 0; x <= (gm.currentCombatArea.MapSizeInSquares.Width - 1); x++)
            {
                for (int y = 0; y <= (gm.currentCombatArea.MapSizeInSquares.Height - 1); y++)
                {
                    //logText("x=" + x.ToString() + " y=" + y.ToString(), Color.Black);
                    //logText(Environment.NewLine, Color.Black);
                    if (gm.currentCombatArea.getCombatTile(x, y).collidable == true)
                        pathfinder.Squares[x, y].ContentCode = SquareContent.Wall;
                    else
                        pathfinder.Squares[x, y].ContentCode = SquareContent.Empty;
                }
            }
            foreach (Creature crt in frm.currentEncounter.EncounterCreatureList.creatures)
            {
                if (crt == c) { continue; }
                if (target == c) continue;
                if (crt.HP > 0)
                {
                    //TODO if creature size is greater than 1, make all squares walls
                    if (crt.Size == 1)
                    {
                        pathfinder.Squares[crt.CombatLocation.X, crt.CombatLocation.Y].ContentCode = SquareContent.Wall;
                    }
                    if (crt.Size == 2)
                    {
                        pathfinder.Squares[crt.CombatLocation.X + 0, crt.CombatLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinder.Squares[crt.CombatLocation.X + 1, crt.CombatLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinder.Squares[crt.CombatLocation.X + 0, crt.CombatLocation.Y + 1].ContentCode = SquareContent.Wall;
                        pathfinder.Squares[crt.CombatLocation.X + 1, crt.CombatLocation.Y + 1].ContentCode = SquareContent.Wall;
                    }
                    if (crt.Size == 3)
                    {
                        pathfinder.Squares[crt.CombatLocation.X + 0, crt.CombatLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinder.Squares[crt.CombatLocation.X + 1, crt.CombatLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinder.Squares[crt.CombatLocation.X + 2, crt.CombatLocation.Y + 0].ContentCode = SquareContent.Wall;
                        pathfinder.Squares[crt.CombatLocation.X + 0, crt.CombatLocation.Y + 1].ContentCode = SquareContent.Wall;
                        pathfinder.Squares[crt.CombatLocation.X + 2, crt.CombatLocation.Y + 1].ContentCode = SquareContent.Wall;
                        pathfinder.Squares[crt.CombatLocation.X + 0, crt.CombatLocation.Y + 2].ContentCode = SquareContent.Wall;
                        pathfinder.Squares[crt.CombatLocation.X + 1, crt.CombatLocation.Y + 2].ContentCode = SquareContent.Wall;
                        pathfinder.Squares[crt.CombatLocation.X + 2, crt.CombatLocation.Y + 2].ContentCode = SquareContent.Wall;
                    }
                }
            }
            foreach (Prop prp in frm.currentEncounter.EncounterPropList.propsList)
            {
                if (prp.HasCollision)
                {
                    pathfinder.Squares[prp.Location.X, prp.Location.Y].ContentCode = SquareContent.Wall;
                }
            }
            foreach (PC pc in gm.playerList.PCList)
            {
                if (pc.HP > 0)
                {
                    pathfinder.Squares[pc.CombatLocation.X, pc.CombatLocation.Y].ContentCode = SquareContent.Wall;
                }
            }
        }     
        //
        public void setupMonsterSquares(Creature source_crt, Creature crt)
        {
            if (crt.Size == 1)
            {
                return;
            }
            if (crt.Size == 2)
            {
                for (int x = source_crt.CombatLocation.X - 1; x < source_crt.CombatLocation.X + 1; x++)
                {
                    for (int y = source_crt.CombatLocation.Y - 1; y < source_crt.CombatLocation.Y + 1; y++)
                    {
                        if (Pathfinder.ValidCoordinates(x, y, gm))
                        {
                            pathfinder.Squares[x, y].ContentCode = SquareContent.Monster;
                        }
                    }
                }
            }
            if (crt.Size == 3)
            {
                for (int x = source_crt.CombatLocation.X - 2; x < source_crt.CombatLocation.X + 1; x++)
                {
                    for (int y = source_crt.CombatLocation.Y - 2; y < source_crt.CombatLocation.Y + 1; y++)
                    {
                        if (Pathfinder.ValidCoordinates(x, y, gm))
                        {
                            pathfinder.Squares[x, y].ContentCode = SquareContent.Monster;
                        }
                    }
                }
            }
        }	
		//        
        public bool creatureWithinMeleeDistance(Creature crt, Creature crt_target)
        {
            if (crt.Size == 1)
            {
                if (CalcDistance(crt_target.CombatLocation, crt.CombatLocation) == 1)
                {
                    return true;
                }
            }
            else if (crt.Size == 2)
            {
                for (int x = crt_target.CombatLocation.X - 2; x <= crt_target.CombatLocation.X + 1; x++)
                {
                    for (int y = crt_target.CombatLocation.Y - 2; y <= crt_target.CombatLocation.Y + 1; y++)
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
                for (int x = crt_target.CombatLocation.X - 3; x <= crt_target.CombatLocation.X + 1; x++)
                {
                    for (int y = crt_target.CombatLocation.Y - 3; y <= crt_target.CombatLocation.Y + 1; y++)
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
		//        
        public void doCreatureCombatFacing(Creature crt_pt, Creature crt_target_pt)
        {
            if ((crt_target_pt.CombatLocation.X == crt_pt.CombatLocation.X) && (crt_target_pt.CombatLocation.Y > crt_pt.CombatLocation.Y))
            { crt_pt.CombatFacing = CharBase.facing.Down; }
            if ((crt_target_pt.CombatLocation.X > crt_pt.CombatLocation.X) && (crt_target_pt.CombatLocation.Y > crt_pt.CombatLocation.Y))
            { crt_pt.CombatFacing = CharBase.facing.DownRight; }
            if ((crt_target_pt.CombatLocation.X < crt_pt.CombatLocation.X) && (crt_target_pt.CombatLocation.Y > crt_pt.CombatLocation.Y))
            { crt_pt.CombatFacing = CharBase.facing.DownLeft; }
            if ((crt_target_pt.CombatLocation.X == crt_pt.CombatLocation.X) && (crt_target_pt.CombatLocation.Y < crt_pt.CombatLocation.Y))
            { crt_pt.CombatFacing = CharBase.facing.Up; }
            if ((crt_target_pt.CombatLocation.X > crt_pt.CombatLocation.X) && (crt_target_pt.CombatLocation.Y < crt_pt.CombatLocation.Y))
            { crt_pt.CombatFacing = CharBase.facing.UpRight; }
            if ((crt_target_pt.CombatLocation.X < crt_pt.CombatLocation.X) && (crt_target_pt.CombatLocation.Y < crt_pt.CombatLocation.Y))
            { crt_pt.CombatFacing = CharBase.facing.UpLeft; }
            if ((crt_target_pt.CombatLocation.X > crt_pt.CombatLocation.X) && (crt_target_pt.CombatLocation.Y == crt_pt.CombatLocation.Y))
            { crt_pt.CombatFacing = CharBase.facing.Right; }
            if ((crt_target_pt.CombatLocation.X < crt_pt.CombatLocation.X) && (crt_target_pt.CombatLocation.Y == crt_pt.CombatLocation.Y))
            { crt_pt.CombatFacing = CharBase.facing.Left; }
        }       
		//        
        public int CalcCreatureDamageToCreature(Creature crt_target, Creature crt) //SD_20131126
        {
            int armDamRed = 0;
            /*if (pc.Body != null)
            {
                armDamRed = pc.Body.ItemDamageReduction;
            }*/
            int dDam = crt.DamageDie;
            float damage = (crt.NumberOfDamageDice * gm.Random(dDam)) - armDamRed + crt.DamageAdder;
            if (damage < 0)
            {
                damage = 0;
            }

            float resist = 0;

            if (crt.TypeOfDamage == DamageType.Acid)
            {
                resist = (float)(1f - ((float)crt_target.DamageTypeResistanceTotalAcid / 100f));
            }
            else if (crt.TypeOfDamage == DamageType.Bludgeoning)
            {
                resist = (float)(1f - ((float)crt_target.DamageTypeResistanceTotalBludgeoning / 100f));
            }
            else if (crt.TypeOfDamage == DamageType.Cold)
            {
                resist = (float)(1f - ((float)crt_target.DamageTypeResistanceTotalCold / 100f));
            }
            else if (crt.TypeOfDamage == DamageType.Electricity)
            {
                resist = (float)(1f - ((float)crt_target.DamageTypeResistanceTotalElectricity / 100f));
            }
            else if (crt.TypeOfDamage == DamageType.Fire)
            {
                resist = (float)(1f - ((float)crt_target.DamageTypeResistanceTotalFire / 100f));
            }
            else if (crt.TypeOfDamage == DamageType.Light)
            {
                resist = (float)(1f - ((float)crt_target.DamageTypeResistanceTotalLight / 100f));
            }
            else if (crt.TypeOfDamage == DamageType.Magic)
            {
                resist = (float)(1f - ((float)crt_target.DamageTypeResistanceTotalMagic / 100f));
            }
            else if (crt.TypeOfDamage == DamageType.Piercing)
            {
                resist = (float)(1f - ((float)crt_target.DamageTypeResistanceTotalPiercing / 100f));
            }
            else if (crt.TypeOfDamage == DamageType.Poison)
            {
                resist = (float)(1f - ((float)crt_target.DamageTypeResistanceTotalPoison / 100f));
            }
            else if (crt.TypeOfDamage == DamageType.Slashing)
            {
                resist = (float)(1f - ((float)crt_target.DamageTypeResistanceTotalSlashing / 100f));
            }
            else if (crt.TypeOfDamage == DamageType.Sonic)
            {
                resist = (float)(1f - ((float)crt_target.DamageTypeResistanceTotalSonic / 100f));
            }

            int totalDam = (int)(damage * resist);
            if (totalDam < 0)
            {
                totalDam = 0;
            }

            return totalDam;
        }
        //
        public int CalcCreatureAttackModifier(Creature crt)
        {
            return crt.Attack;
        }
        //
        public void drawHitSymbolOnCreature(Creature crt)
        {
            gm.CombatAreaHitSymbolOnCreatureRenderAll(crt);
            //gm.drawHitSymbol(crt.CombatLocation.X, crt.CombatLocation.Y);
            //gm.UpdateCombat();
            //Application.DoEvents();
            Thread.Sleep(200);
            //frm.currentCombat.refreshMap();
        }        
        //
        public void CreatureDoesAttack(Creature crt_pt, Creature crt_target_pt)
        {
            crt_target_pt = (Creature)CombatTarget;
            // determine if ranged or melee
            if ((crt_pt.WeaponType == Creature.crCategory.Ranged) && (CalcDistance(crt_target_pt.CombatLocation, crt_pt.CombatLocation) <= crt_pt.AttackRange) && (frm.currentCombat.IsVisibleLineOfSight(crt_pt.CombatLocation, crt_target_pt.CombatLocation)))
            {
                Point starting = new Point((crt_pt.CombatLocation.X * gm._squareSize) + (gm._squareSize / 2), (crt_pt.CombatLocation.Y * gm._squareSize) + (gm._squareSize / 2));
                Point ending = new Point((crt_target_pt.CombatLocation.X * gm._squareSize) + (gm._squareSize / 2), (crt_target_pt.CombatLocation.Y * gm._squareSize) + (gm._squareSize / 2));
                frm.currentCombat.playCreatureAttackSound(crt_pt);
                frm.currentCombat.drawProjectile(starting, ending, crt_pt.ProjectileSpriteFilename);

                #region The actual attack portion
                if (crt_pt.HP > 0)
                {
                    #region OnAttack
                    // run OnStartCombatTurn script 
                    CombatTarget = crt_target_pt;
                    CombatSource = crt_pt;
                    var scriptCrt = crt_pt.OnAttack;
                    frm.doScriptBasedOnFilename(scriptCrt.FilenameOrTag, scriptCrt.Parm1, scriptCrt.Parm2, scriptCrt.Parm3, scriptCrt.Parm4);
                    #endregion
                    #region CreatureAttack
                    // run OnStartCombatTurn script 
                    CombatTarget = crt_target_pt;
                    CombatSource = crt_pt;
                    frm.doScriptBasedOnFilename("dsAttackCreature.cs", "none", "none", "none", "none");
                    #endregion
                }
                else
                {
                    frm.currentCombat.logText(crt_pt.Name, Color.LightGray);
                    frm.currentCombat.logText(" is unconscious...skips turn", Color.Black);
                    frm.currentCombat.logText(Environment.NewLine, Color.Black);
                    frm.currentCombat.logText(Environment.NewLine, Color.Black);
                }
                #endregion
            }
            else
            {
                setupPathfindArray(crt_pt, crt_target_pt);
                pathfinder.Squares[crt_pt.CombatLocation.X, crt_pt.CombatLocation.Y].ContentCode = SquareContent.Monster;
                pathfinder.Squares[crt_target_pt.CombatLocation.X, crt_target_pt.CombatLocation.Y].ContentCode = SquareContent.Hero;
                Recalculate(crt_pt);
                setupMonsterSquares(crt_target_pt, crt_pt);
                navigatePath(crt_pt);

                // if melee, try and move to attack or get close
                // check to see if selected PC is one square away, if so attack else skip
                //if (calcDistance(crt_target_pt.CombatLocation, crt_pt.CombatLocation) == 1)
                if (creatureWithinMeleeDistance(crt_pt, crt_target_pt))
                {
                    doCreatureCombatFacing(crt_pt, crt_target_pt);
                    #region The actual attack portion
                    if (crt_pt.HP > 0)
                    {
                        #region OnAttack
                        // run OnStartCombatTurn script 
                        CombatTarget = crt_target_pt;
                        CombatSource = crt_pt;
                        var scriptCrt = crt_pt.OnAttack;
                        frm.doScriptBasedOnFilename(scriptCrt.FilenameOrTag, scriptCrt.Parm1, scriptCrt.Parm2, scriptCrt.Parm3, scriptCrt.Parm4);
                        #endregion
                        #region CreatureAttack
                        // run OnStartCombatTurn script 
                        CombatTarget = crt_target_pt;
                        CombatSource = crt_pt;
                        frm.doScriptBasedOnFilename("dsAttackCreature.cs", "none", "none", "none", "none");
                        #endregion
                    }
                    else
                    {
                        frm.currentCombat.logText(crt_pt.Name, Color.LightGray);
                        frm.currentCombat.logText(" is unconscious...skips turn", Color.Black);
                        frm.currentCombat.logText(Environment.NewLine, Color.Black);
                        frm.currentCombat.logText(Environment.NewLine, Color.Black);
                    }
                    #endregion
                }
            }
        }		
        #endregion
 
    }
}

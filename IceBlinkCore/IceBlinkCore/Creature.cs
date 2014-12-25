using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using IceBlink;
using IceBlinkToolset;

namespace IceBlinkCore
{
    [Serializable]
    public class Creatures
    {
        [XmlArrayItem("Creatures")]
        public List<Creature> creatures = new List<Creature>();
        [XmlIgnore]
        public Game game;

        public Creatures()
        {
        }
        public void passRefs(Game g)
        {
            game = g;
        }
        public void saveCreaturesFile(string filename)
        {
            StreamWriter writer = null;
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(Creatures));
                writer = new StreamWriter(filename);
                ser.Serialize(writer, this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to save Creature file. Error: " + ex.Message);
                game.errorLog(ex.ToString());
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
                writer = null;
            }
        }
        public Creatures loadCreaturesFile(string filename)
        {
            Creatures toReturn = null;
            XmlSerializer serializer = new XmlSerializer(typeof(Creatures));
            FileStream myFileStream = null;
            try
            {
                myFileStream = new FileStream(filename, FileMode.Open);
                toReturn = (Creatures)serializer.Deserialize(myFileStream);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open xml Creature file. Error:\n{0}", ex.Message);
                game.errorLog(ex.ToString());
            }
            finally
            {
                if (myFileStream != null)
                {
                    myFileStream.Close();
                }
            }
            return toReturn;
        }
        public Creature getCreature(string name)
        {
            foreach (Creature cr in creatures)
            {
                if (cr.Name == name) return cr;
            }
            return null;
        }
        public Creature getCreatureByTag(string tag)
        {
            foreach (Creature crtag in creatures)
            {
                if (crtag.Tag == tag) return crtag;
            }
            return null;
        }
        public Creature getCreatureByResRef(string resref)
        {
            foreach (Creature crtag in creatures)
            {
                if (crtag.ResRef == resref) return crtag;
            }
            return null;
        }
    }

    [Serializable]
    public class Creature : CharBase
    {
        public enum crCategory
        {
            Ranged = 0,
            Melee = 1
        }
        public enum crProjectileImage
        {
            Arrow = 0,
            Bolt = 1,
            Stone = 2,
            Dart = 3,
            Dagger = 4
        }

        #region Fields
        private string cr_parentNodeName = "New Category"; // value type
        private string cr_desc = ""; //detailed description
        private int cr_XP = 10;
        private int cr_fortitude = 0;
        private int cr_will = 0;
        private int cr_reflex = 0;
        private int cr_damReduction = 0; //armor damage reduction
        private int cr_Att = 0;
        private int cr_AttRange = 1;
        private int cr_damageNumDice = 1; //number of dice to roll for damage
        private int cr_damageDie = 4; //type of dice to roll for damage
        private crCategory cr_category = crCategory.Melee; //catergory type (ranged, melee)
        private string cr_projSpriteFilename = "arrow.spt"; //sprite filename including .spt
        private string cr_attackSound = "none";
        // * sinopip, 20.12.14
        private string cr_onHitSound = "none";
        private string cr_onBeingHitSound = "none"; // not implemented yet
        private string cr_onDeathSound = "none";
        //
        private int cr_baseAttBonus = 1;        
        private int cr_baseAttBonusAdders = 0;
        private int cr_numberOfAttacks = 1;
        private int cr_moveDistance = 5;
        private int damageAdder = 0;
        private int initiativeModifier = 0;
        private int perceptionValue = 0;
        private int criticalHitRange = 20;
        private int criticalHitDamageMultiplier = 2;
        private AiBasicTactic cr_ai = AiBasicTactic.BasicAttacker;
        private ScriptSelectEditorReturnObject onScoringHit = new ScriptSelectEditorReturnObject();
        private int damageTypeResistanceValueBludgeoning = 0;
        private int damageTypeResistanceValuePiercing = 0;
        private int damageTypeResistanceValueSlashing = 0;
        private int damageTypeResistanceValueAcid = 0;
        private int damageTypeResistanceValueCold = 0;
        private int damageTypeResistanceValueElectricity = 0;
        private int damageTypeResistanceValueFire = 0;
        private int damageTypeResistanceValueLight = 0;
        private int damageTypeResistanceValueSonic = 0;
        private int damageTypeResistanceValueMagic = 0;
        private int damageTypeResistanceValuePoison = 0;
        private DamageType typeOfDamage = DamageType.Slashing;
        #endregion

        #region Properties
        [XmlElement]
        [CategoryAttribute("00 - Resistance Modifiers"), DescriptionAttribute("Damage resistance value (default is to use -100 to 100 as a percentage of immunity, damage multiplied by percentage so 0 = full damage, 100 = no damage, -100 = double damage)")]
        public int DamageTypeResistanceValuePoison
        {
            get { return damageTypeResistanceValuePoison; }
            set { damageTypeResistanceValuePoison = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Resistance Modifiers"), DescriptionAttribute("Damage resistance value (default is to use -100 to 100 as a percentage of immunity, damage multiplied by percentage so 0 = full damage, 100 = no damage, -100 = double damage)")]
        public int DamageTypeResistanceValueMagic
        {
            get { return damageTypeResistanceValueMagic; }
            set { damageTypeResistanceValueMagic = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Resistance Modifiers"), DescriptionAttribute("Damage resistance value (default is to use -100 to 100 as a percentage of immunity, damage multiplied by percentage so 0 = full damage, 100 = no damage, -100 = double damage)")]
        public int DamageTypeResistanceValueBludgeoning
        {
            get { return damageTypeResistanceValueBludgeoning; }
            set { damageTypeResistanceValueBludgeoning = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Resistance Modifiers"), DescriptionAttribute("Damage resistance value (default is to use -100 to 100 as a percentage of immunity, damage multiplied by percentage so 0 = full damage, 100 = no damage, -100 = double damage)")]
        public int DamageTypeResistanceValuePiercing
        {
            get { return damageTypeResistanceValuePiercing; }
            set { damageTypeResistanceValuePiercing = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Resistance Modifiers"), DescriptionAttribute("Damage resistance value (default is to use -100 to 100 as a percentage of immunity, damage multiplied by percentage so 0 = full damage, 100 = no damage, -100 = double damage)")]
        public int DamageTypeResistanceValueSlashing
        {
            get { return damageTypeResistanceValueSlashing; }
            set { damageTypeResistanceValueSlashing = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Resistance Modifiers"), DescriptionAttribute("Damage resistance value (default is to use -100 to 100 as a percentage of immunity, damage multiplied by percentage so 0 = full damage, 100 = no damage, -100 = double damage)")]
        public int DamageTypeResistanceValueAcid
        {
            get { return damageTypeResistanceValueAcid; }
            set { damageTypeResistanceValueAcid = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Resistance Modifiers"), DescriptionAttribute("Damage resistance value (default is to use -100 to 100 as a percentage of immunity, damage multiplied by percentage so 0 = full damage, 100 = no damage, -100 = double damage)")]
        public int DamageTypeResistanceValueCold
        {
            get { return damageTypeResistanceValueCold; }
            set { damageTypeResistanceValueCold = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Resistance Modifiers"), DescriptionAttribute("Damage resistance value (default is to use -100 to 100 as a percentage of immunity, damage multiplied by percentage so 0 = full damage, 100 = no damage, -100 = double damage)")]
        public int DamageTypeResistanceValueElectricity
        {
            get { return damageTypeResistanceValueElectricity; }
            set { damageTypeResistanceValueElectricity = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Resistance Modifiers"), DescriptionAttribute("Damage resistance value (default is to use -100 to 100 as a percentage of immunity, damage multiplied by percentage so 0 = full damage, 100 = no damage, -100 = double damage)")]
        public int DamageTypeResistanceValueFire
        {
            get { return damageTypeResistanceValueFire; }
            set { damageTypeResistanceValueFire = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Resistance Modifiers"), DescriptionAttribute("Damage resistance value (default is to use -100 to 100 as a percentage of immunity, damage multiplied by percentage so 0 = full damage, 100 = no damage, -100 = double damage)")]
        public int DamageTypeResistanceValueLight
        {
            get { return damageTypeResistanceValueLight; }
            set { damageTypeResistanceValueLight = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Resistance Modifiers"), DescriptionAttribute("Damage resistance value (default is to use -100 to 100 as a percentage of immunity, damage multiplied by percentage so 0 = full damage, 100 = no damage, -100 = double damage)")]
        public int DamageTypeResistanceValueSonic
        {
            get { return damageTypeResistanceValueSonic; }
            set { damageTypeResistanceValueSonic = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Modifiers"), DescriptionAttribute("The Type of Damage (useful with immunity checks)")]
        public DamageType TypeOfDamage
        {
            get { return typeOfDamage; }
            set { typeOfDamage = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Basic Creature - not implemented yet"), DescriptionAttribute("Base Attack Bonus")]
        public int CreatureBaseAttBonus
        {
            get { return this.cr_baseAttBonus; }
            set { this.cr_baseAttBonus = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Basic Creature - not implemented yet"), DescriptionAttribute("Base Attack Bonus total adders from Traits, effects, skills, etc.")]
        public int CreatureBaseAttBonusAdders
        {
            get { return this.cr_baseAttBonusAdders; }
            set { this.cr_baseAttBonusAdders = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Basic Creature"), DescriptionAttribute("Number of attacks per round on same target")]
        public int CreatureNumberOfAttacks
        {
            get { return this.cr_numberOfAttacks; }
            set { this.cr_numberOfAttacks = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Basic Creature"), DescriptionAttribute("Move distance in squares used for creatures in combat")]
        public int CreatureMoveDistance
        {
            get { return this.cr_moveDistance; }
            set { this.cr_moveDistance = value; }
        }
        [XmlElement]
        [CategoryAttribute("07 - Scripts"), DescriptionAttribute("fires when the creature makes a successful hit on a target")]
        [Editor(typeof(ScriptSelectEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ScriptSelectEditorReturnObject OnScoringHit
        {
            get { return onScoringHit; }
            set { onScoringHit = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Basic Creature"), DescriptionAttribute("damage adder")]
        public int DamageAdder
        {
            get { return this.damageAdder; }
            set { this.damageAdder = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Basic Creature"), DescriptionAttribute("initiative modifier")]
        public int InitiativeModifier
        {
            get { return this.initiativeModifier; }
            set { this.initiativeModifier = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Basic Creature - not implemented yet"), DescriptionAttribute("perception value")]
        public int PerceptionValue
        {
            get { return this.perceptionValue; }
            set { this.perceptionValue = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Basic Creature"), DescriptionAttribute("The range for a critical hit (for 19-20, enter 19)")]
        public int CriticalHitRange
        {
            get { return this.criticalHitRange; }
            set { this.criticalHitRange = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Basic Creature"), DescriptionAttribute("The damage multiplier for a successful critical hit")]
        public int CriticalHitDamageMultiplier
        {
            get { return this.criticalHitDamageMultiplier; }
            set { this.criticalHitDamageMultiplier = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Basic Creature"), DescriptionAttribute("Generic AI for the creature to use")]
        public AiBasicTactic CreatureAI
        {
            get { return this.cr_ai; }
            set { this.cr_ai = value; }
        }

        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Category that this creature belongs to")]
        public string CategoryName
        {
            get
            {
                return cr_parentNodeName;
            }
            set
            {
                cr_parentNodeName = value;
            }
        }

        [XmlElement]
        [Editor(typeof(MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Description of the creature")]
        public string Description
        {
            get
            {
                return cr_desc;
            }
            set
            {
                cr_desc = value;
            }
        }

        [XmlElement]
        [Browsable(true), TypeConverter(typeof(SpriteConverter))]
        [CategoryAttribute("00 - Basic Creature"), DescriptionAttribute("Filename of the Sprite to use for the creature's projectiles")]
        public string ProjectileSpriteFilename
        {
            get
            {
                return cr_projSpriteFilename;
            }
            set
            {
                cr_projSpriteFilename = value;
            }
        }
        
        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Experience Points for the creature")]
        public int RewardXP
        {
            get
            {
                return cr_XP;
            }
            set
            {
                cr_XP = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("00 - Basic Creature"), DescriptionAttribute("Fortitude value for Creatures (overrides Class assigned value)")]
        public int CreatureFortitude
        {
            get { return cr_fortitude; }
            set { cr_fortitude = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Basic Creature"), DescriptionAttribute("Will value for Creatures (overrides Class assigned value)")]
        public int CreatureWill
        {
            get { return cr_will; }
            set { cr_will = value; }
        }
        [XmlElement]
        [CategoryAttribute("00 - Basic Creature"), DescriptionAttribute("Reflex value for Creatures (overrides Class assigned value)")]
        public int CreatureReflex
        {
            get { return cr_reflex; }
            set { cr_reflex = value; }
        }

        [XmlElement]
        [CategoryAttribute("00 - Basic Creature"), DescriptionAttribute("Damage reduction of the creature")]
        public int DamageReduction
        {
            get
            {
                return cr_damReduction;
            }
            set
            {
                cr_damReduction = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("00 - Basic Creature"), DescriptionAttribute("Attack of the creature")]
        public int Attack
        {
            get
            {
                return cr_Att;
            }
            set
            {
                cr_Att = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("00 - Basic Creature"), DescriptionAttribute("Attack range of the creature measured in squares")]
        public int AttackRange
        {
            get
            {
                return cr_AttRange;
            }
            set
            {
                cr_AttRange = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("00 - Basic Creature"), DescriptionAttribute("Number of dice to roll for damage")]
        public int NumberOfDamageDice
        {
            get
            {
                return cr_damageNumDice;
            }
            set
            {
                cr_damageNumDice = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("00 - Basic Creature"), DescriptionAttribute("Die to roll for damage")]
        public int DamageDie
        {
            get
            {
                return cr_damageDie;
            }
            set
            {
                cr_damageDie = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("00 - Basic Creature"), DescriptionAttribute("Is weapon a melee or ranged weapon")]
        public crCategory WeaponType
        {
            get
            {
                return cr_category;
            }
            set
            {
                cr_category = value;
            }
        }

        [XmlElement]
        [Browsable(true), TypeConverter(typeof(SoundConverter))]
        [CategoryAttribute("00 - Basic Creature"), DescriptionAttribute("Filename of sound to play when the creature makes an attack (include extension)")]
        public string AttackSound
        {
            get { return cr_attackSound; }
            set { cr_attackSound = value; }
        }   
        
        // * sinopip, 20.12.14
        [XmlElement]
        [Browsable(true), TypeConverter(typeof(SoundConverter))]
        [CategoryAttribute("00 - Basic Creature"), DescriptionAttribute("Filename of sound to play when the creature hits with an attack (include extension)")]
        public string OnHitSound
        {
            get { return cr_onHitSound; }
            set { cr_onHitSound = value; }
        }             
        [XmlElement]
        [Browsable(true), TypeConverter(typeof(SoundConverter))]
        [CategoryAttribute("00 - Basic Creature"), DescriptionAttribute("Filename of sound to play when the creature dies (include extension)")]
        public string OnDeathSound
        {
            get { return cr_onDeathSound; }
            set { cr_onDeathSound = value; }
        }             
        //
        #endregion

        public Creature() : base()
        {
            this.OnScoringHit.FilenameOrTag = "none";
            this.OnAttack.FilenameOrTag = "crtOnAttack.cs";
            this.OnDeath.FilenameOrTag = "crtOnDeath.cs";
            this.OnEndCombatTurn.FilenameOrTag = "crtOnEndCombatTurn.cs";
            this.OnHeartBeat.FilenameOrTag = "crtOnHeartBeat.cs";
            this.OnHit.FilenameOrTag = "crtOnHit.cs";
            this.OnPerception.FilenameOrTag = "crtOnPerception.cs";
            this.OnStartCombatTurn.FilenameOrTag = "crtOnStartCombatTurn.cs";
        }
        public void passRefs(ParentForm pf)
        {
            this.OnScoringHit.prntForm = pf;
            this.OnAttack.prntForm = pf;
            this.OnDeath.prntForm = pf;
            this.OnEndCombatTurn.prntForm = pf;
            this.OnHeartBeat.prntForm = pf;
            this.OnHit.prntForm = pf;
            this.OnPerception.prntForm = pf;
            this.OnStartCombatTurn.prntForm = pf;
        }
        /*public void passRefs(Game g)
        {
            game = g;
        }*/
        /*public Bitmap LoadCreaturePortraitBitmap (string filename)
        {
            // Sets up an image object to be displayed.
            if (portraitBitmap != null)
            {
                portraitBitmap.Dispose();
            }
            portraitBitmap = new Bitmap(filename);
            return portraitBitmap;
        }*/
        /*public Bitmap LoadCreatureIconBitmap (string filename)
        {
            // Sets up an image object to be displayed.
            if (iconBitmap != null)
            {
                iconBitmap.Dispose();
            }
            iconBitmap = new Bitmap(filename);
            return iconBitmap;
        }*/
        public Creature ShallowCopy()
        {
            return (Creature)this.MemberwiseClone();
        }
        public Creature DeepCopy()
        {
            Creature other = (Creature)this.MemberwiseClone();
            other.CharSprite = this.CharSprite.DeepCopy();
            other.Race = this.Race.DeepCopy();
            other.Class = this.Class.DeepCopy();
            other.KnownSpellsList = new Spells();
            foreach (Spell s in this.KnownSpellsList.spellList)
            {
                Spell sp = s.DeepCopy();
                other.KnownSpellsList.spellList.Add(sp);
            }
            other.KnownTraitsList = new Traits();
            foreach (Trait s in this.KnownTraitsList.traitList)
            {
                Trait sp = s.DeepCopy();
                other.KnownTraitsList.traitList.Add(sp);
            }
            other.KnownSkillsList = new Skills();
            foreach (Skill s in this.KnownSkillsList.skillsList)
            {
                Skill sp = s.DeepCopy();
                other.KnownSkillsList.skillsList.Add(sp);
            }
            other.KnownSpellsTags = new List<string>();
            foreach (string s in this.KnownSpellsTags)
            {
                other.KnownSpellsTags.Add(s);
            }
            other.KnownTraitsTags = new List<string>();
            foreach (string s in this.KnownTraitsTags)
            {
                other.KnownTraitsTags.Add(s);
            }
            other.EffectsList = new Effects();
            other.Head = this.Head.DeepCopy();
            other.Neck = this.Neck.DeepCopy();
            other.Body = this.Body.DeepCopy();
            other.MainHand = this.MainHand.DeepCopy();
            other.OffHand = this.OffHand.DeepCopy();
            other.Ring1 = this.Ring1.DeepCopy();
            other.Ring2 = this.Ring2.DeepCopy();
            other.Feet = this.Feet.DeepCopy();
            other.OnScoringHit = this.OnScoringHit.DeepCopy();
            other.OnAttack = this.OnAttack.DeepCopy();
            other.OnDeath = this.OnDeath.DeepCopy();
            other.OnEndCombatTurn = this.OnEndCombatTurn.DeepCopy();
            other.OnHeartBeat = this.OnHeartBeat.DeepCopy();
            other.OnHit = this.OnHit.DeepCopy();
            other.OnPerception = this.OnPerception.DeepCopy();
            other.OnStartCombatTurn = this.OnStartCombatTurn.DeepCopy();
            other.CharLocalInts = new List<LocalInt>();
            foreach (LocalInt l in this.CharLocalInts)
            {
                LocalInt Lint = new LocalInt();
                Lint.Key = l.Key;
                Lint.Value = l.Value;
                other.CharLocalInts.Add(Lint);
            }
            other.CharLocalStrings = new List<LocalString>();
            foreach (LocalString l in this.CharLocalStrings)
            {
                LocalString Lstr = new LocalString();
                Lstr.Key = l.Key;
                Lstr.Value = l.Value;
                other.CharLocalStrings.Add(Lstr);
            }
            other.CharAdditionalPropertyInts = new List<LocalInt>();
            foreach (LocalInt l in this.CharAdditionalPropertyInts)
            {
                LocalInt Lint = new LocalInt();
                Lint.Key = l.Key;
                Lint.Value = l.Value;
                other.CharAdditionalPropertyInts.Add(Lint);
            }
            other.CharAdditionalPropertyStrings = new List<LocalString>();
            foreach (LocalString l in this.CharAdditionalPropertyStrings)
            {
                LocalString Lstr = new LocalString();
                Lstr.Key = l.Key;
                Lstr.Value = l.Value;
                other.CharAdditionalPropertyStrings.Add(Lstr);
            }
            return other;
        }
    }

    [Serializable]
    public class CreatureRefs
    {
        private string creatureName = "";
        private string creatureTag = "";
        private string creatureResRef = "";
        private string mouseOverText = "";
        private int creatureSize = 1;
        [XmlElement]
        [CategoryAttribute("01 - Main"), ReadOnly(true)]
        public string CreatureName
        {
            get { return creatureName; }
            set { creatureName = value; }
        }        
        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Tag of the creature reference (Must be unique)")]
        public string CreatureTag
        {
            get { return creatureTag; }
            set { creatureTag = value; }
        }        
        [XmlElement]
        [CategoryAttribute("01 - Main"), ReadOnly(true)]
        public string CreatureResRef
        {
            get { return creatureResRef; }
            set { creatureResRef = value; }
        }
        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("The text to display when player mouses over the NPC/Creature on the adventure maps")]
        public string MouseOverText
        {
            get { return mouseOverText; }
            set { mouseOverText = value; }
        }
        [XmlElement]
        [CategoryAttribute("01 - Main"), ReadOnly(true)]
        public int CreatureSize
        {
            get { return creatureSize; }
            set { creatureSize = value; }
        }        
        [XmlElement]
        [CategoryAttribute("01 - Main"), ReadOnly(true)]
        public Point CreatureStartLocation;

        public CreatureRefs()
        {
        }
        public CreatureRefs DeepCopy()
        {
            CreatureRefs other = (CreatureRefs)this.MemberwiseClone();
            return other;
        }
    }
}

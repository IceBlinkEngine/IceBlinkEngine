using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using IceBlink;
using IceBlinkToolset;

namespace IceBlinkCore
{
    [Serializable]
    public class Spells
    {
        [XmlIgnore]
        public Game game;
        [XmlElement]
        public List<Spell> spellList = new List<Spell>();

        public Spells()
        {
        }
        public void passRefs(Game g)
        {
            game = g;
        }
        public void saveSpellsFile(string filename)
        {
            StreamWriter writer = null;
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(Spells));
                writer = new StreamWriter(filename);
                ser.Serialize(writer, this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to save Spells file. Error: " + ex.Message);
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
        public Spells loadSpellsFile(string filename)
        {
            Spells toReturn = null;
            XmlSerializer serializer = new XmlSerializer(typeof(Spells));
            FileStream myFileStream = null;
            try
            {
                myFileStream = new FileStream(filename, FileMode.Open);
                toReturn = (Spells)serializer.Deserialize(myFileStream);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open xml Spells file. Error:\n{0}", ex.Message);
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
        public Spell getSpellByTag(string tag)
        {
            foreach (Spell s in spellList)
            {
                if (s.SpellTag == tag) return s;
            }
            return null;
        }
        public Spell getSpellByName(string name)
        {
            foreach (Spell s in spellList)
            {
                if (s.SpellName == name) return s;
            }
            return null;
        }
    }

    [Serializable]
    public class Spell
    {
        [XmlIgnore]
        public Game game;

        public enum AoEShape
        {
            Square = 0,
            Cone = 1,
            Line = 2
        }
        public enum EffectType
        {
            Damage = 0, //usually persistent and negative for effect target
            Heal = 1,   //usually persistent and positive for effect target
            Buff = 2,   //usually temporary and positive for effect target
            Debuff = 3  //usually temporary and negative for effect target
        }

        #region Fields        
        private string name = "newSpell"; //item name
        private string tag = "newSpellTag"; //item unique tag name
        private string description = "";
        private UsableInSituation useableInSituation = UsableInSituation.Always;
        private string spriteFilename = "heal.spt";
        private string spriteEndingFilename = "heal1x1End.spt";
        private string spellStartSound = "none";
        private string spellEndSound = "none";
        // * sinopip, 20.12.14
        private string spellIcon = "none";
        //
        private int costSP = 0;
        private bool targetIsPC = false;
        private bool targetIsSelf = false;
        private bool targetIsPointLocation = false;
        private TargetType spellTargetType = TargetType.Enemy;
        private EffectType spellEffectType = EffectType.Damage;
        private AoEShape aoeShape = AoEShape.Square;
        private int aoeRadiusOrLength = 0;
        private int range = 0;
        private int damageNumDice = 0; //number of dice to roll for damage
        private int damageDie = 0; //type of dice to roll for damage
        private int damageDieAdder = 0; //add to damage per die (2d4 + 1 where 1 is the adder)
        private int healNumDice = 0; //number of dice to roll for heal
        private int healDie = 0; //type of dice to roll for heal
        private int healDieAdder = 0; //add to heal per die (2d4 + 1 where 1 is the adder)
        private ScriptSelectEditorReturnObject spellScript = new ScriptSelectEditorReturnObject();
        #endregion

        #region Properties
        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Name of the Spell")]
        public string SpellName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Tag of the Spell (Must be unique)")]
        public string SpellTag
        {
            get
            {
                return tag;
            }
            set
            {
                tag = value;
            }
        }

        [XmlElement]
        [Editor(typeof(MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Detailed description of spell with some stats and cost as well")]
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("When can this be used: Always means that it can be used in combat and on the main maps, Passive means that it is always on and doesn't need to be activated.")]
        public UsableInSituation UseableInSituation
        {
            get { return useableInSituation; }
            set { useableInSituation = value; }
        }

        [XmlElement]
        [Browsable(true), TypeConverter(typeof(SpriteConverter))]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Sprite to use for the effect (Sprite Filename with extension)")]
        public string SpriteFilename
        {
            get
            {
                return spriteFilename;
            }
            set
            {
                spriteFilename = value;
            }
        }

        [XmlElement]
        [Browsable(true), TypeConverter(typeof(SpriteConverter))]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Sprite to use for the ending effect (Sprite Filename with extension)")]
        public string SpriteEndingFilename
        {
            get
            {
                return spriteEndingFilename;
            }
            set
            {
                spriteEndingFilename = value;
            }
        }

        [XmlElement]
        [Browsable(true), TypeConverter(typeof(SoundConverter))]
        [CategoryAttribute("01- Main"), DescriptionAttribute("Filename of sound to play when the spell starts (include extension)")]
        public string SpellStartSound
        {
            get { return spellStartSound; }
            set { spellStartSound = value; }
        }

        [XmlElement]
        [Browsable(true), TypeConverter(typeof(SoundConverter))]
        [CategoryAttribute("01- Main"), DescriptionAttribute("Filename of sound to play when the spell ends (include extension)")]
        public string SpellEndSound
        {
            get { return spellEndSound; }
            set { spellEndSound = value; }
        }

        // * sinopip, 20.12.14
        [XmlElement]
        //[Browsable(true), TypeConverter(typeof(SoundConverter))]
        [CategoryAttribute("01- Main"), DescriptionAttribute("Filename of icon displayed in spell selector (include extension)")]
        public string SpellIcon
        {
            get { return spellIcon; }
            set { spellIcon = value; }
        }
        
        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("How much SP this Spell costs")]
        public int CostSP
        {
            get
            {
                return costSP;
            }
            set
            {
                costSP = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("04 - No Longer Used"), DescriptionAttribute("No longer used. Use SpellTargetType instead")]
        public bool TargetIsPC
        {
            get
            {
                return targetIsPC;
            }
            set
            {
                targetIsPC = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("04 - No Longer Used"), DescriptionAttribute("No longer used. Use SpellTargetType instead")]
        public bool TargetIsSelf
        {
            get
            {
                return targetIsSelf;
            }
            set
            {
                targetIsSelf = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("04 - No Longer Used"), DescriptionAttribute("No longer used. Use SpellTargetType instead")]
        public bool TargetIsPointLocation
        {
            get
            {
                return targetIsPointLocation;
            }
            set
            {
                targetIsPointLocation = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("02 - Target"), DescriptionAttribute("The type of target for this spell")]
        public TargetType SpellTargetType
        {
            get
            {
                return spellTargetType;
            }
            set
            {
                spellTargetType = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("02 - Target"), DescriptionAttribute("the shape of the Area of Effect"), Browsable(false)]
        public AoEShape AoeShape
        {
            get
            {
                return aoeShape;
            }
            set
            {
                aoeShape = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("02 - Target"), DescriptionAttribute("the radius or length of the AoE")]
        public int AoeRadiusOrLength
        {
            get
            {
                return aoeRadiusOrLength;
            }
            set
            {
                aoeRadiusOrLength = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("02 - Target"), DescriptionAttribute("the range allowed to the center or beginning of the AoE")]
        public int Range
        {
            get
            {
                return range;
            }
            set
            {
                range = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("03 - Effect"), DescriptionAttribute("damage = persistent, negative; heal = persistent, positive; buff = temporary, positive; debuff = temporary, negative")]
        public EffectType SpellEffectType
        {
            get
            {
                return spellEffectType;
            }
            set
            {
                spellEffectType = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("03 - Effect"), DescriptionAttribute("number of dice to roll"), Browsable(false)]
        public int DamageNumDice
        {
            get
            {
                return damageNumDice;
            }
            set
            {
                damageNumDice = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("03 - Effect"), DescriptionAttribute("type of dice to roll"), Browsable(false)]
        public int DamageDie
        {
            get
            {
                return damageDie;
            }
            set
            {
                damageDie = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("03 - Effect"), DescriptionAttribute("add to roll per die (ex. 2d4 + 1 where 1 is the adder)"), Browsable(false)]
        public int DamageDieAdder
        {
            get
            {
                return damageDieAdder;
            }
            set
            {
                damageDieAdder = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("03 - Effect"), DescriptionAttribute("number of dice to roll"), Browsable(false)]
        public int HealNumDice
        {
            get
            {
                return healNumDice;
            }
            set
            {
                healNumDice = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("03 - Effect"), DescriptionAttribute("type of dice to roll"), Browsable(false)]
        public int HealDie
        {
            get
            {
                return healDie;
            }
            set
            {
                healDie = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("03 - Effect"), DescriptionAttribute("add to roll per die (ex. 2d4 + 1 where 1 is the adder)"), Browsable(false)]
        public int HealDieAdder
        {
            get
            {
                return healDieAdder;
            }
            set
            {
                healDieAdder = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("the script to use for this Spell")]
        [Editor(typeof(ScriptSelectEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ScriptSelectEditorReturnObject SpellScript
        {
            get { return spellScript; }
            set { spellScript = value; }
        }
        #endregion

        public Spell()
        {            
        }
        public void passRefs(Game g, ParentForm pf)
        {
            game = g;
            SpellScript.prntForm = pf;
        }
        public override string ToString()
        {
            return SpellName;
        }
        public Spell ShallowCopy()
        {
            return (Spell)this.MemberwiseClone();
        }
        public Spell DeepCopy()
        {
            Spell other = (Spell)this.MemberwiseClone();
            other.SpellScript = this.SpellScript.DeepCopy();
            return other;
        }
    }
}

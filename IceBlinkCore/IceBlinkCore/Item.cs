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
using System.Drawing.Design;
using System.Design;
using System.ComponentModel.Design;

namespace IceBlinkCore
{

    [Serializable]
    public class Items
    {
        [XmlIgnore]
        public Game game;

        [XmlArrayItem("ItemsList")]
        public List<Item> itemsList = new List<Item>();

        public Items()
        {
        }
        public void passRefs(Game g)
        {
            game = g;
        }
        public void saveItemsFile(string filename)
        {
            StreamWriter writer = null;
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(Items));
                writer = new StreamWriter(filename);
                ser.Serialize(writer, this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to save Items file. Error: " + ex.Message);
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
        public Items loadItemsFile(string filename)
        {
            Items toReturn = null;
            XmlSerializer serializer = new XmlSerializer(typeof(Items));
            FileStream myFileStream = null;
            try
            {
                myFileStream = new FileStream(filename, FileMode.Open);
                toReturn = (Items)serializer.Deserialize(myFileStream);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open xml Items file. Error:\n{0}", ex.Message);
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
        public Item getItem(string name)
        {
            foreach (Item it in itemsList)
            {
                if (it.ItemName == name) return it;
            }
            return null;
        }
        public Item getItemByTag(string tag)
        {
            foreach (Item it in itemsList)
            {
                if (it.ItemTag == tag) return it;
            }
            return null;
        }
    }

    [Serializable]
    public class Item : INotifyPropertyChanged
    {
        public enum category
        {
            Armor = 0,
            Ranged = 1,
            Melee = 2,
            General = 3,
            Ring = 4,
            Shield = 5,
            Boots = 6,
            Head = 7,
            Neck = 8
        }
        public enum projectileImage
        {
            Arrow = 0,
            Bolt = 1,
            Stone = 2,
            Dart = 3,
            Dagger = 4,
            Fireball = 5,
            Heal = 6
        }
        public enum ArmorWeight
        {
            Light = 0,
            Medium = 1,
            Heavy = 2
        }
        
        [XmlIgnore]
        public Game game;

        public event PropertyChangedEventHandler PropertyChanged;

        #region Fields
        private Bitmap itemIconBitmap;
        private string p_itemIconfilename = "blank.png"; //item icon filename
        private string p_name = ""; //item name
        private string p_nameWithNotes = "newItemNameWithNotes";
        private string p_tag = "new_tag"; //item unique tag name
        private string p_resref = "new_resref"; //must be different from all other blueprints
        private string p_parentNodeName = "New Category"; // value type
        private string p_desc = ""; //item detailed description
        private UsableInSituation useableInSituation = UsableInSituation.Always;
        private string itemOnUseSound = "none";
        // * sinopip, 22.12.14
        private string itemOnHitSound = "none";
        //
        private string projectileSpriteFilename = "none"; //sprite to use for projectiles
        private string spriteEndingFilename = "none"; //sprite to use for end effect of projectiles
        private category p_category = category.Armor; //catergory type (armor, weapon, ammo, etc.)
        private ArmorWeight p_armorWeightType = ArmorWeight.Light;            
        private int p_value = 0; //cost in credits
        private int p_weight = 0; //weight of the item
        private int p_attackBonus = 0; //attack bonus
        private int p_attackRange = 1; //attack range
        private int p_AreaOfEffect = 0; //AoE
        private int p_damageNumDice = 1; //number of dice to roll for damage
        private int p_damageDie = 2; //type of dice to roll for damage
        private int p_damageAdder = 0; //the adder like 2d4+1 where "1" is the adder
        private int p_armorBonus = 0; //armor bonus
        private int p_damReduction = 0; //armor damage reduction
        private int p_maxDexBonus = 99; //maximum Dexterity bonus allowed with this armor
        private string p_scriptfile = ""; //item script filename
        private CharBase.CharacterAttribute attributeBonusType = CharBase.CharacterAttribute.Strength;
        private int attributeBonusModifier = 0;
        private int attributeBonusModifierStr = 0;
        private int attributeBonusModifierDex = 0;
        private int attributeBonusModifierCon = 0;
        private int attributeBonusModifierInt = 0;
        private int attributeBonusModifierWis = 0;
        private int attributeBonusModifierCha = 0;
        private CharBase.SavingThrow savingThrowBonusType = CharBase.SavingThrow.Fortitude;
        private int savingThrowBonusModifier = 0;
        private int savingThrowModifierReflex = 0;
        private int savingThrowModifierFortitude = 0;
        private int savingThrowModifierWill = 0;
        private int initiativeBonus = 0;
        private int movementPointModifier = 0;
        private int criticalHitRange = 20;
        private int criticalHitDamageMultiplier = 2;
        private int charges = 0;
        private bool identified = true;
        private bool canBeUsedUnidentified = false;
        private string unidentifiedDescription = "not identified";
        private int identifyingDC = 10;
        private string identifyingSkillTag = "lore";
        private bool cursed = false;
        private ScriptSelectEditorReturnObject onScoringHit = new ScriptSelectEditorReturnObject();
        private ScriptSelectEditorReturnObject onGettingHit = new ScriptSelectEditorReturnObject();
        private ScriptSelectEditorReturnObject onUseItem = new ScriptSelectEditorReturnObject();
        private ScriptSelectEditorReturnObject onEquipItem = new ScriptSelectEditorReturnObject();
        private ScriptSelectEditorReturnObject onUnEquipItem = new ScriptSelectEditorReturnObject();
        private ScriptSelectEditorReturnObject onAcquireItem = new ScriptSelectEditorReturnObject();
        private ScriptSelectEditorReturnObject onDropItem = new ScriptSelectEditorReturnObject();
        private ScriptSelectEditorReturnObject onWhileEquipped = new ScriptSelectEditorReturnObject();
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
        [CategoryAttribute("02 - Resistance Modifiers"), DescriptionAttribute("Damage resistance value (default is to use -100 to 100 as a percentage of immunity, damage multiplied by percentage so 0 = full damage, 100 = no damage, -100 = double damage)")]
        public int DamageTypeResistanceValuePoison
        {
            get { return damageTypeResistanceValuePoison; }
            set { damageTypeResistanceValuePoison = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Resistance Modifiers"), DescriptionAttribute("Damage resistance value (default is to use -100 to 100 as a percentage of immunity, damage multiplied by percentage so 0 = full damage, 100 = no damage, -100 = double damage)")]
        public int DamageTypeResistanceValueMagic
        {
            get { return damageTypeResistanceValueMagic; }
            set { damageTypeResistanceValueMagic = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Resistance Modifiers"), DescriptionAttribute("Damage resistance value (default is to use -100 to 100 as a percentage of immunity, damage multiplied by percentage so 0 = full damage, 100 = no damage, -100 = double damage)")]
        public int DamageTypeResistanceValueBludgeoning
        {
            get { return damageTypeResistanceValueBludgeoning; }
            set { damageTypeResistanceValueBludgeoning = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Resistance Modifiers"), DescriptionAttribute("Damage resistance value (default is to use -100 to 100 as a percentage of immunity, damage multiplied by percentage so 0 = full damage, 100 = no damage, -100 = double damage)")]
        public int DamageTypeResistanceValuePiercing
        {
            get { return damageTypeResistanceValuePiercing; }
            set { damageTypeResistanceValuePiercing = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Resistance Modifiers"), DescriptionAttribute("Damage resistance value (default is to use -100 to 100 as a percentage of immunity, damage multiplied by percentage so 0 = full damage, 100 = no damage, -100 = double damage)")]
        public int DamageTypeResistanceValueSlashing
        {
            get { return damageTypeResistanceValueSlashing; }
            set { damageTypeResistanceValueSlashing = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Resistance Modifiers"), DescriptionAttribute("Damage resistance value (default is to use -100 to 100 as a percentage of immunity, damage multiplied by percentage so 0 = full damage, 100 = no damage, -100 = double damage)")]
        public int DamageTypeResistanceValueAcid
        {
            get { return damageTypeResistanceValueAcid; }
            set { damageTypeResistanceValueAcid = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Resistance Modifiers"), DescriptionAttribute("Damage resistance value (default is to use -100 to 100 as a percentage of immunity, damage multiplied by percentage so 0 = full damage, 100 = no damage, -100 = double damage)")]
        public int DamageTypeResistanceValueCold
        {
            get { return damageTypeResistanceValueCold; }
            set { damageTypeResistanceValueCold = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Resistance Modifiers"), DescriptionAttribute("Damage resistance value (default is to use -100 to 100 as a percentage of immunity, damage multiplied by percentage so 0 = full damage, 100 = no damage, -100 = double damage)")]
        public int DamageTypeResistanceValueElectricity
        {
            get { return damageTypeResistanceValueElectricity; }
            set { damageTypeResistanceValueElectricity = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Resistance Modifiers"), DescriptionAttribute("Damage resistance value (default is to use -100 to 100 as a percentage of immunity, damage multiplied by percentage so 0 = full damage, 100 = no damage, -100 = double damage)")]
        public int DamageTypeResistanceValueFire
        {
            get { return damageTypeResistanceValueFire; }
            set { damageTypeResistanceValueFire = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Resistance Modifiers"), DescriptionAttribute("Damage resistance value (default is to use -100 to 100 as a percentage of immunity, damage multiplied by percentage so 0 = full damage, 100 = no damage, -100 = double damage)")]
        public int DamageTypeResistanceValueLight
        {
            get { return damageTypeResistanceValueLight; }
            set { damageTypeResistanceValueLight = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Resistance Modifiers"), DescriptionAttribute("Damage resistance value (default is to use -100 to 100 as a percentage of immunity, damage multiplied by percentage so 0 = full damage, 100 = no damage, -100 = double damage)")]
        public int DamageTypeResistanceValueSonic
        {
            get { return damageTypeResistanceValueSonic; }
            set { damageTypeResistanceValueSonic = value; }
        }
        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("The Type of Damage (useful with immunity checks)")]
        public DamageType TypeOfDamage
        {
            get { return typeOfDamage; }
            set { typeOfDamage = value; }
        }
        [XmlElement]
        [CategoryAttribute("86 - No Longer Used"), DescriptionAttribute("The Attribute that will receive a modifier")]
        public CharBase.CharacterAttribute AttributeBonusType
        {
            get { return attributeBonusType; }
            set { attributeBonusType = value; }
        }
        [XmlElement]
        [CategoryAttribute("86 - No Longer Used"), DescriptionAttribute("The modifier amount for the Attribute")]
        public int AttributeBonusModifier
        {
            get { return attributeBonusModifier; }
            set { attributeBonusModifier = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Modifiers"), DescriptionAttribute("The modifier amount for the Attribute")]
        public int AttributeBonusModifierStr
        {
            get { return attributeBonusModifierStr; }
            set { attributeBonusModifierStr = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Modifiers"), DescriptionAttribute("The modifier amount for the Attribute")]
        public int AttributeBonusModifierDex
        {
            get { return attributeBonusModifierDex; }
            set { attributeBonusModifierDex = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Modifiers"), DescriptionAttribute("The modifier amount for the Attribute")]
        public int AttributeBonusModifierCon
        {
            get { return attributeBonusModifierCon; }
            set { attributeBonusModifierCon = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Modifiers"), DescriptionAttribute("The modifier amount for the Attribute")]
        public int AttributeBonusModifierInt
        {
            get { return attributeBonusModifierInt; }
            set { attributeBonusModifierInt = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Modifiers"), DescriptionAttribute("The modifier amount for the Attribute")]
        public int AttributeBonusModifierWis
        {
            get { return attributeBonusModifierWis; }
            set { attributeBonusModifierWis = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Modifiers"), DescriptionAttribute("The modifier amount for the Attribute")]
        public int AttributeBonusModifierCha
        {
            get { return attributeBonusModifierCha; }
            set { attributeBonusModifierCha = value; }
        }
        [XmlElement]
        [CategoryAttribute("86 - No Longer Used"), DescriptionAttribute("The Saving Throw that will receive a modifier")]
        public CharBase.SavingThrow SavingThrowBonusType
        {
            get { return savingThrowBonusType; }
            set { savingThrowBonusType = value; }
        }
        [XmlElement]
        [CategoryAttribute("86 - No Longer Used"), DescriptionAttribute("The modifier amount for the Saving Throw")]
        public int SavingThrowBonusModifier
        {
            get { return savingThrowBonusModifier; }
            set { savingThrowBonusModifier = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Modifiers"), DescriptionAttribute("The modifier amount for the Reflex Saving Throw")]
        public int SavingThrowModifierReflex
        {
            get { return savingThrowModifierReflex; }
            set { savingThrowModifierReflex = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Modifiers"), DescriptionAttribute("The modifier amount for the Will Saving Throw")]
        public int SavingThrowModifierWill
        {
            get { return savingThrowModifierWill; }
            set { savingThrowModifierWill = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Modifiers"), DescriptionAttribute("The modifier amount for the Fortitude Saving Throw")]
        public int SavingThrowModifierFortitude
        {
            get { return savingThrowModifierFortitude; }
            set { savingThrowModifierFortitude = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Modifiers"), DescriptionAttribute("Bonus to initiative")]
        public int InitiativeBonus
        {
            get { return initiativeBonus; }
            set { initiativeBonus = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Modifiers"), DescriptionAttribute("Modifier to movement")]
        public int MovementPointModifier
        {
            get { return movementPointModifier; }
            set { movementPointModifier = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Modifiers"), DescriptionAttribute("The range for a critical hit (for 19-20, enter 19)")]
        public int CriticalHitRange
        {
            get { return criticalHitRange; }
            set { criticalHitRange = value; }
        }
        [XmlElement]
        [CategoryAttribute("02 - Modifiers"), DescriptionAttribute("The damage multiplier for a successful critical hit")]
        public int CriticalHitDamageMultiplier
        {
            get { return criticalHitDamageMultiplier; }
            set { criticalHitDamageMultiplier = value; }
        }
        [XmlElement]
        [CategoryAttribute("99 - Not Implemented Yet"), DescriptionAttribute("The amount of charges for the item")]
        public int Charges
        {
            get { return charges; }
            set { charges = value; }
        }
        [XmlElement]
        [CategoryAttribute("99 - Not Implemented Yet"), DescriptionAttribute("Is the item already identified?")]
        public bool Identified
        {
            get { return identified; }
            set { identified = value; }
        }
        [XmlElement]
        [CategoryAttribute("99 - Not Implemented Yet"), DescriptionAttribute("Can the item be used or equipped before it is identified?")]
        public bool CanBeUsedUnidentified
        {
            get { return canBeUsedUnidentified; }
            set { canBeUsedUnidentified = value; }
        }
        [XmlElement]
        [CategoryAttribute("99 - Not Implemented Yet"), DescriptionAttribute("Description to show when item is not identified")]
        public string UnidentifiedDescription
        {
            get { return unidentifiedDescription; }
            set { unidentifiedDescription = value; }
        }
        [XmlElement]
        [CategoryAttribute("99 - Not Implemented Yet"), DescriptionAttribute("DC for the identifying skill check OR if no skill rolls are wanted here, then the level of skill needed to ID it")]
        public int IdentifyingDC
        {
            get { return identifyingDC; }
            set { identifyingDC = value; }
        }
        [XmlElement]
        [CategoryAttribute("99 - Not Implemented Yet"), DescriptionAttribute("Tag of the skill used to ID the item")]
        public string IdentifyingSkillTag
        {
            get { return identifyingSkillTag; }
            set { identifyingSkillTag = value; }
        }
        [XmlElement]
        [CategoryAttribute("99 - Not Implemented Yet"), DescriptionAttribute("prevents unequipping, item likely has some negative effects set in other properties")]
        public bool Cursed
        {
            get { return cursed; }
            set { cursed = value; }
        }


        [XmlIgnore]
        [CategoryAttribute("Misc"), DescriptionAttribute("Stored Icon Bitmap"), ReadOnly(true)]
        public Bitmap ItemIconBitmap
        {
            get
            {
                return itemIconBitmap;
            }
            set
            {
                itemIconBitmap = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Name of the Item")]
        public string ItemName
        {
            get
            {
                return p_name;
            }
            set
            {
                p_name = value;
                this.NotifyPropertyChanged("ItemName");
            }
        }

        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Name of the Item that shows up in the toolset list but not in the game engine")]
        public string ItemNameWithNotes
        {
            get
            {
                if ((p_nameWithNotes == "newItemNameWithNotes") && ((p_name != "new item") || (p_name != "")))
                {
                    p_nameWithNotes = p_name;
                }
                return p_nameWithNotes;
            }
            set { p_nameWithNotes = value; }
        }

        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Tag of the Item (Must be unique)")]
        public string ItemTag
        {
            get
            {
                return p_tag;
            }
            set
            {
                p_tag = value;
                this.NotifyPropertyChanged("ItemTag");
            }
        }

        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Resource Reference name of the Item used for updating all placed objects that share the same identifier (must be unique from other blueprints")]
        public string ItemResRef
        {
            get { return p_resref; }
            set 
            { 
                p_resref = value;
                this.NotifyPropertyChanged("ItemResRef");
            }
        }

        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Category that this Item belongs to")]
        public string ItemCategoryName
        {
            get
            {
                return p_parentNodeName;
            }
            set
            {
                p_parentNodeName = value;
                this.NotifyPropertyChanged("ItemCategoryName");
            }
        }

        [XmlElement]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Description of the Item")]
        public string ItemDescription
        {
            get
            {
                return p_desc;
            }
            set
            {
                p_desc = value;
                this.NotifyPropertyChanged("ItemDescription");
            }
        }

        [XmlElement]
        [CategoryAttribute("99 - Not Implemented Yet"), DescriptionAttribute("When can this be used: Always means that it can be used in combat and on the main maps, Passive means that it is always on and doesn't need to be activated.")]
        public UsableInSituation UseableInSituation
        {
            get { return useableInSituation; }
            set { useableInSituation = value; }
        }

        [XmlElement]
        [Browsable(true), TypeConverter(typeof(SoundConverter))]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Filename of sound to play when the item is used (include extension)")]
        public string ItemOnUseSound
        {
            get { return itemOnUseSound; }
            set { itemOnUseSound = value; }
        }

        // * sinopip, 22.12.14
        [XmlElement]
        [Browsable(true), TypeConverter(typeof(SoundConverter))]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Filename of sound to play when the item (weapon) hits a target (include extension)")]
        public string ItemOnHitSound
        {
            get { return itemOnHitSound; }
            set { itemOnHitSound = value; }
        }

        [XmlElement]
        [Browsable(true), TypeConverter(typeof(SpriteConverter))]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Sprite to use for the projectile (Sprite Filename with extension)")]
        public string ProjectileSpriteFilename
        {
            get
            {
                return projectileSpriteFilename;
            }
            set
            {
                projectileSpriteFilename = value;
            }
        }

        [XmlElement]
        [Browsable(true), TypeConverter(typeof(SpriteConverter))]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Sprite to use for the projectile ending effect (Sprite Filename with extension)")]
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
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Icon Filename of the Item"), ReadOnly(true)]
        public string ItemIconFilename
        {
            get
            {
                return p_itemIconfilename;
            }
            set
            {
                p_itemIconfilename = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Item Category Type")]
        public category ItemCategory
        {
            get
            {
                return p_category;
            }
            set
            {
                p_category = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Armor Weight Type (used when the item is an armor)")]
        public ArmorWeight ArmorWeightType
        {
            get
            {
                return p_armorWeightType;
            }
            set
            {
                p_armorWeightType = value;
            }

        }
        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Cost of the Item in Gold Pieces")]
        public int ItemValue
        {
            get
            {
                return p_value;
            }
            set
            {
                p_value = value;
                this.NotifyPropertyChanged("ItemValue");
            }
        }

        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Weight of the item")]
        public int ItemWeight
        {
            get
            {
                return p_weight;
            }
            set
            {
                p_weight = value;
                this.NotifyPropertyChanged("ItemWeight");
            }
        }

        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Item Attack Bonus...Can be used to account for enchantments as well.")]
        public int ItemAttackBonus
        {
            get
            {
                return p_attackBonus;
            }
            set
            {
                p_attackBonus = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Item Attack Range measured in squares (ex 5 = 5 squares)")]
        public int ItemAttackRange
        {
            get
            {
                return p_attackRange;
            }
            set
            {
                p_attackRange = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("99 - Not Implemented Yet"), DescriptionAttribute("Item's Area of Effect radius measured in squares (0 = 1 square, 1 = 9 squares, etc.)")]
        public int ItemAreaOfEffect
        {
            get
            {
                return p_AreaOfEffect;
            }
            set
            {
                p_AreaOfEffect = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("number of dice to roll for damage")]
        public int ItemDamageNumberOfDice
        {
            get
            {
                return p_damageNumDice;
            }
            set
            {
                p_damageNumDice = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("number of sided die to roll for damage")]
        public int ItemDamageDie
        {
            get
            {
                return p_damageDie;
            }
            set
            {
                p_damageDie = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("The damage adder...for example with 2d4+1, the '1' is the adder. Can be used to account for enchantments as well.")]
        public int ItemDamageAdder
        {
            get
            {
                return p_damageAdder;
            }
            set
            {
                p_damageAdder = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Item's armor bonus")]
        public int ItemArmorBonus
        {
            get
            {
                return p_armorBonus;
            }
            set
            {
                p_armorBonus = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Item's damage reduction")]
        public int ItemDamageReduction
        {
            get
            {
                return p_damReduction;
            }
            set
            {
                p_damReduction = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("01 - Main"), DescriptionAttribute("Maximum Dexterity Bonus allowed with this Item")]
        public int ItemMaxDexBonus
        {
            get
            {
                return p_maxDexBonus;
            }
            set
            {
                p_maxDexBonus = value;
            }
        }

        [XmlElement]
        [CategoryAttribute("86 - No Longer Used"), DescriptionAttribute("Use OnUseItem script hook instead")]
        public string ItemScriptFilename
        {
            get
            {
                return p_scriptfile;
            }
            set
            {
                p_scriptfile = value;
            }
        }

        private List<LocalInt> itemLocalInts = new List<LocalInt>();
        [XmlArrayItem("ItemLocalInts")]
        public List<LocalInt> ItemLocalInts
        {
            get { return itemLocalInts; }
            set { itemLocalInts = value; }
        }
        private List<LocalString> itemLocalStrings = new List<LocalString>();
        [XmlArrayItem("ItemLocalStrings")]
        public List<LocalString> ItemLocalStrings
        {
            get { return itemLocalStrings; }
            set { itemLocalStrings = value; }
        }
        private List<LocalObject> itemLocalObjects = new List<LocalObject>();
        [XmlElement]
        public List<LocalObject> ItemLocalObjects
        {
            get { return itemLocalObjects; }
            set { itemLocalObjects = value; }
        }

        [XmlElement]
        [CategoryAttribute("03 - Scripts"), DescriptionAttribute("fires when the item makes a successful hit on a target")]
        [Editor(typeof(ScriptSelectEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ScriptSelectEditorReturnObject OnScoringHit
        {
            get { return onScoringHit; }
            set { onScoringHit = value; }
        }
        [XmlElement]
        [CategoryAttribute("99 - Not Implemented Yet"), DescriptionAttribute("fires when the item is hit by an enemy attack")]
        [Editor(typeof(ScriptSelectEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ScriptSelectEditorReturnObject OnGettingHit
        {
            get { return onGettingHit; }
            set { onGettingHit = value; }
        }
        [XmlElement]
        [CategoryAttribute("03 - Scripts"), DescriptionAttribute("fires when the item is used")]
        [Editor(typeof(ScriptSelectEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ScriptSelectEditorReturnObject OnUseItem
        {
            get { return onUseItem; }
            set { onUseItem = value; }
        }
        [XmlElement]
        [CategoryAttribute("99 - Not Implemented Yet"), DescriptionAttribute("fires when the item is equipped")]
        [Editor(typeof(ScriptSelectEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ScriptSelectEditorReturnObject OnEquipItem
        {
            get { return onEquipItem; }
            set { onEquipItem = value; }
        }
        [XmlElement]
        [CategoryAttribute("99 - Not Implemented Yet"), DescriptionAttribute("fires when the item is unequipped")]
        [Editor(typeof(ScriptSelectEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ScriptSelectEditorReturnObject OnUnEquipItem
        {
            get { return onUnEquipItem; }
            set { onUnEquipItem = value; }
        }
        [XmlElement]
        [CategoryAttribute("99 - Not Implemented Yet"), DescriptionAttribute("fires when the item is first acquired")]
        [Editor(typeof(ScriptSelectEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ScriptSelectEditorReturnObject OnAcquireItem
        {
            get { return onAcquireItem; }
            set { onAcquireItem = value; }
        }
        [XmlElement]
        [CategoryAttribute("99 - Not Implemented Yet"), DescriptionAttribute("fires when the item is dropped from inventory")]
        [Editor(typeof(ScriptSelectEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ScriptSelectEditorReturnObject OnDropItem
        {
            get { return onDropItem; }
            set { onDropItem = value; }
        }
        [XmlElement]
        [CategoryAttribute("03 - Scripts"), DescriptionAttribute("fires every time the UpdateStats() function is called. Useful for modifying PC's stats.")]
        [Editor(typeof(ScriptSelectEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ScriptSelectEditorReturnObject OnWhileEquipped
        {
            get { return onWhileEquipped; }
            set { onWhileEquipped = value; }
        }
        #endregion

        public Item()
        {
        }
        public void passRefs(Game g, ParentForm pf)
        {
            game = g;
            OnScoringHit.prntForm = pf;
            OnGettingHit.prntForm = pf;
            OnUseItem.prntForm = pf;
            OnEquipItem.prntForm = pf;
            OnUnEquipItem.prntForm = pf;
            OnAcquireItem.prntForm = pf;
            OnDropItem.prntForm = pf;
        }
        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        public override string ToString()
        {
            return ItemName;
        }
        public Bitmap LoadItemIconBitmap(string filename)
        {
            // Sets up an image object to be displayed.
            if (itemIconBitmap != null)
            {
                itemIconBitmap.Dispose();
            }
            itemIconBitmap = new Bitmap(filename);
            return itemIconBitmap;
        }
        public Item ShallowCopy()
        {
            return (Item)this.MemberwiseClone();
        }
        public Item DeepCopy()
        {
            Item other = (Item)this.MemberwiseClone();
            other.onAcquireItem = this.onAcquireItem.DeepCopy();
            other.onDropItem = this.onDropItem.DeepCopy();
            other.onEquipItem = this.onEquipItem.DeepCopy();
            other.onUnEquipItem = this.onUnEquipItem.DeepCopy();
            other.onUseItem = this.onUseItem.DeepCopy();
            other.onScoringHit = this.onScoringHit.DeepCopy();
            other.onGettingHit = this.onGettingHit.DeepCopy();
            other.ItemLocalInts = new List<LocalInt>();
            foreach (LocalInt l in this.ItemLocalInts)
            {
                LocalInt Lint = new LocalInt();
                Lint.Key = l.Key;
                Lint.Value = l.Value;
                other.ItemLocalInts.Add(Lint);
            }
            other.ItemLocalStrings = new List<LocalString>();
            foreach (LocalString l in this.ItemLocalStrings)
            {
                LocalString Lstr = new LocalString();
                Lstr.Key = l.Key;
                Lstr.Value = l.Value;
                other.ItemLocalStrings.Add(Lstr);
            }
            return other;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using IceBlinkCore;
using IceBlink;

namespace IceBlink
{
    public class IceBlinkScript
    {
        public void Script(ScriptFunctions sf, string p1, string p2, string p3, string p4)
        {
            // C# code goes here

            Creature source = sf.GetActionCreatureData();
            if (source == null)
            {
            	MessageBox.Show("Invalid script owner, not a Creature of PC");
                return;
            }                           
 
            SpellParameters sp = new SpellParameters();
            sp.Name = "Disintegrate";
            sp.TargetType = "enemies";
            sp.Type = "Damage";
            sp.NbDice = 2 * source.ClassLevel;
            sp.Die = 6;
            sp.DiceAdd = 0;
            sp.BaseDC = 13;
            sp.SuccessSaveResistance = 0.75;
            sp.StatMod = "INT";
            sp.EnergySource = "Magic";
			sp.SpellColor = Color.OrangeRed;
            sp.Description = "is hit by a disrupting ray";
			sf.DoSpell(sp);
        }
    }
}

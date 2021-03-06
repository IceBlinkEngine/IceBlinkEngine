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

			
            SpellParameters sp = new SpellParameters();
            sp.Name = "Hold";
            sp.TargetType = "enemies";
            sp.Type = "Debuff";
			sp.Effect = sf.gm.module.moduleEffectsList.getEffectByTag("hold");
            sp.BaseDC = 13;
            sp.Save = "Will";
            sp.StatMod = "INT";
			sp.SpellColor = Color.Brown;
            sp.EffectDescription = "is held still!";          
            sf.DoSpell(sp);
        }
    }
}

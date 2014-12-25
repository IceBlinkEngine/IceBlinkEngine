using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using IceBlinkCore;
using IceBlink;
using System.Drawing;

namespace IceBlink
{
    public class IceBlinkScript
    {
        //new variable accessible for all methods of IceBlinkScript class
        //stores the penalty for attacks after the first (increase by -5 per additional attack)
        public int creatureMultAttackPenalty;

        public void Script(ScriptFunctions sf, string p1, string p2, string p3, string p4)
        {
            // C# code goes here
            PC pc = (PC)sf.CombatTarget; //this is the PC that is being attacked
            Creature crt = (Creature)sf.CombatSource;
            Combat c = sf.frm.currentCombat;
            
            //check if backstab
            //check if multiple attack same target
            //check if multiple targets
            //check if multiple targets if first dies

            //will allow creatures to attack multiple times
            //idealy later on creatures will have different attacks with different damage values, effects, etc.
            //this is just a first solution until the toolset allows to fine tune each attack
            //Note: actually special attacks could be inserted here easily by cheking for a localint on the creature like "PosionousBite" or some such
            for (int i = 0; i < crt.CreatureNumberOfAttacks; i++)
            {

                //this reduces the to hit bonus for each further creature attack by an additional -5
                creatureMultAttackPenalty = 5 * i;
                bool killed = false;
                killed = doStandardCreatureAttack(sf, pc, crt);
                if (killed)
                {
                    return; //do not try and attack same PC that was just killed
                }
            }
            //since creature made an attack, it loses stealth mode
            if (sf.CheckLocalInt(crt.Tag, "StealthModeOn", "=", 1))
            {
                sf.SetLocalInt(crt.Tag, "StealthModeOn", 0); //turn off stealth mode if creature is attacking
                c.logText(crt.Name, Color.LightGray);
                c.logText(" exits stealth mode", Color.Black);
                c.logText(Environment.NewLine, Color.Black);
            }
        }

        public bool doStandardCreatureAttack(ScriptFunctions sf, PC pc, Creature crt)
        {
            Combat c = sf.frm.currentCombat;

            int attackRoll = sf.gm.Random(20);
            int attackMod = sf.CalcCreatureAttackModifier();
            attackMod -= creatureMultAttackPenalty;
            int defense = sf.CalcPcDefense();
            int damage = sf.CalcCreatureDamageToPc();
            int attack = attackRoll + attackMod;
            #region Critical Hit Stuff
            int crticalHitScored = 0;
            //critical hit (see detail in modified attackPlayer.cs)
            int criticalHitRange = crt.CriticalHitRange;
            //variable for storing the critical hit multiplier
            int criticalHitDamageMultiplier = crt.CriticalHitDamageMultiplier;
            //critical hit mechanism - triggered on a roll equal or higher than criticalHitRange  
            if ((attackRoll >= criticalHitRange) || (attackRoll == 20))
            {
                int attackRoll2 = sf.gm.Random(20);
                int attackMod2 = sf.CalcCreatureAttackModifier();
                attackMod2 -= creatureMultAttackPenalty;
                int attack2 = attackRoll2 + attackMod2;
                int defense2 = sf.CalcPcDefense();
                if ((attack2 >= defense2) || (attackRoll2 == 20))
                {
                    crticalHitScored = 1;
                    for (int i = 1; i < criticalHitDamageMultiplier; i++)
                    {
                        damage = (damage + sf.CalcCreatureDamageToPc());
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
                sf.drawHitSymbolOnPC();
				
				// * sinopip, 22.12.14
                c.playCreatureHitSound(crt);
                //				
//                c.refreshMap();
                string attackResult = (damage.ToString() + " of " + pc.HP.ToString());
                if (crticalHitScored == 1)
                {
                    sf.DrawCombatFloatyTextOverSquare(("CRTITICAL hit: " + attackResult), pc.CombatLocation.X, pc.CombatLocation.Y, 60, 12, -20, Color.Red, Color.Black);
                }
                else
                {
                    sf.DrawCombatFloatyTextOverSquare(attackResult, pc.CombatLocation.X, pc.CombatLocation.Y, 60, 12, -20, Color.Red, Color.Black);
                }

                c.logText(crt.Name, Color.LightGray);
                if (crticalHitScored == 1)
                {
                    c.logText(" CRITICALLY attacks ", Color.Black);
                }
                else
                {
                    c.logText(" attacks ", Color.Black);
                }
                c.logText(pc.Name, Color.Blue);
                c.logText(" and HITS for ", Color.Black);
                c.logText(damage.ToString(), Color.Red);
                c.logText(" point(s) of damage", Color.Black);
                c.logText(Environment.NewLine, Color.Black);
                c.logText(attackRoll.ToString() + " + " + attackMod.ToString() + " >= " + defense.ToString(), Color.Black);
                c.logText(Environment.NewLine, Color.Black);
                c.logText(Environment.NewLine, Color.Black);

                #region onScoringHit script of creature
                IceBlinkCore.ScriptSelectEditorReturnObject scriptItem = crt.OnScoringHit;
                sf.frm.doScriptBasedOnFilename(scriptItem.FilenameOrTag, scriptItem.Parm1, scriptItem.Parm2, scriptItem.Parm3, scriptItem.Parm4);
                #endregion

                pc.HP = pc.HP - damage;
                if (pc.HP <= 0)
                {
                    c.logText(pc.Name + " drops down unconsciously!", Color.Red);
                    c.logText(Environment.NewLine, Color.Black);
                    c.logText(Environment.NewLine, Color.Black);
                    pc.Status = PC.charStatus.Dead;
                    return true;
                }
                return false;
            }
            else
            {
                sf.DrawCombatFloatyTextOverSquare("Evades!", pc.CombatLocation.X, pc.CombatLocation.Y, 60, 16, -5, Color.Blue, Color.Black);
                c.logText(crt.Name, Color.LightGray);
                c.logText(" attacks ", Color.Black);
                c.logText(pc.Name, Color.Blue);
                c.logText(" and MISSES", Color.Black);
                c.logText(Environment.NewLine, Color.Black);
                c.logText(attackRoll.ToString() + " + " + attackMod.ToString() + " < " + defense.ToString(), Color.Black);
                c.logText(Environment.NewLine, Color.Black);
                c.logText(Environment.NewLine, Color.Black);
                return false;
            }
        }
    }
}

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
        public void Script(ScriptFunctions sf, string p1, string p2, string p3, string p4)
        {
            // C# code goes here
            Combat c = sf.frm.currentCombat;
            Creature crt = (Creature)sf.CombatSource; //this is the creature that is calling this script

            //These are the current generic AI types
            //BasicAttacker:          basic attack (ranged or melee)
            //Healer:                 heal Friend(s) until out of SP
            //BattleHealer:           heal Friend(s) and/or attack
            //DamageCaster:           cast damage spells
            //BattleDamageCaster:     cast damage spells and/or attack
            //DebuffCaster:           cast debuff spells
            //BattleDebuffCaster:     cast debuff spells and/or attack
            //GeneralCaster:          cast any of their known spells by random
            //BattleGeneralCaster:    cast any of their known spells by random and/or attack

            #region Get AI Type and jump to AI method
            if (crt.CreatureAI == AiBasicTactic.BasicAttacker)
            {
                if (sf.frm.debugMode)
                {
                    c.logText(crt.Name, Color.LightGray);
                    c.logText(" is a BasicAttacker", Color.Black);
                    c.logText(Environment.NewLine, Color.Black);
                }
                BasicAttacker(sf, c, crt);
            }
            else if (crt.CreatureAI == AiBasicTactic.DebuffCaster)
            {
                if (sf.frm.debugMode)
                {
                    c.logText(crt.Name, Color.LightGray);
                    c.logText(" is a DebuffCaster", Color.Black);
                    c.logText(Environment.NewLine, Color.Black);                    
                }
                DebuffCaster(sf, c, crt);
            }
            else if (crt.CreatureAI == AiBasicTactic.BattleDebuffCaster)
            {
                if (sf.frm.debugMode)
                {
                    c.logText(crt.Name, Color.LightGray);
                    c.logText(" is a BattleDebuffCaster", Color.Black);
                    c.logText(Environment.NewLine, Color.Black);                    
                }
                BattleDebuffCaster(sf, c, crt);
            }
            else if (crt.CreatureAI == AiBasicTactic.DamageCaster)
            {
                if (sf.frm.debugMode)
                {
                    c.logText(crt.Name, Color.LightGray);
                    c.logText(" is a DamageCaster", Color.Black);
                    c.logText(Environment.NewLine, Color.Black);                    
                }
                DamageCaster(sf, c, crt);
            }
            else if (crt.CreatureAI == AiBasicTactic.BattleDamageCaster)
            {
                if (sf.frm.debugMode)
                {
                    c.logText(crt.Name, Color.LightGray);
                    c.logText(" is a BattleDamageCaster", Color.Black);
                    c.logText(Environment.NewLine, Color.Black);                    
                }
                BattleDamageCaster(sf, c, crt);
            }
            else if (crt.CreatureAI == AiBasicTactic.Healer)
            {
                if (sf.frm.debugMode)
                {
                    c.logText(crt.Name, Color.LightGray);
                    c.logText(" is a Healer", Color.Black);
                    c.logText(Environment.NewLine, Color.Black);                    
                }
                Healer(sf, c, crt);
            }
            else if (crt.CreatureAI == AiBasicTactic.BattleHealer)
            {
                if (sf.frm.debugMode)
                {
                    c.logText(crt.Name, Color.LightGray);
                    c.logText(" is a BattleHealer", Color.Black);
                    c.logText(Environment.NewLine, Color.Black);                    
                }
                BattleHealer(sf, c, crt);
            }
            else if (crt.CreatureAI == AiBasicTactic.GeneralCaster)
            {
                if (sf.frm.debugMode)
                {
                    c.logText(crt.Name, Color.LightGray);
                    c.logText(" is a GeneralCaster", Color.Black);
                    c.logText(Environment.NewLine, Color.Black);                    
                }
                GeneralCaster(sf, c, crt);
            }
            else if (crt.CreatureAI == AiBasicTactic.BattleGeneralCaster)
            {
                if (sf.frm.debugMode)
                {
                    c.logText(crt.Name, Color.LightGray);
                    c.logText(" is a BattleGeneralCaster", Color.Black);
                    c.logText(Environment.NewLine, Color.Black);                    
                }
                BattleGeneralCaster(sf, c, crt);
            }
            else
            {
                if (sf.frm.debugMode)
                {
                    c.logText(crt.Name, Color.LightGray);
                    c.logText(" is a BasicAttacker", Color.Black);
                    c.logText(Environment.NewLine, Color.Black);                    
                }
                BasicAttacker(sf, c, crt);
            }
            #endregion
        }

        public void BasicAttacker(ScriptFunctions sf, Combat c, Creature crt)
        {
            if (crt.Intelligence > 13) //will attack PC with lowest HP
            {
                int PCindex = sf.TargetPCWithLowestHP(true); //(true) means that will ignore PCs in Stealth Mode                
                sf.CombatTarget = PCindex; //this will set the creature's target to be the PC with the fewest HP
            }
            else //will attack the closest PC
            {
                int PCindex = sf.TargetClosestPC(true);
                sf.CombatTarget = PCindex;
            }
        }
        public void DebuffCaster(ScriptFunctions sf, Combat c, Creature crt)
        {
            //intelligent
            if (crt.Intelligence > 13)
            {
                sf.SpellToCast = null;
                //go through most powerful KnownSpells first and see if creature has enough SP
                foreach (string spTag in crt.KnownSpellsTags)
                {
                    if (sf.frm.debugMode)
                    {
                        c.logText("KnownSpellTag: " + spTag, Color.Black);
                        c.logText(Environment.NewLine, Color.Black);
                    }
                    Spell sp = sf.gm.module.ModuleSpellsList.getSpellByTag(spTag);
                    if (sp != null)
                    {
                        if (sf.frm.debugMode)
                        {
                            c.logText("KnownSpell: " + sp.SpellName, Color.Black);
                            c.logText(Environment.NewLine, Color.Black);
                        }
                        int mostSpellPointCost = 0;
                        if ((crt.SP >= sp.CostSP) && (sp.CostSP > mostSpellPointCost) && (sp.SpellEffectType == Spell.EffectType.Debuff))
                        {
                            //sf.SpellToCast = sf.gm.module.ModuleSpellsList.getSpellByTag(sp.SpellTag);
                            sf.SpellToCast = sp;
                            mostSpellPointCost = sp.CostSP;
                            if (sf.frm.debugMode)
                            {
                                c.logText("SpellToCast: " + sf.SpellToCast.SpellName, Color.Black);
                                c.logText(Environment.NewLine, Color.Black);
                            }
                        }
                    }
                }
                if (sf.SpellToCast != null)
                {
                    int PcIndex = sf.TargetClosestPcNotHeld(true); //(true) means that will ignore PCs in Stealth Mode
                    if (PcIndex >= 0)
                    {
                        PC pc = sf.gm.playerList.PCList[PcIndex];
                        if (sf.SpellToCast.SpellTargetType == TargetType.PointLocation)
                        //if (sf.SpellToCast.TargetIsPointLocation)
                        {
                            sf.CombatTarget = pc.CombatLocation; //we are using a Point type because the selected spell is looking for a Point type (square location)
                        }
                        else //we are assuming that TargetType is Enemy because this is a debuff spell
                        {
                            sf.CombatTarget = pc;
                        }
                        sf.ActionToTake = ChosenAction.CastSpell;
                    }
                    else
                    {
                        int PCindex = sf.TargetClosestPC(true);
                        sf.CombatTarget = PCindex;
                        sf.ActionToTake = ChosenAction.MeleeRangedAttack;
                    }
                }
                else
                {
                    int PCindex = sf.TargetClosestPC(true);
                    sf.CombatTarget = PCindex;
                    sf.ActionToTake = ChosenAction.MeleeRangedAttack;
                }
            }
            else //not as intelligent
            {
                sf.SpellToCast = null;
                //just pick a random spell from KnownSpells
                //try a few times to pick a random spell that has enough SP and is a debuff
                for (int i = 0; i < 10; i++)
                {
                    int rnd = sf.gm.Random(0, crt.KnownSpellsTags.Count - 1);
                    Spell sp = sf.gm.module.ModuleSpellsList.getSpellByTag(crt.KnownSpellsTags[rnd]);
                    if (sp != null)
                    {
                        if (sf.frm.debugMode)
                        {
                            c.logText("KnownSpell: " + sp.SpellName + "  SPcost: " + sp.CostSP + "  crt.SP: " + crt.SP, Color.Black);
                            c.logText(Environment.NewLine, Color.Black);
                        }
                        if ((sp.CostSP <= crt.SP) && (sp.SpellEffectType == Spell.EffectType.Debuff))
                        {
                            sf.SpellToCast = sp;
                            if (sf.frm.debugMode)
                            {
                                c.logText("SpellToCast: " + sf.SpellToCast.SpellName, Color.Black);
                                c.logText(Environment.NewLine, Color.Black);
                            }
                            int PcIndex = sf.TargetClosestPC(true); //(true) means that will ignore PCs in Stealth Mode
                            PC pc = sf.gm.playerList.PCList[PcIndex];
                            if (sf.SpellToCast.SpellTargetType == TargetType.PointLocation)
                            //if (sf.SpellToCast.TargetIsPointLocation)
                            {
                                sf.CombatTarget = pc.CombatLocation; //we are using a Point type because the selected spell is looking for a Point type (square location)
                                if (sf.frm.debugMode)
                                {
                                    c.logText("sf.CombatTarget: " + sf.CombatTarget.ToString(), Color.Black);
                                    c.logText(Environment.NewLine, Color.Black);
                                }
                            }
                            else //we are assuming that TargetType is Enemy because this is a debuff spell
                            {
                                sf.CombatTarget = pc;
                                if (sf.frm.debugMode)
                                {
                                    c.logText("sf.CombatTarget: " + sf.CombatTarget.ToString(), Color.Black);
                                    c.logText(Environment.NewLine, Color.Black);
                                }
                            }
                            sf.ActionToTake = ChosenAction.CastSpell;
                            break;
                        }
                    }
                }
                if (sf.SpellToCast == null) //didn't find a spell that matched the criteria so use attack instead
                {
                    int PCindex = sf.TargetClosestPC(true);
                    sf.CombatTarget = PCindex;
                    sf.ActionToTake = ChosenAction.MeleeRangedAttack;
                }              
            }            
        }
        public void BattleDebuffCaster(ScriptFunctions sf, Combat c, Creature crt)
        {

            //intelligent
            if (crt.Intelligence > 13)
            {
                int rnd = sf.gm.Random(1, 10);
                if (rnd > 5)
                {
                    sf.SpellToCast = null;
                    //go through most powerful KnownSpells first and see if creature has enough SP
                    foreach (string spTag in crt.KnownSpellsTags)
                    {
                        if (sf.frm.debugMode)
                        {
                            c.logText("KnownSpellTag: " + spTag, Color.Black);
                            c.logText(Environment.NewLine, Color.Black);
                        }
                        Spell sp = sf.gm.module.ModuleSpellsList.getSpellByTag(spTag);
                        if (sp != null)
                        {
                            if (sf.frm.debugMode)
                            {
                                c.logText("KnownSpell: " + sp.SpellName, Color.Black);
                                c.logText(Environment.NewLine, Color.Black);
                            }
                            int mostSpellPointCost = 0;
                            if ((crt.SP >= sp.CostSP) && (sp.CostSP > mostSpellPointCost) && (sp.SpellEffectType == Spell.EffectType.Debuff))
                            {
                                //sf.SpellToCast = sf.gm.module.ModuleSpellsList.getSpellByTag(sp.SpellTag);
                                sf.SpellToCast = sp;
                                mostSpellPointCost = sp.CostSP;
                                if (sf.frm.debugMode)
                                {
                                    c.logText("SpellToCast: " + sf.SpellToCast.SpellName, Color.Black);
                                    c.logText(Environment.NewLine, Color.Black);
                                }
                            }
                        }
                    }
                    if (sf.SpellToCast != null)
                    {
                        int PcIndex = sf.TargetClosestPcNotHeld(true); //(true) means that will ignore PCs in Stealth Mode
                        if (PcIndex >= 0)
                        {
                            PC pc = sf.gm.playerList.PCList[PcIndex];
                            if (sf.SpellToCast.SpellTargetType == TargetType.PointLocation)
                            //if (sf.SpellToCast.TargetIsPointLocation)
                            {
                                sf.CombatTarget = pc.CombatLocation; //we are using a Point type because the selected spell is looking for a Point type (square location)
                            }
                            else //we are assuming that TargetType is Enemy because this is a debuff spell
                            {
                                sf.CombatTarget = pc;
                            }
                            sf.ActionToTake = ChosenAction.CastSpell;
                        }
                        else
                        {
                            int PCindex = sf.TargetClosestPC(true);
                            sf.CombatTarget = PCindex;
                            sf.ActionToTake = ChosenAction.MeleeRangedAttack;
                        }
                    }
                    else
                    {
                        int PCindex = sf.TargetClosestPC(true);
                        sf.CombatTarget = PCindex;
                        sf.ActionToTake = ChosenAction.MeleeRangedAttack;
                    }
                }
                else //does attack instead of spell due to being battle caster (percent chance of attack)
                {
                    int PCindex = sf.TargetClosestPC(true);
                    sf.CombatTarget = PCindex;
                    sf.ActionToTake = ChosenAction.MeleeRangedAttack;
                }
            }
            else //not as intelligent
            {
                int r = sf.gm.Random(1, 10);
                if (r > 5)
                {
                    sf.SpellToCast = null;
                    //just pick a random spell from KnownSpells
                    //try a few times to pick a random spell that has enough SP and is a debuff
                    for (int i = 0; i < 10; i++)
                    {
                        int rnd = sf.gm.Random(0, crt.KnownSpellsTags.Count - 1);
                        Spell sp = sf.gm.module.ModuleSpellsList.getSpellByTag(crt.KnownSpellsTags[rnd]);
                        if (sp != null)
                        {
                            if (sf.frm.debugMode)
                            {
                                c.logText("KnownSpell: " + sp.SpellName, Color.Black);
                                c.logText(Environment.NewLine, Color.Black);
                            }
                            if ((sp.CostSP <= crt.SP) && (sp.SpellEffectType == Spell.EffectType.Debuff))
                            {
                                sf.SpellToCast = sp;
                                if (sf.frm.debugMode)
                                {
                                    c.logText("SpellToCast: " + sf.SpellToCast.SpellName, Color.Black);
                                    c.logText(Environment.NewLine, Color.Black);
                                }
                                int PcIndex = sf.TargetClosestPC(true); //(true) means that will ignore PCs in Stealth Mode
                                PC pc = sf.gm.playerList.PCList[PcIndex];
                                if (sf.SpellToCast.SpellTargetType == TargetType.PointLocation)
                                //if (sf.SpellToCast.TargetIsPointLocation)
                                {
                                    sf.CombatTarget = pc.CombatLocation; //we are using a Point type because the selected spell is looking for a Point type (square location)
                                }
                                else //we are assuming that TargetType is Enemy because this is a debuff spell
                                {
                                    sf.CombatTarget = pc;
                                }
                                sf.ActionToTake = ChosenAction.CastSpell;
                                break;
                            }
                        }
                    }
                    if (sf.SpellToCast == null) //didn't find a spell that matched the criteria so use attack instead
                    {
                        int PCindex = sf.TargetClosestPC(true);
                        sf.CombatTarget = PCindex;
                        sf.ActionToTake = ChosenAction.MeleeRangedAttack;
                    }
                }
                else //does attack instead of spell due to being battle caster (percent chance of attack)
                {
                    int PCindex = sf.TargetClosestPC(true);
                    sf.CombatTarget = PCindex;
                    sf.ActionToTake = ChosenAction.MeleeRangedAttack;
                }
            }
        }
        public void DamageCaster(ScriptFunctions sf, Combat c, Creature crt)
        {
            //sf.frm.debugMode = true; //JamesManhattan for testing.
            //intelligent
            if (crt.Intelligence > 13)
            {
                sf.SpellToCast = null;               
                //go through most powerful KnownSpells first and see if creature has enough SP
                foreach (string spTag in crt.KnownSpellsTags)
                {
                    if (sf.frm.debugMode)
                    {
                        c.logText("KnownSpellTag: " + spTag, Color.Black);
                        c.logText(Environment.NewLine, Color.Black);
                    }
                    Spell sp = sf.gm.module.ModuleSpellsList.getSpellByTag(spTag);
                    if (sp != null)
                    {
                        if (sf.frm.debugMode)
                        {
                            c.logText("KnownSpell: " + sp.SpellName, Color.Black);
                            c.logText(Environment.NewLine, Color.Black);
                        }
                        int mostSpellPointCost = -1; //JamesManhattan have to start at -1 if cantrips cost 0
                        if ((crt.SP >= sp.CostSP) && (sp.CostSP > mostSpellPointCost) && (sp.SpellEffectType == Spell.EffectType.Damage))
                        {
                            sf.SpellToCast = sp;
                            mostSpellPointCost = sp.CostSP;
                            if (sf.frm.debugMode)
                            {
                                c.logText("SpellToCast: " + sf.SpellToCast.SpellName, Color.Black);
                                c.logText(Environment.NewLine, Color.Black);
                            }
                        }
                    }
                }
                if (sf.SpellToCast != null)
                {
                    int PcIndex = sf.TargetClosestPcNotHeld(true); //(true) means that will ignore PCs in Stealth Mode
                    if (PcIndex >= 0)
                    {
                        PC pc = sf.gm.playerList.PCList[PcIndex];
                        if (sf.SpellToCast.SpellTargetType == TargetType.PointLocation)
                        //if (sf.SpellToCast.TargetIsPointLocation)
                        {                            
                            //sf.CombatTarget = pc.CombatLocation; //we are using a Point type because the selected spell is looking for a Point type (square location)
                            //JamesManhattan Utility maximization function   for the VERY INTELLIGENT CREATURE CASTER
                            int utility = 0; //utility
                            int optimalUtil = 0; //optimal utility, a storage of the highest achieved
                            Point selectedPoint = new Point(crt.CombatLocation.X, crt.CombatLocation.Y); //Initial Select Point is Creature itself, then loop through all squares within range!
                            for (int y = sf.SpellToCast.Range; y > -sf.SpellToCast.Range; y--)  //start at far range and work backwards does a pretty good job of avoiding hitting allies.
                            {
                                for (int x = sf.SpellToCast.Range; x > -sf.SpellToCast.Range; x--)
                                {
                                    utility = 0; //reset utility for each point tested
                                    selectedPoint = new Point(crt.CombatLocation.X + x, crt.CombatLocation.Y + y);
                                    //if (c.IsVisibleLineOfSight(crt.CombatLocation, selectedPoint)) //make sure creature caster can actually see the target point. //this causes errors I think on points outside the map
                                    //{
                                    //c.logText("Selecting x=" + selectedPoint.X.ToString() + "y=" + selectedPoint.Y.ToString(), Color.Purple);
                                    if (sf.CalcDistance(crt.CombatLocation, selectedPoint) <= sf.SpellToCast.AoeRadiusOrLength) {
                                        utility = utility - 4; //the creature at least attempts to avoid hurting itself, but if surrounded might fireball itself!
                                        if (crt.HP <= crt.HPMax / 4) //caster is wounded, definately avoids itself.
                                        {
                                            utility = utility - 4;
                                        }
                                    } 
                                    foreach (Creature crtr in c.com_encounter.EncounterCreatureList.creatures) //if its allies are in the burst subtract a point, or half depending on how evil it is.
                                    {
                                        if (sf.CalcDistance(crtr.CombatLocation, selectedPoint) <= sf.SpellToCast.AoeRadiusOrLength) //if friendly creatures are in the AOE burst, count how many, subtract 0.5 for each, evil is evil
                                        {
                                            utility = utility - 1;
                                            //c.logText("found " + crtr.Name + " creature u=" + utility.ToString(), Color.Lime);
                                        }
                                    }
                                    foreach (PC tgt_pc in sf.gm.playerList.PCList)   //sf.gm.currentEncounter.playerList.PCList[2]
                                    {
                                        if (sf.CalcDistance(tgt_pc.CombatLocation, selectedPoint) <= sf.SpellToCast.AoeRadiusOrLength && tgt_pc.Status != PC.charStatus.Dead) //if players are in the AOE burst, count how many, total count is utility  //&& sf.GetLocalInt(tgt_pc.Tag, "StealthModeOn") != 1  <-throws an annoying message if not found!!
                                        {
                                            utility = utility + 2;
                                            //c.logText("found " + tgt_pc.Name + " pc u=" + utility.ToString(), Color.Magenta);
                                            if (utility > optimalUtil)
                                            {
                                                //optimal found, choose this point
                                                optimalUtil = utility;
                                                sf.CombatTarget = selectedPoint;
                                                //c.logText("Optimal x=" + selectedPoint.X.ToString() + "y=" + selectedPoint.Y.ToString(), Color.Red);
                                            }
                                        }
                                    }
                                    //}
                                }
                            }

                        }
                        else //we are assuming that TargetType is Enemy because this is a damage spell
                        {
                            sf.CombatTarget = pc;
                        }
                        sf.ActionToTake = ChosenAction.CastSpell;
                    }
                    else
                    {
                        int PCindex = sf.TargetClosestPC(true);
                        sf.CombatTarget = PCindex;
                        sf.ActionToTake = ChosenAction.MeleeRangedAttack;
                    }
                }
                else
                {
                    int PCindex = sf.TargetClosestPC(true);
                    sf.CombatTarget = PCindex;
                    sf.ActionToTake = ChosenAction.MeleeRangedAttack;
                }
            }
            else //not as intelligent
            {
                sf.SpellToCast = null;
                //just pick a random spell from KnownSpells
                //try a few times to pick a random spell that has enough SP and is a damage
                for (int i = 0; i < 10; i++)
                {
                    int rnd = sf.gm.Random(0, crt.KnownSpellsTags.Count - 1);
                    Spell sp = sf.gm.module.ModuleSpellsList.getSpellByTag(crt.KnownSpellsTags[rnd]);
                    if (sp != null)
                    {
                        if (sf.frm.debugMode)
                        {
                            c.logText("KnownSpell: " + sp.SpellName, Color.Black);
                            c.logText(Environment.NewLine, Color.Black);
                        }
                        if ((sp.CostSP <= crt.SP) && (sp.SpellEffectType == Spell.EffectType.Damage))
                        {
                            sf.SpellToCast = sp;
                            if (sf.frm.debugMode)
                            {
                                c.logText("SpellToCast: " + sf.SpellToCast.SpellName, Color.Black);
                                c.logText(Environment.NewLine, Color.Black);
                            }
                            int PcIndex = sf.TargetClosestPC(true); //(true) means that will ignore PCs in Stealth Mode
                            PC pc = sf.gm.playerList.PCList[PcIndex];
                            if (sf.SpellToCast.SpellTargetType == TargetType.PointLocation)
                            //if (sf.SpellToCast.TargetIsPointLocation)
                            {
                                //sf.CombatTarget = pc.CombatLocation; //we are using a Point type because the selected spell is looking for a Point type (square location) ORIGINAL
                                //JamesManhattan Utility maximization function   for the SUB INTELLIGENT   
                                int utility = 0; //utility
                                int optimalUtil = 0; //optimal utility, a storage of the highest achieved
                                Point selectedPoint = new Point(crt.CombatLocation.X, crt.CombatLocation.Y); //Initial Select Point is Creature itself, then loop through all squares within range!
                                for (int y = sf.SpellToCast.Range; y > -sf.SpellToCast.Range; y--)  //start at far range and work backwards does a pretty good job of avoiding hitting allies.
                                {
                                    for (int x = sf.SpellToCast.Range; x > -sf.SpellToCast.Range; x--)
                                    {
                                        utility = 0; //reset utility for each point tested
                                        selectedPoint = new Point(crt.CombatLocation.X + x, crt.CombatLocation.Y + y);
                                        //if (c.IsVisibleLineOfSight(crt.CombatLocation, selectedPoint)) //make sure creature caster can actually see the target point. //this causes errors I think on points outside the map
                                        //{
                                            //c.logText("Selecting x=" + selectedPoint.X.ToString() + "y=" + selectedPoint.Y.ToString(), Color.Purple);
                                            if (sf.CalcDistance(crt.CombatLocation, selectedPoint) <= sf.SpellToCast.AoeRadiusOrLength) {
                                                utility = utility - 4; //the creature at least attempts to avoid hurting itself, but if surrounded might fireball itself!
                                            } 
                                            foreach (Creature crtr in c.com_encounter.EncounterCreatureList.creatures) //if its allies are in the burst subtract a point, or half depending on how evil it is.
                                            {
                                                if (sf.CalcDistance(crtr.CombatLocation, selectedPoint) <= sf.SpellToCast.AoeRadiusOrLength) //if friendly creatures are in the AOE burst, count how many, subtract 0.5 for each, evil is evil
                                                {
                                                    utility = utility - 1;
                                                    //c.logText("found " + crtr.Name + " creature u=" + utility.ToString(), Color.Lime);
                                                }
                                            }
                                            foreach (PC tgt_pc in sf.gm.playerList.PCList)   //sf.gm.currentEncounter.playerList.PCList[2]
                                            {
                                                if (sf.CalcDistance(tgt_pc.CombatLocation, selectedPoint) <= sf.SpellToCast.AoeRadiusOrLength && tgt_pc.Status != PC.charStatus.Dead) //if players are in the AOE burst, count how many, total count is utility //&& sf.GetLocalInt(tgt_pc.Tag, "StealthModeOn") != 1  <-throws an annoying message if not found!!
                                                {
                                                    utility = utility + 2;
                                                    //c.logText("found " + tgt_pc.Name + " pc u=" + utility.ToString(), Color.Magenta);
                                                    if (utility > optimalUtil)
                                                    {
                                                        //optimal found, choose this point
                                                        optimalUtil = utility;
                                                        sf.CombatTarget = selectedPoint;
                                                        //c.logText("Optimal x=" + selectedPoint.X.ToString() + "y=" + selectedPoint.Y.ToString(), Color.Red);
                                                    }
                                                }
                                            }
                                        //}
                                    }
                                }
                                
                            }
                            else //we are assuming that TargetType is Enemy because this is a damage spell
                            {
                                sf.CombatTarget = pc;
                            }
                            sf.ActionToTake = ChosenAction.CastSpell;
                            break;
                        }
                    }
                }
                if (sf.SpellToCast == null) //didn't find a spell that matched the criteria so use attack instead
                {
                    int PCindex = sf.TargetClosestPC(true);
                    sf.CombatTarget = PCindex;
                    sf.ActionToTake = ChosenAction.MeleeRangedAttack;
                }
            }
        }
        public void BattleDamageCaster(ScriptFunctions sf, Combat c, Creature crt)
        {
            //intelligent
            if (crt.Intelligence > 13)
            {
                int rnd = sf.gm.Random(1, 10);
                if (rnd > 5)
                {
                    sf.SpellToCast = null;
                    //go through most powerful KnownSpells first and see if creature has enough SP
                    foreach (string spTag in crt.KnownSpellsTags)
                    {
                        if (sf.frm.debugMode)
                        {
                            c.logText("KnownSpellTag: " + spTag, Color.Black);
                            c.logText(Environment.NewLine, Color.Black);
                        }
                        Spell sp = sf.gm.module.ModuleSpellsList.getSpellByTag(spTag);
                        if (sp != null)
                        {
                            if (sf.frm.debugMode)
                            {
                                c.logText("KnownSpell: " + sp.SpellName, Color.Black);
                                c.logText(Environment.NewLine, Color.Black);
                            }
                            int mostSpellPointCost = 0;
                            if ((crt.SP >= sp.CostSP) && (sp.CostSP > mostSpellPointCost) && (sp.SpellEffectType == Spell.EffectType.Damage))
                            {
                                sf.SpellToCast = sp;
                                if (sf.frm.debugMode)
                                {
                                    mostSpellPointCost = sp.CostSP;
                                    c.logText("SpellToCast: " + sf.SpellToCast.SpellName, Color.Black);
                                }
                                c.logText(Environment.NewLine, Color.Black);
                            }
                        }
                    }
                    if (sf.SpellToCast != null)
                    {
                        int PcIndex = sf.TargetClosestPcNotHeld(true); //(true) means that will ignore PCs in Stealth Mode
                        if (PcIndex >= 0)
                        {
                            PC pc = sf.gm.playerList.PCList[PcIndex];
                            if (sf.SpellToCast.SpellTargetType == TargetType.PointLocation)
                            //if (sf.SpellToCast.TargetIsPointLocation)
                            {
                                sf.CombatTarget = pc.CombatLocation; //we are using a Point type because the selected spell is looking for a Point type (square location)
                            }
                            else //we are assuming that TargetType is Enemy because this is a damage spell
                            {
                                sf.CombatTarget = pc;
                            }
                            sf.ActionToTake = ChosenAction.CastSpell;
                        }
                        else
                        {
                            int PCindex = sf.TargetClosestPC(true);
                            sf.CombatTarget = PCindex;
                            sf.ActionToTake = ChosenAction.MeleeRangedAttack;
                        }
                    }
                    else
                    {
                        int PCindex = sf.TargetClosestPC(true);
                        sf.CombatTarget = PCindex;
                        sf.ActionToTake = ChosenAction.MeleeRangedAttack;
                    }
                }
                else //does attack instead of spell due to being battle caster (percent chance of attack)
                {
                    int PCindex = sf.TargetClosestPC(true);
                    sf.CombatTarget = PCindex;
                    sf.ActionToTake = ChosenAction.MeleeRangedAttack;
                }
            }
            else //not as intelligent
            {
                int r = sf.gm.Random(1, 10);
                if (r > 5)
                {
                    sf.SpellToCast = null;
                    //just pick a random spell from KnownSpells
                    //try a few times to pick a random spell that has enough SP and is a debuff
                    for (int i = 0; i < 10; i++)
                    {
                        int rnd = sf.gm.Random(0, crt.KnownSpellsTags.Count - 1);
                        Spell sp = sf.gm.module.ModuleSpellsList.getSpellByTag(crt.KnownSpellsTags[rnd]);
                        if (sp != null)
                        {
                            if (sf.frm.debugMode)
                            {
                                c.logText("KnownSpell: " + sp.SpellName, Color.Black);
                                c.logText(Environment.NewLine, Color.Black);
                            }
                            if ((sp.CostSP <= crt.SP) && (sp.SpellEffectType == Spell.EffectType.Damage))
                            {
                                sf.SpellToCast = sp;
                                if (sf.frm.debugMode)
                                {
                                    c.logText("SpellToCast: " + sf.SpellToCast.SpellName, Color.Black);
                                    c.logText(Environment.NewLine, Color.Black);
                                }
                                int PcIndex = sf.TargetClosestPC(true); //(true) means that will ignore PCs in Stealth Mode
                                PC pc = sf.gm.playerList.PCList[PcIndex];
                                if (sf.SpellToCast.SpellTargetType == TargetType.PointLocation)
                                //if (sf.SpellToCast.TargetIsPointLocation)
                                {
                                    sf.CombatTarget = pc.CombatLocation; //we are using a Point type because the selected spell is looking for a Point type (square location)
                                }
                                else //we are assuming that TargetType is Enemy because this is a damage spell
                                {
                                    sf.CombatTarget = pc;
                                }
                                sf.ActionToTake = ChosenAction.CastSpell;
                                break;
                            }
                        }
                    }
                    if (sf.SpellToCast == null) //didn't find a spell that matched the criteria so use attack instead
                    {
                        int PCindex = sf.TargetClosestPC(true);
                        sf.CombatTarget = PCindex;
                        sf.ActionToTake = ChosenAction.MeleeRangedAttack;
                    }
                }
                else //does attack instead of spell due to being battle caster (percent chance of attack)
                {
                    int PCindex = sf.TargetClosestPC(true);
                    sf.CombatTarget = PCindex;
                    sf.ActionToTake = ChosenAction.MeleeRangedAttack;
                }
            }
        }
        public void Healer(ScriptFunctions sf, Combat c, Creature crt)
        {
            sf.SpellToCast = null;
            //find creature with the fewest HP
            Creature targetCrt = sf.GetCreatureWithLowestHP();
            if (targetCrt != null)
            {
                //choose the most efficient heal spell (close to HPMax-HP)
                foreach (string spTag in crt.KnownSpellsTags)
                {
                    if (sf.frm.debugMode)
                    {
                        c.logText("KnownSpellTag: " + spTag, Color.Black);
                        c.logText(Environment.NewLine, Color.Black);
                    }
                    Spell sp = sf.gm.module.ModuleSpellsList.getSpellByTag(spTag);
                    if (sp != null)
                    {
                        if (sf.frm.debugMode)
                        {
                            c.logText("KnownSpell: " + sp.SpellName, Color.Black);
                            c.logText(Environment.NewLine, Color.Black);
                        }
                        int mostSpellPointCost = 0;
                        if ((crt.SP >= sp.CostSP) && (sp.CostSP > mostSpellPointCost) && (sp.SpellEffectType == Spell.EffectType.Heal))
                        {
                            sf.SpellToCast = sp;
                            mostSpellPointCost = sp.CostSP;
                            if (sf.frm.debugMode)
                            {
                                c.logText("SpellToCast: " + sf.SpellToCast.SpellName, Color.Black);
                                c.logText(Environment.NewLine, Color.Black);
                            }
                        }
                    }
                }
                if (sf.SpellToCast != null)
                {
                    sf.CombatTarget = targetCrt;
                    sf.ActionToTake = ChosenAction.CastSpell;
                }
                else
                {
                    int PCindex = sf.TargetClosestPC(true);
                    sf.CombatTarget = PCindex;
                    sf.ActionToTake = ChosenAction.MeleeRangedAttack;
                }
            }            
            else
            {
                int PCindex = sf.TargetClosestPC(true);
                sf.CombatTarget = PCindex;
                sf.ActionToTake = ChosenAction.MeleeRangedAttack;
            }
        }
        public void BattleHealer(ScriptFunctions sf, Combat c, Creature crt)
        {
            int rnd = sf.gm.Random(1, 10);
            if (rnd > 5)
            {
                sf.SpellToCast = null;
                //find creature with the fewest HP
                Creature targetCrt = sf.GetCreatureWithLowestHP();
                if (targetCrt != null)
                {
                    //choose the most efficient heal spell (close to HPMax-HP)
                    foreach (string spTag in crt.KnownSpellsTags)
                    {
                        if (sf.frm.debugMode)
                        {
                            c.logText("KnownSpellTag: " + spTag, Color.Black);
                            c.logText(Environment.NewLine, Color.Black);
                        }
                        Spell sp = sf.gm.module.ModuleSpellsList.getSpellByTag(spTag);
                        if (sp != null)
                        {
                            if (sf.frm.debugMode)
                            {
                                c.logText("KnownSpell: " + sp.SpellName, Color.Black);
                                c.logText(Environment.NewLine, Color.Black);
                            }
                            int mostSpellPointCost = 0;
                            if ((crt.SP >= sp.CostSP) && (sp.CostSP > mostSpellPointCost) && (sp.SpellEffectType == Spell.EffectType.Heal))
                            {
                                sf.SpellToCast = sp;
                                mostSpellPointCost = sp.CostSP;
                                if (sf.frm.debugMode)
                                {
                                    c.logText("SpellToCast: " + sf.SpellToCast.SpellName, Color.Black);
                                    c.logText(Environment.NewLine, Color.Black);
                                }
                            }
                        }
                    }
                    if (sf.SpellToCast != null)
                    {
                        sf.CombatTarget = targetCrt;
                        sf.ActionToTake = ChosenAction.CastSpell;
                    }
                    else // didn't find an appropriate spell to cast (maybe not enough SP or spell that fits criteria)
                    {
                        int PCindex = sf.TargetClosestPC(true);
                        sf.CombatTarget = PCindex;
                        sf.ActionToTake = ChosenAction.MeleeRangedAttack;
                    }
                }
                else //didn't find a creature target
                {
                    int PCindex = sf.TargetClosestPC(true);
                    sf.CombatTarget = PCindex;
                    sf.ActionToTake = ChosenAction.MeleeRangedAttack;
                }
            }
            else //does attack instead of spell due to being battle caster (percent chance of attack)
            {
                int PCindex = sf.TargetClosestPC(true);
                sf.CombatTarget = PCindex;
                sf.ActionToTake = ChosenAction.MeleeRangedAttack;
            }
        }
        public void GeneralCaster(ScriptFunctions sf, Combat c, Creature crt)
        {
            sf.SpellToCast = null;
            //just pick a random spell from KnownSpells
            //try a few times to pick a random spell that has enough SP
            for (int i = 0; i < 10; i++)
            {
                int rnd = sf.gm.Random(0, crt.KnownSpellsTags.Count - 1);
                Spell sp = sf.gm.module.ModuleSpellsList.getSpellByTag(crt.KnownSpellsTags[rnd]);
                if (sp != null)
                {
                    if (sf.frm.debugMode)
                    {
                        c.logText("KnownSpell: " + sp.SpellName, Color.Black);
                        c.logText(Environment.NewLine, Color.Black);
                    }
                    if (sp.CostSP <= crt.SP)
                    {
                        sf.SpellToCast = sp;
                        if (sf.frm.debugMode)
                        {
                            c.logText("SpellToCast: " + sf.SpellToCast.SpellName, Color.Black);
                            c.logText(Environment.NewLine, Color.Black);
                        }

                        if (sf.SpellToCast.SpellTargetType == TargetType.Enemy)
                        {
                            int PcIndex = sf.TargetClosestPC(true); //(true) means that will ignore PCs in Stealth Mode
                            PC pc = sf.gm.playerList.PCList[PcIndex];
                            sf.CombatTarget = pc;
                            sf.ActionToTake = ChosenAction.CastSpell;
                            break;
                        }
                        else if (sf.SpellToCast.SpellTargetType == TargetType.PointLocation)
                        {
                            int PcIndex = sf.TargetClosestPC(true); //(true) means that will ignore PCs in Stealth Mode
                            PC pc = sf.gm.playerList.PCList[PcIndex];
                            sf.CombatTarget = pc.CombatLocation; //we are using a Point type because the selected spell is looking for a Point type (square location)
                            sf.ActionToTake = ChosenAction.CastSpell;
                            break;
                        }
                        else if (sf.SpellToCast.SpellTargetType == TargetType.Friend)
                        {
                            //target is another creature (currently assumed that spell is a heal spell)
                            Creature targetCrt = sf.GetCreatureWithLowestHP();
                            if (targetCrt != null)
                            {
                                sf.CombatTarget = targetCrt;
                                sf.ActionToTake = ChosenAction.CastSpell;
                                break;
                            }
                        }
                        else if (sf.SpellToCast.SpellTargetType == TargetType.Self)
                        {
                            //target is self (currently assumed that spell is a heal spell)
                            Creature targetCrt = crt;
                            if (targetCrt != null)
                            {
                                sf.CombatTarget = targetCrt;
                                sf.ActionToTake = ChosenAction.CastSpell;
                                break;
                            }
                        }
                        else //didn't find a target so set to null so that will use attack instead
                        {
                            sf.SpellToCast = null;
                        }
                    }
                }
            }
            if (sf.SpellToCast == null) //didn't find a spell that matched the criteria so use attack instead
            {
                int PCindex = sf.TargetClosestPC(true);
                sf.CombatTarget = PCindex;
                sf.ActionToTake = ChosenAction.MeleeRangedAttack;
            }
        }
        public void BattleGeneralCaster(ScriptFunctions sf, Combat c, Creature crt)
        {
            int r = sf.gm.Random(1, 10);
            if (r > 5)
            {
                sf.SpellToCast = null;
                //just pick a random spell from KnownSpells
                //try a few times to pick a random spell that has enough SP
                for (int i = 0; i < 10; i++)
                {
                    int rnd = sf.gm.Random(0, crt.KnownSpellsTags.Count - 1);
                    Spell sp = sf.gm.module.ModuleSpellsList.getSpellByTag(crt.KnownSpellsTags[rnd]);
                    if (sp != null)
                    {
                        if (sf.frm.debugMode)
                        {
                            c.logText("KnownSpell: " + sp.SpellName, Color.Black);
                            c.logText(Environment.NewLine, Color.Black);
                        }
                        if (sp.CostSP <= crt.SP)
                        {
                            sf.SpellToCast = sp;
                            if (sf.frm.debugMode)
                            {
                                c.logText("SpellToCast: " + sf.SpellToCast.SpellName, Color.Black);
                                c.logText(Environment.NewLine, Color.Black);
                            }

                            if (sf.SpellToCast.SpellTargetType == TargetType.Enemy)
                            {
                                int PcIndex = sf.TargetClosestPC(true); //(true) means that will ignore PCs in Stealth Mode
                                PC pc = sf.gm.playerList.PCList[PcIndex];
                                sf.CombatTarget = pc;
                                sf.ActionToTake = ChosenAction.CastSpell;
                                break;
                            }
                            else if (sf.SpellToCast.SpellTargetType == TargetType.PointLocation)
                            {
                                int PcIndex = sf.TargetClosestPC(true); //(true) means that will ignore PCs in Stealth Mode
                                PC pc = sf.gm.playerList.PCList[PcIndex];
                                sf.CombatTarget = pc.CombatLocation; //we are using a Point type because the selected spell is looking for a Point type (square location)
                                sf.ActionToTake = ChosenAction.CastSpell;
                                break;
                            }
                            else if (sf.SpellToCast.SpellTargetType == TargetType.Friend)
                            {
                                //target is another creature (currently assumed that spell is a heal spell)
                                Creature targetCrt = sf.GetCreatureWithLowestHP();
                                if (targetCrt != null)
                                {
                                    sf.CombatTarget = targetCrt;
                                    sf.ActionToTake = ChosenAction.CastSpell;
                                    break;
                                }
                            }
                            else if (sf.SpellToCast.SpellTargetType == TargetType.Self)
                            {
                                //target is self (currently assumed that spell is a heal spell)
                                Creature targetCrt = crt;
                                if (targetCrt != null)
                                {
                                    sf.CombatTarget = targetCrt;
                                    sf.ActionToTake = ChosenAction.CastSpell;
                                    break;
                                }
                            }
                            else //didn't find a target so set to null so that will use attack instead
                            {
                                sf.SpellToCast = null;
                            }

                            #region Old Way
                            /*if (!sf.SpellToCast.TargetIsPC) //a damage or debuff spell
                            {
                                int PcIndex = sf.TargetClosestPC(true); //(true) means that will ignore PCs in Stealth Mode
                                PC pc = sf.gm.playerList.PCList[PcIndex];
                                if (sf.SpellToCast.SpellTargetType == TargetType.PointLocation)
                                //if (sf.SpellToCast.TargetIsPointLocation) //target is a point on map usually for AoE spells
                                {
                                    sf.CombatTarget = pc.CombatLocation; //we are using a Point type because the selected spell is looking for a Point type (square location)
                                }
                                else //target is a PC not a Point on map
                                {
                                    sf.CombatTarget = pc;
                                }
                                sf.ActionToTake = ChosenAction.CastSpell;
                                break;
                            }
                            else //target is another creature (currently assumed that spell is a heal spell)
                            {
                                Creature targetCrt = sf.GetCreatureWithLowestHP();
                                if (targetCrt != null)
                                {
                                    sf.CombatTarget = targetCrt;
                                    sf.ActionToTake = ChosenAction.CastSpell;
                                    break;
                                }
                                else //didn't find a target so set to null so that will use attack instead
                                {
                                    sf.SpellToCast = null;
                                }
                            }*/
                            #endregion
                        }
                    }
                }
                if (sf.SpellToCast == null) //didn't find a spell that matched the criteria so use attack instead
                {
                    int PCindex = sf.TargetClosestPC(true);
                    sf.CombatTarget = PCindex;
                    sf.ActionToTake = ChosenAction.MeleeRangedAttack;
                }
            }
            else //does attack instead of spell due to being battle caster (percent chance of attack)
            {
                int PCindex = sf.TargetClosestPC(true);
                sf.CombatTarget = PCindex;
                sf.ActionToTake = ChosenAction.MeleeRangedAttack;
            }
        }
    }
}

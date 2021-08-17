﻿using System;
using System.Collections.Generic;

namespace NPCConsoleTesting
{
    public class Combat
    {
        private static readonly bool doReadLines = false;
        //private static readonly bool doReadLines = true;

        public static RoundResults CombatRound(List<Combatant> combatants)
        {
            ICombatMethods combatMethods = new CombatMethods();
            List<Combatant> charResults = new();
            List<String> logResults = new();
            
            combatants = combatMethods.DetermineTargets(combatants);
            combatants = combatMethods.DetermineInit(combatants);

            foreach (Combatant x in combatants)
            {
                Console.WriteLine($"{x.name} hp: {x.hp}");
            }
                
            int segment = 0;
            int priorityIndex = 0;
            int targetIndex = 0;
            bool opportunityForSimulAttack = false;

            while (priorityIndex <= combatants.Count - 1)
            {
                while (segment < combatants[priorityIndex].init)
                {
                    segment++;
                    opportunityForSimulAttack = false;
                }

                //set targetIndex based on priority combatant's target
                targetIndex = combatants.FindIndex(x => x.name == combatants[priorityIndex].target);

                //no attacks by or against dead combatants, unless there is a simultaneous attack
                if ((combatants[priorityIndex].hp <= 0 && !opportunityForSimulAttack) || combatants[targetIndex].hp <= 0)
                {
                    priorityIndex++;
                    break;
                }

                //Console.WriteLine($"It is segment {segment}, {combatants[priorityIndex].name} is about to attack {combatants[priorityIndex].target}");
                //if (doReadLines) { Console.ReadLine(); }
                
                //priority combatant does an attack against target
                int attackResult = combatMethods.Attack(combatants[priorityIndex].thac0, combatants[targetIndex].ac,
                    combatants[priorityIndex].numberOfAttackDice, combatants[priorityIndex].typeOfAttackDie, combatants[priorityIndex].dmgModifier);
                //Console.WriteLine($"attackResult: {attackResult}");
                //if (doReadLines) { Console.ReadLine(); }

                if (attackResult > 0)
                {
                    logResults.Add($"{combatants[priorityIndex].name} struck {combatants[targetIndex].name} for {attackResult} damage.");

                    //adjust target hp
                    combatants[targetIndex].hp -= attackResult;

                    if (combatants[targetIndex].hp <= 0)
                    {
                        logResults.Add($"{combatants[targetIndex].name} fell.");

                        if (combatants[targetIndex].init == segment)
                        {
                            opportunityForSimulAttack = true;
                        }
                    }

                    Console.WriteLine($"{combatants[priorityIndex].name} struck {combatants[targetIndex].name} for {attackResult} damage.");
                    Console.WriteLine($"{combatants[targetIndex].name} is at {combatants[targetIndex].hp}hp.");
                }
                else
                {
                    logResults.Add($"{combatants[priorityIndex].name} missed {combatants[targetIndex].name}.");
                }

                priorityIndex++;
            }

            //add combatants to charResults
            foreach (Combatant ch in combatants)
            {
                charResults.Add(ch);
            }

            return new RoundResults(charResults, logResults);
        }
    }
}

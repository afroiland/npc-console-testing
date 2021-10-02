﻿using NPCConsoleTesting.Combat;
using System.Collections.Generic;

namespace NPCConsoleTesting
{
    public interface ICombatMethods
    {
        int DoAMeleeAttack(IAttacker attacker, IDefender defender);
        int CalcMeleeDmg(string attackerClass, string weapon, int str, int ex_str, int magicalBonus, int otherDmgBonus = 0);
        void DetermineInit(List<Combatant> chars);
        void DetermineTargets(List<Combatant> chars);
        CombatantUpdateResults ApplyMeleeResultToCombatant(Combatant attacker, Combatant defender, int attackResult, int segment);
        CombatantUpdateResults ApplySpellResultToCombatant(Combatant caster, Combatant target, string spellName, SpellResults spellResults, int segment);
    }
}

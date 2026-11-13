#region

using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Monsters;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Extensions;

//Mostly utilities to get asset paths.
public static class AttackIntentExtensions
{
        public static int GetDamageTo(this AttackIntent attackIntent, IRunState runState, Creature recipient, Creature intentOwner)
        {
            Decimal val2 = attackIntent.DamageCalc();

            val2 = Hook.ModifyDamage(runState, recipient.CombatState, recipient, intentOwner, attackIntent.DamageCalc(), ValueProp.Move, (CardModel) null, (CardPlay) null, ModifyDamageHookType.All, CardPreviewMode.None, out IEnumerable<AbstractModel> _);
            
            return Math.Max(0, (int) val2);
        }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.token;

namespace SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.rare;

public class SomethingUnforgivable() : SpireEnigmasCard.SacrificeCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ("Threshold", 3),
        new DamageVar(99M, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Taboo>()
    ];
    
    protected override bool ShouldGlowGoldInternal => PlayedTaboosThisTurn(this) >= DynamicVars["Threshold"].IntValue;
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (PlayedTaboosThisTurn(this) >= DynamicVars["Threshold"].IntValue)
        {
            VfxCmd.PlayOnCreatureCenter(play.Target, VfxCmd.giantHorizontalSlashPath);
            VfxCmd.PlayOnCreatureCenter(play.Target, VfxCmd.bloodyImpactPath);
            await Cmd.Wait(0.25f);
            await CreatureCmd.Damage(choiceContext, play.Target, DynamicVars.Damage, this, play);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }

    public static int PlayedTaboosThisTurn(CardModel card)
    {
        return CombatManager.Instance.History.Entries.OfType<CardPlayFinishedEntry>().Count(
            e => e.CardPlay.Card is Taboo && e.RoundNumber == card.CombatState.RoundNumber
        );
    }
}
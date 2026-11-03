#region

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SpireEnigmas.SpireEnigmasCode.Cards.renegade.token;
using SpireEnigmas.SpireEnigmasCode.Powers;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Relics.renegade;

public class SelfPortrait : TheRenegadeRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;
    
    //public override RelicModel GetUpgradeReplacement() => ModelDb.Relic<PerfectedCore>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Ward>(),
        HoverTipFactory.FromKeyword(CardKeyword.Retain),
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.FromPower<DodgePower>(),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature) || Owner.PlayerCombatState.TurnNumber > 1)
            return;
        
        await CardPileCmd.AddGeneratedCardsToCombat([Owner.Creature.CombatState!.CreateCard<Ward>(Owner)],
            PileType.Hand, Owner);
    }
}
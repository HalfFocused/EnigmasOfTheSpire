using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.uncommon;

public class AnimalInstinct() : SpireEnigmasCard.SacrificeCard(4, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(20M, ValueProp.Move)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).TargetingAllOpponents(CombatState).WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "heavy_attack.mp3").Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6);
    }
    
    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this || IsClone)
            return Task.CompletedTask;
        ReduceCostBy(CombatManager.Instance.History.Entries.OfType<CardDrawnEntry>().Count(e => e.HappenedThisTurn(card.CombatState) && e.Actor == card.Owner.Creature && !e.FromHandDraw));
        return Task.CompletedTask;
    }

    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw)
    {
        if (fromHandDraw || card.Owner != Owner || card.Owner.Creature.CombatState.CurrentSide != card.Owner.Creature.Side)
            return;
        
        ReduceCostBy(1);
    }

    public void ReduceCostBy(int amount) => EnergyCost.AddThisTurn(-amount);
}
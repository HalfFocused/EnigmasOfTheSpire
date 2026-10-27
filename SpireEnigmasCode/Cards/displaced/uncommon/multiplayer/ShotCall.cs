using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using SpireEnigmas.SpireEnigmasCode.Cards.displaced.token;
using SpireEnigmas.SpireEnigmasCode.Commands;

namespace SpireEnigmas.SpireEnigmasCode.Cards.displaced.uncommon;

public class ShotCall() : SpireEnigmasCard.DisplacedCard(0,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromCard<Vision>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        if (CombatState == null)
            return;
        foreach (Creature teammate in CombatState.GetTeammatesOf(Owner.Creature).Where(c => c != null && c.IsAlive && c.IsPlayer))
        {
            await CardPileCmd.AddGeneratedCardsToCombat([CombatState.CreateCard<Vision>(teammate.Player)],
                PileType.Hand, Owner);
            await Cmd.Wait(0.1f);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
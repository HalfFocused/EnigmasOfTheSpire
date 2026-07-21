using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.token;

namespace SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.uncommon;

public class Distort() : SpireEnigmasCard.SacrificeCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(11M, ValueProp.Move),
        new CardsVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Taboo>()
    ];


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 0, DynamicVars.Cards.IntValue);

        foreach (CardModel selectedCard in await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this))
        {
            CardModel taboo = CombatState.CreateCard<Taboo>(Owner);
            await CardCmd.Transform(selectedCard, taboo);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
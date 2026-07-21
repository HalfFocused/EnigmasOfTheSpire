using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Character;
using SpireEnigmas.SpireEnigmasCode.Character.displaced;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Cards.displaced.common;

[Pool(typeof(TheDisplacedCardPool))]
public class EtchedInBlood() : SpireEnigmasCard.DisplacedCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6M, ValueProp.Move)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
        CardModel selectedCard = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this)).FirstOrDefault<CardModel>();
        if(selectedCard == null) return;
        await CardCmd.Exhaust(choiceContext, selectedCard);
        (await PowerCmd.Apply<EtchedInBloodPower>(choiceContext, Owner.Creature, 1M, Owner.Creature, this)).SetSelectedCard(selectedCard);
    }
    
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3M);
}
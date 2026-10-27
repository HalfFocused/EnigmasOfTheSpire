using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Patches;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Cards.chronicle.common;

public class Contradict() : SpireEnigmasCard.ChronicleCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(4M, ValueProp.Move)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        
        CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        CardModel? card = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, (Func<CardModel, bool>) (c => !c.Keywords.Contains(CardKeyword.Retain)), this)).FirstOrDefault();
        if (card == null)
            return;
        EnigmaCmd.GiveRetainUntilPlayed(card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2M);
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
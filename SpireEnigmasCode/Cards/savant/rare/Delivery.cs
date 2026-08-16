using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.rare;

public class Delivery() : SpireEnigmasCard.SavantCard(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override bool GainsBlock => true;

    //we reference the canonical bomb model for our values so that this card changes automatically if the bomb is changed
    private CardModel CanonicalBombModel
    {
        get
        {
            CardModel theBomb = ModelDb.Card<TheBomb>().ToMutable();
            if (IsUpgraded)
            {
                theBomb.UpgradeInternal();
                theBomb.FinalizeUpgradeInternal();
            }

            return theBomb;
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        ChirpHoverTip(),
        HoverTipFactory.FromCard<TheBomb>(IsUpgraded)
    ];
    
    protected override bool ShouldGlowRedInternal => GetChirp == null;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(12M, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        if(GetChirp is null) return;
        (await PowerCmd.Apply<TheBombPower>(choiceContext, GetChirp, CanonicalBombModel.DynamicVars["Turns"].BaseValue, Owner.Creature, this))?.SetDamage(CanonicalBombModel.DynamicVars["BombDamage"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4);
    }
}
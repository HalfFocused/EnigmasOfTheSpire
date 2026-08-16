using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Powers;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.uncommon;

public class Smokescreen() : SpireEnigmasCard.SavantCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ChirpBlockVar(5M, ValueProp.Move)
    ];
    
    protected override bool ShouldGlowRedInternal => GetChirp == null;
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        EnigmaEnums.Command
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        ChirpHoverTip()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if(GetChirp is null) return;
        Decimal amount = await ChirpCmd.GiveBlockToOwner(Owner, DynamicVars["ChirpBlock"].BaseValue, ((ChirpBlockVar) DynamicVars["ChirpBlock"]).Props, play);
        
        await PowerCmd.Apply<ChirpBlockNextTurnPower>(choiceContext, GetChirp, amount, GetChirp, this);
    }
    
    protected override void OnUpgrade() => DynamicVars["ChirpBlock"].UpgradeValueBy(2M);
}
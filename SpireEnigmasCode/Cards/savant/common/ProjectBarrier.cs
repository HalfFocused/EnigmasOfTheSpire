using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.common;

public class ProjectBarrier() : SpireEnigmasCard.SavantCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override bool ShouldGlowRedInternal => ChirpHelper.GetChirpFromPlayer(Owner) == null;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ChirpBlockVar(10M, ValueProp.Move)
    ];
    
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
        await ChirpCmd.GiveBlockToOwner(Owner, DynamicVars["ChirpBlock"].BaseValue, ((ChirpBlockVar) DynamicVars["ChirpBlock"]).Props, play);
    }
    
    protected override void OnUpgrade() => DynamicVars["ChirpBlock"].UpgradeValueBy(4M);
}
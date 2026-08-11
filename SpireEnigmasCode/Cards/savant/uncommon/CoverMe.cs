using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Commands;

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.uncommon;

public class CoverMe() : SpireEnigmasCard.SavantCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8M, ValueProp.Move),
        new ChirpBlockVar(10M, ValueProp.Move)
    ];
    
    //can glow red and gold. fun
    protected override bool ShouldGlowRedInternal => GetChirp == null;
    protected override bool ShouldGlowGoldInternal => HasChirpAttackedThisTurn;
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        //EnigmaKeywords.Command,
        //CardKeyword.Retain
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        ChirpHoverTip()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        if (GetChirp is null) return;
        if (HasChirpAttackedThisTurn)
        {
            await Cmd.CustomScaledWait(0.35f, 0.5f);
            await ChirpCmd.GiveBlockToOwner(Owner, DynamicVars["ChirpBlock"].BaseValue, ((ChirpBlockVar) DynamicVars["ChirpBlock"]).Props, play);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
        DynamicVars["ChirpBlock"].UpgradeValueBy(2M);
    }
    
    public bool HasChirpAttackedThisTurn
    {
        get
        {
            return CombatManager.Instance.History.Entries.OfType<CreatureAttackedEntry>().Any(e => e.Actor == GetChirp && e.HappenedThisTurn(CombatState));
        }
    }
}
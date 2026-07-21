using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Cards.displaced.rare;

public class TheSecondDream() : SpireEnigmasCard.DisplacedCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];
    
    protected override bool ShouldGlowGoldInternal => EtherealCardsPlayedThisTurn() >= 3;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (EtherealCardsPlayedThisTurn() >= 3)
        {
            NCombatRoom.Instance?.RadialBlur(VfxPosition.Left);
            NGame.Instance?.DoHitStop(ShakeStrength.Strong, ShakeDuration.Normal);
            await PowerCmd.Apply<TheSecondDreamPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        }
    }

    private int EtherealCardsPlayedThisTurn()
    {
        return CombatManager.Instance.History.Entries.OfType<CardPlayStartedEntry>().Count(
            e => e.CardPlay.Card.Keywords.Contains(CardKeyword.Ethereal) && e.RoundNumber == CombatState.RoundNumber
        );
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Events;
using SpireEnigmas.SpireEnigmasCode.Util;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Cards.chronicle.uncommon;

public class Cliffhanger() : SpireEnigmasCard.ChronicleCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy),
    IRenderStory
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(32M, ValueProp.Move)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(EnigmaHoverTips.Story)
    ];

    protected override bool IsPlayable =>
        StoryManager.InChapter(this, StoryManager.PlayerMaxStoryLength(Owner),
            StoryManager.CardInChapterTimings.BeforePlay);

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target!).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(8);
    }
}
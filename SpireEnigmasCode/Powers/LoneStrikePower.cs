using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Events;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class LoneStrikePower : SpireEnigmaPower, IRenderStory, IAfterStoryEnd
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    
    public override bool ShouldPlay(CardModel card, AutoPlayType _)
    {
        if (card.Owner != Owner.Player)
            return true;
        return card.Type != CardType.Attack;
    }

    public async Task AfterStoryEnd(PlayerChoiceContext choiceContext, Player player, List<CardModel> storyState)
    {
        if(player != Owner.Player) return;
        await PowerCmd.Remove(this);
    }
}
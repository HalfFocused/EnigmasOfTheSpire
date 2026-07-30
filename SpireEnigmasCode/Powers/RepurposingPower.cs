using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class RepurposingPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Transform)
    ];

    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw)
    {
        if (card.Owner.Creature != Owner || card.Type != CardType.Status || CombatManager.Instance.History.Entries.OfType<CardDrawnEntry>().Count(e => e.Card.Type == CardType.Status && e.Card.Owner == Owner.Player && e.HappenedThisTurn(CombatState)) > Amount)
            return;
        Flash();
        
        CardModel newCard = CardFactory.GetDistinctForCombat(Owner.Player, Owner.Player.Character.CardPool.GetUnlockedCards(Owner.Player.UnlockState, Owner.Player.RunState.CardMultiplayerConstraint), 1, Owner.Player.RunState.Rng.CombatCardGeneration).FirstOrDefault();
        await CardCmd.Transform(card, newCard, CardPreviewStyle.None);
    }
}
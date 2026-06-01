using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace SpireEnigmas.SpireEnigmasCode;

public class EnigmaHooks() : CustomSingletonModel(HookType.Combat)
{
    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw)
    {
        if (!card.Keywords.Contains(EnigmaKeywords.Improvise) || fromHandDraw || card.CombatState.CurrentSide != card.Owner.Creature.Side)
            return;
        if (CombatManager.Instance.History.Entries.OfType<CardDrawnEntry>()
                .Count(e => e.Card == card && !e.FromHandDraw && e.RoundNumber == card.CombatState.RoundNumber) != 1)
        {
            card.EnergyCost.SetThisTurn(0);
        }
    }
    
}
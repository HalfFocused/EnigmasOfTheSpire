#region

using BaseLib.Patches.Hooks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class SovereignFormPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;
    
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;
        Flash();
        await CardPileCmd.Draw(choiceContext, MaxHandSizePatch.GetMaxHandSize(player, CardPile.MaxCardsInHand) - Owner.Player.PlayerCombatState.Hand.Cards.Count, Owner.Player);
    }
}
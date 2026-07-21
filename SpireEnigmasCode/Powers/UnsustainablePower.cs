using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class UnsustainablePower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    
    public override async Task AfterShuffle(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;
        
        var randomCard = PileType.Draw.GetPile(Owner.Player).Cards.ToList().StableShuffle(Owner.Player.RunState.Rng.Shuffle).FirstOrDefault() ?? PileType.Draw.GetPile(Owner.Player).Cards.ToList().StableShuffle(Owner.Player.RunState.Rng.Shuffle).FirstOrDefault();
        if (randomCard == null)
            return;
        await CardCmd.Exhaust(choiceContext, randomCard);
    }
}
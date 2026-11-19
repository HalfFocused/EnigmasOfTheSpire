#region

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Cards.other;
using SpireEnigmas.SpireEnigmasCode.Util;

#endregion

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class PanicPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override Decimal ModifyHandDraw(Player player, Decimal count)
    {
        return player != Owner.Player ? count : count + Amount;
    }
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromCard<Paranoia>()
    ];
    
    public override async Task AfterShuffle(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;
        
        Flash();

        List<CardModel> paranoias = new();

        for (int i = 0; i < Amount; i++)
        {
            paranoias.Add(Owner.CombatState.CreateCard<Paranoia>(Owner.Player));
        }
        
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(paranoias, PileType.Draw, Owner.Player, CardPilePosition.Random));
    }
    
}
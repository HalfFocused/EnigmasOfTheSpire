using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.token;
using SpireEnigmas.SpireEnigmasCode.Util;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class HeartlessActPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromCard<Taboo>(),
        HoverTipFactory.FromPower<StrengthPower>()
    ];
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player || cardPlay.Card is not Taboo)
            return;

        foreach (Creature hittableEnemy in CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<PiercingWailPower>(choiceContext, hittableEnemy, Amount, Owner, null);
        }
    }
}
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Cards.other;

namespace SpireEnigmas.SpireEnigmasCode.Modifiers;

public class TransformsIntoParanoiaThisTurnModifier : CardModifier
{
    public override void ModifyDescription(Creature? target, ref string description)
    {
        description += "\n[gold]Transform[/gold] this into [gold]Paranoia[/gold].";
    }
    
    public override void AddTips(List<IHoverTip> tips)
    {
        tips.Add(HoverTipFactory.FromCard<Paranoia>());
    }

    public override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Owner!.TransformIntoAfterPlay = Owner!.CombatState!.CreateCard<Paranoia>(Owner.Owner);
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner!.Owner.Creature))
        {
            RemoveModifier(Owner, this);
        }

        return Task.CompletedTask;
    }
}
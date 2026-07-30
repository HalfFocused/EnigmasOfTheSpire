using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Monsters;

namespace SpireEnigmas.SpireEnigmasCode.Extensions;

//Mostly utilities to get asset paths.
public static class AttackCommandExtensions
{
    public static AttackCommand FromChirp(this AttackCommand command, Creature chirp, CardModel card, CardPlay? cardPlay)
    {
        command.Attacker = chirp.Monster is Chirp ? chirp : throw new ArgumentException("Creature is not Chirp");
        command.ModelSource = card;
        command.CardPlay = cardPlay;
        command._attackerAnimName = "attack";
        command._attackerAnimDelay = 0.3f;
        command._sourceType = AttackCommand.SourceType.Card;
        return command.WithAttackerFx(sfx: "event:/sfx/characters/osty/osty_attack");
    }
}
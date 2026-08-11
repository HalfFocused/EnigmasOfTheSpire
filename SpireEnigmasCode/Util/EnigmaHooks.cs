using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using SpireEnigmas.SpireEnigmasCode.Cards.displaced.rare;

namespace SpireEnigmas.SpireEnigmasCode.Util;

public class EnigmaHooks() : CustomSingletonModel(HookType.Combat)
{
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel playedCard = cardPlay.Card;
        Player owner = playedCard.Owner;

        if (playedCard.Keywords.Contains(EnigmaKeywords.TimeLoop)) return;

        foreach (CardModel card in owner.PlayerCombatState.AllCards.ToList())
        {
            if (card.Keywords.Contains(EnigmaKeywords.TimeLoop) || card is TimeLoop)
            {
                CardModel result = playedCard.CreateClone();
                CardCmd.ApplyKeyword(result, EnigmaKeywords.TimeLoop);

                await CardCmd.Transform(card, result, card.Pile.Type == PileType.Hand ? CardPreviewStyle.HorizontalLayout : CardPreviewStyle.None);
            }
        }
    }

    /*
     * Singleton hook with one purpose: Chirp Bomb
     * Simulate Chirp's Bomb Power going down when it should.
     * Curse you MegaCrit!!!
     */
    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        foreach (Creature participant in participants)
        {
            Creature? chirp = ChirpHelper.GetChirpFromPlayer(participant.Player);
            if (chirp != null)
            {
                foreach (PowerModel chirpPower in chirp.Powers.ToList())
                {
                    if (chirpPower is TheBombPower)
                    {
                        if (chirpPower.Amount > 1)
                        {
                            await PowerCmd.Decrement(chirpPower);
                        }
                        else
                        {
                            chirpPower.Flash();
                            await Cmd.CustomScaledWait(0.2f, 0.4f);
                            foreach (Creature hittableEnemy in chirpPower.CombatState.HittableEnemies)
                            {
                                NCombatRoom instance = NCombatRoom.Instance;
                                if (instance != null)
                                    instance.CombatVfxContainer.AddChildSafely((Node) NFireSmokePuffVfx.Create(hittableEnemy));
                            }
                            await Cmd.CustomScaledWait(0.2f, 0.4f);
                            await CreatureCmd.Damage(choiceContext, chirpPower.CombatState.HittableEnemies, chirpPower.DynamicVars.Damage, chirpPower.Owner);
                            await PowerCmd.Remove(chirpPower);
                        }
                    }
                }
            }
        }
    }
}
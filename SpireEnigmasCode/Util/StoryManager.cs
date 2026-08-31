using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Rooms;
using SpireEnigmas.SpireEnigmasCode.Events;
using SpireEnigmas.SpireEnigmasCode.Patches;

namespace SpireEnigmas.SpireEnigmasCode.Util;

public class StoryManager() : CustomSingletonModel(HookType.Combat)
{
    private static List<Node> activeDisplays = new();

    private const int DefaultStoryLength = 5;

    /*
     * It's surprisingly hard to robustly check "is card X in Chapter Y" do to the variety of timings we might ask that
     * question during.
     *
     * Thus, the method for performing this check also expects a timing.
     * BeforePlay:
     * Used to check if the next card played *would* occupy chapter Y.
     * Useful for things like Hand glow.
     * 
     * Resolution: Used to check if the currently resolving card was played into chapter Y.
     * Useful for conditional effects.
     * 
     * BeforePlayAndResolution:
     * BeforePlay OR Resolution.
     * Useful for effects that must be both previewed and happen, like conditional bonus damage in a certain chapter.
     *
     * NonActive:
     * Not concerned with what's currently happening. NonActive checks if a certain card is in an already filled Chapter.
     * Useful for non-resolution card effects that need to check if they are in the Story, like
     * "At the end of the Story, return this to your Hand."
     */
    public enum CardInChapterTimings
    {
        BeforePlay,
        Resolution,
        BeforePlayAndResolution,
        NonActive
    }
    
    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (!(room is CombatRoom combatRoom))
            return;

        foreach (Player player in combatRoom.CombatState.Players)
        {
            StoryFields.PlayerStory.Set(player, []);
            StoryFields.PlayerStoryMaxLength.Set(player, DefaultStoryLength);
            StoryFields.RunEndPlayerStoryHooks.Set(player, false);
            StoryFields.RunStartPlayerStoryHooks.Set(player, false);
        }
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        CardModel card = cardPlay.Card;
        Player player = card.Owner;
        
        //if(cardPlay.IsAutoPlay) return;

        List<CardModel>? cards = StoryFields.PlayerStory.Get(player);

        if (cards is null) throw new Exception("Player Story was null!");
        
        if (cards.Count == StoryFields.PlayerStoryMaxLength.Get(player))
        {
            cards.Clear();
            StoryFields.RunStartPlayerStoryHooks.Set(player, true);
        }
        
        cards.Add(card.CreateClone());
        StoryFields.ChapterPlayedInto.Set(card, cards.Count);
        
        if (cards.Count == StoryFields.PlayerStoryMaxLength.Get(player))
        {
            await EndStory(player, card.CombatState, false);
        }
            
        UpdateStoryVisuals(player, card.CombatState);
    }

    /*
     * Marks the Story to be ended after this the current card resolution.
     * We don't do it immediately so that strange timings don't make card previews inaccurate.
     * This might be able to be retrofitted so that external ends do run hooks immediately,
     * as long as I make sure to not put any card effects after an end turn that might care.
     */
    public static async Task EndStory(Player player, ICombatState combatState, bool external = true)
    {
        if (external)
        {
            List<CardModel>? cards = StoryFields.PlayerStory.Get(player);
            cards.Clear();
        }
        StoryFields.RunEndPlayerStoryHooks.Set(player, true);
        UpdateStoryVisuals(player, combatState);
    }
    
    public static async Task SetStoryLength(Player player, ICombatState combatState, int length)
    {
        StoryFields.PlayerStoryMaxLength.Set(player, length);
        UpdateStoryVisuals(player, combatState);
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel card = cardPlay.Card;
        Player player = card.Owner;
        
        List<CardModel>? cards = StoryFields.PlayerStory.Get(player);
        
        
        /*
         * We end the story *after* the card that ended it finishes playing.
         * This is so that things don't go away after the card is played but before it happens.
         * Can lead to weird card-in-card scenarios where card A marks the story to be finished
         * when played, queues card B as part of its resolution (which enters the next story), card B
         * benefits from the current story, then the current story hooks are dispatched.
         */
        if (StoryFields.RunEndPlayerStoryHooks.Get(player))
        {
            await EnigmaHooks.Dispatch<IAfterStoryEnd>(card.CombatState, m => m.AfterStoryEnd(choiceContext, player));
            StoryFields.RunEndPlayerStoryHooks.Set(player, false);
        }
        
        /*
         * This happens after
         */
        
        if (StoryFields.RunStartPlayerStoryHooks.Get(player))
        {
            await EnigmaHooks.Dispatch<IAfterStoryStart>(card.CombatState, m => m.AfterStoryStart(choiceContext, player));
            StoryFields.RunStartPlayerStoryHooks.Set(player, false);
        }
        
        StoryFields.ChapterPlayedInto.Set(card, -1);
    }

    private static void UpdateStoryVisuals(Player player, ICombatState? combatState)
    {
        List<CardModel>? cards = StoryFields.PlayerStory.Get(player);
        
        if (combatState is null) return;
        if(!LocalContext.IsMe(player) || !ShouldDoVfx(combatState)) return;

        NCreature? playerCreature = player.Creature.GetCreatureNode();
        if(playerCreature is null) return;
        
        foreach(Node display in activeDisplays)
        {
            if (GodotObject.IsInstanceValid(display))
            {
                display.QueueFree();
            }
        }
        activeDisplays.Clear();
        
        for (int i = 0; i < cards.Count; i++)
        {
            CardModel? card = cards.ElementAtOrDefault(i);
            if (card is null) break;
            
            NCard? cardNode = NCard.Create(card);
            if(cardNode is null) break;
            
            NPreviewCardHolder display = NPreviewCardHolder.Create(cardNode, true, true);

            int storyMaxLength = StoryFields.PlayerStoryMaxLength.Get(player);
            
            display.SetPosition(
                (Vector2.Left * 125 * ((storyMaxLength - 1) / 2)) + 
                (Vector2.Right * 125 * i) + 
                Vector2.Up * 400);
            display.SetCardScale(new Vector2(0.35f, 0.35f));
            
            activeDisplays.Add(display);
            playerCreature.AddChildSafely(display);
            cardNode.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
        }
    }
    
    public static bool InChapter(CardModel card, int chapter, CardInChapterTimings timing)
    {
        switch (timing)
        {
            case CardInChapterTimings.BeforePlay:
                return NextCardChapter(card.Owner) == chapter;
            case CardInChapterTimings.Resolution:
                return StoryFields.ChapterPlayedInto.Get(card) == chapter;
            case CardInChapterTimings.BeforePlayAndResolution:
                return InChapter(card, chapter, CardInChapterTimings.BeforePlay) ||
                    InChapter(card, chapter, CardInChapterTimings.Resolution);
            case CardInChapterTimings.NonActive:
                return CardInChapter(card.Owner, chapter) == card;
            default:
                return false;
        }
    }

    public static int NextCardChapter(Player player)
    {
        int currentStoryLength = PlayerCurrentStoryLength(player);
        if (currentStoryLength == 0 || currentStoryLength == PlayerMaxStoryLength(player))
        {
            return 1;
        }

        return currentStoryLength + 1;
    }
    
    /*
     * Checks the card in a given chapter of the story.
     * Even though the story contains clones, this returns the original card that was played into the Chapter.
     */
    public static CardModel? CardInChapter(Player player, int chapter)
    {
        List<CardModel>? cards = StoryFields.PlayerStory.Get(player);
        return cards.ElementAtOrDefault(chapter - 1)?.CloneOf;
    }
    
    public static int PlayerCurrentStoryLength(Player player)
    {
        List<CardModel>? cards = StoryFields.PlayerStory.Get(player);
        return cards.Count;
    }
    
    public static int PlayerMaxStoryLength(Player player)
    {
        return StoryFields.PlayerStoryMaxLength.Get(player);
    }
    
    private static bool ShouldDoVfx(ICombatState combatState)
    {
        return combatState.IterateHookListeners().OfType<IShouldRenderStory>().Any();
    }
}
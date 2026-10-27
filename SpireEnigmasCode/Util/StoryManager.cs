using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using SpireEnigmas.SpireEnigmasCode.Events;
using SpireEnigmas.SpireEnigmasCode.Patches;


namespace SpireEnigmas.SpireEnigmasCode.Util;

public class StoryManager() : CustomSingletonModel(HookType.Combat)
{
    private static List<Node> activeDisplays = new();

    private const int DefaultStoryLength = 5;

    /*
     * It's surprisingly hard to robustly check "is card X in Chapter Y" due to the variety of timings we might ask that
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
    
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.PlayerCombatState!.TurnNumber != 1)
            return;
       
        StoryFields.PlayerStory.Set(player, []);
        StoryFields.PlayerStoryMaxLength.Set(player, DefaultStoryLength);
        /*
         * We make this one true so that Story start hooks are dispatched on the first card of the combat.
         */
        StoryFields.PlayerStoryEnded.Set(player, true);
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        CardModel card = cardPlay.Card;
        Player player = card.Owner;
        
        List<CardModel>? cards = StoryFields.PlayerStory.Get(player);

        if (cards is null) throw new Exception("Player Story was null!");
        

        bool canNeverEnterStory = cardPlay.IsAutoPlay || card.IsDupe || !cardPlay.IsFirstInSeries;

        if (canNeverEnterStory)
        {
            StoryFields.AllowedToEnterStory.Set(card, false);
            return;
        }
        
        //This needs to be set now for resolution-time checks
        StoryFields.AllowedToEnterStory.Set(card, IsCardAllowedToEnterStory(card.CombatState!, player, card));
        StoryFields.ChapterPlayedInto.Set(card, NextCardChapter(player));
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel card = cardPlay.Card;
        Player player = card.Owner;
        
        if(!StoryFields.AllowedToEnterStory.Get(card)) return;
        
        List<CardModel>? cards = StoryFields.PlayerStory.Get(player);
        
        /*
         * if the player story was ended, start a new story by clearing the cards and dispatching story start hooks
         * we can't just check if the story is full because of forceful story ending effects.
         * this field also starts as true (which is imprecise i guess) to make the first card also do this
         */
        if (StoryFields.PlayerStoryEnded.Get(player))
        {
            cards.Clear();
            UpdateStoryVisuals(player, card.CombatState);
            await EnigmaHooks.Dispatch<IAfterStoryStart>(card.CombatState, m => m.AfterStoryStart(choiceContext, player));
            
            StoryFields.PlayerStoryEnded.Set(player, false);
        }
        
        cards.Add(card.CreateClone());
        UpdateStoryVisuals(player, card.CombatState);
        
        //If this card led to the story being full, end the current story (without clearing it, for clarity)
        if (cards.Count == PlayerMaxStoryLength(player))
        {
            await EndStory(choiceContext, player, card.CombatState!, false);
        }
        
        //now that the card is done, set its internal resolution chapter field back to 0
        StoryFields.AllowedToEnterStory.Set(card, true);
        StoryFields.ChapterPlayedInto.Set(card, 0);
    }

    /*
     * End the Story
     * Hooks are dispatched for an ended story. The hook carries a copy of the state of the story.
     * When this happens naturally, the story isn't emptied until the next card is played (mostly for clarity)
     * When this is made to happen forcefully, the story is immediately cleared (also for clarity)
     *
     * Because forcefully ending the Story can change enemy + player powers, it should go after card effects that might
     * depend on those powers, like damage and block. In general, assume it should go last in a list of card effects.
     */
    public static async Task EndStory(PlayerChoiceContext choiceContext, Player player, ICombatState combatState, bool andEmpty = true)
    {
        List<CardModel> cards = StoryFields.PlayerStory.Get(player)!;
        await EnigmaHooks.Dispatch<IAfterStoryEnd>(combatState, m => m.AfterStoryEnd(choiceContext, player, cards.ToList()));
        if (andEmpty)
        {
            cards.Clear();
        }
        //mark player story as having been ended, so the next card play knows to start a new story
        StoryFields.PlayerStoryEnded.Set(player, true);

        UpdateStoryVisuals(player, combatState);
    }
    
    /*
     * Clear a player's Story. This is currently not used internally.
     *
     * Clearing the Story should usually be last on a card's list of effects for a similar reason to ending the story.
     * Though clearing the Story doesn't dispatch hooks, it can still cause issues with in-chapter timings, like
     * Chronicle's starter relic.
     */
    public static async Task ClearStory(PlayerChoiceContext choiceContext, Player player, ICombatState combatState)
    {
        List<CardModel> cards = StoryFields.PlayerStory.Get(player)!;
        cards.Clear();
        
        UpdateStoryVisuals(player, combatState);
    }
    
    /*
     * Sets a player's default story length this combat
     * (the length of the player's story before any hooks modify it.)
     */
    public static async Task SetDefaultStoryLength(Player player, ICombatState combatState, int length)
    {
        StoryFields.PlayerStoryMaxLength.Set(player, length);
        UpdateStoryVisuals(player, combatState);
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
        
        for (int i = 0; i < cards?.Count; i++)
        {
            CardModel? card = cards.ElementAtOrDefault(i);
            if (card is null) break;
            
            NCard? cardNode = NCard.Create(card);
            if(cardNode is null) break;
            
            NPreviewCardHolder? display = NPreviewCardHolder.Create(cardNode, true, true);

            int storyMaxLength = PlayerMaxStoryLength(player);
            
            display?.SetPosition(
                (Vector2.Left * 250 + 
                (Vector2.Right * (500f / (storyMaxLength - 1)) * i) + 
                Vector2.Up * 400));
            display?.SetCardScale(new Vector2(0.35f, 0.35f));
            
            activeDisplays.Add(display);
            playerCreature.AddChildSafely(display);
            cardNode.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
        }
    }
    
    public static bool InChapter(CardModel card, int chapter, CardInChapterTimings timing)
    {
        if (!IsCardAllowedToEnterStory(card.CombatState, card.Owner, card)) return false;
        return timing switch
        {
            CardInChapterTimings.BeforePlay => NextCardChapter(card.Owner) == chapter,
            CardInChapterTimings.Resolution => StoryFields.ChapterPlayedInto.Get(card) == chapter,
            CardInChapterTimings.BeforePlayAndResolution => InChapter(card, chapter, CardInChapterTimings.BeforePlay) ||
                                                            InChapter(card, chapter, CardInChapterTimings.Resolution),
            CardInChapterTimings.NonActive => CardInChapter(card.Owner, chapter) == card,
            _ => false
        };
    }
    
    /// <summary>
    /// Internal method for determining the next Chapter a card will be played into.
    /// This method does not know if the card will actually be allowed to enter that spot or not.
    /// It only knows where the card "should" end up.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="combatState"></param>
    /// <returns></returns>
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
        return cards?.ElementAtOrDefault(chapter - 1)?.CloneOf;
    }

    public static int PlayerCurrentStoryLength(Player player)
    {
        List<CardModel>? cards = StoryFields.PlayerStory.Get(player);
        return cards.Count;
    }
    
    /*
     * Returns the current maximum length of a player's story.
     * The value starts with the player's PlayerStoryMaxLength field and then is modified by IModifyStoryLength hooks.
     */
    public static int PlayerMaxStoryLength(Player player)
    {
        int defaultStoryLength = StoryFields.PlayerStoryMaxLength.Get(player);
        return EnigmaHooks.Modify<IModifyStoryLength, int>(player.Creature.CombatState, defaultStoryLength, (m, amount) => m.ModifyStoryLength(amount, player), out _);
    }
    
    private static bool ShouldDoVfx(ICombatState combatState)
    {
        return combatState.IterateHookListeners().OfType<IRenderStory>().Any();
    }

    /// <summary>
    /// Returns whether or not a card is allowed to enter the Story.
    /// This is important to know even before resolution time to stop cards conditional on being in certain chapter from "glowing" or previewing inaccurately.
    /// The result of this method is used for the public-facing NextCardChapter method.
    /// </summary>
    /// <param name="combatState">The Combat State needed make this check.</param>
    /// <param name="player">The player we are performing the check on. We cannot just get this from the card because card is a nullable field.</param>
    /// <param name="card">The specific card being checked. If null, the check is asking if *any* card is allowed to enter the story.</param>
    /// <returns>True if the card is allowed to enter the story. False otherwise.</returns>
    public static bool IsCardAllowedToEnterStory(ICombatState combatState, Player player, CardModel? card)
    {
        return !EnigmaHooks.Any<IShouldPreventCardAddedToStory>(combatState,
            m => m.ShouldPreventCardAddedToStory(player, card));
    }

    /*
     * Returns the Player's current story.
     * Remember the story holds clones of cards- not the actual cards themselves.
     */
    public static List<CardModel> GetStoryState(Player player)
    {
        List<CardModel>? cards = StoryFields.PlayerStory.Get(player);
        if (cards is null) throw new Exception("Player Story was null!");
        return cards;
    }
}
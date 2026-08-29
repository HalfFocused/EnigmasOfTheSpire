using BaseLib.Common.Rewards.LinkedRewardSet;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using SpireEnigmas.SpireEnigmasCode.Commands;
using SpireEnigmas.SpireEnigmasCode.Powers;

namespace SpireEnigmas.SpireEnigmasCode.Relics.displaced;

public class ShatteredGlass : TheDisplacedRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
    ];
    
    public override bool IsAllowed(IRunState runState)
    {
        return IsBeforeAct3TreasureChest(runState);
    }

    public override bool TryModifyRewards(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (player != Owner)
            return false;

        bool didAnything = false;

        for (int i = 0; i < rewards.Count; i++)
        {
            Reward reward = rewards[i];
            PotionReward? potionReward = reward as PotionReward;
            RelicReward? relicReward = reward as RelicReward;
            if (potionReward?.Potion is not null)
            {
                didAnything = true;
                PotionRarity potionRarity = potionReward.Potion.Rarity;
                IEnumerable<PotionModel> items = Owner.Character.PotionPool.GetUnlockedPotions(Owner.UnlockState).Concat(ModelDb.PotionPool<SharedPotionPool>().GetUnlockedPotions(Owner.UnlockState)).Where((Func<PotionModel, bool>) (p => p.Rarity == potionRarity && p.Id != potionReward.Potion.Id));
                PotionModel alternatePotion = Owner.PlayerRng.Rewards.NextItem(items)!.ToMutable();
                
                Reward newReward = new PotionReward(alternatePotion, Owner);
                
                rewards[i] = new CustomLinkedRewardSet([rewards[i], newReward], Owner);
            }
            if (relicReward?.Relic is not null)
            {
                didAnything = true;
                RelicRarity relicRarity = relicReward.Relic.Rarity;
                IEnumerable<RelicModel> items = Owner.Character.RelicPool.GetUnlockedRelics(Owner.UnlockState).Concat(ModelDb.RelicPool<SharedRelicPool>().GetUnlockedRelics(Owner.UnlockState)).Where((Func<RelicModel, bool>) (r => r.Rarity == relicRarity && r.Id != relicReward.Relic.Id));
                RelicModel alternateRelic = Owner.PlayerRng.Rewards.NextItem(items)!.ToMutable();
                
                Reward newReward = new RelicReward(alternateRelic, Owner);
                
                rewards[i] = new CustomLinkedRewardSet([rewards[i], newReward], Owner);
            }
        }

        if (didAnything) Flash();
        
        return true;
    }
}
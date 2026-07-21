using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Powers;

public class ScorchedEarthPower : SpireEnigmaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        HoverTipFactory.FromCard<Burn>()
    ];
    
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner.Creature == Owner && card is Burn)
        {
            foreach (Creature hittableEnemy in CombatState.HittableEnemies)
            {
                NFireBurstVfx child = NFireBurstVfx.Create(hittableEnemy, 0.75f);
                NCombatRoom instance = NCombatRoom.Instance;
                if (instance != null)
                    instance.CombatVfxContainer.AddChildSafely((Node) child);
            }
            await CreatureCmd.Damage(choiceContext, CombatState.HittableEnemies, Amount, ValueProp.Unpowered, Owner, null, null);
        }
    }
}
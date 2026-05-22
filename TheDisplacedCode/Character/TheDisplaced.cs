using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using TheDisplaced.TheDisplacedCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using TheDisplaced.TheDisplacedCode.Cards;
using TheDisplaced.TheDisplacedCode.Cards.basic;
using TheDisplaced.TheDisplacedCode.Relics;
using Expose = TheDisplaced.TheDisplacedCode.Cards.Expose;

namespace TheDisplaced.TheDisplacedCode.Character;

public class TheDisplaced : PlaceholderCharacterModel
{
    public const string CharacterId = "TheDisplaced";
    
    public static readonly Color Color = new("ffffff");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 74;
    
    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<StrikeDisplaced>(),
        ModelDb.Card<StrikeDisplaced>(),
        ModelDb.Card<StrikeDisplaced>(),
        ModelDb.Card<StrikeDisplaced>(),
        ModelDb.Card<DefendDisplaced>(),
        ModelDb.Card<DefendDisplaced>(),
        ModelDb.Card<DefendDisplaced>(),
        ModelDb.Card<DefendDisplaced>(),
        ModelDb.Card<Gaze>(),
        ModelDb.Card<Persist>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<AstronomicalClock>()
    ];
    
    public override CardPoolModel CardPool => ModelDb.CardPool<TheDisplacedCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<TheDisplacedRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<TheDisplacedPotionPool>();
    
    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets. 
        These are just some of the simplest assets, given some placeholders to differentiate your character with. 
        You don't have to, but you're suggested to rename these images. */
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }
    
    /*
    public override NCreatureVisuals? CreateCustomVisuals()
    {
        return NodeFactory<NCreatureVisuals>.CreateFromResource("res://Path/To/Your/Image.png");
    }*/
    
    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}
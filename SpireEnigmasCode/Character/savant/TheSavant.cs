using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using SpireEnigmas.SpireEnigmasCode.Cards.sacrifice.basic;
using SpireEnigmas.SpireEnigmasCode.Cards.savant.basic;
using SpireEnigmas.SpireEnigmasCode.Character.sacrifice;
using SpireEnigmas.SpireEnigmasCode.Extensions;
using SpireEnigmas.SpireEnigmasCode.Relics;
using SpireEnigmas.SpireEnigmasCode.Relics.sacrifice;
using SpireEnigmas.SpireEnigmasCode.Relics.savant;

namespace SpireEnigmas.SpireEnigmasCode.Character.savant;

public class TheSavant : PlaceholderCharacterModel
{
    public override string PlaceholderID => "ironclad";
    
    public const string CharacterId = "TheSavant";
    
    public static readonly Color Color = new("#B22222");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 65;
    
    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<StrikeSavant>(),
        ModelDb.Card<StrikeSavant>(),
        ModelDb.Card<StrikeSavant>(),
        ModelDb.Card<StrikeSavant>(),
        ModelDb.Card<DefendSavant>(),
        ModelDb.Card<DefendSavant>(),
        ModelDb.Card<DefendSavant>(),
        ModelDb.Card<DefendSavant>(),
        ModelDb.Card<SuppressiveFire>(),
        ModelDb.Card<KineticCharge>()
    ];

    
    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<FunctionalCore>()
    ];
    
    
    public override CardPoolModel CardPool => ModelDb.CardPool<TheSavantCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<TheSavantRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<TheSavantPotionPool>();
    
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
        return NodeFactory<NCreatureVisuals>.CreateFromResource("res://SpireEnigmas/images/character/displaced.png");
    }
    */
    
    public override string CustomCharacterSelectTransitionPath =>
        "res://SpireEnigmas/materials/displaced_transition_mat.tres";
    
    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
    
    //public override string CustomVisualPath => "res://TheDisplaced/scenes/thedisplaced/displaced.tscn";
}
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.token;

[Pool(typeof(TokenCardPool))]
public class MobilityGadget() : AbstractGadget
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(0M, ValueProp.Move),
        new BoolVar("HasBlock", false),
        
        new DamageVar(0M, ValueProp.Move),
        new BoolVar("HasDamage", false),
        
        new EnergyVar(0),
        new BoolVar("HasEnergy", false),
        
        new CardsVar(2),
        new BoolVar("HasCardDraw", true)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];
}
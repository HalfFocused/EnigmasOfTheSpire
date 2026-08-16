using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.token;

[Pool(typeof(TokenCardPool))]
public class Gadget() : SpireEnigmasCard.SavantCard(1,
    CardType.Skill, CardRarity.Token,
    TargetType.Self)
{

    public override CardType Type => ((BoolVar)DynamicVars["HasDamage"]).BoolVal ? CardType.Attack : CardType.Skill;
    public override TargetType TargetType => ((BoolVar)DynamicVars["HasDamage"]).BoolVal ? TargetType.AnyEnemy : TargetType.Self;

    public const string InventionBlockKey = "InventionBlock";
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(0M, ValueProp.Move),
        new BoolVar("HasBlock", false),
        
        //newline 1
        
        new DamageVar(0M, ValueProp.Move),
        new BoolVar("HasDamage", false),
        
        //newline 2
        
        new EnergyVar(0),
        new BoolVar("HasEnergy", false),
        
        //newline 3
        
        new CardsVar(0),
        new BoolVar("HasCardDraw", false)
    ];

    public override bool GainsBlock => ((BoolVar)DynamicVars["HasBlock"]).BoolVal;
    
    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("HasEnergy", ((BoolVar) DynamicVars["HasEnergy"]).BoolVal);
        description.Add("HasCardDraw", ((BoolVar) DynamicVars["HasCardDraw"]).BoolVal);
        description.Add("HasDamage", ((BoolVar) DynamicVars["HasDamage"]).BoolVal);
        
        /*
         * there's gotta be a better way than this but i'm simply too lazy
         */
        
        description.Add("NewLine1", ((BoolVar) DynamicVars["HasDamage"]).BoolVal || 
                                    ((BoolVar) DynamicVars["HasEnergy"]).BoolVal || 
                                    ((BoolVar) DynamicVars["HasCardDraw"]).BoolVal);
        
        description.Add("NewLine2", ((BoolVar) DynamicVars["HasEnergy"]).BoolVal || 
                                    ((BoolVar) DynamicVars["HasCardDraw"]).BoolVal);
        
        description.Add("NewLine3", ((BoolVar) DynamicVars["HasCardDraw"]).BoolVal);
    }
    public override IEnumerable<CardKeyword> CanonicalKeywords => 
    [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];
    
    //public override LocString Description => new LocString("cards", Id.Entry + ".description");

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (((BoolVar) DynamicVars["HasBlock"]).BoolVal)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
        }
        
        if (((BoolVar) DynamicVars["HasDamage"]).BoolVal)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        }
        
        if (((BoolVar) DynamicVars["HasEnergy"]).BoolVal)
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        }
        
        if (((BoolVar) DynamicVars["HasCardDraw"]).BoolVal)
        {
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }

    public void TakeAttributesFrom(IEnumerable<DynamicVar> dynamicVars, IEnumerable<CardKeyword>? keywords = null, int replay = 0)
    {
        foreach (DynamicVar dynamicVar in dynamicVars)
        {
            /*
             * why not make invention block a block var, i hear you ask
             * I KNOW
             * MEGACRIT decided even unpowered block vars should be modified by enchantments like nimble :(
             * so my very clean (citation needed) system is ugly now. review bomb this game IMMEDIATELY
             */
            if (dynamicVar.Name.Equals(InventionBlockKey))
            {
                DynamicVars.Block.BaseValue += dynamicVar.BaseValue;
                ((BoolVar) DynamicVars["HasBlock"]).BoolVal = true;
            }
            else
            {
                switch (dynamicVar)
                {
                    /*
                    case BlockVar:
                        DynamicVars.Block.BaseValue += dynamicVar.BaseValue;
                        ((BoolVar) DynamicVars["HasBlock"]).BoolVal = true;
                        break;
                    */
                    case DamageVar:
                        DynamicVars.Damage.BaseValue += dynamicVar.BaseValue;
                        ((BoolVar) DynamicVars["HasDamage"]).BoolVal = true;
                        break;
                    case EnergyVar:
                        DynamicVars.Energy.BaseValue += dynamicVar.BaseValue;
                        ((BoolVar) DynamicVars["HasEnergy"]).BoolVal = true;
                        break;
                    case CardsVar:
                        DynamicVars.Cards.BaseValue += dynamicVar.BaseValue;
                        ((BoolVar) DynamicVars["HasCardDraw"]).BoolVal = true;
                        break;
                }
            }
        }
        if (keywords is not null)
        {
            foreach (CardKeyword keyword in keywords)
            {
                if (!Keywords.Contains(keyword))
                {
                    CardCmd.ApplyKeyword(this, keyword);
                }
            }
        }
        BaseReplayCount += replay;
    }
}
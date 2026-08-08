using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace SpireEnigmas.SpireEnigmasCode.Cards.savant.token;

[Pool(typeof(TokenCardPool))]
public class AbstractGadget() : SpireEnigmasCard.SavantCard(1,
    CardType.Skill, CardRarity.Token,
    TargetType.Self)
{

    public override CardType Type => ((BoolVar)DynamicVars["HasDamage"]).BoolVal ? CardType.Attack : CardType.Skill;
    public override TargetType TargetType => ((BoolVar)DynamicVars["HasDamage"]).BoolVal ? TargetType.AnyEnemy : TargetType.Self;
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
        CardKeyword.Retain
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

    public void TakeAttributesFrom(AbstractGadget gadget)
    {
        
        if (((BoolVar) gadget.DynamicVars["HasBlock"]).BoolVal)
        {
            DynamicVars.Block.BaseValue += gadget.DynamicVars.Block.BaseValue;
            if (!((BoolVar)DynamicVars["HasBlock"]).BoolVal) MakeComposite();
            ((BoolVar) DynamicVars["HasBlock"]).BoolVal = true;
        }
        
        if (((BoolVar) gadget.DynamicVars["HasDamage"]).BoolVal)
        {
            DynamicVars.Damage.BaseValue += gadget.DynamicVars.Damage.BaseValue;
            if (!((BoolVar)DynamicVars["HasDamage"]).BoolVal) MakeComposite();
            ((BoolVar) DynamicVars["HasDamage"]).BoolVal = true;
        }
        
        if (((BoolVar) gadget.DynamicVars["HasEnergy"]).BoolVal)
        {
            DynamicVars.Energy.BaseValue += gadget.DynamicVars.Energy.BaseValue;
            if (!((BoolVar)DynamicVars["HasEnergy"]).BoolVal) MakeComposite();
            ((BoolVar) DynamicVars["HasEnergy"]).BoolVal = true;
        }
        
        if (((BoolVar) gadget.DynamicVars["HasCardDraw"]).BoolVal)
        {
            DynamicVars.Cards.BaseValue += gadget.DynamicVars.Cards.BaseValue;
            if (!((BoolVar)DynamicVars["HasCardDraw"]).BoolVal) MakeComposite();
            ((BoolVar) DynamicVars["HasCardDraw"]).BoolVal = true;
        }

        foreach (CardKeyword keyword in gadget.Keywords)
        {
            if (!Keywords.Contains(keyword))
            {
                CardCmd.ApplyKeyword(this, keyword);
                MakeComposite();
            }
        }

        if (gadget._baseReplayCount != 0)
        {
            _baseReplayCount += gadget.BaseReplayCount;
            MakeComposite();
        }
    }

    private void MakeComposite()
    {
        _titleLocString = new LocString("cards", ModelDb.GetId(typeof(AbstractGadget)).Entry + ".title");
    }

    public void IncreaseDamage(int damageIncrease)
    {
        DynamicVars.Damage.BaseValue += damageIncrease;
        if (!((BoolVar)DynamicVars["HasDamage"]).BoolVal) MakeComposite();
        ((BoolVar) DynamicVars["HasDamage"]).BoolVal = true;
    }
}
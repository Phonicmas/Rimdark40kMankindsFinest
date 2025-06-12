using RimWorld;
using Verse;

namespace Genes40k;

public class Comp_LifespanDropItem : ThingComp
{
    public int age = -1;

    public CompProperties_LifespanDropItem Props => (CompProperties_LifespanDropItem)props;

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref age, "age", 0);
    }

    public override void CompTick()
    {
        age++;
        if (age >= Props.lifespanTicks)
        {
            Expire();
        }
    }

    public override string CompInspectStringExtra()
    {
        var text = base.CompInspectStringExtra();
        var result = "";
        var num = Props.lifespanTicks - age;
        if (num > 0)
        {
            result = "LifespanExpiry".Translate() + " " + num.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
            if (!text.NullOrEmpty())
            {
                result = "\n" + text;
            }
        }
        return result;
    }

    private void Expire()
    {
        if (!parent.Spawned)
        {
            return;
        }
        
        Props.expireEffect?.Spawn(parent.Position, parent.Map).Cleanup();

        if (Props.droppedThingDef != null)
        {
            var thing = GenSpawn.Spawn(Props.droppedThingDef, parent.Position, parent.Map);
            thing.stackCount = Props.amountDropped;
        }
        
        parent.Destroy(DestroyMode.KillFinalize);
    }
}

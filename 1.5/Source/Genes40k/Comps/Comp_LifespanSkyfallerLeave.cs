using RimWorld;
using Verse;

namespace Genes40k;

public class Comp_LifespanSkyfallerLeave : ThingComp
{
    public int age = -1;

    public CompProperties_LifespanSkyfallerLeave Props => (CompProperties_LifespanSkyfallerLeave)props;

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
        
        var leaver = SkyfallerMaker.MakeSkyfaller(Props.skyfallerLeaving ?? ThingDefOf.DropPodLeaving);

        leaver.DrawColor = parent.DrawColor;
        
        GenSpawn.Spawn(leaver, parent.Position, parent.Map);
        parent.Destroy(DestroyMode.KillFinalize);
    }
}

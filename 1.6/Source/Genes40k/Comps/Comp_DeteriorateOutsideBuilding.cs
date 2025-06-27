using System.Text;
using RimWorld;
using Verse;

namespace Genes40k;

//Thanks to VE for letting me repurpose code from VE ancients
public class Comp_DeteriorateOutsideBuilding : ThingComp
{
    public CompProperties_DeteriorateOutsideBuilding Props => props as CompProperties_DeteriorateOutsideBuilding;

    public bool ShouldDeteriorate
    {
        get
        {
            var thing = parent.StoringThing();
            if (thing == null || !Props.antiDeteriorateContainers.Contains(thing.def))
            {
                return true;
            }
                
            var comp = thing.TryGetComp<CompPowerTrader>();
            if (comp != null)
            {
                return !comp.PowerOn;
            }
            return false;
        }
    }
        
    public override string CompInspectStringExtra()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("BEWH.MankindsFinest.Containers.DeterioratingOutsideContainer".Translate(parent.Label));
        return stringBuilder.ToString();
    }
        
    public override string GetDescriptionPart()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("BEWH.MankindsFinest.Containers.SuitableContainers".Translate());
        foreach (var container in Props.antiDeteriorateContainers)
        {
            stringBuilder.Append("\n");
            stringBuilder.Append(" - " + container.label.CapitalizeFirst());
        }
        return stringBuilder.ToString();
    }

        
}
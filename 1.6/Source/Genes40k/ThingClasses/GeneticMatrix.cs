using UnityEngine;
using Verse;

namespace Genes40k;

[StaticConstructorOnStartup]
public class GeneticMatrix : ThingWithComps
{
    private bool invisible = false;
        
    public override void Print(SectionLayer layer)
    {
        if (invisible)
        {
            return;
        }

        base.Print(layer);
    }

    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        if (invisible)
        {
            return;
        }

        base.DrawAt(drawLoc, flip);
    }

    public void ChangeVisibility(bool newValue)
    {
        invisible = newValue;
    }


    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref invisible, "invisible");
    }
}
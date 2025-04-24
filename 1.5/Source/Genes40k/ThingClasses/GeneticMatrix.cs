using UnityEngine;
using Verse;

namespace Genes40k;

[StaticConstructorOnStartup]
public class GeneticMatrix : ThingWithComps
{
    private bool invisible = false;
        
    public override Graphic Graphic
    {
        get
        {
            var graphic = DefaultGraphic.GetCopy(def.graphicData.drawSize, null);
                
            graphic.drawSize = !invisible ? def.graphicData.drawSize : Vector2.zero;
                
            return graphic;
        }
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
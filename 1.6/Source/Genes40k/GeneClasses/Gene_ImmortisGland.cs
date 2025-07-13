using System.Linq;
using Core40k;
using UnityEngine;
using Verse;

namespace Genes40k;

public class Gene_ImmortisGland : Gene
{
    public bool MaterialExtracted => materialExtracted;
    private bool materialExtracted = false;

    public void ExtractedMaterial()
    {
        materialExtracted = true;
    }
    
    public override void ExposeData()
    {
        Scribe_Values.Look(ref materialExtracted, "materialExtracted");
        base.ExposeData();
    }
}
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Genes40k;

[StaticConstructorOnStartup]
public class GeneMaterialExtra : ThingWithComps
{
    private GeneSet geneSet;
        
    public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
    {
        foreach (var item in base.SpecialDisplayStats())
        {
            yield return item;
        }

        if (geneSet == null)
        {
            var defMod = def.GetModExtension<DefModExtension_GeneFromMaterial>();
            geneSet = new GeneSet();
            geneSet.AddGene(defMod.addedGene);
            if (defMod.extraAddedGeneForDescription != null)
            {
                geneSet.AddGene(defMod.extraAddedGeneForDescription);
            }
        }

        Dialog_InfoCard.Hyperlink? inspectGenesHyperlink = null;
        if (ThingSelectionUtility.SelectableByMapClick(this))
        {
            inspectGenesHyperlink = new Dialog_InfoCard.Hyperlink(this, -1, thingIsGeneOwner: true);
        }
        foreach (var item3 in geneSet.SpecialDisplayStats(inspectGenesHyperlink))
        {
            yield return item3;
        }
    }
}
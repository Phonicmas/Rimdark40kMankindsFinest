using System.Collections.Generic;
using Verse;

namespace Genes40k;

/// <summary>
/// Counts how many spawned pawns on this map carry each primarch gene, so
/// ThoughtWorker_PrimarchPresence does not have to walk every spawned pawn for every pawn it is
/// asked about. Counts rather than flags so a pawn can still be excluded from their own thought.
/// </summary>
public class MapComponent_PrimarchPresence : MapComponent
{
    private const int RefreshInterval = 250;

    private readonly Dictionary<GeneDef, int> primarchGeneCounts = new();

    public MapComponent_PrimarchPresence(Map map) : base(map)
    {
    }

    public int CountOf(GeneDef primarchGene)
    {
        if (primarchGene == null)
        {
            return 0;
        }

        return primarchGeneCounts.TryGetValue(primarchGene, out var count) ? count : 0;
    }

    public override void FinalizeInit()
    {
        base.FinalizeInit();
        Refresh();
    }

    public override void MapComponentTick()
    {
        base.MapComponentTick();

        if ((Find.TickManager.TicksGame + map.uniqueID) % RefreshInterval != 0)
        {
            return;
        }

        Refresh();
    }

    private void Refresh()
    {
        primarchGeneCounts.Clear();

        var relatedPrimarchGenes = Genes40kUtils.RelatedPrimarchGenes;

        if (relatedPrimarchGenes.Count == 0)
        {
            return;
        }

        foreach (var pawn in map.mapPawns.AllPawnsSpawned)
        {
            if (pawn.Dead || pawn.genes == null || !pawn.RaceProps.Humanlike)
            {
                continue;
            }

            foreach (var gene in pawn.genes.GenesListForReading)
            {
                if (!gene.Active || !relatedPrimarchGenes.Contains(gene.def))
                {
                    continue;
                }

                primarchGeneCounts.TryGetValue(gene.def, out var count);
                primarchGeneCounts[gene.def] = count + 1;
            }
        }
    }
}
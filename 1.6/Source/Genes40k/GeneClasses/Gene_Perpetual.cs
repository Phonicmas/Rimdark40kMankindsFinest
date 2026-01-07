using System.Linq;
using RimWorld;
using Verse;

namespace Genes40k;

public class Gene_Perpetual : Gene
{
    private const int ClotCheckInterval = 600;
        
    private const int PermanentInjuryInterval = 60000;
        
    private const int MissingLimbRegenerationInterval = 120000;

    private static readonly FloatRange TendingQualityRange = new FloatRange(0.2f, 0.7f);
        
    private static readonly FloatRange HealPermanentRange = new FloatRange(0.2f, 0.4f);

    private DefModExtension_PerpetualGene defMod = new DefModExtension_PerpetualGene();

    private int perpetualTier => defMod.perpetualTier;

    public bool DontAddToPerpetualTracker = false;

    public override void Tick()
    {
        base.Tick();
            
        //Tend to bleeding or otherwise tendable wounds
        if (pawn.IsHashIntervalTick(ClotCheckInterval))
        {
            var hediffs = pawn.health.hediffSet.hediffs;
            for (var num = hediffs.Count - 1; num >= 0; num--)
            {
                if (hediffs[num].Bleeding || hediffs[num].TendableNow())
                {
                    hediffs[num].Tended(TendingQualityRange.RandomInRange * perpetualTier, TendingQualityRange.TrueMax * perpetualTier, 1);
                }
            }
        }
            
        //Heal other injuries
        if (pawn.IsHashIntervalTick(PermanentInjuryInterval))
        {
            foreach (var hediff in pawn.health.hediffSet.hediffs)
            {
                if (hediff is Hediff_Injury hediff_Injury && hediff_Injury.IsPermanent() && !hediff_Injury.CanHealNaturally())
                {
                    hediff_Injury.Heal(HealPermanentRange.RandomInRange * perpetualTier);
                }
            }
        }
            
        //Heal missing limbs
        if (!pawn.IsHashIntervalTick(MissingLimbRegenerationInterval))
        {
            return;
        }
            
        BodyPartRecord bodyPartRecord = null;
        foreach (var missingParts in pawn.health.hediffSet.GetMissingPartsCommonAncestors().Where(missingParts => !pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(missingParts.Part)))
        {
            bodyPartRecord = missingParts.Part;
        }

        if (bodyPartRecord == null)
        {
            return;
        }
                
        pawn.health.RestorePart(bodyPartRecord);
        var partHealth = pawn.health.hediffSet.GetPartHealth(bodyPartRecord)/2;
        var dinfo = new DamageInfo(DamageDefOf.Psychic, partHealth, 999f, -1f, null, bodyPartRecord);
        pawn.TakeDamage(dinfo);
    }
        
    public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
    {
        if (pawn.PawnHasAlteredCarbonStack() || DontAddToPerpetualTracker)
        {
            //Pawns with altered carbon stacks should not be added for respawning
            base.Notify_PawnDied(dinfo, culprit);
            DontAddToPerpetualTracker = false;
            return;
        }
        
        AddPawnToPerpetualTracker();
        base.Notify_PawnDied(dinfo, culprit);
    }

    public void AddPawnToPerpetualTracker()
    {
        Current.Game.GetComponent<GameComponent_Perpetual>().AddPerpetual(pawn, Find.TickManager.TicksGame + defMod.perpetualRessurectionRange.RandomInRange);
    }
        
    public override void PostAdd()
    {
        defMod = def.GetModExtension<DefModExtension_PerpetualGene>();
        base.PostAdd();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            defMod = def.GetModExtension<DefModExtension_PerpetualGene>();
        }
    }
}
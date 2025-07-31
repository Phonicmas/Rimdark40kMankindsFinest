using RimWorld;
using System;
using System.Collections.Generic;
using Core40k;
using Verse;
using Verse.AI;

namespace Genes40k;

public class Building_GeneTable : Building_WorkTable
{
    private int tickAmount = 0;

    private const int tickAmountDrain = 1000;

    private Pawn workingPawn = null;
    
    private const float psyfocusDrain = -0.05f;
    private const float psyfocusDrainMinimum = -0.005f;

    private const float severityAdd = 0.1f;
    private const float severityAddMinimum = 0.01f;
    
    private static Genes40kModSettings modSettings = null;

    public static Genes40kModSettings ModSettings => modSettings ??= LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>();
        
    private static readonly CachedTexture CraftPrimarchEmbryo = new CachedTexture("UI/Gizmos/BEWH_GestationStartIcon");

    public Building_GeneTable()
    {
        billStack = new BillStack(this);
    }

    public void PawnDidWork(Pawn p)
    {
        if (workingPawn != p && p != null)
        {
            workingPawn = p;
        }
    }

    public override void UsedThisTick()
    {
        base.UsedThisTick();
        if (workingPawn == null)
        {
            return;
        }
            
        tickAmount++;
        if (tickAmount < tickAmountDrain)
        {
            return;
        }

        if (!ModSettings.psychicCrafting)
        {
            return;
        }

        var psysens = workingPawn.GetStatValue(StatDefOf.PsychicSensitivity);
            
        if (ModsConfig.RoyaltyActive)
        {
            if (workingPawn.psychicEntropy.CurrentPsyfocus >= Math.Abs(psyfocusDrain))
            {
                workingPawn.psychicEntropy.OffsetPsyfocusDirectly(Math.Max(psyfocusDrain / psysens, psyfocusDrainMinimum));
            }
            else
            {
                workingPawn.psychicEntropy.OffsetPsyfocusDirectly(workingPawn.psychicEntropy.CurrentPsyfocus * -1);
                DoComaHediff(psysens);
            }
        }
        else
        {
            DoComaHediff(psysens);
        }
        tickAmount = 0;
    }

    private void DoComaHediff(float psysens)
    {
        if (workingPawn == null)
        {
            return;
        }
        var hediff = workingPawn.health.hediffSet.GetFirstHediffOfDef(Genes40kDefOf.BEWH_PsychicCrafting);
        if (hediff == null)
        {
            workingPawn.health.AddHediff(Genes40kDefOf.BEWH_PsychicCrafting);
        }
        else
        {
            if (hediff.Severity + severityAdd >= 3f)
            {
                workingPawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                workingPawn.jobs.ClearQueuedJobs(false);
            }
            hediff.Severity += Math.Max(severityAdd / psysens, severityAddMinimum);
        }
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }
            
        var command_Action = new Command_Action
        {
            defaultLabel = "BEWH.MankindsFinest.GeneManupulationTable.CraftPrimarchEmbryo".Translate() + "...",
            defaultDesc = "BEWH.MankindsFinest.GeneManupulationTable.CraftPrimarchEmbryoDesc".Translate(),
            icon = CraftPrimarchEmbryo.Texture,
            action = delegate
            {
                Find.WindowStack.Add(new Dialog_CraftPrimarchEmbryo(Map, this));
            }
        };
        yield return command_Action;
    }

    public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
    {
        foreach (var floatMenu in base.GetFloatMenuOptions(selPawn))
        {
            yield return floatMenu;
        }
            
        if (!selPawn.apparel.WornApparel.Any(a => a is ApparelMultiColor))
        {
            yield break;
        }
            
        var secondColourChangeFloatMenu = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("BEWH.Framework.ApparelMultiColor.ArmourDecorationFeature".Translate().CapitalizeFirst(), delegate
        {
            selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(Core40kDefOf.BEWH_OpenStylingStationDialogForSecondColour, this), JobTag.Misc);
        }), selPawn, this);
            
        yield return secondColourChangeFloatMenu;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref tickAmount, "tickAmount", 0);
        Scribe_References.Look(ref workingPawn, "workingPawn");
    }
}
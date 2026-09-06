using RimWorld;
using Verse;

namespace Genes40k;

public class IncidentWorker_LegionMaterialHeist  : IncidentWorker
{
	private GameComponent_UnlockedMaterials GameComp => GameComponent_UnlockedMaterials.Instance;
	
	protected override bool CanFireNowSub(IncidentParms parms)
	{
		if (!base.CanFireNowSub(parms))
		{
			return false;
		}
		if (parms.target is not Map map || GameComp == null)
		{
			return false;
		}

		if (ModsConfig.BiotechActive && map.GameConditionManager.ConditionIsActive(GameConditionDefOf.NoxiousHaze))
		{
			return false;
		}

		if (!GameComp.AnyLockedLegionMaterialRemaining())
		{
			return false;
		}
		
		return TryFindEntryCell(map, out _);
	}

	protected override bool TryExecuteWorker(IncidentParms parms)
	{
		if (parms.target is not Map map || GameComp == null)
		{
			return false;
		}

		if (!TryFindEntryCell(map, out var cell))
		{
			return false;
		}	
		var pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(Genes40kDefOf.BEWH_FirstbornPawnLegionMaterialHeist));

		var legionMaterial = ThingMaker.MakeThing(GameComp.GetRandomLockedLegionMaterial());
		pawn.inventory.innerContainer.TryAdd(legionMaterial);
		
		DropPodUtility.DropThingsNear(cell, map, [pawn], canRoofPunch: false);

		var job = JobMaker.MakeJob(Genes40kDefOf.BEWH_WaitLegionHeist);
		pawn.jobs.jobQueue.EnqueueFirst(job);
		
		var title = def.letterLabel.Formatted().CapitalizeFirst();
		var text2 = def.letterText.Formatted(pawn.Named("PAWN")).AdjustedFor(pawn).CapitalizeFirst();
		SendStandardLetter(title, text2, def.letterDef, parms, pawn);
		return true;
	}

	private bool TryFindEntryCell(Map map, out IntVec3 cell)
	{
		var cellFound = CellFinder.TryFindRandomCell(map, c => map.reachability.CanReachColony(c), out var cellToLand);
		cell = cellToLand;
		return cellFound;
	}
}
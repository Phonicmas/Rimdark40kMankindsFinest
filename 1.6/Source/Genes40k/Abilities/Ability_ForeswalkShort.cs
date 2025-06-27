using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Genes40k;

public class Ability_ForeswalkShort : VEF.Abilities.Ability
{
	public virtual FleckDef[] EffectSet => new FleckDef[3]
	{
		FleckDefOf.PsycastSkipFlashEntry,
		FleckDefOf.PsycastSkipInnerExit,
		FleckDefOf.PsycastSkipOuterRingExit
	};

	public override void WarmupToil(Toil toil)
	{
		base.WarmupToil(toil);
		toil.AddPreTickAction(delegate
		{
			if (pawn.jobs.curDriver.ticksLeftThisToil != 5)
			{
				return;
			}
				
			var effectSet = EffectSet;
			for (var i = 0; i < Comp.currentlyCastingTargets.Length; i += 2)
			{
				var thing = Comp.currentlyCastingTargets[i].Thing;
				switch (thing)
				{
					case null:
						continue;
					case Pawn pawn:
					{
						var dataAttachedOverlay = FleckMaker.GetDataAttachedOverlay(pawn, effectSet[0], Vector3.zero);
						dataAttachedOverlay.link.detachAfterTicks = 5;
						pawn.Map.flecks.CreateFleck(dataAttachedOverlay);
						break;
					}
					default:
						FleckMaker.Static(thing.TrueCenter(), thing.Map, FleckDefOf.PsycastSkipFlashEntry);
						break;
				}

				var globalTargetInfo = base.Comp.currentlyCastingTargets[i + 1];
				FleckMaker.Static(globalTargetInfo.Cell, globalTargetInfo.Map, effectSet[1]);
				FleckMaker.Static(globalTargetInfo.Cell, globalTargetInfo.Map, effectSet[2]);
				SoundDefOf.Psycast_Skip_Entry.PlayOneShot(thing);
				SoundDefOf.Psycast_Skip_Exit.PlayOneShot(new TargetInfo(globalTargetInfo.Cell, globalTargetInfo.Map));
				AddEffecterToMaintain(EffecterDefOf.Skip_Entry.Spawn(thing, thing.Map), thing.Position, 60);
				AddEffecterToMaintain(EffecterDefOf.Skip_Exit.Spawn(globalTargetInfo.Cell, globalTargetInfo.Map), globalTargetInfo.Cell, 60);
			}
		});
	}

	private IEnumerable<Pawn> PawnsToSkip()
	{
		var homeMap = pawn.Map.IsPlayerHome;
		foreach (var item in GenRadial.RadialDistinctThingsAround(pawn.Position, pawn.Map, GetRadiusForPawn(), useCenter: true))
		{
			if (!(item is Pawn pawn) || pawn.Dead)
			{
				continue;
			}
			if (!pawn.IsColonist && !pawn.IsPrisonerOfColony)
			{
				if (homeMap || !pawn.RaceProps.Animal)
				{
					continue;
				}
				var faction = pawn.Faction;
				if (faction == null || !faction.IsPlayer)
				{
					continue;
				}
			}
			yield return pawn;
		}
	}
		
	public override void Cast(params GlobalTargetInfo[] targets)
	{
		var list = PawnsToSkip().ToList();

		foreach (var pawn2 in list)
		{
			if (pawn2.Spawned)
			{
				pawn2.teleporting = true;
				pawn2.ExitMap(allowedToJoinOrCreateCaravan: false, Rot4.Invalid);
				pawn2.teleporting = false;
			}

			var targetCell = targets[1].Cell;
			var targetMap = targets[1].Map;
			CellFinder.TryFindRandomSpawnCellForPawnNear(targetCell, targetMap, out var result);
			GenSpawn.Spawn(pawn2, result, targetMap);
			if (pawn2.drafter != null && pawn2.IsColonistPlayerControlled)
			{
				pawn2.drafter.Drafted = true;
			}
			pawn2.Notify_Teleported();
			if (pawn2.IsPrisoner)
			{
				pawn2.guest.WaitInsteadOfEscapingForDefaultTicks();
			}
			FleckMaker.ThrowSmoke(pawn2.DrawPos, pawn2.Map, 1f);
			FleckMaker.ThrowDustPuffThick(pawn2.DrawPos, pawn2.Map, 2f, Color.green);
			if ((pawn2.IsColonist || pawn2.RaceProps.packAnimal) && pawn2.Map.IsPlayerHome)
			{
				pawn2.inventory.UnloadEverything = true;
			}
		}
			
		base.Cast(targets);
	}
		
	public override void GizmoUpdateOnMouseover()
	{
		var radiusForPawn = GetRadiusForPawn();
		GenDraw.DrawRadiusRing(pawn.Position, radiusForPawn, Color.green);
	}

	public override void DrawHighlight(LocalTargetInfo target)
	{
		GenDraw.DrawTargetHighlight(target);
	}
}
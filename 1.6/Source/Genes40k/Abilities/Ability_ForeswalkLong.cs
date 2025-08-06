using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Genes40k;

public class Ability_ForeswalkLong : VEF.Abilities.Ability
{
	public override void DoAction()
	{
		var invalidPawn = PawnsToSkip().FirstOrDefault(p => p.IsQuestLodger());
		if (invalidPawn != null)
		{
			Dialog_MessageBox.CreateConfirmation("FarskipConfirmTeleportingLodger".Translate(invalidPawn.Named("PAWN")), base.DoAction);
		}
		else
		{
			base.DoAction();
		}
	}

	private IEnumerable<Pawn> PawnsToSkip()
	{
		var caravan = pawn.GetCaravan();
		if (caravan != null)
		{
			foreach (var pawn2 in caravan.pawns)
			{
				yield return pawn2;
			}
			yield break;
		}
		var homeMap = pawn.Map.IsPlayerHome;
		foreach (var item in GenRadial.RadialDistinctThingsAround(pawn.Position, pawn.Map, GetRadiusForPawn(), useCenter: true))
		{
			if (item is not Pawn mapPawn || mapPawn.Dead)
			{
				continue;
			}
			if (!mapPawn.IsColonist && !mapPawn.IsPrisonerOfColony)
			{
				if (homeMap || !mapPawn.RaceProps.Animal)
				{
					continue;
				}
				var faction = mapPawn.Faction;
				if (faction == null || !faction.IsPlayer)
				{
					continue;
				}
			}
			yield return mapPawn;
		}
	}

	private Pawn AlliedPawnOnMap(Map targetMap)
	{
		return targetMap.mapPawns.AllPawnsSpawned.FirstOrDefault(p => !p.NonHumanlikeOrWildMan() && p.IsColonist && p.HomeFaction == Faction.OfPlayer && !PawnsToSkip().Contains(p));
	}

	public override bool CanHitTargetTile(GlobalTargetInfo target)
	{
		var num = Find.WorldGrid.TraversalDistanceBetween((CasterPawn.GetCaravan() != null) ? CasterPawn.GetCaravan().Tile : Caster.Map.Tile, target.Tile);
		if (num < GetRangeForPawn() + 1f)
		{
			return num > -1;
		}
		return false;
	}

	public override bool IsEnabledForPawn(out string reason)
	{
		if (!base.IsEnabledForPawn(out reason))
		{
			return false;
		}
		var caravan = pawn.GetCaravan();
		if (caravan == null || !caravan.ImmobilizedByMass)
		{
			return true;
		}
		reason = "CaravanImmobilizedByMass".Translate();
		return false;
	}

	public override void WarmupToil(Toil toil)
	{
		base.WarmupToil(toil);
		toil.AddPreTickAction(delegate
		{
			if (pawn.jobs.curDriver.ticksLeftThisToil != 5)
			{
				return;
			}
			foreach (var pawnToSkip in PawnsToSkip())
			{
				if (pawnToSkip == CasterPawn)
				{
					MakeStaticFleck(CasterPawn.DrawPos, CasterPawn.Map, FleckDefOf.PsycastSkipFlashEntry, def.castFleckScaleWithRadius ? GetRadiusForPawn() : def.castFleckScale, def.castFleckSpeed);
				}
				_ = Caster.Map;
				FleckMaker.ThrowSmoke(pawnToSkip.DrawPos, pawnToSkip.Map, 1f);
				FleckMaker.ThrowDustPuffThick(pawnToSkip.DrawPos, pawnToSkip.Map, 2f, new Color(1f, 1f, 1f, 2.5f));
			}
		});
	}

	public override void Cast(params GlobalTargetInfo[] targets)
	{
		var caravan = pawn.GetCaravan();
		var targetMap = (targets[0].WorldObject is MapParent mapParent) ? mapParent.Map : null;
		var targetCell = IntVec3.Invalid;
		var pawnsToSkip = PawnsToSkip().ToList();
		if (pawn.Spawned)
		{
			SoundDefOf.Psycast_Skip_Pulse.PlayOneShot(new TargetInfo(targets[0].Cell, pawn.Map));
		}
		if (targetMap != null)
		{
			var alliedPawnOnMap = AlliedPawnOnMap(targetMap);
			if (alliedPawnOnMap != null)
			{
				targetCell = alliedPawnOnMap.Position;
			}
		}
		if (targetCell.IsValid)
		{
			foreach (var pawnToSkip in pawnsToSkip)
			{
				if (pawnToSkip.Spawned)
				{
					pawnToSkip.teleporting = true;
					pawnToSkip.ExitMap(allowedToJoinOrCreateCaravan: false, Rot4.Invalid);
					pawnToSkip.teleporting = false;
				}
				CellFinder.TryFindRandomSpawnCellForPawnNear(targetCell, targetMap, out var result, 4, (IntVec3 cell) => cell != targetCell && cell.GetRoom(targetMap) == targetCell.GetRoom(targetMap));
				GenSpawn.Spawn(pawnToSkip, result, targetMap);
				if (pawnToSkip.drafter != null && pawnToSkip.IsColonistPlayerControlled)
				{
					pawnToSkip.drafter.Drafted = true;
				}
				pawnToSkip.Notify_Teleported();
				if (pawnToSkip.IsPrisoner)
				{
					pawnToSkip.guest.WaitInsteadOfEscapingForDefaultTicks();
				}
				FleckMaker.ThrowSmoke(pawnToSkip.DrawPos, pawnToSkip.Map, 1f);
				FleckMaker.ThrowDustPuffThick(pawnToSkip.DrawPos, pawnToSkip.Map, 2f, Color.green);
				if ((pawnToSkip.IsColonist || pawnToSkip.RaceProps.packAnimal) && pawnToSkip.Map.IsPlayerHome)
				{
					pawnToSkip.inventory.UnloadEverything = true;
				}
			}
			if (Find.WorldSelector.IsSelected(caravan))
			{
				Find.WorldSelector.Deselect(caravan);
				CameraJumper.TryJump(targetCell, targetMap);
			}
			caravan?.Destroy();
		}
		else if (targets[0].WorldObject is Caravan caravan2 && caravan2.Faction == base.pawn.Faction)
		{
			if (caravan != null)
			{
				caravan.pawns.TryTransferAllToContainer(caravan2.pawns);
				caravan2.Notify_Merged(new List<Caravan>(1) { caravan });
				caravan.Destroy();
			}
			else
			{
				foreach (Pawn item2 in pawnsToSkip)
				{
					caravan2.AddPawn(item2, addCarriedPawnToWorldPawnsIfAny: true);
					item2.ExitMap(allowedToJoinOrCreateCaravan: false, Rot4.Invalid);
				}
			}
		}
		else if (caravan != null)
		{
			caravan.Tile = targets[0].Tile;
			caravan.pather.StopDead();
		}
		else
		{
			CaravanMaker.MakeCaravan(pawnsToSkip, base.pawn.Faction, targets[0].Tile, addToWorldPawnsIfNotAlready: false);
			foreach (var item3 in pawnsToSkip)
			{
				item3.ExitMap(allowedToJoinOrCreateCaravan: false, Rot4.Invalid);
			}
		}
		base.Cast(targets);
	}

	public override void GizmoUpdateOnMouseover()
	{
		if (WorldRendererUtility.WorldSelected)
		{
			return;
		}
		var radiusForPawn = GetRadiusForPawn();
		GenDraw.DrawRadiusRing(pawn.Position, radiusForPawn, Color.green);
	}
}
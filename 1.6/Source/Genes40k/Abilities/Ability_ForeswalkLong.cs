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
			foreach (var item in PawnsToSkip())
			{
				if (item == CasterPawn)
				{
					MakeStaticFleck(CasterPawn.DrawPos, CasterPawn.Map, FleckDefOf.PsycastSkipFlashEntry, def.castFleckScaleWithRadius ? GetRadiusForPawn() : def.castFleckScale, def.castFleckSpeed);
				}
				_ = Caster.Map;
				FleckMaker.ThrowSmoke(item.DrawPos, item.Map, 1f);
				FleckMaker.ThrowDustPuffThick(item.DrawPos, item.Map, 2f, new Color(1f, 1f, 1f, 2.5f));
			}
		});
	}

	public override void Cast(params GlobalTargetInfo[] targets)
	{
		var caravan = pawn.GetCaravan();
		var targetMap = (targets[0].WorldObject is MapParent mapParent) ? mapParent.Map : null;
		var targetCell = IntVec3.Invalid;
		var list = PawnsToSkip().ToList();
		if (pawn.Spawned)
		{
			SoundDefOf.Psycast_Skip_Pulse.PlayOneShot(new TargetInfo(targets[0].Cell, pawn.Map));
		}
		if (targetMap != null)
		{
			var pawn = AlliedPawnOnMap(targetMap);
			if (pawn != null)
			{
				targetCell = pawn.Position;
			}
		}
		if (targetCell.IsValid)
		{
			foreach (var item in list)
			{
				if (item.Spawned)
				{
					item.teleporting = true;
					item.ExitMap(allowedToJoinOrCreateCaravan: false, Rot4.Invalid);
					item.teleporting = false;
				}
				CellFinder.TryFindRandomSpawnCellForPawnNear(targetCell, targetMap, out var result, 4, (IntVec3 cell) => cell != targetCell && cell.GetRoom(targetMap) == targetCell.GetRoom(targetMap));
				GenSpawn.Spawn(item, result, targetMap);
				if (item.drafter != null && item.IsColonistPlayerControlled)
				{
					item.drafter.Drafted = true;
				}
				item.Notify_Teleported();
				if (item.IsPrisoner)
				{
					item.guest.WaitInsteadOfEscapingForDefaultTicks();
				}
				FleckMaker.ThrowSmoke(item.DrawPos, item.Map, 1f);
				FleckMaker.ThrowDustPuffThick(item.DrawPos, item.Map, 2f, Color.green);
				if ((item.IsColonist || item.RaceProps.packAnimal) && item.Map.IsPlayerHome)
				{
					item.inventory.UnloadEverything = true;
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
				foreach (Pawn item2 in list)
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
			CaravanMaker.MakeCaravan(list, base.pawn.Faction, targets[0].Tile, addToWorldPawnsIfNotAlready: false);
			foreach (var item3 in list)
			{
				item3.ExitMap(allowedToJoinOrCreateCaravan: false, Rot4.Invalid);
			}
		}
		base.Cast(targets);
	}

	public override void GizmoUpdateOnMouseover()
	{
		var radiusForPawn = GetRadiusForPawn();
		GenDraw.DrawRadiusRing(pawn.Position, radiusForPawn, Color.green);
	}
}
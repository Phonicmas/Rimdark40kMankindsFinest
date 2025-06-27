using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Genes40k;

public class Ability_Inspire : VEF.Abilities.Ability
{
	private IEnumerable<Pawn> PawnsToAffect()
	{
		foreach (var item in GenRadial.RadialDistinctThingsAround(pawn.Position, pawn.Map, GetRadiusForPawn(), useCenter: true))
		{
			if (item is not Pawn targetPawn || targetPawn.Dead || targetPawn == pawn)
			{
				continue;
			}
			if (!targetPawn.IsColonist && !targetPawn.IsPrisonerOfColony)
			{
				if (targetPawn.RaceProps.Animal)
				{
					continue;
				}
				var faction = targetPawn.Faction;
				if (faction == null || !faction.IsPlayer)
				{
					continue;
				}
			}
			yield return targetPawn;
		}
	}

	public override void Cast(params GlobalTargetInfo[] targets)
	{
		var pawnList = PawnsToAffect().ToList();

		foreach (var targetPawn in pawnList)
		{
			var inspiredThought = ThoughtMaker.MakeThought(Genes40kDefOf.BEWH_ChaplainInspired, null);
			inspiredThought.durationTicksOverride = def.durationTime;
			targetPawn.needs.mood.thoughts.memories.TryGainMemory(inspiredThought);
		}
			
		base.Cast(targets);
	}

	public override void GizmoUpdateOnMouseover()
	{
		var radiusForPawn = GetRadiusForPawn();
		GenDraw.DrawRadiusRing(pawn.Position, radiusForPawn, Color.green);
	}
}
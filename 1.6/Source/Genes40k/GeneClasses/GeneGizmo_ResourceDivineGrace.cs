using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k;

[StaticConstructorOnStartup]
public class GeneGizmo_ResourceDivineGrace : GeneGizmo_Resource
{
	private static readonly Texture2D DivineGraceCostTex =
		SolidColorMaterials.NewSolidColorTexture(new Color(0.78f, 0.72f, 0.66f));

	private const float TotalPulsateTime = 0.85f;

	protected override bool IsDraggable => false;
	protected override bool DraggingBar { get; set; }

	private List<Pair<IGeneResourceDrain, float>> tmpDrainGenes = new ();

	public GeneGizmo_ResourceDivineGrace(Gene_Resource gene, List<IGeneResourceDrain> drainGenes, Color barColor,
		Color barHighlightColor) : base(gene, drainGenes, barColor, barHighlightColor)
	{
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		var result = base.GizmoOnGUI(topLeft, maxWidth, parms);
		var num = Mathf.Repeat(Time.time, TotalPulsateTime);
		var num2 = 1f;
		if (num < 0.1f)
		{
			num2 = num / 0.1f;
		}
		else if (num >= 0.25f)
		{
			num2 = 1f - (num - 0.25f) / 0.6f;
		}

		if (MapGizmoUtility.LastMouseOverGizmo is Command_Ability command_Ability && gene.Max != 0f)
		{
			foreach (var effectComp in command_Ability.Ability.EffectComps)
			{
				if (effectComp is not CompAbilityEffect_DivineGraceCost compAbilityEffect_DivineGraceCost)
				{
					continue;
				}

				var props = (CompProperties_AbilityDivineGraceCost)compAbilityEffect_DivineGraceCost.Props;

				if (props.divineGraceCost < float.Epsilon)
				{
					continue;
				}
					
				var rect = barRect.ContractedBy(3f);
				var width = rect.width;
				var num3 = gene.Value / gene.Max;
				rect.xMax = rect.xMin + width * num3;
				var num4 = Mathf.Min(props.divineGraceCost / gene.Max, 1f);
				rect.xMin = Mathf.Max(rect.xMin, rect.xMax - width * num4);
				GUI.color = new Color(1f, 1f, 1f, num2 * 0.7f);
				GenUI.DrawTextureWithMaterial(rect, DivineGraceCostTex, null);
				GUI.color = Color.white;
				return result;
			}

			return result;
		}

		return result;
	}

	protected override string GetTooltip()
	{
		tmpDrainGenes.Clear();
		var text = $"{gene.ResourceLabel.CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor)}: {gene.ValueForDisplay} / {gene.MaxForDisplay}\n";

		return text;
	}
}
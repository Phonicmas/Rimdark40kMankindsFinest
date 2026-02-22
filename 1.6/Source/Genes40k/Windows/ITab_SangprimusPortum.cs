using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Genes40k;

public class ITab_SangprimusPortum : ITab
{
	private GameComponent_UnlockedMaterials GameComp => Current.Game?.GetComponent<GameComponent_UnlockedMaterials>();
	private SortedList<int, (ThingDef chapter, ThingDef primarch)> AllMaterials => GameComp.AllMaterialsPaired;

	public override bool IsVisible => SelThing != null && base.IsVisible;

	private static readonly Color ThingLabelColor = ITab_Pawn_Gear.ThingLabelColor;
        
	private static readonly Color ThingLabelColorMissing = new Color(0.5f, 0.5f, 0.5f, 1f);

	private static readonly Color ThingHighlightColor = ITab_Pawn_Gear.HighlightColor;
        
	private static readonly Color LineColour = new Color(0.3f, 0.3f, 0.3f, 1f);

	public override bool VisibleInBlueprintMode => false;
        
	private readonly string containedItemsKey;
        
	private float lastDrawnHeight;
        
	private Vector2 scrollPosition;
        
	public ITab_SangprimusPortum()
	{
		size = new Vector2(460f, 450f);
		labelKey = "BEWH.MankindsFinest.Containers.SangprimusPortumAcquired";
		containedItemsKey = "BEWH.MankindsFinest.Containers.SangprimusPortumAcquired";
	}
        
	protected override void FillTab()
	{
		var outRect = new Rect(default, size).ContractedBy(10f);
		outRect.yMin += 20f;
		var rect = new Rect(0f, 0f, outRect.width-20f, Mathf.Max(lastDrawnHeight, outRect.height));
		Text.Font = GameFont.Small;
		Widgets.BeginScrollView(outRect, ref scrollPosition, rect);
		var curY = 0f;
		DoItemsLists(rect, ref curY);
		lastDrawnHeight = curY;
		Widgets.EndScrollView();
	}
        
	private void DoItemsLists(Rect inRect, ref float curY)
	{
		Widgets.BeginGroup(inRect);
		Text.Anchor = TextAnchor.MiddleCenter;
		var rect = new Rect(0f, curY, inRect.width, 30f);
		Widgets.Label(rect, containedItemsKey.Translate());
		curY += 28f;
		Widgets.DrawLineHorizontal(0f, curY, inRect.width);
            
		var list = AllMaterials;
		var flag = false;
		foreach (var t in list)
		{
			flag = true;
			var singleEntity = false;
			var width = inRect.width / 2f;
			var offset = width;
	            
			if (t.Value.chapter == null || t.Value.primarch == null)
			{
				width *= 2f;
				offset = 0f;
				singleEntity = true;
			}
			if (t.Value.chapter != null)
			{
				ThingRow(t.Value.chapter, width, ref curY, 0, t.Value.chapter.GetModExtension<DefModExtension_ChapterMaterial>().shownMaterialName, singleEntity);
			}

			if (t.Value.primarch != null)
			{
				ThingRow(t.Value.primarch, width, ref curY, offset, t.Value.primarch.GetModExtension<DefModExtension_PrimarchMaterial>().shownMaterialName, singleEntity);
			}
	            
			curY += 28f;
			//Seperates each row of materials
			Widgets.DrawBoxSolid(new Rect(0f, curY, inRect.width, 1f), LineColour);
		}
		if (!flag)
		{
			Widgets.NoneLabel(ref curY, inRect.width);
		}
		Widgets.EndGroup();
	}
        
	private void ThingRow(ThingDef thingDef, float width, ref float curY, float xOffset, string label, bool singleEntity)
	{
		var height = 28f;
		var rect = new Rect(0f + xOffset, curY, width, height);
		var acquired = GameComp.HasMaterial(thingDef);
		if (acquired)
		{
			GUI.color = ThingHighlightColor;
			GUI.DrawTexture(rect, TexUI.HighlightTex);
		}
		Text.Anchor = TextAnchor.MiddleCenter;
		GUI.color = acquired ? ThingLabelColor : ThingLabelColorMissing;
		var rect3 = new Rect(xOffset, curY, rect.width, rect.height);
		Text.WordWrap = false;
		if (!acquired)
		{
			var strikethroughWidth = rect.width;
			if (singleEntity)
			{
				strikethroughWidth /= 2;
				xOffset += strikethroughWidth / 2;
			}
			Widgets.DrawBoxSolid(new Rect(xOffset+30f, curY + height/2, strikethroughWidth-60f, 1), LineColour);
		}
		Widgets.Label(rect3, label.Truncate(rect3.width));
		Text.WordWrap = true;
		Text.Anchor = TextAnchor.UpperLeft;
		if (Widgets.ButtonInvisible(rect))
		{
			Find.WindowStack.Add(new Dialog_InfoCard(thingDef));
		}
		TooltipHandler.TipRegion(rect, thingDef.description + "\n\n" + "BEWH.MankindsFinest.Containers.SangprimusPortumMoreInfo".Translate());
	}
}
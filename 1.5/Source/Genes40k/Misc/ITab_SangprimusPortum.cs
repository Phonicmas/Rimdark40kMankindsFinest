using System;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class ITab_SangprimusPortum : ITab
    {
	    private List<Thing> Container => SangprimusPortum.SearchableContents;

	    private SortedList<int, (ThingDef chapter, ThingDef primarch)> AllMaterials => SangprimusPortum.AllMaterialsPaired;

	    public override bool IsVisible => SelThing != null && base.IsVisible;

        private Building_SangprimusPortum SangprimusPortum => SelThing as Building_SangprimusPortum;

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
            labelKey = "BEWH.SangprimusPortumAcquired";
            containedItemsKey = "BEWH.SangprimusPortumAcquired";
        }
        
        protected override void FillTab()
        {
	        var outRect = new Rect(default(Vector2), size).ContractedBy(10f);
	        outRect.yMin += 20f;
	        var rect = new Rect(0f, 0f, outRect.width - 16f, Mathf.Max(lastDrawnHeight, outRect.height));
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
	            
	            var width = inRect.width / 2;
	            if (t.Value.chapter != null)
	            {
		            ThingRow(t.Value.chapter, width, ref curY, 0, t.Value.chapter.GetModExtension<DefModExtension_ChapterMaterial>().shownMaterialName);
	            }

	            if (t.Value.primarch != null)
	            {
		            ThingRow(t.Value.primarch, width, ref curY, width, t.Value.primarch.GetModExtension<DefModExtension_PrimarchMaterial>().shownMaterialName);
	            }
	            
	            curY += 28f;
	            Widgets.DrawBoxSolid(new Rect(0f, curY, inRect.width, 1f), LineColour);
            }
            if (!flag)
            {
                Widgets.NoneLabel(ref curY, inRect.width);
            }
            Widgets.EndGroup();
        }
        
        private void ThingRow(ThingDef thingDef, float width, ref float curY, float xOffset, string label)
		{
			var rect = new Rect(0f + xOffset, curY, width, 28f);
			var acquired = Container.Any(t => t.def == thingDef);
			if (acquired)
			{
				GUI.color = ThingHighlightColor;
				GUI.DrawTexture(rect, TexUI.HighlightTex);
			}
			if (Mouse.IsOver(rect))
			{
				
			}
			if (thingDef.DrawMatSingle != null && thingDef.DrawMatSingle.mainTexture != null)
			{
				var rect2 = new Rect(4f + xOffset, curY, 28f, 28f);
				//Widgets.ThingIcon(rect2, thingDef);
			}
			Text.Anchor = TextAnchor.MiddleLeft;
			GUI.color = acquired ? ThingLabelColor : ThingLabelColorMissing;
			var rect3 = new Rect(36f + xOffset, curY, rect.width - 36f, rect.height);
			Text.WordWrap = false;
			if (!acquired)
			{
				Widgets.DrawBoxSolid(new Rect(26f + xOffset, curY+14, rect.width - 56f, 1), LineColour);
			}
			Widgets.Label(rect3, label.Truncate(rect3.width));
			Text.WordWrap = true;
			Text.Anchor = TextAnchor.UpperLeft;
			TooltipHandler.TipRegion(rect, label.Truncate(rect3.width));
		}
    }
}
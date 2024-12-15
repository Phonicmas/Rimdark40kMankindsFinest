using System;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class ITab_ContentsGeneticStorage : ITab_ContentsBase
    {
        public override IList<Thing> container => GeneticStorage?.slotGroup?.HeldThings.ToList() != null ? GeneticStorage.slotGroup.HeldThings.ToList() : null;

        public override bool IsVisible => SelThing != null && base.IsVisible;

        private Building_GeneticStorage GeneticStorage => SelThing as Building_GeneticStorage;

        public override bool VisibleInBlueprintMode => false;

        public ITab_ContentsGeneticStorage()
        {
            labelKey = "TabCasketContents";
            containedItemsKey = "TabCasketContents";
        }
        
        protected override void DoItemsLists(Rect inRect, ref float curY)
        {
            Widgets.BeginGroup(inRect);
            Widgets.ListSeparator(ref curY, inRect.width, containedItemsKey.Translate());
            var list = container;
            var flag = false;
            foreach (var t in list)
            {
	            if (t == null)
	            {
		            continue;
	            }
	            
	            flag = true;
	            tmpSingleThing.Clear();
	            tmpSingleThing.Add(t);
	            var t1 = t;
	            ThingRow(t.def, t.stackCount, tmpSingleThing, inRect.width, ref curY, delegate(int x)
	            {
		            OnDropThing(t1, x);
	            });
	            tmpSingleThing.Clear();
            }
            if (!flag)
            {
                Widgets.NoneLabel(ref curY, inRect.width);
            }
            Widgets.EndGroup();
        }
        
        private void ThingRow(ThingDef thingDef, int count, List<Thing> things, float width, ref float curY, Action<int> discardAction)
		{
			var rect = new Rect(0f, curY, width, 28f);
			if (things.Count == 1)
			{
				Widgets.InfoCardButton(rect.width - 24f, curY, things[0]);
			}
			else
			{
				Widgets.InfoCardButton(rect.width - 24f, curY, thingDef);
			}
			rect.width -= 24f;
			if (Mouse.IsOver(rect))
			{
				GUI.color = ThingHighlightColor;
				GUI.DrawTexture(rect, TexUI.HighlightTex);
			}
			if (thingDef.DrawMatSingle != null && thingDef.DrawMatSingle.mainTexture != null)
			{
				var rect2 = new Rect(4f, curY, 28f, 28f);
				if (things.Count == 1)
				{
					Widgets.ThingIcon(rect2, things[0]);
				}
				else
				{
					Widgets.ThingIcon(rect2, thingDef);
				}
			}
			Text.Anchor = TextAnchor.MiddleLeft;
			GUI.color = ThingLabelColor;
			var rect3 = new Rect(36f, curY, rect.width - 36f, rect.height);
			var text2 = ((things.Count != 1 || count != things[0].stackCount) ? GenLabel.ThingLabel(thingDef, null, count).CapitalizeFirst() : things[0].LabelCap);
			Text.WordWrap = false;
			Widgets.Label(rect3, text2.StripTags().Truncate(rect3.width));
			Text.WordWrap = true;
			Text.Anchor = TextAnchor.UpperLeft;
			TooltipHandler.TipRegion(rect, text2);
			if (Mouse.IsOver(rect))
			{
				foreach (var t in things)
				{
					TargetHighlighter.Highlight(t);
				}
			}
			curY += 28f;
		}
    }
}
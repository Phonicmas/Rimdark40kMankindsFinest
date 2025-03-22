using System.Collections.Generic;
using Core40k;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class ChapterApparelColourTwo : ApparelColourTwo
    {
        private Genes40kModSettings modSettings = null;

        protected Genes40kModSettings ModSettings => modSettings ?? (modSettings = LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>());

        private bool initialColourSet = false;
        
        private Dictionary<ExtraDecorationDef, bool> originalExtraDecorations = new Dictionary<ExtraDecorationDef, bool>();
        private Dictionary<ExtraDecorationDef, bool> extraDecorations = new Dictionary<ExtraDecorationDef, bool>();

        public Dictionary<ExtraDecorationDef, bool> ExtraDecorationDefs => extraDecorations;

        public void AddOrRemoveDecoration(ExtraDecorationDef decoration)
        {
            if (extraDecorations.ContainsKey(decoration) && extraDecorations[decoration])
            {
                extraDecorations.Remove(decoration);
            }
            else if (extraDecorations.ContainsKey(decoration))
            {
                extraDecorations[decoration] = true;
            }
            else
            {
                extraDecorations.Add(decoration, false);
            }
            Notify_ColorChanged();
        }
        
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (initialColourSet)
            {
                return;
            }
            DrawColor = ModSettings?.chapterColorOne ?? base.DrawColor;
            SetSecondaryColor(ModSettings?.chapterColorTwo ?? base.DrawColorTwo);
            SetUpMisc();
            SetInitialColour();
        }

        public virtual void ApplyColourPreset(ChapterColourDef chapterColour)
        {
            DrawColor = chapterColour.primaryColour;
            SetSecondaryColor(chapterColour.secondaryColour);
            SetInitialColour();
        }
        
        public void ApplyColourPreset(Color primaryColour, Color secondaryColour)
        {
            DrawColor = primaryColour;
            SetSecondaryColor(secondaryColour);
            SetInitialColour();
        }

        public override void SetOriginals()
        {
            originalExtraDecorations = extraDecorations;
            
            base.SetOriginals();
        }

        public override void Reset()
        {
            extraDecorations = originalExtraDecorations;
            
            base.Reset();
        }

        public virtual void SetUpMisc()
        {
            
        }

        protected void SetInitialColour()
        {
            initialColourSet = true;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref initialColourSet, "initialColourSet");
            
            Scribe_Collections.Look(ref extraDecorations, "extraDecorations");
            Scribe_Collections.Look(ref originalExtraDecorations, "originalExtraDecorations");
            base.ExposeData();
        }
    }
}
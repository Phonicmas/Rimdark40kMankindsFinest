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
        protected bool InitialColourSet => initialColourSet;
        
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

        public virtual void SetUpMisc()
        {
            
        }

        protected void SetInitialColour()
        {
            initialColourSet = true;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref initialColourSet, "initialColourSet");
        }
    }
}
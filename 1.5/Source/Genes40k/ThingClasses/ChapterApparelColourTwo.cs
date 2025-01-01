using Core40k;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class ChapterApparelColourTwo : ApparelColourTwo
    {
        private Genes40kModSettings modSettings = null;

        protected Genes40kModSettings ModSettings => modSettings ?? (modSettings = LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>());
        
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            DrawColor = ModSettings?.chapterColorOne ?? base.DrawColor;
            SetSecondaryColor(ModSettings?.chapterColorTwo ?? base.DrawColorTwo);
        }

        public void ApplyColourPreset(ChapterColourDef chapterColour)
        {
            DrawColor = chapterColour.primaryColour;
            SetSecondaryColor(chapterColour.secondaryColour);
        }
    }
}
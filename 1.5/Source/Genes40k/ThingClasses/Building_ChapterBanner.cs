using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Genes40k
{
    [StaticConstructorOnStartup]
    public class Building_ChapterBanner : Building
    {
        public Genes40kModSettings modSettings = null;
        
        public override Graphic Graphic
        {
            get
            {
                if (modSettings == null)
                {
                    modSettings = LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>();
                }

                return modSettings.useChaosVersion ? GetChaosBannerGraphic() : GetImperialBannerGraphic();
            }
        }

        private Graphic GetImperialBannerGraphic()
        {
           return GraphicDatabase.Get<Graphic_Multi>(def.graphicData.texPath, def.graphicData.shaderType.Shader, def.graphicData.drawSize, modSettings.bannerColorOne, modSettings.bannerColorTwo);
        }

        private Graphic GetChaosBannerGraphic()
        {
            const string chaosBannerPath = "Things/Buildings/ChapterBanner/BEWH_ThingChapterBanner_Chaos";
            return GraphicDatabase.Get<Graphic_Multi>(chaosBannerPath, def.graphicData.shaderType.Shader, def.graphicData.drawSize, modSettings.bannerColorOne, modSettings.bannerColorTwo);
        }
        
    }
}
using UnityEngine;
using Verse;

namespace Genes40k;

[StaticConstructorOnStartup]
public class Building_ChapterBanner : Building
{
    private Genes40kModSettings modSettings = null;

    public Genes40kModSettings ModSettings => modSettings ??= LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>();

    public override Color DrawColorTwo => ModSettings?.chapterColorTwo ?? base.DrawColorTwo;

    public override Color DrawColor => ModSettings?.chapterColorOne ?? base.DrawColor;
            
    public override Graphic Graphic => GetImperialBannerGraphic();
        
    private Graphic GetImperialBannerGraphic()
    { 
        const string imperialBannerPathMask = "Things/Building/ChapterBanner/BEWH_ThingChapterBannerm";
        var shader = ShaderDatabase.CutoutComplex;
        if (def.graphicData.shaderType != null)
        {
            shader = def.graphicData.shaderType.Shader;
        }
        return GraphicDatabase.Get<Graphic_Single>(def.graphicData.texPath, shader, def.graphicData.drawSize, ModSettings.chapterColorOne, ModSettings.chapterColorTwo, def.graphicData, imperialBannerPathMask);
    }
}
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Genes40k;

[StaticConstructorOnStartup]
public class Building_DecorativeFlag : Building
{
    private Genes40kModSettings modSettings = null;
    private Genes40kModSettings ModSettings => modSettings ??= LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>();
    
    private Color drawColorOne = Color.white;
    private Color originalColorOne = Color.white;
    public override Color DrawColor => ModSettings?.chapterColorOne ?? base.DrawColor;
    
    private Color drawColorTwo = Color.white;
    private Color originalColorTwo = Color.white;
    public override Color DrawColorTwo => ModSettings?.chapterColorTwo ?? base.DrawColorTwo;

    public override Graphic Graphic => GetGraphic();
    private Graphic GetGraphic()
    {
        var maskPath = def.graphicData.maskPath;
        var shader = ShaderDatabase.CutoutComplex;
        if (def.graphicData.shaderType != null)
        {
            shader = def.graphicData.shaderType.Shader;
        }
        return GraphicDatabase.Get<Graphic_Single>(def.graphicData.texPath, shader, def.graphicData.drawSize, ModSettings.chapterColorOne, ModSettings.chapterColorTwo, def.graphicData, maskPath);

    }
    
    private static readonly CachedTexture EditFlagIcon = new ("UI/Gizmos/BEWH_GestationStartIcon");
        
    [Unsaved(false)]
    private Graphic flagInsigniaGraphic;
    private string flagInsigniaFilePath = "UI/Decoration/LegionBadges/BEWH_iconUI_Aquila";
    public string FlagInsigniaFilePath => flagInsigniaFilePath;
    
    private const string NoIcon = "UI/Decoration/LegionBadges/BEWH_NoneSingle";
    private Graphic FlagInsigniaGraphic => flagInsigniaGraphic ??= GraphicDatabase.Get<Graphic_Single>(flagInsigniaFilePath, ShaderDatabase.Cutout, Vector2.one, Color.white);

    public void SetFlagInsignia(string path, bool noIcon = false)
    {
        flagInsigniaFilePath = noIcon ? NoIcon : path;
        flagInsigniaGraphic = GraphicDatabase.Get<Graphic_Single>(flagInsigniaFilePath, ShaderDatabase.Cutout, Vector2.one, Color.white);
        Notify_ColorChanged();
    }
    
    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        base.DrawAt(drawLoc, flip);

        var newDrawLoc = drawLoc;

        newDrawLoc.y += 0.1f;
        newDrawLoc.z += 0.8f;
        
        FlagInsigniaGraphic.DrawFromDef(newDrawLoc, Rot4.North, null);
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }
        
        var command_Action = new Command_Action
        {
            defaultLabel = "BEWH.MankindsFinest.Decorations.EditFlag".Translate() + "...",
            defaultDesc = "BEWH.MankindsFinest.Decorations.EditFlagDesc".Translate(),
            icon = EditFlagIcon.Texture,
            action = delegate
            {
                Find.WindowStack.Add(new Dialog_EditFlag(this));
            }
        };
        yield return command_Action;
    }
    
    public void SetOriginals()
    {
        originalColorOne = drawColorOne;
        originalColorTwo = drawColorTwo;
    }
    public void Reset()
    {
        drawColorOne = originalColorOne;
        drawColorTwo = originalColorTwo;
        Notify_ColorChanged();
    }
    
    public void SetPrimaryColor(Color color)
    {
        drawColorOne = color;
        Notify_ColorChanged();
    }
    public void SetSecondaryColor(Color color)
    {
        drawColorTwo = color;
        Notify_ColorChanged();
    }
    
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref originalColorOne, "originalColorOne", Color.white);
        Scribe_Values.Look(ref originalColorTwo, "originalColorTwo", Color.white);
        Scribe_Values.Look(ref drawColorTwo, "drawColorTwo", Color.white);
        Scribe_Values.Look(ref drawColorOne, "drawColorOne", Color.white);
        
        Scribe_Values.Look(ref flagInsigniaFilePath, "flagInsigniaFilePath");

        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            Notify_ColorChanged();
        }
    }
}
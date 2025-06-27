using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Genes40k;

[StaticConstructorOnStartup]
public class Building_DecorativeFlag : Building
{
    private Genes40kModSettings modSettings = null;
    private Genes40kModSettings ModSettings => modSettings ??= LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>();

    public Building_DecorativeFlag()
    {
        drawColorOne = ModSettings?.CurrentlySelectedPreset.primaryColour ?? Color.white;
        originalColorOne = drawColorOne;
        drawColorTwo = ModSettings?.CurrentlySelectedPreset.secondaryColour ?? Color.white;
        originalColorTwo = drawColorTwo;

        flagInsigniaFilePath = ModSettings?.CurrentlySelectedPreset.relatedChapterIcon.iconPath ?? originalFlagInsigniaFilePath;

        currentlySelectedPreset = ModSettings?.CurrentlySelectedPreset == ModSettings?.CustomPreset ? null : ModSettings?.CurrentlySelectedPreset;
    }
    
    private Color drawColorOne;
    private Color originalColorOne;
    public override Color DrawColor => drawColorOne;
    
    private Color drawColorTwo;
    private Color originalColorTwo;
    public override Color DrawColorTwo => drawColorTwo;

    public ChapterColourDef currentlySelectedPreset;
    
    public override Graphic Graphic => GetGraphic();
    private Graphic GetGraphic()
    {
        var maskPath = def.graphicData.maskPath;
        var shader = ShaderDatabase.CutoutComplex;
        if (def.graphicData.shaderType != null)
        {
            shader = def.graphicData.shaderType.Shader;
        }
        
        return GraphicDatabase.Get<Graphic_Single>(def.graphicData.texPath, shader, def.graphicData.drawSize, DrawColor, DrawColorTwo, def.graphicData, maskPath);
    }
    
    private static readonly CachedTexture EditFlagIcon = new ("UI/Gizmos/BEWH_CogIcon");
        
    [Unsaved(false)]
    private Graphic flagInsigniaGraphic;
    
    private string originalFlagInsigniaFilePath = "UI/Decoration/LegionBadges/BEWH_iconUI_Aquila";
    private string flagInsigniaFilePath;
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
                Find.WindowStack.Add(new Dialog_ChangeFlagColour(this));
            }
        };
        yield return command_Action;
    }
    
    public void SetOriginals()
    {
        originalFlagInsigniaFilePath = flagInsigniaFilePath;
        originalColorOne = drawColorOne;
        originalColorTwo = drawColorTwo;
    }
    public void Reset()
    {
        flagInsigniaFilePath = originalFlagInsigniaFilePath;
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
        Scribe_Values.Look(ref originalFlagInsigniaFilePath, "originalFlagInsigniaFilePath");
        Scribe_Defs.Look(ref currentlySelectedPreset, "currentlySelectedPreset");

        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            Notify_ColorChanged();
        }
    }
}
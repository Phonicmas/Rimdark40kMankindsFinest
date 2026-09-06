using Core40k;
using RimWorld;
using Verse;

namespace Genes40k;

public class ShoulderIconDef : ExtraDecorationDef
{
    public bool leftShoulder = false;
        
    public bool rightShoulder = false;
        
    public GeneDef relatedChapterGene = null;

    public bool setsNull = false;

    //Older name for colourable, still read so existing defs keep working.
    public bool useColour = false;

    public override void ResolveReferences()
    {
        colourable |= useColour;
        flipable = true;
        showInDecorationTab = false;
        shaderType ??= ShaderTypeDefOf.Cutout;
        base.ResolveReferences();
    }
}

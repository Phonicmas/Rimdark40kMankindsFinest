using System;
using RimWorld;
using Verse;

namespace Genes40k;

[Obsolete]
public class Ability_UpdateGizmo : Ability
{
    public override string Tooltip
    {
        get
        {
            var text = base.Tooltip;

            var warpShield = pawn?.genes?.GetFirstGeneOfType<Gene_WarpShield>();

            if (warpShield == null)
            {
                return text;
            }
                
            var textBit = warpShield.IsShielded ? "BEWH.MankindsFinest.CommonKeywords.On".Translate() : "BEWH.MankindsFinest.CommonKeywords.Off".Translate();
                
            return text + "\n\n" + "BEWH.MankindsFinest.WarpShield.Tooltip".Translate(textBit);
        }
    }

    public Ability_UpdateGizmo()
    {
    }

    public Ability_UpdateGizmo(Pawn pawn)
    {
        this.pawn = pawn;
    }

    public Ability_UpdateGizmo(Pawn pawn, Precept sourcePrecept)
    {
        this.pawn = pawn;
        this.sourcePrecept = sourcePrecept;
    }

    public Ability_UpdateGizmo(Pawn pawn, AbilityDef def)
    {
        this.pawn = pawn;
        this.def = def;
        Initialize();
    }

    public Ability_UpdateGizmo(Pawn pawn, Precept sourcePrecept, AbilityDef def)
    {
        this.pawn = pawn;
        this.def = def;
        this.sourcePrecept = sourcePrecept;
        Initialize();
    }
}
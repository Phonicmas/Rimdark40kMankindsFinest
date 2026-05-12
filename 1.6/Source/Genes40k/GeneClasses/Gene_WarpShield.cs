using System;
using System.Collections.Generic;
using Verse;

namespace Genes40k;

public class Gene_WarpShield : Gene
{
    public bool IsShielded = false;

    public override IEnumerable<Gizmo> GetGizmos()
    {
        var toggleCommand = new Command_Action
        {
            defaultLabel = "BEWH.MankindsFinest.WarpShield.TurnX".Translate(IsShielded ? "BEWH.MankindsFinest.CommonKeywords.Off".Translate() : "BEWH.MankindsFinest.CommonKeywords.On".Translate()),
            icon = IsShielded ? Genes40kUtils.MindShieldOffIcon : Genes40kUtils.MindShieldOnIcon,
            action = () => IsShielded = !IsShielded,
        };

        yield return toggleCommand;
    }

    [Obsolete]
    private bool OldAbilityRemoved = false;
    [Obsolete]
    private void RemoveOldAbility()
    {
        pawn.abilities.RemoveAbility(Genes40kDefOf.BEWH_WarpShield);
        OldAbilityRemoved = true;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref IsShielded, "IsShielded");
        Scribe_Values.Look(ref OldAbilityRemoved, "OldAbilityRemoved");

        if (Scribe.mode == LoadSaveMode.PostLoadInit && !OldAbilityRemoved)
        {
            RemoveOldAbility();
        }
    }
}
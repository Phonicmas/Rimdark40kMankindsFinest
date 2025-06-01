using Core40k;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k;

public class LivingSaintBodyDecorativeApparelColourTwo : BodyDecorativeApparelColourTwo
{
    private BodyTypeDef originalBodyType = null;

    private static readonly Color GoldColour = new(0.8358f, 0.619f, 0.298f);
    private static readonly Color RedColour = new(0.407f, 0.094f, 0.129f);

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        if (InitialColourSet)
        {
            return;
        }
        SetInitialColours(GoldColour, RedColour);
        AddOrRemoveDecoration(Genes40kDefOf.BEWH_LivingSaintHalo_2);
        AddOrRemoveDecoration(Genes40kDefOf.BEWH_LivingSaintWings_1);
    }
    
    public override void Notify_Equipped(Pawn pawn)
    {
        if (pawn.story.bodyType != BodyTypeDefOf.Female)
        {
            originalBodyType = pawn.story.bodyType;
            pawn.story.bodyType = BodyTypeDefOf.Female;
        }
        base.Notify_Equipped(pawn);
    }

    public override void Notify_Unequipped(Pawn pawn)
    {
        if (originalBodyType != null)
        {
            pawn.story.bodyType = originalBodyType;
        }
        base.Notify_Unequipped(pawn);
    }

    public override void ExposeData()
    {
        Scribe_Defs.Look(ref originalBodyType, "originalBodyType");
        base.ExposeData();
    }
}
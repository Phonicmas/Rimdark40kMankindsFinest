using Core40k;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k;

public class LivingSaintBodyDecorativeApparelColourTwo : BodyDecorativeApparelColourTwo
{
    private BodyTypeDef originalBodyType = null;

    private static readonly Color GoldColour = new(0.878f, 0.615f, 0.239f);
    private static readonly Color RedColour = new(0.584f, 0.133f, 0.137f);

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        if (InitialColourSet)
        {
            return;
        }
        SetInitialColours(GoldColour, RedColour);
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
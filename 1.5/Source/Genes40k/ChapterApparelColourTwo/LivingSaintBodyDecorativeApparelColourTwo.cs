using Core40k;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k;

public class LivingSaintBodyDecorativeApparelColourTwo : BodyDecorativeApparelColourTwo
{
    private BodyTypeDef originalBodyType = null;

    private static readonly Color GoldColour = new(224, 157, 61, 1);
    private static readonly Color RedColour = new(149,34,35, 1);

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        if (InitialColourSet)
        {
            return;
        }
        //Self made colours not work?
        SetInitialColours(Color.yellow, Color.red);
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
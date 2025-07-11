﻿using Verse;

namespace Genes40k;

public class PawnRenderNodeWorker_LivingSaintAttachmentBody : PawnRenderNodeWorker_AttachmentBody
{
    public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
    {
        var lSaintApparel = parms.pawn.apparel.WornApparel.FirstOrFallback(a => a.def == Genes40kDefOf.BEWH_LivingSaintArmor);

        return lSaintApparel == null && base.CanDrawNow(node, parms);
    }
}
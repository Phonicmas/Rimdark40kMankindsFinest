using Verse;

namespace Genes40k;

public class PawnRenderNodeWorker_LivingSaintAttachmentHead : PawnRenderNodeWorker_FlipWhenCrawling
{
    public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
    {
        var lSaintApparel = parms.pawn.apparel.WornApparel.FirstOrFallback(a => a is LivingSaintBodyDecorativeApparelColourTwo);

        return lSaintApparel == null && base.CanDrawNow(node, parms);
    }
}
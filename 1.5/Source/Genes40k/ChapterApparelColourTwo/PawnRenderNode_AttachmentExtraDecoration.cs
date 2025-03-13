using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Genes40k
{
    public class PawnRenderNode_AttachmentExtraDecoration : PawnRenderNode
    {
        public PawnRenderNode_AttachmentExtraDecoration(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }

        protected override IEnumerable<Graphic> GraphicsFor(Pawn pawn)
        {
            var apparelColourTwo = (ExtraIconsChapterApparelColourTwo)apparel;

            var drawnTextures = apparelColourTwo.ExtraDecorationDefs.Select(def => def.drawnTextureIconPath);

            //somewhere check apparel for its renders nodes, or check this for its parents to then add more renders nodes or some shit likke that
            //need to find parent render node, somehow be able to add chldren.
            
            foreach (var texture in drawnTextures)
            {
                yield return GraphicDatabase.Get<Graphic_Multi>(texture, ShaderFor(pawn), Props.drawSize, apparelColourTwo.DrawColor, apparelColourTwo.DrawColorTwo);
            }
        }
    }
}
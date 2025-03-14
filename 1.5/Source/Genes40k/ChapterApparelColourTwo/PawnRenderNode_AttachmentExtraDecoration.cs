using UnityEngine;
using Verse;

namespace Genes40k
{
    public class PawnRenderNode_AttachmentExtraDecoration : PawnRenderNode
    {
        public PawnRenderNode_AttachmentExtraDecoration(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }

        public override Graphic GraphicFor(Pawn pawn)
        {
            Props.color ??= Color.white;
            
            return GraphicDatabase.Get<Graphic_Multi>(Props.texPath, ShaderFor(pawn), Props.drawSize, Props.color.Value);
        }
    }
}
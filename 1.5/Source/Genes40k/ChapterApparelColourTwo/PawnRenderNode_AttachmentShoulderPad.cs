using System;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class PawnRenderNode_AttachmentShoulderPad : PawnRenderNode
    {
        public PawnRenderNode_AttachmentShoulderPad(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }

        public override Graphic GraphicFor(Pawn pawn)
        {
            var apparelColourTwo = (ChapterApparelColourTwo)apparel;

            return GraphicDatabase.Get<Graphic_Multi>(Props.texPath, ShaderFor(pawn), Props.drawSize, apparelColourTwo.DrawColor, apparelColourTwo.DrawColorTwo);
        }
    }
}
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Genes40k
{
    [StaticConstructorOnStartup]
    public class Window_PrimarchEmbryoGenes : Window
    {
        private PrimarchEmbryo embryo;
        
        private Vector2 scrollPosition;

        private Vector2 size;

        public Window_PrimarchEmbryoGenes(PrimarchEmbryo embryo)
        {
            this.embryo = embryo;
            size = new Vector2(Mathf.Min(736f, UI.screenWidth), 550f);
        }
        
        public override void DoWindowContents(Rect inRect)
        {
            GeneUIUtility.DrawGenesInfo(new Rect(0f, 20f, size.x, size.y - 20f), embryo, 550f, ref size, ref scrollPosition);
        }
    }
}
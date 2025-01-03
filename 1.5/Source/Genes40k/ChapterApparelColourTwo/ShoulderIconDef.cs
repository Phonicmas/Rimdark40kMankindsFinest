using Core40k;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class ShoulderIconDef : Def
    {
        [NoTranslate]
        public string iconPath;
        
        [Unsaved]
        private Texture2D icon;

        public Texture2D Icon
        {
            get
            {
                if (icon != null)
                {
                    return icon;
                }
                
                icon = !iconPath.NullOrEmpty() ? ContentFinder<Texture2D>.Get(iconPath) : BaseContent.BadTex;
                return icon;
            }
        }

        [NoTranslate]
        public string drawnTextureIconPath;

        public bool leftShoulder = false;
        
        public bool rightShoulder = false;

        public float sortOrder = 0f;

        public RankDef mustHaveRank = null;
        
        //public GeneDef mustHaveGene = null;
        
        //public TraitDef mustHaveTrait = null;
        
        //public HediffDef mustHaveHediff = null;
    }
}   
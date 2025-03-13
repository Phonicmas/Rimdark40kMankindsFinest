using Core40k;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class ExtraDecorationDef : Def
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
                
                icon = !iconPath.NullOrEmpty() ? ContentFinder<Texture2D>.Get(iconPath) : ContentFinder<Texture2D>.Get(Genes40kDefOf.BEWH_ShoulderNone.iconPath);
                return icon;
            }
        }

        [NoTranslate]
        public string drawnTextureIconPath;

        public bool flip = false;

        public float sortOrder = 0f;
    }
}   
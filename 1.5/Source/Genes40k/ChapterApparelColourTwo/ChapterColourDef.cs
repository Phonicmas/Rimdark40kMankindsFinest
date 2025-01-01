using Core40k;
using UnityEngine;
using Verse;

namespace Genes40k
{
    public class ChapterColourDef : ColourPresetDef
    {
        public GeneDef relatedChapterGene = null;
        
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

        public string relatedChapterIconPath = null;
    }
}   
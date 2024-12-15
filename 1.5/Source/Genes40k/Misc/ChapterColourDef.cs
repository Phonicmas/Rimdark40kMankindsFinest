using UnityEngine;
using Verse;

namespace Genes40k
{
    public class ChapterColourDef : Def
    {
        public Color primaryColour = Color.white;
        public Color secondaryColour = Color.white;
        
        public GeneDef relatedChapterGene = null;
    }
}
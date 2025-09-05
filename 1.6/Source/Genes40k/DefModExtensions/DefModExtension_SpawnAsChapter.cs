using System.Collections.Generic;
using Verse;

namespace Genes40k;

public class DefModExtension_SpawnAsChapter : DefModExtension
{
    public bool loyalist = true;
    
    public List<ChapterColourDef> specificChapters = new List<ChapterColourDef>();
}
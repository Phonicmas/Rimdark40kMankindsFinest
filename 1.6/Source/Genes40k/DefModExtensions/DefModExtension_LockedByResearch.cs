using System.Collections.Generic;
using Verse;

namespace Genes40k;

public class DefModExtension_LockedByResearch : DefModExtension
{
    public List<ResearchProjectDef> researchPrerequisites = new List<ResearchProjectDef>();
}
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Genes40k;

public class DefModExtension_ThoughtXenophobiaWhitelist : DefModExtension
{
    public List<XenotypeDef> xenotypesNotHated = new List<XenotypeDef>();
    public bool hateIfDifferentIdeo = false;
}
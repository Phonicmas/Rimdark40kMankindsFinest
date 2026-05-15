using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Genes40k;

public class DefModExtension_ThoughtXenophobiaWhitelist : DefModExtension
{
    public List<XenotypeDef> xenotypesNotHated = new();
    public List<XenotypeDef> xenotypesImpure = new();
    public bool hateIfDifferentIdeo = false;
}
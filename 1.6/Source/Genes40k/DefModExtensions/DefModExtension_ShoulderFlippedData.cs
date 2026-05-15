using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Genes40k;

public class DefModExtension_ShoulderFlippedData : DefModExtension
{
    public Dictionary<Rot4, Vector3> offsetForRankWhenFacing = new();
    
    public Dictionary<Rot4, Vector3> offsetForChapterWhenFacing = new();
}
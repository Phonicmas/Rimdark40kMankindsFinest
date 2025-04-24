using UnityEngine;
using Verse;

namespace Genes40k;

public class DefModExtension_PrimarchVatTexture : DefModExtension
{
    public string earlyFetusTexture = "Other/VatGrownFetus_EarlyStage";
    public Vector2 earlyFetusSize = Vector2.one;
    public Vector3 earlyFetusOffset = Vector3.zero;
        
    public string lateFetusTexture = "Other/VatGrownFetus_LateStage";
    public Vector2 lateFetusSize = Vector2.one;
    public Vector3 lateFetusOffset = Vector3.zero;
}
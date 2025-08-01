using Core40k;
using Verse;

namespace Genes40k;

public class ShoulderIconSettings : ExtraDecorationSettings
{
    public ShoulderIconDef ShoulderIcon = null;

    public ShoulderIconSettings()
    {
    }
    
    public ShoulderIconSettings(ShoulderIconSettings shoulderIconSettings)
    {
        ShoulderIcon = shoulderIconSettings.ShoulderIcon;
        Flipped = shoulderIconSettings.Flipped;
        Color = shoulderIconSettings.Color;
        ColorTwo = shoulderIconSettings.ColorTwo;
        ColorThree = shoulderIconSettings.ColorThree;
    }
    
    public override void ExposeData()
    {
        Scribe_Defs.Look(ref ShoulderIcon, "ShoulderIcon");
        base.ExposeData();
    }
}
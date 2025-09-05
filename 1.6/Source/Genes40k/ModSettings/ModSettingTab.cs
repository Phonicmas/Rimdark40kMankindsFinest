using UnityEngine;
using Verse;

namespace Genes40k;

public abstract class ModSettingTab
{
    public abstract void DrawTab(Rect inRect, Genes40kModSettings settings);
}
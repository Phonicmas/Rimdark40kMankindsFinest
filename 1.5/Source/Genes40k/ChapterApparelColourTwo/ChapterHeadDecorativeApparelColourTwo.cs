﻿using Core40k;
using Verse;

namespace Genes40k;

public class ChapterHeadDecorativeApparelColourTwo : HeadDecorativeApparelColourTwo
{
    private Genes40kModSettings modSettings = null;

    protected Genes40kModSettings ModSettings => modSettings ??= LoadedModManager.GetMod<Genes40kMod>().GetSettings<Genes40kModSettings>();

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        if (InitialColourSet)
        {
            return;
        }

        SetInitialColours(ModSettings?.chapterColorOne ?? base.DrawColor, ModSettings?.chapterColorTwo ?? base.DrawColorTwo);
    }
}
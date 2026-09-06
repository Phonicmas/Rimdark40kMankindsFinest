using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace Genes40k;

[StaticConstructorOnStartup]
public class Building_GeneticStorage : Building_Storage
{
    [Unsaved]
    private DefModExtension_GeneStorageGraphicProgression cachedDefMod;

    private DefModExtension_GeneStorageGraphicProgression DefMod => cachedDefMod ??= def.GetModExtension<DefModExtension_GeneStorageGraphicProgression>();
        
    [Unsaved]
    private Graphic cachedHalfFullGraphic;

    private Graphic HalfFullGraphic =>
        cachedHalfFullGraphic ??= GraphicDatabase.Get<Graphic_Multi>(DefMod.halfFullGraphic, ShaderDatabase.DefaultShader,
            def.graphicData.drawSize, Color.white, Color.white, DefaultGraphic.data);

    [Unsaved]
    private Graphic cachedFullGraphic;

    private Graphic FullGraphic =>
        cachedFullGraphic ??= GraphicDatabase.Get<Graphic_Multi>(DefMod.fullGraphic,
            ShaderDatabase.DefaultShader, def.graphicData.drawSize, Color.white, Color.white,
            DefaultGraphic.data);
        
    [Unsaved]
    private bool countsDirty = true;
    [Unsaved]
    private int storedCount;
    [Unsaved]
    private int maximumItems;
    [Unsaved]
    private string countLabel;
    [Unsaved]
    private bool? isSangprimusPortum;

    private bool IsSangprimusPortum => isSangprimusPortum ??= def.HasModExtension<DefModExtension_SangprimusPortum>();

    /// <summary>
    /// Stored/maximum counts are only recomputed after something enters or leaves the storage.
    /// </summary>
    private void RefreshCounts()
    {
        if (!countsDirty)
        {
            return;
        }

        countsDirty = false;
        storedCount = slotGroup?.HeldThings.Count() ?? 0;
        maximumItems = def.building.maxItemsInCell * AllSlotCells().Count();
        countLabel = storedCount + "/" + maximumItems;
    }

    public override Graphic Graphic
    {
        get
        {
            RefreshCounts();

            if (DefMod.halfFullGraphic.NullOrEmpty())
            {
                return storedCount == maximumItems ? FullGraphic : DefaultGraphic;
            }

            var filledPercent = (float)storedCount / maximumItems;
            if (filledPercent < 0.5f)
            {
                return DefaultGraphic;
            }
            return filledPercent < 1 ? HalfFullGraphic : FullGraphic;
        }
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        countsDirty = true;
    }

    public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
    {
        foreach (var item in slotGroup.HeldThings)
        {
            UnhideItem(item);
        }
        base.Destroy(mode);
    }

    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        foreach (var item in slotGroup.HeldThings)
        {
            UnhideItem(item);
        }
        base.DeSpawn(mode);
    }

    public override void Kill(DamageInfo? dinfo = null, Hediff exactCulprit = null)
    {
        foreach (var item in slotGroup.HeldThings)
        {
            UnhideItem(item);
        }
        base.Kill(dinfo, exactCulprit);
    }

    public override void Notify_ReceivedThing(Thing newItem)
    {
        countsDirty = true;
        switch (newItem)
        {
            case GeneseedVial geneseedVial:
                geneseedVial.ChangeVisibility(true);
                break;
            case GeneticMatrix geneticMatrix:
                geneticMatrix.ChangeVisibility(true);
                break;
            case PrimarchEmbryo primarchEmbryo:
                primarchEmbryo.ChangeVisibility(true);
                break;
        }
        base.Notify_ReceivedThing(newItem);
    }

    public override void Notify_LostThing(Thing newItem)
    {
        countsDirty = true;
        UnhideItem(newItem);
        base.Notify_LostThing(newItem);
    }

    private void UnhideItem(Thing item)
    {
        switch (item)
        {
            case GeneseedVial geneseedVial:
                geneseedVial.ChangeVisibility(false);
                break;
            case GeneticMatrix geneticMatrix:
                geneticMatrix.ChangeVisibility(false);
                break;
            case PrimarchEmbryo primarchEmbryo:
                primarchEmbryo.ChangeVisibility(false);
                break;
        }
    }
        
    public override void DrawGUIOverlay()
    {
        if (IsSangprimusPortum)
        {
            return;
        }

        RefreshCounts();
        GenMapUI.DrawThingLabel(this, countLabel);
    }
}
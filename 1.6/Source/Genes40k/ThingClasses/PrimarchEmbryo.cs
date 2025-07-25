﻿using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Genes40k;

[StaticConstructorOnStartup]
public class PrimarchEmbryo : GeneSetHolderBase
{
    public XenotypeIconDef iconDef;
    public XenotypeDef xenotype;

    private Pawn mother;
    private Pawn father;

    public Pawn Mother
    {
        get
        {
            if (mother == null)
            {
                var randomMother = Find.WorldPawns?.AllPawnsAlive?.FirstOrFallback(pawn => pawn.gender == Gender.Female && pawn.genes.Xenotype == XenotypeDefOf.Baseliner);
                if (randomMother == null)
                {
                    PawnGenerator.GeneratePawn(new PawnGenerationRequest(Faction.OfPlayer.def.basicMemberKind, Faction.OfPlayer, fixedGender: Gender.Female, biologicalAgeRange: new FloatRange(21, 46), allowedXenotypes: new List<XenotypeDef>() { XenotypeDefOf.Baseliner }));
                }
                mother = randomMother;
            }

            return mother;
        }
    }

    public Pawn Father
    {
        get
        {
            if (father == null)
            {
                var randomFather = Find.WorldPawns?.AllPawnsAlive?.FirstOrFallback(pawn => pawn.gender == Gender.Male && pawn.genes.Xenotype == XenotypeDefOf.Baseliner);
                if (randomFather == null)
                {
                    PawnGenerator.GeneratePawn(new PawnGenerationRequest(Faction.OfPlayer.def.basicMemberKind, Faction.OfPlayer, fixedGender: Gender.Male, biologicalAgeRange: new FloatRange(21, 46), allowedXenotypes: new List<XenotypeDef>() { XenotypeDefOf.Baseliner }));
                }
                father = randomFather;
            }

            return father;
        }
    }
    

    public GeneSet PrimarchGenes
    {
        get
        {
            if (primarchGenes == null)
            {
                primarchGenes = new GeneSet();
            }

            if (primarchGenes.GenesListForReading.NullOrEmpty())
            {
                foreach (var gene in Genes40kUtils.PrimarchGenes)
                {
                    primarchGenes.AddGene(gene);
                }
            }

            return primarchGenes;
        }
    }
    private GeneSet primarchGenes;
    public GeneSet birthGenes;
        
    private bool invisible = false;
        
    public override Graphic Graphic
    {
        get
        {
            var graphic = DefaultGraphic.GetCopy(def.graphicData.drawSize, null);
                
            graphic.drawSize = !invisible ? def.graphicData.drawSize : Vector2.zero;
                
            return graphic;
        }
    }

    public override void PostMake()
    {
        base.PostMake();
        geneSet = new GeneSet();
        birthGenes = new GeneSet();
        primarchGenes = new GeneSet();
    }

    public override void Notify_DebugSpawned()
    {
        if (Map.mapPawns.AllPawns.Where(x => x.RaceProps.Humanlike && x.gender == Gender.Male).TryRandomElement(out var result))
        {
            father = result;
        }
        if (Map.mapPawns.AllPawns.Where( x => x.RaceProps.Humanlike && x.gender == Gender.Female).TryRandomElement(out var result2))
        {
            mother = result2;
        }
        
        birthGenes = PregnancyUtility.GetInheritedGeneSet(father, Mother);
        geneSet = birthGenes;

        foreach (var gene in Genes40kUtils.PrimarchGenes)
        {
            primarchGenes.AddGene(gene);
        }

        xenotype = Genes40kDefOf.BEWH_Primarch;
    }

    public void Initialize(Pawn mother, Pawn father, GeneSet primarchGenes, GeneSet birthGenes, XenotypeIconDef iconDef, XenotypeDef xenotype)
    {
        this.mother = mother;
        this.father = father;
        this.primarchGenes = primarchGenes;
        this.birthGenes = birthGenes;
        this.iconDef = iconDef;
        this.xenotype = xenotype;

        birthGenes ??= PregnancyUtility.GetInheritedGeneSet(father, Mother);
        
        foreach (var gene in birthGenes.GenesListForReading)
        {
            geneSet.AddGene(gene);
        }
    }
        
    public void ChangeVisibility(bool newValue)
    {
        invisible = newValue;
    }
        
    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var gizmo in base.GetGizmos())
        {
            if (gizmo.ToString().Contains("InspectGenes".Translate() + "..."))
            {
                continue;
            }
            yield return gizmo;
        }
        if (geneSet != null)
        {
            yield return new Command_Action
            {
                defaultLabel = "InspectGenes".Translate() + "...",
                defaultDesc = "InspectGenesEmbryoDesc".Translate(),
                icon = GeneticInfoTex.Texture,
                action = delegate
                {
                    Genes40kUtils.InspectPrimarchEmbryoGenes(this);
                }
            };
        }
    }
        
    public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
    {
        foreach (var item in base.SpecialDisplayStats())
        {
            yield return item;
        }
        if (geneSet == null)
        {
            yield break;
        }
        Dialog_InfoCard.Hyperlink? inspectGenesHyperlink = null;
        if (ThingSelectionUtility.SelectableByMapClick(this))
        {
            inspectGenesHyperlink = new Dialog_InfoCard.Hyperlink(this, -1, thingIsGeneOwner: true);
        }
        foreach (var item3 in primarchGenes.SpecialDisplayStats(inspectGenesHyperlink))
        {
            yield return item3;
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref mother, "mother");
        Scribe_References.Look(ref father, "father");
        Scribe_Defs.Look(ref xenotype, "xenotype");
        Scribe_Defs.Look(ref iconDef, "iconDef");
        Scribe_Deep.Look(ref primarchGenes, "primarchGenes");
        Scribe_Deep.Look(ref birthGenes, "birthGenes");
        Scribe_Values.Look(ref invisible, "invisible");

        if (Scribe.mode != LoadSaveMode.PostLoadInit)
        {
            return;
        }
            
        geneSet ??= new GeneSet();

        if (birthGenes == null)
        {
            return;
        }
            
        foreach (var gene in birthGenes.GenesListForReading)
        {
            geneSet.AddGene(gene);
        }
    }
}
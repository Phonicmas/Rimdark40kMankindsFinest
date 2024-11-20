using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;


namespace Genes40k
{
    [StaticConstructorOnStartup]
    public class PrimarchEmbryo : GeneSetHolderBase
    {
        protected override string InspectGeneDescription => "InspectGenesEmbryoDesc".Translate();

        public XenotypeIconDef iconDef;
        public XenotypeDef xenotype;

        public Pawn mother;
        public Pawn father;

        public GeneSet primarchGenes;
        public GeneSet birthGenes;

        public override void PostMake()
        {
            base.PostMake();
            geneSet = new GeneSet();
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
            birthGenes = PregnancyUtility.GetInheritedGeneSet(father, mother);
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
            
            foreach (var gene in birthGenes.GenesListForReading)
            {
                geneSet.AddGene(gene);
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

            if (Scribe.mode != LoadSaveMode.PostLoadInit)
            {
                return;
            }
            
            if (geneSet == null)
            {
                geneSet = new GeneSet();
            }

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
}
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
        private List<Building_PrimarchGrowthVat> tmpEligibleVats = new List<Building_PrimarchGrowthVat>();

        protected override string InspectGeneDescription => "InspectGenesEmbryoDesc".Translate();

        public const int EmbryoGestationTicks = 540000;

        public XenotypeIconDef iconDef;
        public XenotypeDef xenotype;

        public Thing implantTarget;

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
            geneSet = PregnancyUtility.GetInheritedGeneSet(father, mother);
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
            foreach (var gene in primarchGenes.GenesListForReading)
            {
                geneSet.AddGene(gene);
            }            
        }

        private Building_PrimarchGrowthVat BestAvailableGrowthVat()
        {
            if (MapHeld == null)
            {
                return null;
            }
            var list = MapHeld.listerBuildings.AllBuildingsColonistOfDef(Genes40kDefOf.BEWH_PrimarchGrowthVat);
            tmpEligibleVats.Clear();
            foreach (var item in list)
            {
                if (item is Building_PrimarchGrowthVat building_GrowthVat && !building_GrowthVat.Working && building_GrowthVat.selectedEmbryo == null && building_GrowthVat.SelectedPawn == null)
                {
                    tmpEligibleVats.Add(building_GrowthVat);
                }
            }
            if (tmpEligibleVats.NullOrEmpty())
            {
                return null;
            }
            tmpEligibleVats.SortBy((Building_PrimarchGrowthVat v) => (!v.PowerOn) ? 1 : 0, (Building_PrimarchGrowthVat v) => (base.PositionHeld - v.Position).LengthHorizontal);
            return tmpEligibleVats[0];
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            if (!Find.Storyteller.difficulty.ChildrenAllowed || base.MapHeld == null)
            {
                yield break;
            }

            var bestVat = BestAvailableGrowthVat();
            var command_Action2 = new Command_Action
            {
                defaultLabel = "InsertGrowthVatLabel".Translate() + "...",
                defaultDesc = "InsertEmbryoGrowthVatDesc".Translate(EmbryoGestationTicks.ToStringTicksToPeriod()).Resolve(),
                icon = Building_PrimarchGrowthVat.InsertEmbryoIcon.Texture,
                activateSound = SoundDefOf.Tick_Tiny,
                action = delegate
                {
                    bestVat.SelectEmbryo(this);
                    implantTarget = bestVat;
                }
            };
            if (bestVat == null)
            {
                command_Action2.Disable("ImplantDisabledNoVats".Translate());
            }
            yield return command_Action2;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref implantTarget, "implantTarget");
            Scribe_References.Look(ref mother, "mother");
            Scribe_References.Look(ref father, "father");
            Scribe_Defs.Look(ref xenotype, "xenotype");
            Scribe_Defs.Look(ref iconDef, "iconDef");
            Scribe_Deep.Look(ref primarchGenes, "primarchGenes");
            Scribe_Deep.Look(ref birthGenes, "birthGenes");
            
            if (Scribe.mode != LoadSaveMode.PostLoadInit) return;
            
            if (geneSet == null)
            {
                geneSet = new GeneSet();
            }
            if (birthGenes != null)
            {
                foreach (var gene in birthGenes.GenesListForReading)
                {
                    geneSet.AddGene(gene);
                }
            }

            if (primarchGenes == null) return;
            
            foreach (var gene in primarchGenes.GenesListForReading)
            {
                geneSet.AddGene(gene);
            }
        }
    }
}
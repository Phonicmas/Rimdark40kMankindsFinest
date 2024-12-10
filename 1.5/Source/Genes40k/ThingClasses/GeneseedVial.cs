﻿using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;


namespace Genes40k
{
    [StaticConstructorOnStartup]
    public class GeneseedVial : ThingWithComps
    {
        protected GeneSet geneSet;

        public string xenotypeName;

        public XenotypeDef xenotype;

        public XenotypeIconDef iconDef;

        public GeneDef extraGeneFromMaterial = null;

        private bool invisible = false;
        
        private static readonly CachedTexture GeneticInfoTex = new CachedTexture("UI/Gizmos/ViewGenes");

        private const int MaxGeneLabels = 5;

        private List<string> tmpGeneLabelsDesc = new List<string>();

        private List<string> tmpGeneLabels = new List<string>();

        public GeneSet GeneSet => geneSet;

        public override string DescriptionDetailed
        {
            get
            {
                tmpGeneLabelsDesc.Clear();
                var text = base.DescriptionDetailed;
                if (geneSet == null || !geneSet.GenesListForReading.Any())
                {
                    return text;
                }
                if (!text.NullOrEmpty())
                {
                    text += "\n\n";
                }
                foreach (var t in geneSet.GenesListForReading)
                {
                    tmpGeneLabelsDesc.Add(t.label);
                }
                return text + ("Genes".Translate().CapitalizeFirst() + ":\n" + tmpGeneLabelsDesc.ToLineList("  - ", capitalizeItems: true));
            }
        }

        public override string LabelNoCount
        {
            get
            {
                if (xenotypeName.NullOrEmpty())
                {
                    return base.LabelNoCount;
                }
                return "BEWH.NamedGeneseedVial".Translate(xenotypeName.Named("NAME"));
            }
        }

        public override void PostMake()
        {
            base.PostMake();
            geneSet = new GeneSet();
            Initialize();
        }

        public void ChangeVisibility(bool newValue)
        {
            invisible = newValue;
        }
        
        public override Graphic Graphic
        {
            get
            {
                var graphic = DefaultGraphic.GetCopy(def.graphicData.drawSize, null);
                
                graphic.drawSize = !invisible ? def.graphicData.drawSize : Vector2.zero;
                
                return graphic;
            }
        }

        public void AddExtraGene(GeneDef gene)
        {
            geneSet.AddGene(gene);
        }

        public void AddExtraGenes(List<GeneDef> genes)
        {
            if (genes.NullOrEmpty())
            {
                return;
            }
            
            foreach (var gene in genes)
            {
                geneSet.AddGene(gene);
            }
        }

        public void Initialize()
        {
            var defMod = def.GetModExtension<DefModExtension_GeneseedVial>();

            if (!defMod.xenotype.genes.NullOrEmpty())
            {
                foreach (var gene in defMod.xenotype.genes)
                {
                    geneSet.AddGene(gene);
                }
            }

            xenotype = defMod.xenotype; 
            xenotypeName = defMod.xenotype.label;
            iconDef = defMod.xenotypeIcon;
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var gizmo in base.GetGizmos())
            {
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
                        Genes40kUtils.InspectGeneseedVialGenes(this);
                    }
                };
            }
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            foreach (FloatMenuOption floatMenuOption in base.GetFloatMenuOptions(selPawn))
            {
                yield return floatMenuOption;
            }

            if (def != Genes40kDefOf.BEWH_GeneseedVialPrimarch)
            {
                yield break;
            }
            
            var humanEmbryos = Map.listerThings.ThingsOfDef(ThingDefOf.HumanEmbryo);

            if (humanEmbryos.NullOrEmpty())
            {
                yield break;
            }
            
            var pawn = Find.Selector.SelectedPawns.FirstOrDefault();
            var geneTable = Map.listerThings.ThingsOfDef(Genes40kDefOf.BEWH_GeneManipulationTable);
            
            if (pawn == null || geneTable.NullOrEmpty())
            {
                yield return new FloatMenuOption("BEWH.NoGeneManipulationTable".Translate(), null);
                yield break;
            }

            var recipe = Genes40kDefOf.BEWH_MakePrimarchEmbryo;

            if (!recipe.PawnSatisfiesSkillRequirements(pawn))
            {
                var text = recipe.skillRequirements.Aggregate("", (current, skillRequirement) => current + (skillRequirement.skill.label + ": " + skillRequirement.minLevel + " "));

                text = text.Trim();
                yield return new FloatMenuOption("BEWH.NotSkilledEnoughPrimarchEmbryo".Translate(pawn, text), null);
                yield break;
            }
            
            foreach (var humanEmbryo in humanEmbryos)
            {
                if (!(humanEmbryo is HumanEmbryo embryo))
                {
                    continue;
                }
                
                var text = "BEWH.CraftPrimarchEmbryo".Translate(embryo.Mother.NameShortColored);
                        
                yield return new FloatMenuOption(text, delegate
                {
                    var chosenGeneTable = (Building_GeneTable)geneTable.MinBy(table => table.Position.DistanceTo(Position));
                    var thingCount = new List<ThingCount>
                    {
                        new ThingCount(this, 1),
                        new ThingCount(humanEmbryo, 1)
                    };
                    
                    var bill = recipe.MakeNewBill();
                    bill.billStack = new BillStack(chosenGeneTable);
                    
                    var job = WorkGiver_DoBill.TryStartNewDoBillJob(pawn, bill, chosenGeneTable, thingCount, out _);
                    pawn.jobs.TryTakeOrderedJob(job);
                    chosenGeneTable.billStack.AddBill(bill);
                });
            }
        }
        
        public override string GetInspectString()
        {
            var text = base.GetInspectString();
            tmpGeneLabels.Clear();
            if (geneSet == null || !geneSet.GenesListForReading.Any()) return text;
            
            if (!text.NullOrEmpty())
            {
                text += "\n";
            }
            var genesListForReading = geneSet.GenesListForReading;
            var num = Mathf.Min(MaxGeneLabels, genesListForReading.Count);
            
            for (var i = 0; i < num; i++)
            {
                var text2 = genesListForReading[i].label;
                if (geneSet.IsOverridden(genesListForReading[i]))
                {
                    text2 += " (" + "Overridden".Translate() + ")";
                }
                tmpGeneLabels.Add(text2);
            }
            if (genesListForReading.Count > num)
            {
                tmpGeneLabels.Add("Etc".Translate() + "...");
            }
            text += "Genes".Translate().CapitalizeFirst() + ":\n" + tmpGeneLabels.ToLineList("  - ", capitalizeItems: true);
            return text;
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
            foreach (var item2 in geneSet.SpecialDisplayStats(inspectGenesHyperlink))
            {
                yield return item2;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref xenotypeName, "xenotypeName");
            Scribe_Defs.Look(ref iconDef, "iconDef");
            Scribe_Defs.Look(ref extraGeneFromMaterial, "extraGeneFromMaterial");
            Scribe_Deep.Look(ref geneSet, "geneSet");
            Scribe_Values.Look(ref invisible, "invisible");
            
            if (iconDef == null && Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                iconDef = XenotypeIconDefOf.Basic;
            }
        }
    }
}
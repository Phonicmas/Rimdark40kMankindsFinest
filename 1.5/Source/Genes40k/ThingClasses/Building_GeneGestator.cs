using Core40k;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Genes40k
{

    [StaticConstructorOnStartup]
    public class Building_GeneGestator : Building
    {
        public ThingDef selectedMatrix = null;

        public Thing containedMatrix = null;

        public ThingDef selectedMaterial = null;

        public bool haulJobStarted = false;

        public Pawn jobDoer = null;

        public bool hasBeenStarted = false;

        private float totalTime = 0;

        private float progressInt = 0;

        public bool InProgress => totalTime - progressInt > 0 && hasBeenStarted;

        public bool Finished => totalTime - progressInt <= 0 && containedMatrix != null;


        [Unsaved(false)]
        private Effecter progressBar;

        private static readonly Texture2D CancelIcon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel");
        
        private static readonly Texture2D EjectIcon = ContentFinder<Texture2D>.Get("UI/Gizmos/BEWH_EjectMatrixIcon");

        private static readonly Texture2D StartIcon = ContentFinder<Texture2D>.Get("UI/Gizmos/BEWH_GestationStartIcon");

        private static readonly Texture2D EmptyChapterMaterialIcon = ContentFinder<Texture2D>.Get("Things/Item/ChapterMaterial/BEWH_ChapterMaterial_None");
        
        private static readonly Texture2D EmptyPrimarchMaterialIcon = ContentFinder<Texture2D>.Get("Things/Item/PrimarchMaterial/BEWH_PrimarchMaterial_None");
        
        private static readonly Texture2D MatrixSelectionTex = ContentFinder<Texture2D>.Get("Things/Item/GeneMatrix/BEWH_GeneMatrix_Empty");

        public bool PowerOn => PowerTraderComp.PowerOn;

        [Unsaved(false)]
        private CompPowerTrader cachedPowerComp;

        private CompPowerTrader PowerTraderComp => cachedPowerComp ?? (cachedPowerComp = this.TryGetComp<CompPowerTrader>());

        public void AddGeneMatrix(Thing geneMatrix)
        {
            var singleGeneMatrix = geneMatrix.stackCount > 1 ? geneMatrix.SplitOff(1) : geneMatrix;
            containedMatrix = singleGeneMatrix;
            
            totalTime = containedMatrix.def.GetModExtension<DefModExtension_GeneMatrix>().ticksToGestate;
            progressInt = 0;
            haulJobStarted = false;
            singleGeneMatrix.Destroy();
        }

        private List<ThingDef> AvailableGestatables()
        {
            var defMod = def.GetModExtension<DefModExtension_GeneGestator>();
            var result = new List<ThingDef>();
            if (defMod.gestatableThings.NullOrEmpty())
            {
                return result;
            }

            result.AddRange(defMod.gestatableThings.Where(thing => thing.GetModExtension<DefModExtension_GeneMatrix>().researchNeeded.IsFinished));
            return result;
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            if (progressBar != null)
            {
                progressBar.Cleanup();
                progressBar = null;
            }
            base.DeSpawn(mode);
        }

        public override void Tick()
        {
            base.Tick();
            if (!hasBeenStarted) return;
            
            if (PowerOn)
            {
                if (Finished)
                {
                    Finish();
                }
                else
                {
                    TickEffects();
                    progressInt++;
                }
            }
            else
            {
                if (progressBar == null) return;
                
                progressBar.Cleanup();
                progressBar = null;
            }
        }

        private void TickEffects()
        {
            if (progressBar == null)
            {
                progressBar = EffecterDefOf.ProgressBarAlwaysVisible.Spawn();
            }
            progressBar.EffectTick(new TargetInfo(this.TrueCenter().ToIntVec3(), Map), TargetInfo.Invalid);
            var mote = ((SubEffecter_ProgressBar)progressBar.children[0]).mote;
            if (mote == null) return;
            
            mote.progress = Mathf.Clamp01(progressInt / totalTime);
            mote.offsetZ = Rotation == Rot4.North ? 0.5f : -0.5f;
        }

        private void Finish()
        {
            var geneseedVial = (GeneseedVial)ThingMaker.MakeThing(containedMatrix.def.GetModExtension<DefModExtension_GeneMatrix>().makesGeneVial);

            if (selectedMaterial != null)
            {
                geneseedVial.extraGeneFromMaterial = selectedMaterial.GetModExtension<DefModExtension_GeneFromMaterial>().addedGene;
                //geneseedVial.AddExtraGene(selectedMaterial.GetModExtension<DefModExtension_GeneFromMaterial>().addedGene);
            }

            GenSpawn.Spawn(geneseedVial, InteractionCell, Map);

            var letter = LetterMaker.MakeLetter("BEWH.GestateFinishLetter".Translate(), "BEWH.GestateFinishMessage".Translate(selectedMatrix.label, geneseedVial.Label), LetterDefOf.PositiveEvent, geneseedVial);

            Find.LetterStack.ReceiveLetter(letter);
            Reset();
        }

        private void Reset()
        {
            totalTime = 0;
            progressInt = 0;
            if (progressBar != null)
            {
                progressBar.Cleanup();
                progressBar = null;
            }
            haulJobStarted = false;
            hasBeenStarted = false;
            selectedMatrix = null;
            containedMatrix = null;
            selectedMaterial = null;
            jobDoer = null;
        }

        public override string GetInspectString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());
            stringBuilder.Append("\n");

            stringBuilder.Append(containedMatrix == null ? "BEWH.ContainsNoGeneMatrix".Translate() : "BEWH.ContainsGeneMatrix".Translate(containedMatrix.Label));

            if (selectedMaterial != null)
            {
                stringBuilder.Append("\n");
                stringBuilder.Append("BEWH.ContainsExtraMaterial".Translate(selectedMaterial.label));
            }

            if (!InProgress) return stringBuilder.ToString();
            
            stringBuilder.Append("\n");
            if (Finished)
            {
                stringBuilder.Append("BEWH.GeneVialFinished".Translate());
            }
            else
            {
                var divider = 60000f;
                string timeDenoter = "LetterDay".Translate();

                var timeLeft = totalTime - progressInt;

                if (timeLeft < 60000)
                {
                    divider = 2500f;
                    timeDenoter = "LetterHour".Translate();
                }
                stringBuilder.Append("BEWH.GeneVialFinishesIn".Translate(Math.Round(timeLeft / divider, 2), timeDenoter));
            }

            return stringBuilder.ToString();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            if (DebugSettings.ShowDevGizmos)
            {
                if (hasBeenStarted)
                {
                    //DEV: ALMOST FINISH
                    var command_Action5 = new Command_Action();
                    command_Action5.defaultLabel = "DEV: ALMOST FINISH".Translate();
                    command_Action5.icon = CancelIcon;
                    command_Action5.activateSound = SoundDefOf.Designate_Cancel;
                    command_Action5.action = delegate
                    {
                        progressInt = totalTime - 250;
                    };
                    yield return command_Action5;
                }
            }
            if (!hasBeenStarted && containedMatrix != null)
            {
                if (this.HasComp<CompAffectedByFacilities>() && (containedMatrix.def.GetModExtension<DefModExtension_GeneMatrix>().canUsePrimarchMaterial || containedMatrix.def.GetModExtension<DefModExtension_GeneMatrix>().canUseChapterMaterial))
                {
                    var comp = GetComp<CompAffectedByFacilities>();
                    if (!comp.LinkedFacilitiesListForReading.Where(x => x.def == Genes40kDefOf.BEWH_SangprimusPortum).EnumerableNullOrEmpty())
                    {
                        var sangprimus = (Building_SangprimusPortum)comp.LinkedFacilitiesListForReading.First(b => b is Building_SangprimusPortum);
                        var availableMaterial = new List<Thing>();
                        string geneticType;

                        Texture2D emptyMaterialIcon = null;
                        if (containedMatrix.def.GetModExtension<DefModExtension_GeneMatrix>().canUsePrimarchMaterial)
                        {
                            availableMaterial.AddRange(sangprimus.SearchableContentsPrimarch.Where(x => x.def.HasModExtension<DefModExtension_PrimarchMaterial>()));
                            geneticType = "BEWH.Primarch".Translate();
                            emptyMaterialIcon = EmptyPrimarchMaterialIcon;
                        }
                        else
                        {
                            availableMaterial.AddRange(sangprimus.SearchableContentsChapter.Where(x => x.def.HasModExtension<DefModExtension_ChapterMaterial>()));
                            geneticType = "BEWH.Chapter".Translate();
                            emptyMaterialIcon = EmptyChapterMaterialIcon;
                        }
                        //SELECT PRIMARCH OR CHAPTER MATERIAL
                        var command_Action10 = new Command_Action();
                        command_Action10.defaultLabel = "BEWH.SelectXMaterial".Translate(geneticType);
                        command_Action10.defaultDesc = "BEWH.SelectXMaterialDesc".Translate(geneticType);

                        command_Action10.icon = selectedMaterial == null ? emptyMaterialIcon : selectedMaterial.GetModExtension<DefModExtension_GeneFromMaterial>().addedGene.Icon;
                       
                        command_Action10.action = delegate
                        {
                            var list = new List<FloatMenuOption>();
                            foreach (var material in availableMaterial)
                            {
                                if (selectedMaterial == material.def)
                                {
                                    continue;
                                }
                                var text = material.Label;
                                list.Add(new FloatMenuOption(text, delegate
                                {
                                    selectedMaterial = material.def;
                                }));
                            }
                            if (selectedMaterial != null)
                            {
                                list.Add(new FloatMenuOption("NoneBrackets".Translate(), delegate { selectedMaterial = null; }));
                            }
                            if (!list.Any())
                            {
                                list.Add(new FloatMenuOption("BEWH.NoAvailableMaterial".Translate(), null));
                            }
                            Find.WindowStack.Add(new FloatMenu(list));
                        };
                        yield return command_Action10;
                    }
                }
                //STARTS MACHINE
                var command_Action4 = new Command_Action();
                command_Action4.defaultLabel = "BEWH.StartGeneGestating".Translate();
                command_Action4.defaultDesc = "BEWH.StartGeneGestatingDesc".Translate();
                command_Action4.icon = StartIcon;
                command_Action4.activateSound = SoundDefOf.Designate_Cancel;
                command_Action4.action = delegate
                {
                    hasBeenStarted = true;
                };
                yield return command_Action4;

                //EJECT LOADED MATRIX
                var command_Action2 = new Command_Action();
                command_Action2.defaultLabel = "BEWH.EjectGeneMatrix".Translate(containedMatrix.Label);
                command_Action2.defaultDesc = "BEWH.EjectGeneMatrixDesc".Translate(containedMatrix.Label);
                command_Action2.icon = EjectIcon;
                command_Action2.activateSound = SoundDefOf.Designate_Cancel;
                command_Action2.action = delegate
                {
                    GenSpawn.Spawn(selectedMatrix, InteractionCell, Map);
                    Reset();
                };
                yield return command_Action2;
            }
            if (containedMatrix == null)
            {
                if (!haulJobStarted)
                {
                    //SELECTS MATRIX TO LOAD AND START HAUL JOB
                    var command_Action1 = new Command_Action();
                    command_Action1.defaultLabel = "BEWH.SelectMatrix".Translate() + "...";
                    command_Action1.defaultDesc = "BEWH.SelectMatrixDesc".Translate();
                    command_Action1.icon = selectedMatrix == null ? MatrixSelectionTex : selectedMatrix.uiIcon;
                    
                    var gestatablesAvailable = new List<ThingDef>();
                    gestatablesAvailable.AddRange(AvailableGestatables());
                    command_Action1.action = delegate
                    {
                        var list = new List<FloatMenuOption>();
                        foreach (var gestatables in gestatablesAvailable)
                        {
                            var text = gestatables.label;
                            var floatMenu = new FloatMenuOption(text, delegate
                            {
                                selectedMatrix = gestatables;
                                haulJobStarted = true;
                                if (containedMatrix == null)
                                {
                                    return;
                                }
                                
                                GenPlace.TryPlaceThing(containedMatrix, InteractionCell, Map, ThingPlaceMode.Direct);
                                containedMatrix = null;
                            });

                            var doesNotHaveMatrix = Map.listerThings.ThingsOfDef(gestatables).NullOrEmpty();
                            
                            if (doesNotHaveMatrix)
                            {
                                floatMenu.Disabled = true;
                            }

                            list.Add(floatMenu);
                        }
                        if (!list.Any())
                        {
                            list.Add(new FloatMenuOption("BEWH.NoAvailableMaterial".Translate(), null));
                        }
                        Find.WindowStack.Add(new FloatMenu(list));
                    };
                    if (!PowerOn)
                    {
                        command_Action1.Disable("NoPower".Translate().CapitalizeFirst());
                    }
                    if (gestatablesAvailable.NullOrEmpty())
                    {
                        command_Action1.Disable("BEWH.MissingGestateResearch".Translate().CapitalizeFirst());
                    }
                    yield return command_Action1;
                    
                }
                else
                {
                    //STOPS HAUL JOB
                    var command_Action3 = new Command_Action();
                    command_Action3.defaultLabel = "CommandCancelLoad".Translate();
                    command_Action3.defaultDesc = "CommandCancelLoadDesc".Translate();
                    command_Action3.icon = CancelIcon;
                    command_Action3.activateSound = SoundDefOf.Designate_Cancel;
                    command_Action3.action = delegate
                    {
                        haulJobStarted = false;
                        if (jobDoer == null) return;
                        
                        foreach (var job in jobDoer.jobs.AllJobs())
                        {
                            if (job.def != Genes40kDefOf.BEWH_FillGeneGestator)
                            {
                                continue;
                            }
                            
                            jobDoer.jobs.EndCurrentOrQueuedJob(job, JobCondition.InterruptForced);
                            Reset();
                            break;
                        }
                    };
                    yield return command_Action3;
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref totalTime, "totalTime", 0f);
            Scribe_Values.Look(ref progressInt, "progress", 0f);
            Scribe_Defs.Look(ref selectedMatrix, "selectedMatrix");
            Scribe_Defs.Look(ref selectedMaterial, "selectedMaterial");
            Scribe_Deep.Look(ref containedMatrix, "containedMatrix");
            Scribe_Values.Look(ref haulJobStarted, "haulJobStarted", false);
            Scribe_Values.Look(ref hasBeenStarted, "hasBeenStarted", false);
            Scribe_References.Look(ref jobDoer, "jobDoer");
        }
    }
}
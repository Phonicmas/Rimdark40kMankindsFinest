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

        public bool InProgress
        {
            get { return totalTime - progressInt > 0 && hasBeenStarted; }
        }

        public bool Finished
        {
            get { return totalTime - progressInt <= 0 && containedMatrix != null; }
        }


        [Unsaved(false)]
        private Effecter progressBar;

        private static readonly Texture2D CancelIcon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel");

        private static readonly Texture2D EmptyMaterialIcon = ContentFinder<Texture2D>.Get("Things/Item/GeneMatrix/GeneMatrix_Empty");

        [Unsaved(false)]
        private Texture2D cachedMatrixSelectionTex;

        public Texture2D matrixSelectionTex
        {
            get
            {
                if (cachedMatrixSelectionTex == null)
                {
                    cachedMatrixSelectionTex = ContentFinder<Texture2D>.Get("Things/Item/GeneMatrix/GeneMatrix_Empty");
                }
                return cachedMatrixSelectionTex;
            }
        }

        public bool PowerOn => PowerTraderComp.PowerOn;

        [Unsaved(false)]
        private CompPowerTrader cachedPowerComp;

        private CompPowerTrader PowerTraderComp
        {
            get
            {
                if (cachedPowerComp == null)
                {
                    cachedPowerComp = this.TryGetComp<CompPowerTrader>();
                }
                return cachedPowerComp;
            }
        }

        public void AddGeneMatrix(Thing geneMatrix)
        {
            if (geneMatrix.stackCount > 1)
            {
                containedMatrix = geneMatrix.SplitOff(1);
            }
            else
            {
                containedMatrix = geneMatrix;
            }
            totalTime = containedMatrix.def.GetModExtension<DefModExtension_GeneMatrix>().ticksToGestate;
            progressInt = 0;
            haulJobStarted = false;
            geneMatrix.Destroy();
        }

        private List<ThingDef> AvailableGestatables()
        {
            DefModExtension_GeneGestator defMod = def.GetModExtension<DefModExtension_GeneGestator>();
            List<ThingDef> result = new List<ThingDef>();
            if (!defMod.gestatableThings.NullOrEmpty())
            {
                foreach (ThingDef thing in defMod.gestatableThings)
                {
                    if (thing.GetModExtension<DefModExtension_GeneMatrix>().researchNeeded.IsFinished)
                    {
                        result.Add(thing);
                    }
                }
            }
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
            if (hasBeenStarted)
            {
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
                    if (progressBar != null)
                    {
                        progressBar.Cleanup();
                        progressBar = null;
                    }
                }
            }
            //At end, eject the gene vial and make a letter to notify the user.
        }

        private void TickEffects()
        {
            if (progressBar == null)
            {
                progressBar = EffecterDefOf.ProgressBarAlwaysVisible.Spawn();
            }
            progressBar.EffectTick(new TargetInfo(this.TrueCenter().ToIntVec3(), base.Map), TargetInfo.Invalid);
            MoteProgressBar mote = ((SubEffecter_ProgressBar)progressBar.children[0]).mote;
            if (mote != null)
            {
                mote.progress = Mathf.Clamp01((float)progressInt / totalTime);
                mote.offsetZ = ((base.Rotation == Rot4.North) ? 0.5f : (-0.5f));
            }
        }

        private void Finish()
        {
            GeneseedVial geneseedVial = (GeneseedVial)GenSpawn.Spawn(containedMatrix.def.GetModExtension<DefModExtension_GeneMatrix>().makesGeneVial, InteractionCell, Map);

            if (selectedMaterial != null)
            {
                geneseedVial.AddExtraGene(selectedMaterial.GetModExtension<DefModExtension_GeneFromMaterial>().addedGene);
            }

            ChoiceLetter letter = LetterMaker.MakeLetter("BEWH.GestateFinishLetter".Translate(), "BEWH.GestateFinishMessage".Translate(selectedMatrix.label, geneseedVial.Label), LetterDefOf.PositiveEvent, geneseedVial);

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
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());
            stringBuilder.Append("\n");

            if (containedMatrix == null)
            {
                stringBuilder.Append("BEWH.ContainsNoGeneMatrix".Translate());
            }
            else
            {
                stringBuilder.Append("BEWH.ContainsGeneMatrix".Translate(containedMatrix.Label));
            }

            if (selectedMaterial != null)
            {
                stringBuilder.Append("\n");
                stringBuilder.Append("BEWH.ContainsExtraMaterial".Translate(selectedMaterial.label));
            }

            if (InProgress)
            {
                stringBuilder.Append("\n");
                if (Finished)
                {
                    stringBuilder.Append("BEWH.GeneVialFinished".Translate());
                }
                else
                {
                    float divider = 60000f;
                    string timeDenoter = "LetterDay".Translate();

                    float timeLeft = totalTime - progressInt;

                    if (timeLeft < 60000)
                    {
                        divider = 2500f;
                        timeDenoter = "LetterHour".Translate();
                    }
                    stringBuilder.Append("BEWH.GeneVialFinishesIn".Translate(Math.Round(timeLeft / divider, 2), timeDenoter));
                }
            }

            return stringBuilder.ToString();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            if (DebugSettings.ShowDevGizmos)
            {
                if (hasBeenStarted)
                {
                    //ALMOST FINISHES PROCEDURE
                    Command_Action command_Action5 = new Command_Action();
                    command_Action5.defaultLabel = "DEV: ALMOST FINISH".Translate();
                    command_Action5.icon = CancelIcon;
                    command_Action5.activateSound = SoundDefOf.Designate_Cancel;
                    command_Action5.action = delegate
                    {
                        progressInt = totalTime - 100;
                    };
                    yield return command_Action5;
                }
            }
            if (!hasBeenStarted && containedMatrix != null)
            {
                if (this.HasComp<CompAffectedByFacilities>() && (containedMatrix.def.HasModExtension<DefModExtension_PrimarchMaterial>() || containedMatrix.def.HasModExtension<DefModExtension_ChapterMaterial>()))
                {
                    CompAffectedByFacilities comp = GetComp<CompAffectedByFacilities>();
                    if (!comp.LinkedFacilitiesListForReading.Where(x => x.def == Genes40kDefOf.BEWH_SangprimusPortum).EnumerableNullOrEmpty())
                    {
                        Building_GeneStorage sangprimus = (Building_GeneStorage)comp.LinkedFacilitiesListForReading.First();
                        List<Thing> availableMaterial = new List<Thing>();
                        string geneticType;
                        if (containedMatrix.def.HasModExtension<DefModExtension_PrimarchMaterial>())
                        {
                            availableMaterial.AddRange(sangprimus.SearchableContents.Where(x => x.def.HasModExtension<DefModExtension_PrimarchMaterial>()));
                            geneticType = "BEWH.Primarch".Translate();
                        }
                        else
                        {
                            availableMaterial.AddRange(sangprimus.SearchableContents.Where(x => x.def.HasModExtension<DefModExtension_ChapterMaterial>()));
                            geneticType = "BEWH.Chapter".Translate();
                        }
                        Command_Action command_Action10 = new Command_Action();
                        command_Action10.defaultLabel = "BEWH.SelectXMaterial".Translate(geneticType);
                        command_Action10.defaultDesc = "BEWH.SelectXMaterialDesc".Translate(geneticType);

                        if (selectedMaterial == null)
                        {
                            command_Action10.icon = EmptyMaterialIcon;
                        }
                        else
                        {
                            command_Action10.icon = selectedMaterial.GetModExtension<DefModExtension_GeneFromMaterial>().addedGene.Icon;
                        }
                       
                        command_Action10.action = delegate
                        {
                            List<FloatMenuOption> list = new List<FloatMenuOption>();
                            string text;
                            foreach (Thing material in availableMaterial)
                            {
                                if (selectedMaterial == material.def)
                                {
                                    continue;
                                }
                                text = material.Label;
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
                Command_Action command_Action4 = new Command_Action();
                command_Action4.defaultLabel = "BEWH.StartGeneGestating".Translate();
                command_Action4.defaultDesc = "BEWH.StartGeneGestatingDesc".Translate();
                command_Action4.icon = CancelIcon;
                command_Action4.activateSound = SoundDefOf.Designate_Cancel;
                command_Action4.action = delegate
                {
                    hasBeenStarted = true;
                };
                yield return command_Action4;

                //EJECT LOADED MATRIX
                Command_Action command_Action2 = new Command_Action();
                command_Action2.defaultLabel = "BEWH.EjectGeneMatrix".Translate(containedMatrix.Label);
                command_Action2.defaultDesc = "BEWH.EjectGeneMatrixDesc".Translate(containedMatrix.Label);
                command_Action2.icon = CancelIcon;
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
                    //SELECTS MATRIX TO LOAD
                    Command_Action command_Action1 = new Command_Action();
                    command_Action1.defaultLabel = "BEWH.SelectMatrix".Translate() + "...";
                    command_Action1.defaultDesc = "BEWH.SelectMatrixDesc".Translate();
                    if (selectedMatrix == null)
                    {
                        command_Action1.icon = matrixSelectionTex;
                    }
                    else
                    {
                        command_Action1.icon = selectedMatrix.uiIcon;
                    }
                    
                    List<ThingDef> gestatablesAvailable = new List<ThingDef>();
                    gestatablesAvailable.AddRange(AvailableGestatables());
                    command_Action1.action = delegate
                    {
                        //Make a float menu for slection specialization just like when selecting a pawn to insert
                        List<FloatMenuOption> list = new List<FloatMenuOption>();
                        string text;
                        foreach (ThingDef gestatables in gestatablesAvailable)
                        {
                            text = gestatables.label;
                            list.Add(new FloatMenuOption(text, delegate
                            {
                                selectedMatrix = gestatables;
                                haulJobStarted = true;
                                if (containedMatrix != null)
                                {
                                    GenPlace.TryPlaceThing(containedMatrix, InteractionCell, Map, ThingPlaceMode.Direct);
                                    containedMatrix = null;
                                }
                            }));
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
                    Command_Action command_Action3 = new Command_Action();
                    command_Action3.defaultLabel = "CommandCancelLoad".Translate();
                    command_Action3.defaultDesc = "CommandCancelLoadDesc".Translate();
                    command_Action3.icon = CancelIcon;
                    command_Action3.activateSound = SoundDefOf.Designate_Cancel;
                    command_Action3.action = delegate
                    {
                        haulJobStarted = false;
                        if (jobDoer != null)
                        {
                            foreach (Job job in jobDoer.jobs.AllJobs())
                            {
                                if (job.def == Genes40kDefOf.BEWH_FillGeneGestator)
                                {
                                    jobDoer.jobs.EndCurrentOrQueuedJob(job, JobCondition.InterruptForced);
                                    Reset();
                                    break;
                                }
                            }
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
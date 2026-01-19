using RimWorld;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Genes40k;

[StaticConstructorOnStartup]
public class Building_GeneGestator : Building
{
    public ThingDef selectedMatrix = null;
    public Thing containedMatrix = null;

    public ThingDef selectedMaterial = null;

    private bool doWork = false;
    private float totalTime = 0;
    private float TotalTimeAdjusted => totalTime * (Genes40kUtils.ModSettings.matrixGestationTimeFactor/100f);
    private float progressInt = 0;
    public bool InProgress => TotalTimeAdjusted - progressInt > 0;
    public bool Finished => TotalTimeAdjusted - progressInt <= 0 && containedMatrix != null;
    
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
    private CompPowerTrader PowerTraderComp => cachedPowerComp ??= this.TryGetComp<CompPowerTrader>();
    
    [Unsaved(false)]
    private Building_SangprimusPortum cachedNearbySangprimusPortum;
    private Building_SangprimusPortum NearbySangprimusPortum => cachedNearbySangprimusPortum ??= (Building_SangprimusPortum)GetComp<CompAffectedByFacilities>()?.LinkedFacilitiesListForReading.FirstOrFallback(b => b is Building_SangprimusPortum);

    public void AddGeneMatrix(Thing geneMatrix)
    {
        var singleGeneMatrix = geneMatrix.stackCount > 1 ? geneMatrix.SplitOff(1) : geneMatrix;
        containedMatrix = singleGeneMatrix;
            
        totalTime = containedMatrix.def.GetModExtension<DefModExtension_GeneMatrix>().ticksToGestate;
        progressInt = 0;
        singleGeneMatrix.Destroy();
        selectedMatrix = null;
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

    protected override void Tick()
    {
        base.Tick();
        if (!doWork)
        {
            return;
        }
            
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
        progressBar ??= EffecterDefOf.ProgressBarAlwaysVisible.Spawn();
        progressBar.EffectTick(new TargetInfo(this.TrueCenter().ToIntVec3(), Map), TargetInfo.Invalid);
        var mote = ((SubEffecter_ProgressBar)progressBar.children[0]).mote;
        if (mote == null) return;
            
        mote.progress = Mathf.Clamp01(progressInt / TotalTimeAdjusted);
        mote.offsetZ = Rotation == Rot4.North ? 0.5f : -0.5f;
    }

    private void Finish()
    {
        var geneseedVial = (GeneseedVial)ThingMaker.MakeThing(containedMatrix.def.GetModExtension<DefModExtension_GeneMatrix>().makesGeneVial);
        
        if (selectedMaterial != null)
        {
            geneseedVial.extraGeneFromMaterial = selectedMaterial.GetModExtension<DefModExtension_GeneFromMaterial>().addedGene;
        }
        
        GenSpawn.Spawn(geneseedVial, InteractionCell, Map);
        
        var letter = LetterMaker.MakeLetter("BEWH.MankindsFinest.GeneGestator.GestateFinishLetter".Translate(), "BEWH.MankindsFinest.GeneGestator.GestateFinishMessage".Translate(containedMatrix.def.label, geneseedVial.Label), LetterDefOf.PositiveEvent, geneseedVial);
        
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
        doWork = false;
        selectedMatrix = null;
        containedMatrix = null;
        selectedMaterial = null;
    }

    public override string GetInspectString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(base.GetInspectString());
        stringBuilder.Append("\n");

        stringBuilder.Append(containedMatrix == null ? "BEWH.MankindsFinest.GeneGestator.ContainsNoGeneMatrix".Translate() : "BEWH.MankindsFinest.GeneGestator.ContainsGeneMatrix".Translate(containedMatrix.Label));

        if (selectedMaterial != null)
        {
            stringBuilder.Append("\n");
            stringBuilder.Append("BEWH.MankindsFinest.GeneGestator.ContainsExtraMaterial".Translate(selectedMaterial.label));
        }

        if (!doWork)
        {
            return stringBuilder.ToString();
        }
            
        stringBuilder.Append("\n");
        if (Finished)
        {
            stringBuilder.Append("BEWH.MankindsFinest.GeneGestator.GeneVialFinished".Translate());
        }
        else
        {
            var divider = 60000f;
            string timeDenoter = "LetterDay".Translate();

            var timeLeft = TotalTimeAdjusted - progressInt;

            if (timeLeft < 60000)
            {
                divider = 2500f;
                timeDenoter = "LetterHour".Translate();
            }
            stringBuilder.Append("BEWH.MankindsFinest.GeneGestator.GeneVialFinishesIn".Translate(Math.Round(timeLeft / divider, 2), timeDenoter));
        }

        return stringBuilder.ToString();
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }

        if (!doWork)
        {
            if (containedMatrix != null)
            { 
                //SELECT PRIMARCH OR CHAPTER MATERIAL
                if (CanUseAnyMaterial())
                {
                    if (NearbySangprimusPortum != null)
                    {
                        var sangprimus = NearbySangprimusPortum;
                        var availableMaterial = new List<Thing>();
                        string geneticType;

                        Texture2D emptyMaterialIcon = null;
                        if (containedMatrix.def.GetModExtension<DefModExtension_GeneMatrix>().canUsePrimarchMaterial)
                        {
                            availableMaterial.AddRange(sangprimus.SearchableContentsPrimarch.Where(x => x.def.HasModExtension<DefModExtension_PrimarchMaterial>()));
                            geneticType = "BEWH.MankindsFinest.CommonKeywords.Primarch".Translate();
                            emptyMaterialIcon = EmptyPrimarchMaterialIcon;
                        }
                        else
                        {
                            availableMaterial.AddRange(sangprimus.SearchableContentsChapter.Where(x => x.def.HasModExtension<DefModExtension_ChapterMaterial>()));
                            geneticType = "BEWH.MankindsFinest.CommonKeywords.Chapter".Translate();
                            emptyMaterialIcon = EmptyChapterMaterialIcon;
                        }
                    
                        var command_Action10 = new Command_Action();
                        command_Action10.defaultLabel = "BEWH.MankindsFinest.GeneGestator.SelectXMaterial".Translate(geneticType);
                        command_Action10.defaultDesc = "BEWH.MankindsFinest.GeneGestator.SelectXMaterialDesc".Translate(geneticType);

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
                                list.Add(new FloatMenuOption("BEWH.MankindsFinest.GeneGestator.NoAvailableMaterial".Translate(), null));
                            }
                            Find.WindowStack.Add(new FloatMenu(list));
                        };
                        yield return command_Action10;
                    }
                }
                
                //STARTS MACHINE
                var command_Action2 = new Command_Action
                {
                    defaultLabel = "BEWH.MankindsFinest.GeneGestator.StartGeneGestating".Translate(),
                    defaultDesc = "BEWH.MankindsFinest.GeneGestator.StartGeneGestatingDesc".Translate(),
                    icon = StartIcon,
                    activateSound = SoundDefOf.Designate_Cancel,
                    action = delegate
                    {
                        if (selectedMaterial == null && CanUseAnyMaterial() && NearbySangprimusPortum != null)
                        {
                            var sangprimus = NearbySangprimusPortum;
                            var availableMaterialAmount = 0;
                            
                            if (containedMatrix.def.GetModExtension<DefModExtension_GeneMatrix>().canUsePrimarchMaterial)
                            {
                                availableMaterialAmount += sangprimus.SearchableContentsPrimarch.Count(x => x.def.HasModExtension<DefModExtension_PrimarchMaterial>());
                            }
                            else
                            {
                                availableMaterialAmount += sangprimus.SearchableContentsChapter.Count(x => x.def.HasModExtension<DefModExtension_ChapterMaterial>());
                            }

                            if (availableMaterialAmount > 0)
                            {
                                var confirmationText = "BEWH.MankindsFinest.GeneGestator.NoGeneMaterialSelectedProceed".Translate();
                                Find.WindowStack.Add(new Dialog_MessageBox(confirmationText, "Yes".Translate(), delegate
                                {
                                    doWork = true;
                                }, "No".Translate()));
                            }
                        }
                        else
                        {
                            doWork = true;
                        }
                    }
                };
                yield return command_Action2;

                //EJECT LOADED MATRIX
                var command_Action3 = new Command_Action
                {
                    defaultLabel = "BEWH.MankindsFinest.GeneGestator.EjectGeneMatrix".Translate(containedMatrix.Label),
                    defaultDesc = "BEWH.MankindsFinest.GeneGestator.EjectGeneMatrixDesc".Translate(containedMatrix.Label),
                    icon = EjectIcon,
                    activateSound = SoundDefOf.Designate_Cancel,
                    action = delegate
                    {
                        GenSpawn.Spawn(containedMatrix.def, InteractionCell, Map);
                        Reset();
                    }
                };
                yield return command_Action3;
            }
            else
            {
                //SELECTS MATRIX TO LOAD AND START HAUL JOB
                if (selectedMatrix == null)
                {
                    var command_Action4 = new Command_Action
                    {

                        defaultLabel = "BEWH.MankindsFinest.GeneGestator.SelectMatrix".Translate() + "...",
                        defaultDesc = "BEWH.MankindsFinest.GeneGestator.SelectMatrixDesc".Translate(), 
                        icon = selectedMatrix == null ? MatrixSelectionTex : selectedMatrix.uiIcon
                    };

                    var gestatablesAvailable = new List<ThingDef>();
                    gestatablesAvailable.AddRange(AvailableGestatables());
                    command_Action4.action = delegate
                    {
                        var list = new List<FloatMenuOption>();
                        foreach (var matrix in gestatablesAvailable)
                        {
                            var text = matrix.label;
                            var floatMenu = new FloatMenuOption(text, delegate
                            {
                                selectedMatrix = matrix;
                                if (containedMatrix == null)
                                { 
                                    return;
                                }
                                
                                GenPlace.TryPlaceThing(containedMatrix, InteractionCell, Map, ThingPlaceMode.Direct);
                                containedMatrix = null;
                            });

                            var doesNotHaveMatrix = Map.listerThings.ThingsOfDef(matrix).NullOrEmpty();
                            
                            if (doesNotHaveMatrix)
                            {
                                floatMenu.Disabled = true;
                            }

                            list.Add(floatMenu);
                        }
                        if (!list.Any())
                        {
                            list.Add(new FloatMenuOption("BEWH.MankindsFinest.GeneGestator.NoAvailableMaterial".Translate(), null));
                        } 
                        Find.WindowStack.Add(new FloatMenu(list));
                    };
                    if (!PowerOn)
                    {
                        command_Action4.Disable("NoPower".Translate().CapitalizeFirst());
                    }
                    if (gestatablesAvailable.NullOrEmpty()) 
                    {
                        command_Action4.Disable("BEWH.MankindsFinest.GeneGestator.MissingGestateResearch".Translate().CapitalizeFirst());
                    }
                    yield return command_Action4;
                }
                //STOPS HAUL JOB
                else
                {
                    var command_Action3 = new Command_Action
                    {
                        defaultLabel = "CommandCancelLoad".Translate(),
                        defaultDesc = "CommandCancelLoadDesc".Translate(),
                        icon = CancelIcon,
                        activateSound = SoundDefOf.Designate_Cancel,
                        action = delegate
                        {
                            selectedMatrix = null;
                        }
                    };
                    yield return command_Action3;
                }
            }
        }

        if (!DebugSettings.ShowDevGizmos)
        {
            yield break;
        }
        
        if (doWork)
        {
            //DEV: ALMOST FINISH
            var command_Action1 = new Command_Action
            {
                defaultLabel = "DEV: ALMOST FINISH",
                icon = CancelIcon,
                activateSound = SoundDefOf.Designate_Cancel,
                action = delegate
                {
                    progressInt = totalTime - 250;
                }
            };
            yield return command_Action1;
        }
    }

    private bool CanUseAnyMaterial()
    {
        return containedMatrix.def.GetModExtension<DefModExtension_GeneMatrix>().canUsePrimarchMaterial ||
                containedMatrix.def.GetModExtension<DefModExtension_GeneMatrix>().canUseChapterMaterial;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref totalTime, "totalTime", 0f);
        Scribe_Values.Look(ref progressInt, "progress", 0f);
        Scribe_Values.Look(ref doWork, "doWork");
        Scribe_Defs.Look(ref selectedMatrix, "selectedMatrix");
        Scribe_Defs.Look(ref selectedMaterial, "selectedMaterial");
        Scribe_Deep.Look(ref containedMatrix, "containedMatrix");
    }
}
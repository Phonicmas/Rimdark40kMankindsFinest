using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Genes40k;

[StaticConstructorOnStartup]
public class Building_PrimarchGrowthVat : Building_Enterable, IStoreSettingsParent, IThingHolderWithDrawnPawn
{
    public PrimarchEmbryo selectedEmbryo;

    public PrimarchEmbryo containedEmbryo;

    public bool haulJobStarted = false;

    public bool hasBeenStarted = false;

    public Pawn jobDoer = null;

    private float embryoStarvation;

    private float containedNutrition;

    private StorageSettings allowedNutritionSettings;

    [Unsaved(false)]
    private CompPowerTrader cachedPowerComp;

    [Unsaved(false)]
    private Graphic fetusEarlyStageGraphic;

    [Unsaved(false)]
    private Graphic fetusLateStageGraphic;

    [Unsaved(false)]
    private Sustainer sustainerWorking;

    private static readonly Texture2D CancelIcon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel");

    private static readonly Texture2D StartIcon = ContentFinder<Texture2D>.Get("UI/Gizmos/BEWH_PrimarchVatStart");
        
    private static readonly Texture2D EjectEmbryoIcon = ContentFinder<Texture2D>.Get("UI/Gizmos/BEWH_EjectPrimarchEmbryo");

    private static readonly CachedTexture InsertEmbryoIcon = new CachedTexture("UI/Gizmos/BEWH_InsertPrimarchEmbryo");
        
    private Mote workingMote;

    private const float BaseEmbryoConsumedNutritionPerDay = 6f;

    private const float NutritionBuffer = 10f;

    private const int EmbryoGestationTicks = 600000;
        
    private const int EmbryoLateStageGraphicTicksRemaining = 300000;

    private const float FetusMinSize = 0.4f;

    private const float FetusMaxSize = 0.95f;
        
    private float EmbryoGestationPct => 1f - Mathf.Clamp01((float)EmbryoGestationTicksRemaining / EmbryoGestationTicks);

    public bool StorageTabVisible => true;

    public float HeldPawnDrawPos_Y => DrawPos.y + 1f / 26f;

    public float HeldPawnBodyAngle => Rotation.AsAngle;

    public PawnPosture HeldPawnPosture => PawnPosture.LayingOnGroundFaceUp;

    public bool PowerOn => PowerTraderComp.PowerOn;

    public override Vector3 PawnDrawOffset => CompBiosculpterPod.FloatingOffset(Find.TickManager.TicksGame);

    private CompPowerTrader PowerTraderComp => cachedPowerComp ??= this.TryGetComp<CompPowerTrader>();

    private float BiostarvationDailyOffset
    {
        get
        {
            if (!Working || !hasBeenStarted)
            {
                return 0f;
            }
            if (!PowerOn || containedNutrition <= 0f)
            {
                return 0.5f;
            }
            return -0.1f;
        }
    }

    private float BiostarvationSeverityPercent => selectedEmbryo != null ? embryoStarvation : 0f;

    private float NutritionConsumedPerDay
    {
        get
        {
            var num = selectedEmbryo != null ? BaseEmbryoConsumedNutritionPerDay : 3f;

            if (!(BiostarvationSeverityPercent > 0f))
            {
                return num;
            }
                
            var num2 = 1.1f;
            num *= num2;
            return num;
        }
    }

    private float NutritionStored => containedNutrition + innerContainer.Sum(thing => thing.stackCount * thing.GetStatValue(StatDefOf.Nutrition));

    public float NutritionNeeded => selectedEmbryo == null ? 0f : NutritionBuffer - NutritionStored;

    private int EmbryoGestationTicksRemaining => startTick - Find.TickManager.TicksGame;

    private Graphic cylinderGraphic;

    private Graphic topGraphic;

    private Graphic FetusEarlyStage =>
        fetusEarlyStageGraphic ??= GraphicDatabase.Get<Graphic_Single>(DefModTexture.earlyFetusTexture, ShaderDatabase.Cutout,
            DefModTexture.earlyFetusSize, Color.white);

    private Graphic FetusLateStage =>
        fetusLateStageGraphic ??= GraphicDatabase.Get<Graphic_Single>(DefModTexture.lateFetusTexture, ShaderDatabase.Cutout,
            DefModTexture.lateFetusSize, Color.white);

    private DefModExtension_PrimarchVatTexture DefModTexture => def.GetModExtension<DefModExtension_PrimarchVatTexture>();
        
    public override void PostMake()
    {
        base.PostMake();
        allowedNutritionSettings = new StorageSettings(this);
        if (def.building.defaultStorageSettings != null)
        {
            allowedNutritionSettings.CopyFrom(def.building.defaultStorageSettings);
        }
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        if (respawningAfterLoad && selectedEmbryo != null && containedEmbryo != null)
        {
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                var color = EmbryoColor();
                fetusEarlyStageGraphic = FetusEarlyStage.GetColoredVersion(ShaderDatabase.Cutout, color, color);
                fetusLateStageGraphic = FetusLateStage.GetColoredVersion(ShaderDatabase.Cutout, color, color);
            });
        }
    }

    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        sustainerWorking = null;
        DestroyEmbryo();
        base.DeSpawn(mode);
    }

    public override void Tick()
    {
        base.Tick();
        innerContainer.ThingOwnerTick();
        if (this.IsHashIntervalTick(250))
        {
            PowerTraderComp.PowerOutput = Working ? 0f - PowerComp.Props.PowerConsumption : 0f - PowerComp.Props.idlePowerDraw;
        }

        ThingDef thingDef = null;
            
        if (hasBeenStarted && Working)
        {
            thingDef = def.building.gestatorFormingMote.GetForRotation(Rotation);
            if (selectedEmbryo != null)
            {
                if (EmbryoGestationTicksRemaining <= 0)
                {
                    Finish();
                    return;
                }
                embryoStarvation = Mathf.Clamp01(embryoStarvation + BiostarvationDailyOffset / 60000f);
            }
            if (BiostarvationSeverityPercent >= 1f)
            {
                Fail();
                return;
            }
            if (sustainerWorking == null || sustainerWorking.Ended)
            {
                sustainerWorking = SoundDefOf.GrowthVat_Working.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));
            }
            else
            {
                sustainerWorking.Maintain();
            }
            containedNutrition = Mathf.Clamp(containedNutrition - NutritionConsumedPerDay / 60000f, 0f, 2.14748365E+09f);
            if (containedNutrition <= 0f)
            {
                TryAbsorbNutritiousThing();
            }
        }
        else
        {
            TryGrowEmbryo();
        }

        if (thingDef == null)
        {
            return;
        }
            
        if (workingMote == null || workingMote.Destroyed || workingMote.def != thingDef)
        {
            workingMote = MoteMaker.MakeAttachedOverlay(this, thingDef, Vector3.zero);
        }
            
        workingMote.yOffset = -4.9f;
        workingMote.Maintain();
    }

    public override AcceptanceReport CanAcceptPawn(Pawn pawn)
    {
        return false;
    }

    public override void TryAcceptPawn(Pawn pawn)
    {
    }

    private void TryGrowEmbryo()
    {
        if (Working || !PowerOn || selectedEmbryo == null || containedEmbryo == null)
        {
            return;
        }
            
        SoundDefOf.GrowthVat_Close.PlayOneShot(SoundInfo.InMap(this));
        startTick = Find.TickManager.TicksGame + EmbryoGestationTicks;
        LongEventHandler.ExecuteWhenFinished(delegate
        {
            var color = EmbryoColor();
            fetusEarlyStageGraphic = FetusEarlyStage.GetColoredVersion(ShaderDatabase.Cutout, color, color);
            fetusLateStageGraphic = FetusLateStage.GetColoredVersion(ShaderDatabase.Cutout, color, color);
        });
    }

    private void TryAbsorbNutritiousThing()
    {
        foreach (var thing in innerContainer)
        {
            if (thing.def != Genes40kDefOf.BEWH_RawGestationalSlurry)
            {
                continue;
            }
            var statValue = thing.GetStatValue(StatDefOf.Nutrition);
            if (!(statValue > 0f))
            {
                continue;
            }
                    
            containedNutrition += statValue;
            thing.SplitOff(1).DeSpawn();
                
            break;
        }
    }

    private void Finish()
    {
        FinishEmbryo();
    }

    private void FinishEmbryo()
    {
        EmbryoBirth();
        DestroyEmbryo();
        OnStop();
    }

    private void Fail()
    {
        DestroyEmbryo(biostarvation: true);
        OnStop();
    }

    private void OnStop()
    {
        selectedEmbryo = null;
        containedEmbryo = null;
        startTick = -1;
        embryoStarvation = 0f;
        sustainerWorking = null;
        hasBeenStarted = false;
        haulJobStarted = false;
        jobDoer = null;
    }

    private void DestroyEmbryo(bool biostarvation = false)
    {
        if (startTick < 0 || selectedEmbryo == null || containedEmbryo == null)
        {
            return;
        }
            
        if (startTick > Find.TickManager.TicksGame)
        {
            Messages.Message(
                biostarvation
                    ? "EmbryoEjectedFromGrowthVatBiostarvation".Translate(selectedEmbryo.Label)
                    : "EmbryoEjectedFromGrowthVat".Translate(selectedEmbryo.Label), this,
                MessageTypeDefOf.NegativeEvent);
        }

        if (!selectedEmbryo.Destroyed)
        {
            selectedEmbryo.Destroy();
        }
        selectedEmbryo = null;
            
        if (!containedEmbryo.Destroyed)
        {
            containedEmbryo.Destroy();
        }
        containedEmbryo = null;
    }

    private void EmbryoBirth()
    {
        if (selectedEmbryo == null || containedEmbryo == null || startTick > Find.TickManager.TicksGame)
        {
            return;
        }

        var geneDef = selectedEmbryo.primarchGenes.GenesListForReading.FirstOrDefault(g => g.HasModExtension<DefModExtension_PrimarchVatExtras>());
        var childAmount = geneDef == null ? 1 : geneDef.GetModExtension<DefModExtension_PrimarchVatExtras>().childAmount;
            
        var children = new List<Pawn>();

        var ritual = Faction.OfPlayer.ideos.PrimaryIdeo.GetPrecept(PreceptDefOf.ChildBirth) as Precept_Ritual;
        for (var i = 0; i < childAmount; i++)
        {
            var thing = PregnancyUtility.ApplyBirthOutcome(((RitualOutcomeEffectWorker_ChildBirth)RitualOutcomeEffectDefOf.ChildBirth.GetInstance()).GetOutcome(100f, null), 100f, ritual, selectedEmbryo.birthGenes.GenesListForReading, selectedEmbryo.mother, this, selectedEmbryo.father);
            var pawn2 = (Pawn)thing;
            foreach (var gene in selectedEmbryo.primarchGenes.GenesListForReading)
            {
                pawn2.genes.AddGene(gene, true);
            }
            pawn2.genes.SetXenotypeDirect(Genes40kDefOf.BEWH_Primarch);
            children.Add(pawn2);
            if (thing == null || !(embryoStarvation > 0f))
            {
                continue;
            }
            
            var pawn = thing is Corpse corpse ? corpse.InnerPawn : (Pawn)thing;
            var hediff = HediffMaker.MakeHediff(HediffDefOf.BioStarvation, pawn);
            hediff.Severity = Mathf.Lerp(0f, HediffDefOf.BioStarvation.maxSeverity, embryoStarvation);
            pawn.health.AddHediff(hediff);
        }

        Pawn firstTwin = null;
            
        foreach (var pawn3 in children.Where(pawn3 => pawn3.genes.HasActiveGene(Genes40kDefOf.BEWH_PrimarchSpecificGeneXX)))
        {
            if (firstTwin == null)
            {
                firstTwin = pawn3;
            }
            else
            {
                ((Gene_TwinConnected)firstTwin.genes.GetGene(Genes40kDefOf.BEWH_PrimarchSpecificGeneXX)).SetTwin(pawn3);
                ((Gene_TwinConnected)pawn3.genes.GetGene(Genes40kDefOf.BEWH_PrimarchSpecificGeneXX)).SetTwin(firstTwin);
                    
                firstTwin.relations.AddDirectRelation(PawnRelationDefOf.Sibling, pawn3);
                pawn3.relations.AddDirectRelation(PawnRelationDefOf.Sibling, firstTwin);
                    
                firstTwin = null;
            }
        }
    }

    public bool CanAcceptNutrition(Thing thing)
    {
        return allowedNutritionSettings.AllowedToAccept(thing);
    }

    public StorageSettings GetStoreSettings()
    {
        return allowedNutritionSettings;
    }

    public StorageSettings GetParentStoreSettings()
    {
        return def.building.fixedStorageSettings;
    }

    public void Notify_SettingsChanged()
    {
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }
        foreach (var item in StorageSettingsClipboard.CopyPasteGizmosFor(allowedNutritionSettings))
        {
            yield return item;
        }
            
        if (!hasBeenStarted)
        {
            //STARTS MACHINE
            var command_Action1 = new Command_Action
            {
                defaultLabel = "BEWH.MankindsFinest.PrimarchGrowthVat.StartPrimarchGrowth".Translate(),
                defaultDesc = "BEWH.MankindsFinest.PrimarchGrowthVat.StartPrimarchGrowthDesc".Translate(),
                icon = StartIcon,
                activateSound = SoundDefOf.Designate_Cancel,
                action = delegate
                {
                    var window = Dialog_MessageBox.CreateConfirmation("BEWH.MankindsFinest.PrimarchGrowthVat.PrimarchVatStartConfirmation".Translate(), Action, destructive: true);
                    Find.WindowStack.Add(window);
                    return;

                    void Action()
                    {
                        hasBeenStarted = true;
                    }
                }
            };
            yield return command_Action1;
            if (containedEmbryo == null)
            {
                command_Action1.Disable("BEWH.MankindsFinest.PrimarchGrowthVat.ContainsNoEmbryo".Translate().CapitalizeFirst());
            }
            else if (!PowerOn)
            {
                command_Action1.Disable("NoPower".Translate().CapitalizeFirst());
            }

            if (containedEmbryo != null)
            {
                //EJECT PRIMARCH EMBRYO
                var command_Action2 = new Command_Action
                {
                    defaultLabel = "BEWH.MankindsFinest.PrimarchGrowthVat.EjectPrimarchEmbryo".Translate(),
                    defaultDesc = "BEWH.MankindsFinest.PrimarchGrowthVat.EjectPrimarchEmbryoDesc".Translate(),
                    icon = EjectEmbryoIcon,
                    activateSound = SoundDefOf.Designate_Cancel,
                    action = delegate
                    {
                        GenPlace.TryPlaceThing(containedEmbryo, InteractionCell, Map, ThingPlaceMode.Direct);
                        containedEmbryo = null;
                            
                        OnStop();
                    }
                };
                yield return command_Action2;
            }
        }
            
        if(containedEmbryo == null)
        {
            //START HAUL JOB
            if (!haulJobStarted)
            {
                var embryos = AvailableEmbryo();
                var command_Action3 = new Command_Action
                {
                    defaultLabel = "ImplantEmbryo".Translate() + "...",
                    defaultDesc = "InsertEmbryoGrowthVatDesc".Translate(EmbryoGestationTicks.ToStringTicksToPeriod()).Resolve(),
                    icon = InsertEmbryoIcon.Texture,
                    action = delegate
                    {
                        var list = new List<FloatMenuOption>();
                        foreach (var embryo in embryos)
                        {
                            var embryoName = "BEWH.MankindsFinest.PrimarchGrowthVat.PrimarchMother".Translate(embryo.mother.Name.ToStringFull);
                            var primarchChapterGenes = embryo.primarchGenes.GenesListForReading.Where(gene => gene.HasModExtension<DefModExtension_PrimarchMaterial>()).ToList();
                            if (primarchChapterGenes.Any())
                            {
                                embryoName += "\n";
                                embryoName += "BEWH.MankindsFinest.PrimarchGrowthVat.PrimarchFather".Translate(primarchChapterGenes.First().label);
                            }
                            list.Add(new FloatMenuOption(embryoName, delegate
                            {
                                SelectEmbryo(embryo);
                                haulJobStarted = true;
                            }, embryo, Color.white));
                        }
                        Find.WindowStack.Add(new FloatMenu(list));
                    }
                };
                if (embryos.NullOrEmpty())
                {
                    command_Action3.Disable("ImplantNoEmbryos".Translate().CapitalizeFirst());
                }
                else if (!PowerOn)
                {
                    command_Action3.Disable("NoPower".Translate().CapitalizeFirst());
                }
                yield return command_Action3;
            }
            else //CANCEL HAUL JOB
            {
                var command_Action4 = new Command_Action
                {
                    defaultLabel = "CommandCancelLoad".Translate(),
                    defaultDesc = "CommandCancelLoadDesc".Translate(),
                    icon = CancelIcon,
                    activateSound = SoundDefOf.Designate_Cancel,
                    action = delegate
                    {
                        if (jobDoer != null)
                        {
                            foreach (var job in jobDoer.jobs.AllJobs())
                            {
                                if (job.def != Genes40kDefOf.BEWH_FillGeneGestator)
                                {
                                    continue;
                                }
                                
                                jobDoer.jobs.EndCurrentOrQueuedJob(job, JobCondition.InterruptForced);
                                break;
                            }
                        }
                        OnStop();
                    }
                };
                yield return command_Action4;
            }
        }
        else
        {
            yield return new Command_Action
            {
                defaultLabel = "TEST INSPECT",
                action = delegate
                {
                    Genes40kUtils.InspectPrimarchEmbryoGenes(containedEmbryo);
                }
            };
        }

        if (!DebugSettings.ShowDevGizmos)
        {
            yield break;
        }
            
        //DEV: FILL NUTRITION
        yield return new Command_Action
        {
            defaultLabel = "DEV: Fill nutrition",
            action = delegate
            {
                containedNutrition = NutritionBuffer;
            }
        };
            
        //DEV: EMPTY NUTRITION
        yield return new Command_Action
        {
            defaultLabel = "DEV: Empty nutrition",
            action = delegate
            {
                containedNutrition = 0f;
            }
        };

        if (!Working || !hasBeenStarted)
        {
            yield break;
        }

        //DEV: INSTA BIRTH
        yield return new Command_Action
        {
            defaultLabel = "DEV: Embryo birth now",
            action = delegate
            {
                startTick = Find.TickManager.TicksGame;
                Finish();
            }
        };
                        
        //DEV: ALMOST INSTA BIRTH
        yield return new Command_Action
        {
            defaultLabel = "DEV: Embryo almost done",
            action = delegate
            {
                startTick = Find.TickManager.TicksGame + 2500;
            }
        };
            
        //DEV: DECREASE TIME REMAINING BY 12H
        yield return new Command_Action
        {
            defaultLabel = "DEV: decrease time by 12h",
            action = delegate
            {
                startTick -= 30000;
            }
        };
    }

    private List<PrimarchEmbryo> AvailableEmbryo()
    {
        var embryos = new List<Thing>();
        embryos.AddRange(Map.listerThings.ThingsOfDef(Genes40kDefOf.BEWH_PrimarchEmbryo));

        return embryos.Cast<PrimarchEmbryo>().ToList();
    }

    private void SelectEmbryo(PrimarchEmbryo embryo)
    {
        selectedEmbryo = embryo;
    }

    public void AddPrimarchEmbryo(Thing primarchEmbryo, Pawn jobdoer)
    {
        if (primarchEmbryo.stackCount > 1)
        {
            containedEmbryo = (PrimarchEmbryo)primarchEmbryo.SplitOff(1);
        }
        else
        {
            containedEmbryo = (PrimarchEmbryo)primarchEmbryo;
        }

        if (selectedEmbryo == null || selectedEmbryo != containedEmbryo)
        {
            selectedEmbryo = containedEmbryo;
        }
            
        if (jobdoer.IsCarryingThing(primarchEmbryo))
        {
            jobdoer.carryTracker.TryDropCarriedThing(jobdoer.Position, ThingPlaceMode.Direct, out _);
        }
            
        primarchEmbryo.DeSpawn();
        haulJobStarted = false;
    }
        
    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        base.DrawAt(drawLoc, flip);
        if (Working && hasBeenStarted && selectedEmbryo != null && containedEmbryo != null)
        {
            var loc = drawLoc + def.building.formingMechPerRotationOffset[Rotation.AsInt];
            loc.y += 1f / 52f;
            loc.z += Mathf.PingPong(Find.TickManager.TicksGame * def.building.formingMechBobSpeed, def.building.formingMechYBobDistance);
                
            if (EmbryoGestationTicksRemaining > EmbryoLateStageGraphicTicksRemaining)
            {
                FetusEarlyStage.drawSize = DefModTexture.earlyFetusSize * Mathf.Lerp(FetusMinSize, FetusMaxSize, EmbryoGestationPct);
                loc += DefModTexture.earlyFetusOffset;
                FetusEarlyStage.DrawFromDef(loc, Rot4.North, null);
            }
            else
            {
                FetusLateStage.drawSize = DefModTexture.lateFetusSize * Mathf.Lerp(FetusMinSize, FetusMaxSize, EmbryoGestationPct);
                loc += DefModTexture.lateFetusOffset;
                FetusLateStage.DrawFromDef(loc, Rot4.North, null);
            }
        }

        if (topGraphic == null)
        {
            topGraphic = def.building.mechGestatorTopGraphic.GraphicColoredFor(this);
        }
        if (cylinderGraphic == null)
        {
            cylinderGraphic = def.building.mechGestatorCylinderGraphic.GraphicColoredFor(this);
        }
            
        var loc2 = new Vector3(drawLoc.x, AltitudeLayer.BuildingBelowTop.AltitudeFor(), drawLoc.z);
        cylinderGraphic.Draw(loc2, Rotation, this);
            
        var loc3 = new Vector3(drawLoc.x, AltitudeLayer.BuildingOnTop.AltitudeFor(), drawLoc.z);
        topGraphic.Draw(loc3, Rotation, this);
    }

    private Color EmbryoColor()
    {
        var result = PawnSkinColors.GetSkinColor(0.5f);
        if (selectedEmbryo?.GeneSet == null) return result;
            
        foreach (var item in selectedEmbryo.GeneSet.GenesListForReading)
        {
            if (item.skinColorOverride.HasValue)
            {
                return item.skinColorOverride.Value;
            }
            if (item.skinColorBase.HasValue)
            {
                result = item.skinColorBase.Value;
            }
        }
        return result;
    }

    public override void DrawExtraSelectionOverlays()
    {
        base.DrawExtraSelectionOverlays();
        if (selectedEmbryo != null && selectedEmbryo.Map == Map)
        {
            GenDraw.DrawLineBetween(this.TrueCenter(), selectedEmbryo.TrueCenter());
        }
    }

    public override string GetInspectString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(base.GetInspectString());
        if (Working && hasBeenStarted)
        {
            if (selectedEmbryo != null && containedEmbryo != null)
            {
                stringBuilder.Append("\n");
                if (EmbryoGestationTicksRemaining > 60000)
                {
                    stringBuilder.AppendTagged("EmbryoTimeUntilBirth".Translate() + ": " + EmbryoGestationTicksRemaining.ToStringTicksToDays().Colorize(ColoredText.DateTimeColor));
                }
                else
                {
                    stringBuilder.AppendTagged("EmbryoTimeUntilBirth".Translate() + ": " + EmbryoGestationTicksRemaining.ToStringTicksToPeriod(allowYears: false).Colorize(ColoredText.DateTimeColor));
                }
            }
                
            var biostarvationSeverityPercent = BiostarvationSeverityPercent;
            if (biostarvationSeverityPercent > 0f)
            {
                var text = BiostarvationDailyOffset >= 0f ? "+" : string.Empty;
                stringBuilder.Append("\n");
                stringBuilder.Append(string.Format("{0}: {1} ({2})", "Biostarvation".Translate(), biostarvationSeverityPercent.ToStringPercent(), "PerDay".Translate(text + BiostarvationDailyOffset.ToStringPercent())));
            }
        }
            
        stringBuilder.Append("\n");
        stringBuilder.Append("Nutrition".Translate()).Append(": ").Append(NutritionStored.ToStringByStyle(ToStringStyle.FloatMaxOne));
        if (Working && hasBeenStarted)
        {
            stringBuilder.Append(" (-").Append("PerDay".Translate(NutritionConsumedPerDay.ToString("F1"))).Append(")");
        }
            
        return stringBuilder.ToString();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Deep.Look(ref selectedEmbryo, "selectedEmbryo");
        Scribe_References.Look(ref containedEmbryo, "containedEmbryo");
        Scribe_Values.Look(ref embryoStarvation, "embryoStarvation", 0f);
        Scribe_Values.Look(ref containedNutrition, "containedNutrition", 0f);
        Scribe_Values.Look(ref haulJobStarted, "haulJobStarted", false);
        Scribe_Values.Look(ref hasBeenStarted, "hasBeenStarted", false);
        Scribe_Deep.Look(ref allowedNutritionSettings, "allowedNutritionSettings", this);
        Scribe_References.Look(ref jobDoer, "jobDoer");
            
        if (allowedNutritionSettings != null)
        {
            return;
        }
            
        allowedNutritionSettings = new StorageSettings(this);
            
        if (def.building.defaultStorageSettings != null)
        {
            allowedNutritionSettings.CopyFrom(def.building.defaultStorageSettings);
        }
    }
}
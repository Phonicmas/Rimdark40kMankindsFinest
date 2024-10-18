using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Noise;
using Verse.Sound;

namespace Genes40k
{
    [StaticConstructorOnStartup]
    public class Building_PrimarchGrowthVat : Building_Enterable, IStoreSettingsParent, IThingHolderWithDrawnPawn, IThingHolder
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
        private Graphic cachedTopGraphic;

        [Unsaved(false)]
        private Graphic fetusEarlyStageGraphic;

        [Unsaved(false)]
        private Graphic fetusLateStageGraphic;

        [Unsaved(false)]
        private Sustainer sustainerWorking;

        [Unsaved(false)]
        private Effecter bubbleEffecter;

        private static readonly Texture2D CancelIcon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel");

        private static readonly Texture2D StartIcon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel");

        public static readonly CachedTexture InsertPawnIcon = new CachedTexture("UI/Gizmos/InsertPawn");

        public static readonly CachedTexture InsertEmbryoIcon = new CachedTexture("UI/Gizmos/InsertEmbryo");

        private const float BaseEmbryoConsumedNutritionPerDay = 6f;

        public const float NutritionBuffer = 10f;

        public const int AgeTicksPerTickInGrowthVat = 20;

        private const float EmbryoBirthQuality = 0.7f;

        public const int EmbryoGestationTicks = 600000;

        private const int EmbryoLateStageGraphicTicksRemaining = 600000;

        private const float FetusMinSize = 0.4f;

        private const float FetusMaxSize = 0.95f;

        private const int GlowIntervalTicks = 132;

        private static Dictionary<Rot4, ThingDef> GlowMotePerRotation;

        private static Dictionary<Rot4, EffecterDef> BubbleEffecterPerRotation;

        public bool StorageTabVisible => true;

        public float HeldPawnDrawPos_Y => DrawPos.y + 1f / 26f;

        public float HeldPawnBodyAngle => base.Rotation.AsAngle;

        public PawnPosture HeldPawnPosture => PawnPosture.LayingOnGroundFaceUp;

        public bool PowerOn => PowerTraderComp.PowerOn;

        public override Vector3 PawnDrawOffset => CompBiosculpterPod.FloatingOffset(Find.TickManager.TicksGame);

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

        public float BiostarvationDailyOffset
        {
            get
            {
                if (!base.Working)
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

        private float BiostarvationSeverityPercent
        {
            get
            {
                if (selectedEmbryo != null)
                {
                    return embryoStarvation;
                }
                return 0f;
            }
        }

        public float NutritionConsumedPerDay
        {
            get
            {
                float num = ((selectedEmbryo != null) ? BaseEmbryoConsumedNutritionPerDay : 3f);
                if (BiostarvationSeverityPercent > 0f)
                {
                    float num2 = 1.1f;
                    num *= num2;
                }
                return num;
            }
        }

        public float NutritionStored
        {
            get
            {
                float num = containedNutrition;
                for (int i = 0; i < innerContainer.Count; i++)
                {
                    Thing thing = innerContainer[i];
                    num += (float)thing.stackCount * thing.GetStatValue(StatDefOf.Nutrition);
                }
                return num;
            }
        }

        public float NutritionNeeded
        {
            get
            {
                if (selectedEmbryo == null)
                {
                    return 0f;
                }
                return 10f - NutritionStored;
            }
        }

        public int EmbryoGestationTicksRemaining => startTick - Find.TickManager.TicksGame;

        public float EmbryoGestationPct => 1f - Mathf.Clamp01((float)EmbryoGestationTicksRemaining / EmbryoGestationTicks);

        private Graphic TopGraphic
        {
            get
            {
                if (cachedTopGraphic == null)
                {
                    cachedTopGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Building/Misc/GrowthVat/GrowthVatTop", ShaderDatabase.Transparent, def.graphicData.drawSize, Color.white);
                }
                return cachedTopGraphic;
            }
        }

        private Graphic FetusEarlyStage
        {
            get
            {
                if (fetusEarlyStageGraphic == null)
                {
                    fetusEarlyStageGraphic = GraphicDatabase.Get<Graphic_Single>("Other/VatGrownFetus_EarlyStage", ShaderDatabase.Cutout, Vector2.one, Color.white);
                }
                return fetusEarlyStageGraphic;
            }
        }

        private Graphic FetusLateStage
        {
            get
            {
                if (fetusLateStageGraphic == null)
                {
                    fetusLateStageGraphic = GraphicDatabase.Get<Graphic_Single>("Other/VatGrownFetus_LateStage", ShaderDatabase.Cutout, Vector2.one, Color.white);
                }
                return fetusLateStageGraphic;
            }
        }

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
            if (respawningAfterLoad && selectedEmbryo != null && innerContainer.Contains(selectedEmbryo))
            {
                LongEventHandler.ExecuteWhenFinished(delegate
                {
                    Color color = EmbryoColor();
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
                PowerTraderComp.PowerOutput = (base.Working ? (0f - base.PowerComp.Props.PowerConsumption) : (0f - base.PowerComp.Props.idlePowerDraw));
            }
            Pawn pawn = selectedPawn;
            if (pawn == null || !pawn.Destroyed)
            {
                PrimarchEmbryo humanEmbryo = selectedEmbryo;
                if (humanEmbryo == null || !humanEmbryo.Destroyed)
                {
                    goto IL_0084;
                }
            }
            OnStop();
            goto IL_0084;
        IL_0084:
            foreach (Thing item in (IEnumerable<Thing>)innerContainer)
            {
                if (item is PrimarchEmbryo humanEmbryo2 && humanEmbryo2 != selectedEmbryo)
                {
                    innerContainer.TryDrop(item, InteractionCell, base.Map, ThingPlaceMode.Near, 1, out var _);
                }
            }
            if (hasBeenStarted && base.Working)
            {
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
                if (GlowMotePerRotation == null)
                {
                    GlowMotePerRotation = new Dictionary<Rot4, ThingDef>
                {
                    {
                        Rot4.South,
                        ThingDefOf.Mote_VatGlowVertical
                    },
                    {
                        Rot4.East,
                        ThingDefOf.Mote_VatGlowHorizontal
                    },
                    {
                        Rot4.West,
                        ThingDefOf.Mote_VatGlowHorizontal
                    },
                    {
                        Rot4.North,
                        ThingDefOf.Mote_VatGlowVertical
                    }
                };
                    BubbleEffecterPerRotation = new Dictionary<Rot4, EffecterDef>
                {
                    {
                        Rot4.South,
                        EffecterDefOf.Vat_Bubbles_South
                    },
                    {
                        Rot4.East,
                        EffecterDefOf.Vat_Bubbles_East
                    },
                    {
                        Rot4.West,
                        EffecterDefOf.Vat_Bubbles_West
                    },
                    {
                        Rot4.North,
                        EffecterDefOf.Vat_Bubbles_North
                    }
                };
                }
                if (this.IsHashIntervalTick(GlowIntervalTicks))
                {
                    MoteMaker.MakeStaticMote(DrawPos + OffsetFromRotation(base.Rotation), base.MapHeld, GlowMotePerRotation[base.Rotation]);
                }
                if (bubbleEffecter == null)
                {
                    bubbleEffecter = BubbleEffecterPerRotation[base.Rotation].SpawnAttached(this, base.MapHeld);
                }
                bubbleEffecter.EffectTick(this, this);
            }
            else
            {
                TryGrowEmbryo();
                bubbleEffecter?.Cleanup();
                bubbleEffecter = null;
            }
        }

        public override AcceptanceReport CanAcceptPawn(Pawn pawn)
        {
            return false;
        }

        public override void TryAcceptPawn(Pawn pawn)
        {
            return;
        }

        private void TryGrowEmbryo()
        {
            if (!base.Working && PowerOn && selectedEmbryo != null && innerContainer.Contains(selectedEmbryo))
            {
                SoundDefOf.GrowthVat_Close.PlayOneShot(SoundInfo.InMap(this));
                startTick = Find.TickManager.TicksGame + EmbryoGestationTicks;
                LongEventHandler.ExecuteWhenFinished(delegate
                {
                    Color color = EmbryoColor();
                    fetusEarlyStageGraphic = FetusEarlyStage.GetColoredVersion(ShaderDatabase.Cutout, color, color);
                    fetusLateStageGraphic = FetusLateStage.GetColoredVersion(ShaderDatabase.Cutout, color, color);
                });
                if (selectedPawn != null)
                {
                    Log.Error("Growing embryo while pawn was somehow marked as selected");
                    selectedPawn = null;
                }
            }
        }

        private void TryAbsorbNutritiousThing()
        {
            for (int i = 0; i < innerContainer.Count; i++)
            {
                if (innerContainer[i].def != ThingDefOf.Xenogerm && innerContainer[i].def != Genes40kDefOf.BEWH_PrimarchEmbryo)
                {
                    float statValue = innerContainer[i].GetStatValue(StatDefOf.Nutrition);
                    if (statValue > 0f)
                    {
                        containedNutrition += statValue;
                        innerContainer[i].SplitOff(1).Destroy();
                        break;
                    }
                }
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
            selectedPawn = null;
            selectedEmbryo = null;
            startTick = -1;
            embryoStarvation = 0f;
            sustainerWorking = null;
            hasBeenStarted = false;
            haulJobStarted = false;
            jobDoer = null;
        }

        private void DestroyEmbryo(bool biostarvation = false)
        {
            if (startTick < 0 || selectedEmbryo == null || !innerContainer.Contains(selectedEmbryo))
            {
                return;
            }
            if (startTick > Find.TickManager.TicksGame)
            {
                if (biostarvation)
                {
                    Messages.Message("EmbryoEjectedFromGrowthVatBiostarvation".Translate(selectedEmbryo.Label), this, MessageTypeDefOf.NegativeEvent);
                }
                else
                {
                    Messages.Message("EmbryoEjectedFromGrowthVat".Translate(selectedEmbryo.Label), this, MessageTypeDefOf.NegativeEvent);
                }
            }
            innerContainer.Remove(selectedEmbryo);
            selectedEmbryo.Destroy();
            selectedEmbryo = null;
        }

        private void EmbryoBirth()
        {
            if (selectedEmbryo != null && innerContainer.Contains(selectedEmbryo) && startTick <= Find.TickManager.TicksGame)
            {
                Precept_Ritual ritual = Faction.OfPlayer.ideos.PrimaryIdeo.GetPrecept(PreceptDefOf.ChildBirth) as Precept_Ritual;
                Thing thing = PregnancyUtility.ApplyBirthOutcome(((RitualOutcomeEffectWorker_ChildBirth)RitualOutcomeEffectDefOf.ChildBirth.GetInstance()).GetOutcome(EmbryoBirthQuality, null), EmbryoBirthQuality, ritual, selectedEmbryo?.GeneSet?.GenesListForReading, selectedEmbryo.mother, this, selectedEmbryo.father);
                Pawn pawn2 = (Pawn)thing;
                foreach (GeneDef gene in selectedEmbryo.primarchGenes.GenesListForReading)
                {
                    pawn2.genes.AddGene(gene, true);
                }
                pawn2.genes.SetXenotypeDirect(selectedEmbryo.xenotype);
                if (thing != null && embryoStarvation > 0f)
                {
                    Pawn pawn = ((thing is Corpse corpse) ? corpse.InnerPawn : ((Pawn)thing));
                    Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.BioStarvation, pawn);
                    hediff.Severity = Mathf.Lerp(0f, HediffDefOf.BioStarvation.maxSeverity, embryoStarvation);
                    pawn.health.AddHediff(hediff);
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
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            foreach (Gizmo item in StorageSettingsClipboard.CopyPasteGizmosFor(allowedNutritionSettings))
            {
                yield return item;
            }
            if (!hasBeenStarted)
            {
                //STARTS MACHINE
                Command_Action command_Action4 = new Command_Action();
                command_Action4.defaultLabel = "BEWH.StartGeneGestating".Translate();
                command_Action4.defaultDesc = "BEWH.StartGeneGestatingDesc".Translate();
                command_Action4.icon = StartIcon;
                command_Action4.activateSound = SoundDefOf.Designate_Cancel;
                command_Action4.action = delegate
                {
                    hasBeenStarted = true;
                };
                yield return command_Action4;
            }
            else if (base.Working)
            {
                /*Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = "CommandCancelGrowth".Translate();
                command_Action.defaultDesc = "CommandCancelGrowthDesc".Translate();
                command_Action.icon = CancelLoadingIcon;
                command_Action.activateSound = SoundDefOf.Designate_Cancel;
                command_Action.action = delegate
                {
                    Action action = delegate
                    {
                        Finish();
                        innerContainer.TryDropAll(InteractionCell, base.Map, ThingPlaceMode.Near);
                    };
                    if (startTick > Find.TickManager.TicksGame && containedEmbryo != null && innerContainer.Contains(containedEmbryo))
                    {
                        Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("ImplantEmbryoCancelVat".Translate(containedEmbryo.Label), action, destructive: true);
                        Find.WindowStack.Add(window);
                    }
                    else
                    {
                        action();
                    }
                };
                yield return command_Action;*/
                if (containedEmbryo != null)
                {
                    //INSPECT GENES
                    yield return new Command_Action
                    {
                        defaultLabel = "InspectGenes".Translate() + "...",
                        defaultDesc = "InspectGenesEmbryoDesc".Translate(),
                        icon = GeneSetHolderBase.GeneticInfoTex.Texture,
                        action = delegate
                        {
                            InspectPaneUtility.OpenTab(typeof(ITab_Genes));
                        }
                    };
                }
                if (DebugSettings.ShowDevGizmos)
                {
                    if (containedEmbryo != null && innerContainer.Contains(containedEmbryo))
                    {
                        yield return new Command_Action
                        {
                            defaultLabel = "DEV: Embryo birth now",
                            action = delegate
                            {
                                startTick = Find.TickManager.TicksGame;
                                Finish();
                            }
                        };
                        yield return new Command_Action
                        {
                            defaultLabel = "DEV: Embryo almost done",
                            action = delegate
                            {
                                startTick = Find.TickManager.TicksGame + 2500;
                            }
                        };
                    }
                }
            }
            if (containedEmbryo == null)
            {
                if (!haulJobStarted)
                {
                    //START HAUL JOB
                    List<PrimarchEmbryo> embryos = AvailableEmbryo();
                    Command_Action command_Action4 = new Command_Action();
                    command_Action4.defaultLabel = "ImplantEmbryo".Translate() + "...";
                    command_Action4.defaultDesc = "InsertEmbryoGrowthVatDesc".Translate(EmbryoGestationTicks.ToStringTicksToPeriod()).Resolve();
                    command_Action4.icon = InsertEmbryoIcon.Texture;
                    command_Action4.action = delegate
                    {
                        List<FloatMenuOption> list = new List<FloatMenuOption>();
                        foreach (PrimarchEmbryo embryo in embryos)
                        {
                            PrimarchEmbryo primarchEmbryo = embryo;
                            var embryoName = "Mother: " + primarchEmbryo.mother.Name.ToStringFull;
                            var primarchChapterGenes = primarchEmbryo.primarchGenes.GenesListForReading.Where(gene => gene.HasModExtension<DefModExtension_PrimarchMaterial>());
                            if (primarchChapterGenes.Any())
                            {
                                embryoName += "\nPrimarch Father: " + primarchChapterGenes.First().label;
                            }
                            list.Add(new FloatMenuOption(embryoName, delegate
                            {
                                SelectEmbryo(embryo);
                                haulJobStarted = true;
                                if (containedEmbryo != null)
                                {
                                    GenPlace.TryPlaceThing(containedEmbryo, InteractionCell, Map, ThingPlaceMode.Direct);
                                    containedEmbryo = null;
                                }
                            }, embryo, Color.white));
                        }
                        Find.WindowStack.Add(new FloatMenu(list));
                    };
                    if (embryos.NullOrEmpty())
                    {
                        command_Action4.Disable("ImplantNoEmbryos".Translate().CapitalizeFirst());
                    }
                    else if (selectedPawn != null)
                    {
                        command_Action4.Disable("PersonSelected".Translate().CapitalizeFirst());
                    }
                    else if (!PowerOn)
                    {
                        command_Action4.Disable("NoPower".Translate().CapitalizeFirst());
                    }
                    yield return command_Action4;
                }
                else
                {
                    //CANCEL HAUL JOB
                    Command_Action command_Action2 = new Command_Action();
                    command_Action2.defaultLabel = "CommandCancelLoad".Translate();
                    command_Action2.defaultDesc = "CommandCancelLoadDesc".Translate();
                    command_Action2.icon = CancelIcon;
                    command_Action2.activateSound = SoundDefOf.Designate_Cancel;
                    command_Action2.action = delegate
                    {
                        haulJobStarted = false;
                        if (jobDoer != null)
                        {
                            foreach (Job job in jobDoer.jobs.AllJobs())
                            {
                                if (job.def == Genes40kDefOf.BEWH_FillGeneGestator)
                                {
                                    jobDoer.jobs.EndCurrentOrQueuedJob(job, JobCondition.InterruptForced);
                                    haulJobStarted = false;
                                    hasBeenStarted = false;
                                    jobDoer = null;
                                    break;
                                }
                            }
                        }
                        /*DestroyEmbryo();
                        innerContainer.TryDropAll(InteractionCell, base.Map, ThingPlaceMode.Near);*/
                        OnStop();
                    };
                    yield return command_Action2;
                }
            }
            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEV: Fill nutrition",
                    action = delegate
                    {
                        containedNutrition = 10f;
                    }
                };
                yield return new Command_Action
                {
                    defaultLabel = "DEV: Empty nutrition",
                    action = delegate
                    {
                        containedNutrition = 0f;
                    }
                };
            }
        }

        private List<PrimarchEmbryo> AvailableEmbryo()
        {
            List<PrimarchEmbryo> availableEmbryos = new List<PrimarchEmbryo>();

            List<Thing> embryos = new List<Thing>();
            embryos.AddRange(Map.listerThings.ThingsOfDef(Genes40kDefOf.BEWH_PrimarchEmbryo));

            foreach (Building_GeneStorageGraphicProgression building in Map.listerBuildings.AllBuildingsColonistOfDef(Genes40kDefOf.BEWH_PrimarchEmbryoContainer).Cast<Building_GeneStorageGraphicProgression>())
            {
                foreach (var item in building.GeneAmount)
                {
                    embryos.Add(item);
                }
            }

            foreach (var item in embryos)
            {
                PrimarchEmbryo primarchEmbryo = (PrimarchEmbryo)item;
                availableEmbryos.Add(primarchEmbryo);
            }

            return availableEmbryos;
        }

        public void SelectEmbryo(PrimarchEmbryo embryo)
        {
            selectedEmbryo = embryo;
            embryo.implantTarget = this;
        }

        public void AddPrimarcEmbryo(Thing primarchEmbryo)
        {
            if (primarchEmbryo.stackCount > 1)
            {
                containedEmbryo = (PrimarchEmbryo)primarchEmbryo.SplitOff(1);
            }
            else
            {
                containedEmbryo = (PrimarchEmbryo)primarchEmbryo;
            }
            haulJobStarted = false;
            primarchEmbryo.Destroy();
        }

        public Vector3 OffsetFromRotation(Rot4 rotation)
        {
            Vector3 vect;
            if (rotation == Rot4.South)
            {
                vect = new Vector3(-0.5f, 0, 0);
            }
            else if (rotation == Rot4.North)
            {
                vect = new Vector3(0.5f, 0, 0);
            }
            else if (rotation == Rot4.West)
            {
                vect = new Vector3(0, 0, 0.5f);
            }
            else
            {
                vect = new Vector3(0, 0, -0.5f);
            }

            return vect;
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);
            if (base.Working && selectedEmbryo != null && innerContainer.Contains(selectedEmbryo))
            {
                Vector2 drawSize = Vector2.one * Mathf.Lerp(FetusMinSize, FetusMaxSize, EmbryoGestationPct);
                Vector3 drawPos1 = DrawPos + PawnDrawOffset + Altitudes.AltIncVect * 0.25f;
                drawPos1 += OffsetFromRotation(base.Rotation);

                if (EmbryoGestationTicksRemaining > EmbryoLateStageGraphicTicksRemaining)
                {
                    FetusEarlyStage.drawSize = drawSize;
                    FetusEarlyStage.DrawFromDef(drawPos1, base.Rotation, null);
                }
                else
                {
                    FetusLateStage.drawSize = drawSize;
                    FetusLateStage.DrawFromDef(drawPos1, base.Rotation, null);
                }
            }
            TopGraphic.drawSize = new Vector2(1, 2);
            Vector3 drawPos2 = DrawPos + Altitudes.AltIncVect * 2f;
            drawPos2 += OffsetFromRotation(base.Rotation);
            TopGraphic.Draw(drawPos2, base.Rotation, this);
        }

        private Color EmbryoColor()
        {
            Color result = PawnSkinColors.GetSkinColor(0.5f);
            if (selectedEmbryo?.GeneSet != null)
            {
                foreach (GeneDef item in selectedEmbryo.GeneSet.GenesListForReading)
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
            return result;
        }

        public override void DrawExtraSelectionOverlays()
        {
            base.DrawExtraSelectionOverlays();
            if (selectedEmbryo != null && selectedEmbryo.Map == base.Map)
            {
                GenDraw.DrawLineBetween(this.TrueCenter(), selectedEmbryo.TrueCenter());
            }
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());
            if (base.Working)
            {
                if (selectedEmbryo != null && innerContainer.Contains(selectedEmbryo))
                {
                    stringBuilder.AppendLineIfNotEmpty().AppendLine("Gestating".Translate() + ": " + selectedEmbryo.Label.CapitalizeFirst());
                    stringBuilder.AppendLineTagged("EmbryoTimeUntilBirth".Translate() + ": " + EmbryoGestationTicksRemaining.ToStringTicksToDays().Colorize(ColoredText.DateTimeColor));
                    stringBuilder.Append("EmbryoBirthQuality".Translate() + ": " + 0.7f.ToStringPercent());
                }
                float biostarvationSeverityPercent = BiostarvationSeverityPercent;
                if (biostarvationSeverityPercent > 0f)
                {
                    string text = ((BiostarvationDailyOffset >= 0f) ? "+" : string.Empty);
                    stringBuilder.AppendLineIfNotEmpty().Append(string.Format("{0}: {1} ({2})", "Biostarvation".Translate(), biostarvationSeverityPercent.ToStringPercent(), "PerDay".Translate(text + BiostarvationDailyOffset.ToStringPercent())));
                }
            }
            stringBuilder.AppendLineIfNotEmpty().Append("Nutrition".Translate()).Append(": ")
                .Append(NutritionStored.ToStringByStyle(ToStringStyle.FloatMaxOne));
            if (base.Working)
            {
                stringBuilder.Append(" (-").Append("PerDay".Translate(NutritionConsumedPerDay.ToString("F1"))).Append(")");
            }
            return stringBuilder.ToString();
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions_NonPawn(Thing selectedThing)
        {
            PrimarchEmbryo embryo;
            if ((embryo = selectedThing as PrimarchEmbryo) == null)
            {
                yield break;
            }
            if (base.SelectedPawn != null)
            {
                yield return new FloatMenuOption("CannotInsertEmbryo".Translate() + ": " + "Occupied".Translate(), null);
            }
            else if (selectedEmbryo == null)
            {
                yield return new FloatMenuOption("ImplantEmbryo".Translate(), delegate
                {
                    SelectEmbryo(embryo);
                }, embryo, Color.white);
            }
            else
            {
                yield return new FloatMenuOption("CannotInsertEmbryo".Translate() + ": " + "EmbryoAlreadyInside".Translate(), null);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref selectedEmbryo, "selectedEmbryo");
            Scribe_References.Look(ref containedEmbryo, "containedEmbryo");
            Scribe_Values.Look(ref embryoStarvation, "embryoStarvation", 0f);
            Scribe_Values.Look(ref containedNutrition, "containedNutrition", 0f);
            Scribe_Values.Look(ref haulJobStarted, "haulJobStarted", false);
            Scribe_Values.Look(ref hasBeenStarted, "hasBeenStarted", false);
            Scribe_Deep.Look(ref allowedNutritionSettings, "allowedNutritionSettings", this);
            Scribe_References.Look(ref jobDoer, "jobDoer");
            if (allowedNutritionSettings == null)
            {
                allowedNutritionSettings = new StorageSettings(this);
                if (def.building.defaultStorageSettings != null)
                {
                    allowedNutritionSettings.CopyFrom(def.building.defaultStorageSettings);
                }
            }
        }

    }
}